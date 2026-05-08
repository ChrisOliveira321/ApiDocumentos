namespace CrudApi.Services;
using CrudApi.Enums;

public class LayoutRegistryService
{
    private readonly Dictionary<string, TipoLayout> _layouts = new()
    {
        // WorkSystem
        {"59.716.660/0001-16", TipoLayout.DanfePadraoModerno},
        // Guaiba
        {"03.962.178/0002-92", TipoLayout.DanfePadraoModerno},
        // Soow Sigma
        {"78.766.151/0001-42", TipoLayout.DanfePadraoModerno},
        // RKF Nobreaks
        {"07.395.076/0001-02", TipoLayout.DanfePadraoModerno},
        // Teste Magazine Luiza
        {"47.960.950/0897-85", TipoLayout.DanfePadraoModerno}
    };

    public TipoLayout ObterLayout(string cnpj)
    {
        if (_layouts.ContainsKey(cnpj))
        {
            return _layouts[cnpj];
        }

        return TipoLayout.Desconhecido;
    }
}