using System.Text.RegularExpressions;

namespace CrudApi.Services;

public class DanfePadraoModernoParser : NotaFiscalParserBase
{

    public override string ExtrairValorTotal(string texto)
    {
        Console.WriteLine("DanfePadraoModernoParser: ExtrairValorTotal chamado");
        var regex = new Regex(
            @"VALOR TOTAL DA NFS-E.*?R\$\s*([\d\.,]+)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"DanfePadraoModernoParser: ValorTotal regex encontrou: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        return "Valor não encontrado";
    }
    
    public override string ExtrairNumeroNota(string texto)
    {
        Console.WriteLine("DanfePadraoModernoParser: ExtrairNumeroNota chamado");
        var regex = new Regex(
            @"Número da NFS-e.*?\n(\d+)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        var match = regex.Match(texto);

        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }

        return "Número não encontrado";
    }

    public override string ExtrairNomeFornecedor(string texto)
    {
        texto = texto.ToLower();

        var regex = new Regex(
            @"nome\s*\/\s*nome\s*empresarial.*?\n([^\n]+)"
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            var linha = match.Groups[1].Value.Trim();

            // Se tiver email, remove
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
        Console.WriteLine("DanfePadraoModernoParser: ExtrairDataEmissao chamado");
        var regex = new Regex(
            @"Data e Hora da emissão da NFS-e.*?\n\d+\s+(\d{2}/\d{2}/\d{4})",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }

        return null;
    }

}