using CrudApi.Data;
using CrudApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace CrudApi.Controllers;

[ApiController]
[Route("api/documentos")]
public class WeatherForecastController : ControllerBase
{
    // GET - Listar todos   
    [HttpGet]
    public IActionResult getAll()
    {
        return Ok(FakeDb.Documentos);
    }

    // GET - Buscar por ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var doc = FakeDb.Documentos.FirstOrDefault(x => x.Id == id);

        if (doc == null)
            return NotFound();

        return Ok(doc);
    }

    // POST - Criar 
    [HttpPost]
    public IActionResult Create(Documento doc)
    {
        doc.Id = FakeDb.Documentos.Count + 1;
        doc.DataUpload = DateTime.Now;

        FakeDb.Documentos.Add(doc);
        
        return Ok(doc);
    }

    // PUT - Atualizar
    [HttpPut("{id}")]
    public IActionResult Update(int id, Documento updated)
    {
        var doc = FakeDb.Documentos.FirstOrDefault(x => x.Id == id);

        if (doc == null) 
            return NotFound();

        doc.nomeArquivo = updated.nomeArquivo;
        doc.Tipo = updated.Tipo;
        doc.ConteudoExtraido = updated.ConteudoExtraido;

        return Ok(doc);
    }

    // DELETE 
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var doc = FakeDb.Documentos.FirstOrDefault(x => x.Id == id);

        if (doc == null) 
            return NotFound();

        FakeDb.Documentos.Remove(doc);

        return NoContent();
    }

    //
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        // 1. Validação 
        if (file == null || file.Length == 0)
            return BadRequest("Arquivo inválido");

        // 2. Pasta onde vai salvar 
        var pasta = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(pasta)) 
            Directory.CreateDirectory(pasta);

        // 3. Caminho final 
        var caminho = Path.Combine(pasta, file.FileName);

        // 4.Salvar arquivo 
        using (var stream = new FileStream(caminho, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }          

        // 5. Usar services
        var pdfService = new PdfService();
        var extracaoService = new ExtracaoService();

        var texto = pdfService.LerPdf(caminho);
        Console.WriteLine(texto);
        var dados = extracaoService.ExtrairDados(texto);

        // 6. Criar objeto 
        var doc = new Documento
        {


            Id = FakeDb.Documentos.Count + 1,
            nomeArquivo = file.FileName,
            Tipo = "NF",
            ConteudoExtraido = 
            $"CNPJ Emitente: {dados.DocumentoEmitente} | CNPJ Destinatário: {dados.DocumentoDestinatario} | Valor: {dados.valorTotal}",
            DataUpload = DateTime.Now
        };

        // 7. Salvar 
        FakeDb.Documentos.Add(doc);

        // 8. Retornar resposta 
        return Ok(doc);
    }

}