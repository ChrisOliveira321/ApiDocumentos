using CrudApi.Models;
using CrudApi.Enums;

namespace CrudApi.Repositories;

public class FornecedorRepository
{
    private readonly List<Fornecedor> _fornecedores = new()
    {
        // DANFE PRODUTO ANTIGO 
        new Fornecedor
        {
            Nome = "WorkSystem",
            Cnpj = "59.716.660/0001-16",
            Layout = TipoLayout.DanfeProdutoAntigo
        },

        // DANFE PADRÃO MODERNO
        new Fornecedor
        {
            Nome = "Ingram",
            Cnpj = "01.771.935/0002-15",
            Layout = TipoLayout.DanfePadraoModerno
        },

        new Fornecedor
        {
            Nome = "Inside Core",
            Cnpj = "51.814.736/0001-34",
            Layout = TipoLayout.DanfePadraoModerno
        },

        new Fornecedor
        {
            Nome = "WorkSystem",
            Cnpj = "59.716.660/0001-16",
            Layout = TipoLayout.DanfePadraoModerno
        },

        new Fornecedor
        {
            Nome = "Guaiba",
            Cnpj = "03.962.178/0002-92",
            Layout = TipoLayout.DanfePadraoModerno
        },

        new Fornecedor
        {
            Nome = "Soow Sigma",
            Cnpj = "78.766.151/0001-42",
            Layout = TipoLayout.DanfePadraoModerno
        },

        new Fornecedor
        {
            Nome = "RKF Nobreaks",
            Cnpj = "07.395.076/0001-02",
            Layout = TipoLayout.DanfePadraoModerno
        },

        new Fornecedor
        {
            Nome = "Magazine Luiza",
            Cnpj = "47.960.950/0897-85",
            Layout = TipoLayout.DanfePadraoModerno
        },

        // NFS-e MUNICIPAL
        new Fornecedor
        {
            Nome = "TD Power",
            Cnpj = "07.776.469/0001-66",
            Layout = TipoLayout.NFSeMunicipal
        },

        new Fornecedor
        {
            Nome = "Engine",
            Cnpj = "13.551.747/0001-80",
            Layout = TipoLayout.NFSeMunicipal
        },

        new Fornecedor
        {
            Nome = "Ploomes",
            Cnpj = "17.682.570/0001-01",
            Layout = TipoLayout.NFSeMunicipal
        },

        new Fornecedor
        {
            Nome = "Thomson Reuters",
            Cnpj = "00.910.509/0001-71",
            Layout = TipoLayout.NFSeMunicipal
        },

        new Fornecedor
        {
            Nome = "Unisolution",
            Cnpj = "67.212.506/0001-35",
            Layout = TipoLayout.NFSeMunicipal
        },

        // NFS-e MUNICIPAL VARIAÇÃO
        new Fornecedor
        {
            Nome = "Loginfo",
            Cnpj = "21.278.305/0001-30",
            Layout = TipoLayout.NFSeMunicipalVariacao
        }
    };

    public Fornecedor BuscarPorCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj)) return null;

        var exactMatch = _fornecedores
            .FirstOrDefault(f => f.Cnpj == cnpj);

        if (exactMatch != null)
        {
            return exactMatch;
        }

        static string Normalize(string value) => new string((value ?? string.Empty).Where(char.IsDigit).ToArray());

        var search = Normalize(cnpj);

        return _fornecedores
            .FirstOrDefault(f => Normalize(f.Cnpj) == search);
    }
}