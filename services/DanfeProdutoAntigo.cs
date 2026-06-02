using System.Text.RegularExpressions;

namespace CrudApi.Services;

public class DanfeProdutoAntigoParser : NotaFiscalParserBase
{

    public override string ExtrairValorTotal(string texto)
    {
        Console.WriteLine("DanfeProdutoAntigoParser: ExtrairValorTotal chamado");

        var regex = new Regex(
            @"VALOR TOTAL DA NOTA\s+(?:[\d\.,]+\s+){5}([\d\.,]+)",
            RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Valor encontrado: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        return "Valor não encontrado";
    }
    
    public override string ExtrairNumeroNota(string texto)
    {
        Console.WriteLine("DanfeProdutoAntigoParser: ExtrairNumeroNota chamado");

        // Padrão principal da DANFE
        var regex = new Regex(
            @"N[ºo]\.?\s*(\d+)",
            RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Número encontrado: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        // fallback para layouts diferentes
        regex = new Regex(
            @"Número\s*[:\-]?\s*(\d+)",
            RegexOptions.IgnoreCase
        );

        match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Número fallback encontrado: {match.Groups[1].Value}");
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
        Console.WriteLine("DanfeProdutoAntigoParser: ExtrairDataEmissao chamado");

        var regex = new Regex(
            @"DATA DA EMISSÃO\s+(\d{2}/\d{2}/\d{4})",
            RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Data encontrada: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        // fallback
        regex = new Regex(
            @"\b(\d{2}/\d{2}/\d{4})\s+\d{2}:\d{2}:\d{2}",
            RegexOptions.IgnoreCase
        );

        match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Data fallback encontrada: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        return "Data não encontrada";
    }

}