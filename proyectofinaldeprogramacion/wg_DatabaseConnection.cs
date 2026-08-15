using System;
using System.Data;
using Microsoft.Data.SqlClient;

public class wg_DatabaseConnection
{
    // 1. Instancia estática privada 
    private static wg_DatabaseConnection wg_instancia = null;

    // 2. Cadena de conexión extraída de tu configuración 
    private readonly string wg_cadenaConexion = @"Server=(localdb)\MSSQLLocalDB;Database=proyectofinal;Integrated Security=True;TrustServerCertificate=True;";

    // 3. Constructor privado para evitar que otras clases instancien la conexión libremente
    private wg_DatabaseConnection()
    {
    }

    // 4. Método público para obtener la única instancia de la base de datos
    public static wg_DatabaseConnection wg_GetInstance()
    {
        if (wg_instancia == null)
        {
            wg_instancia = new wg_DatabaseConnection();
        }
        return wg_instancia;
    }

    // 5. Método para operaciones CRUD de lectura 
    public DataTable wg_ExecuteQuery(string wg_query)
    {
        DataTable wg_tablaResultados = new DataTable();

        try
        {
            // El bloque 'using' garantiza el Green IT: cierra la conexión y libera memoria automáticamente
            using (SqlConnection wg_conexion = new SqlConnection(wg_cadenaConexion))
            {
                wg_conexion.Open();
                using (SqlCommand wg_comando = new SqlCommand(wg_query, wg_conexion))
                {
                    using (SqlDataAdapter wg_adaptador = new SqlDataAdapter(wg_comando))
                    {
                        wg_adaptador.Fill(wg_tablaResultados);
                    }
                }
            }
        }
        catch (SqlException wg_ex)
        {
            // Más adelante reemplazaremos este Console.WriteLine por nuestro Log de Auditoría
            Console.WriteLine($"\n[ERROR DE BASE DE DATOS]: No se pudo ejecutar la consulta. Detalle: {wg_ex.Message}");
        }

        return wg_tablaResultados;
    }

    // 6. Método para operaciones CRUD de escritura (INSERT, UPDATE, DELETE)
    public int wg_ExecuteNonQuery(string wg_query)
    {
        int wg_filasAfectadas = 0;

        try
        {
            using (SqlConnection wg_conexion = new SqlConnection(wg_cadenaConexion))
            {
                wg_conexion.Open();
                using (SqlCommand wg_comando = new SqlCommand(wg_query, wg_conexion))
                {
                    wg_filasAfectadas = wg_comando.ExecuteNonQuery();
                }
            }
        }
        catch (SqlException wg_ex)
        {
            Console.WriteLine($"\n[ERROR DE BASE DE DATOS]: No se pudo modificar la información. Detalle: {wg_ex.Message}");
        }

        return wg_filasAfectadas;
    }
}