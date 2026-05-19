using CrudApi.Enums;

namespace CrudApi.Services;

public class LayoutDetectorService
{
    private readonly CnpjReaderService _cnpjReader = new();
    private readonly LayoutRegistryService _layoutRegistry = new();

    public TipoLayout Detectar(string texto)
    {
        var cnpjs = _cnpjReader.ExtrairCnpjs(texto);
        Console.WriteLine($"LayoutDetectorService: CNPJs extraídos: {cnpjs.Count}");

        foreach (var cnpj in cnpjs)
        {
            Console.WriteLine($"LayoutDetectorService: verificando CNPJ: {cnpj}");
            var layout = _layoutRegistry.ObterLayout(cnpj);
            Console.WriteLine($"LayoutDetectorService: layout encontrado para {cnpj}: {layout}");

            if (layout != TipoLayout.Desconhecido)
            {
                return layout;
            }
        }

        Console.WriteLine("LayoutDetectorService: nenhum layout detectado");
        return TipoLayout.Desconhecido;
    }
}