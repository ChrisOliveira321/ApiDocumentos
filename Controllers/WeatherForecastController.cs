using CrudApi.Models;
using CrudApi.Repositories;
using CrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudApi.Controllers;

[ApiController]
[Route("api/documentos")]
public class WeatherForecastController : ControllerBase
{
    private readonly DocumentoRepository _documentoRepository;
    private readonly NotaFiscalProcessingService _notaFiscalProcessingService;

    public WeatherForecastController(
        DocumentoRepository documentoRepository,
        NotaFiscalProcessingService notaFiscalProcessingService)
    {
        _documentoRepository = documentoRepository;
        _notaFiscalProcessingService = notaFiscalProcessingService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_documentoRepository.ListarTodos());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var documento = _documentoRepository.BuscarPorId(id);

        if (documento == null)
        {
            return NotFound();
        }

        return Ok(documento);
    }

    [HttpPost]
    public IActionResult Create(Documento documento)
    {
        var criado = _documentoRepository.Adicionar(documento);
        return Ok(criado);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Documento atualizado)
    {
        var documento = _documentoRepository.Atualizar(id, atualizado);

        if (documento == null)
        {
            return NotFound();
        }

        return Ok(documento);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var removido = _documentoRepository.Remover(id);

        if (!removido)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Arquivo inválido");
        }

        var pasta = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(pasta))
        {
            Directory.CreateDirectory(pasta);
        }

        var caminho = Path.Combine(pasta, file.FileName);

        using (var stream = new FileStream(caminho, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var documentoProcessado = _notaFiscalProcessingService.ProcessarDocumento(caminho, file.FileName);
        var documentoSalvo = _documentoRepository.Adicionar(documentoProcessado);

        return Ok(documentoSalvo);
    }
}
