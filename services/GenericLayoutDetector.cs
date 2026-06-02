using CrudApi.Enums;
using CrudApi.Interfaces;

namespace CrudApi.Services;

public class GenericLayoutDetector : ILayoutDetector
{
    public TipoLayout Detectar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return TipoLayout.Desconhecido;

        var t = texto.ToUpperInvariant();

        // Try NFSe patterns
        if (t.Contains("NOTA FISCAL ELETRÔNICA DE SERVIÇOS") || (t.Contains("PRESTADOR DE SERVIÇOS") && t.Contains("TOMADOR DE SERVIÇOS")))
        {
            Console.WriteLine("GenericLayoutDetector: detectado NFSeMunicipal");
            return TipoLayout.NFSeMunicipal;
        }

        // Try moderno DANFE with authorization protocol
        if (t.Contains("PROTOCOLO DE AUTORIZAÇÃO DE USO") || (t.Contains("DESTINATÁRIO / REMETENTE") && t.Contains("DADOS DOS PRODUTOS")))
        {
            Console.WriteLine("GenericLayoutDetector: detectado DanfeProdutoModerno");
            return TipoLayout.DanfeProdutoModerno;
        }

        // Try antigo DANFE simple heuristic
        if (t.Contains("Nº") && t.Contains("DATA DA EMISSÃO") && t.Contains("VALOR TOTAL"))
        {
            Console.WriteLine("GenericLayoutDetector: detectado DanfeProduto");
            return TipoLayout.DanfeProduto;
        }

        return TipoLayout.Desconhecido;
    }
}
