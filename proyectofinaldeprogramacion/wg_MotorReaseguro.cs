using System;

public static class wg_MotorReaseguro
{
    private const double wg_LIMITE_RETENCION = 500000;
    private const double wg_PORCENTAJE_RETENCION = 0.10;
    private const double wg_PORCENTAJE_CONTRATO = 0.20;

    public static void wg_CalcularCascadaReaseguro(
        double wg_capitalAsegurado,
        ref double wg_retencion,
        ref double wg_contrato,
        ref double wg_facultativo)
    {
        //  Validación estricta 
        if (wg_capitalAsegurado <= 0)
        {
            throw new wg_PolizaInvalidaException("El capital asegurado debe ser mayor a cero para calcular el reaseguro.");
        }

        //  Nivel 1 - Retención 
        double wg_retencionCalculada = wg_capitalAsegurado * wg_PORCENTAJE_RETENCION;

        // Aplicación del tope máximo estricto
        if (wg_retencionCalculada > wg_LIMITE_RETENCION)
        {
            wg_retencion = wg_LIMITE_RETENCION;
        }
        else
        {
            wg_retencion = wg_retencionCalculada;
        }

        wg_contrato = wg_capitalAsegurado * wg_PORCENTAJE_CONTRATO;

        wg_facultativo = wg_capitalAsegurado - wg_retencion - wg_contrato;

    }
}