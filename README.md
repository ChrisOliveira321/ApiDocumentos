# ApiDocumentos

API ASP.NET Core para upload e processamento de documentos fiscais (PDF). O projeto lê PDF, detecta layout, extrai dados de nota fiscal e armazena documentos em memória.

## Visão geral

- Plataforma: .NET 8.0
- Tipo: API Web ASP.NET Core
- Função principal: processar PDF de nota fiscal, detectar layout e extrair dados chave como número, fornecedor, valor e data de emissão.
- Armazenamento temporário: banco de dados fake em memória (`FakeDb.Documentos`).
- Suporte a uploads via POST e endpoints CRUD para documentos.

## Estrutura principal

- `Program.cs` - configuração da aplicação e injeção de dependências.
- `Controllers/WeatherForecastController.cs` - controlador que expõe endpoints CRUD e upload.
- `services/` - serviços responsáveis por leitura de PDF, detecção de layout, extração de CNPJ e processamento de nota fiscal.
- `Repositories/` - repositórios em memória para fornecedores e clientes.
- `Models/` - modelos de dados para `Documento`, `DadosNotaFiscal`, `Fornecedor` e `Cliente`.
- `Data/FakeDb.cs` - armazenamento em memória para documentos.

## Dependências

- `itext7` - leitura e extração de texto de PDF
- `Newtonsoft.Json` - serialização JSON
- `Swashbuckle.AspNetCore` - geração de Swagger/OpenAPI

## Funcionalidades

- Listar documentos cadastrados
- Obter documento por ID
- Criar documento manualmente
- Atualizar documento
- Remover documento
- Upload de arquivo PDF e processamento automático
- Extração de CNPJ, detecção de layout e seleção de parser
- Montagem de conteúdo extraído em texto resumido

## Endpoints

Base: `https://localhost:{porta}/api/documentos`

- `GET /api/documentos` - lista todos os documentos
- `GET /api/documentos/{id}` - busca documento por ID
- `POST /api/documentos` - cria documento manualmente com corpo JSON
- `PUT /api/documentos/{id}` - atualiza documento
- `DELETE /api/documentos/{id}` - remove documento
- `POST /api/documentos/upload` - envia PDF para processamento

## Modelo de documento

`Documento`

- `Id` - identificador numérico
- `nomeArquivo` - nome do arquivo enviado
- `Tipo` - tipo do documento (ex: `NF`)
- `ConteudoExtraido` - texto gerado com dados extraídos
- `DataUpload` - data de upload

## Uso

1. Restaurar dependências e compilar:

```bash
cd c:\Users\christian.leite\Documents\Argus\ApiDocumentos
dotnet restore
dotnet build
```

2. Iniciar a aplicação:

```bash
dotnet run
```

3. Acessar Swagger (em ambiente de desenvolvimento):

- `https://localhost:{porta}/swagger`

4. Exemplo de upload via `curl`:

```bash
curl -X POST "https://localhost:{porta}/api/documentos/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@caminho/para/arquivo.pdf"
```

5. Exemplo de criação manual de documento:

```bash
curl -X POST "https://localhost:{porta}/api/documentos" \
  -H "Content-Type: application/json" \
  -d '{"nomeArquivo":"teste.pdf","Tipo":"NF","ConteudoExtraido":"Conteudo de exemplo"}'
```

## Observações

- Os documentos são armazenados em memória e não persistem após reiniciar a aplicação.
- A pasta `Uploads` é criada automaticamente quando um arquivo é enviado.
- A detecção de layout usa CNPJs encontrados no texto para escolher o parser apropriado.
- O projeto já contém fornecedores e clientes de exemplo no repositório em memória.

## Melhorias possíveis

- Persistência real em banco de dados
- Validação avançada do PDF e do conteúdo extraído
- Tratamento de erros mais robusto
- Suporte a mais layouts de nota fiscal
- Documentação OpenAPI completa
