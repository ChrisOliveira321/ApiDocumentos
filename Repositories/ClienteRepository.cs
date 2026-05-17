using System.Linq;
using CrudApi.Models;

namespace CrudApi.Repositories;

public class ClienteRepository
{
    private readonly List<Cliente> _clientes = new()
    {
        // CLIENTE PADRÃO
        new Cliente
        {
            Nome = "Matriz",
            Cnpj = "81.716.144/0001-40",
            Centro = "1001"
        }
    };

    public Cliente BuscarPorCnpj(string cnpj)
{
    if (string.IsNullOrWhiteSpace(cnpj))
        return null;

    return _clientes.FirstOrDefault(c => c.Cnpj == cnpj);
}
}