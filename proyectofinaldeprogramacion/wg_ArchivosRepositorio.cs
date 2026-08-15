using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class wg_ArchivosRepositorio
{
    private readonly string wg_rutaClientes = "clientes.csv";
    private readonly string wg_rutaRamos = "ramos.csv";
    private readonly string wg_rutaAuditoria = "auditoria.txt";

    // Carga inicial de clientes a la memoria RAM
    public List<wg_Cliente> wg_CargarClientes()
    {
        List<wg_Cliente> wg_listaClientes = new List<wg_Cliente>();

        try
        {
            if (!File.Exists(wg_rutaClientes))
                throw new wg_ArchivoDatosException($"El catálogo {wg_rutaClientes} no existe en el disco.");

            // Uso estricto de using  para liberar el archivo y la RAM
            using (StreamReader wg_lector = new StreamReader(wg_rutaClientes))
            {
                string wg_linea;
                while ((wg_linea = wg_lector.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(wg_linea)) continue;

                    string[] wg_datos = wg_linea.Split(';');
                    if (wg_datos.Length >= 3)
                    {
                        int wg_id = int.Parse(wg_datos[0]);
                        string wg_cedula = wg_datos[1];
                        string wg_nombre = wg_datos[2];

                        wg_listaClientes.Add(new wg_Cliente(wg_id, wg_cedula, wg_nombre));
                    }
                }
            }
        }
        catch (Exception wg_ex)
        {
            throw new wg_ArchivoDatosException("Fallo crítico al cargar el CSV de clientes.", wg_ex);
        }

        return wg_listaClientes;
    }

    // Carga inicial de ramos a la memoria RAM
    public List<wg_Ramo> wg_CargarRamos()
    {
        List<wg_Ramo> wg_listaRamos = new List<wg_Ramo>();

        try
        {
            if (!File.Exists(wg_rutaRamos))
                throw new wg_ArchivoDatosException($"El catálogo {wg_rutaRamos} no existe en el disco.");

            using (StreamReader wg_lector = new StreamReader(wg_rutaRamos))
            {
                string wg_linea;
                while ((wg_linea = wg_lector.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(wg_linea)) continue;

                    string[] wg_datos = wg_linea.Split(';');
                    if (wg_datos.Length >= 4)
                    {
                        int wg_id = int.Parse(wg_datos[0]);
                        string wg_nombre = wg_datos[1];
                        double wg_tasa = double.Parse(wg_datos[2], CultureInfo.InvariantCulture);
                        double wg_deducible = double.Parse(wg_datos[3], CultureInfo.InvariantCulture);

                        wg_listaRamos.Add(new wg_Ramo(wg_id, wg_nombre, wg_tasa, wg_deducible));
                    }
                }
            }
        }
        catch (Exception wg_ex)
        {
            throw new wg_ArchivoDatosException("Fallo crítico al cargar el CSV de ramos.", wg_ex);
        }

        return wg_listaRamos;
    }

    // Escritura de auditoría 
    public void wg_GuardarLogAuditoria(wg_LogSistema wg_log)
    {
        try
        {
            using (StreamWriter wg_escritor = new StreamWriter(wg_rutaAuditoria, true))
            {
                string wg_linea = $"{wg_log.wg_Fecha:yyyy-MM-dd HH:mm:ss} | Módulo: {wg_log.wg_Modulo} | Mensaje: {wg_log.wg_Mensaje}";
                wg_escritor.WriteLine(wg_linea);
            }
        }
        catch (Exception wg_ex)
        {
            throw new wg_ArchivoDatosException("No se pudo escribir en el archivo de auditoría.", wg_ex);
        }
    }
}