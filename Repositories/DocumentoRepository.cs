using CrudApi.Data;
using CrudApi.Models;

namespace CrudApi.Repositories;

public class DocumentoRepository
{
    public IReadOnlyList<Documento> ListarTodos()
    {
        return FakeDb.Documentos;
    }

    public Documento? BuscarPorId(int id)
    {
        return FakeDb.Documentos.FirstOrDefault(x => x.Id == id);
    }

    public Documento Adicionar(Documento documento)
    {
        documento.Id = ObterProximoId();
        documento.DataUpload = DateTime.Now;

        FakeDb.Documentos.Add(documento);

        return documento;
    }

    public Documento? Atualizar(int id, Documento atualizado)
    {
        var documento = BuscarPorId(id);

        if (documento == null)
        {
            return null;
        }

        documento.nomeArquivo = atualizado.nomeArquivo;
        documento.Tipo = atualizado.Tipo;
        documento.ConteudoExtraido = atualizado.ConteudoExtraido;

        return documento;
    }

    public bool Remover(int id)
    {
        var documento = BuscarPorId(id);

        if (documento == null)
        {
            return false;
        }

        FakeDb.Documentos.Remove(documento);

        return true;
    }

    private static int ObterProximoId()
    {
        if (FakeDb.Documentos.Count == 0)
        {
            return 1;
        }

        return FakeDb.Documentos.Max(x => x.Id) + 1;
    }
}
