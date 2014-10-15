using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Fonade.Clases;
using System.Configuration;

namespace Fonade.FONADE.interventoria
{
    public partial class AdicionarVisita : Negocio.Base_Page
    {
        string idVisita;
        string tipo;
        string txtSQL;

        DataTable RSVisita;
        DataTable RS;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Establecer valores en las cajas de texto.
                DateTime fecha = DateTime.Today;
                string sMes = UppercaseFirst(fecha.ToString("MMM"));
                txtDate.Text = fecha.ToString("dd") + "/" + sMes + "/" + fecha.ToString("yyyy"); //.ToString("dd/MMM/yyyy");
                txtDate2.Text = fecha.ToString("dd") + "/" + sMes + "/" + fecha.ToString("yyyy"); //.ToString("dd/MMM/yyyy");

                idVisita = Session["Id_Visita"] != null && !string.IsNullOrEmpty(Session["Id_Visita"].ToString()) ? Session["Id_Visita"].ToString() : "0";
                tipo = Session["Tipo"] != null && !string.IsNullOrEmpty(Session["Tipo"].ToString()) ? Session["Tipo"].ToString() : "0";

                if (idVisita == "0")
                {
                    #region Procesar la información de la visita a crear.

                    pnl_tarea_agendada.Visible = false;
                    pnl_tarea_a_crear.Visible = true;

                    #endregion
                }
                else
                {
                    #region Procesar la información de la visita agendada.

                    if (tipo.Equals("2"))
                    {
                        ddlempresa.Enabled = false;

                        txtSQL = "SELECT * FROM Visita v, Empresa e, Ciudad c WHERE v.Id_Empresa = e.Id_Empresa AND e.CodCiudad = c.Id_Ciudad AND v.id_visita = " + idVisita;

                        RSVisita = consultas.ObtenerDataTable(txtSQL, "text");
                    }

                    try
                    {
                        if (RSVisita != null && RSVisita.Rows.Count > 0)
                        {
                            //Cuando es de tipo 2, se carga el Id_Vista en variable oculta para usarlo en inserción de nueva 
                            //viista de tipo 2.
                            hdt_t2_NumVisita.Value = RSVisita.Rows[0]["Id_Visita"].ToString();
                            ddlempresa.SelectedValue = RSVisita.Rows[0]["nit"].ToString();
                            lblFechaInicio.Text = RSVisita.Rows[0]["FechaInicio"].ToString();
                            lblFechaFin.Text = RSVisita.Rows[0]["FechaFin"].ToString();
                            lblnitempresa.Text = RSVisita.Rows[0]["nit"].ToString();
                            lblciudad.Text = RSVisita.Rows[0]["NomCiudad"].ToString();
                            lblobjeto.Text = RSVisita.Rows[0]["Objeto"].ToString();
                        }
                    }
                    catch (NullReferenceException) { }

                    #endregion
                }

                //Cargar DropDownLists.
                CargarDropDownLists();

                //Cargar la ciduad por defecto "como en FONADE clásico".
                CargarCiudad();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 16/05/2014.
        /// Cargar los DropDownLists.
        /// </summary>
        private void CargarDropDownLists()
        {
            txtSQL = " SELECT DISTINCT p.Id_Proyecto AS CodProyecto, p.NomProyecto, e.nit, e.id_empresa, e.razonsocial, ciudad.id_ciudad AS CodCiudad, ciudad.nomciudad " +
                     " FROM Empresa e INNER JOIN Proyecto p ON e.codproyecto = p.Id_Proyecto  INNER JOIN EmpresaInterventor ei ON e.id_empresa = ei.CodEmpresa " +
                     " INNER JOIN Ciudad ON e.CodCiudad = Ciudad.Id_Ciudad AND p.CodCiudad = Ciudad.Id_Ciudad  WHERE (ei.CodContacto = " + usuario.IdContacto + ") " +
                     " AND (ei.Inactivo = 0) ORDER BY e.razonsocial ";

            RS = consultas.ObtenerDataTable(txtSQL, "text");

            DD_Empresas.Items.Clear();
            ddlempresa.Items.Clear();

            ListItem ll_item = new ListItem();
            ll_item.Text = "Seleccione...";
            ll_item.Value = "";
            DD_Empresas.Items.Add(ll_item);
            ddlempresa.Items.Add(ll_item);

            if (RS.Rows.Count > 0)
            {
                foreach (DataRow item in RS.Rows)
                {
                    ll_item = new ListItem();

                    ll_item.Value = item["razonsocial"].ToString();
                    ll_item.Text = item["nit"].ToString();
                    DD_Empresas.Items.Add(ll_item);

                    ll_item.Value = item["nit"].ToString();
                    ll_item.Text = item["razonsocial"].ToString();
                    ddlempresa.Items.Add(ll_item);
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 16/05/2014.
        /// Enviar Email "método de FONADE clásico".
        /// </summary>
        /// <param name="txtFrom">De:</param>
        /// <param name="txtSubject">Para:</param>
        /// <param name="txtMessage">Cuerpo del mensaje.</param>
        private void EnviarMail(string txtFrom, string txtSubject, string txtMessage)
        {
            //Inicializar variables.
            DataTable RSContacto = new DataTable();
            DataTable RS = new DataTable();

            try
            {
                //Consulta.
                txtSQL = "Select nombres, apellidos, identificacion from contacto where id_contacto = " + usuario.IdContacto;

                //Asignar resultados de la consulta a variable DataTable
                RSContacto = consultas.ObtenerDataTable(txtSQL, "text");

                //Información de los emprendedores de la Empresa.
                txtSQL = " SELECT Empresa.razonsocial, Empresa.codproyecto, ProyectoContacto.CodContacto " +
                         " FROM Empresa INNER JOIN ProyectoContacto ON Empresa.codproyecto = ProyectoContacto.CodProyecto " +
                         " WHERE (ProyectoContacto.CodRol = " + Datos.Constantes.CONST_RolEmprendedor + ") " +
                         " AND (ProyectoContacto.Inactivo = 0) " +
                         " AND (Empresa.Nit = '" + DD_Empresas.SelectedValue + "')";

                //Asignar resultados de la consulta anterior a variable DataTable.
                RS = consultas.ObtenerDataTable(txtSQL, "text");

                //Recorrer filas del DataTable anterior para generar tareas pendientes.
                for (int i = 0; i < RS.Rows.Count; i++)
                {
                    string Mensaje = "<br/><br/><br/>" + RSContacto.Rows[0]["nombres"].ToString() + " " + RSContacto.Rows[0]["apellidos"].ToString() +
                                     "<br/>C.C. " + RSContacto.Rows[0]["identificacion"].ToString();

                    //Instancia de la clase "Tarea".
                    AgendarTarea tarea = new AgendarTarea(Int32.Parse(RSContacto.Rows[0]["identificacion"].ToString()), "Visita de Interventoría",
                         Mensaje, RS.Rows[i]["CodProyecto"].ToString(), Datos.Constantes.CONST_Generica, "0", true, 1, true, false,
                         usuario.IdContacto, "", "", "Adicionar Visita");
                    //Agendar tarea.
                    tarea.Agendar();
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 16/05/2014.
        /// Agenadar nueva visita.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_agendar_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            DateTime? FechaInicio = new DateTime();
            DateTime? FechaFin = new DateTime();
            //bool correcto = false;

            //Convertir las fecha seleccionadas.
            try
            {
                FechaInicio = Convert.ToDateTime(txtDate.Text);
                FechaFin = Convert.ToDateTime(txtDate2.Text);
            }
            catch
            { FechaInicio = null; FechaFin = null; }

            try
            {
                #region Validaciones.

                tipo = Session["Tipo"] != null && !string.IsNullOrEmpty(Session["Tipo"].ToString()) ? Session["Tipo"].ToString() : "0";
                if (tipo == "0") { return; }

                //if (CalendarExtender1.SelectedDate == null || CalendarExtender2.SelectedDate == null)
                if (FechaInicio == null || FechaFin == null)
                { return; }

                //if (CalendarExtender1.SelectedDate < DateTime.Today)
                if (FechaInicio < DateTime.Today)
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Imposible Agendar una visita con Fecha Inicial menor que la fecha actual.')", true);
                    return;
                }
                //if (CalendarExtender2.SelectedDate < CalendarExtender1.SelectedDate)
                if (FechaFin < FechaInicio)
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La Fecha de Fin debe ser más reciente que la fecha de Inicio.')", true);
                    return;
                }
                if (TXT_objeto.Text.Trim() == "")
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe especificar el Objeto de la Visita!!!.')", true);
                    return;
                }
                if (DD_Empresas.SelectedValue == "" || DD_Empresas.SelectedValue == "0")
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Por favor, Seleccione una Empresa.')", true);
                    return;
                }
                #endregion

                if (tipo == "1")
                {
                    #region Consulta e inserción para el tipo 1.

                    //Consulta.
                    txtSQL = "Select Id_Empresa from Empresa where nit = '" + DD_Empresas.SelectedValue + "' ";

                    //Asignación de resultados de la consulta anterior a variable DataTable.
                    var c_1 = consultas.ObtenerDataTable(txtSQL, "text");

                    //Si la tabla tiene datos...
                    if (c_1.Rows.Count > 0)
                    {
                        //Ejecutar sp:
                        consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@Id_Interventor" ,Value = usuario.IdContacto
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@Id_Empresa" ,Value = c_1.Rows[0]["Id_Empresa"].ToString()
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@FechaInicio" ,Value = FechaInicio
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@FechaFin" ,Value = FechaFin
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@Estado" ,Value = "Pendiente"
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@Objeto" ,Value = TXT_objeto.Text.Trim()
                                                   }
                                           };

                        //Asignar resultados de la consulta a la variable DataTable.
                        var t_1 = consultas.ObtenerDataTable("MD_AdicionarVisita_Tipo1");

                        //Si tiene un valor, significa que sí guardó datos.
                        if (t_1.Rows.Count > 0)
                        {
                            //Información de los emprendedores de la empresa.
                            string txtFrom = "Fonade";
                            string txtSubject = "Se ha programado una visita!!!";
                            string txtMessage = "<br/><br/> Ha sido Programada una visita a <b>" + DD_Empresas.SelectedItem.Text + "</b>" +
                                                "<br/><br><br>Fecha de Inicio: " + FechaInicio +
                                                "<br/><br>Fecha de Finalización: " + FechaFin +
                                                "<br/><br>Objeto de la visita: " + TXT_objeto.Text.Trim() + " <br/>" +
                                                "<br/><br><br>Gracias por su atención.";

                            //Enviar Mail
                            EnviarMail(txtFrom, txtSubject, txtMessage);
                            //Salir.
                            //Response.Redirect("InterventorAgenda.aspx");
                            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Nueva visita agendada correctamente.');window.close();", true);
                            return;
                        }
                        else
                        {
                            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo generar la nueva visita.')", true);
                            return;
                        }

                        #region NO BORRAR.
                        ////Inserción.
                        //txtSQL = "INSERT INTO Visita(Id_Interventor, Id_Empresa, FechaInicio, FechaFin, Estado, Objeto) " +
                        //        " VALUES (" + usuario.IdContacto + ", " + c_1.Rows[0]["Id_Empresa"].ToString() + ", '"
                        //        + FechaInicio + "', '" + FechaFin + "', 'Pendiente', '" + TXT_objeto.Text.Trim() + "') ";

                        //SqlCommand cmd = new SqlCommand(txtSQL, conn);
                        //correcto = EjecutarSQL(conn, cmd);

                        //if (correcto == true)
                        //{
                        //    //Información de los emprendedores de la empresa.
                        //    string txtFrom = "Fonade";
                        //    string txtSubject = "Se ha programado una visita!!!";
                        //    string txtMessage = "<br/><br/> Ha sido Programada una visita a <b>" + DD_Empresas.SelectedItem.Text + "</b>" +
                        //                        "<br/><br><br>Fecha de Inicio: " + FechaInicio +
                        //                        "<br/><br>Fecha de Finalización: " + FechaFin +
                        //                        "<br/><br>Objeto de la visita: " + TXT_objeto.Text.Trim() + " <br/>" +
                        //                        "<br/><br><br>Gracias por su atención.";

                        //    //Enviar Mail
                        //    EnviarMail(txtFrom, txtSubject, txtMessage);
                        //    //Salir.
                        //    //Response.Redirect("InterventorAgenda.aspx");
                        //    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Nueva visita agendada correctamente.');window.close();", true);
                        //    return;
                        //}
                        //else
                        //{
                        //    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo generar la nueva visita.')", true);
                        //    return;
                        //} 
                        #endregion
                    }

                    #endregion
                }
                if (tipo == "2")
                {
                    #region Consulta e inserción para el tipo 2.

                    //Ejecutar sp:
                    consultas.Parameters = new[]
                                           {
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@FechaInicio" ,Value = FechaInicio
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@FechaFin" ,Value = FechaFin
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@Objeto" ,Value = TXT_objeto.Text.Trim()
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@Id_Visita" ,Value = hdt_t2_NumVisita.Value
                                                   }
                                           };

                    //Asignar resultados de la consulta a la variable DataTable.
                    var t_2 = consultas.ObtenerDataTable("MD_AdicionarVisita_Tipo1");

                    //Si tiene un valor, significa que sí guardó datos.
                    if (t_2.Rows.Count > 0)
                    {
                        //Realizar siguiente consulta:
                        txtSQL = "Select razonsocial from Empresa where nit = '" + DD_Empresas.SelectedValue + "'";

                        //Asignar resultados de la consulta anterior a variable DataTable
                        var c_2 = consultas.ObtenerDataTable(txtSQL, "text");

                        //Si la tabla anterior contiene datos...
                        if (c_2.Rows.Count > 0)
                        {
                            //Nombre de la empresa
                            String NombreEmpresa = c_2.Rows[0]["razonsocial"].ToString();

                            //Cuerpo del mensaje.
                            string txtFrom = "Fonade";
                            string txtSubject = "Se ha modificado una visita!!!";
                            string txtMessage = "<br/><br/>La visita programada a <b>" + NombreEmpresa + "</b> ha sido modificada de la siguiente manera: " +
                                                "<br/><br><br>Fecha de Inicio: " + txtDate.Text +
                                                "<br/><br>Fecha de Finalización: " + txtDate2.Text +
                                                "<br/><br>Objeto de la visita: " + TXT_objeto.Text.Trim() + "<br/>" +
                                                "<br/><br><br>Gracias por su atención.";

                            //Enviar mensaje.
                            EnviarMail(txtFrom, txtSubject, txtMessage);

                            //Salir.
                            //Response.Redirect("InterventorAgenda.aspx");
                            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Nueva visita agendada correctamente.');window.close();", true);
                            return;
                        }
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo actualizar la visita.')", true);
                        return;
                    }

                    #region NO BORRAR.
                    ////Consulta.
                    //txtSQL = "UPDATE Visita SET FechaInicio = '" + FechaInicio +
                    //          "', FechaFin = '" + FechaFin + "', Objeto = '" + TXT_objeto.Text.Trim() +
                    //          "' WHERE Id_Visita = " + hdt_t2_NumVisita.Value;

                    ////Inicializar variables para ejecutar consulta.
                    //SqlCommand cmd = new SqlCommand(txtSQL, conn);
                    //correcto = EjecutarSQL(conn, cmd);

                    ////Si la ejecución de la consulta SQL fue correcta, debe seguir con el flujo del sistema.
                    //if (correcto == true)
                    //{
                    //    //Realizar siguiente consulta:
                    //    txtSQL = "Select razonsocial from Empresa where nit = '" + DD_Empresas.SelectedValue + "'";

                    //    //Asignar resultados de la consulta anterior a variable DataTable
                    //    var c_2 = consultas.ObtenerDataTable(txtSQL, "text");

                    //    //Si la tabla anterior contiene datos...
                    //    if (c_2.Rows.Count > 0)
                    //    {
                    //        //Nombre de la empresa
                    //        String NombreEmpresa = c_2.Rows[0]["razonsocial"].ToString();

                    //        //Cuerpo del mensaje.
                    //        string txtFrom = "Fonade";
                    //        string txtSubject = "Se ha modificado una visita!!!";
                    //        string txtMessage = "<br/><br/>La visita programada a <b>" + NombreEmpresa + "</b> ha sido modificada de la siguiente manera: " +
                    //                            "<br/><br><br>Fecha de Inicio: " + txtDate.Text +
                    //                            "<br/><br>Fecha de Finalización: " + txtDate2.Text +
                    //                            "<br/><br>Objeto de la visita: " + TXT_objeto.Text.Trim() + "<br/>" +
                    //                            "<br/><br><br>Gracias por su atención.";

                    //        //Enviar mensaje.
                    //        EnviarMail(txtFrom, txtSubject, txtMessage);

                    //        //Salir.
                    //        //Response.Redirect("InterventorAgenda.aspx");
                    //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Nueva visita agendada correctamente.');window.close();", true);
                    //        return;
                    //    }
                    //}
                    //else
                    //{
                    //    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo actualizar la visita.')", true);
                    //    return;
                    //} 
                    #endregion

                    #endregion
                }
            }
            catch
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al generar la nueva visita.')", true);
                return;
            }

        }

        /// <summary>
        /// SeelctedIndexChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DD_Empresas_SelectedIndexChanged(object sender, EventArgs e)
        { CargarCiudad(); }

        /// <summary>
        /// Establecer el texto del TextBox "TXT_ciudad" de acuerdo a la selección del 
        /// DropDownList "DD_Empresas".
        /// </summary>
        private void CargarCiudad()
        {
            try
            {
                TXT_nit.Text = DD_Empresas.SelectedValue;

                txtSQL = "SELECT NomCiudad FROM Ciudad c, Empresa e WHERE c.Id_Ciudad = e.CodCiudad AND e.nit = '" + TXT_nit.Text + "'";
                var rt = consultas.ObtenerDataTable(txtSQL, "text");

                if (rt.Rows.Count > 0) { TXT_ciudad.Text = rt.Rows[0]["NomCiudad"].ToString(); } else { TXT_ciudad.Text = lblciudad.Text; }

                rt = null;
            }
            catch { }
        }

        /// <summary>
        /// Colocar en mayúscula la primera letra del parámetro.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}