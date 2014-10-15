#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Archivo>BuscarProyecto.cs</Archivo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Administracion
{
    public partial class BuscarProyecto : Negocio.Base_Page
    {
        /// <summary>
        /// Diego Quiñonez
        /// metodo de carga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //agrega la propiedad de validacion al textbox
            //con el fin que solo deje ingresar numeros en el
            txtnoproyecto.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
        }

        /// <summary>
        /// Diego Quiñonez
        /// metodo asociado al boton buscar
        /// se encarga de buscar un proyecto de acuerdo al id o a alguna letra o palabra que este contenida dentro del nombre del proyecto
        /// en caso de no coincidir muestra toda la lista de proyectos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnbuscar_Click(object sender, EventArgs e)
        {
            //devuelove todos los proyectos de acuerdo a un estado
            var resul = from p in consultas.Db.Proyecto1s
                        where p.CodEstado == 7
                        select new
                        {
                            p.Id_Proyecto,
                            p.NomProyecto
                        };

            //si busca por nombre lo filtra
            if (!string.IsNullOrEmpty(txtProyecto.Text))
                resul = resul.Where(p => p.NomProyecto.Contains(txtProyecto.Text));

            //si busca por id lo filtra
            if (!string.IsNullOrEmpty(txtnoproyecto.Text))
                resul = resul.Where(p=>p.Id_Proyecto==Convert.ToInt32(txtnoproyecto.Text));

            gvproyectos.DataSource = resul;
            gvproyectos.DataBind();
        }

        /// <summary>
        /// Diego Quiñonez
        /// evento asociado a la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvproyectos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //recoge los parametros de la fila de la grilla desde la que se alla disparado el evento
            string []param = e.CommandArgument.ToString().Split(';');

            //guarda en session los parametros para ser utilizados
            //en la pagina de llamada
            Session["Id_ProyectoEval"] = param[0];
            Session["NomproyectoEval"] = param[1];

            //crea un objeto de tipo script
            ClientScriptManager cm = this.ClientScript;
            //recaga y cierra la ventana emergente
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        }
    }
}