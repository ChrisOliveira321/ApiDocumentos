using CrudApi.Interfaces;

namespace CrudApi.Services;

public class ExcelBackgroundService : BackgroundService
{
    private readonly IExcelQueue _excelQueue;
    private readonly IExcelService _excelService;
    private readonly ILogger<ExcelBackgroundService> _logger;

    public ExcelBackgroundService(
        IExcelQueue excelQueue,
        IExcelService excelService,
        ILogger<ExcelBackgroundService> logger)
    {
        _excelQueue = excelQueue;
        _excelService = excelService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de gravacao no Excel iniciado.");

        await foreach (var dados in _excelQueue.LerTodosAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation(
                    "Processando item da fila do Excel. NF: {NumeroNota}; Fornecedor: {Fornecedor}.",
                    dados.NumeroNota,
                    dados.NomeFornecedor);

                await _excelService.AdicionarNotaAsync(dados);

                _logger.LogInformation(
                    "Item da fila do Excel processado com sucesso. NF: {NumeroNota}.",
                    dados.NumeroNota);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Falha ao processar item da fila do Excel. NF: {NumeroNota}; Fornecedor: {Fornecedor}.",
                    dados.NumeroNota,
                    dados.NomeFornecedor);
            }
        }
    }
}
