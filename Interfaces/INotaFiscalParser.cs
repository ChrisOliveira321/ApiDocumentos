namespace CrudApi.Interfaces;

public interface INotaFiscalParser
{
    DadosNotaFiscal ExtrairDados(string texto);
}