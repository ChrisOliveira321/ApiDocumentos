using CrudApi.Enums;
using CrudApi.Interfaces;

namespace CrudApi.Services;

public class PloomesLayoutDetector : ILayoutDetector
{
    public TipoLayout Detectar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return TipoLayout.Desconhecido;

        var t = texto.ToUpperInvariant();

        // Ploomes known patterns for NFSe Municipal
        if (t.Contains("NOTA FISCAL ELETRÔNICA DE SERVIÇOS") && t.Contains("PRESTADOR DE SERVIÇOS"))
        {
            Console.WriteLine("PloomesLayoutDetector: detectado NFSeMunicipal");
            return TipoLayout.NFSeMunicipal;
        }

        // Otherwise unknown here - let global detectors try
        return TipoLayout.Desconhecido;
    }
}
