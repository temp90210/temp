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

namespace Fonade.FONADE.Convocatoria
{
    public partial class ProyectosXConvocatoria : Negocio.Base_Page
    {
        public const int PAGE_SIZE = 10;
        public int idConvocatoria;

        protected void Page_Load(object sender, EventArgs e)
        {
            idConvocatoria = Convert.ToInt32(Session["Id_ProyPorConvoct"]);
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("PROYECTOS CONVOCADOS");
                llenarInfoConvoct(idConvocatoria);
            }
        }

        protected void lds_listadoProyXConvoct_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {
                var query = from P in consultas.MostrarProyectosPorConvocatoria(idConvocatoria)
                            select P;
                e.Arguments.TotalRowCount = query.Count();

                query = query.Skip(GridViewConvoct.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);

                e.Result = query;
            }
            catch (Exception exc)
            { }

        }

        protected void GridViewConvoct_DataBound(object sender, EventArgs e)
        {
            
            try
            {
                foreach (GridViewRow grd_Row in this.GridViewConvoct.Rows)
                {
                    string nombresCompletos = "";
                    int cod_Proyecto =Convert.ToInt32(((HiddenField)grd_Row.FindControl("hiddenIdProy")).Value);
                    var query = from PC in consultas.Db.ProyectoContactos
                                from C in consultas.Db.Contactos
                                where C.Id_Contacto == PC.CodContacto
                                && PC.CodProyecto == cod_Proyecto
                                && PC.Inactivo == false
                                && PC.CodRol == Constantes.CONST_RolEmprendedor
                                select new
                                {
                                    nombres = "-" + C.Nombres + " " + C.Apellidos,
                                };
                    foreach (var nom in query)
                    {
                        nombresCompletos += nom.nombres;
                        nombresCompletos += Environment.NewLine;
                    }
                    ((Label)grd_Row.FindControl("hl_presentado")).Text = nombresCompletos;
                }
            }
            catch (Exception)
            {}
            
        }

        protected void llenarInfoConvoct(int idconvoct)
        {
            try
            {
                var query = (from convt in consultas.Db.Convocatorias
                             where convt.Id_Convocatoria == idconvoct
                             select new
                             {
                                 apertura = convt.Id_Convocatoria,
                                 nombre = convt.NomConvocatoria,
                                 descripcion = convt.Descripcion,
                                 fechainicio = convt.FechaInicio,
                                 l_fechafin = convt.FechaFin,
                                 presupuesto = convt.Presupuesto,
                                 valorminimo= convt.MinimoPorPlan,
                             }).FirstOrDefault();
                l_numeroapertura.Text = query.apertura.ToString("D6");
                l_descipcion.Text = query.descripcion;
                l_fechafin.Text = query.l_fechafin.ToString("dd 'de' MMMM 'de' yyyy h:mm tt");
                l_fechainicio.Text = query.fechainicio.ToString("dd 'de' MMMM 'de' yyyy");
                l_nombre.Text = query.nombre;
                l_presupuesto.Text = query.presupuesto.ToString("C");
                l_valorminimo.Text = query.valorminimo.ToString("C");
            }
            catch (Exception)
            {}
        }

        protected void GridViewConvoct_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString() == "ImprResumenProyecto")
            {
                string IdProyImp = e.CommandArgument.ToString();
                Session["Id_ProyImp"] = IdProyImp;

                Redirect(null, "VerImpresion.aspx", "_Blank", "width=1000,height=700");
            }
        }

        protected void btn_IraConvoct_Click(object sender, EventArgs e)
        {
            Session["IdConvocatoria"] = idConvocatoria;
            Response.Redirect("Convocatoria.aspx");
        }
    }


}