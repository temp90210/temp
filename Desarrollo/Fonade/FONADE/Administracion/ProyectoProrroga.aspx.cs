using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Administracion
{
    public partial class ProyectoProrroga : Negocio.Base_Page
    {
        string txtSQL;

        string Id_ProyectoEval;
        string NomproyectoEval;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtSQL = "select Id_proyecto,nomproyecto,prorroga from ProyectoProrroga,Proyecto where id_proyecto=codProyecto";

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            gv_proyectosProrroga.DataSource = dt;
            gv_proyectosProrroga.DataBind();

            Id_ProyectoEval = Session["Id_ProyectoEval"] != null && !string.IsNullOrEmpty(Session["Id_ProyectoEval"].ToString()) ? Session["Id_ProyectoEval"].ToString() : "0";
            NomproyectoEval = Session["NomproyectoEval"] != null && !string.IsNullOrEmpty(Session["NomproyectoEval"].ToString()) ? Session["NomproyectoEval"].ToString() : "0";

            if (Id_ProyectoEval != "0")
            {
                pnlproyectos.Visible = false;
                pnlproyectos.Enabled = false;

                pnlAgregar.Visible = true;
                pnlAgregar.Enabled = true;

                txtproyecto.Text = NomproyectoEval;
            }
        }

        protected void imgagregar_Click(object sender, ImageClickEventArgs e)
        {
            txtproyecto.Text = null;
            txtmeses.Text = null;

            pnlproyectos.Visible = false;
            pnlproyectos.Enabled = false;

            pnlAgregar.Visible = true;
            pnlAgregar.Enabled = true;
        }

        protected void lnkagregar_Click(object sender, EventArgs e)
        {
            txtproyecto.Text = null;
            txtmeses.Text = null;

            pnlproyectos.Visible = false;
            pnlproyectos.Enabled = false;

            pnlAgregar.Visible = true;
            pnlAgregar.Enabled = true;
        }

        protected void btnvolver_Click(object sender, EventArgs e)
        {
            Session["Id_ProyectoEval"] = null;
            Session["NomproyectoEval"] = null;

            pnlproyectos.Visible = true;
            pnlproyectos.Enabled = true;

            pnlAgregar.Visible = false;
            pnlAgregar.Enabled = false;
        }

        protected void lnkbuscar_Click(object sender, EventArgs e)
        {
            Redirect(null, "BuscarProyecto.aspx", "_Blank", "width=730,height=585");
        }

        protected void btnadicionar_Click(object sender, EventArgs e)
        {
            txtSQL = "select * from ProyectoProrroga where codproyecto=" + Id_ProyectoEval;

            SqlDataReader reader = ejecutaReader(txtSQL, 1);

            if (reader.Read())
            {
                txtSQL = "update ProyectoProrroga set prorroga = prorroga + " + txtmeses.Text + " where codproyecto = " + Id_ProyectoEval;
                reader = ejecutaReader(txtSQL, 2);
            }
            else
            {
                txtSQL = "insert into ProyectoProrroga values (" + Id_ProyectoEval + "," + txtmeses.Text + ")";
                reader = ejecutaReader(txtSQL, 2);
            }

            txtSQL = "select Id_proyecto,nomproyecto,prorroga from ProyectoProrroga,Proyecto where id_proyecto=codProyecto";

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            gv_proyectosProrroga.DataSource = dt;
            gv_proyectosProrroga.DataBind();

            Session["Id_ProyectoEval"] = null;
            Session["NomproyectoEval"] = null;

            pnlproyectos.Visible = true;
            pnlproyectos.Enabled = true;

            pnlAgregar.Visible = false;
            pnlAgregar.Enabled = false;
        }

        protected void gv_proyectosProrroga_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_proyectosProrroga.PageIndex = e.NewPageIndex;
            gv_proyectosProrroga.DataBind();
        }

        private void ModifcarFecha(int id_tareausario)
        {
            int repeticion;

            repeticion = id_tareausario;

            var result = (from tu in consultas.Db.TareaUsuarioRepeticions
                          where tu.Id_TareaUsuarioRepeticion == repeticion
                          select tu).FirstOrDefault();
        }
    }
}