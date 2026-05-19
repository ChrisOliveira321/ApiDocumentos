using System.Text.RegularExpressions;

namespace CrudApi.Services;

public class NFSeMunicipalParser : NotaFiscalParserBase
{

    public override string ExtrairNumeroNota(string texto)
    {
        Console.WriteLine("ENTROU NO NFSeMunicipalParser");

        var match = Regex.Match(
            texto,
            @"Número da Nota\s+.*?\s+(?<numero>\d{8})",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        if (match.Success)
        {
            return match.Groups["numero"].Value.TrimStart('0');
        }

        return "Número não encontrado";
    }

    public override string ExtrairValorTotal(string texto)
    {
        Console.WriteLine("ENTROU NO PARSER DE VALOR TOTAL");

        var match = Regex.Match(
            texto,
            @"VALOR TOTAL DO SERVIÇO\s*=\s*R\$\s*(?<valor>[\d\.,]+)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        if (match.Success)
        {
            return match.Groups["valor"].Value.Trim();
        }

        return "Valor não encontrado";
    }

    public override string ExtrairNomeFornecedor(string texto)
    {
        texto = texto.ToLower();

        var regex = new Regex(
            @"nome\s*\/\s*nome\s*empresarial.*?\n([^\n]+)",
            RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            var linha = match.Groups[1].Value.Trim();

            if (linha.Contains("@"))
            {
                var regexEmail = new Regex(@"\S+@\S+");
                linha = regexEmail.Replace(linha, "").Trim();
            }

            return linha;
        }

        return "Fornecedor não encontrado";
    }

    public override string ExtrairDataEmissao(string texto)
    {
        Console.WriteLine("ENTROU NO PARSER DE DATA");

        var match = Regex.Match(
            texto,
            @"Data e Hora de Emissão\s+.*?\s+(?<data>\d{2}/\d{2}/\d{4})",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        if (match.Success)
        {
            return match.Groups["data"].Value.Trim();
        }

        return "Data não encontrada";
    }

}