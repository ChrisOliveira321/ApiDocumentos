using CrudApi.Enums;

namespace CrudApi.Services;

public class LayoutDetectorService
{
    private readonly CnpjReaderService _cnpjReader = new();
    private readonly LayoutRegistryService _layoutRegistry = new();

    public TipoLayout Detectar(string texto)
    {
        var cnpjs = _cnpjReader.ExtrairCnpjs(texto);

        foreach (var cnpj in cnpjs)
        {
            var layout = _layoutRegistry.ObterLayout(cnpj);

            if (layout != TipoLayout.Desconhecido)
            {
                return layout;
            }
        }

        return TipoLayout.Desconhecido;
    }
}