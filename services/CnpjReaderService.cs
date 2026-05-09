using System.Text.RegularExpressions;

namespace CrudApi.Services;

public class CnpjReaderService
{
    public List<string> ExtrairCnpjs(string texto)
    {
        var regex = new Regex(@"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}");

        var matches = regex.Matches(texto);

        List<string> cnpjs = new();

        foreach (Match match in matches)
        {
            cnpjs.Add(match.Value);
        }

        return cnpjs;
    }
}