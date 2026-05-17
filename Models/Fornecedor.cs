namespace CrudApi.Models;

using CrudApi.Enums;

public class Fornecedor
{
    public string Cnpj { get; set; }
    public string Nome { get; set; }
    public TipoLayout Layout { get; set; }
}   