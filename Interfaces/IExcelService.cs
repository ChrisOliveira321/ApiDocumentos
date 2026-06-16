using CrudApi.Models;

namespace CrudApi.Interfaces;

public interface IExcelService
{
    Task AdicionarNotaAsync(DadosNotaFiscal dados);
}
