using System.Threading.Channels;
using CrudApi.Interfaces;
using CrudApi.Models;

namespace CrudApi.Services;

public class ExcelQueue : IExcelQueue
{
    private readonly Channel<DadosNotaFiscal> _channel = Channel.CreateUnbounded<DadosNotaFiscal>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    private readonly ILogger<ExcelQueue> _logger;

    public ExcelQueue(ILogger<ExcelQueue> logger)
    {
        _logger = logger;
    }

    public void Enfileirar(DadosNotaFiscal dados)
    {
        if (dados == null)
        {
            throw new ArgumentNullException(nameof(dados));
        }

        if (!_channel.Writer.TryWrite(dados))
        {
            _logger.LogError("Nao foi possivel enfileirar nota para gravacao no Excel. NF: {NumeroNota}.", dados.NumeroNota);
            throw new InvalidOperationException("Nao foi possivel enfileirar a gravacao no Excel.");
        }

        _logger.LogInformation(
            "Nota enfileirada para gravacao no Excel. NF: {NumeroNota}; Fornecedor: {Fornecedor}; Valor: {ValorTotal}.",
            dados.NumeroNota,
            dados.NomeFornecedor,
            dados.ValorTotal);
    }

    public IAsyncEnumerable<DadosNotaFiscal> LerTodosAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
