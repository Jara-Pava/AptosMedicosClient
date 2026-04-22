using System.Collections.Generic;

namespace DataAccessDesarrollos.Interfaces
{
    public interface IModuloCatalogo
    {
        List<ModuloCatalogo> ObtenerTodos();
        bool Crear(ModuloCatalogo item);
        bool Actualizar(ModuloCatalogo item);
        void Eliminar(int id);
        bool ExisteConNombre(string nombre, int? idExcluir = null);
    }
}