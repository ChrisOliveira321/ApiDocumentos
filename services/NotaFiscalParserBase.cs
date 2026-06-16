using System.Linq;
using CrudApi.Interfaces;
using CrudApi.Models;
using CrudApi.Repositories;

namespace CrudApi.Services;

public abstract class NotaFiscalParserBase : INotaFiscalParser
{
    private readonly CnpjReaderService _cnpjReader = new();
    private readonly FornecedorRepository _fornecedorRepository = new();

    public DadosNotaFiscal ExtrairDados(string texto)
    {
        var cnpj = ExtrairCnpj(texto);

        var dados = new DadosNotaFiscal
        {
            NumeroNota = ExtrairNumeroNota(texto),
            ValorTotal = ExtrairValorTotal(texto),
            DataEmissao = ExtrairDataEmissao(texto),
            CnpjFornecedor = cnpj
        };

        dados.NomeFornecedor = ObterNomeFornecedorPeloCnpj(cnpj) ?? ExtrairNomeFornecedor(texto);

        return dados;
    }

    protected string ExtrairCnpj(string texto)
    {
        return _cnpjReader.ExtrairCnpjs(texto).FirstOrDefault() ?? string.Empty;
    }

    protected string? ObterNomeFornecedorPeloCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
        {
            return null;
        }

        var fornecedor = _fornecedorRepository.BuscarPorCnpj(cnpj);
        return fornecedor?.Nome;
    }

    public abstract string ExtrairNumeroNota(string texto);
    public abstract string ExtrairDataEmissao(string texto);
    public abstract string ExtrairValorTotal(string texto);
    public abstract string ExtrairNomeFornecedor(string texto);
}
