namespace CrudApi.Services;
using CrudApi.Enums;

public class LayoutRegistryService
{
    private readonly Dictionary<TipoLayout, List<string>> _layouts = new()
    {
        {
            TipoLayout.DanfePadraoAntigo,
            new List<string>
            {
                "" // Futuramente layout alternativo da WorkSystem
            }
        },

        {
            TipoLayout.DanfePadraoModerno,
            new List<string>
            {
                "59.716.660/0001-16", // WorkSystem
                "03.962.178/0002-92", // Guaiba
                "78.766.151/0001-42", // Soow Sigma
                "07.395.076/0001-02", // RKF Nobreaks
                "47.960.950/0897-85"  // Teste Magazine Luiza
            }
        },

        {
            TipoLayout.NFSeMunicipal,
            new List<string>
            {
                "13.551.747/0001-80", // Engine
                "17.682.570/0001-01", // Ploomes
                "00.910.509/0001-71", // Thonsom Reuters
                "67.212.506/0001-35"  // Unisolution
            }
        },

        {
            TipoLayout.NFSeMunicipalVariacao,
            new List<string>
            {
                "21.278.305/0001-30", // Loginfo
            }
        }
    };

    public TipoLayout ObterLayout(string cnpj)
    {
        foreach (var layout in _layouts)
        {
            if (layout.Value.Contains(cnpj))
            {
                return layout.Key;
            }
        }

        return TipoLayout.Desconhecido;
    }
}    