# ApiDocumentos

> README atualizado em 16/06/2026 para refletir os serviços, layouts, parsers e fluxo atual do projeto.

API ASP.NET Core para upload e processamento de documentos fiscais em PDF. O projeto lê arquivos PDF, detecta o layout da nota fiscal, seleciona o parser adequado, extrai dados principais e mantém os documentos processados em memória.

## Visão Geral

- Plataforma: .NET 8.0
- Tipo: API Web ASP.NET Core
- Finalidade: processar PDFs de documentos fiscais, principalmente NF-e/DANFE e NFS-e.
- Armazenamento atual: banco fake em memória (`FakeDb.Documentos`).
- Interface de teste: Swagger em ambiente de desenvolvimento.

## Funcionalidades

- Listar documentos cadastrados.
- Buscar documento por ID.
- Criar documento manualmente via JSON.
- Atualizar documento existente.
- Remover documento.
- Fazer upload de PDF para processamento automático.
- Extrair texto de PDF com iText.
- Extrair CNPJs do texto.
- Detectar layout por fornecedor e por detectores globais.
- Selecionar parser conforme o layout detectado.
- Montar `conteudoExtraido` como objeto JSON com layout, número da nota, fornecedor, CNPJ, valor total, data de emissão e CNPJ do cliente quando encontrado.
- Enviar os dados extraídos para a planilha Excel `Planilha de Teste.xlsx`, tabela `TesteArgus`.

## Estrutura Principal

- `Program.cs` - configura controllers, Swagger e injeção de dependências.
- `Controllers/WeatherForecastController.cs` - controller atual dos endpoints de documentos em `api/documentos`.
- `Data/FakeDb.cs` - armazenamento temporário em memória.
- `Models/` - modelos `Documento`, `DadosNotaFiscal`, `Fornecedor` e `Cliente`.
- `Repositories/` - repositórios em memória de documentos, fornecedores e clientes.
- `Interfaces/` - contratos de parsers e detectores de layout.
- `Enums/TipoLayout.cs` - enum com os layouts suportados.
- `services/` - serviços de leitura de PDF, detecção de layout, parsing e processamento.
- `Uploads/` - pasta onde os PDFs enviados são salvos.

## Serviços

- `PdfService` - lê texto de arquivos PDF.
- `CnpjReaderService` - extrai CNPJs do texto.
- `LayoutDetectorService` - orquestra a detecção de layout.
- `DetectorRegistryService` - registra detectores globais e detectores específicos por fornecedor.
- `GenericLayoutDetector` - detector genérico usado como fallback.
- `WorkSystemLayoutDetector` - detector específico para documentos WorkSystem.
- `PloomesLayoutDetector` - detector específico para documentos Ploomes.
- `ParserRegistryService` - seleciona o parser conforme o `TipoLayout`.
- `NotaFiscalProcessingService` - coordena leitura, detecção, parsing, enriquecimento e criação do `Documento`.
- `ExcelQueue` - fila em memória para desacoplar a requisição HTTP da escrita no Excel.
- `ExcelBackgroundService` - processa a fila do Excel em segundo plano.
- `ExcelService` - adiciona os dados extraídos na planilha Excel sem sobrescrever registros existentes e registra logs de diagnóstico da gravação.
- `NotaFiscalParserBase` - classe base com lógica comum para parsers.
- Parsers atuais:
  - `DanfeProdutoModernoParser`
  - `DanfeProdutoAntigoParser`
  - `DanfePadraoModernoParser`
  - `NFSeMunicipalParser`

## Layouts Suportados

Os layouts registrados em `TipoLayout` são:

- `Desconhecido`
- `DanfePadraoModerno`
- `DanfeProdutoModerno`
- `DanfeProdutoAntigo`
- `NFSeMunicipal`
- `NFSeMunicipalVariacao`

O `ParserRegistryService` mapeia os layouts para parsers específicos. Atualmente, `NFSeMunicipalVariacao` usa o mesmo parser de `NFSeMunicipal`.

## Dependências

- `ClosedXML` 0.104.2 - leitura e escrita em planilhas Excel.
- `itext7` 9.6.0 - leitura e extração de texto de PDF.
- `Newtonsoft.Json` 13.0.4 - serialização JSON.
- `Swashbuckle.AspNetCore` 6.6.2 - Swagger/OpenAPI.

## Endpoints

Base: `https://localhost:{porta}/api/documentos`

- `GET /api/documentos` - lista todos os documentos.
- `GET /api/documentos/{id}` - busca documento por ID.
- `POST /api/documentos` - cria documento manualmente com corpo JSON.
- `PUT /api/documentos/{id}` - atualiza documento.
- `DELETE /api/documentos/{id}` - remove documento.
- `POST /api/documentos/upload` - envia PDF para processamento automático.

## Modelo de Documento

`Documento`

- `Id` - identificador numérico.
- `nomeArquivo` - nome do arquivo enviado.
- `Tipo` - tipo do documento, hoje preenchido como `NF` no processamento automático.
- `ConteudoExtraido` - objeto JSON estruturado com os dados extraídos da nota fiscal.
- `DataUpload` - data e hora do cadastro/upload.

## Fluxo de Upload

1. O cliente envia um PDF em `POST /api/documentos/upload` usando `multipart/form-data`.
2. O controller valida o arquivo.
3. O arquivo é salvo em `Uploads/`.
4. O `NotaFiscalProcessingService` lê o PDF com `PdfService`.
5. O texto é analisado para extrair CNPJs.
6. O `LayoutDetectorService` tenta identificar o layout usando detectores específicos do fornecedor, layout cadastrado no repositório e detectores globais.
7. O `ParserRegistryService` seleciona o parser correspondente.
8. Os dados principais da nota são extraídos.
9. O serviço tenta enriquecer os dados com fornecedor e cliente cadastrados.
10. Os dados extraídos são enviados para a fila do Excel.
11. O `ExcelBackgroundService` processa a fila em segundo plano.
12. O `ExcelService` adiciona os dados na próxima linha disponível da tabela `TesteArgus`.
13. Um novo `Documento` é salvo em `FakeDb.Documentos` por meio de `DocumentoRepository`.

Exemplo de `conteudoExtraido` retornado pela API:

```json
{
  "layout": "DanfePadraoModerno",
  "numeroNota": "12345",
  "dataEmissao": "01/06/2026",
  "nomeFornecedor": "Fornecedor Exemplo",
  "cnpjFornecedor": "00.000.000/0001-00",
  "cnpjCliente": "11.111.111/0001-11",
  "valorTotal": "1500,00"
}
```

## Integração com Excel

A escrita em Excel fica isolada atrás da interface `IExcelService`, permitindo trocar o `FakeDb` por banco de dados sem alterar a lógica da planilha. As requisições HTTP não gravam diretamente no arquivo: elas enfileiram os dados em `IExcelQueue`, e o `ExcelBackgroundService` realiza a gravação em segundo plano.

Configuração atual em `appsettings.json`:

```json
{
  "Excel": {
    "CaminhoArquivo": "C:\\Users\\christian.leite\\OneDrive - Rocha Terminais Portuários e Logística S.A\\Documentos\\Argus\\Planilha de Teste.xlsx",
    "NomeTabela": "TesteArgus",
    "NomeAba": "TesteArgus"
  }
}
```

Mapeamento centralizado em `ExcelService`:

- `Layout` -> `Layout`
- `NumeroNota` -> `NF`
- `DataEmissao` -> `Data de Emissão`
- `NomeFornecedor` -> `Fornecedor`
- `CnpjFornecedor` -> `CNPJ Fornecedor`
- `CnpjCliente` -> `CNPJ Rocha`
- `ValorTotal` -> `Valor`

O serviço cria o arquivo caso ele não exista, valida cabeçalhos quando a tabela já existe, usa controle de concorrência com `SemaphoreSlim` e aplica retry em falhas de escrita comuns, como arquivo aberto ou bloqueado.

Logs relacionados ao Excel:

- `WeatherForecastController` registra quando uma criação manual ou upload envia dados para a fila da planilha e quando a operação termina.
- `ExcelQueue` registra cada nota enfileirada.
- `ExcelBackgroundService` registra o início do worker, o processamento de cada item e falhas da fila.
- `ExcelService` registra a solicitação de gravação, abertura ou criação do arquivo, criação/localização da tabela, linha preparada, sucesso da gravação, espera/liberação do lock em nível `Debug`, tentativas de retry em `Warning` e falha definitiva em `Error`.
- Os logs usam campos estruturados como `NumeroNota`, `Fornecedor`, `ValorTotal`, `CaminhoArquivo`, `NomeAba`, `NomeTabela` e `Linha`, facilitando filtro no console ou em ferramentas de observabilidade.

Para uma evolução futura, o ideal é mover a escrita no Excel para um `BackgroundService`: a API gravaria o documento no banco e publicaria uma pendência de sincronização em uma fila/tabela; o job processaria itens pendentes, aplicaria controle de redundância por chave natural, como `CNPJ Fornecedor + NF + Valor`, e marcaria cada item como sincronizado ou com erro.

## Como Executar

1. Abra o terminal na pasta do projeto:

```bash
cd "C:\Users\christian.leite\OneDrive - Rocha Terminais Portuários e Logística S.A\Documentos\Argus\ApiDocumentos"
```

2. Restaure as dependências:

```bash
dotnet restore
```

3. Compile:

```bash
dotnet build
```

4. Execute:

```bash
dotnet run
```

5. Acesse o Swagger em ambiente de desenvolvimento:

```text
https://localhost:{porta}/swagger
```

## Exemplos de Uso

Upload de PDF:

```bash
curl -X POST "https://localhost:{porta}/api/documentos/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@caminho/para/arquivo.pdf"
```

Criação manual de documento:

```bash
curl -X POST "https://localhost:{porta}/api/documentos" \
  -H "Content-Type: application/json" \
  -d '{"nomeArquivo":"teste.pdf","tipo":"NF","conteudoExtraido":{"layout":"DanfePadraoModerno","numeroNota":"12345","dataEmissao":"01/06/2026","nomeFornecedor":"Fornecedor Exemplo","cnpjFornecedor":"00.000.000/0001-00","cnpjCliente":"11.111.111/0001-11","valorTotal":"1500,00"}}'
```

## Como Adicionar Novo Layout

1. Adicione o novo valor em `Enums/TipoLayout.cs`.
2. Crie um parser em `services/` implementando `INotaFiscalParser` ou herdando de `NotaFiscalParserBase`.
3. Registre o parser em `ParserRegistryService`.
4. Se necessário, crie um detector implementando `ILayoutDetector`.
5. Registre o detector em `DetectorRegistryService`.
6. Cadastre o fornecedor e o layout em `FornecedorRepository`.

## Observações

- Os documentos são armazenados apenas em memória e são perdidos ao reiniciar a aplicação.
- A pasta `Uploads/` é criada automaticamente quando necessário.
- A detecção depende dos CNPJs encontrados no PDF e de regras simples de texto.
- Layouts não reconhecidos retornam `TipoLayout.Desconhecido`; nesse caso, os dados extraídos podem ficar vazios ou incompletos.
- O controller principal ainda se chama `WeatherForecastController.cs`, embora exponha endpoints de documentos.
- O projeto compila atualmente com `dotnet build`.

## Melhorias Possíveis

- Persistência real em banco de dados.
- Renomear `WeatherForecastController.cs` para um nome alinhado ao domínio, como `DocumentosController.cs`.
- Validação mais robusta dos arquivos enviados.
- Tratamento de erros mais detalhado no processamento.
- Testes automatizados para detectores e parsers.
- Documentação OpenAPI mais completa com exemplos de request/response.
