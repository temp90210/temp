using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data.Linq.SqlClient;
using System.Linq.Expressions;
using LinqKit;
using AjaxControlToolkit;
using System.ComponentModel;
using System.Data;

namespace Fonade.FONADE.Proyecto
{
    public partial class ProyectoFrameSet : Negocio.Base_Page
    {
        public string codProyecto;
        public string codConvocatoria;
        /// <summary>
        /// NEW! según "SeguimientoEmpresas.aspx, este valor también viaja por variable de sesión.
        /// </summary>
        public string codEmpresa;
        public int codigoEstado;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Comentarios.
            //Session["codProyecto"] = string.Empty;
            //Session["codConvocatoria"] = string.Empty;

            //if (Request.QueryString["codProyecto"] != null && Request.QueryString["codProyecto"] != "")
            //{
            //    codProyecto = Request.QueryString["codProyecto"].ToString();
            //    Session["codProyecto"] = Request.QueryString["codProyecto"].ToString();
            //}
            //else
            //{
            //    Response.Redirect("~/Default.aspx");
            //}

            //if (Request.QueryString["codConvocatoria"] != null && Request.QueryString["codConvocatoria"] != "")
            //{
            //    codConvocatoria = Request.QueryString["codConvocatoria"].ToString();
            //    Session["codConvocatoria"] = Request.QueryString["codConvocatoria"].ToString();
            //}

            //setTabsStatus(); 
            #endregion

            #region Nueva versión.

            if (!IsPostBack)
            {
                codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
                codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "0";
                //codEmpresa = Session["CodEmpresa"] != null && !string.IsNullOrEmpty(Session["CodEmpresa"].ToString()) ? Session["CodEmpresa"].ToString() : "0";

                if (codProyecto == "0")// && codEmpresa == "0")
                { codProyecto = Request["CodProyecto"].ToString(); }
            }

            setTabsStatus();

            #endregion
        }

        private void setTabsStatus()
        {
            var codEstado = (from p in consultas.Db.Proyectos
                             where p.Id_Proyecto == Convert.ToInt32(codProyecto)
                             select p).FirstOrDefault();

            if (codEstado != null)
            { codigoEstado = codEstado.CodEstado; }

            #region Comentarios hechos antes de las modificaciones del 14/05/2014.
            //setTab(Constantes.CONST_Mercado, tc_mercado);
            //setTab(Constantes.CONST_Operacion, tc_operacion);
            //setTab(Constantes.CONST_Organizacion, tc_organizacion);
            //setTab(Constantes.CONST_Finanzas, tc_finanzas);
            //setTab(Constantes.CONST_PlanOperativo, tc_planOperativo);
            //setTab(Constantes.CONST_Impacto, tc_impacto);
            //setTab(Constantes.CONST_ResumenEjecutivo, tc_resumen);
            //setTab(Constantes.CONST_Anexos, tc_anexos); 
            #endregion

            if (codigoEstado >= Constantes.CONST_LegalizacionContrato && codigoEstado <= Constantes.CONST_Condonacion)
            {
                tc_empresa.Visible = true;
                tc_seguimiento.Visible = true;
                tc_contrato.Visible = true;
            }
        }

        protected string setTab(int idPestana, String txtNomOpcion)
        {
            #region LINQ ajustado al desarrollo llevado hasta el momento...

            string css_class = "";
            List<short> idtabs = consultas.Db.Tabs.Where(t => t.CodTab == idPestana).Select(t => t.Id_Tab).ToList();
            var query = from tur in consultas.Db.TareaUsuarioRepeticions
                        from tu in consultas.Db.TareaUsuarios
                        from tp in consultas.Db.TareaProgramas
                        where tp.Id_TareaPrograma == tu.CodTareaPrograma
                        && tu.Id_TareaUsuario == tur.CodTareaUsuario
                        && tu.CodProyecto == Convert.ToInt32(codProyecto)
                        && tp.Id_TareaPrograma == Constantes.CONST_PostIt
                        && tur.FechaCierre == null
                        select tur;

            var predicate = PredicateBuilder.False<Datos.TareaUsuarioRepeticion>();

            if (idtabs.Count > 0)
            {
                foreach (short idtab in idtabs)
                {
                    predicate = predicate.Or(t => SqlMethods.Like(t.Parametros, "Codproyecto=" + codProyecto + "&tab=" + idtab + "&Campo=%"));
                }
            }
            else
                predicate = predicate.Or(t => SqlMethods.Like(t.Parametros, "%tab=" + idPestana + "%"));

            query = query.Where(predicate);
            int cuantos = query.Count();

            if (cuantos > 0)
            {

                css_class = "tab_advertencia";
            }
            else
            {
                //var tbp = consultas.Db.TabProyectos.FirstOrDefault(t => t.CodProyecto == Convert.ToInt32(codProyecto) && t.CodTab == idPestana);
                //if (tbp != null && tbp.Realizado)
                //{

                //    css_class = "tab_aprobado";
                //}

                var RS = consultas.ObtenerDataTable("select realizado from tabproyecto where CodTab=" + idPestana + " and codproyecto=" + codProyecto, "text");
                if (RS.Rows.Count > 0) { if (Boolean.Parse(RS.Rows[0]["realizado"].ToString())) { css_class = "tab_aprobado"; } }
            }

            return css_class;

            #endregion

            ///Se usa como parámetro el nombre de "CodOpcion" en lugar de "idPestana".
            #region COMENTARIOS NO BORRAR! ajusta correctamente "Según FONADE clásico" las imágenes y valida la imagen en la pestaña.
            //DataTable RS = new DataTable();
            //String txtSQL = "";
            //String txtLink = "";
            //String txtPostIt = "";
            //String txtRealizado = "";
            //String txtWhere = "";
            //string css_class = "";

            //try
            //{
            //    #region Traer los Hijos de un tab.

            //    txtSQL = "select id_tab from tab where codtab = " + CodOpcion;

            //    RS = consultas.ObtenerDataTable(txtSQL, "text");

            //    if (RS.Rows.Count > 0)
            //    {
            //        foreach (DataRow row_RS in RS.Rows)
            //        {
            //            if (txtWhere == "") { txtWhere = txtWhere + "  parametros like 'Codproyecto= " + codProyecto + "&tab=" + row_RS["id_tab"].ToString() + "&Campo=%' "; }
            //            else { txtWhere = txtWhere + "  or parametros like 'Codproyecto=" + codProyecto + "&tab=" + row_RS["id_tab"].ToString() + "&Campo=%' "; }
            //        }
            //    }
            //    else { txtWhere = " parametros like '%tab=" + CodOpcion + "%' "; }

            //    #endregion

            //    #region Determinar si el tab tiene postit "y establecer la css_class para la pestaña".

            //    txtSQL = " select count(id_tareausuario) as cuantos " +
            //             " from tareausuariorepeticion tr, tareausuario t, tareaprograma " +
            //             " where id_tareaprograma=codtareaprograma and id_tareausuario=codtareausuario " +
            //             " and CodProyecto=" + codProyecto + " and id_tareaprograma=" + Constantes.CONST_PostIt +
            //             " and fechacierre is null ";

            //    txtSQL = txtSQL + "and (" + txtWhere + ")";

            //    RS = consultas.ObtenerDataTable(txtSQL, "text");

            //    if (RS.Rows.Count > 0)
            //    {
            //        if (Int32.Parse(RS.Rows[0]["cuantos"].ToString()) > 0)
            //        { /*css_class = "tab_advertencia";*/ txtPostIt = "Advertencia"; }
            //        else
            //        {
            //            txtSQL = " select realizado from tabproyecto where CodTab = " + CodOpcion + " and codproyecto=" + codProyecto;
            //            RS = consultas.ObtenerDataTable(txtSQL, "text");

            //            if (RS.Rows.Count > 0)
            //            {
            //                if (Boolean.Parse(RS.Rows[0]["realizado"].ToString())) { /*css_class = "tab_aprobado";*/ txtRealizado = "Aprobado"; }
            //            }
            //        }
            //    }

            //    #endregion

            //    css_class = "../../Images/bot" + txtNomOpcion + txtPostIt + txtRealizado + ".gif";
            //    return css_class;
            //}
            //catch { return css_class; } 
            #endregion
        }
    }

    public class MyTemplate : ITemplate
    {
        ListItemType _type;
        string _css_class;
        string _text;

        public MyTemplate(ListItemType type)
        {
            _type = type;
        }

        public MyTemplate(ListItemType type, string css_class, string text)
        {
            _css_class = css_class;
            _text = text;
        }

        public void InstantiateIn(Control control)
        {
            if (_type == ListItemType.Header)
            {
                control.Controls.Add(new LiteralControl("<span class='" + _css_class + "'>" + _text + "</span>"));
            }
        }
    }
}