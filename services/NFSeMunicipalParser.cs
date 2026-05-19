using System.Text.RegularExpressions;
using CrudApi.Enums;
using CrudApi.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace CrudApi.Services; 

public class NFSeMunicipalParser : INotaFiscalParser
{
    public DadosNotaFiscal ExtrairDados(string texto)
    {
        return new DadosNotaFiscal
        {
            NumeroNota = ExtrairNumeroNota(texto),
            NomeFornecedor = ExtrairNomeFornecedor(texto),
            ValorTotal = ExtrairValorTotal(texto),
            DataEmissao = ExtrairDataEmissao(texto),
        };       
    }

    private string ExtrairNumeroNota(string texto)
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

    private string ExtrairValorTotal(string texto)
    {
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
    
    private string ExtrairNomeFornecedor(string texto)
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

    private string ExtrairDataEmissao(string texto)
    {
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