#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>08 - 07 - 2014</Fecha>
// <Archivo>PostIt.cs</Archivo>

#endregion

#region using

using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace Fonade.Controles
{
    public partial class PostIt : Negocio.Base_Page
    {
        String CodTareaPrograma;
        String ParaQuien;
        String Proyecto;
        String NomTarea;
        String Descripcion;
        String Recurrente;
        String RecordatorioEmail;
        String NivelUrgencia;
        String RecordatorioPantalla;
        String RequiereRespuesta;
        String CodContactoAgendo;
        String DocumentoRelacionado;
        String intTemp;

        #region entradaDatos
        private String codProyecto;
        private Int32 CodUsuario;
        private String Accion;
        private Int32 codGrupo;

        private Int32 CONST_PostIt;
        private String tabEval;
        private String txtCampo;
        #endregion

        String txtSQL = "";

        private DataTable _DataContactos;
        private Consultas consulta = new Consultas(); //WAFS 11-OCT-2014

        protected void Page_Load(object sender, EventArgs e)
        {
            recogeSession();

            if (!IsPostBack)
            {

                C_Fecha.SelectedDate = DateTime.Now;

                //busca el nombre del usuario en la base de datos
                var contacto = (from c in consulta.Db.Contactos
                                where c.Id_Contacto == CodUsuario
                                select new 
                                {
                                    c.Nombres, 
                                    c.Apellidos
                                }).FirstOrDefault();

                //muestra en un label el nombre del usuario quien asignara la tarea
                L_Nombreusuario.Text = contacto.Nombres + " " + contacto.Apellidos;

                if (String.IsNullOrEmpty(Accion))
                    L_PostIt.Text = "AGENDAR POST IT";
                else
                {
                    if (Accion.Equals("Modificar") || Accion.Equals("Consultar"))
                        L_PostIt.Text = "REVISAR POST IT";
                    else
                        L_PostIt.Text = "AGENDAR POST IT";
                }

                Queryable();

                if (_DataContactos != null)
                    CrearlistemItem(_DataContactos);
            }
        }

        protected void B_Grabar_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in LB_Para.Items)
            {
                if (item.Selected)
                {
                    CodTareaPrograma = "" + CONST_PostIt;
                    ParaQuien = item.Value;
                    Proyecto = "" + codProyecto;
                    NomTarea = TB_Tarea.Text;
                    Descripcion = TB_Descripcion.Text;
                    Recurrente = "0";
                    RecordatorioEmail = DD_Email.SelectedValue;
                    NivelUrgencia = "1";
                    RecordatorioPantalla = "0";
                    RequiereRespuesta = "0";
                    CodContactoAgendo = "" + CodUsuario;
                    DocumentoRelacionado = " ";

                    string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;

                    using (var con = new SqlConnection(conexionStr))
                    {
                        using (var com = con.CreateCommand())
                        {
                            com.CommandText = "MD_NuevaTareaInsert";
                            com.CommandType = System.Data.CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@CodTareaPrograma", CodTareaPrograma);
                            com.Parameters.AddWithValue("@CodContacto", ParaQuien);
                            com.Parameters.AddWithValue("@CodProyecto", Proyecto);
                            com.Parameters.AddWithValue("@NomTareaUsuario", NomTarea);
                            com.Parameters.AddWithValue("@Descripcion", Descripcion);
                            com.Parameters.AddWithValue("@Recurrente", Recurrente);
                            com.Parameters.AddWithValue("@RecordatorioEmail", RecordatorioEmail);
                            com.Parameters.AddWithValue("@NivelUrgencia", NivelUrgencia);
                            com.Parameters.AddWithValue("@RecordatorioPantalla", RecordatorioPantalla);
                            com.Parameters.AddWithValue("@RequiereRespuesta", RequiereRespuesta);
                            com.Parameters.AddWithValue("@CodContactoAgendo", CodContactoAgendo);
                            com.Parameters.AddWithValue("@DocumentoRelacionado", DocumentoRelacionado);
                            try
                            {
                                con.Open();
                                com.ExecuteReader();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                            finally
                            {
                                con.Close();
                            }
                        }
                    }

                    Cadena_SQLTemp();

                    TareaUsuarioRepeticion();

                    if (RecordatorioEmail.Equals("1"))
                    {
                        try
                        {
                            enviarPorEmail(item.Text, "Envío módulo de tareas", item.Text + " Tarea Pendiente Fondo Emprender " + NomTarea + " " + Descripcion);
                        }
                        catch (Exception) { }
                    }
                }
            }

            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        }

        protected void C_Cerrar_Click(object sender, EventArgs e)
        {

        }

        private void recogeSession()
        {
            try
            {
                try
                {
                    codProyecto = Session["EvalCodProyectoPOst"].ToString();
                }
                catch (Exception)
                {
                    codProyecto = "";
                }

                if (String.IsNullOrEmpty(codProyecto))
                    codProyecto = Session["CodProyecto"].ToString();

                try
                {
                    CodUsuario = Int32.Parse(Session["EvalCodUsuario"].ToString());
                }
                catch (FormatException)
                {
                    CodUsuario = usuario.IdContacto;
                }
                catch (Exception)
                {
                    CodUsuario = usuario.IdContacto;
                }
                try
                {
                    Accion = Session["EvalAccion"].ToString();
                }
                catch (Exception)
                {
                    Accion = "Adicionar";
                }
                try
                {
                    codGrupo = usuario.CodGrupo;
                }
                catch (Exception)
                {
                    codGrupo = -1;
                }
                try
                {
                    tabEval = Session["tabEval"].ToString();
                }
                catch (Exception)
                {
                    tabEval = "";
                }
                try
                {
                    CONST_PostIt = Int32.Parse(Session["EvalConsPOST"].ToString());
                }
                catch (FormatException)
                {
                    CONST_PostIt = Constantes.CONST_PostIt;
                }
                catch (Exception)
                {
                    CONST_PostIt = Constantes.CONST_PostIt;
                }
                try
                {
                    txtCampo = Session["Campo"].ToString();
                }
                catch (Exception)
                {
                    txtCampo = "nulo";
                }
            }
            catch (Exception)
            {
                ClientScriptManager cm = this.ClientScript;
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.close();</script>");
            }
        }

        private void Queryable()
        {
            txtSQL = "";
            //            if (String.IsNullOrEmpty(tabInter))
            //            {
            //                txtSQL = @"SELECT DISTINCT Id_Contacto, Nombres +' '+ Apellidos as Nombre, R.Nombre Rol FROM Contacto,ProyectoContacto P, Rol R
            //                        where Id_Contacto = CodContacto and Id_Rol=CodRol and P.inactivo = 0 and id_contacto<>" + CodUsuario + " and codproyecto=" + codProyecto + "";
            //            }
            //            else
            //            {
            switch (codGrupo)
            {
                case Constantes.CONST_GerenteInterventor:
                    txtSQL = @"SELECT DISTINCT Id_Contacto, Nombres +' '+ Apellidos as Nombre, R.Nombre Rol
                                FROM Contacto,Empresa,EmpresaInterventor P, Rol R
                                where id_empresa=codempresa and Id_Contacto = CodContacto and Id_Rol=Rol and
                                P.inactivo = 0 and id_contacto<>" + CodUsuario + " and codproyecto=" + codProyecto + @"
                                union
                                SELECT DISTINCT Contacto.Id_Contacto, Contacto.Nombres + ' ' + Contacto.Apellidos AS Nombre, 'Coordinador Interventoria' AS Rol
                                FROM Interventor INNER JOIN EmpresaInterventor ON Interventor.CodContacto = EmpresaInterventor.CodContacto
                                INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa
                                INNER JOIN Contacto ON Interventor.CodCoordinador = Contacto.Id_Contacto
                                WHERE (Empresa.codproyecto = " + codProyecto + ")";
                    break;
                case Constantes.CONST_CoordinadorInterventor:
                    txtSQL = @"SELECT DISTINCT Id_Contacto, Nombres +' '+ Apellidos as Nombre, R.Nombre Rol
                                FROM Contacto,Empresa,EmpresaInterventor P, Rol R
                                where id_empresa=codempresa and Id_Contacto = CodContacto and Id_Rol=Rol and
                                P.inactivo = 0 and id_contacto<>" + CodUsuario + " and codproyecto=" + codProyecto + @"
                                union
                                SELECT DISTINCT Id_Contacto, Nombres +' '+ Apellidos as Nombre, 'Emprendedor' Rol
                                FROM Contacto,Empresa,EmpresaContacto P
                                where id_empresa=codempresa and Id_Contacto = CodContacto and
                                id_contacto<>" + CodUsuario + " and codproyecto=" + codProyecto + "";
                    break;
                case Constantes.CONST_Interventor:
                    txtSQL = @"SELECT DISTINCT Id_Contacto, Nombres +' '+ Apellidos as Nombre, 'Emprendedor' Rol
                                FROM Contacto,Empresa,EmpresaContacto P
                                where id_empresa=codempresa and Id_Contacto = CodContacto and
                                id_contacto<>" + CodUsuario + " and codproyecto=" + codProyecto + @"
                                UNION
                                SELECT DISTINCT Contacto.Id_Contacto, Contacto.Nombres + ' ' + Contacto.Apellidos AS Nombre, 'Coordinador Interventoria' AS Rol
                                FROM Interventor INNER JOIN EmpresaInterventor ON Interventor.CodContacto = EmpresaInterventor.CodContacto
                                INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa 
                                INNER JOIN Contacto ON Interventor.CodCoordinador = Contacto.Id_Contacto
                                WHERE (Empresa.codproyecto = " + codProyecto + ")";
                    break;
                case Constantes.CONST_Emprendedor:
                    txtSQL = @"SELECT DISTINCT Id_Contacto, Nombres +' '+ Apellidos as Nombre, R.Nombre Rol
                                FROM Contacto,Empresa,EmpresaInterventor P, Rol R
                                where id_empresa=codempresa and Id_Contacto = CodContacto and Id_Rol=Rol and
                                P.inactivo = 0 and id_contacto<>" + CodUsuario + " and codproyecto=" + codProyecto + "";
                    break;
                default:
                    txtSQL = @"SELECT DISTINCT Id_Contacto, Nombres +' '+ Apellidos as Nombre, R.Nombre Rol FROM Contacto,ProyectoContacto P, Rol R 
								where Id_Contacto = CodContacto and Id_Rol=CodRol and P.inactivo = 0 and id_contacto<>" + CodUsuario + " and codproyecto=" + codProyecto + "";
                    break;
            }
            //}
            if (!String.IsNullOrEmpty(txtSQL))
            {
                txtSQL = txtSQL + "" + " ORDER BY Nombre";
                try
                {
                    consulta = new Consultas();

                    _DataContactos = consulta.ObtenerDataTable(txtSQL, "text");
                }
                catch (NullReferenceException) { }
                catch (SqlException) { }
            }
        }

        private void CrearlistemItem(DataTable _Conts)
        {
            LB_Para.Items.Clear();
            try
            {
                for (int i = 0; i < _Conts.Rows.Count; i++)
                {
                    ListItem listItemFor = new ListItem();
                    listItemFor.Text = "" + _Conts.Rows[i]["Nombre"].ToString() + "( " + _Conts.Rows[i]["Rol"].ToString() + " )";
                    listItemFor.Value = "" + _Conts.Rows[i]["Id_Contacto"].ToString();
                    LB_Para.Items.Add(listItemFor);
                }
            }
            catch (NullReferenceException) { }
        }

        private void Cadena_SQLTemp()
        {
            var result = (from tu in consultas.Db.TareaUsuarios
                          where tu.CodContacto == Convert.ToInt32(ParaQuien)
                          && tu.NomTareaUsuario == NomTarea
                          && tu.CodTareaPrograma == Convert.ToInt32(CodTareaPrograma)
                          && tu.Recurrente == Recurrente
                          && tu.RecordatorioEmail == Convert.ToBoolean(RecordatorioEmail)
                          && tu.NivelUrgencia == Convert.ToChar(NivelUrgencia)
                          && tu.RecordatorioPantalla == Convert.ToBoolean(RecordatorioPantalla)
                          && tu.RequiereRespuesta == Convert.ToBoolean(RequiereRespuesta)
                          && tu.CodContactoAgendo == CodUsuario
                          select tu.Id_TareaUsuario).Max();

            if (result == 0) { intTemp = result.ToString(); }
            else { intTemp = (0).ToString(); }
        }

        private void TareaUsuarioRepeticion()
        {
            String fecha = "" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            String sql = "INSERT INTO TareaUsuarioRepeticion (Fecha, CodTareaUsuario, Parametros) VALUES ('" + fecha + "','" + intTemp + "'," + "'CodProyecto=" + codProyecto + "&Campo=" + txtCampo + "'" + ")";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {

                conn.Open();
                cmd.ExecuteReader();
                conn.Close();

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La tarea " + NomTarea + "  ha sido agendada.')", true);
            }
            catch (SqlException se) { }
            finally { conn.Close(); }
        }

        private void enviarPorEmail(String toTxt, String asuntoTxt, String mensajeTxt)
        {
            string To;
            string Subject;
            string Body;

            MailMessage mail;
            Attachment Data;

            if (!(toTxt.Trim() == ""))
            {
                To = toTxt;
                Subject = asuntoTxt;
                Body = mensajeTxt;

                mail = new MailMessage();
                mail.To.Add(new MailAddress(To));
                mail.From = new MailAddress(usuario.Email);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = false;

                SmtpClient client = new SmtpClient("smtp.live.com", 587);
                using (client)
                {
                    client.Credentials = new System.Net.NetworkCredential(usuario.Email, usuario.GetPassword());
                    client.EnableSsl = true;
                    client.Send(mail);
                }
                System.Windows.Forms.MessageBox.Show("Mensaje enviado", "Correcto", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }
    }
}