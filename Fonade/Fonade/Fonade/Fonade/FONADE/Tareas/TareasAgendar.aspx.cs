using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Account;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Fonade.Clases;

namespace Fonade.FONADE.Tareas
{
    public partial class TareasAgendar : Negocio.Base_Page/*System.Web.UI.Page*/
    {
        #region Variables globales.

        /// <summary>
        /// Tarea seleccionada.
        /// </summary>
        Int32 Id_TareaUsuarioRepeticion;
        /// <summary>
        /// Variable que contiene las consultas SQL.
        /// </summary>
        String txtSQL;

        #endregion

        protected void lds_tareas_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime? fechaActual = DateTime.Today;
            //calExtender2.SelectedDate = fechaActual;
            DateTime fechamostar = DateTime.Today; //DateTime.Now.AddDays(1); //OJO! han reportado varias veces que sea a fecha siguiente al día actual!.
            txtDate2.Text = fechamostar.ToString("dd/MM/yyyy");//ToString("MM/dd/yyyy");

            Id_TareaUsuarioRepeticion = Session["Id_tareaRepeticion"] != null ? Id_TareaUsuarioRepeticion = Convert.ToInt32(Session["Id_tareaRepeticion"].ToString()) : 0;

            if (!IsPostBack)
            {
                if (Id_TareaUsuarioRepeticion != 0)
                {
                    menuMostar();
                    tbl1.Visible = false;
                    tbl1.Enabled = false;
                    Panel2.Visible = true;
                    Panel2.Enabled = true;
                    lbl_Titulo.Text = "REVISAR TAREA";
                }
                else
                {
                    CargarPlanesDeNegocio();
                    tbl1.Visible = true;
                    tbl1.Enabled = true;
                    Panel2.Visible = false;
                    Panel2.Enabled = false;
                    lbl_Titulo.Text = "AGENDAR TAREA";
                }
            }
            else { }
        }

        protected void Correo(string mailusuario, string asunto)
        {
            try
            {
                #region Email anterior.
                //MailMessage mailMessage = new MailMessage();
                //mailMessage.To.Add(mailusuario.Trim());
                //mailMessage.From = new MailAddress("lawrent.suelta@glogic.com.co");
                //mailMessage.Subject = "ASP.NET e-mail test";
                //mailMessage.Body = "Hello world,\n\nThis is an ASP.NET test e-mail!";
                //SmtpClient smtp = new SmtpClient();
                //smtp.EnableSsl = true;
                //smtp.Send(mailMessage);
                //Response.Write("E-mail sent!"); 
                #endregion
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Could not send the e-mail - error: " + ex.Message + "')", true);
                return;
            }
        }

        protected void Button1_click(object sender, EventArgs e)
        {
            if (ListBox1.Items.Count != 0)
            {
                string listausuarios = "<ul>";

                foreach (ListItem li in ListBox1.Items)
                {
                    if (li.Selected)
                    {
                        listausuarios = listausuarios + "<li>" + li.Text + "</li>";

                        //AgendarTarea agenda = new AgendarTarea(Int32.Parse(li.Value), tb_tarea.Text, tb_descripcion.Text, DropDownList1.SelectedValue, 1, "0", Convert.ToBoolean(ddl_avisar.SelectedValue), Int32.Parse(ddl_urgencia.SelectedValue.ToString()), false, false, usuario.IdContacto, null, null, null);
                        AgendarTarea agenda = new AgendarTarea(Int32.Parse(li.Value), tb_tarea.Text, tb_descripcion.Text, plan_seleccionado.Value, 1, "0", Convert.ToBoolean(ddl_avisar.SelectedValue), Int32.Parse(ddl_urgencia.SelectedValue.ToString()), false, false, usuario.IdContacto, null, null, null);

                        agenda.Agendar();

                        //if (ddl_avisar.SelectedValue == "Sí" || ddl_avisar.SelectedValue == "True")
                        //{
                        //    Correo("pruebasfonade@glogic.com.co", tb_tarea.Text);
                        //    // this.Correo("pruebasfonade@glogic.com.co", tb_tarea.Text);
                        //}
                    }

                }
                listausuarios = listausuarios + "</ul>";
                if (ddl_avisar.SelectedValue == "Sí" || ddl_avisar.SelectedValue == "True")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('se ha enviado correo a los siguiente usuarios: " + listausuarios + "');window.location='TareasAgendar.aspx'", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Tarea " + tb_tarea.Text + " Agendada');window.location='TareasAgendar.aspx'", true);
                }
            }
        }

        protected void ListBox1_OnSelectedIndexChanged(object sender, EventArgs e)
        {


        }

        protected void btn_Grabar_Click(object sender, EventArgs e)
        {
            String txtSQL = "UPDATE TareaUsuarioRepeticion SET ";

            if (CheckBox1.Checked)
            {
                txtSQL += " FechaCierre = '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "'";
            }
            else
            {
                txtSQL += " FechaCierre = null";
            }

            if (String.IsNullOrEmpty(TextBox9.Text))
            {
                txtSQL += ", Respuesta = ''";
            }
            else
            {
                txtSQL += " , Respuesta = '" + TextBox9.Text + "'";
            }

            txtSQL += " WHERE Id_TareaUsuarioRepeticion = " + Id_TareaUsuarioRepeticion;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(txtSQL, conn);
            try
            {

                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            }
            catch (SqlException se) { }
            finally
            {
                conn.Close();
            }

            menuMostar();
        }

        private void menuMostar()
        {
            consultas.Parameters = new[] { new SqlParameter
                                                   { 
                                                        ParameterName = "@Id_TareaUsuarioRepeticion",
                                                        Value = Id_TareaUsuarioRepeticion
                                                   }
                };
            DataTable dtActas = consultas.ObtenerDataTable("MP_ReporteTareasUsuario");

            try
            {
                TextBox1.Text = dtActas.Rows[0]["NomUsuarioAgendo"].ToString();
                TextBox2.Text = dtActas.Rows[0]["NomUsuario"].ToString();
                TextBox3.Text = dtActas.Rows[0]["NomTareaPrograma"].ToString();
                TextBox4.Text = dtActas.Rows[0]["NomProyecto"].ToString();

                if (String.IsNullOrEmpty(TextBox4.Text))
                {
                    tr_planNegocio.Visible = false;
                    tr_planNegocio.Attributes.Add("display", "none");
                    //TextBox4.Visible = false;//correcta
                    //TextBox4.Text = "---------";
                }

                #region Mostrar el link "Aprobación Solicitud de Pago" PENDIENTE REDIRECCIÓN A "PagosActividadInter.aspx".

                try
                {
                    if (!String.IsNullOrEmpty(dtActas.Rows[0]["Ejecutable"].ToString()))
                    {
                        //Adicionar evento dinámico.
                        lnk_SolicitudPago.Visible = true;
                        if (lnk_SolicitudPago.CommandArgument.ToString().Trim() == "")
                        { lnk_SolicitudPago.CommandArgument = dtActas.Rows[0]["Ejecutable"].ToString() + ";" + dtActas.Rows[0]["Parametros"].ToString(); }
                        //lnk_SolicitudPago.Command += new CommandEventHandler(DynamicCommand_SolicitudPago);
                        lnk_SolicitudPago.Text = dtActas.Rows[0]["NomTareaUsuario"].ToString();//Ejecutable
                        TextBox5.Visible = false;

                        if (TextBox5.Visible)
                        {
                            TextBox5.Text = dtActas.Rows[0]["NomTareaUsuario"].ToString();
                            if (TextBox5.Text.Contains("+#39;")) { TextBox5.Text = TextBox5.Text.Replace("+#39;", "'"); }
                        }
                    }
                }
                catch { TextBox5.Text = ""; lnk_SolicitudPago.Visible = false; }

                #endregion


                TextBox6.Text = dtActas.Rows[0]["Descripcion"].ToString();
                if (TextBox6.Text.Contains("+#39;")) { TextBox6.Text = TextBox6.Text.Replace("+#39;", "'"); }
                TextBox7.Text = dtActas.Rows[0]["SoloFecha"].ToString() + " " + dtActas.Rows[0]["SoloHora"].ToString();

                switch (Int32.Parse(dtActas.Rows[0]["NivelUrgencia"].ToString()))
                {
                    case 1:
                        //TextBox8.Text = "Muy Alta";
                        img_urgencia.ImageUrl = "../../Images/Tareas/Urgencia1.gif";
                        img_urgencia.ToolTip = "Muy Alta";
                        lblUrgencia_Text.Text = img_urgencia.ToolTip;
                        img_urgencia.AlternateText = img_urgencia.ToolTip;
                        break;
                    case 2:
                        //TextBox8.Text = "Alta";
                        img_urgencia.ImageUrl = "../../Images/Tareas/Urgencia2.gif";
                        img_urgencia.ToolTip = "Alta";
                        lblUrgencia_Text.Text = img_urgencia.ToolTip;
                        img_urgencia.AlternateText = img_urgencia.ToolTip;
                        break;
                    case 3:
                        //TextBox8.Text = "Normal";
                        img_urgencia.ImageUrl = "../../Images/Tareas/Urgencia3.gif";
                        img_urgencia.ToolTip = "Normal";
                        lblUrgencia_Text.Text = img_urgencia.ToolTip;
                        img_urgencia.AlternateText = img_urgencia.ToolTip;
                        break;
                    case 4:
                        //TextBox8.Text = "Baja";
                        img_urgencia.ImageUrl = "../../Images/Tareas/Urgencia4.gif";
                        img_urgencia.ToolTip = "Baja";
                        lblUrgencia_Text.Text = img_urgencia.ToolTip;
                        img_urgencia.AlternateText = img_urgencia.ToolTip;
                        break;
                    default:
                        //TextBox8.Text = "Muy Baja";
                        img_urgencia.ImageUrl = "../../Images/Tareas/Urgencia5.gif";
                        img_urgencia.ToolTip = "Muy Baja";
                        lblUrgencia_Text.Text = img_urgencia.ToolTip;
                        img_urgencia.AlternateText = img_urgencia.ToolTip;
                        break;
                }

                TextBox9.Text = dtActas.Rows[0]["Respuesta"].ToString();

                if (String.IsNullOrEmpty(dtActas.Rows[0]["FechaCierre"].ToString()))
                {
                    CheckBox1.Enabled = true;
                    CheckBox1.Checked = false;
                }
                else
                {
                    CheckBox1.Checked = true;
                    CheckBox1.Enabled = false;
                    TextBox9.Enabled = false;
                }
            }
            catch (Exception) { }
        }

        #region Código comentado NO BORRAR!.
        //protected void ldsddl_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        //{
        //    var result = (from pn in consultas.Db.MD_PlanNegocio(usuario.IdContacto, usuario.CodGrupo, usuario.CodInstitucion)
        //                  select new
        //                  {
        //                      pn.NomProyecto,
        //                      pn.Id_Proyecto
        //                  });

        //    e.Result = result.ToList();
        //} 
        #endregion

        private void CargarPlanesDeNegocio()
        {
            //Inicializar variables.
            DataTable tabla = new DataTable();

            try
            {
                #region Validar según el rol/grupo del usuario, qué planes de negocio cargarán. COMENTADA!

                //if (usuario.CodGrupo == Constantes.CONST_Asesor || usuario.CodGrupo == Constantes.CONST_Emprendedor)
                //{
                //    txtSQL = " SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0 " +
                //             " and CodInstitucion = @Cod_institucion and  exists (select codproyecto from proyectocontacto pc where id_proyecto=codproyecto and pc.codcontacto= @id_usuario and pc.inactivo=0) ";
                //}
                //if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                //{
                //    txtSQL = " SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0 and CodInstitucion = " + usuario.CodInstitucion;
                //}

                //if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador)
                //{
                //    txtSQL = " SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0 " +
                //             " and CodEstado = " + Constantes.CONST_Evaluador +
                //             " ORDER BY NomProyecto";
                //}
                //if (usuario.CodGrupo == Constantes.CONST_Evaluador || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
                //{
                //    txtSQL = " SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0  " +
                //             " and exists (select codproyecto from proyectocontacto pc where id_proyecto = codproyecto and pc.codcontacto = " + usuario.IdContacto +
                //             " and pc.inactivo = 0) ";
                //}
                //if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                //{
                //    txtSQL = " SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0 and CodEstado IN (6,7) ";
                //}

                //if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                //{
                //    txtSQL = " SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0 " +
                //             " and  exists (select codproyecto from proyectocontacto pc where id_proyecto = codproyecto " +
                //             " and pc.codcontacto = " + usuario.IdContacto + " and pc.inactivo = 0)";
                //}

                //if (usuario.CodGrupo == Constantes.CONST_Interventor)
                //{
                //    txtSQL = " SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0 and " +
                //             " Id_Proyecto IN (SELECT Empresa.codproyecto FROM Empresa INNER JOIN EmpresaInterventor " +
                //             " ON Empresa.id_empresa = EmpresaInterventor.CodEmpresa WHERE (EmpresaInterventor.Inactivo = 0) " +
                //             " AND (EmpresaInterventor.CodContacto = " + usuario.IdContacto + "))";
                //}

                #endregion

                #region Según FONADE clásico.

                txtSQL = "SELECT Id_Proyecto, NomProyecto FROM Proyecto WHERE Inactivo = 0 ";

                switch (usuario.CodGrupo)
                {
                    case Constantes.CONST_Asesor:
                    case Constantes.CONST_Emprendedor:
                        txtSQL = txtSQL + " and CodInstitucion = " + usuario.CodInstitucion +
                                          " and  exists (select codproyecto from proyectocontacto pc " +
                                          " where id_proyecto = codproyecto and pc.codcontacto = " + usuario.IdContacto + " and pc.inactivo = 0) ";
                        break;

                    case Constantes.CONST_JefeUnidad:
                        txtSQL = txtSQL + " and CodInstitucion = " + usuario.IdContacto;
                        break;

                    case Constantes.CONST_GerenteEvaluador:
                        txtSQL = txtSQL + " and CodEstado = " + Constantes.CONST_Evaluacion;
                        break;

                    case Constantes.CONST_CoordinadorEvaluador:
                    case Constantes.CONST_Evaluador:
                        txtSQL = txtSQL + " and  exists (select codproyecto from proyectocontacto pc " +
                                          " where id_proyecto = codproyecto and pc.codcontacto = " + usuario.IdContacto + " and pc.inactivo = 0) ";
                        break;

                    //Interventoria.
                    case Constantes.CONST_GerenteInterventor:
                        txtSQL = txtSQL + " and CodEstado IN (" + Constantes.CONST_LegalizacionContrato + "," + Constantes.CONST_Ejecucion + ")";
                        break;

                    //OJO!!!!!!!! Pendiente de definicion del perfil Interventor
                    case Constantes.CONST_CoordinadorInterventor:
                        txtSQL = txtSQL + " and  exists (select codproyecto from proyectocontacto pc " +
                                          " where id_proyecto = codproyecto and pc.codcontacto = " + usuario.IdContacto + " and pc.inactivo = 0) ";
                        break;

                    case Constantes.CONST_Interventor:
                        txtSQL = txtSQL + " and  Id_Proyecto IN (SELECT Empresa.codproyecto FROM Empresa INNER JOIN EmpresaInterventor ON Empresa.id_empresa = EmpresaInterventor.CodEmpresa " +
                                          " WHERE (EmpresaInterventor.Inactivo = 0) AND (EmpresaInterventor.CodContacto = " + usuario.IdContacto + ")) ";
                        break;
                    default:
                        break;
                }

                txtSQL = txtSQL + " ORDER BY NomProyecto";

                #endregion

                //Limpiar los elementos que puedan contener el DropDownList.
                DropDownList1.Items.Clear();

                //Generar el ítem vacío por defecto.
                ListItem item_default = new ListItem();
                item_default.Text = "";
                item_default.Value = "";
                DropDownList1.Items.Add(item_default);

                if (txtSQL.Trim() != "")
                {
                    tabla = consultas.ObtenerDataTable(txtSQL, "text");

                    foreach (DataRow row in tabla.Rows)
                    {
                        ListItem items = new ListItem();
                        items.Text = row["NomProyecto"].ToString();
                        items.Value = row["Id_Proyecto"].ToString();
                        DropDownList1.Items.Add(items);
                    }
                }
            }
            catch { }
        }

        protected void ldslistbox_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var result = (from atr in consultas.Db.MD_AgendarTareas_Prueba(usuario.IdContacto, usuario.CodGrupo, usuario.CodInstitucion, "TraerUsuarios", null, null, null, null, null, null, null, null, null, null, null, null)
                          select new
                          {
                              atr.Id_Contacto,
                              //atr.Nombre
                              Nombre = atr.Nombre + " (" + atr.NombreRol + ")"
                          });

            e.Result = result.ToList();
        }

        protected void ldscontacto_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var result = (from tp in consultas.Db.TareaProgramas
                          where tp.delSistema == null | tp.delSistema != 1
                          select new
                          {
                              Id_Tarea = tp.Id_TareaPrograma,
                              Nombre_Tarea = tp.NomTareaPrograma,
                          });

            e.Result = result.ToList();
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.plan_seleccionado.Value = DropDownList1.SelectedValue;
        }

        /// <summary>
        /// Evento OnClick para enviar por sesión los parámetros y mostrar así la ventana emergente.
        /// SE HA DEBE MODIFICAR LA FUNCIOALIDAD YA QUE EL CAMPO "Parametros" NO SIEMPRE TIENE
        /// LOS DATOS REQUERIDOS PARA LLAMAR A LA VENTANA EMERGENTE.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnk_SolicitudPago_Click(object sender, EventArgs e)
        {
            #region Código generado gracias a búsqueda en StackOverFlow y ajustes propios.

            try
            {
                //Obtener el CommandArgument del botón clickeado.
                //http://stackoverflow.com/questions/5381139/linkbutton-send-value-to-code-behind-onclick
                LinkButton btn = (LinkButton)(sender);

                //Obtiene los valores del CommandArgument a procesar.
                var valores_command = btn.CommandArgument.ToString().Split(';');

                //Crea la variable "Ejecutable" (que contiene el nombre de la página a redireccionar).
                string Ejecutable = valores_command[0];
                //Evalúo, si la página contiene la parte ".asp", se cambiará a ".aspx".
                if (Ejecutable.Contains(".asp")) { Ejecutable = Ejecutable.Replace(".asp", ".aspx"); }

                //Crea la variable "inputString" (que contiene los parámetros de la página).
                string inputString = valores_command[1];

                //Listas que contendrán la división de los valores procesados.
                var valores_ampersand = new string[] { };
                var valores_equals = new string[] { };

                //Divido en strings los valores que tengan como delimitador el ampersand "&".
                valores_ampersand = inputString.Split('&');

                //Variable que determina si puede abrir la ventana emergente "o no".
                bool AbrirVentana = false;

                //Recorrer cada valor obtenidos al separarlos por los ampersands.
                foreach (string item in valores_ampersand)
                {
                    //Inicializo las variables "esta vez, para separarlos por el signo (=)"
                    valores_equals = new string[] { };
                    valores_equals = item.Split('=');

                    //Crea las variables de sesión.
                    Session[valores_equals[0]] = valores_equals[1];

                    //Permitir "cuando salga del ciclo" generar la ventana emergente.
                    AbrirVentana = true;
                }

                if (AbrirVentana)
                {
                    //Verificar la ubicación de la página antes de redireccionarlo.
                    switch (Ejecutable)
                    {
                        case "AdicionarInformeBimensual.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "AdicionarInformePresupuestal.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "AgregarInformeFinalInterventoria.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "CatalogoActividadPO.aspx":
                            Ejecutable = "../evaluacion/" + Ejecutable;
                            break;
                        case "CatalogoIndicadorInter.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "CatalogoInterventorTMP.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "CatalogoProduccionTMP.aspx":
                            Ejecutable = "../evaluacion/" + Ejecutable;
                            break;
                        case "CatalogoRiesgoInter.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "CatalogoUnidadEmprende.aspx":
                            Ejecutable = "../Administracion/" + Ejecutable;
                            break;
                        case "CatalogoVentasTMP.aspx":
                            Ejecutable = "../evaluacion/" + Ejecutable;
                            break;
                        case "FrameAsesorProyecto.aspx":
                            Ejecutable = "../AdministrarPerfiles/" + Ejecutable;
                            break;
                        case "FrameCoordinadorEvaluador.aspx":
                            break;
                        case "FrameEvaluadorProyecto.aspx":
                            break;
                        case "FrameInterventorProyecto.aspx":
                            break;
                        case "PagosActividad.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "PagosActividadCoord.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        case "PagosActividadInter.aspx":
                            Ejecutable = "../interventoria/" + Ejecutable;
                            break;
                        default:
                            break;
                    }

                    //Se crea una ventana emergente con los valores de sesión.
                    Redirect(null, Ejecutable, "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100");
                }
            }
            catch { /*No hace nada.*/ }

            #endregion
        }
    }
}