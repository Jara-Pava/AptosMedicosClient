using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Interfaces
{
    public interface ISolicitudRHRepository
    {
        List<SolicitudRH> ObtenerTodasSolicitudesRH();
        List<SolicitudRH> ObtenerSolicitudesRHPorUsuario(int empleadoId);
        SolicitudRH ObtenerSolicitudRHPorId(int id);
        bool CrearSolicitudRH(SolicitudRH solicitud);
        bool ActualizarSolicitudRH(SolicitudRH solicitud);
        void EliminarSolicitudRH(int id);
        bool AprobarSolicitudRH(int idSolicitud, bool aprobado);

        // Catálogo de tipos de solicitud
        List<TipoVisitante> ObtenerTiposSolicitud();
        bool ExisteTipoVisitaConNombre(string nombre, int? idTipoVisita = null);
        bool CrearTipoVisita(TipoVisitante item);
        bool ActualizarTipoVisita(TipoVisitante item);
        bool EliminarTipoVisita(int idTipoVisita);

        // Catálogo de proyectos
        List<Proyecto> ObtenerProyectos();
        bool ExisteProyectoConNombre(string nombre, int? idProyecto = null);
        bool CrearProyecto(Proyecto item);
        bool ActualizarProyecto(Proyecto proyecto);
        bool EliminarProyecto(int idProyecto);

        // Catálogo de plantas
        List<Planta> ObtenerPlantas();
        bool ExistePlantaConNombre(string nombre, int? idPlanta = null);
        bool CrearPlanta(Planta item);
        bool ActualizarPlanta(Planta planta);
        bool EliminarPlanta(int idPlanta);

        // Catálogo de contratistas
        List<EmpresaContratista> ObtenerContratistas();
        bool ExisteContratistaConNombre(string nombre, int? idContratista = null);
        bool CrearContratista(EmpresaContratista contratista);
        bool ActualizarContratista(EmpresaContratista contratista);
        bool EliminarContratista(int idContratista);
    }
}