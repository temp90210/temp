#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>08 - 07 - 2014</Fecha>
// <Archivo>Post_It.cs</Archivo>

#endregion

using Datos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.Controles
{
    public partial class Post_It : System.Web.UI.UserControl
    {
        private String codProyecto;
        private String codConvocatoria;
        private String sql;
        private DataTable resulData;
        private Consultas cons;

        /// <summary>
        /// Diego QUiñonez
        /// Metodo de carga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtCampo))
            {
                //recoger variables de session
                try
                {
                    if (!String.IsNullOrEmpty(Session["codProyecto"].ToString()))
                    {
                        codProyecto = Session["CodProyecto"].ToString();
                        codConvocatoria = Session["CodConvocatoria"].ToString();
                    }
                }
                catch (Exception) { }

                realizarConsulta();
            }
        }

        /// <summary>
        /// Diego Quiñonez
        /// metodo que devuelve cantidad de tareas
        /// visibles por el usuario
        /// ademas valida de acuerdo a las tareas si se ve o no el post_it
        /// </summary>
        private void realizarConsulta()
        {
//            sql = @"select count(id_tareausuario) as cuantos
//                    from tareausuariorepeticion tr, tareausuario t, tareaprograma
//                    where id_tareausuario=codtareausuario and CodProyecto=" + codProyecto + @" and id_tareaprograma=codtareaprograma
//                    and id_tareaprograma=" + Constantes.CONST_PostIt + " and fechacierre is null";
            //WAFS se coloca el codigo de proyeco en "null" para cuando el proyecto no tiene datos de organizacion estrategia porque estaba abordando
            string icodProyecto = string.IsNullOrEmpty((codProyecto)) ? "null" : codProyecto; //WAFS 19-OCT-2014
            sql = @"select count(id_tareausuario) as cuantos
                    from tareausuariorepeticion tr, tareausuario t, tareaprograma
                    where id_tareausuario=codtareausuario and CodProyecto=" + icodProyecto + @" and id_tareaprograma=codtareaprograma
                    and id_tareaprograma=" + Constantes.CONST_PostIt + " and fechacierre is null"; //WAFS 19-OCT-2014

            if (!String.IsNullOrEmpty(txtCampo))
            {
                if (String.IsNullOrEmpty(codConvocatoria))
                    sql += " and parametros like '%Campo=" + txtCampo + "%'";
                else
                    sql += " and parametros like '%Campo=" + txtCampo + "%'";

            }

            cons = new Consultas();

            resulData = cons.ObtenerDataTable(sql, "text");

            if (Int32.Parse(resulData.Rows[0][0].ToString()) == 0)
            {
                LB_Listar.Visible = false;
                LB_Listar.Enabled = false;
            }
            else
            {
                LB_Listar.Visible = true;
                LB_Listar.Enabled = true;
            }

            LB_Listar.Text = "" + resulData.Rows[0][0].ToString();
            LB_Listar.CssClass = "encima";
        }

        //protected void I_Listar_Click(object sender, ImageClickEventArgs e)
        //{
        //    Session["EvalCodProyectoPOst"] = codProyecto;
        //    Session["EvalCodUsuario"] = codUsuario;
        //    Session["tabEval"] = txtTab;
        //    Session["EvalConsPOST"] = Constantes.CONST_PostIt;
        //    Session["Campo"] = txtCampo;

        //    //cons = new Consultas();

        //    Redirect(null, "~/Controles/ListarPostIt.aspx", "_Blank", "width=730,height=470");
        //}

        /// <summary>
        /// Diego Quiñonez
        /// permite abrir paginas emergentes
        /// </summary>
        /// <param name="response"></param>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <param name="windowFeatures"></param>
        public static void Redirect(HttpResponse response, string url, string target, string windowFeatures)
        {
            if ((string.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && string.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;

                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);

                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
            }
        }

        /// <summary>
        /// Diego Quiñonez
        /// metodo que llma a listar todas las tareas
        /// creadas y pendientes de acuerdo al id del usuario logeado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LB_Listar_Click(object sender, EventArgs e)
        {
            Session["EvalCodProyectoPOst"] = codProyecto;
            Session["EvalCodUsuario"] = codUsuario;
            Session["tabEval"] = txtTab;
            Session["EvalConsPOST"] = Constantes.CONST_PostIt;
            Session["Campo"] = txtCampo;

            Redirect(null, "~/Controles/ListarPostIt.aspx", "_Blank", "width=730,height=470");
        }

        /// <summary>
        /// Diego Quiñonez
        /// carga una ventana emergente que permite agendar una nueva tarea
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void I_POs_Click(object sender, ImageClickEventArgs e)
        {
            Session["EvalCodProyectoPOst"] = codProyecto;
            Session["EvalCodUsuario"] = codUsuario;
            Session["tabEval"] = txtTab;
            Session["EvalConsPOST"] = Constantes.CONST_PostIt;
            Session["Campo"] = txtCampo;
            Session["EvalAccion"] = Accion;

            cons = new Consultas();

            Redirect(null, "~/Controles/PostIt.aspx", "_Blank", "width=730,height=585");
        }

        /// <summary>
        /// Diego Quiñonez
        /// parametros que permiten construir
        /// el objeto post_it
        /// </summary>
        #region encapsularData
        private Int32 txtTab;

        public Int32 _txtTab
        {
            get { return txtTab; }
            set { txtTab = value; }
        }

        private String txtCampo;

        public String _txtCampo
        {
            get { return txtCampo; }
            set { txtCampo = value; }
        }

        private String codUsuario;

        public String _codUsuario
        {
            get { return codUsuario; }
            set { codUsuario = value; }
        }

        private String Accion;

        public String _accion
        {
            get { return Accion; }
            set { Accion = value; }
        }
        #endregion
    }
}