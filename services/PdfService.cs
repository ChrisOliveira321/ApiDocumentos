using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace CrudApi.Services;

public class PdfService
{
    public string LerPdf(string caminho)
    {
        using var reader = new PdfReader(caminho);
        using var pdf = new PdfDocument(reader);

        string texto = "";

        for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
        {
            texto += PdfTextExtractor.GetTextFromPage(pdf.GetPage(i));
        }

        return texto;
    }
}