namespace CrudApi.Models;

public class DadosNotaFiscal
{
    public string Layout { get; set; } = string.Empty;

    public string NumeroNota { get; set; } = string.Empty;

    public string DataEmissao { get; set; } = string.Empty;

    public string NomeFornecedor { get; set; } = string.Empty;

    public string CnpjFornecedor { get; set; } = string.Empty;

    public string CnpjCliente { get; set; } = string.Empty;

    public string ValorTotal { get; set; } = string.Empty;
}
