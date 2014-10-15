using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Fiduciaria
{
    public partial class PagosActividadFiducia : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //lblTitulo.Text = "Solicitudes de Pago Enviadas a Fiduciaria";
            lbl_Titulo.Text = void_establecerTitulo("Solicitudes de Pago");
            if (!IsPostBack)
            {
                CargarSolicitudes();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/04/2014.
        /// Cargar la grilla de "Tipos de Aprendices".
        /// </summary>
        private void CargarSolicitudes()
        {
            //Inicializar variables.
            DataTable tabla = new DataTable();
            DataTable tabla_dos = new DataTable();
            String txtSQL;

            txtSQL =    " SELECT PagosActaSolicitudes.Id_Acta, PagosActaSolicitudes.Fecha, convert(varchar(8000),PagosActaSolicitudes.DatosFirma) AS DatosFirma " +
                        " FROM PagosActaSolicitudes " +
                        " INNER JOIN PagosActaSolicitudPagos ON PagosActaSolicitudes.Id_Acta = PagosActaSolicitudPagos.CodPagosActaSolicitudes " +
                        " WHERE (PagosActaSolicitudes.DescargadoFA = 0) " +
                        " AND (PagosActaSolicitudes.CodRechazoFirmaDigital IS NULL) " +
                        " AND (PagosActaSolicitudes.Tipo = 'Fonade') " +
                        " AND (PagosActaSolicitudPagos.Aprobado = 1) " +
                        " AND (PagosActaSolicitudes.CodContactoFiduciaria = " + usuario.IdContacto + " ) " +
                        " GROUP BY PagosActaSolicitudes.Id_Acta, PagosActaSolicitudes.Fecha, convert(varchar(8000),PagosActaSolicitudes.DatosFirma) ";

            tabla = consultas.ObtenerDataTable(txtSQL, "text");

            //Crear la columna de numero de solicitudes:
            tabla.Columns.Add("NEW", typeof(System.String));

            //Crear la columna de estado:
            tabla.Columns.Add("ESTADO", typeof(System.String));

            foreach (DataRow fila in tabla.Rows)
            {
                txtSQL = " SELECT COUNT(CodPagosActaSolicitudes) AS NumSolicitudes FROM PagosActaSolicitudPagos WHERE Aprobado = 1 AND CodPagosActaSolicitudes = " + fila["Id_Acta"].ToString();
                var tabla_temporal = consultas.ObtenerDataTable(txtSQL, "text");

                if (tabla_temporal.Rows.Count > 0)
                {
                    fila["NEW"] = tabla_temporal.Rows[0]["NumSolicitudes"].ToString();
                }
                else
                {
                    fila["NEW"] = "0";
                }

                //if (fila["DescargadoFA"].ToString() == "0")
                //{
                //    fila["ESTADO"] = "No descargada";
                //}
                //else
                //{
                //    fila["ESTADO"] = "Descargada";
                //}
                tabla_temporal = null;
            }

            Session["Tabla"] = tabla;

            gvSolicitudes.DataSource = tabla;
            gvSolicitudes.DataBind();


        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Navegación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvSolicitudes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["Tabla"] as DataTable;

            if (dt != null)
            {
                gvSolicitudes.PageIndex = e.NewPageIndex;
                gvSolicitudes.DataSource = dt;
                gvSolicitudes.DataBind();
            }
        }

        protected void gvSolicitudes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("mostrar"))
            {
                Session["CodActaFonade"] = e.CommandArgument.ToString();

                Response.Redirect("DescargarFA.aspx");
            }
        }
    }
}