using CrudApi.Models;

namespace CrudApi.Interfaces;

public interface IExcelQueue
{
    void Enfileirar(DadosNotaFiscal dados);

    IAsyncEnumerable<DadosNotaFiscal> LerTodosAsync(CancellationToken cancellationToken);
}
