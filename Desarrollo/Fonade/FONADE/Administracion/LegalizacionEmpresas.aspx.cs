using Datos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Administracion
{
    public partial class LegalizacionEmpresas : Negocio.Base_Page
    {
        string txtSQL;
        //DataTable datatable;
        string[] arr_meses = { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
        DateTime fecha_hoy = DateTime.Today;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                llenarGrilla();
                GenerarFecha_Year();
                CargarDropDown_Convocatorias();
                DropDown_Fecha_Actual();
            }

            txtnummemorando.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
        }

        /// <summary>
        /// Cargar el GridView.
        /// </summary>
        private void llenarGrilla()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("ID_Proyecto");
            datatable.Columns.Add("NomProyecto");

            txtSQL = " SELECT Id_Proyecto, NomProyecto, Inactivo, CodCiudad FROM Proyecto WHERE Inactivo = 0 AND CodEstado = " + Constantes.CONST_LegalizacionContrato;

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            foreach (DataRow dr in dt.Rows)
            {
                DataRow fila = datatable.NewRow();

                fila["ID_Proyecto"] = dr["ID_Proyecto"].ToString();

                #region asignar nombre empresa
                txtSQL = " SELECT RazonSocial FROM Empresa WHERE CodProyecto = " + dr["Id_Proyecto"].ToString();

                var dtas = consultas.ObtenerDataTable(txtSQL, "text");

                if (dtas.Rows.Count > 0)
                {
                    fila["NomProyecto"] = dtas.Rows[0]["RazonSocial"].ToString();
                }
                else
                {
                    string fechaAc = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

                    txtSQL = " INSERT INTO Empresa (RazonSocial, CodProyecto, CodCiudad, REFechaNorma, CFechaNorma, ARFechaNorma, DFechaNorma, ERFFechaNorma, GCFechaNorma) " +
                             " VALUES('" + dr["NomProyecto"].ToString() + "'," + dr["Id_Proyecto"].ToString() + "," + dr["CodCiudad"].ToString() + ",'" + fechaAc + "','" + fechaAc + "','" + fechaAc + "','" + fechaAc + "','" + fechaAc + "','" + fechaAc + "')";

                    ejecutaReader(txtSQL, 2);

                    fila["NomProyecto"] = dr["NomProyecto"].ToString();
                }
                #endregion

                datatable.Rows.Add(fila);
            }

            if (datatable.Rows.Count > 0)
            {
                gvplanesnegocio.DataSource = datatable;
                gvplanesnegocio.DataBind();
            }
        }

        protected void gvplanesnegocio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("proyectoframeset"))
            {
                Session["codProyecto"] = e.CommandArgument.ToString();

                Response.Redirect("~/FONADE/Proyecto/ProyectoFrameSet.aspx");
            }
        }

        /// <summary>
        /// Actualizar legalización de empresas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnactualizar_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            string validado = "";
            validado = ValidarCampos();

            //Si pasa la validación, continúa con el flujo.
            if (validado == "")
            {
                #region Continuar con el flujo del sistema, generar / actualizar detalles del memorando.

                //string fecha = cldmemorando.SelectedDate.Year + "-" + cldmemorando.SelectedDate.Month + "-" + cldmemorando.SelectedDate.Day + " " + cldmemorando.SelectedDate.Hour + ":" + cldmemorando.SelectedDate.Minute + ":" + cldmemorando.SelectedDate.Second;

                //Obtener la fecha.
                DateTime fecha_now = DateTime.Now;
                string fecha = dd_fecha_year_Memorando.SelectedValue + "-" + dd_fecha_mes_Memorando.SelectedValue + "-" + dd_fecha_dias_Memorando.SelectedValue + " " + fecha_now.Hour + ":" + fecha_now.Minute + ":" + fecha_now.Second;

                #region Inserción.

                txtSQL = "INSERT INTO LegalizacionActa (NomACta, NumActa, FechaActa, Observaciones, Publicado, CodConvocatoria) " +
                                 " VALUES('" + txtnommemorando.Text + "','" + txtnummemorando.Text + "','" + fecha + "','" + txtobservaciones.Text + "',1," + ddlconvocatoria.SelectedValue + ")";

                ejecutaReader(txtSQL, 2);

                #endregion

                txtSQL = "SELECT Max(Id_Acta) AS Id_Acta FROM LegalizacionActa " + " WHERE NomActa = '" + txtnommemorando.Text + "' " +
                         " AND NumActa = '" + txtnummemorando.Text + "' " + " AND FechaActa = '" + fecha + "'" + " AND Observaciones = '" + txtobservaciones.Text + "'";

                SqlDataReader reader = ejecutaReader(txtSQL, 1);

                Int32 IdActa = 0;

                if (reader.Read())
                {
                    try
                    {
                        IdActa = Convert.ToInt32(reader["Id_Acta"].ToString());

                        foreach (GridViewRow gvfila in gvplanesnegocio.Rows)
                        {
                            if (((CheckBox)gvfila.FindControl("rbtnlegalizadosi")).Checked || ((CheckBox)gvfila.FindControl("rbtnlegalizadono")).Checked)
                            {
                                Int32 id_proyecto = Convert.ToInt32(gvplanesnegocio.DataKeys[gvfila.RowIndex].Value.ToString());

                                int cbxgarantia = Convert.ToInt32(((CheckBox)gvfila.FindControl("cbxgarantia")).Checked);
                                int cbxpagare = Convert.ToInt32(((CheckBox)gvfila.FindControl("cbxpagare")).Checked);
                                int cbxcontrato = Convert.ToInt32(((CheckBox)gvfila.FindControl("cbxcontrato")).Checked);
                                int cbxplanoperativo = Convert.ToInt32(((CheckBox)gvfila.FindControl("cbxplanoperativo")).Checked);
                                int lealizado = Convert.ToInt32(((RadioButton)gvfila.FindControl("rbtnlegalizadosi")).Checked);

                                txtSQL = " INSERT INTO LegalizacionActaProyecto (CodActa, CodProyecto, garantia, Pagare, Contrato, PlanOperativo, Legalizado) " +
                                         " VALUES(" + IdActa + "," + id_proyecto + "," + cbxgarantia + "," + cbxpagare + "," + cbxcontrato + "," + cbxplanoperativo + "," + lealizado + ")";

                                ejecutaReader(txtSQL, 2);

                                if (((CheckBox)gvfila.FindControl("rbtnlegalizadosi")).Checked)
                                {
                                    txtSQL = " SELECT Id_Empresa FROM Empresa WHERE CodProyecto = " + id_proyecto;
                                    reader = ejecutaReader(txtSQL, 1);
                                    if (reader.Read())
                                    {
                                        txtSQL = " insert into indicadorgenerico (CodEmpresa, Nombreindicador, descripcion, numerador, denominador, evaluacion, observacion) " +
                                                 " select " + reader["Id_Empresa"].ToString() + ", Nombreindicador, descripcion, numerador, denominador, evaluacion, observacion from IndicadoresGenericosModelo ";

                                        ejecutaReader(txtSQL, 2);

                                        txtSQL = " UPDATE Proyecto SET CodEstado = " + Constantes.CONST_Ejecucion + " WHERE Id_proyecto = " + id_proyecto;

                                        ejecutaReader(txtSQL, 2);
                                    }
                                }

                                if (((CheckBox)gvfila.FindControl("rbtnlegalizadono")).Checked)
                                {
                                    txtSQL = "pr_LegalizacionPaso7 " + id_proyecto + "," + Constantes.CONST_Inscripcion + "," + Constantes.CONST_RolEmprendedor;

                                    ejecutaReader(txtSQL, 2);
                                }
                            }
                        }
                    }
                    catch (FormatException fm)
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + fm.Message + "')", true);
                    }
                    catch (Exception) { }

                    finally
                    {
                        crearGriDVIew(IdActa);
                    }

                    #region Vaciar campos y establecer fecha actual.
                    txtnommemorando.Text = "";
                    txtnummemorando.Text = "";
                    DropDown_Fecha_Actual();
                    ddlconvocatoria.Items[0].Selected = true;
                    txtobservaciones.Text = "";
                    #endregion
                }
                #endregion
            }
            else
            {
                //Muestra mensaje de error y/o falta completar el campo.
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + validado + "')", true);
                return;
            }
        }

        private void crearGriDVIew(int IdActa)
        {
            DataTable data = new DataTable();

            data.Columns.Add("campo01");
            data.Columns.Add("campo02");
            data.Columns.Add("campo03");
            data.Columns.Add("campo04");
            data.Columns.Add("campo05");
            data.Columns.Add("campo06");
            data.Columns.Add("campo07");
            data.Columns.Add("campo08");
            data.Columns.Add("campo09");
            data.Columns.Add("campo10");

            txtSQL = "SELECT p.Id_Proyecto FROM dbo.Proyecto p INNER JOIN dbo.Ciudad ON p.CodCiudad = dbo.Ciudad.Id_Ciudad " +
            "INNER JOIN dbo.departamento ON dbo.Ciudad.CodDepartamento = dbo.departamento.Id_Departamento " +
            "INNER JOIN LegalizacionActaProyecto ON p.Id_Proyecto = LegalizacionActaProyecto.CodProyecto " +
            "WHERE (p.Inactivo = 0)  AND (p.CodEstado >= " + Constantes.CONST_Ejecucion + ") AND (LegalizacionActaProyecto.CodActa = " + IdActa + ")";

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i += 1;
                txtSQL = "select p.id_proyecto, p.nomproyecto, e.razonsocial, e.DomicilioEmpresa, e.CodCiudad, e.Telefono, e.Email, c.identificacion" +
                                    " from proyecto p LEFT JOIN (empresa e INNER JOIN (empresacontacto ec INNER JOIN contacto c on ec.codcontacto=c.id_contacto ) ON e.id_empresa = ec.Codempresa) ON" +
                                    " p.id_proyecto=e.codproyecto Where p.id_proyecto = " + dr["ID_Proyecto"].ToString();

                var dtas = consultas.ObtenerDataTable(txtSQL, "text");

                if (dtas.Rows.Count > 0)
                {
                    DataRow fila = data.NewRow();

                    fila["campo01"] = "e";
                    fila["campo02"] = "" + i;
                    fila["campo03"] = dr["ID_Proyecto"].ToString();
                    fila["campo04"] = dtas.Rows[0].ItemArray[1].ToString();
                    fila["campo05"] = dtas.Rows[0].ItemArray[2].ToString();
                    fila["campo06"] = dtas.Rows[0].ItemArray[3].ToString();
                    fila["campo07"] = dtas.Rows[0].ItemArray[4].ToString();
                    fila["campo08"] = dtas.Rows[0].ItemArray[5].ToString();
                    fila["campo09"] = dtas.Rows[0].ItemArray[6].ToString();
                    fila["campo10"] = dtas.Rows[0].ItemArray[7].ToString();

                    data.Rows.Add(fila);
                }

                txtSQL = "select c.id_contacto, c.nombres, c.apellidos, c.identificacion, c.direccion, c.email, pc.participacion" +
                                    " from contacto c, proyectocontacto pc where c.id_contacto = pc.codcontacto and pc.codrol = 3 and pc.inactivo = 0 and pc.codproyecto = " + dr["ID_Proyecto"].ToString();

                dtas = consultas.ObtenerDataTable(txtSQL, "text");

                if (dtas.Rows.Count > 0)
                {
                    DataRow fila = data.NewRow();

                    fila["campo01"] = "s";
                    fila["campo02"] = "" + i;
                    fila["campo03"] = dtas.Rows[0].ItemArray[1].ToString();
                    fila["campo04"] = dtas.Rows[0].ItemArray[2].ToString();
                    fila["campo05"] = dtas.Rows[0].ItemArray[3].ToString();
                    fila["campo06"] = dtas.Rows[0].ItemArray[4].ToString();
                    fila["campo07"] = dtas.Rows[0].ItemArray[5].ToString();
                    fila["campo08"] = dtas.Rows[0].ItemArray[6].ToString();
                    fila["campo09"] = "";
                    fila["campo10"] = "";

                    data.Rows.Add(fila);
                }
            }

            ExportToExcel(data, IdActa);
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Archivo Confecamaras creado satisfactoriamente.')", true);
            llenarGrilla();
            CargarDropDown_Convocatorias();
            DropDown_Fecha_Actual();
        }

        public void ExportToExcel(DataTable dt, int IdActa)
        {
            if (dt.Rows.Count > 0)
            {
                string filename = Server.MapPath("/FonadeDocumentos/Confecamaras/") + "CargueConfecamaras" + IdActa + ".csv";
                const string FIELDSEPARATOR = "\t";
                const string ROWSEPARATOR = "\n";
                StringBuilder output = new StringBuilder();
                output.Append(ROWSEPARATOR);
                foreach (DataRow item in dt.Rows)
                {
                    foreach (object value in item.ItemArray)
                    {
                        output.Append(value.ToString().Replace('\n', ' ').Replace('\r', ' ').Replace('.', ','));
                        output.Append(FIELDSEPARATOR);
                    }
                    // Escribir una línea de registro        
                    output.Append(ROWSEPARATOR);
                }
                // Valor de retorno    
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(output.ToString());
                sw.Close();
            }
        }


        private void CargarDropDown_Convocatorias()
        {
            //Inicializar variables.
            String txtSQL;
            DataTable tabla = new DataTable();

            try
            {
                txtSQL = " SELECT Id_Convocatoria, NomConvocatoria FROM Convocatoria ";

                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                ddlconvocatoria.Items.Clear();

                ListItem item_default = new ListItem();
                item_default.Value = "";
                item_default.Text = "Seleccione";
                ddlconvocatoria.Items.Add(item_default);
                item_default = null;

                foreach (DataRow row in tabla.Rows)
                {
                    ListItem item = new ListItem();
                    item.Value = row["Id_Convocatoria"].ToString();
                    item.Text = row["NomConvocatoria"].ToString();
                    ddlconvocatoria.Items.Add(item);
                    item = null;
                }

                txtSQL = null;
                tabla = null;
            }
            catch { }
        }

        private void GenerarFecha_Year()
        {
            try
            {
                int currentYear = DateTime.Today.AddYears(-11).Year;
                int futureYear = DateTime.Today.AddYears(5).Year;

                for (int i = currentYear; i < futureYear; i++)
                {
                    ListItem item = new ListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    dd_fecha_year_Memorando.Items.Add(item);
                }
            }
            catch { }
        }

        private void DropDown_Fecha_Actual()
        {
            dd_fecha_dias_Memorando.SelectedValue = fecha_hoy.Day.ToString();
            dd_fecha_mes_Memorando.SelectedValue = fecha_hoy.Month.ToString();
            dd_fecha_year_Memorando.SelectedValue = fecha_hoy.Year.ToString();
        }

        private string ValidarCampos()
        {
            //Inicializar variables.
            string resultado = "";

            if (txtnummemorando.Text.Trim() == "")
            { resultado = "Debe Ingresar un valor de Numero Memorando"; return resultado; }
            if (txtnommemorando.Text.Trim() == "")
            { resultado = "Debe Ingresar un valor de Nombre Memorando"; return resultado; }
            if (ddlconvocatoria.SelectedValue == "")
            { resultado = "Debe Selecionar una convocatoria"; return resultado; }
            if (txtobservaciones.Text.Trim() == "")
            { resultado = "Debe Ingresar comentarios"; return resultado; }

            return resultado;
        }
    }
}