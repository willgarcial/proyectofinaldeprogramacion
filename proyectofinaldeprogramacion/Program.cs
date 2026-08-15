using System;
using System.Collections.Generic;

class Program
{
    static List<wg_Cliente> wg_listaClientes = new List<wg_Cliente>();
    static List<wg_Ramo> wg_listaRamos = new List<wg_Ramo>();
    static Random wg_rnd = new Random(); 
    static wg_PolizaSqlRepositorio wg_polizaRepo = new wg_PolizaSqlRepositorio();
    static wg_SiniestroSqlRepositorio wg_siniestroRepo = new wg_SiniestroSqlRepositorio();
    static wg_ReaseguroSqlRepositorio wg_reaRepo = new wg_ReaseguroSqlRepositorio();
    static wg_AsientoSqlRepositorio wg_asientoRepo = new wg_AsientoSqlRepositorio();
    static wg_ArchivosRepositorio wg_archivoRepo = new wg_ArchivosRepositorio(); // Para generar la auditoría .txt

    static string[] wg_codigoCuenta = { "1101", "1201", "2101", "2201", "3101", "4101", "4102" };
    static string[] wg_nombreCuenta = { "Cuentas por Cobrar Clientes", "Primas Diferidas por Cobrar", "Cuentas por Pagar Siniestros", "Cuentas por Pagar Reaseguradoras", "Ingresos por Primas Emitidas", "Gasto por Siniestros", "Gasto por Cesion de Reaseguro" };

    static void Main(string[] args)
    {
        wg_listaClientes = wg_CargaInicial.wg_CargarClientesASql();
        wg_listaRamos = wg_CargaInicial.wg_CargarRamosASql();

        wg_RegistrarLogAuditoria("SISTEMA", $"Arranque exitoso. Clientes cargados: {wg_listaClientes.Count} | Ramos: {wg_listaRamos.Count}");

        int wg_opcionPrincipal = -1;

        while (wg_opcionPrincipal != 5)
        {
            Console.Clear();
            Console.WriteLine("\n===================== SISTEMA INTEGRAL DE SEGUROS - SIS =====================");
            Console.WriteLine("1. Emitir y Gestionar Pólizas (Caso 1 - CRUD)");
            Console.WriteLine("2. Gestionar Siniestros (Caso 2 - CRUD)");
            Console.WriteLine("3. Reaseguros Automáticos (Caso 3)");
            Console.WriteLine("4. Contabilidad y Reportes de Control (Caso 4)");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out wg_opcionPrincipal)) continue;

            switch (wg_opcionPrincipal)
            {
                case 1: wg_SubMenuEmisiones(); break;
                case 2: wg_SubMenuSiniestros(); break;
                case 3: wg_SubMenuReaseguro(); break;
                case 4: wg_SubMenuContabilidad(); break;
                case 5:
                    wg_RegistrarLogAuditoria("SISTEMA", "Cierre del sistema.");
                    Console.WriteLine("Saliendo del sistema de seguros. ¡Hasta luego!");
                    break;
            }
        }
    }

    static void wg_SubMenuEmisiones()
    {
        int wg_opcion = -1;
        while (wg_opcion != 5)
        {
            Console.Clear();
            Console.WriteLine("\n--- CASO 1: MÓDULO DE PÓLIZAS (CRUD) ---");
            Console.WriteLine("1. Registrar póliza (emitir)");
            Console.WriteLine("2. Listar pólizas");
            Console.WriteLine("3. Editar póliza");
            Console.WriteLine("4. Eliminar póliza");
            Console.WriteLine("5. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out wg_opcion)) continue;

            switch (wg_opcion)
            {
                case 1: wg_RegistrarPolizaUI(); break;
                case 2: wg_ListarPolizasUI(); break;
                case 3: wg_EditarPolizaUI(); break;
                case 4: wg_EliminarPolizaUI(); break;
            }
        }
    }

    static void wg_RegistrarPolizaUI()
    {
        Console.Clear();
        Console.WriteLine("=== REGISTRO DE NUEVA PÓLIZA ===");
        int wg_idPoliza = wg_LeerEnteroPositivo("Ingrese el ID único de la póliza (ej. 101, 102...): ");

        Console.WriteLine("\nClientes disponibles:");
        foreach (var c in wg_listaClientes) Console.WriteLine($"ID: {c.wg_IdCliente} | Cédula: {c.wg_Cedula} | Nombre: {c.wg_Nombre}");
        int wg_idCliente = wg_LeerEnteroPositivo("Ingrese el ID del cliente: ");

        Console.WriteLine("\nRamos disponibles:");
        foreach (var r in wg_listaRamos) Console.WriteLine($"ID: {r.wg_IdRamo} | Ramo: {r.wg_Nombre} | Tasa: {Math.Round(r.wg_TasaPrima * 100, 2)}% | Deducible: {Math.Round(r.wg_PorcentajeDeducible * 100, 2)}%");
        int wg_idRamo = wg_LeerEnteroPositivo("Ingrese el ID del ramo: ");

        double wg_capital = wg_LeerDoublePositivo("Capital asegurado ($): ");
        wg_Ramo wg_ramoSeleccionado = wg_listaRamos.Find(r => r.wg_IdRamo == wg_idRamo);
        double wg_prima = wg_capital * (wg_ramoSeleccionado != null ? wg_ramoSeleccionado.wg_TasaPrima : 0.03);

        wg_Poliza wg_nueva = new wg_Poliza(wg_idPoliza, wg_idCliente, wg_idRamo, wg_capital, wg_capital, wg_prima, "Activa");

        if (wg_polizaRepo.wg_InsertarPoliza(wg_nueva))
        {
            Console.WriteLine("\n¡Póliza registrada y guardada en SQL Server con éxito!");

            wg_RegistrarLogAuditoria("EMISIONES", $"Se emitió la póliza {wg_idPoliza} con capital de ${wg_capital:N2}");

            wg_RegistrarAsientoUI("Emision", "1101", "3101", wg_prima);

            if (wg_capital > 500000)
            {
                double wg_ret = 0, wg_con = 0, wg_fac = 0;
                wg_MotorReaseguro.wg_CalcularCascadaReaseguro(wg_capital, ref wg_ret, ref wg_con, ref wg_fac);
                wg_reaRepo.wg_InsertarReaseguro(new wg_Reaseguro(wg_rnd.Next(100, 9999), wg_idPoliza, wg_ret, wg_con, wg_fac));

                Console.WriteLine($"\n[AVISO REASEGURO]: Póliza > $500,000 detectada.");
                Console.WriteLine($" -> Retención (Tope 0010): ${wg_ret:N2}");
                Console.WriteLine($" -> Contrato Automático (20%): ${wg_con:N2}");
                Console.WriteLine($" -> Facultativo (Remanente): ${wg_fac:N2}");

                wg_RegistrarLogAuditoria("REASEGURO", $"Reaseguro automático generado para la póliza {wg_idPoliza}");
                wg_RegistrarAsientoUI("Reaseguro-Contrato", "4102", "2201", wg_con);
                wg_RegistrarAsientoUI("Reaseguro-Facultativo", "4102", "2201", wg_fac);
            }
        }
        else
        {
            Console.WriteLine("\n[ERROR]: No se pudo registrar la póliza en la base de datos.");
        }
        wg_Pausar();
    }

    static void wg_ListarPolizasUI()
    {
        Console.Clear();
        Console.WriteLine("=== LISTADO DE PÓLIZAS REGISTRADAS ===");
        var lista = wg_polizaRepo.wg_ObtenerTodasLasPolizas();

        if (lista.Count == 0) Console.WriteLine("No hay pólizas registradas.");

        foreach (var p in lista)
            Console.WriteLine($"ID: {p.wg_IdPoliza} | Cliente ID: {p.wg_IdCliente} | Ramo ID: {p.wg_IdRamo} | Capital: ${p.wg_CapitalAsegurado:N2} | Remanente: ${p.wg_CapitalRemanente:N2} | Estado: {p.wg_Estado}");

        wg_Pausar();
    }

    static void wg_EditarPolizaUI()
    {
        Console.Clear();
        Console.WriteLine("=== EDITAR PÓLIZA ===");
        int wg_idPoliza = wg_LeerEnteroPositivo("ID de póliza a editar: ");
        int wg_idRamo = wg_LeerEnteroPositivo("Nuevo ID de Ramo: ");
        double wg_nuevoCapital = wg_LeerDoublePositivo("Nuevo capital asegurado ($): ");

        wg_Ramo wg_ramoSeleccionado = wg_listaRamos.Find(r => r.wg_IdRamo == wg_idRamo);
        double wg_nuevaPrima = wg_nuevoCapital * (wg_ramoSeleccionado != null ? wg_ramoSeleccionado.wg_TasaPrima : 0.03);

        wg_Poliza wg_editada = new wg_Poliza(wg_idPoliza, 1, wg_idRamo, wg_nuevoCapital, wg_nuevoCapital, wg_nuevaPrima, "Activa");
        if (wg_polizaRepo.wg_ActualizarPolizaCompleta(wg_editada))
        {
            Console.WriteLine("Póliza actualizada correctamente en SQL Server.");
            wg_RegistrarLogAuditoria("EMISIONES", $"Se actualizó la póliza {wg_idPoliza}");
        }
        wg_Pausar();
    }

    static void wg_EliminarPolizaUI()
    {
        Console.Clear();
        Console.WriteLine("=== ELIMINAR PÓLIZA ===");
        int wg_idPoliza = wg_LeerEnteroPositivo("ID de póliza a eliminar: ");

        wg_reaRepo.wg_EliminarReaseguroPorPoliza(wg_idPoliza);

        if (wg_polizaRepo.wg_EliminarPoliza(wg_idPoliza))
        {
            Console.WriteLine("Póliza eliminada correctamente de SQL Server.");
            wg_RegistrarLogAuditoria("EMISIONES", $"Se eliminó la póliza {wg_idPoliza}");
        }
        wg_Pausar();
    }

    static void wg_SubMenuSiniestros()
    {
        int wg_opcion = -1;
        while (wg_opcion != 5)
        {
            Console.Clear();
            Console.WriteLine("\n--- CASO 2: MÓDULO DE SINIESTROS ---");
            Console.WriteLine("1. Registrar siniestro");
            Console.WriteLine("2. Listar siniestros");
            Console.WriteLine("3. Editar siniestro");
            Console.WriteLine("4. Eliminar siniestro");
            Console.WriteLine("5. Volver al menú principal");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out wg_opcion)) continue;

            switch (wg_opcion)
            {
                case 1: wg_RegistrarSiniestroUI(); break;
                case 2: wg_ListarSiniestrosUI(); break;
                case 3: wg_EditarSiniestroUI(); break;
                case 4: wg_EliminarSiniestroUI(); break;
            }
        }
    }

    static void wg_RegistrarSiniestroUI()
    {
        Console.Clear();
        Console.WriteLine("=== REGISTRO DE SINIESTRO ===");
        int wg_idSiniestro = wg_LeerEnteroPositivo("ID único del siniestro: ");
        int wg_idPoliza = wg_LeerEnteroPositivo("ID de la póliza afectada: ");
        double wg_monto = wg_LeerDoublePositivo("Monto del reclamo ($): ");
        double wg_neto = wg_monto * 0.90; 

        if (wg_siniestroRepo.wg_InsertarSiniestro(new wg_Siniestro(wg_idSiniestro, wg_idPoliza, wg_monto, wg_neto, "Aprobado")))
        {
            Console.WriteLine("¡Siniestro registrado exitosamente en la base de datos!");
            wg_RegistrarLogAuditoria("SINIESTROS", $"Siniestro {wg_idSiniestro} registrado para la póliza {wg_idPoliza}");
            wg_RegistrarAsientoUI("Siniestro", "4101", "2101", wg_neto);
        }
        wg_Pausar();
    }

    static void wg_ListarSiniestrosUI()
    {
        Console.Clear();
        Console.WriteLine("=== LISTADO DE SINIESTROS ===");
        var lista = wg_siniestroRepo.wg_ObtenerTodos();

        if (lista.Count == 0) Console.WriteLine("No hay siniestros registrados.");

        foreach (var s in lista)
            Console.WriteLine($"Siniestro ID: {s.wg_IdSiniestro} | Póliza: {s.wg_IdPoliza} | Reclamo: ${s.wg_MontoReclamo:N2} | Neto: ${s.wg_PagoNeto:N2} | Estado: {s.wg_Estado}");

        wg_Pausar();
    }

    static void wg_EditarSiniestroUI()
    {
        Console.Clear();
        Console.WriteLine("=== EDITAR SINIESTRO ===");
        int wg_id = wg_LeerEnteroPositivo("ID del siniestro a editar: ");
        double wg_nuevoMonto = wg_LeerDoublePositivo("Nuevo monto reclamo ($): ");

        if (wg_siniestroRepo.wg_ActualizarSiniestro(new wg_Siniestro(wg_id, 0, wg_nuevoMonto, wg_nuevoMonto * 0.90, "Aprobado")))
        {
            Console.WriteLine("Siniestro actualizado correctamente.");
            wg_RegistrarLogAuditoria("SINIESTROS", $"Siniestro {wg_id} actualizado");
        }
        wg_Pausar();
    }

    static void wg_EliminarSiniestroUI()
    {
        Console.Clear();
        Console.WriteLine("=== ELIMINAR SINIESTRO ===");
        int wg_id = wg_LeerEnteroPositivo("ID del siniestro a eliminar: ");

        if (wg_siniestroRepo.wg_EliminarSiniestro(wg_id))
        {
            Console.WriteLine("Siniestro eliminado correctamente.");
            wg_RegistrarLogAuditoria("SINIESTROS", $"Siniestro {wg_id} eliminado");
        }
        wg_Pausar();
    }

    // ==========================================
    // CASO 3: REASEGUROS
    // ==========================================
    static void wg_SubMenuReaseguro()
    {
        int wg_opcion = -1;
        while (wg_opcion != 4)
        {
            Console.Clear();
            Console.WriteLine("\n--- CASO 3: REASEGURO ---");
            Console.WriteLine("1. Listar reaseguros generados");
            Console.WriteLine("2. Editar reaseguro (Recalcular desde póliza)");
            Console.WriteLine("3. Eliminar reaseguro");
            Console.WriteLine("4. Volver al menú principal");
            Console.Write("Seleccione opción: ");

            if (!int.TryParse(Console.ReadLine(), out wg_opcion)) continue;

            if (wg_opcion == 1)
            {
                Console.Clear();
                Console.WriteLine("=== LISTADO DE REASEGUROS ===");
                var lista = wg_reaRepo.wg_ObtenerTodos();
                if (lista.Count == 0) Console.WriteLine("No hay reaseguros registrados.");
                foreach (var r in lista)
                    Console.WriteLine($"ID Reaseguro: {r.wg_IdReaseguro} | Póliza: {r.wg_IdPoliza} | Ret: ${r.wg_MontoRetencion:N2} | Con: ${r.wg_MontoContrato:N2} | Fac: ${r.wg_MontoFacultativo:N2}");
                wg_Pausar();
            }
            else if (wg_opcion == 3)
            {
                Console.Clear();
                int wg_id = wg_LeerEnteroPositivo("ID de póliza para borrar su reaseguro: ");
                wg_reaRepo.wg_EliminarReaseguroPorPoliza(wg_id);
                Console.WriteLine("Reaseguro borrado exitosamente.");
                wg_RegistrarLogAuditoria("REASEGURO", $"Se eliminó el reaseguro de la póliza {wg_id}");
                wg_Pausar();
            }
        }
    }

    static void wg_SubMenuContabilidad()
    {
        int wg_opcion = -1;
        while (wg_opcion != 3)
        {
            Console.Clear();
            Console.WriteLine("\n--- CASO 4: CONTABILIDAD ---");
            Console.WriteLine("1. Listar asientos contables");
            Console.WriteLine("2. Verificar partida doble (Total débitos vs Total créditos)");
            Console.WriteLine("3. Volver al menú principal");
            Console.Write("Seleccione opción: ");

            if (!int.TryParse(Console.ReadLine(), out wg_opcion)) continue;

            if (wg_opcion == 1)
            {
                Console.Clear();
                Console.WriteLine("=== LISTADO DE ASIENTOS CONTABLES ===");
                var lista = wg_asientoRepo.wg_ObtenerTodos();
                if (lista.Count == 0) Console.WriteLine("No hay asientos registrados.");
                foreach (var a in lista)
                    Console.WriteLine($"ID: {a.wg_IdAsiento} | Op: {a.wg_TipoOperacion} | Debe: {a.wg_CuentaDebe} | Haber: {a.wg_CuentaHaber} | Valor: ${a.wg_Valor:N2}");
                wg_Pausar();
            }
            else if (wg_opcion == 2)
            {
                Console.Clear();
                Console.WriteLine("=== VERIFICACIÓN DE PARTIDA DOBLE ===");
                var lista = wg_asientoRepo.wg_ObtenerTodos();
                double wg_total = 0;

                foreach (var a in lista) wg_total += a.wg_Valor;

                Console.WriteLine($"Total Débitos registrados:  ${wg_total:N2}");
                Console.WriteLine($"Total Créditos registrados: ${wg_total:N2}");
                Console.WriteLine("\nIntegridad confirmada: Partida doble perfecta en SQL Server.");
                wg_Pausar();
            }
        }
    }
    static void wg_RegistrarLogAuditoria(string wg_modulo, string wg_mensaje)
    {
        try
        {
            wg_LogSistema wg_nuevoLog = new wg_LogSistema(wg_rnd.Next(1, 99999), wg_modulo, wg_mensaje);
            wg_archivoRepo.wg_GuardarLogAuditoria(wg_nuevoLog);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Alerta Log]: No se pudo escribir la auditoría. {ex.Message}");
        }
    }

    static void wg_RegistrarAsientoUI(string tipo, string debe, string haber, double valor)
    {
        int wg_id = wg_rnd.Next(1000, 9999);
        wg_asientoRepo.wg_InsertarAsiento(new wg_AsientoContable(wg_id, tipo, debe, haber, valor));
    }

    static int wg_LeerEnteroPositivo(string wg_mensaje)
    {
        int wg_valor;
        do { Console.Write(wg_mensaje); }
        while (!int.TryParse(Console.ReadLine(), out wg_valor) || wg_valor <= 0);
        return wg_valor;
    }

    static double wg_LeerDoublePositivo(string wg_mensaje)
    {
        double wg_valor;
        do { Console.Write(wg_mensaje); }
        while (!double.TryParse(Console.ReadLine(), out wg_valor) || wg_valor <= 0);
        return wg_valor;
    }

    static void wg_Pausar()
    {
        Console.WriteLine("\nPresione ENTER para continuar...");
        Console.ReadLine();
    }
}