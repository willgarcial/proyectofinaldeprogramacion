using System;

// excepcion para la persistencia dual 
public class wg_ArchivoDatosException : Exception
{
    public wg_ArchivoDatosException() : base()
    {
    }

    public wg_ArchivoDatosException(string wg_mensaje) : base(wg_mensaje)
    {
    }

    public wg_ArchivoDatosException(string wg_mensaje, Exception wg_excepcionInterna) : base(wg_mensaje, wg_excepcionInterna)
    {
    }
}

// excepcion para validar reglas de negocio del Cliente 
public class wg_ClienteInvalidoException : Exception
{
    public wg_ClienteInvalidoException() : base()
    {
    }

    public wg_ClienteInvalidoException(string wg_mensaje) : base(wg_mensaje)
    {
    }

    public wg_ClienteInvalidoException(string wg_mensaje, Exception wg_excepcionInterna) : base(wg_mensaje, wg_excepcionInterna)
    {
    }
}

// excepcion para el control estricto de Pólizas (Montos negativos, capitales absurdos)
public class wg_PolizaInvalidaException : Exception
{
    public wg_PolizaInvalidaException() : base()
    {
    }

    public wg_PolizaInvalidaException(string wg_mensaje) : base(wg_mensaje)
    {
    }

    public wg_PolizaInvalidaException(string wg_mensaje, Exception wg_excepcionInterna) : base(wg_mensaje, wg_excepcionInterna)
    {
    }
}

// excepcion para blindar los Siniestros (Evitar que un reclamo supere el capital remanente)
public class wg_SiniestroInvalidoException : Exception
{
    public wg_SiniestroInvalidoException() : base()
    {
    }

    public wg_SiniestroInvalidoException(string wg_mensaje) : base(wg_mensaje)
    {
    }

    public wg_SiniestroInvalidoException(string wg_mensaje, Exception wg_excepcionInterna) : base(wg_mensaje, wg_excepcionInterna)
    {
    }
}