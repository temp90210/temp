using Datos;
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
    public partial class InterventorAgenda : Negocio.Base_Page
    {
        string txtSQL;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtSQL = "UPDATE Visita SET Estado='Realizada' WHERE ID_Visita IN (SELECT Id_Visita FROM Visita WHERE FechaFin < getdate())";

                ejecutaReader(txtSQL, 2);

                txtSQL = "UPDATE Visita SET Estado='Pendiente' WHERE ID_Visita IN (SELECT Id_Visita FROM Visita WHERE FechaFin >= getdate())";

                ejecutaReader(txtSQL, 2);

                txtSQL = "SELECT CodContacto,Rol From EmpresaInterventor,Empresa Where id_empresa=codempresa and CodContacto =" + usuario.IdContacto + " and inactivo=0 and FechaFin is null order by rol desc";

                SqlDataReader reader = ejecutaReader(txtSQL, 1);

                if (reader != null)
                {
                    if (reader.Read())
                        Session["CodRol"] = reader["Rol"].ToString();
                    else
                        Session["CodRol"] = null;
                }

                if (usuario.CodGrupo == Constantes.CONST_Interventor && Session["CodRol"].ToString() == Constantes.CONST_RolInterventorLider.ToString())
                {
                    btnNuevaVisita.Visible = true;
                    btnNuevaVisita.Enabled = true;
                }
                else
                {
                    btnNuevaVisita.Enabled = false;
                    btnNuevaVisita.Visible = false;
                }

                llenarGrilla();
            }
        }

        private void llenarGrilla()
        {
            txtSQL = @"SELECT Empresa.razonsocial, Empresa.Nit, Visita.Id_Visita, Visita.FechaInicio, Visita.FechaFin, lower(Visita.Estado) as Estado
                        FROM Visita
                        INNER JOIN Empresa ON Visita.Id_Empresa = Empresa.id_empresa
                        WHERE (Visita.Id_Interventor = " + usuario.IdContacto + @")
                        ORDER BY Empresa.razonsocial";

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            gv_agenda.DataSource = dt;
            gv_agenda.DataBind();
        }

        protected void gv_agenda_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id_visita = e.CommandArgument.ToString();

            Session["Id_Visita"] = id_visita;
            Session["Tipo"] = 2;

            Redirect(null, "AdicionarVisita.aspx", "_Blank", "width=995,height=300");
        }

        protected void btnNuevaVisita_Click(object sender, EventArgs e)
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_RolInterventorLider)
            {
                Session["Id_Visita"] = "0";
                Session["Tipo"] = "1";
                Redirect(null, "AdicionarVisita.aspx", "_blank", "width=1000,height=350");
            }
        }
    }
}