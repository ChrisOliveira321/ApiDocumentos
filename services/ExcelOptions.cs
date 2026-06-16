namespace CrudApi.Services;

public class ExcelOptions
{
    public string CaminhoArquivo { get; set; } = Path.Combine(
        Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? Directory.GetCurrentDirectory(),
        "Planilha de Teste.xlsx");

    public string NomeTabela { get; set; } = "TesteArgus";

    public string NomeAba { get; set; } = "TesteArgus";
}
