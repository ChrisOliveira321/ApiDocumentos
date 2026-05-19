using System.Text.RegularExpressions;
using CrudApi.Enums;
using CrudApi.Interfaces;
using CrudApi.Repositories;

namespace CrudApi.Services;

public class DanfePadraoModernoParser : INotaFiscalParser
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

    private string ExtrairValorTotal(string texto)
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
    
    private string ExtrairNumeroNota(string texto)
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

    private string ExtrairCnpjFornecedor(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return null;

        var regex = new Regex(@"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}|\d{14}");
        var match = regex.Match(texto);

        if (match.Success)
        {
            return match.Value.Trim();
        }

        return null;
    }
}