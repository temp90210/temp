using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.Ayuda
{
    public partial class Mensaje : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            L_Mensaje.Text = "";
            P_mensaje1.Visible = false;
            P_mensaje2.Visible = false;
            P_mensaje3.Visible = false;
            P_mensaje4.Visible = false;
            P_mensaje5.Visible = false;
            P_mensaje6.Visible = false;
            P_mensaje7.Visible = false;
            P_mensaje8.Visible = false;
            try
            {
                if (Session["mensaje"].ToString().Equals("1"))
                {
                    L_Mensaje.Text = "Proyección de ventas";
                    P_mensaje1.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("2"))
                {
                    L_Mensaje.Text = "Justificación de Proyección de Ventas";
                    P_mensaje2.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("3"))
                {
                    L_Mensaje.Text = "Política de Cartera";
                    P_mensaje3.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("4"))
                {
                    L_Mensaje.Text = "Indicadores de Gestión";
                    P_mensaje4.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("5"))
                {
                    L_Mensaje.Text = "Producción";
                    P_mensaje5.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("6"))
                {
                    L_Mensaje.Text = "Ventas";
                    P_mensaje6.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("7"))
                {
                    L_Mensaje.Text = "Conceptos de Justificación";
                    P_mensaje7.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("8"))
                {
                    L_Mensaje.Text = "Justificación";
                    P_mensaje8.Visible = true;
                }
                if (Session["mensaje"].ToString().Equals("9"))
                {
                    L_Mensaje.Text = "Consumos por Unidad de Producto";
                    P_mensaje9.Visible = true;
                }
            }
            catch (Exception) { }
        }
    }
}