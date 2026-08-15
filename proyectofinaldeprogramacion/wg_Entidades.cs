using System;

//  Entidad Cliente
public class wg_Cliente
{
    private int wg_idCliente;
    private string wg_cedula;
    private string wg_nombre;

    public int wg_IdCliente
    {
        get { return wg_idCliente; }
        set { wg_idCliente = value; }
    }

    public string wg_Cedula
    {
        get { return wg_cedula; }
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 10)
                throw new wg_ClienteInvalidoException("La cédula debe tener exactamente 10 caracteres.");

            foreach (char c in value)
            {
                if (c < '0' || c > '9')
                    throw new wg_ClienteInvalidoException("La cédula solo puede contener números.");
            }
            wg_cedula = value;
        }
    }

    public string wg_Nombre
    {
        get { return wg_nombre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new wg_ClienteInvalidoException("El nombre del cliente no puede estar vacío.");
            wg_nombre = value.Trim();
        }
    }

    public wg_Cliente(int id, string cedula, string nombre)
    {
        wg_IdCliente = id;
        wg_Cedula = cedula;
        wg_Nombre = nombre;
    }
}

//  Entidad Ramo
public class wg_Ramo
{
    private int wg_idRamo;
    private string wg_nombre;
    private double wg_tasaPrima;
    private double wg_porcentajeDeducible;

    public int wg_IdRamo { get { return wg_idRamo; } set { wg_idRamo = value; } }

    public string wg_Nombre
    {
        get { return wg_nombre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del ramo no puede estar vacío.");
            wg_nombre = value;
        }
    }

    public double wg_TasaPrima
    {
        get { return wg_tasaPrima; }
        set
        {
            if (value < 0 || value > 1)
                throw new ArgumentException("La tasa de prima debe ser un porcentaje entre 0 y 1.");
            wg_tasaPrima = value;
        }
    }

    public double wg_PorcentajeDeducible
    {
        get { return wg_porcentajeDeducible; }
        set
        {
            if (value < 0 || value > 1)
                throw new ArgumentException("El deducible debe ser un porcentaje entre 0 y 1.");
            wg_porcentajeDeducible = value;
        }
    }

    public wg_Ramo(int id, string nombre, double tasa, double deducible)
    {
        wg_IdRamo = id;
        wg_Nombre = nombre;
        wg_TasaPrima = tasa;
        wg_PorcentajeDeducible = deducible;
    }
}

//  Entidad Póliza
public class wg_Poliza
{
    private int wg_idPoliza;
    private int wg_idCliente;
    private int wg_idRamo;
    private double wg_capitalAsegurado;
    private double wg_capitalRemanente;
    private double wg_primaTotal;
    private string wg_estado;

    public int wg_IdPoliza { get { return wg_idPoliza; } set { wg_idPoliza = value; } }
    public int wg_IdCliente { get { return wg_idCliente; } set { wg_idCliente = value; } }
    public int wg_IdRamo { get { return wg_idRamo; } set { wg_idRamo = value; } }

    public double wg_CapitalAsegurado
    {
        get { return wg_capitalAsegurado; }
        set
        {
            if (value <= 0)
                throw new wg_PolizaInvalidaException("El capital asegurado debe ser mayor a cero.");
            wg_capitalAsegurado = value;
        }
    }

    public double wg_CapitalRemanente
    {
        get { return wg_capitalRemanente; }
        set
        {
            if (value < 0)
                throw new wg_PolizaInvalidaException("El capital remanente no puede ser negativo.");
            if (value > wg_capitalAsegurado)
                throw new wg_PolizaInvalidaException("El capital remanente no puede superar al capital asegurado.");
            wg_capitalRemanente = value;
        }
    }

    public double wg_PrimaTotal
    {
        get { return wg_primaTotal; }
        set
        {
            if (value < 0)
                throw new wg_PolizaInvalidaException("La prima no puede ser negativa.");
            wg_primaTotal = value;
        }
    }

    public string wg_Estado { get { return wg_estado; } set { wg_estado = value; } }

    public wg_Poliza(int id, int idCliente, int idRamo, double capital, double remanente, double prima, string estado)
    {
        wg_IdPoliza = id;
        wg_IdCliente = idCliente;
        wg_IdRamo = idRamo;
        wg_CapitalAsegurado = capital;
        wg_CapitalRemanente = remanente;
        wg_PrimaTotal = prima;
        wg_Estado = estado;
    }
}

//  Entidad Siniestro
public class wg_Siniestro
{
    private int wg_idSiniestro;
    private int wg_idPoliza;
    private double wg_montoReclamo;
    private double wg_pagoNeto;
    private string wg_estado;

    public int wg_IdSiniestro { get { return wg_idSiniestro; } set { wg_idSiniestro = value; } }
    public int wg_IdPoliza { get { return wg_idPoliza; } set { wg_idPoliza = value; } }

    public double wg_MontoReclamo
    {
        get { return wg_montoReclamo; }
        set
        {
            if (value <= 0)
                throw new wg_SiniestroInvalidoException("El monto del reclamo debe ser mayor a cero.");
            wg_montoReclamo = value;
        }
    }

    public double wg_PagoNeto
    {
        get { return wg_pagoNeto; }
        set
        {
            if (value < 0)
                throw new wg_SiniestroInvalidoException("El pago neto no puede ser negativo.");
            wg_pagoNeto = value;
        }
    }

    public string wg_Estado { get { return wg_estado; } set { wg_estado = value; } }

    public wg_Siniestro(int id, int idPoliza, double reclamo, double pagoNeto, string estado)
    {
        wg_IdSiniestro = id;
        wg_IdPoliza = idPoliza;
        wg_MontoReclamo = reclamo;
        wg_PagoNeto = pagoNeto;
        wg_Estado = estado;
    }
}

//  Entidad Reaseguro
public class wg_Reaseguro
{
    private int wg_idReaseguro;
    private int wg_idPoliza;
    private double wg_montoRetencion;
    private double wg_montoContrato;
    private double wg_montoFacultativo;

    public int wg_IdReaseguro { get { return wg_idReaseguro; } set { wg_idReaseguro = value; } }
    public int wg_IdPoliza { get { return wg_idPoliza; } set { wg_idPoliza = value; } }
    public double wg_MontoRetencion { get { return wg_montoRetencion; } set { wg_montoRetencion = value; } }
    public double wg_MontoContrato { get { return wg_montoContrato; } set { wg_montoContrato = value; } }
    public double wg_MontoFacultativo { get { return wg_montoFacultativo; } set { wg_montoFacultativo = value; } }

    public wg_Reaseguro(int id, int idPoliza, double retencion, double contrato, double facultativo)
    {
        wg_IdReaseguro = id;
        wg_IdPoliza = idPoliza;
        wg_MontoRetencion = retencion;
        wg_MontoContrato = contrato;
        wg_MontoFacultativo = facultativo;
    }
}

//  Entidad Asiento Contable
public class wg_AsientoContable
{
    private int wg_idAsiento;
    private string wg_tipoOperacion;
    private string wg_cuentaDebe;
    private string wg_cuentaHaber;
    private double wg_valor;

    public int wg_IdAsiento { get { return wg_idAsiento; } set { wg_idAsiento = value; } }
    public string wg_TipoOperacion { get { return wg_tipoOperacion; } set { wg_tipoOperacion = value; } }
    public string wg_CuentaDebe { get { return wg_cuentaDebe; } set { wg_cuentaDebe = value; } }
    public string wg_CuentaHaber { get { return wg_cuentaHaber; } set { wg_cuentaHaber = value; } }

    public double wg_Valor
    {
        get { return wg_valor; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("El valor del asiento contable debe ser mayor a cero.");
            wg_valor = value;
        }
    }

    public wg_AsientoContable(int id, string tipo, string debe, string haber, double valor)
    {
        wg_IdAsiento = id;
        wg_TipoOperacion = tipo;
        wg_CuentaDebe = debe;
        wg_CuentaHaber = haber;
        wg_Valor = valor;
    }
}

// Entidad Log de Sistema
public class wg_LogSistema
{
    public int wg_IdLog { get; set; }
    public DateTime wg_Fecha { get; set; }
    public string wg_Modulo { get; set; }
    public string wg_Mensaje { get; set; }

    public wg_LogSistema(int id, string modulo, string mensaje)
    {
        wg_IdLog = id;
        wg_Fecha = DateTime.Now;
        wg_Modulo = modulo;
        wg_Mensaje = mensaje;
    }
}