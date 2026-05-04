using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace CrudApi.Services;

public class ExtracaoService
{
    public DadosNotaFiscal ExtrairDados(string texto)
    {
        return new DadosNotaFiscal
        {
            DocumentoDestinatario = ExtrairDocumentoDestinatario(texto),
            DocumentoEmitente = ExtrairDocumentoEmitente(texto),
            valorTotal = ExtrairValorTotal(texto)
        };
    }

    public string ExtrairDocumentoEmitente(string texto)
    {
        var regex = new Regex(
            @"INSCRIÇÃO ESTADUAL.*?(\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})",
            RegexOptions.Singleline
        );

        var match = regex.Match(texto);

        if (match.Success)
            return match.Groups[1].Value;

        return "Documento emitente não encontrado";
    }

    public string ExtrairDocumentoDestinatario(string texto)
    {
        var regex = new Regex(
            @"DESTINATÁRIO \/ REMETENTE.*?(\d{3}\.\d{3}\.\d{3}-\d{2}|\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})",
            RegexOptions.Singleline
        );

        var match = regex.Match(texto);

        if (match.Success)
            return match.Groups[1].Value;

        return "Documento destinatário não encontrado";
    }

    public string ExtrairValorTotal(string texto)
    {
        int indice = texto.IndexOf("VALOR TOTAL DA NOTA");

        if (indice == -1)
            return "Valor não encontrado";

        string trecho = texto.Substring(indice, 150);

        var matches = Regex.Matches(
            trecho,
            @"\d{1,3}(\.\d{3})*,\d{2}"
        );

        if (matches.Count > 0)
        {
            return matches[matches.Count - 1].Value;
        }

        return "Valor não encontrado";
    }
}