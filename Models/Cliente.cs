namespace CrudApi.Models;

using CrudApi.Enums;

public class Cliente
{
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Centro { get; set; } = string.Empty;
}   
