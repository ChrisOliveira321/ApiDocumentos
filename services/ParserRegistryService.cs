using CrudApi.Enums;
using CrudApi.Interfaces;

namespace CrudApi.Services;

public class ParserRegistryService
{
    private readonly Dictionary<TipoLayout, INotaFiscalParser> _parsers = new()
    {
        { TipoLayout.DanfePadraoModerno, new DanfePadraoModernoParser() },
        { TipoLayout.NFSeMunicipal, new NFSeMunicipalParser() },
        { TipoLayout.NFSeMunicipalVariacao, new NFSeMunicipalParser() }
    };

    public INotaFiscalParser ObterParser(TipoLayout layout)
    {
        if (_parsers.ContainsKey(layout))
        {
            return _parsers[layout];
        }

        return null;
    }
}