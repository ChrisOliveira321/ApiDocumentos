using System.Text.RegularExpressions;

namespace CrudApi.Services;

public class DanfeProdutoModernoParser : NotaFiscalParserBase
{
    public override string ExtrairNumeroNota(string texto)
    {
        Console.WriteLine("DanfeProdutoModernoParser: ExtrairNumeroNota chamado");

        // Padrão principal: "Nº 000.000.447" ou variações "Nº 000000447" ou "N o 000.000.447"
        var regex = new Regex(
            @"\bN[º°\u00BAoO]?\s*\.?\s*([0-9\.\-/]{3,})",
            RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            var raw = match.Groups[1].Value.Trim();
            // Normalizar para somente dígitos (remover pontos/barra/traço)
            var digitsOnly = Regex.Replace(raw, "[^0-9]", "");
            Console.WriteLine($"Número encontrado (raw): {raw} -> {digitsOnly}");
            return digitsOnly;
        }

        // Fallback: procurar por "Nº" com possíveis espaços e quebras
        regex = new Regex(@"\bN\s*[º°oO]?\s*[:\-]?\s*([0-9\.\-/]{3,})", RegexOptions.IgnoreCase);

        match = regex.Match(texto);

        if (match.Success)
        {
            var raw = match.Groups[1].Value.Trim();
            var digitsOnly = Regex.Replace(raw, "[^0-9]", "");
            Console.WriteLine($"Número fallback encontrado (raw): {raw} -> {digitsOnly}");
            return digitsOnly;
        }

        return "Número não encontrado";
    }

    public override string ExtrairDataEmissao(string texto)
    {
        Console.WriteLine("DanfeProdutoModernoParser: ExtrairDataEmissao chamado");

        // Padrão direto: "DATA DA EMISSÃO" seguido de uma data no formato dd/MM/yyyy
        var regex = new Regex(
            @"DATA\s*DA\s*EMI\S*[:\s\-]{0,8}(\d{1,2}/\d{1,2}/\d{4})",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Data encontrada: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        // Fallback: pegar a primeira data próxima a palavras que indiquem emissão
        regex = new Regex(@"EMISS.*?(\d{1,2}/\d{1,2}/\d{4})", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Data fallback encontrada: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        return "Data não encontrada";
    }

    public override string ExtrairValorTotal(string texto)
    {
        Console.WriteLine("DanfeProdutoModernoParser: ExtrairValorTotal chamado");

        // Padrão: "VALOR TOTAL DA NOTA" seguido por vários valores de colunas e o total final
        var regex = new Regex(
            @"VALOR\s*TOTAL\s*DA\s*NOTA([\s\S]{0,120})",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            var trailingText = match.Groups[1].Value;
            var numberMatches = Regex.Matches(trailingText, @"[0-9]{1,3}(?:[\.,][0-9]{2})");

            if (numberMatches.Count > 0)
            {
                var found = numberMatches[numberMatches.Count - 1].Value;
                Console.WriteLine($"Valor encontrado: {found}");
                return found.Trim();
            }
        }

        // Fallback: procurar o termo "VALOR TOTAL" e capturar o próximo número com vírgula/decimal
        regex = new Regex(@"VALOR\s*TOTAL[\s\S]{0,40}?([0-9]+[\.,][0-9]{2})", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"Valor fallback encontrado: {match.Groups[1].Value}");
            return match.Groups[1].Value.Trim();
        }

        return "Valor não encontrado";
    }

    public override string ExtrairNomeFornecedor(string texto)
    {
        // Este parser não extrai o nome do fornecedor; será obtido pelo repositório.
        return "Fornecedor não extraído";
    }
}
