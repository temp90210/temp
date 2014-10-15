using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Caching;

namespace Fonade.FONADE.Proyecto
{
    public partial class Proyecto : Negocio.Base_Master
    {
        public string codProyecto;
        public string codConvocatoria;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Código anterior comentado.
            //if (Request.QueryString["codProyecto"] != null && Request.QueryString["codProyecto"] != "")
            //{
            //    codProyecto = Request.QueryString["codProyecto"].ToString();
            //}
            //else
            //{
            //    //Mauricio Arias Olave. "14/05/2014": Se comenta esta línea, ya que NO existe la página "Home.aspx".
            //    //Response.Redirect("~/Home.aspx");
            //    Session.Abandon();//Clear();
            //    Response.Redirect("~/Account/Login.aspx");
            //}

            ////Valida si viene por ueryString el id de convocatoria
            //if (Request.QueryString["codConvocatoria"] != null && Request.QueryString["codConvocatoria"] != "")
            //{
            //    codConvocatoria = Request.QueryString["codConvocatoria"].ToString();
            //}

            //if (codConvocatoria != null && codConvocatoria != "")
            //{
            //    string convocatoria = consultas.Db.Convocatorias.FirstOrDefault(t => t.Id_Convocatoria == Convert.ToInt32(codConvocatoria)).NomConvocatoria;
            //    if (!string.IsNullOrEmpty(convocatoria))
            //    {
            //        lbl_convocatoria.Text = convocatoria + " - ";
            //    }
            //}

            //codProyecto = Request.QueryString["codProyecto"].ToString();
            //var query = (from p in consultas.Db.Proyectos
            //             from i in consultas.Db.Institucions
            //             from s in consultas.Db.SubSectors
            //             from c in consultas.Db.Ciudads
            //             from d in consultas.Db.departamentos
            //             where p.Id_Proyecto == Convert.ToInt32(codProyecto)
            //                 && s.Id_SubSector == p.CodSubSector
            //                 && i.Id_Institucion == p.CodInstitucion
            //                 && p.CodCiudad == c.Id_Ciudad
            //                 && c.CodDepartamento == d.Id_Departamento
            //             select new
            //             {
            //                 p.Id_Proyecto,
            //                 p.NomProyecto,
            //                 s.NomSubSector,
            //                 i.NomUnidad,
            //                 c.NomCiudad,
            //                 d.NomDepartamento,
            //                 d.Id_Departamento,
            //                 i.NomInstitucion,
            //                 p.CodEstado
            //             });

            //foreach (var obj in query)
            //{
            //    lbl_title.Text = obj.Id_Proyecto
            //        + " - " + obj.NomProyecto
            //        + " - " + obj.NomUnidad
            //        + " (" + obj.NomInstitucion + ")";
            //    img_lt.Src = "~/Images/ImgLT" + obj.CodEstado + ".jpg";
            //    img_map.Src = "~/Images/Mapas/" + remplazarTilde(obj.NomDepartamento) + "Pq.gif";
            //    img_map.Alt = obj.NomCiudad + "(" + obj.NomDepartamento + ")";
            //    link_map.HRef = "~/Mapas/Mapas.aspx?ver=1&pc=" + obj.Id_Departamento + "&pid=" + obj.Id_Proyecto;
            //    lbl_convocatoria.Text = lbl_convocatoria.Text + obj.NomSubSector;
            //    break;
            //} 
            #endregion

            #region Nueva versión.

            codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";


            if (codProyecto == "0")
            {
                //Mauricio Arias Olave. "14/05/2014": Se comenta esta línea, ya que NO existe la página "Home.aspx".
                //Response.Redirect("~/Home.aspx");
                Session.Abandon();//Clear();
                Response.Redirect("~/Account/Login.aspx");
            }

            ///También se debe validar la variable "codConvocatoria", pero toca ver de dónde la envía
            ///PARA HACER ESTO!!!
            //if (codConvocatoria != null && codConvocatoria != "")
            //{
            //    string convocatoria = consultas.Db.Convocatorias.FirstOrDefault(t => t.Id_Convocatoria == Convert.ToInt32(codConvocatoria)).NomConvocatoria;
            //    if (!string.IsNullOrEmpty(convocatoria))
            //    {
            //        lbl_convocatoria.Text = convocatoria + " - ";
            //    }
            //}

            var query = (from p in consultas.Db.Proyectos
                         from i in consultas.Db.Institucions
                         from s in consultas.Db.SubSectors
                         from c in consultas.Db.Ciudads
                         from d in consultas.Db.departamentos
                         where p.Id_Proyecto == Convert.ToInt32(codProyecto)
                             && s.Id_SubSector == p.CodSubSector
                             && i.Id_Institucion == p.CodInstitucion
                             && p.CodCiudad == c.Id_Ciudad
                             && c.CodDepartamento == d.Id_Departamento
                         select new
                         {
                             p.Id_Proyecto,
                             p.NomProyecto,
                             s.NomSubSector,
                             i.NomUnidad,
                             c.NomCiudad,
                             d.NomDepartamento,
                             d.Id_Departamento,
                             i.NomInstitucion,
                             p.CodEstado
                         });

            foreach (var obj in query)
            {
                lbl_title.Text = obj.Id_Proyecto
                    + " - " + obj.NomProyecto
                    + " - " + obj.NomUnidad
                    + " (" + obj.NomInstitucion + ")";
                img_lt.Src = "~/Images/ImgLT" + obj.CodEstado + ".jpg";
                img_map.Src = "~/Images/Mapas/" + remplazarTilde(obj.NomDepartamento) + "Pq.gif";
                img_map.Alt = obj.NomCiudad + "(" + obj.NomDepartamento + ")";
                link_map.HRef = "~/Mapas/Mapas.aspx?ver=1&pc=" + obj.Id_Departamento + "&pid=" + obj.Id_Proyecto;
                lbl_convocatoria.Text = lbl_convocatoria.Text + obj.NomSubSector;
                break;
            }

            #endregion
        }

        private string remplazarTilde(string texto)
        {
            string result = texto.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u");
            return result;
        }

        protected void LoginStatus_LoggedOut(Object sender, System.EventArgs e)
        {
            MemoryCache.Default.Dispose();
            Session.Abandon();
        }
    }
}