#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>08 - 07 - 2014</Fecha>
// <Archivo>ListarPostIt1.cs</Archivo>

#endregion

using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.Controles
{
    public partial class ListarPostIt1 : Negocio.Base_Page
    {
        #region entradaDatos
        private Int32 CONST_PostIt;
        private String codProyecto;
        private Int32 CodUsuario;
        private String tabEval;
        private String txtCampo;
        private Int32 codGrupo;
        #endregion

        //string utilizado para realizar consultas a la base de datos
        private String txtSQL = "";

        //datatables que recogen la informacion de las tareas a mostrar
        private DataTable dtContacto;
        private DataTable datatable;
        private DataTable dt;

        /// <summary>
        /// Diego Quiñonez
        /// retorna la cantidad de tareas encontradas
        /// por el usuario
        /// </summary>
        public Int32 PublicData
        {
            get { return dt.Rows.Count; }
        }

        //objeto consulta que permite acceder a la capa datos
        //y obtener lo requerido por la BD
        private Consultas consulta = new Consultas();

        /// <summary>
        /// Diego Quiñonez
        /// metodo de carga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            recogeSession();

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

            llenarDataTable();
            lenargrilla();

            GV_POST.DataSource = dt;
            GV_POST.DataBind();
        }

        /// <summary>
        /// Diego Quiñonez
        /// recoge los datos de entrada de la session
        /// en caso de no existir cierra el post_it
        /// </summary>
        public void recogeSession()
        {
            try
            {
                #region cargar datos de session

                //datos de session
                codProyecto = Session["EvalCodProyectoPOst"] != null && !string.IsNullOrEmpty(Session["EvalCodProyectoPOst"].ToString()) ? Session["EvalCodProyectoPOst"].ToString() : string.Empty;
                CodUsuario = Session["EvalCodUsuario"] != null && !string.IsNullOrEmpty(Session["EvalCodUsuario"].ToString()) ? Convert.ToInt32(Session["EvalCodUsuario"].ToString()) : usuario.IdContacto;
                tabEval = Session["tabEval"] != null && !string.IsNullOrEmpty(Session["tabEval"].ToString()) ? Session["tabEval"].ToString() : string.Empty;
                CONST_PostIt = Session["EvalConsPOST"] != null && !string.IsNullOrEmpty(Session["EvalConsPOST"].ToString()) ? Convert.ToInt32(Session["EvalConsPOST"].ToString()) : Constantes.CONST_PostIt;
                txtCampo = Session["Campo"] != null && !string.IsNullOrEmpty(Session["Campo"].ToString()) ? Session["Campo"].ToString() : "nulo";
                codProyecto = string.IsNullOrEmpty(codProyecto) && Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : string.Empty;

                #endregion

                try { codGrupo = usuario.CodGrupo; }
                catch (Exception) { codGrupo = -1; }
            }
            catch (Exception)
            {
                ClientScriptManager cm = this.ClientScript;
                //si algo falla cierra el post_it
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.close();</script>");
            }
        }

        /// <summary>
        /// Diego Quiñonez
        /// </summary>
        public void llenarDataTable()
        {
            txtSQL = @"select id_tareausuariorepeticion, nomtareausuario, codcontacto, fecha,
                    c.nombres+' '+c.apellidos as NombreAgendo, c2.nombres+' '+c2.apellidos as Nombre, codcontactoagendo
                    from tareausuariorepeticion tr, tareausuario t, tareaprograma, contacto c, contacto c2
                    where id_tareausuario=codtareausuario and c.id_contacto=codcontactoagendo
                    and c2.id_contacto=codcontacto and CodProyecto=" + codProyecto + @" and
                    id_tareaprograma=codtareaprograma and id_tareaprograma=" + CONST_PostIt + " and fechacierre is null";

            if (!String.IsNullOrEmpty(txtCampo))
            {
                txtSQL += " and parametros like '%Campo=" + txtCampo + "%'";
            }

            try
            {
                dtContacto = consulta.ObtenerDataTable(txtSQL, "text");
            }
            catch (NullReferenceException) { }
            catch (SqlException) { }
        }

        /// <summary>
        /// Diego Quiñonez
        /// </summary>
        public void lenargrilla()
        {
            dt = new DataTable();

            dt.Columns.Add("Fecha");
            dt.Columns.Add("Tarea");
            dt.Columns.Add("Agendado");
            dt.Columns.Add("Agendo");

            if (dtContacto.Rows.Count > 0)
            {

                txtSQL = @"select codrol from contacto c, proyectocontacto pc
                        where id_contacto=" + dtContacto.Rows[0]["codcontactoagendo"].ToString() + " and id_contacto=codcontacto";

                try
                {
                    consulta = new Consultas();

                    datatable = consulta.ObtenerDataTable(txtSQL, "text");
                }
                catch (NullReferenceException) { }
                catch (SqlException) { }

                for (int i = 0; i < dtContacto.Rows.Count; i++)
                {
                    DateTime time = (DateTime.Parse(dtContacto.Rows[i]["fecha"].ToString()));
                    String Agendo = "";

                    if (codGrupo != 9 && codGrupo != 10)
                    {
                        if (datatable.Rows.Count > 0)
                        {
                            if (datatable.Rows[0]["CodRol"].ToString().Equals("4"))
                            {
                                Agendo = "EVALUADOR";
                            }
                            else
                            {
                                Agendo = dtContacto.Rows[i]["NombreAgendo"].ToString();
                            }
                        }
                        else
                        {
                            Agendo = dtContacto.Rows[i]["NombreAgendo"].ToString();
                        }
                    }

                    DataRow dr = dt.NewRow();

                    dr["Fecha"] = time;
                    dr["Tarea"] = dtContacto.Rows[i]["nomtareausuario"].ToString();
                    dr["Agendado"] = dtContacto.Rows[i]["Nombre"].ToString();
                    dr["Agendo"] = Agendo;

                    dt.Rows.Add(dr);
                }
            }
        }
    }
}