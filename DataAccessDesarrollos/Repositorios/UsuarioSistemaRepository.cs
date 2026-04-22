using DataAccessDesarrollos.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace DataAccessDesarrollos.Repositorios
{
    public class UsuarioSistemaRepository : IUsuarioSistema
    {
        private const string SpCode_GetAll = "USUARIOS_GETALL";
        private const string SpCode_Insert = "USUARIOS_INSERT";
        private const string SpCode_Update = "USUARIOS_UPDATE";
        private const string SpCode_Delete = "USUARIOS_DELETE";

        public List<Usuario> ObtenerTodosUsuarios()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("usp_GetUsers", rdr => new Usuario
                    {
                        id_usuario = rdr["id_usuario"] != DBNull.Value ? Convert.ToInt32(rdr["id_usuario"]) : 0,
                        nombre = rdr["nombre"] != DBNull.Value ? rdr["nombre"].ToString() : string.Empty,
                        sigla_red = rdr["sigla_red"] != DBNull.Value ? rdr["sigla_red"].ToString() : string.Empty,
                        activo = rdr["activo"] != DBNull.Value && Convert.ToBoolean(rdr["activo"]),
                        Email = rdr["Email"] != DBNull.Value ? rdr["Email"].ToString() : string.Empty,
                        creado_por = rdr["creado_por"] != DBNull.Value ? rdr["creado_por"].ToString() : string.Empty,
                        modificado_por = rdr["modificado_por"] != DBNull.Value ? rdr["modificado_por"].ToString() : string.Empty
                    });

                    return lista ?? new List<Usuario>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerTodosUsuarios: {0}", ex);
                return new List<Usuario>();
            }
        }

        public bool CrearUsuario(Usuario usuario)
        {
            if (usuario == null) return false;

            try
            {
                using (var da = new DataAccess())
                {
                    int rows = da.ExecuteNonQueryByCode("usp_InsertUsers", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@nombre", usuario.nombre ?? string.Empty);
                        cmd.Parameters.AddWithValue("@sigla_red", usuario.sigla_red ?? string.Empty);
                        cmd.Parameters.AddWithValue("@activo", usuario.activo);
                        cmd.Parameters.AddWithValue("@Email", usuario.Email ?? string.Empty);
                        cmd.Parameters.AddWithValue("@creado_por", (object)usuario.creado_por ?? DBNull.Value);
                    });

                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error CrearUsuario: {0}", ex);
                return false;
            }
        }

        public bool ActualizarUsuario(Usuario usuario)
        {
            if (usuario == null || usuario.id_usuario <= 0) return false;

            try
            {
                using (var da = new DataAccess())
                {
                    int rows = da.ExecuteNonQueryByCode("usp_UpdateUsers", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", usuario.id_usuario);
                        cmd.Parameters.AddWithValue("@nombre", usuario.nombre ?? string.Empty);
                        cmd.Parameters.AddWithValue("@sigla_red", usuario.sigla_red ?? string.Empty);
                        cmd.Parameters.AddWithValue("@activo", usuario.activo);
                        cmd.Parameters.AddWithValue("@Email", usuario.Email ?? string.Empty);
                        cmd.Parameters.AddWithValue("@modificado_por", (object)usuario.modificado_por ?? DBNull.Value);
                    });

                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ActualizarUsuario: {0}", ex);
                return false;
            }
        }

        public bool ExisteUsuario(string siglaRed, string email, out string mensajeError)
        {
            mensajeError = string.Empty;

            try
            {
                var usuarios = ObtenerTodosUsuarios();

                var usuarioConSigla = usuarios.FirstOrDefault(u =>
                    u.sigla_red?.Trim().Equals(siglaRed?.Trim(), StringComparison.OrdinalIgnoreCase) == true);

                var usuarioConEmail = usuarios.FirstOrDefault(u =>
                    u.Email?.Trim().Equals(email?.Trim(), StringComparison.OrdinalIgnoreCase) == true);

                if (usuarioConSigla != null && usuarioConEmail != null &&
                    usuarioConSigla.id_usuario == usuarioConEmail.id_usuario)
                {
                    mensajeError = $"Proceso no exitoso el usuario con la sigla '{siglaRed}' y el email '{email}' ya existen.";
                    return true;
                }

                if (usuarioConSigla != null)
                {
                    mensajeError = $"Proceso no exitoso ya existe un usuario con la sigla de red '{siglaRed}'.";
                    return true;
                }

                if (usuarioConEmail != null)
                {
                    mensajeError = $"Proceso no exitoso ya existe un usuario con el email '{email}'.";
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ExisteUsuario: {0}", ex);
                mensajeError = "Error al validar la existencia del usuario.";
                return true;
            }
        }

        public bool ExisteUsuarioEmail(string siglaRed, string email, out string mensajeError)
        {
            mensajeError = string.Empty;

            try
            {
                var usuarios = ObtenerTodosUsuarios();
                var usuarioConSigla = usuarios.FirstOrDefault(u =>
                    u.sigla_red?.Trim().Equals(siglaRed?.Trim(), StringComparison.OrdinalIgnoreCase) == true);

                var usuarioConEmail = usuarios.FirstOrDefault(u =>
                    u.Email?.Trim().Equals(email?.Trim(), StringComparison.OrdinalIgnoreCase) == true);

                if (usuarioConEmail != null && usuarioConSigla.id_usuario != usuarioConEmail.id_usuario)
                {
                    mensajeError = $"Proceso no exitoso ya existe un usuario con el email '{email}'.";
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error en ExisteUsuarioEmail: {0}", ex);
                mensajeError = "Proceso no exitoso ha ocurrido un error en el servicio ExisteUsuarioEmail.";
                return true;
            }
        }

        public bool EliminarUsuario(int id)
        {
            if (id <= 0) return false;

            try
            {
                using (var da = new DataAccess())
                {
                    int rows = da.ExecuteNonQueryByCode("usp_SoftDeleteUser", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", id);
                    });

                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error EliminarUsuario: {0}", ex);
                return false;
            }
        }
    }
}