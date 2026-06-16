using CrudApi.Models;

namespace CrudApi.Interfaces;

public interface INotaFiscalParser
{
    DadosNotaFiscal ExtrairDados(string texto);
    string ExtrairNumeroNota(string texto);
    string ExtrairDataEmissao(string texto);
    string ExtrairValorTotal(string texto);
    string ExtrairNomeFornecedor(string texto);
}
