using ClosedXML.Excel;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.Extensions.Options;

namespace CrudApi.Services;

public class ExcelService : IExcelService
{
    private const int MaxTentativasEscrita = 3;
    private static readonly TimeSpan IntervaloTentativas = TimeSpan.FromMilliseconds(250);
    private static readonly SemaphoreSlim EscritaLock = new(1, 1);

    private static readonly IReadOnlyList<ExcelColumnMapping> ColumnMappings =
    [
        new("Layout", dados => dados.Layout),
        new("NF", dados => dados.NumeroNota),
        new("Data de Emissão", dados => dados.DataEmissao),
        new("Fornecedor", dados => dados.NomeFornecedor),
        new("CNPJ Fornecedor", dados => dados.CnpjFornecedor),
        new("CNPJ Rocha", dados => dados.CnpjCliente),
        new("Valor", dados => dados.ValorTotal)
    ];

    private readonly ExcelOptions _options;
    private readonly ILogger<ExcelService> _logger;

    public ExcelService(IOptions<ExcelOptions> options, ILogger<ExcelService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task AdicionarNotaAsync(DadosNotaFiscal dados)
    {
        if (dados == null)
        {
            throw new ArgumentNullException(nameof(dados));
        }

        await EscritaLock.WaitAsync();

        try
        {
            await ExecutarComRetryAsync(() =>
            {
                var caminhoArquivo = ObterCaminhoArquivo();
                Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo)!);

                using var workbook = File.Exists(caminhoArquivo)
                    ? new XLWorkbook(caminhoArquivo)
                    : CriarWorkbookComEstruturaInicial(dados);

                var resultadoTabela = ObterOuCriarTabela(workbook, dados);

                if (!resultadoTabela.LinhaJaAdicionada)
                {
                    AdicionarLinha(resultadoTabela.Tabela, dados);
                }

                workbook.SaveAs(caminhoArquivo);
            });
        }
        finally
        {
            EscritaLock.Release();
        }
    }

    private async Task ExecutarComRetryAsync(Action operacao)
    {
        for (var tentativa = 1; tentativa <= MaxTentativasEscrita; tentativa++)
        {
            try
            {
                operacao();
                return;
            }
            catch (IOException ex) when (tentativa < MaxTentativasEscrita)
            {
                _logger.LogWarning(ex, "Falha ao escrever no Excel. Tentativa {Tentativa} de {MaxTentativas}.", tentativa, MaxTentativasEscrita);
                await Task.Delay(IntervaloTentativas);
            }
            catch (UnauthorizedAccessException ex) when (tentativa < MaxTentativasEscrita)
            {
                _logger.LogWarning(ex, "Acesso negado ao escrever no Excel. Tentativa {Tentativa} de {MaxTentativas}.", tentativa, MaxTentativasEscrita);
                await Task.Delay(IntervaloTentativas);
            }
        }

        throw new InvalidOperationException("Não foi possível gravar os dados na planilha Excel. Verifique se o arquivo está aberto ou bloqueado.");
    }

    private string ObterCaminhoArquivo()
    {
        if (string.IsNullOrWhiteSpace(_options.CaminhoArquivo))
        {
            throw new InvalidOperationException("O caminho da planilha Excel não foi configurado.");
        }

        return _options.CaminhoArquivo;
    }

    private XLWorkbook CriarWorkbookComEstruturaInicial(DadosNotaFiscal dados)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(_options.NomeAba);

        PreencherCabecalhos(worksheet);
        PreencherLinha(worksheet.Row(2), dados);
        worksheet.Range(1, 1, 2, ColumnMappings.Count).CreateTable(_options.NomeTabela);

        return workbook;
    }

    private (IXLTable Tabela, bool LinhaJaAdicionada) ObterOuCriarTabela(XLWorkbook workbook, DadosNotaFiscal dados)
    {
        var tabela = workbook.Worksheets
            .SelectMany(worksheet => worksheet.Tables)
            .FirstOrDefault(table => string.Equals(table.Name, _options.NomeTabela, StringComparison.OrdinalIgnoreCase));

        if (tabela != null)
        {
            ValidarCabecalhos(tabela);
            return (tabela, false);
        }

        var worksheet = workbook.Worksheets
            .FirstOrDefault(sheet => string.Equals(sheet.Name, _options.NomeAba, StringComparison.OrdinalIgnoreCase))
            ?? workbook.Worksheets.FirstOrDefault()
            ?? workbook.Worksheets.Add(_options.NomeAba);

        var ultimaLinhaUsada = worksheet.LastRowUsed()?.RowNumber() ?? 0;

        if (ultimaLinhaUsada == 0)
        {
            PreencherCabecalhos(worksheet);
            PreencherLinha(worksheet.Row(2), dados);
            var novaTabela = worksheet.Range(1, 1, 2, ColumnMappings.Count).CreateTable(_options.NomeTabela);

            return (novaTabela, true);
        }

        ValidarOuPreencherCabecalhos(worksheet);

        if (ultimaLinhaUsada == 1)
        {
            PreencherLinha(worksheet.Row(2), dados);
            var novaTabela = worksheet.Range(1, 1, 2, ColumnMappings.Count).CreateTable(_options.NomeTabela);

            return (novaTabela, true);
        }

        var tabelaCriada = worksheet.Range(1, 1, ultimaLinhaUsada, ColumnMappings.Count).CreateTable(_options.NomeTabela);
        ValidarCabecalhos(tabelaCriada);

        return (tabelaCriada, false);
    }

    private static void PreencherCabecalhos(IXLWorksheet worksheet)
    {
        for (var index = 0; index < ColumnMappings.Count; index++)
        {
            worksheet.Cell(1, index + 1).Value = ColumnMappings[index].Header;
        }
    }

    private static void ValidarOuPreencherCabecalhos(IXLWorksheet worksheet)
    {
        var primeiraLinhaVazia = !worksheet.Row(1).Cells(1, ColumnMappings.Count)
            .Any(cell => !string.IsNullOrWhiteSpace(cell.GetString()));

        if (primeiraLinhaVazia)
        {
            PreencherCabecalhos(worksheet);
            return;
        }

        var headers = worksheet.Row(1).Cells(1, ColumnMappings.Count)
            .Select(cell => cell.GetString())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var faltantes = ColumnMappings
            .Select(mapping => mapping.Header)
            .Where(header => !headers.Contains(header))
            .ToList();

        if (faltantes.Count > 0)
        {
            throw new InvalidOperationException($"A planilha Excel não possui os cabeçalhos esperados: {string.Join(", ", faltantes)}.");
        }
    }

    private static void AdicionarLinha(IXLTable tabela, DadosNotaFiscal dados)
    {
        var worksheet = tabela.Worksheet;
        var novaLinhaNumero = tabela.RangeAddress.LastAddress.RowNumber + 1;

        worksheet.Row(novaLinhaNumero).InsertRowsAbove(1);
        PreencherLinhaTabela(tabela, worksheet.Row(novaLinhaNumero), dados);

        var primeiraLinha = tabela.RangeAddress.FirstAddress.RowNumber;
        var primeiraColuna = tabela.RangeAddress.FirstAddress.ColumnNumber;
        var ultimaLinha = novaLinhaNumero;
        var ultimaColuna = primeiraColuna + ColumnMappings.Count - 1;

        tabela.Resize(worksheet.Range(primeiraLinha, primeiraColuna, ultimaLinha, ultimaColuna));
    }

    private static void PreencherLinha(IXLRow row, DadosNotaFiscal dados)
    {
        for (var index = 0; index < ColumnMappings.Count; index++)
        {
            row.Cell(index + 1).Value = ColumnMappings[index].GetValue(dados);
        }
    }

    private static void PreencherLinhaTabela(IXLTable tabela, IXLRow row, DadosNotaFiscal dados)
    {
        var colunasPorCabecalho = ObterColunasPorCabecalho(tabela);

        foreach (var mapping in ColumnMappings)
        {
            row.Cell(colunasPorCabecalho[mapping.Header]).Value = mapping.GetValue(dados);
        }
    }

    private static Dictionary<string, int> ObterColunasPorCabecalho(IXLTable tabela)
    {
        return tabela.HeadersRow().Cells()
            .ToDictionary(
                cell => cell.GetString(),
                cell => cell.Address.ColumnNumber,
                StringComparer.OrdinalIgnoreCase);
    }

    private static void ValidarCabecalhos(IXLTable tabela)
    {
        var headers = tabela.HeadersRow().Cells()
            .Select(cell => cell.GetString())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var faltantes = ColumnMappings
            .Select(mapping => mapping.Header)
            .Where(header => !headers.Contains(header))
            .ToList();

        if (faltantes.Count > 0)
        {
            throw new InvalidOperationException($"A tabela Excel não possui os cabeçalhos esperados: {string.Join(", ", faltantes)}.");
        }
    }

    private sealed record ExcelColumnMapping(string Header, Func<DadosNotaFiscal, string> GetValue);
}
