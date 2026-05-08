using System.Runtime.CompilerServices;
using CrudApi.Enums;
using iText.Layout.Element;

namespace CrudApi.Services;

public class LayoutDetectorService
{
    private readonly ExtracaoService _extracaoservice = new ();
    private readonly LayoutRegistryService _layoutRegistry = new ();

    public TipoLayout Detectar(string texto)
    {
        var dados = _extracaoservice.ExtrairDados(texto);

        var layout = _layoutRegistry.ObterLayout(dados.DocumentoEmitente);

        return layout;
    }
}

