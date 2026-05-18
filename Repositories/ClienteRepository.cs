using System.Linq;
using CrudApi.Models;

namespace CrudApi.Repositories;

public class ClienteRepository
{
    private readonly List<Cliente> _clientes = new()
    {
        // CLIENTE 
        new Cliente
        {
            Nome = "Matriz",
            Cnpj = "81.716.144/0001-40",
            Centro = "1001"
        },
        new Cliente
        {
            Nome = "Gexpo",
            Cnpj = "81.716.144/0015-46",
            Centro = "1015"
        },
        new Cliente
        {
            Nome = "Porto Seco",
            Cnpj = "07.057.278/0001-44",
            Centro = "2001"
        },
        new Cliente
        {
            Nome = "Rocha RS",
            Cnpj = "07.770.268/0001-51",
            Centro = "3001"
        },
        new Cliente
        {
            Nome = "Rocha Santana",
            Cnpj = "59.748.580/0001-42",
            Centro = "6001"
        },
    };

    public Cliente BuscarPorCnpj(string cnpj)
{
    if (string.IsNullOrWhiteSpace(cnpj))
        return null;

    return _clientes.FirstOrDefault(c => c.Cnpj == cnpj);
}
}