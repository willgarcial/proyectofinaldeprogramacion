using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public static class wg_CargaInicial
{
    private static readonly string wg_rutaClientes = "clientes.csv";
    private static readonly string wg_rutaRamos = "ramos.csv";

    public static List<wg_Cliente> wg_CargarClientesASql()
    {
        List<wg_Cliente> wg_lista = new List<wg_Cliente>();
        wg_ClienteSqlRepositorio wg_sqlRepo = new wg_ClienteSqlRepositorio();

        try
        {
            if (!File.Exists(wg_rutaClientes)) return wg_lista;

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

                        wg_Cliente wg_cliente = new wg_Cliente(wg_id, wg_cedula, wg_nombre);
                        wg_lista.Add(wg_cliente);
                        wg_sqlRepo.wg_InsertarClienteSiNoExiste(wg_cliente);
                    }
                }
            }
        }
        catch (Exception wg_ex)
        {
            Console.WriteLine($"[ERROR CARGA CLIENTES]: {wg_ex.Message}");
        }

        return wg_lista;
    }

    public static List<wg_Ramo> wg_CargarRamosASql()
    {
        List<wg_Ramo> wg_lista = new List<wg_Ramo>();
        wg_RamoSqlRepositorio wg_sqlRepo = new wg_RamoSqlRepositorio();

        try
        {
            if (!File.Exists(wg_rutaRamos)) return wg_lista;

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

                        wg_Ramo wg_ramo = new wg_Ramo(wg_id, wg_nombre, wg_tasa, wg_deducible);
                        wg_lista.Add(wg_ramo);
                        wg_sqlRepo.wg_InsertarRamoSiNoExiste(wg_ramo);
                    }
                }
            }
        }
        catch (Exception wg_ex)
        {
            Console.WriteLine($"[ERROR CARGA RAMOS]: {wg_ex.Message}");
        }

        return wg_lista;
    }
}