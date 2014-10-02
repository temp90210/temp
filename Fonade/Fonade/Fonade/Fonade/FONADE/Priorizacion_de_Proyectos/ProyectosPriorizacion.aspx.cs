using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Fonade.Account;
using LinqKit;
using AjaxControlToolkit;
using System.ComponentModel;


namespace Fonade.FONADE.Priorizacion_deProyectos
{

    public partial class ProyectosPriorizacion : Negocio.Base_Page
    {
        public bool columnasAdicionales;
        public int conteo;
        protected void Page_Load(object sender, EventArgs e)
        {
            txt_numeroacta.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            txt_fecha.Enabled = false;
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("PRIORIZACIÓN DE PROYECTOS");
                columnasAdicionales = true;
                conteo = 1;
                llenarConvocatorias(ddl_convocatoria);
                txt_fecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        protected void lds_proyectospriorizar_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var query = from P in consultas.Db.MD_MostrarProyectosaPriorizar(Constantes.CONST_AsignacionRecursos)
                            select P;
                e.Result = query;
            }
            catch (Exception)
            { }
        }

        protected void ChBSeleccionarTodo_CheckedChanged(object sender, EventArgs e)
        {
            bool Chequeado = false;
            if (ChBSeleccionarTodo.Checked)
            {
                Chequeado = true;
            }
            //foreach (GridViewRow grd_Row in this.gv_proyectospriorizar.Rows)
            //{
            //    ((CheckBox)grd_Row.FindControl("ChBSelct")).Checked = Chequeado;
            //}
            //((CheckBox)gv_proyectospriorizar.FindControl("ChBSelct")).Checked = Chequeado;

            llenarColumnas();

            foreach (GridViewRow gvr in gv_proyectospriorizar.Rows)
            {
                ((CheckBox)gvr.FindControl("ChBSelct")).Checked = Chequeado;
            }
        }

        protected void gv_proyectospriorizar_Load(object sender, EventArgs e)
        {
            if (columnasAdicionales)
            {
                var query = (from crit in consultas.Db.CriterioPriorizacions
                             select new
                             {
                                 crit.Id_Criterio,
                                 crit.Sigla,
                             });
                foreach (var num in query)
                {

                    TemplateField nuevaColumna = new TemplateField();
                    nuevaColumna.HeaderText = num.Sigla;

                    nuevaColumna.ItemTemplate = new labelasigado(conteo);
                    gv_proyectospriorizar.Columns.Add(nuevaColumna);
                }
                columnasAdicionales = false;
                llenarColumnas();
            }
            else
            { }
        }

        protected void llenarColumnas()
        {
            int indexColumn = 4;
            var query = (from crit in consultas.Db.CriterioPriorizacions
                         select new
                         {
                             crit.Id_Criterio,
                             crit.Sigla,
                         });

            foreach (var num in query)
            {
                indexColumn++;
                foreach (GridViewRow grd_Row in this.gv_proyectospriorizar.Rows)
                {
                    string codProyecto = ((Label)grd_Row.FindControl("codigo_proyecto")).Text;
                    string codConvoc = ((HiddenField)grd_Row.FindControl("hiddenIDConvoc")).Value;
                    var query2 = (from PCP in consultas.Db.PuntajeCriterioPriorizacions
                                  where PCP.CodConvocatoria == Convert.ToInt32(codConvoc)
                                  && PCP.CodProyecto == Convert.ToInt32(codProyecto)
                                  && PCP.CodCriterioPriorizacion == num.Id_Criterio
                                  select new
                                  {
                                      valorA = PCP.Valor,
                                  }).FirstOrDefault();
                    try
                    {
                        string nombre2 = "text" + conteo.ToString();
                        Label tx1 = new Label();
                        tx1.ID = nombre2;
                        if (query2 == null)
                        {
                            tx1.Text = "";
                        }
                        else
                        {
                            tx1.Text = Math.Round(query2.valorA, 2).ToString("0.00");
                        }
                        grd_Row.Cells[indexColumn].Controls.Add(tx1);
                        conteo++;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        protected void llenarConvocatorias(DropDownList lista)
        {
            var query = (from conv in consultas.Db.Convocatorias
                         where conv.Publicado == true
                         select new
                         {
                             idConv = conv.Id_Convocatoria,
                             nomConv = conv.NomConvocatoria,
                         }).OrderBy(t => t.nomConv);
            lista.DataSource = query.ToList();
            lista.DataValueField = "idConv";
            lista.DataTextField = "nomConv";
            lista.DataBind();
        }

        protected void btn_asignarRecursos_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            String txtSQL = "";
            DataTable rs = new DataTable();
            int IdActa = 0;

            try
            {
                if (Convert.ToDateTime(txt_fecha.Text) > DateTime.Now)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La fecha del acta no puede ser mayor a la actual!')", true);
                    return;
                }
                else
                {
                    #region Comentarios del SQL Anterior.
                    //string txtSQL;

                    //txtSQL = "INSERT INTO AsignacionActa (NomActa,NumActa,FechaActa,Observaciones,CodConvocatoria,Publicado)" +
                    // " VALUES('" + txt_nombreacta.Text + "','" + txt_numeroacta.Text + "','" + txt_fecha.Text + "','" + txt_observaciones.Text + "'," + ddl_convocatoria.SelectedValue + ",1)";

                    //ejecutaReader(txtSQL, 2);

                    //txtSQL = "SELECT Max(Id_Acta) AS Id_Acta FROM AsignacionActa " +
                    // " WHERE NomActa = '" + txt_nombreacta.Text + "'" +
                    // " AND NumActa = '" + txt_numeroacta.Text + "'" +
                    // " AND FechaActa = '" + txt_fecha.Text + "'" +
                    // " AND Observaciones = '" + txt_observaciones.Text + "'" +
                    // " AND CodConvocatoria = " + ddl_convocatoria.SelectedValue;

                    //SqlDataReader reader = ejecutaReader(txtSQL, 1);

                    //if (reader != null)
                    //{
                    //    if (reader.Read())
                    //    {
                    //        foreach (GridViewRow gvr in gv_proyectospriorizar.Rows)
                    //        {
                    //            string codValue = gv_proyectospriorizar.DataKeys[gvr.RowIndex].Value.ToString();

                    //            if (string.IsNullOrEmpty(codValue))
                    //            {
                    //                txtSQL = "INSERT INTO AsignacionActaProyecto (CodActa, CodProyecto, Asignado) VALUES(" + txt_numeroacta.Text + "," + gvr.RowIndex + ",0)";
                    //                ejecutaReader(txtSQL, 2);
                    //            }
                    //            else
                    //            {
                    //                txtSQL = "update proyecto set codestado=" + Constantes.CONST_LegalizacionContrato + " where id_proyecto=" + codValue;
                    //                ejecutaReader(txtSQL, 2);

                    //                txtSQL = "INSERT INTO AsignacionActaProyecto (CodActa, CodProyecto, Asignado) VALUES(" + txt_numeroacta.Text + "," + codValue + ",1)";
                    //                ejecutaReader(txtSQL, 2);
                    //            }
                    //        }
                    //    }
                    //}

                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Nueva Acta Creada: " + IdActa.ToString() + "!')", true); 
                    #endregion

                    #region Código anterior.
                    //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    //SqlCommand cmd = new SqlCommand("MD_InsertActa", con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@NomActa", txt_nombreacta.Text);
                    //cmd.Parameters.AddWithValue("@NumActa", txt_numeroacta.Text);
                    //cmd.Parameters.AddWithValue("@FechaActa", Convert.ToDateTime(txt_fecha.Text).ToString("yyyy-MM-dd"));
                    //cmd.Parameters.AddWithValue("@Observaciones", txt_observaciones.Text);
                    //cmd.Parameters.AddWithValue("@CodConvocatoria", Convert.ToInt32(ddl_convocatoria.SelectedValue));

                    //con.Open();
                    //SqlParameter retorno = new SqlParameter("@codigoacta", SqlDbType.Int);
                    //retorno.Direction = ParameterDirection.ReturnValue;
                    //cmd.Parameters.Add(retorno);
                    //SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                    //cmd2.ExecuteNonQuery();


                    //cmd.ExecuteNonQuery();
                    //int IdActa = (int)cmd.Parameters["@codigoacta"].Value;
                    //con.Close();
                    //con.Dispose();
                    //cmd.Dispose();
                    //cmd2.Dispose(); 
                    #endregion

                    #region Insertar datos.

                    try
                    {
                        //NEW RESULTS:
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                        SqlCommand cmd = new SqlCommand("MD_InsertActa", con);

                        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NomActa", txt_nombreacta.Text);
                        cmd.Parameters.AddWithValue("@NumActa", txt_numeroacta.Text);
                        cmd.Parameters.AddWithValue("@FechaActa", Convert.ToDateTime(txt_fecha.Text).ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Observaciones", txt_observaciones.Text);
                        cmd.Parameters.AddWithValue("@CodConvocatoria", Convert.ToInt32(ddl_convocatoria.SelectedValue));
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                        cmd.Dispose();
                    }
                    catch { }

                    #endregion

                    #region Consultar el máximo del acta.

                    txtSQL = " SELECT Max(Id_Acta) AS Id_Acta FROM AsignacionActa " +
                             " WHERE NomActa = '" + txt_nombreacta.Text + "'" +
                             " AND NumActa = '" + txt_numeroacta.Text + "'" +
                        //" AND FechaActa = '"+Convert.ToDateTime(txt_fecha.Text).ToString("yyyy-MM-dd")+"'" +
                             " AND Observaciones = '" + txt_observaciones.Text + "'" +
                             " AND CodConvocatoria = " + ddl_convocatoria.SelectedValue;

                    rs = consultas.ObtenerDataTable(txtSQL, "text");

                    if (rs.Rows.Count > 0) { IdActa = int.Parse(rs.Rows[0]["Id_Acta"].ToString()); }

                    #endregion

                    Session["Id_Acta"] = IdActa.ToString();
                    cambiarestaroProyectos(IdActa);
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Nueva Acta Creada: " + txt_numeroacta.Text + "!')", true);
                }
            }
            catch { }
        }

        /// <summary>
        /// Método que hace las inserciones y actualizaciones de los proyectos seleccionados para su legalización "o no".
        /// </summary>
        /// <param name="IdActa">Id del acta.</param>
        protected void cambiarestaroProyectos(int IdActa)
        {
            try
            {
                foreach (GridViewRow grd_Row in this.gv_proyectospriorizar.Rows)
                {
                    int asignar = 0;
                    string caso = "";
                    string codigoProyecto = ((Label)grd_Row.FindControl("codigo_proyecto")).Text;

                    if (((CheckBox)grd_Row.FindControl("ChBSelct")).Checked)
                    {
                        asignar = 1;
                        caso = "update";
                    }

                    #region Asignación de acta.

                    try
                    {
                        //NEW RESULTS:
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                        SqlCommand cmd = new SqlCommand("MD_AsignacionActaProyecto", con);

                        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@LegalizacionContrato", Constantes.CONST_LegalizacionContrato);
                        cmd.Parameters.AddWithValue("@NumAct", IdActa);
                        cmd.Parameters.AddWithValue("@CodProyecto", codigoProyecto);
                        cmd.Parameters.AddWithValue("@asignar", asignar);
                        cmd.Parameters.AddWithValue("@caso", caso);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                        cmd.Dispose();
                    }
                    catch { }

                    #endregion

                    #region Código anterior.
                    //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    //SqlCommand cmd = new SqlCommand("MD_AsignacionActaProyecto", con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@LegalizacionContrato", Constantes.CONST_LegalizacionContrato);
                    //cmd.Parameters.AddWithValue("@NumAct", IdActa);
                    //cmd.Parameters.AddWithValue("@CodProyecto", codigoProyecto);
                    //cmd.Parameters.AddWithValue("@asignar", asignar);
                    //cmd.Parameters.AddWithValue("@caso", caso);
                    //SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                    //con.Open();
                    //cmd2.ExecuteNonQuery();
                    //cmd.ExecuteNonQuery();
                    //con.Close();
                    //con.Dispose();
                    //cmd2.Dispose();
                    //cmd.Dispose(); 
                    #endregion
                }
                PanelIr_A.Visible = true;
                lbtn_iraacta.Text = "Se ha generado el acta número " + txt_numeroacta.Text + ", de Asignación de recursos " + txt_fecha.Text + ". Clic aquí para verla.";
                lbtn_iraacta.Focus();
            }
            catch (Exception)
            { }

        }

        protected void lbtn_iraacta_Click(object sender, EventArgs e)
        {
            Response.Redirect("VerActaAsignacionRecursos.aspx");
        }
    }

    class labelasigado : ITemplate
    {
        private int Contador = 0;
        public labelasigado(int contador)
        {
            this.Contador = contador;
        }
        public void InstantiateIn(System.Web.UI.Control container)
        {
            //TextBox labelAAsignar = new TextBox();
            //labelAAsignar.ID = "l_asignar";
            ////labelAAsignar.ID = "l_asignar" + this.Contador.ToString();
            ////labelAAsignar.Text = "texto1";

            //container.Controls.Add(labelAAsignar);
        }
    }
}