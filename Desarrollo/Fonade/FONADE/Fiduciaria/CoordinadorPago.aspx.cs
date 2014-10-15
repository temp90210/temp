using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Fiduciaria
{
    public partial class CoordinadorPago : Negocio.Base_Page
    {
        String IdPagoActividad;

        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime fecha = DateTime.Now;
            string sMes = fecha.ToString("MMM", CultureInfo.CreateSpecificCulture("es-CO"));
            FechaActual.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year;

            IdPagoActividad = Session["Id_PagoActividad"] != null ? IdPagoActividad = Session["Id_PagoActividad"].ToString() : "0";
            CargarDetalleSolicitud(IdPagoActividad);
            NombreUsuario.Text = usuario.Nombres + " " + usuario.Apellidos;
            NombrePagina.Text = "Solicitud De Pago";
        }

        /// <summary>
        /// Establecer el primer valor en mayúscula, retornando un string con la primera en maýsucula.
        /// </summary>
        /// <param name="s">String a procesar</param>
        /// <returns>String procesado.</returns>
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        private void CargarDetalleSolicitud(string IdPagoActividad)
        {
            //Inicializar variables.
            DataTable tabla = new DataTable();
            String txtSQL;

            txtSQL = " SELECT CodProyecto, NomProyecto, CantidadDinero, NomPagoConcepto, FechaInterventor " +
                     " FROM PagoActividad, Proyecto, PagoConcepto " +
                     " WHERE Id_PagoActividad = " + IdPagoActividad +
                     " AND CodProyecto = Id_Proyecto " +
                     " AND Id_PagoConcepto = CodPagoConcepto";

            tabla = consultas.ObtenerDataTable(txtSQL, "text");

            //Session["Tabla_Detalle"] = tabla;
            //Session["Temporal"] = "temporal";
            //detalleSolicitud.DataSource = tabla;
            //detalleSolicitud.DataBind();
            //TraerArchivosB(tabla);

            if (tabla.Rows.Count > 0)
            {
                NumeroSolicitud.Text = IdPagoActividad;
                NumeroProyecto.Text = tabla.Rows[0]["CodProyecto"].ToString();
                NombreProyecto.Text = tabla.Rows[0]["NomProyecto"].ToString();
                ValorSolicitud.Text = tabla.Rows[0]["CantidadDinero"].ToString();
                ConceptoSolicitud.Text = tabla.Rows[0]["NomPagoConcepto"].ToString();
                FechaSolicitud.Text = tabla.Rows[0]["FechaInterventor"].ToString();
            }

            txtSQL = "SELECT NomPagoActividadArchivo, URL FROM PagoActividadarchivo WHERE codPagoActividad = " + IdPagoActividad;
            tabla = consultas.ObtenerDataTable(txtSQL, "text");

            if (tabla.Rows.Count > 0)
            {
                ArchivosAdjuntos.Text = tabla.Rows[0]["NomPagoActividadArchivo"].ToString();

                try
                {
                    ///Concatenar a la URL lo siguiente:
                    ///http://www.fondoemprender.com/fonade/Documentos/
                    ///PERO el código fuente NO encuentra el archivo que contiene la URL (Ya que la columna URL
                    ///simplemente contiene la ruta desde "Documentos/...").
                    ///Al concatenarle lo anterior si encuentra el documento, pero el código fuente NO indica esto...
                    if (File.Exists(tabla.Rows[0]["URL"].ToString()))
                    {
                        ArchivosAdjuntos.NavigateUrl = tabla.Rows[0]["URL"].ToString();
                        ArchivosAdjuntos.Target = "_blank";
                    }
                    else
                    {
                        ArchivosAdjuntos.Text = tabla.Rows[0]["NomPagoActividadArchivo"].ToString();
                        ArchivosAdjuntos.ToolTip = "No existe el archivo";
                        ArchivosAdjuntos.NavigateUrl = "#";
                    }
                }
                catch { }
            }
            else { ArchivosAdjuntos.Text = "No hay archivos adjuntos"; }
        }
    }
}