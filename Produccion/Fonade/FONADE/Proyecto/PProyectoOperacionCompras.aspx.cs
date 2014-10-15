using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Web.Services;
using System.Web.Script.Serialization;
 

namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoOperacionCompras : Negocio.Base_Page
    {
        public string codProyecto;
        public int txtTab = Constantes.CONST_Compras;
        public string codConvocatoria;
        //public ProyectoOperacion po;

        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo== Constantes.CONST_AdministradorFonade)
            {
                Post_It1.Visible = false;
            }
            if (Request.QueryString["codProyecto"] != null)
            {
                codProyecto = Request.QueryString["codProyecto"].ToString();
            }
            if (Request.QueryString["codConvocatoria"] != null)
                codConvocatoria = Request.QueryString["codConvocatoria"].ToString();
           
            if (!IsPostBack)
            {
                construirEncabezado();
            }
        }

        protected void btn_addInsumo_Click(object sender, EventArgs e)
        {
            Session["CodProyecto"] = "49781";
            Session["Id_Producto"] = "90824";
            Session["Insumo"] = "0";
            Response.Redirect("VerInsumos.aspx");
        }


    }
}