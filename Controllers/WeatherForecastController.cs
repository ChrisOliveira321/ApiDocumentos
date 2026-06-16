using CrudApi.Interfaces;
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
    private readonly IExcelService _excelService;
    private readonly NotaFiscalProcessingService _notaFiscalProcessingService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        DocumentoRepository documentoRepository,
        IExcelService excelService,
        NotaFiscalProcessingService notaFiscalProcessingService,
        ILogger<WeatherForecastController> logger)
    {
        _documentoRepository = documentoRepository;
        _excelService = excelService;
        _notaFiscalProcessingService = notaFiscalProcessingService;
        _logger = logger;
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
    public async Task<IActionResult> Create(Documento documento)
    {
        if (documento.ConteudoExtraido == null)
        {
            _logger.LogWarning("Criacao manual de documento recusada porque ConteudoExtraido nao foi informado.");
            return BadRequest("ConteudoExtraido e obrigatorio para gravacao no Excel.");
        }

        _logger.LogInformation(
            "Recebida solicitacao de criacao manual de documento com gravacao no Excel. Arquivo: {NomeArquivo}; NF: {NumeroNota}.",
            documento.nomeArquivo,
            documento.ConteudoExtraido.NumeroNota);

        await _excelService.AdicionarNotaAsync(documento.ConteudoExtraido);

        var criado = _documentoRepository.Adicionar(documento);
        _logger.LogInformation(
            "Documento manual criado e enviado ao Excel. DocumentoId: {DocumentoId}; Arquivo: {NomeArquivo}.",
            criado.Id,
            criado.nomeArquivo);

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

        _logger.LogInformation(
            "Documento processado sera enviado ao Excel. Arquivo: {NomeArquivo}; NF: {NumeroNota}; Layout: {Layout}.",
            file.FileName,
            documentoProcessado.ConteudoExtraido.NumeroNota,
            documentoProcessado.ConteudoExtraido.Layout);

        await _excelService.AdicionarNotaAsync(documentoProcessado.ConteudoExtraido);

        var documentoSalvo = _documentoRepository.Adicionar(documentoProcessado);

        _logger.LogInformation(
            "Upload processado, gravado no Excel e salvo em memoria. DocumentoId: {DocumentoId}; Arquivo: {NomeArquivo}.",
            documentoSalvo.Id,
            documentoSalvo.nomeArquivo);

        return Ok(documentoSalvo);
    }
}
