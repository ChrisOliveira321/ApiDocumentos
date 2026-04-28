using System.Text.RegularExpressions;

namespace CrudApi.Services;

public class ExtracaoService
{
    public string ExtrairCnpj(string texto)
    {
        var regex = new Regex(@"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}");
        var match = regex.Match(texto);

        if (match.Success) 
            return match.Value;

        return "CNPJ não encontrado";
    }
}