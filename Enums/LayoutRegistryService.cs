using CrudApi.Enums;
using CrudApi.Repositories;

namespace CrudApi.Services;

public class LayoutRegistryService
{
    private readonly FornecedorRepository _repository = new();

    public TipoLayout ObterLayout(string cnpj)
    {
        var fornecedor = _repository.BuscarPorCnpj(cnpj);

        if (fornecedor != null)
        {
            return fornecedor.Layout;
        }

        return TipoLayout.Desconhecido;
    }
}