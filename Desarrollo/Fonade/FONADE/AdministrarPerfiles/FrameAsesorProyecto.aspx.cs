#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>08 - 07 - 2014</Fecha>
// <Archivo>FrameAsesorProyecto.cs</Archivo>

#endregion

using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade
{
    public partial class FrameAsesorProyecto : Negocio.Base_Page
    {
        #region variables globales

        int CodInstitucion;
        int CodProyecto = 0;

        #endregion

        /// <summary>
        /// Diego Quiñonez
        /// 08 - 07 - 2014
        /// metodo de carga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //recoge el codigo de la institucion relaciona con el usuario de Session
            CodInstitucion = usuario.CodInstitucion;

            if (CodProyecto == 0)
            {
                gvrasesorlider.Visible = false;
                gvrasesores.Visible = false;
                lnkasignacionasesores.Visible = false;
                btnactualizar.Visible = false;
                lbltitulo.Text = "Para ver los Asesores de un plan de negocio, seleccione uno a la izquierda.";
            }
        }

        #region cargue datos

        /// <summary>
        /// Diego Quiñonez
        /// 08 - 07 - 2014
        /// lista todos los proyectos relacionados con la institucion
        /// y los dibuja en la grilla gv_proyectos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ldsproyectos_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var proyecto = (from p in consultas.Db.Proyectos
                            orderby p.Id_Proyecto
                            where p.Inactivo == false
                            && p.CodInstitucion == CodInstitucion
                            select new
                            {
                                p.Id_Proyecto,
                                p.NomProyecto
                            });

            e.Result = proyecto.ToList();
        }

        /// <summary>
        /// Diego Quiñonez
        /// 09 - 07 - 2014
        /// carga el asesor lider en la grilla gvrasesorlider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ldsasesorlider_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var asesor = (from c in consultas.Db.Contactos
                          from pc in consultas.Db.ProyectoContactos
                          from p in consultas.Db.Proyectos
                          where c.Id_Contacto == pc.CodContacto
                          && pc.FechaFin == null
                          && pc.CodRol == 1
                          && pc.CodProyecto == CodProyecto
                          && p.CodInstitucion == CodInstitucion

                          select new
                          {
                              Nombre = c.Nombres + " " + c.Apellidos,
                              c.Email
                          }).Distinct();

            e.Result = asesor.ToList();
        }

        /// <summary>
        /// Diego Quiñonez
        /// 09 - 07 - 2014
        /// carga los asesores relacionados al proyecto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ldsasesores_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var asesor = (from c in consultas.Db.Contactos
                          from pc in consultas.Db.ProyectoContactos
                          from p in consultas.Db.Proyectos
                          where c.Id_Contacto == pc.CodContacto
                          && pc.FechaFin == null
                          && pc.CodRol == 2
                          && pc.CodProyecto == CodProyecto
                          && p.CodInstitucion == CodInstitucion

                          select new
                          {
                              Nombre = c.Nombres + " " + c.Apellidos,
                              c.Email
                          }).Distinct();

            e.Result = asesor.ToList();
        }

        /// <summary>
        /// Diego Quiñonez
        /// 09 - 07 - 2014
        /// carga los asesores disponibles para asignar a un proyecto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ldsasesoresasignar_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var asesores = (from c in consultas.Db.Contactos
                            from gc in consultas.Db.GrupoContactos
                            where gc.CodGrupo == 5
                            && c.Id_Contacto == gc.CodContacto
                            && c.CodInstitucion == CodInstitucion
                            select new
                            {
                                c.Id_Contacto,
                                Nombre = c.Nombres + " " + c.Apellidos
                            }).Distinct(); ;

            e.Result = asesores.ToList();
        }

        #endregion

        #region eventos grilla proyectos

        /// <summary>
        /// Diego Quiñonez
        /// 08 - 07 - 2014
        /// genera el paginado de la grilla gv_proyectos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_proyectos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //obtiene el indice de la nueva pagina
            gv_proyectos.PageIndex = e.NewPageIndex;
        }

        /// <summary>
        /// Diego Quiñonez
        /// 08 - 07 - 2014
        /// agrega informacion en la creacion de la grilla proyectos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_proyectos_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //recoge el id del proyecto de cada fila de la grilla proyectos
                int idproyecto = Convert.ToInt32(gv_proyectos.DataKeys[e.Row.RowIndex].Value.ToString());

                var contactoLider = (from pc in consultas.Db.ProyectoContactos
                                     where pc.CodProyecto == idproyecto
                                     && pc.FechaFin == null
                                     && (pc.CodRol == Constantes.CONST_RolAsesorLider)
                                     select pc.CodContacto).FirstOrDefault();

                //si no hay un asesor lider asignado muestra un icono de admiracion que de aviso al usuario
                if (!(contactoLider > 0))
                {
                    e.Row.FindControl("imgadmiracion").Visible = true;
                }

                var contactos = (from pc in consultas.Db.ProyectoContactos
                                 where pc.CodProyecto == idproyecto
                                 && pc.FechaFin == null
                                 && (pc.CodRol == Constantes.CONST_RolAsesorLider || pc.CodRol == Constantes.CONST_RolAsesor)
                                 select pc.CodContacto).Count();

                //muestra cuantos asesores estan asignados a cada proyecto
                ((Label)e.Row.FindControl("lblcontactos")).Text = contactos.ToString() + " Asesores";
            }
            //siempre inicia en -1 un argumento fura de rango
            catch (ArgumentOutOfRangeException) { }
        }

        /// <summary>
        /// Diego Quiñonez
        /// 08 - 07 - 2014
        /// recoge el id del proyecto
        /// para cargar los asesores asociados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_proyectos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int parametro = Convert.ToInt32(e.CommandArgument.ToString());

            cargueAsesores(parametro);
        }

        #endregion

        #region eventos grilla gvrasignarasesores

        /// <summary>
        /// Diego Quiñonez
        /// 09 - 07 - 2014
        /// al momento de crear la grilla
        /// selecciona los asesores asignados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvrasignarasesores_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //recoge el id del proyecto de la grilla
                var id = Convert.ToInt32(gvrasignarasesores.DataKeys[e.Row.RowIndex].Value.ToString());

                //trae los asesores relacionados al proyecto 
                var rol = (from pc in consultas.Db.ProyectoContactos
                           where pc.CodContacto == id
                           && pc.CodProyecto == Convert.ToInt32(hdproyecto.Value)
                           && pc.FechaFin == null
                           select pc.CodRol).FirstOrDefault();

                //si es asesor el checked pasa a true
                if (rol == 1 || rol == 2)
                    ((CheckBox)e.Row.FindControl("cbxasesor")).Checked = true;
                else
                    ((CheckBox)e.Row.FindControl("cbxasesor")).Checked = false;

                //si es asesor lider el checked pasa a true
                if (rol == 1)
                    ((RadioButton)e.Row.FindControl("rbasesorlider")).Checked = true;
                else
                    ((RadioButton)e.Row.FindControl("rbasesorlider")).Checked = false;
            }
            //siempre inicia en -1 un argumento fura de rango
            catch (ArgumentOutOfRangeException) { }
            //se dispara al cambiar de proyecto
            catch (FormatException) { }
        }

        #endregion

        #region metodos generales

        private void cargueAsesores(int id_proyecto)
        {
            hdproyecto.Value = id_proyecto.ToString();

            CodProyecto = id_proyecto;
            infoProyecto(id_proyecto);
            gvrasesorlider.DataBind();
            gvrasesores.DataBind();

            gvrasesorlider.Visible = true;
            gvrasesores.Visible = true;
            lnkasignacionasesores.Visible = true;
            gvrasignarasesores.Visible = false;
            btnactualizar.Visible = false;
        }

        /// <summary>
        /// Diego Quiñonez
        /// 09 - 07 - 2014
        /// establece el titulo del proyecto
        /// </summary>
        /// <param name="id_proyecto"></param>
        private void infoProyecto(int id_proyecto)
        {
            //devuelve el estado y el nombre del proyecto de la BD de acuerdo al id del proyecto
            var proyecto = (from p in consultas.Db.Proyectos
                            where p.Id_Proyecto == id_proyecto
                            select new
                            {
                                p.NomProyecto,
                                p.CodEstado
                            }).FirstOrDefault();

            lbltitulo.Text = "Asesores para el Plan de Negocio " + proyecto.NomProyecto;
        }

        protected void lnkasignacionasesores_Click(object sender, EventArgs e)
        {
            gvrasesores.Visible = false;
            gvrasesorlider.Visible = false;
            gvrasignarasesores.Visible = true;
            btnactualizar.Visible = true;
            infoProyecto(Convert.ToInt32(hdproyecto.Value));
            gvrasignarasesores.DataBind();
        }

        /// <summary>
        /// Diego Quiñonez
        /// 09 - 07 - 2014
        /// solo es un lider pro proyecto
        /// selecciona solo el que a asignado el usuario
        /// y el resto le asigna como false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbasesorlider_CheckedChanged(object sender, EventArgs e)
        {
            //recorre las filas de la grilla
            foreach (GridViewRow gvr in gvrasignarasesores.Rows)
            {
                //busca los radiobutton para pasar el checked a flase
                ((RadioButton)gvr.FindControl("rbasesorlider")).Checked = false;
            }

            //el que selecciono el usuario lo asigna a true
            ((RadioButton)sender).Checked = true;
            //coloca informacion del proyecto
            infoProyecto(Convert.ToInt32(hdproyecto.Value));
            btnactualizar.Visible = true;
        }

        #endregion

        /// <summary>
        /// Diego Quiñonez
        /// 09 - 07 - 2014
        /// actualiza la lista de  asesores
        /// que se asignan a un proyecto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnactualizar_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow gvr in gvrasignarasesores.Rows)
            {
                var id = Convert.ToInt32(gvrasignarasesores.DataKeys[gvr.RowIndex].Value.ToString());

                CheckBox cb = (CheckBox)gvr.FindControl("cbxasesor");

                if (cb.Checked)
                {
                    #region checkeado, verifica, si es necesario inserta en proyectocontacto
                    try
                    {
                        var proyecto = (from pc in consultas.Db.ProyectoContactos
                                        where pc.FechaFin == null
                                        && (pc.CodRol == 1 || pc.CodRol == 2)
                                        && pc.CodProyecto == Convert.ToInt32(hdproyecto.Value)
                                        && pc.CodContacto == id
                                        select pc.Id_ProyectoContacto).FirstOrDefault();

                        if (proyecto == 0) { insertar(id); }
                    }
                    catch (InvalidOperationException) { insertar(id); }
                    #endregion

                    RadioButton rb = (RadioButton)gvr.FindControl("rbasesorlider");

                    #region checkeado lider, verifica, cambia de rol
                    try
                    {
                        var procontacto = (from p in consultas.Db.ProyectoContactos
                                           where p.FechaFin == null
                                           && p.CodRol == 1
                                           && p.CodProyecto == Convert.ToInt32(hdproyecto.Value)
                                           && p.CodContacto == id
                                           select p.Id_ProyectoContacto).First();

                        if (!rb.Checked)
                        {
                            if (procontacto > 0)
                            {
                                try
                                {
                                    Datos.ProyectoContacto pc1 = (from p in consultas.Db.ProyectoContactos
                                                                  where p.CodRol == 1
                                                                  && p.CodProyecto == Convert.ToInt32(hdproyecto.Value)
                                                                  && p.CodContacto == id
                                                                  select p).First();

                                    pc1.CodRol = 2;

                                    try
                                    {
                                        consultas.Db.SubmitChanges();
                                    }
                                    catch (Exception) { }
                                }
                                catch (InvalidOperationException) { }
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        if (rb.Checked)
                        {
                            try
                            {
                                Datos.ProyectoContacto pc1 = (from p in consultas.Db.ProyectoContactos
                                                              where p.FechaFin == null
                                                              && p.CodProyecto == Convert.ToInt32(hdproyecto.Value)
                                                              && p.CodContacto == id
                                                              select p).First();

                                pc1.CodRol = 1;

                                try
                                {
                                    consultas.Db.SubmitChanges();
                                }
                                catch (Exception) { }
                            }
                            catch (InvalidOperationException) { }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region no checkeado
                    try
                    {
                        var proyecto = (from pc in consultas.Db.ProyectoContactos
                                        where pc.FechaFin == null
                                        && (pc.CodRol == 1 || pc.CodRol == 2)
                                        && pc.CodProyecto == Convert.ToInt32(hdproyecto.Value)
                                        && pc.CodContacto == id
                                        select new
                                        {
                                            pc.Id_ProyectoContacto,
                                            pc.CodRol
                                        }).FirstOrDefault();

                        if (proyecto != null)
                        {
                            Datos.ProyectoContacto pc1 = (from p in consultas.Db.ProyectoContactos
                                                          where p.FechaFin == null
                                                          && p.CodRol == proyecto.CodRol
                                                          && p.CodProyecto == Convert.ToInt32(hdproyecto.Value)
                                                          && p.CodContacto == id
                                                          select p).First();

                            pc1.Inactivo = true;
                            pc1.FechaFin = DateTime.Now;

                            try
                            {
                                consultas.Db.SubmitChanges();
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (InvalidOperationException) { }
                    #endregion
                }
            }

            cargueAsesores(Convert.ToInt32(hdproyecto.Value));
        }

        /// <summary>
        /// Diego Quiñonez
        /// 18 - 07 - 2014
        /// guarda la relacion del usuario y proyecto en BD
        /// </summary>
        /// <param name="id"></param>
        private void insertar(int id)
        {
            Datos.ProyectoContacto pc = new Datos.ProyectoContacto();

            pc.CodProyecto = Convert.ToInt32(hdproyecto.Value);
            pc.CodContacto = id;
            pc.CodRol = 2;
            pc.FechaInicio = DateTime.Now;
            pc.Inactivo = false;

            consultas.Db.ProyectoContactos.InsertOnSubmit(pc);

            try
            {
                consultas.Db.SubmitChanges();
            }
            catch (Exception) { }
        }
    }
}