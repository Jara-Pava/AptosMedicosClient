using DataAccessDesarrollos.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Repositorios
{
    public class SolicitudRHRepository : ISolicitudRHRepository
    {
        // SP VALIDO
        public bool ActualizarSolicitudRH(SolicitudRH solicitud)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_UpdateSolicitud", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_Solicitud", solicitud.ID_Solicitud));
                        cmd.Parameters.Add(new SqlParameter("@id_TipoSolicitud", solicitud.id_TipoSolicitud));
                        cmd.Parameters.Add(new SqlParameter("@id_Solicitante", solicitud.id_Solicitante));
                        cmd.Parameters.Add(new SqlParameter("@id_Proyecto", solicitud.id_Proyecto));
                        cmd.Parameters.Add(new SqlParameter("@id_Planta", solicitud.id_Planta));
                        cmd.Parameters.Add(new SqlParameter("@Visitante", (object)solicitud.Visitante ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@FechaInicio", solicitud.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@FechaFin", solicitud.FechaFin));
                        cmd.Parameters.Add(new SqlParameter("@RFC", (object)solicitud.RFC ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@id_Contratista", solicitud.id_Contratista));
                        cmd.Parameters.Add(new SqlParameter("@Responsable", (object)solicitud.Responsable ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@AreaTrabajo", (object)solicitud.AreaTrabajo ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Actividad", (object)solicitud.Actividad ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Estancia", (object)solicitud.Estancia ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@aprobado", solicitud.aprobado));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ActualizarSolicitudRH: {0}", ex);
                return false;
            }
        }

        //sp valido
        public bool AprobarSolicitudRH(int idSolicitud, bool aprobado)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_AprobarSolicitud", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_Solicitud", idSolicitud));
                        cmd.Parameters.Add(new SqlParameter("@aprobado", aprobado));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error AprobarSolicitudRH: {0}", ex);
                return false;
            }
        }

        //TODO: Validar SP y parámetros
        public bool CrearSolicitudRH(SolicitudRH solicitud)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    // El SP devuelve el ID mediante SELECT SCOPE_IDENTITY(), no mediante parámetro OUTPUT
                    var idResult = da.ExecuteScalarByCode("rhsp_InsertSolicitud", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@id_TipoSolicitud", solicitud.id_TipoSolicitud));
                        cmd.Parameters.Add(new SqlParameter("@id_Solicitante", solicitud.id_Solicitante));
                        cmd.Parameters.Add(new SqlParameter("@id_Proyecto", (object)solicitud.id_Proyecto ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@id_Planta", (object)solicitud.id_Planta ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Visitante", (object)solicitud.Visitante ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@FechaInicio", (object)solicitud.FechaInicio ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@FechaFin", (object)solicitud.FechaFin ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@RFC", (object)solicitud.RFC ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Id_Contratista", (object)solicitud.id_Contratista ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Responsable", (object)solicitud.Responsable ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@AreaTrabajo", (object)solicitud.AreaTrabajo ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Actividad", (object)solicitud.Actividad ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Estancia", (object)solicitud.Estancia ?? DBNull.Value));
                        // NO enviar @FechaSolicitud ni @aprobado - el SP los maneja internamente
                        // NO enviar @ID_Solicitud como OUTPUT - el SP usa SELECT SCOPE_IDENTITY()
                    });

                    if (idResult != null && idResult != DBNull.Value)
                    {
                        solicitud.ID_Solicitud = Convert.ToInt32(idResult);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error CrearSolicitudRH: {0}", ex);
                return false;
            }
        }

        public void EliminarSolicitudRH(int id)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    da.ExecuteNonQueryByCode("rhsp_DeleteSolicitud", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_Solicitud", id));
                    });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error EliminarSolicitudRH: {0}", ex);
                throw;
            }
        }

        //SP valido
        public List<EmpresaContratista> ObtenerContratistas()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetContratistas", rdr => new EmpresaContratista
                    {
                        id_contratista = rdr["id_contratista"] != DBNull.Value ? Convert.ToInt32(rdr["id_contratista"]) : 0,
                        Nombre = rdr["Nombre"] != DBNull.Value ? rdr["Nombre"].ToString() : string.Empty,
                        RFC = rdr["RFC"] != DBNull.Value ? rdr["RFC"].ToString() : string.Empty,
                        Responsable = rdr["Responsable"] != DBNull.Value ? rdr["Responsable"].ToString() : string.Empty,
                        Email = rdr["Email"] != DBNull.Value ? rdr["Email"].ToString() : string.Empty,
                        Telefono = rdr["Telefono"] != DBNull.Value ? rdr["Telefono"].ToString() : string.Empty,
                        Activo = rdr["Activo"] != DBNull.Value ? Convert.ToBoolean(rdr["Activo"]) : false
                    });
                    return lista ?? new List<EmpresaContratista>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerContratistas: {0}", ex);
                return new List<EmpresaContratista>();
            }
        }

        public bool ExisteContratistaConNombre(string nombre, int? idContratista = null)
        {
            try
            {
                var contratistas = ObtenerContratistas();
                return contratistas.Any(r =>
                    r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                    (!idContratista.HasValue || r.id_contratista != idContratista.Value));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ExisteRolConNombre: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Insertar contratista
        public bool CrearContratista(EmpresaContratista contratista)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var result = da.ExecuteScalarByCode("rhsp_InsertContratista", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@Nombre", contratista.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@RFC", contratista.RFC));
                        cmd.Parameters.Add(new SqlParameter("@Responsable", contratista.Responsable));
                        cmd.Parameters.Add(new SqlParameter("@Email", contratista.Email));
                        cmd.Parameters.Add(new SqlParameter("@Telefono", contratista.Telefono));
                    });
                    if (result != null && result != DBNull.Value)
                    {
                        contratista.id_contratista = Convert.ToInt32(result);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error InsertarContratista: {0}", ex);
                return false;
            }
        }


        // Stored Procedure Actualizar contratista
        public bool ActualizarContratista(EmpresaContratista contratista)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_UpdateContratista", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@id_contratista", contratista.id_contratista));
                        cmd.Parameters.Add(new SqlParameter("@Nombre", contratista.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@RFC", contratista.RFC));
                        cmd.Parameters.Add(new SqlParameter("@Responsable", contratista.Responsable));
                        cmd.Parameters.Add(new SqlParameter("@Email", contratista.Email));
                        cmd.Parameters.Add(new SqlParameter("@Telefono", contratista.Telefono));
                        cmd.Parameters.Add(new SqlParameter("@Activo", contratista.Activo));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ActualizarContratista: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Eliminar contratista
        public bool EliminarContratista(int idContratista)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_DeleteContratista", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@id_contratista", idContratista));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error EliminarContratista: {0}", ex);
                return false;
            }
        }

        public List<Planta> ObtenerPlantas()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetPlantas", rdr => new Planta
                    {
                        ID_Planta = rdr["ID_Planta"] != DBNull.Value ? Convert.ToInt32(rdr["ID_Planta"]) : 0,
                        NombrePlanta = rdr["Planta"] != DBNull.Value ? rdr["Planta"].ToString() : string.Empty,
                        Activo = rdr["Activo"] != DBNull.Value ? Convert.ToBoolean(rdr["Activo"]) : false
                    });
                    return lista ?? new List<Planta>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerPlantas: {0}", ex);
                return new List<Planta>();
            }
        }

        // Validar si existe una planta con el mismo nombre (excluyendo un ID específico para actualizaciones)
        public bool ExistePlantaConNombre(string nombre, int? idPlanta = null)
        {
            try
            {
                var plantas = ObtenerPlantas();
                return plantas.Any(p =>
                    p.NombrePlanta.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                    (!idPlanta.HasValue || p.ID_Planta != idPlanta.Value));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ExistePlantaConNombre: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Crear planta
        public bool CrearPlanta(Planta item)
        {
            if (item == null) return false;
            try
            {
                using (var da = new DataAccess())
                {
                    var result = da.ExecuteScalarByCode("rhsp_InsertPlanta", cmd =>
                    {
                        cmd.Parameters.AddWithValue("@Planta", item.NombrePlanta ?? string.Empty);
                    });

                    if (result != null && result != DBNull.Value)
                    {
                        item.ID_Planta = Convert.ToInt32(result);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error CrearPlanta: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Actualizar planta
        public bool ActualizarPlanta(Planta planta)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_UpdatePlanta", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_Planta", planta.ID_Planta));
                        cmd.Parameters.Add(new SqlParameter("@Planta", planta.NombrePlanta));
                        cmd.Parameters.Add(new SqlParameter("@Activo", planta.Activo));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ActualizarPlanta: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Eliminar planta
        public bool EliminarPlanta(int idPlanta)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_DeletePlanta", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_Planta", idPlanta));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error EliminarPlanta: {0}", ex);
                return false;
            }
        }

        public bool ExisteProyectoConNombre(string nombre, int? idProyecto = null)
        {
            try
            {
                var proyectos = ObtenerProyectos();
                return proyectos.Any(p =>
                    p.NombreProyecto.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                    (!idProyecto.HasValue || p.ID_Proyecto != idProyecto.Value));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ExisteProyectoConNombre: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Crear proyecto
        public bool CrearProyecto(Proyecto item)
        {
            if (item == null) return false;
            try
            {
                using (var da = new DataAccess())
                {
                    var result = da.ExecuteScalarByCode("rhsp_InsertProyecto", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@Proyecto", item.NombreProyecto ?? string.Empty));
                    });

                    if (result != null && result != DBNull.Value)
                    {
                        item.ID_Proyecto = Convert.ToInt32(result);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error CrearProyecto: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Actualizar proyecto
        public bool ActualizarProyecto(Proyecto proyecto)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_UpdateProyecto", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_Proyecto", proyecto.ID_Proyecto));
                        cmd.Parameters.Add(new SqlParameter("@Proyecto", proyecto.NombreProyecto));
                        cmd.Parameters.Add(new SqlParameter("@Activo", proyecto.Activo));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ActualizarProyecto: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Eliminar proyecto
        public bool EliminarProyecto(int idProyecto)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_DeleteProyecto", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_Proyecto", idProyecto));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error EliminarProyecto: {0}", ex);
                return false;
            }
        }
        public List<Proyecto> ObtenerProyectos()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetProyectos", rdr => new Proyecto
                    {
                        ID_Proyecto = rdr["ID_Proyecto"] != DBNull.Value ? Convert.ToInt32(rdr["ID_Proyecto"]) : 0,
                        NombreProyecto = rdr["Proyecto"] != DBNull.Value ? rdr["Proyecto"].ToString() : string.Empty,
                        Activo = rdr["Activo"] != DBNull.Value ? Convert.ToBoolean(rdr["Activo"]) : false
                    });
                    return lista ?? new List<Proyecto>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerProyectos: {0}", ex);
                return new List<Proyecto>();
            }
        }

        public bool ExisteTipoVisitaConNombre(string nombre, int? idTipoVisita = null)
        {
            try
            {
                var tipos = ObtenerTiposSolicitud();
                return tipos.Any(t =>
                    t.Visita.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
                    (!idTipoVisita.HasValue || t.ID_TipoVisita != idTipoVisita.Value));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ExisteTipoVisitaConNombre: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Crear tipo de visita
        public bool CrearTipoVisita(TipoVisitante item)
        {
            if (item == null) return false;
            try
            {
                using (var da = new DataAccess())
                {
                    var result = da.ExecuteScalarByCode("rhsp_InsertTipoVisita", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@Visita", item.Visita ?? string.Empty));
                        cmd.Parameters.Add(new SqlParameter("@Estancia", (object)item.Estancia ?? DBNull.Value));
                    });

                    if (result != null && result != DBNull.Value)
                    {
                        item.ID_TipoVisita = Convert.ToInt32(result);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error CrearTipoVisita: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Actualizar tipo de visita
        public bool ActualizarTipoVisita(TipoVisitante item)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_UpdateTipoVisita", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_TipoVisita", item.ID_TipoVisita));
                        cmd.Parameters.Add(new SqlParameter("@Visita", item.Visita ?? string.Empty));
                        cmd.Parameters.Add(new SqlParameter("@Activo", item.Activo));
                        cmd.Parameters.Add(new SqlParameter("@Estancia", (object)item.Estancia ?? DBNull.Value));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ActualizarTipoVisita: {0}", ex);
                return false;
            }
        }

        // Stored Procedure Eliminar tipo de visita
        public bool EliminarTipoVisita(int idTipoVisita)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    int result = da.ExecuteNonQueryByCode("rhsp_DeleteTipoVisita", cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@ID_TipoVisita", idTipoVisita));
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error EliminarTipoVisita: {0}", ex);
                return false;
            }
        }

        public List<SolicitudRH> ObtenerSolicitudesRHPorUsuario(int empleadoId)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetSolicitudesByUser", rdr => new SolicitudRH
                    {
                        ID_Solicitud = rdr["ID_Solicitud"] != DBNull.Value ? Convert.ToInt32(rdr["ID_Solicitud"]) : 0,
                        id_TipoSolicitud = rdr["id_TipoSolicitud"] != DBNull.Value ? Convert.ToInt32(rdr["id_TipoSolicitud"]) : 0,
                        id_Solicitante = rdr["id_Solicitante"] != DBNull.Value ? Convert.ToInt32(rdr["id_Solicitante"]) : 0,
                        id_Proyecto = rdr["id_Proyecto"] != DBNull.Value ? Convert.ToInt32(rdr["id_Proyecto"]) : 0,
                        id_Planta = rdr["id_Planta"] != DBNull.Value ? Convert.ToInt32(rdr["id_Planta"]) : 0,
                        Visitante = rdr["Visitante"] != DBNull.Value ? rdr["Visitante"].ToString() : string.Empty,
                        FechaInicio = rdr["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaInicio"]) : default(DateTime),
                        FechaFin = rdr["FechaFin"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaFin"]) : default(DateTime),
                        RFC = rdr["RFC"] != DBNull.Value ? rdr["RFC"].ToString() : string.Empty,
                        id_Contratista = rdr["id_Contratista"] != DBNull.Value ? Convert.ToInt32(rdr["id_Contratista"]) : 0,
                        Responsable = rdr["Responsable"] != DBNull.Value ? rdr["Responsable"].ToString() : string.Empty,
                        AreaTrabajo = rdr["AreaTrabajo"] != DBNull.Value ? rdr["AreaTrabajo"].ToString() : string.Empty,
                        Actividad = rdr["Actividad"] != DBNull.Value ? rdr["Actividad"].ToString() : string.Empty,
                        Estancia = rdr["Estancia"] != DBNull.Value ? rdr["Estancia"].ToString() : string.Empty,
                        FechaSolicitud = rdr["FechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaSolicitud"]) : default(DateTime),
                        aprobado = rdr["aprobado"] != DBNull.Value ? Convert.ToBoolean(rdr["aprobado"]) : false
                    }, cmd =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@EmpleadoID", empleadoId));
                    });
                    return lista ?? new List<SolicitudRH>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerSolicitudesPorUsuario: {0}", ex);
                return new List<SolicitudRH>();
            }
        }

        // Obtener la solicitud por ID
        // Obtener la solicitud por ID
        public SolicitudRH ObtenerSolicitudRHPorId(int id)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetSolicitudById", rdr => new SolicitudRH
                    {
                        ID_Solicitud = rdr["ID_Solicitud"] != DBNull.Value ? Convert.ToInt32(rdr["ID_Solicitud"]) : 0,
                        id_TipoSolicitud = rdr["id_TipoSolicitud"] != DBNull.Value ? Convert.ToInt32(rdr["id_TipoSolicitud"]) : 0,
                        id_Solicitante = rdr["id_Solicitante"] != DBNull.Value ? Convert.ToInt32(rdr["id_Solicitante"]) : 0,
                        id_Proyecto = rdr["id_Proyecto"] != DBNull.Value ? Convert.ToInt32(rdr["id_Proyecto"]) : 0,
                        id_Planta = rdr["id_Planta"] != DBNull.Value ? Convert.ToInt32(rdr["id_Planta"]) : 0,
                        Visitante = rdr["Visitante"] != DBNull.Value ? rdr["Visitante"].ToString() : string.Empty,
                        FechaInicio = rdr["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaInicio"]) : default(DateTime),
                        FechaFin = rdr["FechaFin"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaFin"]) : default(DateTime),
                        RFC = rdr["RFC"] != DBNull.Value ? rdr["RFC"].ToString() : string.Empty,
                        id_Contratista = rdr["id_Contratista"] != DBNull.Value ? Convert.ToInt32(rdr["id_Contratista"]) : 0,
                        Responsable = rdr["Responsable"] != DBNull.Value ? rdr["Responsable"].ToString() : string.Empty,
                        AreaTrabajo = rdr["AreaTrabajo"] != DBNull.Value ? rdr["AreaTrabajo"].ToString() : string.Empty,
                        Actividad = rdr["Actividad"] != DBNull.Value ? rdr["Actividad"].ToString() : string.Empty,
                        Estancia = rdr["Estancia"] != DBNull.Value ? rdr["Estancia"].ToString() : string.Empty,
                        FechaSolicitud = rdr["FechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaSolicitud"]) : default(DateTime),
                        aprobado = rdr["aprobado"] != DBNull.Value ? Convert.ToBoolean(rdr["aprobado"]) : false,
                        // Campos adicionales opcionales
                        NombreTipoSolicitud = rdr["NombreTipoSolicitud"] != DBNull.Value ? rdr["NombreTipoSolicitud"].ToString() : string.Empty,
                        NombreSolicitante = rdr["NombreSolicitante"] != DBNull.Value ? rdr["NombreSolicitante"].ToString() : string.Empty,
                        NombreProyecto = rdr["NombreProyecto"] != DBNull.Value ? rdr["NombreProyecto"].ToString() : string.Empty,
                        NombrePlanta = rdr["NombrePlanta"] != DBNull.Value ? rdr["NombrePlanta"].ToString() : string.Empty,
                        NombreContratista = rdr["NombreContratista"] != DBNull.Value ? rdr["NombreContratista"].ToString() : string.Empty
                    }, cmd =>
                    {
                        // AQUÍ ESTABA EL PROBLEMA: Faltaba agregar el parámetro
                        cmd.Parameters.Add(new SqlParameter("@ID_Solicitud", id));
                    });

                    // Retornar el primer elemento o null si la lista está vacía
                    return lista != null && lista.Count > 0 ? lista[0] : null;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerSolicitudRHPorId: {0}", ex);
                return null;
            }
        }
        public List<TipoVisitante> ObtenerTiposSolicitud()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetTiposSolicitud", rdr => new TipoVisitante
                    {
                        ID_TipoVisita = rdr["ID_TipoVisita"] != DBNull.Value ? Convert.ToInt32(rdr["ID_TipoVisita"]) : 0,
                        Visita = rdr["Visita"] != DBNull.Value ? rdr["Visita"].ToString() : string.Empty,
                        Activo = true,
                        Estancia = rdr["Estancia"] != DBNull.Value ? rdr["Estancia"].ToString() : string.Empty
                    });
                    return lista ?? new List<TipoVisitante>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerTiposSolicitud: {0}", ex);
                return new List<TipoVisitante>();
            }
        }

        public List<SolicitudRH> ObtenerTodasSolicitudesRH()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var lista = da.ExecuteReaderByCode("rhsp_GetAllSolicitudes", rdr => new SolicitudRH
                    {
                        ID_Solicitud = rdr["ID_Solicitud"] != DBNull.Value ? Convert.ToInt32(rdr["ID_Solicitud"]) : 0,
                        id_TipoSolicitud = rdr["id_TipoSolicitud"] != DBNull.Value ? Convert.ToInt32(rdr["id_TipoSolicitud"]) : 0,
                        id_Solicitante = rdr["id_Solicitante"] != DBNull.Value ? Convert.ToInt32(rdr["id_Solicitante"]) : 0,
                        id_Planta = rdr["id_Planta"] != DBNull.Value ? Convert.ToInt32(rdr["id_Planta"]) : 0,
                        id_Proyecto = rdr["id_Proyecto"] != DBNull.Value ? Convert.ToInt32(rdr["id_Proyecto"]) : 0,
                        Visitante = rdr["Visitante"] != DBNull.Value ? rdr["Visitante"].ToString() : string.Empty,
                        FechaInicio = rdr["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaInicio"]) : default(DateTime),
                        FechaFin = rdr["FechaFin"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaFin"]) : default(DateTime),
                        RFC = rdr["RFC"] != DBNull.Value ? rdr["RFC"].ToString() : string.Empty,
                        id_Contratista = rdr["id_Contratista"] != DBNull.Value ? Convert.ToInt32(rdr["id_Contratista"]) : 0,
                        Responsable = rdr["Responsable"] != DBNull.Value ? rdr["Responsable"].ToString() : string.Empty,
                        AreaTrabajo = rdr["AreaTrabajo"] != DBNull.Value ? rdr["AreaTrabajo"].ToString() : string.Empty,
                        Actividad = rdr["Actividad"] != DBNull.Value ? rdr["Actividad"].ToString() : string.Empty,
                        Estancia = rdr["Estancia"] != DBNull.Value ? rdr["Estancia"].ToString() : string.Empty,
                        FechaSolicitud = rdr["FechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(rdr["FechaSolicitud"]) : default(DateTime),
                        aprobado = rdr["aprobado"] != DBNull.Value ? Convert.ToBoolean(rdr["aprobado"]) : false,
                        // Mapear los nombres
                        NombreTipoSolicitud = rdr["NombreTipoSolicitud"] != DBNull.Value ? rdr["NombreTipoSolicitud"].ToString() : string.Empty,
                        NombreSolicitante = rdr["NombreSolicitante"] != DBNull.Value ? rdr["NombreSolicitante"].ToString() : string.Empty,
                        NombreProyecto = rdr["NombreProyecto"] != DBNull.Value ? rdr["NombreProyecto"].ToString() : string.Empty,
                        NombrePlanta = rdr["NombrePlanta"] != DBNull.Value ? rdr["NombrePlanta"].ToString() : string.Empty,
                        NombreContratista = rdr["NombreContratista"] != DBNull.Value ? rdr["NombreContratista"].ToString() : string.Empty

                    });
                    return lista ?? new List<SolicitudRH>();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error ObtenerTodasSolicitudes: {0}", ex);
                return new List<SolicitudRH>();
            }
        }
    }
}
