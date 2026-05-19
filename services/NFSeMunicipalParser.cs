using System.Text.RegularExpressions;
using CrudApi.Enums;
using CrudApi.Interfaces;
using CrudApi.Repositories;

namespace CrudApi.Services;

public class NFSeMunicipalParser : INotaFiscalParser
{
    public DadosNotaFiscal ExtrairDados(string texto)
    {
        var dados = new DadosNotaFiscal
        {
            NumeroNota = ExtrairNumeroNota(texto),
            ValorTotal = ExtrairValorTotal(texto),
            DataEmissao = ExtrairDataEmissao(texto),
        };

        var cnpj = ExtrairCnpjFornecedor(texto);

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            dados.CnpjFornecedor = cnpj;
            var repo = new FornecedorRepository();
            var fornecedor = repo.BuscarPorCnpj(cnpj);

            if (fornecedor != null)
            {
                dados.NomeFornecedor = fornecedor.Nome;
                return dados;
            }
        }

        dados.NomeFornecedor = ExtrairNomeFornecedor(texto);
        return dados;
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

    private string ExtrairNomeFornecedor(string texto)
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

   private string ExtrairDataEmissao(string texto)
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

    private string ExtrairCnpjFornecedor(string texto)
    {
        Console.WriteLine("NFSeMunicipalParser: ExtrairCnpjFornecedor chamado");
        if (string.IsNullOrWhiteSpace(texto)) return null;

        var regex = new Regex(@"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}|\d{14}");
        var match = regex.Match(texto);

        if (match.Success)
        {
            Console.WriteLine($"NFSeMunicipalParser: CNPJ encontrado: {match.Value}");
            return match.Value.Trim();
        }

        return null;
    }
}