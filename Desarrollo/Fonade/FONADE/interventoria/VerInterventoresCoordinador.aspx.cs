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

namespace Fonade.FONADE.interventoria
{
    public partial class VerInterventoresCoordinador : Negocio.Base_Page
    {


        protected int CodContacto;

        protected void Page_Load(object sender, EventArgs e)
        {
            CodContacto = Convert.ToInt32(Session["ContactoInterventor"]);

            var query = (from x in consultas.Db.Contactos
                         where x.Id_Contacto == CodContacto
                         select new
                         {
                             nombre = x.Nombres + " " + x.Apellidos,
                         }).FirstOrDefault();

            lbl_Titulo.Text = void_establecerTitulo("Interventores Para " + query.nombre);
            l_fechaActual.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
        }

        protected void lds_eval_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var query = from P in consultas.VerInterventoresDeCoordinador(CodContacto)
                            select P;
                e.Result = query;
            }
            catch (Exception exc)
            {
                throw new Exception("error al seleccionar los interventores", exc);
            }
        }

        protected void btn_cerrar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.close();", true);
        }
    }
}