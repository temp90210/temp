using Datos;
using Fonade.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class CatalogoRiesgoInter : Negocio.Base_Page
    {
        #region Variables globales.

        public string txtTab = Constantes.CONST_RiesgosInter.ToString();
        string CodProyecto;
        string CodEmpresa;
        string CodConvocatoria;
        string anioConvocatoria;
        string CodRiesgo;
        string txtSQL;
        string NomProyecto;
        string Tarea;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CodRiesgo = Session["CodRiesgo"] != null && !string.IsNullOrEmpty(Session["CodRiesgo"].ToString()) ? Session["CodRiesgo"].ToString() : "0";
            CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            Tarea = Session["Tarea"] != null && !string.IsNullOrEmpty(Session["Tarea"].ToString()) ? Session["Tarea"].ToString() : "0";

            if (!IsPostBack)
            {
                //Cargar el listado de ejes funcionales.
                ejefuncional();

                //Mostrar el check de aprobar cuando se seleccione un riesgo y el usuario sea uno quien pueda aprobarlo o no.
                if (CodRiesgo != "0" && CodProyecto != "0" && (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor))
                { B_Crear.Text = "Actualizar"; CargarRiesgo(); tr_aprobar.Visible = true; }

                if (CodProyecto != "0")
                {
                    var rt = consultas.ObtenerDataTable("SELECT NomProyecto FROM Proyecto WHERE Id_Proyecto = " + CodProyecto);
                    if (rt.Rows.Count > 0) { NomProyecto = rt.Rows[0]["NomProyecto"].ToString(); }
                    else { NomProyecto = ""; } rt = null;
                }
            }
        }

        private void datosEntrada()
        {
            CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            CodEmpresa = Session["CodEmpresa"] != null && !string.IsNullOrEmpty(Session["CodEmpresa"].ToString()) ? Session["CodEmpresa"].ToString() : "0";

            txtSQL = "SELECT Max(CodConvocatoria) AS CodConvocatoria FROM ConvocatoriaProyecto WHERE CodProyecto = " + CodProyecto;

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            if (dt.Rows.Count > 0)
                CodConvocatoria = dt.Rows[0]["CodConvocatoria"].ToString();

            if (!string.IsNullOrEmpty(CodConvocatoria))
            {
                txtSQL = "select year(fechainicio) from convocatoria where id_Convocatoria=" + CodConvocatoria;

                dt = consultas.ObtenerDataTable(txtSQL, "text");

                if (dt.Rows.Count > 0)
                    anioConvocatoria = dt.Rows[0][0].ToString();
            }
        }

        /// <summary>
        /// Cargar Eje Funcional "DropDownList".
        /// </summary>
        public void ejefuncional()
        {
            #region Comentarios.
            //return consultas.ObtenerDataTable("SELECT * FROM EjeFuncional ORDER BY NomEjeFuncional", "text"); 
            #endregion

            var rt = consultas.ObtenerDataTable("SELECT * FROM EjeFuncional ORDER BY NomEjeFuncional", "text");
            ddlejefuncional.Items.Clear();

            foreach (DataRow row in rt.Rows)
            {
                ListItem item = new ListItem();
                item.Text = row["NomEjeFuncional"].ToString();
                item.Value = row["Id_EjeFuncional"].ToString();
                ddlejefuncional.Items.Add(item);
            }
        }

        protected void B_Crear_Click(object sender, EventArgs e)
        {
            Acciones(B_Crear.Text);
        }

        /// <summary>
        /// De acuerdo al valor del parámetro, ejecutará la acción "Crear, Actualizar, etc".
        /// </summary>
        /// <param name="accion"></param>
        private void Acciones(String accion)
        {
            //Inicializar variables.
            DataTable Rs = new DataTable();
            DataTable RsActividad = new DataTable();

            #region Verifica si los campos de textp contienen el caracter "'".

            if (TB_Riesgo.Text.Trim() != "")
            {
                if (TB_Riesgo.Text.Contains("'"))
                { TB_Riesgo.Text = TB_Riesgo.Text.Replace("'", ""); }
            }

            if (TB_Mitigacion.Text.Trim() != "")
            {
                if (TB_Mitigacion.Text.Contains("'"))
                { TB_Mitigacion.Text = TB_Mitigacion.Text.Replace("'", ""); }
            }

            if (txtobservacion.Text.Trim() != "")
            {
                if (txtobservacion.Text.Contains("'"))
                { txtobservacion.Text = txtobservacion.Text.Replace("'", ""); }
            }

            #endregion

            switch (accion)
            {
                case "Crear":
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        #region Sólo puede crear el INTERVENTOR.

                        datosEntrada();
                        string riesgo = TB_Riesgo.Text;
                        string mitigacion = TB_Mitigacion.Text;
                        string ejefuncional = ddlejefuncional.SelectedValue;
                        string observacion = txtobservacion.Text;
                        String NomProyecto = "";

                        //Nombre del proyecto.
                        var nmb = consultas.ObtenerDataTable("SELECT NomProyecto FROM Proyecto WHERE Id_Proyecto = " + CodProyecto, "text");
                        if (nmb.Rows.Count > 0) { NomProyecto = nmb.Rows[0]["NomProyecto"].ToString(); }
                        nmb = null;

                        if (usuario.CodGrupo == Constantes.CONST_Interventor)
                        {
                            txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;

                            SqlDataReader rd = ejecutaReader(txtSQL, 1);

                            if (rd != null)
                            {
                                if (rd.Read())
                                {
                                    int codInter = Convert.ToInt32(rd["CodCoordinador"].ToString());
                                    txtSQL = "select max(Id_Riesgo)+1 as Id_Riesgo from InterventorRiesgoTMP";
                                    string ActividadTmp;
                                    rd = ejecutaReader(txtSQL, 1);
                                    if (rd != null)
                                    {
                                        if (rd.Read())
                                        { ActividadTmp = rd["Id_Riesgo"].ToString(); }
                                        else
                                        { ActividadTmp = "1"; }

                                        txtSQL = @"Insert into InterventorRiesgoTMP (Id_riesgo,CodProyecto,Riesgo,Mitigacion,CodejeFuncional,Observacion) " +
                                            "values (" + ActividadTmp + "," + CodProyecto + ",'" + riesgo + "','" + mitigacion + "', " + ejefuncional + ", '" + observacion + "')";

                                        //Agendar la tarea...
                                        AgendarTarea agenda =
                                            new AgendarTarea(
                                                        codInter,
                                                        "Revisión Riesgos al Plan Operativo",
                                                        "Revisión Adición, Modificación o Borrado de Riesgos del interventor al Plan Operativo " + NomProyecto,
                                                        CodProyecto,
                                                        22,
                                                        "0",
                                                        false,
                                                        1,
                                                        true,
                                                        false,
                                                        usuario.IdContacto,
                                                        "Accion=Editar&CodProyecto=" + CodProyecto + "&CodRiesgo=" + ActividadTmp,
                                                        "",
                                                        "");
                                        agenda.Agendar();

                                        //NOTA: Se usa para "ingresar" datos en tablas "que por x o y motivos NO tienen llave primaria".
                                        ejecutaReader(txtSQL, 2);

                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                                        return;
                                    }
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                                    return;
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.');window.opener.location.reload();window.close();", true);
                                return;
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                            return;
                        }

                        #endregion
                    }
                    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor && Session["Tarea"].ToString() == "Adicionar")
                    {
                        //Si está aprobado...
                        if (Aprobar.Checked)
                        {
                            #region Gerente Interventor "Adicionar"...

                            //TRAE LOS REGISTROS DE LA TABLA TEMPORAL
                            txtSQL = " select * from InterventorRiesgoTMP where CodProyecto=" + CodProyecto + " and Id_riesgo=" + CodRiesgo;
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            //INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA
                            txtSQL = " Insert into InterventorRiesgo (CodProyecto,Riesgo,Mitigacion,CodejeFuncional,Observacion) " +
                                     " values (" + CodProyecto + ",'" + TB_Riesgo.Text.Trim() + "','" + TB_Mitigacion.Text.Trim() + "', "
                                     + RsActividad.Rows[0]["CodEjeFuncional"].ToString() + ", '" + txtobservacion.Text.Trim() + "')";

                            //Ejecutar consulta.
                            ejecutaReader(txtSQL, 2);

                            //Actualizar fecha modificación del tab
                            prActualizarTabInter(txtTab, CodProyecto);
                            //bRepetido = false

                            //BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL
                            txtSQL = "DELETE FROM InterventorRiesgoTMP where CodProyecto=" + CodProyecto + " and Id_riesgo=" + CodRiesgo;

                            //Ejecutar consulta.
                            ejecutaReader(txtSQL, 2);

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                            return;

                            #endregion
                        }
                    }
                    break;
                case "Actualizar":
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        #region Interventor.

                        //Verifica si el interventor tiene un coordinador asignado
                        txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;
                        Rs = consultas.ObtenerDataTable(txtSQL, "text");

                        if (Rs.Rows.Count > 0)
                        {
                            //Asigna la tarea al coordinador
                            txtSQL = " Insert into InterventorRiesgoTMP (Id_riesgo,CodProyecto,Riesgo,Mitigacion,CodejeFuncional,Observacion,Tarea) " +
                                     " values (" + CodRiesgo + "," + CodProyecto + ",'" + TB_Riesgo.Text.Trim() + "','" + TB_Mitigacion.Text.Trim() + "', " + ddlejefuncional.SelectedValue +
                                     ", '" + txtobservacion.Text.Trim() + "','Modificar')";

                            //Ejecutar consulta.
                            ejecutaReader(txtSQL, 2);

                            //Agendar la tarea...
                            AgendarTarea agenda =
                                new AgendarTarea(
                                            Int32.Parse(Rs.Rows[0]["CodCoordinador"].ToString()),
                                            "Revisión Riesgos al Plan Operativo",
                                            "Revisión Adición, Modificación o Borrado de Riesgos del interventor al Plan Operativo " + NomProyecto,
                                            CodProyecto,
                                            22,
                                            "0",
                                            false,
                                            1,
                                            true,
                                            false,
                                            usuario.IdContacto,
                                            "Accion=Editar&CodProyecto=" + CodProyecto + "&CodRiesgo=" + CodRiesgo,
                                            "",
                                            "");
                            agenda.Agendar();

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                            return;
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.');window.opener.location.reload();window.close();", true);
                            return;
                        }

                        #endregion
                    }
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                    {
                        #region Coordinador Interventor...

                        //Si está aprobado...
                        if (Aprobar.Checked)
                        {
                            #region Coordinador Interventor.

                            txtSQL = " UPDATE InterventorRiesgoTMP SET ChequeoCoordinador=1" +
                                     " where CodProyecto=" + CodProyecto + " and Id_riesgo=" + CodRiesgo;

                            //Ejecutar consulta.
                            ejecutaReader(txtSQL, 2);

                            txtSQL = " select Id_grupo from Grupo " +
                                     " where NomGrupo = 'Gerente Interventor' ";
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            txtSQL = " select CodContacto from GrupoContacto " +
                                     " where CodGrupo =" + RsActividad.Rows[0]["Id_grupo"].ToString();
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            //Agendar la tarea...
                            AgendarTarea agenda =
                                new AgendarTarea(
                                            Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                                            "Revisión Riesgos al Plan Operativo",
                                            "Revisión Adición, Modificación o Borrado de Riesgos del interventor al Plan Operativo " + NomProyecto,
                                            CodProyecto,
                                            23,
                                            "0",
                                            false,
                                            1,
                                            true,
                                            false,
                                            usuario.IdContacto,
                                            "Accion=Editar&CodProyecto=" + CodProyecto + "&CodRiesgo=" + CodRiesgo,
                                            "",
                                            "");
                            agenda.Agendar();

                            #endregion
                        }

                        //Cierra al final la ventana "Emergente".
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        return;

                        #endregion
                    }
                    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor && Session["Tarea"].ToString() == "Adicionar")
                    {
                        #region Gerente Interventor "Modificar"...

                        //Si está aprobado...
                        if (Aprobar.Checked)
                        {
                            #region Gerente Interventor.

                            //TRAE LOS REGISTROS DE LA TABLA TEMPORAL
                            txtSQL = "select * from InterventorRiesgoTMP where CodProyecto=" + CodProyecto + " and Id_riesgo=" + CodRiesgo;
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            //ACTUALIZA LOS REGISTROS EN LA TABLA DEFINITIVA
                            txtSQL = " UPDATE InterventorRiesgo " +
                                     " SET Riesgo = '" + TB_Riesgo.Text.Trim() + "', " +
                                     " Mitigacion = '" + TB_Mitigacion.Text.Trim() + "', " +
                                     " CodEjeFuncional = '" + ddlejefuncional.SelectedValue + "', " +
                                     " Observacion = '" + txtobservacion.Text.Trim() + "' " +
                                     " WHERE Id_Riesgo=" + CodRiesgo;

                            //Ejecutar consulta.
                            ejecutaReader(txtSQL, 2);

                            //Actualizar fecha modificación del tab
                            prActualizarTabInter(txtTab, CodProyecto);
                            //bRepetido = false				

                            //BORRAR EL REGISTRO YA ACTUALIZADO DE LA TABLA TEMPORAL
                            txtSQL = "DELETE FROM InterventorRiesgoTMP where CodProyecto=" + CodProyecto + " and Id_riesgo=" + CodRiesgo;

                            //Ejecutar consulta.
                            ejecutaReader(txtSQL, 2);

                            #endregion
                        }

                        //Cierra al final la ventana "Emergente".
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        return;

                        #endregion
                    }
                    break;
                case "Borrar":
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        #region Sólo puede eliminar riesgos si es INTERVENTOR.

                        //Verifica si el interventor tiene un coordinador asignado
                        txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;
                        Rs = consultas.ObtenerDataTable(txtSQL, "text");

                        if (Rs.Rows.Count > 0)
                        {
                            //Asigna la tarea al coordinador
                            txtSQL = "SELECT * FROM InterventorRiesgo WHERE Id_riesgo=" + CodRiesgo;
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            txtSQL = " Insert into InterventorRiesgoTMP (Id_riesgo,CodProyecto,Riesgo,Mitigacion,CodejeFuncional,Observacion,Tarea) " +
                                     " values (" + CodRiesgo + "," + CodProyecto + ",'" + RsActividad.Rows[0]["Riesgo"].ToString() + "','" + RsActividad.Rows[0]["Mitigacion"].ToString() + "', "
                                     + RsActividad.Rows[0]["CodEjeFuncional"].ToString() + ", '" + RsActividad.Rows[0]["Observacion"].ToString() + "','Borrar')";

                            //Ejecutar consulta.
                            ejecutaReader(txtSQL, 2);

                            //Agendar la tarea...
                            AgendarTarea agenda =
                                new AgendarTarea(
                                            Int32.Parse(Rs.Rows[0]["CodCoordinador"].ToString()),
                                            "Revisión Riesgos al Plan Operativo",
                                            "Revisión Adición, Modificación o Borrado de Riesgos del interventor al Plan Operativo " + NomProyecto,
                                            CodProyecto,
                                            22,
                                            "0",
                                            false,
                                            1,
                                            true,
                                            false,
                                            usuario.IdContacto,
                                            "Accion=Editar&CodProyecto=" + CodProyecto + "&CodRiesgo=" + CodRiesgo,
                                            "",
                                            "");
                            agenda.Agendar();

                            //Al terminar, se cierra la ventana "emergente".
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                            return;
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.');window.opener.location.reload();window.close();", true);
                            return;
                        }

                        #endregion
                    }
                    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor && Session["Tarea"].ToString() == "Adicionar")
                    {
                        #region Gerente Interventor "Borrar"...

                        //BORRA LOS REGISTROS EN LA TABLA DEFINITIVA
                        txtSQL = "DELETE FROM InterventorRiesgo WHERE Id_riesgo=" + CodRiesgo;

                        //Ejecutar consulta.
                        ejecutaReader(txtSQL, 2);

                        //Actualizar fecha modificación del tab
                        prActualizarTabInter(txtTab, CodProyecto);

                        //BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL
                        txtSQL = "DELETE FROM InterventorRiesgoTMP where CodProyecto=" + CodProyecto + " and Id_riesgo=" + CodRiesgo;

                        //Ejecutar consulta.
                        ejecutaReader(txtSQL, 2);

                        //Cierra al final la ventana "Emergente".
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        return;

                        #endregion
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Cerrar ventana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true);
            return;
        }

        /// <summary>
        /// Cargar la información del riesgo seleccionado "en este caso, desde la tarea
        /// agendada".
        /// </summary>
        private void CargarRiesgo()
        {
            //Inicializar variables.
            DataTable rs = new DataTable();

            try
            {
                if (!Aprobar.Checked)
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Si es Coordinador o Gerente interventor.

                        rs = consultas.ObtenerDataTable("SELECT * FROM InterventorRiesgoTMP WHERE Id_Riesgo = " + CodRiesgo, "text");

                        if (rs.Rows.Count > 0)
                        {
                            B_Crear.Text = "Actualizar";
                            TB_Riesgo.Text = rs.Rows[0]["Riesgo"].ToString();
                            TB_Riesgo.Enabled = false;
                            TB_Mitigacion.Text = rs.Rows[0]["Mitigacion"].ToString();
                            TB_Mitigacion.Enabled = false;

                            if (ddlejefuncional.Items.Count > 0)
                            { ddlejefuncional.SelectedValue = rs.Rows[0]["CodEjeFuncional"].ToString(); }
                            ddlejefuncional.Enabled = false;

                            txtobservacion.Text = rs.Rows[0]["Observacion"].ToString();
                            txtobservacion.Enabled = false;

                            Session["Tarea"] = rs.Rows[0]["Tarea"].ToString();

                            try
                            {
                                if (!String.IsNullOrEmpty(rs.Rows[0]["ChequeoCoordinador"].ToString()))
                                { Aprobar.Checked = Boolean.Parse(rs.Rows[0]["ChequeoCoordinador"].ToString()); }

                                if (!String.IsNullOrEmpty(rs.Rows[0]["ChequeoGerente"].ToString()))
                                { Aprobar.Checked = Boolean.Parse(rs.Rows[0]["ChequeoGerente"].ToString()); }
                            }
                            catch { }
                        }
                        else
                        { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Tarea ya aprobada.');window.close();", true); }

                        #endregion
                    }
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        #region Si es Interventor.

                        rs = consultas.ObtenerDataTable("SELECT Riesgo, Mitigacion, CodEjeFuncional, Observacion FROM InterventorRiesgo WHERE Id_Riesgo = " + CodRiesgo, "text");

                        if (rs.Rows.Count > 0)
                        {
                            B_Crear.Text = "Actualizar";
                            TB_Riesgo.Text = rs.Rows[0]["Riesgo"].ToString();
                            TB_Mitigacion.Text = rs.Rows[0]["Mitigacion"].ToString();

                            if (ddlejefuncional.Items.Count > 0)
                            { ddlejefuncional.SelectedValue = rs.Rows[0]["CodEjeFuncional"].ToString(); }

                            txtobservacion.Text = rs.Rows[0]["Observacion"].ToString();
                        }

                        #endregion
                    }
                }
            }
            catch { B_Crear.Text = "Crear"; }
        }
    }
}