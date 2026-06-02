using CrudApi.Enums;

namespace CrudApi.Interfaces;

public interface ILayoutDetector
{
    TipoLayout Detectar(string texto);
}
