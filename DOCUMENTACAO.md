# Documentação do Argus - ApiDocumentos

## 1. Documentação Técnica

### 1.1 Visão Geral
Argus (`ApiDocumentos`) é uma API ASP.NET Core (.NET 8.0) para upload e processamento de documentos fiscais em PDF. A aplicação:
- lê PDF usando `iText7`;
- detecta o layout da nota fiscal a partir de CNPJs extraídos;
- seleciona um parser compatível com o layout;
- extrai dados-chave da nota fiscal;
- registra o resultado em memória em um repositório temporário.

### 1.2 Arquitetura e Componentes Principais

#### `Program.cs`
- Configura os serviços e a injeção de dependências.
- Registra serviços singleton:
  - `PdfService`
  - `LayoutDetectorService`
  - `CnpjReaderService`
  - `FornecedorRepository`
  - `ClienteRepository`
  - `ParserRegistryService`
  - `NotaFiscalProcessingService`
- Habilita Swagger em ambiente de desenvolvimento.

#### `Controllers/WeatherForecastController.cs`
- Expõe endpoints CRUD para `Documento`.
- Exponencial endpoint de upload em `POST /api/documentos/upload`.
- Salva o PDF em `Uploads/` e delega o processamento ao `NotaFiscalProcessingService`.

#### `services/`
- `PdfService`: leitura de texto de arquivos PDF.
- `LayoutDetectorService`: detecta layout usando CNPJs extraídos do conteúdo.
- `CnpjReaderService`: extrai CNPJs do texto com regex.
- `ParserRegistryService`: obtém parser a partir do tipo de layout.
- `NotaFiscalProcessingService`: orquestra leitura, detecção de layout, parsing, enriquecimento e criação do documento.
- `NotaFiscalParserBase`: base para parsers de nota fiscal, com lógica comum de extração de dados.
- `DanfePadraoModernoParser`, `NFSeMunicipalParser`: parsers específicos de layout.

#### `Repositories/`
- `FornecedorRepository`: busca fornecedor por CNPJ e fornece layout esperado.
- `ClienteRepository`: busca cliente por CNPJ.

#### `Models/`
- `Documento`: registro armazenado com `Id`, `nomeArquivo`, `Tipo`, `ConteudoExtraido` e `DataUpload`.
- `DadosNotaFiscal`: dados extraídos da nota fiscal.

#### `Data/FakeDb.cs`
- Lista estática `FakeDb.Documentos` usada como armazenamento temporário.

### 1.3 Tecnologias e Dependências
- .NET 8.0
- ASP.NET Core Web API
- `iText.Kernel` / `iText.Kernel.Pdf.Canvas.Parser` para leitura de PDF
- `Newtonsoft.Json` para serialização JSON
- `Swashbuckle.AspNetCore` para Swagger/OpenAPI

### 1.4 Regras de Negócio Técnicas
- O sistema extrai CNPJs no formato `##.###.###/####-##`.
- A detecção de layout prioriza o primeiro CNPJ reconhecido com layout conhecido.
- Se nenhum layout for detectado, o parser retornará `TipoLayout.Desconhecido` e a extração de dados resultará em campos padrão.
- O conteúdo extraído é montado como texto concatenado dos campos principais.
- Documentos são mantidos em memória e não persistem entre reinicializações.

## 2. Fluxo Funcional

### 2.1 Fluxo de Upload e Processamento
1. O cliente faz `POST /api/documentos/upload` com o arquivo PDF.
2. O controller valida o arquivo.
3. O arquivo é salvo na pasta `Uploads/`.
4. O `NotaFiscalProcessingService` processa o documento:
   - lê o PDF com `PdfService`;
   - extrai CNPJs com `CnpjReaderService`;
   - detecta o layout com `LayoutDetectorService` e `LayoutRegistryService`;
   - obtém o parser adequado com `ParserRegistryService`;
   - extrai os dados da nota com o parser selecionado;
   - preenche dados de fornecedor e cliente usando os repositórios;
   - monta o texto final de `ConteudoExtraido`.
5. O documento criado é salvo em `FakeDb.Documentos`.
6. A API retorna o `Documento` processado.

### 2.2 Endpoints Disponíveis
- `GET /api/documentos` - lista todos os documentos processados.
- `GET /api/documentos/{id}` - retorna um documento específico por ID.
- `POST /api/documentos` - cria um documento manualmente com JSON.
- `PUT /api/documentos/{id}` - atualiza dados de um documento existente.
- `DELETE /api/documentos/{id}` - exclui um documento.
- `POST /api/documentos/upload` - envia PDF para processamento automático.

### 2.3 Comportamento do Parser
- `DanfePadraoModernoParser`: processa notas com layout moderno e extrai número, data, valor e nome do fornecedor.
- `NFSeMunicipalParser`: processa notas de NFS-e municipal e extrai os mesmos dados adaptados ao layout.
- A base `NotaFiscalParserBase` extrai CNPJ e tenta preencher nome do fornecedor usando o repositório, se disponível.

### 2.4 Regras de Enriquecimento de Dados
- Se o parser não fornecer `CnpjFornecedor`, o serviço tenta preencher a partir dos CNPJs encontrados no texto.
- Se houver CNPJ de cliente válido, ele também é associado ao documento.
- O resultado final inclui `Layout`, `Número da NF`, `Nome do Fornecedor`, `CNPJ do Fornecedor`, `Valor Total`, `Data de Emissão` e, quando disponível, `CNPJ do Cliente`.

## 3. Operação e Suporte

### 3.1 Instalação e Execução
1. Abra o terminal no diretório do projeto:
   ```bash
   cd c:\Users\christian.leite\Documents\Argus\ApiDocumentos
   ```
2. Restaure dependências:
   ```bash
   dotnet restore
   ```
3. Compile:
   ```bash
   dotnet build
   ```
4. Execute a aplicação:
   ```bash
   dotnet run
   ```
5. Acesse Swagger para teste e documentação de endpoints em:
   `https://localhost:{porta}/swagger`

### 3.2 Uso de Upload
- Envie o PDF usando `multipart/form-data` em `POST /api/documentos/upload`.
- Exemplo `curl`:
  ```bash
  curl -X POST "https://localhost:{porta}/api/documentos/upload" \
    -H "Content-Type: multipart/form-data" \
    -F "file=@caminho/para/arquivo.pdf"
  ```

### 3.3 Logs e Diagnóstico
- O projeto usa `Console.WriteLine` para registrar passos importantes durante a detecção de layout e parsing.
- Logs relevantes aparecem em:
  - `LayoutDetectorService`
  - `ParserRegistryService`
  - `DanfePadraoModernoParser`
  - `NFSeMunicipalParser`
  - `NotaFiscalProcessingService`
- Mensagens de debug incluem CNPJs encontrados, layout detectado e parser selecionado.

### 3.4 Suporte e Manutenção
- Verifique se a pasta `Uploads/` existe e está acessível pelo processo.
- Atualize os repositórios de fornecedores/cliente em `Repositories/` para suportar novos CNPJs e layouts.
- Para adicionar novos layouts:
  1. Inclua novo valor em `Enums/TipoLayout.cs`.
  2. Crie parser em `services/` implementando `INotaFiscalParser`.
  3. Registre o parser em `ParserRegistryService`.
  4. Adicione o CNPJ/layout no `FornecedorRepository`.
- Para persistência permanente, substitua `FakeDb.Documentos` por banco de dados real e atualize os repositórios.

### 3.5 Limitações Conhecidas
- Armazenamento atual é somente em memória.
- A extração de CNPJ considera apenas o formato com pontos e traço.
- Layouts não mapeados resultam em `TipoLayout.Desconhecido`.
- O parser depende de regexs simples e pode falhar em PDFs de texto não estruturado ou com formatação diferente.

---

Para dúvidas ou ajustes no fluxo, consulte o código-fonte dos serviços em `services/` e os modelos em `Models/`.