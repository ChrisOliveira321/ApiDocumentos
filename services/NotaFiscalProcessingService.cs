using System.Text;
using CrudApi.Data;
using CrudApi.Enums;
using CrudApi.Models;
using CrudApi.Repositories;

namespace CrudApi.Services;

public class NotaFiscalProcessingService
{
    private readonly PdfService _pdfService;
    private readonly LayoutDetectorService _layoutDetector;
    private readonly CnpjReaderService _cnpjReader;
    private readonly FornecedorRepository _fornecedorRepository;
    private readonly ParserRegistryService _parserRegistry;

    public NotaFiscalProcessingService(
        PdfService pdfService,
        LayoutDetectorService layoutDetector,
        CnpjReaderService cnpjReader,
        FornecedorRepository fornecedorRepository,
        ParserRegistryService parserRegistry)
    {
        _pdfService = pdfService;
        _layoutDetector = layoutDetector;
        _cnpjReader = cnpjReader;
        _fornecedorRepository = fornecedorRepository;
        _parserRegistry = parserRegistry;
    }

    public Documento ProcessarDocumento(string caminho, string nomeArquivo)
    {
        var texto = _pdfService.LerPdf(caminho);
        var layout = _layoutDetector.Detectar(texto);
        var parser = _parserRegistry.ObterParser(layout);
        var dados = parser?.ExtrairDados(texto) ?? new DadosNotaFiscal();

        var cnpjs = _cnpjReader.ExtrairCnpjs(texto);
        PreencherDadosFornecedor(dados, cnpjs);

        return new Documento
        {
            Id = FakeDb.Documentos.Count + 1,
            nomeArquivo = nomeArquivo,
            Tipo = "NF",
            ConteudoExtraido = MontarConteudoExtraido(layout, dados),
            DataUpload = DateTime.Now
        };
    }

    private void PreencherDadosFornecedor(DadosNotaFiscal dados, List<string> cnpjs)
    {
        if (!string.IsNullOrWhiteSpace(dados.CnpjFornecedor))
        {
            return;
        }

        foreach (var cnpj in cnpjs)
        {
            var fornecedor = _fornecedorRepository.BuscarPorCnpj(cnpj);

            if (fornecedor == null)
            {
                continue;
            }

            dados.CnpjFornecedor = cnpj;

            if (string.IsNullOrWhiteSpace(dados.NomeFornecedor))
            {
                dados.NomeFornecedor = fornecedor.Nome;
            }

            break;
        }
    }

    private string MontarConteudoExtraido(TipoLayout layout, DadosNotaFiscal dados)
    {
        var builder = new StringBuilder();

        builder.Append($"Layout: {layout} |");
        builder.Append($"NF: {dados.NumeroNota} |");
        builder.Append($"NomeFornecedor: {dados.NomeFornecedor} |");
        builder.Append($"CNPJ Fornecedor: {dados.CnpjFornecedor} |");
        builder.Append($"Valor Total: {dados.ValorTotal} |");
        builder.Append($"Data de Emissão: {dados.DataEmissao} |");

        if (!string.IsNullOrWhiteSpace(dados.CnpjCliente))
        {
            builder.Append($"CNPJ Cliente: {dados.CnpjCliente} |");
        }

        return builder.ToString();
    }
}
