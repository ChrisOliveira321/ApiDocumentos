using System.Text.RegularExpressions;
using CrudApi.Enums;
using CrudApi.Interfaces;

namespace CrudApi.Services; 

public class DanfePadraoModernoParser : INotaFiscalParser
{
    public DadosNotaFiscal ExtrairDados(string texto)
    {
        return new DadosNotaFiscal
        {
            NumeroNota = ExtrairNumeroNota(texto),
            NomeFornecedor = ExtrairNomeFornecedor(texto),
            ValorTotal = ExtrairValorTotal(texto),
            DataEmissao = ExtrairDataEmissao(texto),
            CnpjFornecedor = ExtrairCnpjFornecedor(texto),
            CnpjCliente = ExtrairCnpjCliente(texto),
        };
    }

    private string ExtrairValorTotal(string texto)
    {
        var regex = new Regex(
            @"VALOR TOTAL DA NFS-E.*?R\$\s*([\d\.,]+)",
            RegexOptions.Singleline
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }

        return "Valor não encontrado";
    }
    
    private string ExtrairNumeroNota(string texto)
    {
        var regex = new Regex(
            @"Número da NFS-e.*?\n(\d+)",
            RegexOptions.Singleline);

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
        var regex = new Regex(
            @"Data e Hora da emissão da NFS-e.*?\n\d+\s+(\d{2}/\d{2}/\d{4})",
            RegexOptions.Singleline
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
        var regex = new Regex(
            @"CNPJ\s*(?:/CPF)?\s*[:\-]?\s*(\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})",
            RegexOptions.IgnoreCase
        );

        var match = regex.Match(texto);

        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }

        var fallback = new Regex(@"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}");
        match = fallback.Match(texto);

        return match.Success ? match.Value.Trim() : null;
    }

    private string ExtrairCnpjCliente(string texto)
    {
        var regex = new Regex(
            @"CNPJ.*?cliente.*?(\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})",
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