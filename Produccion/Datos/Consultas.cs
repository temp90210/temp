#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>13 - 03 - 2014</Fecha>
// <Archivo>Consultas.cs</Archivo>

#endregion

#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using Datos.DataType;
//using TSHAK.Components;

#endregion

namespace Datos
{
    public class Consultas
    {
        #region Variables

        private readonly string _cadena = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        
        private readonly FonadeDBDataContext db;
        public SqlParameter[] Parameters;

        #endregion

        #region Constructores.

        public Consultas()
        {
            db = new FonadeDBDataContext();
        }

        public FonadeDBDataContext Db
        {
            get { return db; }
        }

        public bool ValidarContacto(string email, string password)
        {
            if (db.Contactos.Any(t => t.Email == email && t.Clave == password))
                return true;
            else
                return false;
        }

        public UsuarioFonade GetContacto(string email)
        {
            List<UsuarioFonade> users = (from c in db.Contactos
                                         from gc in db.GrupoContactos
                                         where c.Id_Contacto == gc.CodContacto && c.Inactivo == false
                                               && c.Email == email
                                         select new UsuarioFonade(
                                             c.Nombres,
                                             c.Apellidos,
                                             c.Email, gc.CodGrupo,
                                             c.Id_Contacto,
                                             c.CodInstitucion.HasValue ? c.CodInstitucion.Value : -1,
                                             c.Identificacion,
                                             c.fechaCreacion.HasValue ? c.fechaCreacion.Value : DateTime.Now,
                                             c.Clave)).ToList();
            if (users.Count > 0)
            {
                return users.First();
            }
            else
                throw new Exception("Usuario no existe en la base de datos");
        }

        public bool CrearContacto(
            string username,
            string password,
            string email)
        {
            return true;
        }

        #endregion

        #region Metodos Linq

        public Grupo GetGrupo(int codGrupo)
        {
            return db.Grupos.First(t => t.Id_Grupo == codGrupo);
        }

        public List<Grupo> GetGrupos()
        {
            return db.Grupos.ToList();
        }

        public List<MD_VerEstudiosAsesorResult> VerEstudiosAsesor(int Id_usuario)
        {
            List<MD_VerEstudiosAsesorResult> tasks = db.MD_VerEstudiosAsesor(Id_usuario).ToList();
            return tasks;
        }

        public List<MD_FormalizarProyectoResult> FormalizarProyecto(int Id_usuario, int anexos, int cod_institucion,
                                                                    int inscripcion)
        {
            List<MD_FormalizarProyectoResult> tasks =
                db.MD_FormalizarProyecto(Id_usuario, anexos, cod_institucion, inscripcion).ToList();
            return tasks;
        }

        public List<MD_VerFormalizacionResult> VerFormalizacion(int Id_proyecto, int rolasesor, int rolasesorlider,
                                                                int rolemprendedor, int caso)
        {
            List<MD_VerFormalizacionResult> tasks =
                db.MD_VerFormalizacion(Id_proyecto, rolasesor, rolasesorlider, rolemprendedor, caso).ToList();
            return tasks;
        }


        public List<MD_MostrarBeneficiariosResult> MostrarBeneficiarios(int Id_proyecto)
        {
            List<MD_MostrarBeneficiariosResult> tasks = db.MD_MostrarBeneficiarios(Id_proyecto).ToList();
            return tasks;
        }

        public List<MD_MostrarProgramasAcademicosResult> MostrarProgramasAcademicos(int Ciudad, int Nivel,
                                                                                    string institucion, string programa)
        {
            List<MD_MostrarProgramasAcademicosResult> tasks =
                db.MD_MostrarProgramasAcademicos(Ciudad, Nivel, institucion, programa).ToList();
            return tasks;
        }

        public List<MD_MostrarArchivosOfflineResult> MostrarArchivosOffline(int CodContacto)
        {
            List<MD_MostrarArchivosOfflineResult> tasks = db.MD_MostrarArchivosOffline(CodContacto).ToList();
            return tasks;
        }

        public List<MD_VerFormalizacionEmprendedorResult> VerFormalizacionEmprendedor(int Id_proyecto,
                                                                                      int rolemprendedor)
        {
            List<MD_VerFormalizacionEmprendedorResult> tasks =
                db.MD_VerFormalizacionEmprendedor(Id_proyecto, rolemprendedor).ToList();
            return tasks;
        }

        public List<MD_VerFormalizacionEmpleosResult> VerFormalizacionEmpleos(int Id_proyecto)
        {
            List<MD_VerFormalizacionEmpleosResult> tasks = db.MD_VerFormalizacionEmpleos(Id_proyecto).ToList();
            return tasks;
        }

        public List<MD_VerEmprendedoresInactResult> VerEmprendedoresInact(int CodigoIntitucion, int ConstRolEmprendedor,
                                                                          string caso, string whereSeleccion)
        {
            List<MD_VerEmprendedoresInactResult> tasks =
                db.MD_VerEmprendedoresInact(CodigoIntitucion, ConstRolEmprendedor, caso, whereSeleccion).ToList();
            return tasks;
        }

        public List<PlanDeNegocio> misPlanesDeNegocio(int codeGroup, int codInstitucion, int codUsuario)
        {
            var consulta = from p in db.Proyectos
                           from c in db.Ciudads
                           from d in db.departamentos
                           from i in db.Institucions
                           where c.Id_Ciudad == p.CodCiudad
                                 & c.CodDepartamento == d.Id_Departamento
                                 & p.CodInstitucion == i.Id_Institucion
                           select new PlanDeNegocio
                           {
                               IdProyecto = p.Id_Proyecto,
                               NombreProyecto = p.NomProyecto,
                               CodigoInstitucion = i.Id_Institucion,
                               CodigoEstado = p.CodEstado,
                               NombreUnidad = i.NomUnidad,
                               NombreInstitucion = i.NomInstitucion,
                               NombreCiudad = c.NomCiudad,
                               NombreDepartamento = d.NomDepartamento,
                               Inactivo = p.Inactivo
                           };

            List<PlanDeNegocio> result = new List<PlanDeNegocio>();

            switch (codeGroup)
            {
                case Constantes.CONST_AdministradorFonade:
                case Constantes.CONST_AdministradorSena:
                    result = consulta.Where(p => p.Inactivo == false).ToList();
                    break;
                case Constantes.CONST_JefeUnidad:
                    result = consulta.Where(p => p.CodigoInstitucion == codInstitucion).ToList();
                    break;
                case Constantes.CONST_Asesor:
                case Constantes.CONST_Emprendedor:
                    var tempConsult = db.ProyectoContactos.Where(p => p.Proyecto.Id_Proyecto == p.CodProyecto
                                                                      && p.CodContacto == codUsuario
                                                                      && p.Inactivo == false).Select(t => t.CodProyecto);

                    result = consulta.Where(p => tempConsult.Contains(p.IdProyecto)
                                                 && p.Inactivo == false
                                                 && p.CodigoInstitucion == codInstitucion).ToList();
                    break;
                case Constantes.CONST_Evaluador:
                case Constantes.CONST_CoordinadorEvaluador:
                    var tempConsult1 = db.ProyectoContactos.Where(p => p.Proyecto.Id_Proyecto == p.CodProyecto
                                                                       && p.CodContacto == codUsuario
                                                                       && p.Inactivo == false).Select(t => t.CodProyecto);

                    result = consulta.Where(p => tempConsult1.Contains(p.IdProyecto)
                                                 && p.Inactivo == false
                                                 && p.CodigoEstado == Constantes.CONST_Evaluacion).ToList();
                    break;
                case Constantes.CONST_GerenteEvaluador:
                    result = consulta.Where(p => p.Inactivo == false
                                                 && (p.CodigoEstado == Constantes.CONST_Convocatoria
                                                     || p.CodigoEstado == Constantes.CONST_Evaluacion)).ToList();
                    break;
                default:
                    break;
            }

            return result;
        }

        public List<RetornarTareasAsesorResult> misTareas(int usuario)
        {
            List<RetornarTareasAsesorResult> tareas = db.RetornarTareasAsesor(usuario).ToList();
            return tareas;
        }

        public List<UsuarioFonade> ObtenerUsuario(string email)
        {
            List<UsuarioFonade> Users = ((from c in db.Contactos
                                          from gc in db.GrupoContactos
                                          where c.Id_Contacto == gc.CodContacto && c.Inactivo == false
                                          select new UsuarioFonade
                                              (
                                              c.Nombres,
                                              c.Apellidos,
                                              c.Email, gc.CodGrupo,
                                              c.Id_Contacto,
                                              c.CodInstitucion.HasValue ? c.CodInstitucion.Value : -1,
                                              c.Identificacion,
                                              c.fechaCreacion.HasValue ? c.fechaCreacion.Value : DateTime.Now,
                                              c.Clave)).Distinct
                ()).ToList();
            return Users;
        }

        public List<MD_Mostrar_resumen_equipoResult> Mostrar_resumen_equipo(int rolEmprendedor, int rolAsesor,
                                                                            int rolAsesorLider, int codigoProyecto,
                                                                            string caso)
        {
            List<MD_Mostrar_resumen_equipoResult> tasks =
                db.MD_Mostrar_resumen_equipo(rolEmprendedor, rolAsesor, rolAsesorLider, codigoProyecto, caso).ToList();
            return tasks;
        }

        public List<MD_MostrarProyectosaPriorizarResult> MostrarProyectosaPriorizar(int CodigoEstado)
        {
            List<MD_MostrarProyectosaPriorizarResult> tasks = db.MD_MostrarProyectosaPriorizar(CodigoEstado).ToList();
            return tasks;
        }

        public List<MD_VerActaAsignacionRecursosResult> VerActaAsignacionRecursos(int IdActa, string caso)
        {
            List<MD_VerActaAsignacionRecursosResult> tasks = db.MD_VerActaAsignacionRecursos(IdActa).ToList();
            return tasks;
        }

        public List<MD_ImprimirActaAsignacionResult> ImprimirActaAsignacion(int IdActa)
        {
            List<MD_ImprimirActaAsignacionResult> tasks = db.MD_ImprimirActaAsignacion(IdActa).ToList();
            return tasks;
        }

        public List<MD_VerListadoActasResult> VerListadoActas()
        {
            List<MD_VerListadoActasResult> tasks = db.MD_VerListadoActas().ToList();
            return tasks;
        }

        public List<MD_VerListadoConvocatoriasResult> VerListadoConvocatorias()
        {
            List<MD_VerListadoConvocatoriasResult> tasks = db.MD_VerListadoConvocatorias().ToList();
            return tasks;
        }

        public List<MD_MostrarProyectosPorConvocatoriaResult> MostrarProyectosPorConvocatoria(int idConvoct)
        {
            List<MD_MostrarProyectosPorConvocatoriaResult> tasks =
                db.MD_MostrarProyectosPorConvocatoria(idConvoct).ToList();
            return tasks;
        }

        public List<MD_VerReglasConvocatoriaResult> VerReglasConvocatoria(int idConvoct)
        {
            List<MD_VerReglasConvocatoriaResult> tasks = db.MD_VerReglasConvocatoria(idConvoct).ToList();
            return tasks;
        }

        public List<MD_Mostrar_ConvocatoriaCriterioPriorizacionResult> MostrarConvocatoriaCriterioPriorizacion(
            int idConvoct)
        {
            List<MD_Mostrar_ConvocatoriaCriterioPriorizacionResult> tasks =
                db.MD_Mostrar_ConvocatoriaCriterioPriorizacion(idConvoct).ToList();
            return tasks;
        }

        public List<MD_Mostrar_ListadoCriteriosResult> Mostrar_ListadoCriterios(int idConvoct)
        {
            List<MD_Mostrar_ListadoCriteriosResult> tasks = db.MD_Mostrar_ListadoCriterios(idConvoct).ToList();
            return tasks;
        }

        public List<MD_listarCatalogoInsumoResult> listarCatalogoInsumo(int proyecto, int tipoinsumo, int producto)
        {
            List<MD_listarCatalogoInsumoResult> tasks =
                db.MD_listarCatalogoInsumo(proyecto, tipoinsumo, producto).ToList();
            return tasks;
        }

        public List<MD_txt_insumoResult> txt_insumo()
        {
            List<MD_txt_insumoResult> tasks = db.MD_txt_insumo().ToList();
            return tasks;
        }

        public List<MD_ConsultarResult> Consultar(int usuario, int codgrupo, int institucion, string accion,
                                                  string palabraclave)
        {
            List<MD_ConsultarResult> tasks =
                db.MD_Consultar(usuario, codgrupo, institucion, accion, palabraclave).ToList();
            return tasks;
        }

        public List<MD_MostrarIntegrantesCentralesRiesgosResult> MostrarIntegrantesCentralesRiesgos(int codproyecto,
                                                                                                    int rolemprendedor,
                                                                                                    int codconvocatoria)
        {
            List<MD_MostrarIntegrantesCentralesRiesgosResult> tasks =
                db.MD_MostrarIntegrantesCentralesRiesgos(codproyecto, rolemprendedor, codconvocatoria).ToList();
            return tasks;
        }

        public List<MD_MostrarEvaluacionRubroProyectoResult> MostrarEvaluacionRubroProyecto(int codproyecto,
                                                                                            int codconvocatoria)
        {
            List<MD_MostrarEvaluacionRubroProyectoResult> tasks =
                db.MD_MostrarEvaluacionRubroProyecto(codproyecto, codconvocatoria).ToList();
            return tasks;
        }

        public List<MD_VerCoordinadoresDEEvaluacionResult> VerCoordinadoresDeEvaluacion()
        {
            List<MD_VerCoordinadoresDEEvaluacionResult> tasks = db.MD_VerCoordinadoresDEEvaluacion().ToList<MD_VerCoordinadoresDEEvaluacionResult>();
            return tasks;
        }

        public List<MD_VerEvaluadoresDeCoordinadorResult> VerEvaluadoresDeCoordinador(int contacto, int estado)
        {
            List<MD_VerEvaluadoresDeCoordinadorResult> tasks = db.MD_VerEvaluadoresDeCoordinador(contacto, estado).ToList<MD_VerEvaluadoresDeCoordinadorResult>();
            return tasks;
        }

        public List<MD_VerProyectosEvaluadorResult> VerProyectosEvaluador(int contacto, int evaluacion)
        {
            List<MD_VerProyectosEvaluadorResult> tasks = db.MD_VerProyectosEvaluador(contacto, evaluacion).ToList<MD_VerProyectosEvaluadorResult>();
            return tasks;
        }

        public List<MD_VerEvaluadoresResult> VerEvaluadores()
        {
            List<MD_VerEvaluadoresResult> tasks = db.MD_VerEvaluadores().ToList<MD_VerEvaluadoresResult>();
            return tasks;
        }
        public List<MD_VerGerenteIntervResult> VerGerenteInterv()
        {
            List<MD_VerGerenteIntervResult> tasks = db.MD_VerGerenteInterv().ToList<MD_VerGerenteIntervResult>();
            return tasks;
        }



        public List<MD_cargarProyectosEvalResult> CargarProyectosEval(int contacto, int constRoleval)
        {
            List<MD_cargarProyectosEvalResult> tasks = db.MD_cargarProyectosEval(contacto, constRoleval).ToList<MD_cargarProyectosEvalResult>();
            return tasks;
        }

        public List<MD_cargarProyectosIntervResult> CargarProyectosInterv(int contacto, int constRolinterv, int constRolintervLider)
        {
            List<MD_cargarProyectosIntervResult> tasks = db.MD_cargarProyectosInterv(contacto, constRolinterv, constRolintervLider).ToList<MD_cargarProyectosIntervResult>();
            return tasks;
        }

        public List<MD_VerCoordinadorAsignadoResult> VerCoordinadorAsignado(int contacto)
        {
            List<MD_VerCoordinadorAsignadoResult> tasks = db.MD_VerCoordinadorAsignado(contacto).ToList<MD_VerCoordinadorAsignadoResult>();
            return tasks;
        }

        public List<MD_cargarProyectosSectorResult> cargarProyectosSector(int proyecto)
        {
            List<MD_cargarProyectosSectorResult> tasks = db.MD_cargarProyectosSector(proyecto).ToList<MD_cargarProyectosSectorResult>();
            return tasks;
        }

        public List<MD_InformeVistaInterventoriaResult> VerInformeVisitasInterventoria(int CodGrupo, int IdContacto)
        {
            List<MD_InformeVistaInterventoriaResult> tasks = db.MD_InformeVistaInterventoria(CodGrupo, IdContacto).ToList<MD_InformeVistaInterventoriaResult>();
            return tasks;
        }

        public List<MD_InformeBimensualInterventoriaResult> VerInformeBimensualInterventoria(int CodGrupo, int IdContacto)
        {
            List<MD_InformeBimensualInterventoriaResult> tasks = db.MD_InformeBimensualInterventoria(CodGrupo, IdContacto).ToList<MD_InformeBimensualInterventoriaResult>();
            return tasks;
        }
  
        public List<MD_InformeEjecucionInterventoriaResult> VerInformeEjecucionInterventoria(int CodGrupo, int IdContacto)
        {
            List<MD_InformeEjecucionInterventoriaResult> tasks = db.MD_InformeEjecucionInterventoria(CodGrupo, IdContacto).ToList<MD_InformeEjecucionInterventoriaResult>();
            return tasks;
        }

        public List<MD_InformeConsolidadoInterventoriaResult> VerInformeConsolidadoInterventoria(int CodGrupo, int IdContacto)
        {
            List<MD_InformeConsolidadoInterventoriaResult> tasks = db.MD_InformeConsolidadoInterventoria(CodGrupo, IdContacto).ToList<MD_InformeConsolidadoInterventoriaResult>();
            return tasks;
        }

        public List<MD_cargarProyectoSumarioActualResult> cargarProyectoSumarioActual(int proyecto)
        {
            List<MD_cargarProyectoSumarioActualResult> tasks = db.MD_cargarProyectoSumarioActual(proyecto).ToList<MD_cargarProyectoSumarioActualResult>();
            return tasks;
        }
        public List<MD_VerInterventoresDeCoordinadorResult> VerInterventoresDeCoordinador(int contacto)
        {
            List<MD_VerInterventoresDeCoordinadorResult> tasks = db.MD_VerInterventoresDeCoordinador(contacto).ToList<MD_VerInterventoresDeCoordinadorResult>();
            return tasks;
        }
        public List<MD_VerProyectosInterventorResult> VerProyectosInterventor(int contacto, int estado)
        {
            List<MD_VerProyectosInterventorResult> tasks =
                db.MD_VerProyectosInterventor(contacto, estado).ToList();
            return tasks;
        }
        #region Infraestructura

        public List<MD_ObtenerInfraestructuraResult> ObtenerInfraestructura(string codigoProyecto)
        {
            var infraestructura = db.MD_ObtenerInfraestructura(codigoProyecto).ToList();

            return infraestructura;
        }

        #endregion

        #region Aportes

        public List<MD_GetIntegrantesIniciativaResult> ObtenerIntegrantesIniciativa(string proyecto, string convocatoria)
        {
            var integrantes = db.MD_GetIntegrantesIniciativa(proyecto, convocatoria).ToList();

            return integrantes;
        }

        public List<MD_GetAportesResult> ObtenerAportes(string proyecto, string convocatoria)
        {
            var aportes = db.MD_GetAportes(proyecto, convocatoria).ToList();

            return aportes;
        }

        public int ObtenerTabs(int proyecto)
        {
            var codproyecto = db.MD_obtenerTabs(proyecto).First().codproyecto;

            return codproyecto;
        }

        #endregion

        #region ProyectoInfraestructura

        public ISingleResult<MD_ObtenerProyectoOperacionInfraestructuraResult> CrudProyectoInfraestructura(int tipo,
                                                                                                           int proyecto,
                                                                                                           string text,
                                                                                                           ref string
                                                                                                               MensajeDeError)
        {
            ISingleResult<MD_ObtenerProyectoOperacionInfraestructuraResult> tasks = null;
            try
            {
                tasks = db.MD_ObtenerProyectoOperacionInfraestructura(tipo, proyecto, text);
            }
            catch (Exception ex)
            {
                MensajeDeError = string.Format("Se presento un error : {0}, Tipo : {1}", ex.Message, ex.StackTrace);
            }

            return tasks;
        }

        #endregion

        #region EvaluacionAspectos

        public List<MD_ObtenerCamposEvaluacionObservacionesResult> ObtenerCamposEvaluacionObservaciones(int proyecto,
                                                                                                        int convocatoria,
                                                                                                        int idcampo,
                                                                                                        ref string
                                                                                                            mensajeDeError)
        {
            List<MD_ObtenerCamposEvaluacionObservacionesResult> var = null;

            try
            {
                var = db.MD_ObtenerCamposEvaluacionObservaciones(proyecto, convocatoria, idcampo).ToList();
            }
            catch (Exception ex)
            {
                mensajeDeError = string.Format("Error al consultar los aspectos : {0} ,{1}", ex.Message,
                                               ex.StackTrace);
            }

            return var;
        }


        public List<MD_ObtenerCamposEvaluacionObservacionesHijasResult> ObtenerCamposEvaluacionObservacionesHijas(
            string campo, int proyecto, int convocatoria, int idcampo, ref string mensajeDeError)
        {
            List<MD_ObtenerCamposEvaluacionObservacionesHijasResult> var = null;

            try
            {
                var =
                    db.MD_ObtenerCamposEvaluacionObservacionesHijas(Convert.ToInt32(campo), proyecto, convocatoria,
                                                                    idcampo).ToList();
            }
            catch (Exception ex)
            {
                mensajeDeError = string.Format("Error al consultar los aspectos : {0} ,{1}", ex.Message,
                                               ex.StackTrace);
            }

            return var;
        }

        #endregion

        #region informesEvaluacion

        public List<MD_ObtenerInformacionEvaluacionResult> ObtenercamposEvaluacion(int codproyecto, ref string mensaje)
        {
            List<MD_ObtenerInformacionEvaluacionResult> consulta = null;
            try
            {
                consulta = db.MD_ObtenerInformacionEvaluacion(codproyecto).ToList();
            }
            catch (Exception ex)
            {
                mensaje = string.Format("se presento un error : {0}, de tipo : {1}", ex.Message, ex.StackTrace);
            }

            return consulta;
        }


        public List<MD_HojaAvanceEvaluacionResult> HojaEvaluacion(int codgrupo, ref string mensaje)
        {
            List<MD_HojaAvanceEvaluacionResult> consulta = null;
            try
            {
                consulta = db.MD_HojaAvanceEvaluacion(codgrupo).ToList();
            }
            catch (Exception ex)
            {
                mensaje = string.Format("se presento un error obteniendo la hoja de avance : {0}, de tipo : {1}", ex.Message, ex.StackTrace);
            }

            return consulta;
        }
        #endregion

        #endregion

        #region Metodos system Data

        #region "Metodos que Retornan DataTable

        public DataTable ObtenerDataTable(string procedimiento, string typo = "")
        {
            var myDataTable = new DataTable("Controles");
            var connection = new SqlConnection(_cadena);
            try
            {
                if (!string.IsNullOrEmpty(procedimiento) && procedimiento != "")
                {
                    var adapter = new SqlDataAdapter(procedimiento, connection);

                    if (typo == "text")
                    {
                        adapter.SelectCommand.CommandType = CommandType.Text;
                    }
                    else
                    {

                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    }


                    if (Parameters != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(Parameters);
                    }

                    adapter.SelectCommand.Connection.Open();
                    adapter.Fill(myDataTable);
                    adapter.SelectCommand.Connection.Close();
                    Parameters = null;
                }
            }
            catch (Exception) { }
            return myDataTable;
        }



        #endregion

        #region "Metodos que Retornan Dataset

        public DataSet ObtenerDataSet(string procedimiento, string typo = "")
        {

            var myDataSet = new DataSet("Dataset");
            var connection = new SqlConnection(_cadena);


            try
            {
                if (!string.IsNullOrEmpty(procedimiento) && procedimiento != "")
                {
                    var adapter = new SqlDataAdapter(procedimiento, connection);


                    if (typo == "text")
                    {
                        adapter.SelectCommand.CommandType = CommandType.Text;
                    }
                    else
                    {

                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    }

                    if (Parameters != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(Parameters);
                    }

                    adapter.SelectCommand.Connection.Open();
                    adapter.Fill(myDataSet);
                    //adapter.SelectCommand.Connection.Close();
                    Parameters = null;
                }
            }
            catch (Exception exception)
            {
                Parameters = null;
                throw new Exception(string.Format("se ha generado un error de tipo: {0}, al tratar de consultar : {1}",
                                                  exception.Message,
                                                  exception.StackTrace));
            }
            return myDataSet;
        }

        #endregion

        #region "Metodos Para eliminar"

        public bool Eliminar(string procedimiento, int parametro)
        {

            var connection = new SqlConnection(_cadena);
            bool bandera = false;

            try
            {
                if (!string.IsNullOrEmpty(procedimiento) && procedimiento != "")
                {
                    var adapter = new SqlDataAdapter(procedimiento.Trim(), connection)
                    {
                        SelectCommand =
                        {
                            CommandType = CommandType.StoredProcedure
                        }
                    };
                    if (Parameters != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(Parameters);
                    }

                    adapter.SelectCommand.Connection.Open();
                    bandera = Convert.ToBoolean(adapter.SelectCommand.ExecuteScalar());
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("se ha generado un error de tipo: {1}, al tratar de eliminar  : {0}",
                                                  exception.Message,
                                                  exception.StackTrace));
            }
            return bandera;
        }

        #endregion

        #region "Metodo que retorna un booleano, CRUD"

        public bool InsertarDataTable(string procedimiento)
        {
            var connection = new SqlConnection(_cadena);
            bool bandera = false;

            try
            {
                if (!string.IsNullOrEmpty(procedimiento) && procedimiento != "")
                {
                    var adapter = new SqlDataAdapter(procedimiento.Trim(), connection)
                    {
                        SelectCommand =
                        {
                            CommandType = CommandType.StoredProcedure
                        }
                    };
                    if (Parameters != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(Parameters);
                    }

                    adapter.SelectCommand.Connection.Open();
                    bandera = Convert.ToBoolean(adapter.SelectCommand.ExecuteScalar());
                    adapter.SelectCommand.Connection.Close();
                    Parameters = null;
                }
            }
            catch (Exception exception)
            {
                Parameters = null;
                throw new Exception(
                    string.Format("se ha generado un error al tratar de insertar el excel : {0},  tipo: {1}",
                                  exception.Message,
                                  exception.StackTrace));
            }
            return bandera;
        }


        public object RetornarEscalar(string procedimiento, string tipo)
        {
            var connection = new SqlConnection(_cadena);
            bool bandera = false;

            try
            {
                if (!string.IsNullOrEmpty(procedimiento) && procedimiento != "")
                {
                    var adapter = new SqlDataAdapter(procedimiento, connection);

                    if (tipo == "text")
                    {
                        adapter.SelectCommand.CommandType = CommandType.Text;
                    }
                    else
                    {

                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    }

                    if (Parameters != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(Parameters);
                    }

                    adapter.SelectCommand.Connection.Open();
                    var miobject = adapter.SelectCommand.ExecuteScalar();
                    adapter.SelectCommand.Connection.Close();
                    Parameters = null;
                    return miobject;
                }
            }
            catch (Exception exception)
            {
                Parameters = null;
                throw new Exception(
                    string.Format("se ha generado un error al tratar de insertar el excel : {0},  tipo: {1}",
                                  exception.Message,
                                  exception.StackTrace));
            }
            return bandera;
        }
        #endregion

        #endregion



    }
}