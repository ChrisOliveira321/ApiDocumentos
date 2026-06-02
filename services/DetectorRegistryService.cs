using CrudApi.Interfaces;
using System.Linq;

namespace CrudApi.Services;

public class DetectorRegistryService
{
    private readonly Dictionary<string, List<ILayoutDetector>> _byFornecedor = new();
    private readonly List<ILayoutDetector> _globalDetectors = new();

    public DetectorRegistryService()
    {
        // Registre detectores globais
        _globalDetectors.Add(new GenericLayoutDetector());

        // Registre detectores por fornecedor (normalize CNPJ digits only)
        // WorkSystem
        _byFornecedor[Normalize("59.716.660/0001-16")] = new List<ILayoutDetector>
        {
            new WorkSystemLayoutDetector()
        };

        // Ploomes
        _byFornecedor[Normalize("17.682.570/0001-01")] = new List<ILayoutDetector>
        {
            new PloomesLayoutDetector()
        };

        // Loginfo
        _byFornecedor[Normalize("21.278.305/0001-30")] = new List<ILayoutDetector>
        {
            new GenericLayoutDetector()
        };
    }

    public IEnumerable<ILayoutDetector> GetDetectorsForFornecedor(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj)) return Enumerable.Empty<ILayoutDetector>();

        var key = Normalize(cnpj);

        if (_byFornecedor.ContainsKey(key))
        {
            return _byFornecedor[key];
        }

        return Enumerable.Empty<ILayoutDetector>();
    }

    public IEnumerable<ILayoutDetector> GetGlobalDetectors() => _globalDetectors;

    private static string Normalize(string value) => new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
}
