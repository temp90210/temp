#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>28 - 02 - 2014</Fecha>
// <Archivo>EvaluacionFrameSet.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using LinqKit;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionFrameSet : Negocio.Base_Page
    {
        public string codConvocatoria;
        public string codProyecto;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador))
            {
                TabPanel1.Visible = false;
                TabPanel1.Enabled = false;
            }

            try
            {
                if (!string.IsNullOrEmpty(Session["CodProyecto"].ToString()))
                {
                    codProyecto = Session["CodProyecto"].ToString();

                }
                else
                {
                    Response.Redirect("../MiPerfil/Home.aspx");
                }
                if (!string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()))
                {
                    codConvocatoria = Session["CodConvocatoria"].ToString();
                }
            }
            catch (NullReferenceException) { return; }
            setTabsStatus();
            
        }

        private void setTabsStatus()
        {
            var codEstado = (from p in consultas.Db.Proyectos
                             where p.Id_Proyecto == Convert.ToInt32(codProyecto)
                             select p).FirstOrDefault();

            int codigoEstado = 0;
            if (codEstado != null)
            {
                codigoEstado = codEstado.CodEstado;
            }

            
        }

        protected string setTab(int idPestana)
        {
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

            var predicate = PredicateBuilder.False<TareaUsuarioRepeticion>();

            if (idtabs.Count > 0)
            {
                foreach (short idtab in idtabs)
                {
                    predicate =
                        predicate.Or(
                            t =>
                            SqlMethods.Like(t.Parametros, "Codproyecto=" + codProyecto + "&tab=" + idtab + "&Campo=%"));
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
                var tbp =
                    consultas.Db.TabProyectos.FirstOrDefault(
                        t => t.CodProyecto == Convert.ToInt32(codProyecto) && t.CodTab == idPestana);
                if (tbp != null && tbp.Realizado)
                {
                    css_class = "tab_aprobado";
                }
            }
            return css_class;
        }
    }

    
}public class MyTemplate : ITemplate
    {
        private readonly string _css_class;
        private readonly string _text;
        private readonly ListItemType _type;

        public MyTemplate(ListItemType type)
        {
            _type = type;
        }

        public MyTemplate(ListItemType type, string css_class, string text)
        {
            _css_class = css_class;
            _text = text;
        }

        #region ITemplate Members

        public void InstantiateIn(Control control)
        {
            if (_type == ListItemType.Header)
            {
                control.Controls.Add(new LiteralControl("<span class='" + _css_class + "'>" + _text + "</span>"));
            }
        }

        #endregion
    }

