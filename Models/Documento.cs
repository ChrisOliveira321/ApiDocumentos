namespace CrudApi.Models;

public class Documento
{
    public int Id { get; set; }
    public string nomeArquivo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DadosNotaFiscal ConteudoExtraido { get; set; } = new();
    public DateTime DataUpload { get; set; } 

}
