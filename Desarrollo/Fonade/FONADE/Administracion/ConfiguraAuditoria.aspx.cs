using Fonade.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Administracion
{
    public partial class ConfiguraAuditoria : Negocio.Base_Page
    {
        string txtSQL;

        string bolActivar;
        string TiempoActualizacion;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtSQL = "SELECT Valor FROM parametro WHERE nomparametro='ActivarActualizacionInformacionUsuarios'";

            SqlDataReader reader = ejecutaReader(txtSQL, 1);
            if(reader.Read())
                bolActivar = reader["Valor"].ToString();

            txtSQL = "SELECT Valor FROM parametro WHERE nomparametro='TiempoValidacionActualizacionInformacionUsuarios'";

            reader = ejecutaReader(txtSQL, 1);
            if (reader.Read())
                TiempoActualizacion = reader["Valor"].ToString();

            if (!IsPostBack)
            {
                txtSQL = "SELECT TABLE_NAME FROM Fonade.INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME";

                var dt = consultas.ObtenerDataTable(txtSQL, "text");

                if (dt.Rows.Count > 0)
                {
                    ddltablas.DataSource = dt;
                    ddltablas.DataTextField = "TABLE_NAME";
                    ddltablas.DataValueField = "TABLE_NAME";
                    ddltablas.DataBind();

                    ddltablas.SelectedValue = "Contacto";

                    llenarCampos();
                }
            }
        }

        private void llenarCampos()
        {
            txtSQL = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + ddltablas.SelectedValue + "' ORDER BY ORDINAL_POSITION";

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            foreach (DataRow dr in dt.Rows)
            {
                TableRow fila = new TableRow();

                fila.Cells.Add(crearceldanormal((new CheckBox()
                    {
                        ID = dr["TABLE_NAME"].ToString() + dr["COLUMN_NAME"].ToString(),
                        Text = dr["COLUMN_NAME"].ToString(),
                        Checked = true
                    }), 1, 1, ""));

                tblcampos.Rows.Add(fila);
            }
        }

        private TableCell crearceldanormal(Control mensaje, int colspan, int rowspan, string cssestilo)
        {
            TableCell celda1 = new TableCell()
            {
                ColumnSpan = colspan,
                RowSpan = rowspan,
                CssClass = cssestilo
            };

            celda1.Controls.Add(mensaje);

            return celda1;
        }

        protected void btnguardar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(bolActivar))
                bolActivar = "1";
            else
                bolActivar = "0";

            txtSQL = "SELECT Valor FROM parametro WHERE nomparametro='ActivarActualizacionInformacionUsuarios'";

            SqlDataReader reader = ejecutaReader(txtSQL, 1);
            if (reader.Read())
                txtSQL = "UPDATE parametro SET valor = '" + bolActivar + "' WHERE nomparametro='ActivarActualizacionInformacionUsuarios'";
            else
                txtSQL = "INSERT INTO parametro (nomparametro,valor) VALUES('ActivarActualizacionInformacionUsuarios','" + bolActivar + "')";

            ejecutaReader(txtSQL, 2);

            if (!string.IsNullOrEmpty(TiempoActualizacion)) { }
            else
                TiempoActualizacion = "30";

            txtSQL = "SELECT Valor FROM parametro WHERE nomparametro='TiempoValidacionActualizacionInformacionUsuarios'";

            reader = ejecutaReader(txtSQL, 1);
            if (reader.Read())
                txtSQL = "UPDATE parametro SET valor = '" + TiempoActualizacion + "' WHERE nomparametro='TiempoValidacionActualizacionInformacionUsuarios'";
            else
                txtSQL = "INSERT INTO parametro (nomparametro,valor) VALUES('TiempoValidacionActualizacionInformacionUsuarios','" + TiempoActualizacion + "')";

            ejecutaReader(txtSQL, 2);

            if (bolActivar.Equals("1"))
            {
                var result = from a in consultas.Db.MD_AuditoriaAdministrador() select a;
            }

            Correos workerObject = new Correos();
            Thread workerThread = new Thread(workerObject.DOCorreos);

            workerObject.inicio();

            workerThread.Start();
        }

        protected void ddltablas_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarCampos();
        }
    }

    public class Correos : Negocio.Base_Page
    {
        private DataTable dt;

        public void DOCorreos()
        {
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    Correo correo = new Correo(usuario.Email.Trim(),
                                               "Fondo Emprender",
                                               dr["Email"].ToString().Trim(),
                                               dr["Nombres"].ToString() + " " + dr["Apellidos"].ToString(),
                                               "Fondo Emprender Actualizacion Masiva",
                                               "Registro a Fondo Emprender");
                    correo.Enviar();
                }
                catch { }
            }
        }

        public void inicio()
        {
            dt = consultas.ObtenerDataTable("MD_AuditoriaAdministrador");
        }
    }
}