using System.Text.RegularExpressions;
using CrudApi.Enums;

namespace CrudApi.Services; 

public class DanfePadraoModernoParser : INotaFiscalParser
{
    public string ExtrairNumeroNota(string texto)
    {
        var regex = new Regex(@"Número da NFS-e.*?\n(\d+)",
            RegexOptions.Singleline);

        var match = regex.Match(texto);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return "Número não encontrado";
    }
}