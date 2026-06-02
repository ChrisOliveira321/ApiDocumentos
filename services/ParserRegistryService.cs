using CrudApi.Enums;
using CrudApi.Interfaces;

namespace CrudApi.Services;

public class ParserRegistryService
{
    private readonly Dictionary<TipoLayout, INotaFiscalParser> _parsers = new()
    {
        { TipoLayout.DanfeProdutoModerno, new DanfeProdutoModernoParser() },
        { TipoLayout.DanfeProduto, new DanfeProdutoParser() },
        { TipoLayout.DanfePadraoModerno, new DanfePadraoModernoParser() },
        { TipoLayout.NFSeMunicipal, new NFSeMunicipalParser() },
        { TipoLayout.NFSeMunicipalVariacao, new NFSeMunicipalParser() }
    };

    public INotaFiscalParser ObterParser(TipoLayout layout)
    {
        if (_parsers.ContainsKey(layout))
        {
            Console.WriteLine($"ParserRegistryService: parser encontrado para layout {layout}");
            return _parsers[layout];
        }

        Console.WriteLine($"ParserRegistryService: parser não encontrado para layout {layout}");
        return null;
    }
}