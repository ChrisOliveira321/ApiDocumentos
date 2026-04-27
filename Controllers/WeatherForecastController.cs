using CrudApi.Data;
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

}