using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Localizacion
{
    public partial class ProyectoLocalizacion : Negocio.Base_Page
    {
        string pid;
        string pc;

        string txtSQL;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                pid = Session["pid"] != null && !string.IsNullOrEmpty(Session["pid"].ToString()) ? Session["pid"].ToString() : "0";
                pc = Session["pc"] != null && !string.IsNullOrEmpty(Session["pc"].ToString()) ? Session["pc"].ToString() : "0";
            }
            catch (NullReferenceException)
            {
                ClientScriptManager cm = this.ClientScript;
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                return;
            }

            if (string.IsNullOrEmpty(pid))
                txtSQL = "Select NomDepartamento, Id_Departamento FROM Departamento WHERE Id_Departamento = " + pc;
            else
                txtSQL = "select nomdepartamento, Id_Departamento from proyecto p, institucion, subsector, ciudad, departamento WHERE id_subsector=codsubsector and id_institucion = codinstitucion and id_ciudad=p.codciudad and id_departamento=codDepartamento and id_proyecto=" + pid;

            SqlDataReader reader = ejecutaReader(txtSQL, 1);

            if (reader != null)
            {
                if (reader.Read())
                {
                    imgbtn_mapa.ImageUrl = "~/Images/mapas/" + reader["NomDepartamento"].ToString() + ".gif";
                    Session["Id_Departamento_Localizacion"] = reader["Id_Departamento"].ToString();
                }
                reader = null;
            }
        }

        protected void imgbtn_mapa_Click(object sender, ImageClickEventArgs e)
        {
            ClientScriptManager cm = this.ClientScript;


            int coorX = e.X;
            int coorY = e.Y;

            Session["coorXX"] = coorX;
            Session["coorYY"] = coorY;

            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        }
    }
}