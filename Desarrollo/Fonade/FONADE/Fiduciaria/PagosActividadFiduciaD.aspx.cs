using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Fiduciaria
{
    public partial class PagosActividadFiduciaD : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Titulo.Text = void_establecerTitulo("Solicitudes De Pago Descargadas");

            if (!IsPostBack)
            {
                CargarTiposDeAprendices();
            }

        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (textBoxSolicitud.Text != "")
            {
                CargarTiposDeAprendices(textBoxSolicitud.Text);
            }
            else
            {
                CargarTiposDeAprendices();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/04/2014.
        /// Cargar la grilla de "Tipos de Aprendices".
        /// </summary>
        private void CargarTiposDeAprendices()
        {
            //Inicializar variables.
            DataTable tabla = new DataTable();
            DataTable tabla_dos = new DataTable();
            String txtSQL;

            txtSQL = " SELECT TOP 10 PagosActaSolicitudes.Id_Acta, PagosActaSolicitudes.Fecha, CONVERT(varchar(8000), PagosActaSolicitudes.DatosFirma) AS DatosFirma," +
                        " PagosActaSolicitudes.CodActaFonade, PagosActaSolicitudes.DescargadoFA " +
                        " FROM PagosActaSolicitudes " +
                        " INNER JOIN PagosActaSolicitudPagos ON PagosActaSolicitudes.Id_Acta = PagosActaSolicitudPagos.CodPagosActaSolicitudes " +
                        " WHERE (PagosActaSolicitudes.Tipo = 'Fiduciaria') AND (PagosActaSolicitudes.CodActaFonade IS NOT NULL) " +
                        " AND PagosActaSolicitudes.CodContactoFiduciaria =  " + usuario.IdContacto +
                        " GROUP BY PagosActaSolicitudes.Id_Acta, PagosActaSolicitudes.Fecha, CONVERT(varchar(8000), PagosActaSolicitudes.DatosFirma), " +
                        " PagosActaSolicitudes.CodActaFonade, PagosActaSolicitudes.DescargadoFA " +
                        " ORDER BY PagosActaSolicitudes.Fecha  DESC ";

            tabla = consultas.ObtenerDataTable(txtSQL, "text");

            //Crear la columna de numero de solicitudes:
            tabla.Columns.Add("NEW", typeof(System.String));

            //Crear la columna de estado:
            tabla.Columns.Add("ESTADO", typeof(System.String));

            foreach (DataRow fila in tabla.Rows)
            {
                txtSQL = " SELECT COUNT(CodPagosActaSolicitudes) AS NumSolicitudes FROM PagosActaSolicitudPagos WHERE Aprobado = 1 AND CodPagosActaSolicitudes = " + fila["CodActaFonade"].ToString();
                var tabla_temporal = consultas.ObtenerDataTable(txtSQL, "text");

                if (tabla_temporal.Rows.Count > 0)
                {
                    fila["NEW"] = tabla_temporal.Rows[0]["NumSolicitudes"].ToString();
                }
                else
                {
                    fila["NEW"] = "0";
                }

                if (fila["DescargadoFA"].ToString() == "False")
                {
                    fila["ESTADO"] = "No descargada";
                }
                else
                {
                    fila["ESTADO"] = "Descargada";
                }
                tabla_temporal = null;

            }

            Session["Tabla"] = tabla;

            gvActas.DataSource = tabla;
            gvActas.DataBind();


        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/04/2014.
        /// Cargar la grilla de "Tipos de Aprendices".
        /// </summary>
        private void CargarTiposDeAprendices(String filtro)
        {
            //Inicializar variables.
            DataTable tabla = new DataTable();
            DataTable tabla_dos = new DataTable();
            String txtSQL;

            txtSQL = " SELECT TOP 10 PagosActaSolicitudes.Id_Acta, PagosActaSolicitudes.Fecha, CONVERT(varchar(8000), PagosActaSolicitudes.DatosFirma) AS DatosFirma," +
                        " PagosActaSolicitudes.CodActaFonade, PagosActaSolicitudes.DescargadoFA " +
                        " FROM PagosActaSolicitudes " +
                        " INNER JOIN PagosActaSolicitudPagos ON PagosActaSolicitudes.Id_Acta = PagosActaSolicitudPagos.CodPagosActaSolicitudes " +
                        " WHERE (PagosActaSolicitudes.Tipo = 'Fiduciaria') AND (PagosActaSolicitudes.CodActaFonade IS NOT NULL) " +
                        " AND PagosActaSolicitudes.CodContactoFiduciaria =  '" + usuario.IdContacto + "' " +
                        " AND PagosActaSolicitudes.CodActaFonade = '" + filtro + "' " +
                        " GROUP BY PagosActaSolicitudes.Id_Acta, PagosActaSolicitudes.Fecha, CONVERT(varchar(8000), PagosActaSolicitudes.DatosFirma), " +
                        " PagosActaSolicitudes.CodActaFonade, PagosActaSolicitudes.DescargadoFA " +
                        " ORDER BY PagosActaSolicitudes.Fecha  DESC ";

            tabla = consultas.ObtenerDataTable(txtSQL, "text");

            //Crear la columna de numero de solicitudes:
            tabla.Columns.Add("NEW", typeof(System.String));

            //Crear la columna de estado:
            tabla.Columns.Add("ESTADO", typeof(System.String));

            foreach (DataRow fila in tabla.Rows)
            {
                txtSQL = " SELECT COUNT(CodPagosActaSolicitudes) AS NumSolicitudes FROM PagosActaSolicitudPagos WHERE Aprobado = 1 AND CodPagosActaSolicitudes = " + fila["CodActaFonade"].ToString();
                var tabla_temporal = consultas.ObtenerDataTable(txtSQL, "text");

                if (tabla_temporal.Rows.Count > 0)
                {
                    fila["NEW"] = tabla_temporal.Rows[0]["NumSolicitudes"].ToString();
                }
                else
                {
                    fila["NEW"] = "0";
                }

                if (fila["DescargadoFA"].ToString() == "False")
                {
                    fila["ESTADO"] = "No descargada";
                }
                else
                {
                    fila["ESTADO"] = "Descargada";
                }
                tabla_temporal = null;

            }

            Session["Tabla"] = tabla;

            gvActas.DataSource = tabla;
            gvActas.DataBind();


        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Navegación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvActas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["Tabla"] as DataTable;

            if (dt != null)
            {
                gvActas.PageIndex = e.NewPageIndex;
                gvActas.DataSource = dt;
                gvActas.DataBind();
            }
        }

        protected void gvActas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("mostrar"))
            {
                Session["CodActaFonade"] = e.CommandArgument.ToString();

                Response.Redirect("DescargarFA.aspx");
            }
        }

    }
}