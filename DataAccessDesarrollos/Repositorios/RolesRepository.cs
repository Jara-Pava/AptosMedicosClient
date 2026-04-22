using DataAccessDesarrollos.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Repositorios
{
    public class RolesRepository : IRoles
    {
        /// <summary>
        /// Verifica si ya existe un rol con el mismo nombre, excluyendo el rol actual (para edición).
        /// </summary>
        public bool ExisteRolConNombre(string nombre, int? idRolExcluir = null)
        {
            try
            {
                var roles = ObtenerRoles();
                return roles.Any(r =>
                    r.nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                    (!idRolExcluir.HasValue || r.id_rol != idRolExcluir.Value));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ExisteRolConNombre: {0}", ex);
                return false;
            }
        }

        public bool ActualizarRol(Rol rol)
        {
            if (rol == null || rol.id_rol <= 0) return false;

            try
            {
                using (var da = new DataAccess())
                {
                    int rows = da.ExecuteNonQueryByCode("rhsp_UpdateRol", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@id_rol", rol.id_rol);
                        cmd.Parameters.AddWithValue("@nombre_rol", rol.nombre ?? string.Empty);
                        cmd.Parameters.AddWithValue("@descripcion", (object)rol.descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@activo", rol.activo);
                    });

                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ActualizarRol: {0}", ex);
                return false;
            }
        }

        public bool CrearRol(Rol rol)
        {
            if (rol == null) return false;

            try
            {
                using (var da = new DataAccess())
                {
                    var result = da.ExecuteScalarByCode("rhsp_InsertRol", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@nombre_rol", rol.nombre ?? string.Empty);
                        cmd.Parameters.AddWithValue("@descripcion", (object)rol.descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@creado_por", (object)rol.creado_por ?? DBNull.Value);
                    });

                    return result != null && result != DBNull.Value;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error CrearRol: {0}", ex);
                return false;
            }
        }

        public void EliminarRol(int id)
        {
            if (id <= 0) return;

            try
            {
                using (var da = new DataAccess())
                {
                    da.ExecuteNonQueryByCode("rhsp_DeleteRol", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@id_rol", id);
                    });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error EliminarRol: {0}", ex);
                throw;
            }
        }

        public List<Rol> ObtenerRoles()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetAllRoles", rdr => new Rol
                    {
                        id_rol = Convert.ToInt32(rdr["id_rol"]),
                        nombre = rdr["nombre_rol"].ToString(),
                        descripcion = rdr["descripcion"] != DBNull.Value ? rdr["descripcion"].ToString() : null,
                        activo = Convert.ToBoolean(rdr["activo"]),
                        fecha_creacion = Convert.ToDateTime(rdr["fecha_creacion"]),
                        creado_por = rdr["creado_por"] != DBNull.Value ? Convert.ToInt32(rdr["creado_por"]) : (int?)null,
                        nombre_creador = rdr["nombre_creador"] != DBNull.Value ? rdr["nombre_creador"].ToString() : null
                    }).ToList();
                    return lista ?? new List<Rol>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerRoles: {0}", ex);
                return new List<Rol>();
            }
        }

        public Rol ObtenerRolPorId(int id)
        {
            throw new NotImplementedException();
        }

        public Rol ObtenerRolPorUsuario(int idUsuario)
        {
            throw new NotImplementedException();
        }
    }
}
