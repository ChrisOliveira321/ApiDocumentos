using CrudApi.Enums;
using CrudApi.Interfaces;

namespace CrudApi.Services;

public class WorkSystemLayoutDetector : ILayoutDetector
{
    public TipoLayout Detectar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return TipoLayout.Desconhecido;

        var t = texto.ToUpperInvariant();

        // NFSeMunicipal rules (NFS-e municipal com serviços)
        if (t.Contains("NOTA FISCAL ELETRÔNICA DE SERVIÇOS") && t.Contains("PRESTADOR DE SERVIÇOS") && t.Contains("TOMADOR DE SERVIÇOS"))
        {
            Console.WriteLine("WorkSystemLayoutDetector: detectado NFSeMunicipal");
            return TipoLayout.NFSeMunicipal;
        }

        // DanfePadraoModerno rules (NFS-e padrão moderno com palavras-chave características)
        // Baseado no DanfePadraoModernoParser: procura por "Número da NFS-e", "VALOR TOTAL DA NFS-E", etc.
        if (t.Contains("NÚMERO DA NFS-E") || t.Contains("VALOR TOTAL DA NFS-E") || t.Contains("DATA E HORA DA EMISSÃO DA NFS-E"))
        {
            Console.WriteLine("WorkSystemLayoutDetector: detectado DanfePadraoModerno");
            return TipoLayout.DanfePadraoModerno;
        }

        // DanfeProdutoModerno rules (DANFE com protocolo de autorização)
        if (t.Contains("PROTOCOLO DE AUTORIZAÇÃO DE USO") && t.Contains("DESTINATÁRIO / REMETENTE") && t.Contains("DADOS DOS PRODUTOS / SERVIÇOS"))
        {
            Console.WriteLine("WorkSystemLayoutDetector: detectado DanfeProdutoModerno");
            return TipoLayout.DanfeProdutoModerno;
        }

        // Fallback for WorkSystem: DanfeProduto (antigo)
        Console.WriteLine("WorkSystemLayoutDetector: fallback para DanfeProduto");
        return TipoLayout.DanfeProduto;
    }
}
