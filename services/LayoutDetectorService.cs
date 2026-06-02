using CrudApi.Enums;
using CrudApi.Interfaces;
using CrudApi.Repositories;

namespace CrudApi.Services;

public class LayoutDetectorService
{
    private readonly CnpjReaderService _cnpjReader = new();
    private readonly FornecedorRepository _fornecedorRepository = new();
    private readonly DetectorRegistryService _detectorRegistry = new();

    // Detect flow: 1) extract CNPJ -> identify fornecedor; 2) run fornecedor-specific detectors; 3) fall back to global detectors; 4) last resort: fornecedor.Layout
    public TipoLayout Detectar(string texto)
    {
        var cnpjs = _cnpjReader.ExtrairCnpjs(texto);
        Console.WriteLine($"LayoutDetectorService: CNPJs extraídos: {cnpjs.Count}");

        foreach (var cnpj in cnpjs)
        {
            Console.WriteLine($"LayoutDetectorService: verificando CNPJ: {cnpj}");
            var fornecedor = _fornecedorRepository.BuscarPorCnpj(cnpj);

            if (fornecedor != null)
            {
                Console.WriteLine($"LayoutDetectorService: fornecedor identificado: {fornecedor.Nome}");

                // Run fornecedor-specific detectors
                var detectors = _detectorRegistry.GetDetectorsForFornecedor(cnpj);

                foreach (var detector in detectors)
                {
                    var found = detector.Detectar(texto);
                    if (found != TipoLayout.Desconhecido)
                    {
                        Console.WriteLine($"LayoutDetectorService: layout detectado por detector do fornecedor {fornecedor.Nome}: {found}");
                        return found;
                    }
                }

                // If no detector matched, fall back to fornecedor layout declared in repository (backwards compatibility)
                if (fornecedor.Layout != TipoLayout.Desconhecido)
                {
                    Console.WriteLine($"LayoutDetectorService: usando layout padrão do fornecedor {fornecedor.Nome}: {fornecedor.Layout}");
                    return fornecedor.Layout;
                }
            }
        }

        // If no fornecedor-based detection, try global detectors
        foreach (var detector in _detectorRegistry.GetGlobalDetectors())
        {
            var found = detector.Detectar(texto);
            if (found != TipoLayout.Desconhecido)
            {
                Console.WriteLine($"LayoutDetectorService: layout detectado por detector global: {found}");
                return found;
            }
        }

        Console.WriteLine("LayoutDetectorService: nenhum layout detectado");
        return TipoLayout.Desconhecido;
    }
}