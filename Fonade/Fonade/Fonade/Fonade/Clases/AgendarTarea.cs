using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Fonade.Clases
{
    public class AgendarTarea : Negocio.Base_Page
    {
        #region Atributos de la clase.

        private int ParaQuien { get; set; }
        private string NomTarea { get; set; }
        private string Descripcion { get; set; }
        private string CodProyecto { get; set; }
        private int CodTareaPrograma { get; set; }
        private string Recurrente { get; set; }
        private bool RecordatorioEmail { get; set; }
        private int NivelUrgencia { get; set; }
        private bool RecordatorioPantalla { get; set; }
        private bool RequiereRespuesta { get; set; }
        private int CodUsuarioAgendo { get; set; }
        private string Parametros { get; set; }
        private string DocumentoRelacionado { get; set; }
        private string Programa { get; set; }

        #endregion

        #region Nuevas variables "determinan el valor "bit" de los campos de tipo "bit".

        /// <summary>
        /// RecordatorioEmail = "bit".
        /// </summary>
        private int i_RecordatorioEmail = 0;

        /// <summary>
        /// RecordatorioPantalla = "bit".
        /// </summary>
        private int i_RecordatorioPantalla = 0;

        /// <summary>
        /// RequiereRespuesta = "bit".
        /// </summary>
        private int i_RequiereRespuesta = 0;

        #endregion

        /// <summary>
        /// Agendar tarea "básicamente se asignan los valores obtenidos en los parámetros a las variables internas".
        /// </summary>
        /// <param name="paraQuien"></param>
        /// <param name="nomTarea"></param>
        /// <param name="descripcion"></param>
        /// <param name="codProyecto"></param>
        /// <param name="codTareaPrograma"></param>
        /// <param name="recurrente"></param>
        /// <param name="recordatorioEmail"></param>
        /// <param name="nivelUrgencia"></param>
        /// <param name="recordatorioPantalla"></param>
        /// <param name="requiereRespuesta"></param>
        /// <param name="codUsuarioAgendo"></param>
        /// <param name="parametros"></param>
        /// <param name="documentoRelacionado"></param>
        /// <param name="programa"></param>
        public AgendarTarea(int paraQuien, string nomTarea, string descripcion, string codProyecto, int codTareaPrograma, string recurrente, bool recordatorioEmail, int nivelUrgencia,
        bool recordatorioPantalla, bool requiereRespuesta, int codUsuarioAgendo,
        string parametros, string documentoRelacionado, string programa)
        {
            this.ParaQuien = paraQuien;
            this.NomTarea = nomTarea;
            this.Descripcion = descripcion;
            this.CodProyecto = string.IsNullOrEmpty(codProyecto) ? "NULL" : codProyecto;
            this.CodTareaPrograma = codTareaPrograma;
            this.Recurrente = recurrente == "" ? "0" : recurrente;
            this.RecordatorioEmail = recordatorioEmail;
            this.NivelUrgencia = nivelUrgencia;
            this.RecordatorioPantalla = recordatorioPantalla;
            this.RequiereRespuesta = requiereRespuesta;
            this.CodUsuarioAgendo = usuario.IdContacto;
            this.Parametros = parametros;
            this.DocumentoRelacionado = documentoRelacionado;
            this.Programa = programa;
        }

        public string Agendar()
        {
            //Inicializar variables.
            DataTable RS = new DataTable();
            Consultas consulta = new Consultas();

            //Obtener el código de la tarea para generarla.
            int codigoTarea = InsertarTareaNueva();

            //Obtener la "respuesta (mensaje que verá el usuario al finalizar el proceso de generar tareas)".
            string respuesta = InsertarRepeticionTareaNueva(codigoTarea);

            if (this.RecordatorioEmail)
            {
                try
                {
                    var queryContacto = (from c in consulta.Db.Contactos
                                         where c.Id_Contacto == this.ParaQuien
                                         select new { c.Email, c.Nombres, c.Apellidos }).FirstOrDefault();

                    Correo correo = new Correo(usuario.Email, "Tarea Pendiente Fondo Emprender", queryContacto.Email, (queryContacto.Nombres + queryContacto.Apellidos), this.NomTarea, this.Descripcion);
                    correo.Enviar();
                }
                catch
                { respuesta = "La notificación por correo al usuario " + this.ParaQuien + " no pudo ser enviada."; }
            }
            return respuesta;
        }

        /// <summary>
        /// Obtener el código de la tarea a generar "es decir, el nuevo ID a usar para la nueva tarea".
        /// </summary>
        /// <returns>Código de la tarea a usar // Si devuelve cero, es porque hubo un error o NO consultó el ID.</returns>
        private int InsertarTareaNueva()
        {
            //Inicializar variables.
            String txtSQL = "";
            int codigoTarea = 0;
            Consultas consulta = new Consultas();
            DataTable RSTemporal = new DataTable();
            SqlCommand cmd = new SqlCommand();
            int i_RecordatorioEmail = 0;
            int i_RecordatorioPantalla = 0;
            int i_RequiereRespuesta = 0;

            if (RecordatorioEmail) { i_RecordatorioEmail = 1; }
            if (RecordatorioPantalla) { i_RecordatorioPantalla = 1; }
            if (RequiereRespuesta) { i_RequiereRespuesta = 1; }
            if (NomTarea.Contains("'")) { NomTarea = NomTarea.Replace("'", "&#39;"); }
            if (Descripcion.Contains("'")) { Descripcion = Descripcion.Replace("'", "&#39;"); }

            #region Instancia para usar la clase "TareaUsuario" asignando los valores obtenidos del input.

            //TareaUsuario datoNuevo = new TareaUsuario();
            //datoNuevo.CodContacto = this.ParaQuien;

            //try
            //{
            //    datoNuevo.CodProyecto = Convert.ToInt32(CodProyecto);
            //    datoNuevo.NomTareaUsuario = this.NomTarea;
            //    datoNuevo.Descripcion = this.Descripcion;
            //    datoNuevo.CodTareaPrograma = this.CodTareaPrograma;
            //    datoNuevo.Recurrente = this.Recurrente;
            //    datoNuevo.RecordatorioEmail = this.RecordatorioEmail;
            //    datoNuevo.NivelUrgencia = (short)this.NivelUrgencia;
            //    datoNuevo.RecordatorioPantalla = this.RecordatorioPantalla;
            //    datoNuevo.RequiereRespuesta = this.RequiereRespuesta;
            //    datoNuevo.CodContactoAgendo = this.CodUsuarioAgendo;
            //    datoNuevo.DocumentoRelacionado = this.DocumentoRelacionado;
            //}
            //catch (FormatException) { } 

            #endregion

            try
            {
                #region Consultar el ID del usuario después de haber hecho la inserción.

                ///Inserción.
                //consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                //consulta.Db.SubmitChanges();
                ///Consultar el ID de la inserción hecha.
                //codigoTarea = consulta.Db.TareaUsuarios.Where(c => (c.CodContacto == datoNuevo.CodContacto && c.NomTareaUsuario == datoNuevo.NomTareaUsuario
                //    && c.CodTareaPrograma == datoNuevo.CodTareaPrograma && c.Recurrente == datoNuevo.Recurrente && c.RecordatorioEmail == datoNuevo.RecordatorioEmail
                //    && c.NivelUrgencia == datoNuevo.NivelUrgencia && c.RecordatorioPantalla == datoNuevo.RecordatorioPantalla && c.RequiereRespuesta == datoNuevo.RequiereRespuesta
                //    && c.CodContactoAgendo == datoNuevo.CodContactoAgendo
                //    )).Max(c => c.Id_TareaUsuario); 

                #endregion

                //Inserción.
                txtSQL = " INSERT INTO TareaUsuario " +
                         "  (CodContacto," +
                         "  CodProyecto,  " +
                         "  NomTareaUsuario, " +
                         "  Descripcion,  " +
                         "  CodTareaPrograma, " +
                         "  Recurrente, " +
                         "  RecordatorioEmail, " +
                         "  NivelUrgencia, " +
                         "  RecordatorioPantalla," +
                         "  RequiereRespuesta, " +
                         "  CodContactoAgendo) " +
                    //"  DocumentoRelacionado)" +
                         "  VALUES " +
                         "  (" + ParaQuien + ", " +
                         "  " + CodProyecto + ",  " +
                         "  '" + NomTarea + "', " +
                         "  '" + Descripcion + "',  " +
                         "  " + CodTareaPrograma + ", " +
                         "  '" + Recurrente + "', " +
                         "  " + i_RecordatorioEmail + ", " +
                         "  " + NivelUrgencia + ", " +
                         "  " + i_RecordatorioPantalla + "," +
                         "  " + i_RequiereRespuesta + ", " +
                         "  " + CodUsuarioAgendo + ")"; //NO se envía el DocumentoRelacionado
                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();


                    #region Obtener el ID de la inserción realizada.

                    //Se averigua el autonumérico generado en TareaUsuario.
                    txtSQL = " SELECT Max(Id_TareaUsuario) AS Maximo FROM TareaUsuario WHERE CodContacto = " + ParaQuien +
                             " AND NomTareaUsuario = '" + NomTarea + "' AND CodTareaPrograma = " + CodTareaPrograma +
                             " AND Recurrente = '" + Recurrente + "' AND RecordatorioEmail = " + i_RecordatorioEmail +
                             " AND NivelUrgencia = " + NivelUrgencia + " AND RecordatorioPantalla = " + i_RecordatorioPantalla +
                             " AND RequiereRespuesta = " + i_RequiereRespuesta +
                             " AND CodContactoAgendo = " + CodUsuarioAgendo;

                    RSTemporal = consulta.ObtenerDataTable(txtSQL, "text");

                    if (RSTemporal.Rows.Count > 0)
                    { codigoTarea = Int32.Parse(RSTemporal.Rows[0]["Maximo"].ToString()); }

                    RSTemporal = null;

                    #endregion

                }
                catch (Exception ex) { string error_msg = ex.Message; codigoTarea = 0; }

                #region Comentarios.
                ////ejecutaReader(txtSQL, 2);

                ////Obtener el ID de la inserción realizada. 
                #endregion
            }
            catch { codigoTarea = 0; }
            return codigoTarea;
        }

        /// <summary>
        /// Realiza la inserción en la tabla "TareaUsuarioRepeticion".
        /// </summary>
        /// <param name="codigoTarea">Código de la tarea recién generada y consultada en el método "InsertarTareaNueva".</param>
        /// <returns>respuesta "mensaje que verá el usuario al terminarse el proceso de generar la tarea".</returns>
        private string InsertarRepeticionTareaNueva(int codigoTarea)
        {
            #region Comentarios.
            //string respuesta = "La tarea " + this.NomTarea + " ha sido agendada.";

            //Consultas consulta = new Consultas();

            //TareaUsuarioRepeticion datoRepeticionNuevo = new TareaUsuarioRepeticion();
            //datoRepeticionNuevo.Fecha = DateTime.Now;
            //datoRepeticionNuevo.CodTareaUsuario = codigoTarea;
            //datoRepeticionNuevo.Parametros = this.Parametros;
            //consulta.Db.TareaUsuarioRepeticions.InsertOnSubmit(datoRepeticionNuevo);
            //consulta.Db.SubmitChanges();

            //return respuesta; 
            #endregion

            //Inicializar variables.
            Consultas consulta = new Consultas();
            SqlCommand cmd = new SqlCommand();
            String txtSQL = "";
            string respuesta = "La tarea " + this.NomTarea + " ha sido agendada.";

            try
            {
                //Se inserta en la tabla de repeticiones TareaUsuarioRepeticion.
                txtSQL = " INSERT INTO TareaUsuarioRepeticion (Fecha, CodTareaUsuario, Parametros) " +
                         " VALUES (GETDATE()," + codigoTarea + ",'" + Parametros + "'" + ") ";

                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
                catch (Exception ex) { respuesta = ex.Message; }
            }
            catch (Exception ex) { respuesta = ex.Message; }

            //Retornar respuesta.
            return respuesta;
        }
    }
}