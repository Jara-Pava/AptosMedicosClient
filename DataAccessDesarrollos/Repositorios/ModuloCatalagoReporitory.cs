using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataAccessDesarrollos.Interfaces;

namespace DataAccessDesarrollos.Repositorios
{
    public class ModuloCatalogoRepository : IModuloCatalogo
    {
        public List<ModuloCatalogo> ObtenerTodos()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetAllModuloCatalogo", rdr => new ModuloCatalogo
                    {
                        id_modulo_catalogo = Convert.ToInt32(rdr["id_modulo_catalogo"]),
                        nombre = rdr["nombre"].ToString(),
                        descripcion = rdr["descripcion"] != DBNull.Value ? rdr["descripcion"].ToString() : null,
                        activo = Convert.ToBoolean(rdr["activo"]),
                        fecha_creacion = Convert.ToDateTime(rdr["fecha_creacion"]),
                        creado_por = rdr["creado_por"] != DBNull.Value ? Convert.ToInt32(rdr["creado_por"]) : (int?)null,
                        nombre_creador = rdr["nombre_creador"] != DBNull.Value ? rdr["nombre_creador"].ToString() : null
                    }).ToList();
                    return lista ?? new List<ModuloCatalogo>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerTodos ModuloCatalogo: {0}", ex);
                return new List<ModuloCatalogo>();
            }
        }

        public bool Crear(ModuloCatalogo item)
        {
            if (item == null) return false;

            try
            {
                using (var da = new DataAccess())
                {
                    var result = da.ExecuteScalarByCode("rhsp_InsertModuloCatalogo", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@nombre", item.nombre ?? string.Empty);
                        cmd.Parameters.AddWithValue("@descripcion", (object)item.descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@creado_por", (object)item.creado_por ?? DBNull.Value);
                    });
                    return result != null && result != DBNull.Value;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error Crear ModuloCatalogo: {0}", ex);
                return false;
            }
        }

        public bool Actualizar(ModuloCatalogo item)
        {
            if (item == null || item.id_modulo_catalogo <= 0) return false;

            try
            {
                using (var da = new DataAccess())
                {
                    int rows = da.ExecuteNonQueryByCode("rhsp_UpdateModuloCatalogo", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@id_modulo_catalogo", item.id_modulo_catalogo);
                        cmd.Parameters.AddWithValue("@nombre", item.nombre ?? string.Empty);
                        cmd.Parameters.AddWithValue("@descripcion", (object)item.descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@activo", item.activo);
                    });
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error Actualizar ModuloCatalogo: {0}", ex);
                return false;
            }
        }

        public void Eliminar(int id)
        {
            if (id <= 0) return;

            try
            {
                using (var da = new DataAccess())
                {
                    da.ExecuteNonQueryByCode("rhsp_DeleteModuloCatalogo", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@id_modulo_catalogo", id);
                    });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error Eliminar ModuloCatalogo: {0}", ex);
                throw;
            }
        }

        public bool ExisteConNombre(string nombre, int? idExcluir = null)
        {
            try
            {
                var todos = ObtenerTodos();
                return todos.Any(m =>
                    m.nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                    (!idExcluir.HasValue || m.id_modulo_catalogo != idExcluir.Value));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ExisteConNombre ModuloCatalogo: {0}", ex);
                return false;
            }
        }
    }
}