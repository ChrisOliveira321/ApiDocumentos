using CrudApi.Repositories;
using CrudApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<LayoutDetectorService>();
builder.Services.AddSingleton<CnpjReaderService>();
builder.Services.AddSingleton<FornecedorRepository>();
builder.Services.AddSingleton<ClienteRepository>();
builder.Services.AddSingleton<ParserRegistryService>();
builder.Services.AddSingleton<NotaFiscalProcessingService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

internal class DocumentoProcessingService
{
}