namespace CrudApi.Models;

using CrudApi.Enums;

public class Fornecedor
{
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public TipoLayout Layout { get; set; }
}   
