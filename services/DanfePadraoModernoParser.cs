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
            DataEmissao = ExtrairDataEmissao(texto),
            CnpjFornecedor = ExtrairCnpjFornecedor(texto),
            CnpjCliente = ExtrairCnpjCliente(texto),
            ValorTotal = ExtrairValorTotal(texto)
        };
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

    private string ExtrairValorTotal(string texto)
    {
        return null;
    }

    private string ExtrairDataEmissao(string texto)
    {
        return null;
    }

    private string ExtrairCnpjFornecedor(string texto)
    {
        return null;
    }

    private string ExtrairCnpjCliente(string texto)
    {
        return null;
    }
}