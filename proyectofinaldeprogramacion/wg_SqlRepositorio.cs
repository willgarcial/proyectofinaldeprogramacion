using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

public class wg_ClienteSqlRepositorio
{
    private wg_DatabaseConnection wg_db = wg_DatabaseConnection.wg_GetInstance();

    public void wg_InsertarClienteSiNoExiste(wg_Cliente wg_cliente)
    {
        string wg_sqlCheck = $"SELECT COUNT(*) FROM Clientes WHERE IdCliente = {wg_cliente.wg_IdCliente}";
        DataTable wg_dt = wg_db.wg_ExecuteQuery(wg_sqlCheck);

        int wg_count = wg_dt.Rows.Count > 0 ? Convert.ToInt32(wg_dt.Rows[0][0]) : 0;

        if (wg_count == 0)
        {
            string wg_sqlInsert = $"INSERT INTO Clientes (IdCliente, Cedula, Nombre) VALUES ({wg_cliente.wg_IdCliente}, '{wg_cliente.wg_Cedula}', '{wg_cliente.wg_Nombre}')";
            wg_db.wg_ExecuteNonQuery(wg_sqlInsert);
        }
    }
}

public class wg_RamoSqlRepositorio
{
    private wg_DatabaseConnection wg_db = wg_DatabaseConnection.wg_GetInstance();

    public void wg_InsertarRamoSiNoExiste(wg_Ramo wg_ramo)
    {
        string wg_sqlCheck = $"SELECT COUNT(*) FROM Ramos WHERE IdRamo = {wg_ramo.wg_IdRamo}";
        DataTable wg_dt = wg_db.wg_ExecuteQuery(wg_sqlCheck);

        int wg_count = wg_dt.Rows.Count > 0 ? Convert.ToInt32(wg_dt.Rows[0][0]) : 0;

        if (wg_count == 0)
        {
            string wg_tasaStr = wg_ramo.wg_TasaPrima.ToString(CultureInfo.InvariantCulture);
            string wg_deducibleStr = wg_ramo.wg_PorcentajeDeducible.ToString(CultureInfo.InvariantCulture);

            string wg_sqlInsert = $"INSERT INTO Ramos (IdRamo, Nombre, TasaPrima, PorcentajeDeducible) VALUES ({wg_ramo.wg_IdRamo}, '{wg_ramo.wg_Nombre}', {wg_tasaStr}, {wg_deducibleStr})";
            wg_db.wg_ExecuteNonQuery(wg_sqlInsert);
        }
    }
}

public class wg_PolizaSqlRepositorio
{
    private wg_DatabaseConnection wg_db = wg_DatabaseConnection.wg_GetInstance();

    public bool wg_InsertarPoliza(wg_Poliza wg_poliza)
    {
        string wg_capitalAsegurado = wg_poliza.wg_CapitalAsegurado.ToString(CultureInfo.InvariantCulture);
        string wg_capitalRemanente = wg_poliza.wg_CapitalRemanente.ToString(CultureInfo.InvariantCulture);
        string wg_primaTotal = wg_poliza.wg_PrimaTotal.ToString(CultureInfo.InvariantCulture);

        string wg_sqlInsert = $@"
            INSERT INTO Poliza (IdPoliza, IdCliente, IdRamo, CapitalAsegurado, CapitalRemanente, PrimaTotal, Estado)
            VALUES ({wg_poliza.wg_IdPoliza}, {wg_poliza.wg_IdCliente}, {wg_poliza.wg_IdRamo}, {wg_capitalAsegurado}, {wg_capitalRemanente}, {wg_primaTotal}, '{wg_poliza.wg_Estado}')";
        return wg_db.wg_ExecuteNonQuery(wg_sqlInsert) > 0;
    }

    public List<wg_Poliza> wg_ObtenerTodasLasPolizas()
    {
        List<wg_Poliza> wg_lista = new List<wg_Poliza>();
        DataTable wg_tabla = wg_db.wg_ExecuteQuery("SELECT * FROM Poliza");

        foreach (DataRow wg_fila in wg_tabla.Rows)
        {
            wg_lista.Add(new wg_Poliza(
                Convert.ToInt32(wg_fila["IdPoliza"]),
                Convert.ToInt32(wg_fila["IdCliente"]),
                Convert.ToInt32(wg_fila["IdRamo"]),
                Convert.ToDouble(wg_fila["CapitalAsegurado"]),
                Convert.ToDouble(wg_fila["CapitalRemanente"]),
                Convert.ToDouble(wg_fila["PrimaTotal"]),
                wg_fila["Estado"].ToString()
            ));
        }
        return wg_lista;
    }

    public bool wg_ActualizarPolizaCompleta(wg_Poliza wg_poliza)
    {
        string wg_capitalAsegurado = wg_poliza.wg_CapitalAsegurado.ToString(CultureInfo.InvariantCulture);
        string wg_capitalRemanente = wg_poliza.wg_CapitalRemanente.ToString(CultureInfo.InvariantCulture);
        string wg_primaTotal = wg_poliza.wg_PrimaTotal.ToString(CultureInfo.InvariantCulture);

        string wg_sql = $@"
            UPDATE Poliza 
            SET IdCliente = {wg_poliza.wg_IdCliente}, 
                IdRamo = {wg_poliza.wg_IdRamo}, 
                CapitalAsegurado = {wg_capitalAsegurado}, 
                CapitalRemanente = {wg_capitalRemanente}, 
                PrimaTotal = {wg_primaTotal} 
            WHERE IdPoliza = {wg_poliza.wg_IdPoliza}";
        return wg_db.wg_ExecuteNonQuery(wg_sql) > 0;
    }

    public bool wg_ActualizarRemanentePoliza(int wg_idPoliza, double wg_nuevoRemanente)
    {
        string wg_remanenteStr = wg_nuevoRemanente.ToString(CultureInfo.InvariantCulture);
        string wg_sqlUpdate = $"UPDATE Poliza SET CapitalRemanente = {wg_remanenteStr} WHERE IdPoliza = {wg_idPoliza}";
        return wg_db.wg_ExecuteNonQuery(wg_sqlUpdate) > 0;
    }

    public bool wg_EliminarPoliza(int wg_idPoliza)
    {
        return wg_db.wg_ExecuteNonQuery($"DELETE FROM Poliza WHERE IdPoliza = {wg_idPoliza}") > 0;
    }
}

public class wg_SiniestroSqlRepositorio
{
    private wg_DatabaseConnection wg_db = wg_DatabaseConnection.wg_GetInstance();

    public bool wg_InsertarSiniestro(wg_Siniestro wg_siniestro)
    {
        string wg_monto = wg_siniestro.wg_MontoReclamo.ToString(CultureInfo.InvariantCulture);
        string wg_neto = wg_siniestro.wg_PagoNeto.ToString(CultureInfo.InvariantCulture);

        string wg_sql = $"INSERT INTO Siniestros VALUES ({wg_siniestro.wg_IdSiniestro}, {wg_siniestro.wg_IdPoliza}, {wg_monto}, {wg_neto}, '{wg_siniestro.wg_Estado}')";
        return wg_db.wg_ExecuteNonQuery(wg_sql) > 0;
    }

    public List<wg_Siniestro> wg_ObtenerTodos()
    {
        List<wg_Siniestro> wg_lista = new List<wg_Siniestro>();
        DataTable wg_tabla = wg_db.wg_ExecuteQuery("SELECT * FROM Siniestros");

        foreach (DataRow wg_fila in wg_tabla.Rows)
        {
            wg_lista.Add(new wg_Siniestro(
                Convert.ToInt32(wg_fila["IdSiniestro"]),
                Convert.ToInt32(wg_fila["IdPoliza"]),
                Convert.ToDouble(wg_fila["MontoReclamo"]),
                Convert.ToDouble(wg_fila["PagoNeto"]),
                wg_fila["Estado"].ToString()
            ));
        }
        return wg_lista;
    }

    public bool wg_ActualizarSiniestro(wg_Siniestro wg_siniestro)
    {
        string wg_monto = wg_siniestro.wg_MontoReclamo.ToString(CultureInfo.InvariantCulture);
        string wg_neto = wg_siniestro.wg_PagoNeto.ToString(CultureInfo.InvariantCulture);

        string wg_sql = $"UPDATE Siniestros SET MontoReclamo = {wg_monto}, PagoNeto = {wg_neto} WHERE IdSiniestro = {wg_siniestro.wg_IdSiniestro}";
        return wg_db.wg_ExecuteNonQuery(wg_sql) > 0;
    }

    public bool wg_EliminarSiniestro(int wg_idSiniestro)
    {
        return wg_db.wg_ExecuteNonQuery($"DELETE FROM Siniestros WHERE IdSiniestro = {wg_idSiniestro}") > 0;
    }

    public List<wg_Siniestro> wg_FiltrarSiniestrosPorCliente(int wg_idCliente)
    {
        List<wg_Siniestro> wg_lista = new List<wg_Siniestro>();
        string wg_sqlSelect = $@"
            SELECT s.IdSiniestro, s.IdPoliza, s.MontoReclamo, s.PagoNeto, s.Estado 
            FROM Siniestros s
            INNER JOIN Poliza p ON s.IdPoliza = p.IdPoliza
            WHERE p.IdCliente = {wg_idCliente}";

        DataTable wg_tabla = wg_db.wg_ExecuteQuery(wg_sqlSelect);

        foreach (DataRow wg_fila in wg_tabla.Rows)
        {
            wg_lista.Add(new wg_Siniestro(
                Convert.ToInt32(wg_fila["IdSiniestro"]),
                Convert.ToInt32(wg_fila["IdPoliza"]),
                Convert.ToDouble(wg_fila["MontoReclamo"]),
                Convert.ToDouble(wg_fila["PagoNeto"]),
                wg_fila["Estado"].ToString()
            ));
        }
        return wg_lista;
    }
}

public class wg_ReaseguroSqlRepositorio
{
    private wg_DatabaseConnection wg_db = wg_DatabaseConnection.wg_GetInstance();

    public void wg_InsertarReaseguro(wg_Reaseguro wg_rea)
    {
        string wg_ret = wg_rea.wg_MontoRetencion.ToString(CultureInfo.InvariantCulture);
        string wg_con = wg_rea.wg_MontoContrato.ToString(CultureInfo.InvariantCulture);
        string wg_fac = wg_rea.wg_MontoFacultativo.ToString(CultureInfo.InvariantCulture);

        wg_db.wg_ExecuteNonQuery($"INSERT INTO Reaseguros VALUES ({wg_rea.wg_IdReaseguro}, {wg_rea.wg_IdPoliza}, {wg_ret}, {wg_con}, {wg_fac})");
    }

    public List<wg_Reaseguro> wg_ObtenerTodos()
    {
        List<wg_Reaseguro> wg_lista = new List<wg_Reaseguro>();
        DataTable wg_tabla = wg_db.wg_ExecuteQuery("SELECT * FROM Reaseguros");

        foreach (DataRow wg_fila in wg_tabla.Rows)
        {
            wg_lista.Add(new wg_Reaseguro(
                Convert.ToInt32(wg_fila["IdReaseguro"]),
                Convert.ToInt32(wg_fila["IdPoliza"]),
                Convert.ToDouble(wg_fila["MontoRetencion"]),
                Convert.ToDouble(wg_fila["MontoContrato"]),
                Convert.ToDouble(wg_fila["MontoFacultativo"])
            ));
        }
        return wg_lista;
    }

    public void wg_EliminarReaseguroPorPoliza(int wg_idPoliza)
    {
        wg_db.wg_ExecuteNonQuery($"DELETE FROM Reaseguros WHERE IdPoliza = {wg_idPoliza}");
    }
}

public class wg_AsientoSqlRepositorio
{
    private wg_DatabaseConnection wg_db = wg_DatabaseConnection.wg_GetInstance();

    public void wg_InsertarAsiento(wg_AsientoContable wg_asiento)
    {
        string wg_valor = wg_asiento.wg_Valor.ToString(CultureInfo.InvariantCulture);
        wg_db.wg_ExecuteNonQuery($"INSERT INTO AsientosContables VALUES ({wg_asiento.wg_IdAsiento}, '{wg_asiento.wg_TipoOperacion}', '{wg_asiento.wg_CuentaDebe}', '{wg_asiento.wg_CuentaHaber}', {wg_valor})");
    }

    public List<wg_AsientoContable> wg_ObtenerTodos()
    {
        List<wg_AsientoContable> wg_lista = new List<wg_AsientoContable>();
        DataTable wg_tabla = wg_db.wg_ExecuteQuery("SELECT * FROM AsientosContables");

        foreach (DataRow wg_fila in wg_tabla.Rows)
        {
            wg_lista.Add(new wg_AsientoContable(
                Convert.ToInt32(wg_fila["IdAsiento"]),
                wg_fila["TipoOperacion"].ToString(),
                wg_fila["CuentaDebe"].ToString(),
                wg_fila["CuentaHaber"].ToString(),
                Convert.ToDouble(wg_fila["Valor"])
            ));
        }
        return wg_lista;
    }
}