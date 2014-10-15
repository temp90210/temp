#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>03 - 03 - 2014</Fecha>
// <Archivo>Proyectos.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Clases;
using Fonade.Negocio;
using System.Data.SqlClient;
using System.Configuration;

#endregion

namespace Fonade.Fonade.Proyecto
{
    public partial class Proyectos : Base_Page
    {
        public const int PAGE_SIZE = 20;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void lds_proyectos_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            // LINQ query

            var query = from p in consultas.Db.Proyectos
                        from c in consultas.Db.Ciudads
                        from d in consultas.Db.departamentos
                        from i in consultas.Db.Institucions
                        where c.Id_Ciudad == p.CodCiudad
                              & c.CodDepartamento == d.Id_Departamento
                              & p.CodInstitucion == i.Id_Institucion
                        select new
                                   {
                                       IdProyecto = p.Id_Proyecto,
                                       NombreProyecto = p.NomProyecto,
                                       CodigoInstitucion = i.Id_Institucion,
                                       CodigoEstado = p.CodEstado,
                                       NombreUnidad = i.NomUnidad,
                                       NombreInstitucion = i.NomInstitucion,
                                       NombreCiudad = c.NomCiudad,
                                       NombreDepartamento = d.NomDepartamento,
                                       p.Inactivo
                                   };

            switch (usuario.CodGrupo)
            {
                case Constantes.CONST_AdministradorFonade:
                case Constantes.CONST_AdministradorSena:
                    query = query.Where(p => p.Inactivo == false);
                    break;
                case Constantes.CONST_JefeUnidad:
                    query = query.Where(p => p.CodigoInstitucion == usuario.CodInstitucion);
                    break;
                case Constantes.CONST_Asesor:
                case Constantes.CONST_Emprendedor:
                    query =
                        query.Where(
                            v => (consultas.Db.ProyectoContactos.Where(p => p.Proyecto.Id_Proyecto == p.CodProyecto
                                                                            && p.CodContacto == usuario.IdContacto
                                                                            && p.Inactivo == false).Select(
                                                                                t => t.CodProyecto)).Contains(
                                                                                    v.IdProyecto)
                                 && v.Inactivo == false
                                 && v.CodigoInstitucion == usuario.CodInstitucion);
                    break;
                case Constantes.CONST_Evaluador:
                case Constantes.CONST_CoordinadorEvaluador:

                    query =
                        query.Where(
                            v => (consultas.Db.ProyectoContactos.Where(p => p.Proyecto.Id_Proyecto == p.CodProyecto
                                                                            && p.CodContacto == usuario.IdContacto
                                                                            && p.Inactivo == false).Select(
                                                                                t => t.CodProyecto)).Contains(
                                                                                    v.IdProyecto)
                                 && v.Inactivo == false
                                 && v.CodigoEstado == Constantes.CONST_Evaluacion);
                    break;
                case Constantes.CONST_GerenteEvaluador:
                    query = query.Where(p => p.Inactivo == false
                                             && (p.CodigoEstado == Constantes.CONST_Convocatoria
                                                 || p.CodigoEstado == Constantes.CONST_Evaluacion));
                    break;
                default:
                    break;
            }

            switch (gw_proyectos.SortExpression)
            {
                case "NombreCiudad":
                    if (gw_proyectos.SortDirection == SortDirection.Ascending)
                        query = query.OrderBy(t => t.NombreCiudad);
                    else
                        query = query.OrderByDescending(t => t.NombreCiudad);
                    break;
                case "NombreProyecto":
                    if (gw_proyectos.SortDirection == SortDirection.Ascending)
                        query = query.OrderBy(t => t.NombreProyecto);
                    else
                        query = query.OrderByDescending(t => t.NombreProyecto);
                    break;
            }

            if (!string.IsNullOrEmpty(ddlbuscar.SelectedValue))
            {
                query = query.Where(t => t.NombreProyecto.StartsWith(ddlbuscar.SelectedValue));
            }

            // Do advanced query logic here (dynamically build WHERE clause, etc.)     

            // Set the total count     
            // so GridView knows how many pages to create    
            e.Arguments.TotalRowCount = query.Count();

            // Get only the rows we need for the page requested
            query = query.Skip(gw_proyectos.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);

            e.Result = query.OrderByDescending(q => q.IdProyecto);
        }

        protected void gw_proyectos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                {
                    if (Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "CodigoEstado")) == Constantes.CONST_Inscripcion)
                    {
                        if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Inactivo")) == false)
                        {
                            (e.Row.Cells[0].FindControl("ibtn_Inactivar")).Visible = true;
                        }
                        else
                        {
                            (e.Row.Cells[0].FindControl("ibtn_Activar")).Visible = true;
                        }
                    }
                }

                if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador ||
                    usuario.CodGrupo == Constantes.CONST_Evaluador
                    || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
                {
                    var query = (from cp in consultas.Db.ConvocatoriaProyectos
                                 from c in consultas.Db.Convocatorias
                                 where c.Id_Convocatoria == cp.CodConvocatoria
                                       &&
                                       cp.CodProyecto == Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "IdProyecto"))
                                 select new { cp.CodConvocatoria, c.NomConvocatoria, cp.Fecha }).OrderByDescending(
                                     t => t.Fecha).Take(1);

                    //Make an adition
                    ((LinkButton)e.Row.Cells[0].FindControl("hl_evaluacion")).CommandArgument =
                        string.Format("{0},{1}", DataBinder.Eval(e.Row.DataItem, "IdProyecto"), query.First().CodConvocatoria);


                    ((LinkButton)e.Row.Cells[0].FindControl("hl_evaluacion_hp")).CommandArgument =
                        string.Format("{0},{1}", DataBinder.Eval(e.Row.DataItem, "IdProyecto"), query.First().CodConvocatoria);


                    var lblnombreC = e.Row.Cells[0].FindControl("lbconvocatoria") as Label;
                    var lblevaluador = e.Row.Cells[0].FindControl("levaluador_") as Label;

                    lblnombreC.Text = query.First().NomConvocatoria;
                    int codproyect = (int)DataBinder.Eval(e.Row.DataItem, "IdProyecto");
                    lblevaluador.Text = CargarEvaluadores(codproyect);
                    var lblavalado = e.Row.Cells[0].FindControl("lblavalado") as Label;
                    if (CargarAvalado(codproyect) != 0)
                    {
                        lblavalado.Text = "<img src='../../Images/chulo.gif' />";
                    }
                    else
                    {
                        lblavalado.Text = string.Empty;
                    }
                }


                //Unidad Reasignar
                if (usuario.CodGrupo == Constantes.CONST_AdministradorFonade ||
                    usuario.CodGrupo == Constantes.CONST_AdministradorSena)
                {
                    if (usuario.CodInstitucion != Constantes.CONST_UnidadTemporal)
                    {
                        ((LinkButton)e.Row.Cells[0].FindControl("lbtn_reasignar")).Enabled = false;
                    }
                }

                //inactivo
                if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                {
                    if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "Inactivo")) == false)
                    {
                        LinkButton lkb = ((LinkButton)e.Row.Cells[0].FindControl("lbtn_inactivo"));
                        lkb.Text = "Activo";
                        lkb.Enabled = false;
                    }
                }

                //
            }
        }

        public string CargarEvaluadores(int codproyecto)
        {
            string evaluador = string.Empty;
            if (codproyecto != 0)
            {
                var query = consultas.Db.Contactos.Join(consultas.Db.ProyectoContactos,
                                                        (c => c.Id_Contacto),
                                                        (p => p.CodContacto),
                                                        (c, p) => new
                                                                      {
                                                                          Evaluador = c.Nombres + " " + c.Apellidos,
                                                                          PInactivo = p.Inactivo,
                                                                          CInactivo = c.Inactivo,
                                                                          Codproyect = p.CodProyecto,
                                                                          Rols = p.CodRol
                                                                      }).Where(
                                                                          r =>
                                                                          r.PInactivo == false &&
                                                                          r.Rols == Constantes.CONST_RolEvaluador &&
                                                                          r.Codproyect == codproyecto &&
                                                                          r.CInactivo == false);


                if (query.Any())
                {
                    evaluador = Enumerable.Aggregate(query, evaluador, (valor, items) => valor + items.Evaluador);
                }
            }


            return evaluador;
        }

        public int CargarAvalado(int codproyecto)
        {
            int proyecto = 0;
            if (codproyecto != 0)
            {
                proyecto = consultas.ObtenerTabs(codproyecto);
            }

            return proyecto;
        }

        protected void gw_proyectos_DataBound(object sender, EventArgs e)
        {
            //Iterar según el rol del usuario en sesión.
            switch (usuario.CodGrupo)
            {
                case Constantes.CONST_GerenteEvaluador:

                    gw_proyectos.Columns[2].Visible = false; //evaluar
                    gw_proyectos.Columns[3].Visible = false; //ciudad
                    gw_proyectos.Columns[4].Visible = false; //unidad
                    gw_proyectos.Columns[5].Visible = false; //estado
                    gw_proyectos.Columns[6].Visible = false; // evaluador
                    gw_proyectos.Columns[7].Visible = false; //avalado
                    gw_proyectos.Columns[8].Visible = false; //evaluar

                    break;

                case Constantes.CONST_CoordinadorEvaluador:

                    gw_proyectos.Columns[2].Visible = false; //evaluar
                    gw_proyectos.Columns[4].Visible = false; //unidad
                    gw_proyectos.Columns[5].Visible = false; //estado

                    break;
                case Constantes.CONST_Evaluador:

                    gw_proyectos.Columns[2].Visible = false; // ciudad
                    gw_proyectos.Columns[4].Visible = false; // evaluador
                    gw_proyectos.Columns[5].Visible = false; //avalado
                    gw_proyectos.Columns[6].Visible = false; //evaluar
                    gw_proyectos.Columns[7].Visible = false; //evaluar
                    break;


                case Constantes.CONST_JefeUnidad:
                    gw_proyectos.Columns[2].Visible = false; // evaluador
                    gw_proyectos.Columns[4].Visible = false; //avalado
                    gw_proyectos.Columns[6].Visible = false; // evaluador
                    gw_proyectos.Columns[7].Visible = false; //avalado
                    gw_proyectos.Columns[8].Visible = false; //avalado
                    gw_proyectos.Columns[9].Visible = false; //evaluar
                    break;

                case Constantes.CONST_Asesor:

                    gw_proyectos.Columns[2].Visible = false; // evaluador
                    gw_proyectos.Columns[4].Visible = false; //avalado
                    gw_proyectos.Columns[5].Visible = false; // evaluador
                    gw_proyectos.Columns[6].Visible = false; // evaluador
                    gw_proyectos.Columns[7].Visible = false; //avalado
                    gw_proyectos.Columns[8].Visible = false; //avalado
                    gw_proyectos.Columns[9].Visible = false; //evaluar

                    break;
                case Constantes.CONST_AdministradorFonade:
                    gw_proyectos.Columns[2].Visible = false; // evaluador
                    gw_proyectos.Columns[5].Visible = false;
                    gw_proyectos.Columns[6].Visible = false; // evaluador
                    gw_proyectos.Columns[7].Visible = false; //avalado
                    gw_proyectos.Columns[8].Visible = false; //avalado
                    gw_proyectos.Columns[9].Visible = false; //avalado
                    break;

                case Constantes.CONST_CallCenter:
                    gw_proyectos.Columns[2].Visible = false; //evaluar
                    gw_proyectos.Columns[3].Visible = true; //ciudad
                    //Ocultar el resto de las columnas.
                    gw_proyectos.Columns[4].Visible = false; //unidad
                    gw_proyectos.Columns[5].Visible = false; //estado
                    gw_proyectos.Columns[6].Visible = false; // evaluador
                    gw_proyectos.Columns[7].Visible = false; //avalado
                    gw_proyectos.Columns[8].Visible = false; //evaluar
                    gw_proyectos.Columns[9].Visible = false; //avalado
                    break;
                default:
                    gw_proyectos.Columns[5].Visible = false;
                    gw_proyectos.Columns[6].Visible = false; // evaluador
                    gw_proyectos.Columns[7].Visible = false; //avalado
                    gw_proyectos.Columns[8].Visible = false; //avalado
                    gw_proyectos.Columns[9].Visible = false; //avalado
                    break;
            }
        }

        protected void gw_proyectos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var IdProyecto = e.CommandArgument.ToString();
            string[] variables = IdProyecto.Split(',');

            switch (e.CommandName)
            {
                case "Evaluacion":
                    Session["CodProyecto"] = variables[0];
                    Session["CodConvocatoria"] = variables[1];
                    Response.Redirect("../evaluacion/EvaluacionFrameSet.aspx");
                    break;
                case "InActivar":
                    CargarFormularioInActivarProyecto(variables[0]);
                    break;
                case "Activar":
                    ActivarProyecto(variables[0]);
                    break;
                case "Evaluacions":
                    Session["CodProyecto"] = variables[0];
                    Session["CodConvocatoria"] = 0;
                    Response.Redirect("../evaluacion/EvaluacionFrameSet.aspx");
                    break;
                case "Frameset":
                    Session["CodProyecto"] = e.CommandArgument;
                    Response.Redirect("ProyectoFrameSet.aspx");
                    break;
                case "ventanaInactivo":
                    LinkButton lnkBtn = e.CommandSource as LinkButton;

                    if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                    {
                        if (lnkBtn.Text == "Inactivo")
                        {
                            Session["CodProyecto"] = lnkBtn.CommandArgument;
                            Redirect(null, "InactivarProyecto.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100");
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        protected void ActivarProyecto(string IdProyecto)
        {
            var query = (from p in consultas.Db.Proyectos
                         where p.Id_Proyecto == Convert.ToInt32(IdProyecto) &&
                               p.CodInstitucion == usuario.CodInstitucion
                         select p).First();
            query.Inactivo = false;
            query.MotivoDesactivacion = null;
            consultas.Db.SubmitChanges();

            var queryNombre = (from p in consultas.Db.Proyectos
                               where p.Id_Proyecto == Convert.ToInt32(IdProyecto)
                               select new { p.NomProyecto }).First();
            string nombreProyecto = queryNombre.NomProyecto;
            AgendarTarea agenda = new AgendarTarea(usuario.IdContacto, "Asignar Asesor",
                                                   "Asignar Asesor a el proyecto " + nombreProyecto, IdProyecto, 3, "0",
                                                   false,
                                                   1, true, false, usuario.IdContacto, "CodProyecto=" + IdProyecto, "",
                                                   "Asignar Asesor");
            agenda.Agendar();
        }

        protected void CargarFormularioInActivarProyecto(string IdProyecto)
        {
            pnlPrincipal.Visible = false;
            pnlInActivar.Visible = true;
            btnInActivar.Visible = true;
            hddIdProyecto.Value = IdProyecto;

            var query = (from p in consultas.Db.Proyectos
                         where p.Id_Proyecto == Convert.ToInt32(IdProyecto)
                         select new { p.MotivoDesactivacion, p.Inactivo, p.NomProyecto }).First();
            txtMotivoInactivacion.Text = query.MotivoDesactivacion;
            lblTitulo.Text = "MOTIVO INACTIVACION";
            if (query.Inactivo == false)
            {
                btnInActivar.Visible = true;
                lblTitulo.Text = "INACTIVAR PROYECTO " + query.NomProyecto.ToUpper() + "";
            }
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = true;
            pnlInActivar.Visible = false;
            hddIdProyecto.Value = "";
            txtMotivoInactivacion.Text = "";
        }

        protected void btnInActivar_Click(object sender, EventArgs e)
        {
            string idProyecto = hddIdProyecto.Value;

            if (String.IsNullOrEmpty(txtMotivoInactivacion.Text))
            {
                System.Windows.Forms.MessageBox.Show("El Motivo de Inactivación no debe tener mas de 300 caracteres", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            try
            {
                //Inactivar proyecto
                consultas.Db.ExecuteCommand(
                    "update proyecto set fechadesactivacion=getdate(), MotivoDesactivacion={0}, inactivo=1 where id_proyecto={1}",
                    txtMotivoInactivacion.Text, Convert.ToInt32(idProyecto));

                //Inactivar usuarios emprendedores del proyecto  
                string modifica = "update contacto set inactivo=1 where id_contacto in ";
                modifica += " (select p.codcontacto from proyectocontacto p, grupocontacto g ";
                modifica += " where p.codcontacto=g.codcontacto and codgrupo={0}";
                modifica += " and codproyecto={1} and inactivo=0) ";
                consultas.Db.ExecuteCommand(modifica, Constantes.CONST_Emprendedor, Convert.ToInt32(idProyecto));

                //Inactivar usuarios emprendedores. Los asesores no se inactivan porque pueden tener otros proyectos
                string elimina = "delete from grupocontacto where codgrupo={0} and codcontacto in ";
                elimina += "(select codcontacto from proyectocontacto where codproyecto={1} and inactivo=0)";
                consultas.Db.ExecuteCommand(elimina, Constantes.CONST_Emprendedor, Convert.ToInt32(idProyecto));

                //Inactivar usuarios dentro del proyecto
                consultas.Db.ExecuteCommand(
                    "update proyectocontacto set inactivo=1, fechafin=getdate() where inactivo=0 and codproyecto={1}",
                    Convert.ToInt32(idProyecto));

                //Cerrar Tareas Pendientes relacionadas con el proyecto
                string modificaTarea =
                    "update tareausuariorepeticion set respuesta = 'Cerrada por Inactivacion proyecto', fechacierre=getdate() where codtareausuario in ";
                modificaTarea += "(select id_tareausuario from tareausuario where codproyecto={0}) and fechacierre is null";
                consultas.Db.ExecuteCommand(modificaTarea, Convert.ToInt32(idProyecto));

                pnlPrincipal.Visible = true;
                pnlInActivar.Visible = false;
                hddIdProyecto.Value = "";
                txtMotivoInactivacion.Text = "";
            }
            catch (FormatException) { }
            catch (Exception) { }
            gw_proyectos.DataBind();
            Response.Redirect("Proyectos.aspx");
        }

        protected void ibtn_Activar_Click(object sender, ImageClickEventArgs e)
        {

            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GVProyecto = gw_proyectos.Rows[indicefila];
            Int64 CodProyecto = Int64.Parse(gw_proyectos.DataKeys[GVProyecto.RowIndex].Value.ToString());

            ActivarEIncativar(CodProyecto);
        }

        protected void lbtn_inactivo_Click(object sender, EventArgs e)
        {

            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GVProyecto = gw_proyectos.Rows[indicefila];
            Int64 CodProyecto = Int64.Parse(gw_proyectos.DataKeys[GVProyecto.RowIndex].Value.ToString());

            ActivarEIncativar(CodProyecto);
        }

        private void ActivarEIncativar(Int64 CodProyecto)
        {
            String txtSQL = "update proyecto set inactivo=0, motivodesactivacion=NULL where id_proyecto=" + CodProyecto + " and codinstitucion=" + usuario.CodInstitucion;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(txtSQL, conn);
            try
            {

                conn.Open();
                cmd.ExecuteReader();
                conn.Close();

                ActivarProyecto("" + CodProyecto);

                gw_proyectos.DataBind();
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }
        }

        protected void ddlbuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            gw_proyectos.DataBind();
        }
    }
}