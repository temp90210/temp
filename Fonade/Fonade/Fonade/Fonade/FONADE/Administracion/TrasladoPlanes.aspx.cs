﻿#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Archivo>TrasladoPlanes.cs</Archivo>

#endregion

using Datos;
using Fonade.Clases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Fonade.FONADE.Administracion
{
    public partial class TrasladoPlanes : Negocio.Base_Page
    {
        #region variables globales

        string txtSQL;

        string Id_ciudadEval;
        string id_ProyectoEval;

        #endregion

        /// <summary>
        /// Diego Quiñonez
        /// metodo de carga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //recoge datos de session
            Id_ciudadEval = Session["Id_ciudadEval"] != null && !string.IsNullOrEmpty(Session["Id_ciudadEval"].ToString()) ? Session["Id_ciudadEval"].ToString() : "0";
            id_ProyectoEval = Session["id_ProyectoEval"] != null && !string.IsNullOrEmpty(Session["id_ProyectoEval"].ToString()) ? Session["id_ProyectoEval"].ToString() : "0";

            //valida la session
            if (Id_ciudadEval.Equals("0") && id_ProyectoEval.Equals("0"))
            {
                pnlprincipal.Visible = true;
                pnlprincipal.Enabled = true;

                pnltransaldar.Visible = false;
                pnltransaldar.Enabled = false;
            }
        }

        /// <summary>
        /// Diego Quiñonez
        /// busca los planes de negocio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnbuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnombrepalnnegico.Text) && string.IsNullOrEmpty(txtnumeropannegocio.Text)) { }
            else
            {
                var result = from p in consultas.Db.MD_ListarPlanesDenegocio()
                             select new
                             {
                                 p.Id_ciudad,
                                 p.Id_Proyecto,
                                 p.NomProyecto,
                                 p.NomUnidad
                             };

                if (!string.IsNullOrEmpty(txtnombrepalnnegico.Text))
                    result = result.Where(p => p.NomProyecto.Contains(txtnombrepalnnegico.Text));
                if (!string.IsNullOrEmpty(txtnumeropannegocio.Text))
                    result = result.Where(p => p.Id_Proyecto == Convert.ToInt32(txtnumeropannegocio.Text));

                gv_listarpannegocio.DataSource = result;
                gv_listarpannegocio.DataBind();
            }

            pnlprincipal.Visible = true;
            pnlprincipal.Enabled = true;

            pnltransaldar.Visible = false;
            pnltransaldar.Enabled = false;

        }

        /// <summary>
        /// Diego Quiñonez
        /// evento disparado por la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_listarpannegocio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] param = e.CommandArgument.ToString().Split(';');

            Session["Id_ciudadEval"] = param[0];
            Session["id_ProyectoEval"] = param[1];

            Id_ciudadEval = Session["Id_ciudadEval"] != null && !string.IsNullOrEmpty(Session["Id_ciudadEval"].ToString()) ? Session["Id_ciudadEval"].ToString() : "0";
            id_ProyectoEval = Session["id_ProyectoEval"] != null && !string.IsNullOrEmpty(Session["id_ProyectoEval"].ToString()) ? Session["id_ProyectoEval"].ToString() : "0";

            pnlprincipal.Visible = false;
            pnlprincipal.Enabled = false;

            pnltransaldar.Visible = true;
            pnltransaldar.Enabled = true;

            caragarInfoTranslado();
        }

        /// <summary>
        /// Diego QUiñonez
        /// carga la informacion de translado
        /// </summary>
        private void caragarInfoTranslado()
        {
            consultas.Parameters = new[] { new SqlParameter { ParameterName = "@CodProyecto", Value = id_ProyectoEval } };
            var dt = consultas.ObtenerDataTable("MD_Translado_Planes");

            if (dt.Rows.Count > 0)
            {
                txtNombreUnidad.Text = dt.Rows[0]["NomUnidad"].ToString() + " (" + dt.Rows[0]["NomInstitucion"].ToString() + ")";
                txtNombreDepto.Text = dt.Rows[0]["NomDepartamento"].ToString();
                txtNombreCiudad.Text = dt.Rows[0]["NomCiudad"].ToString();
                txtNombreSector.Text = dt.Rows[0]["NomSector"].ToString();
                txtNombreSubsector.Text = dt.Rows[0]["NomSubSector"].ToString();
                txtIdSubsector.Text = dt.Rows[0]["Id_Institucion"].ToString();
                ddldepartament.SelectedValue = dt.Rows[0]["Id_Departamento"].ToString();//NomDepartamento
                lblnombreproyecto.Text = " " + dt.Rows[0]["NomProyecto"].ToString();

                var institucion = from i in consultas.Db.Institucions
                                  select new
                                  {
                                      i.Id_Institucion,
                                      nombre = i.NomUnidad + " (" + i.NomInstitucion + ")"
                                  };

                ddlnombreunidad.DataSource = institucion;
                ddlnombreunidad.DataTextField = "nombre";
                ddlnombreunidad.DataValueField = "Id_Institucion";
                ddlnombreunidad.DataBind();

                try { ddlnombreunidad.SelectedValue = dt.Rows[0]["Id_Institucion"].ToString(); }
                catch { }

                #region Comentarios.
                //var departamento = from d in consultas.Db.departamentos

                //                   orderby d.NomDepartamento
                //                   select new
                //                   {
                //                       d.Id_Departamento,
                //                       d.NomDepartamento
                //                   };

                /*          from p in consultas.Db.Proyecto1s
                               from i in consultas.Db.Institucions
                               from c in consultas.Db.Ciudads
                               from d in consultas.Db.departamentos
                               from sb in consultas.Db.SubSectors
                               from se in consultas.Db.Sectors
                               where p.CodInstitucion == i.Id_Institucion & p.CodCiudad == c.Id_Ciudad & c.CodDepartamento==d.Id_Departamento
                               && sb.Id_SubSector == p.CodSubSector & se.Id_Sector== sb.CodSector 
                                select new
                          {
                             d.NomDepartamento,
                             d.Id_Departamento
                          };*/


                /*var departamento = from d in consultas.Db.departamentos

                                   orderby d.NomDepartamento
                                   select new
                                   {
                                       d.Id_Departamento,
                                       d.NomDepartamento
                                   };*/

                #endregion

                #region Cargar departamentos y seleccionar el departamento.

                txtSQL = " SELECT Id_Departamento, NomDepartamento FROM Departamento ";
                var tabla = consultas.ObtenerDataTable(txtSQL, "text");
                ddldepartament.Items.Clear();
                foreach (DataRow row in tabla.Rows)
                {
                    ListItem item = new ListItem();
                    item.Text = row["NomDepartamento"].ToString();
                    item.Value = row["Id_Departamento"].ToString();
                    ddldepartament.Items.Add(item);
                }
                tabla = null;

                //Establecer el departamento seleccionado.
                try { ddldepartament.SelectedValue = dt.Rows[0]["Id_Departamento"].ToString(); }
                catch { }

                #endregion

                #region Comentarios.
                //ddldepartament.DataSource = departamento;
                //ddldepartament.DataTextField = "NomDepartamento";
                //ddldepartament.DataValueField = "Id_Departamento";
                //ddldepartament.DataBind(); 
                #endregion

                //Llenar ciudad...
                llenarddlciudad(Convert.ToInt32(dt.Rows[0]["Id_Departamento"].ToString()));

                try { ddlciudad.SelectedValue = dt.Rows[0]["Id_Ciudad"].ToString(); }
                catch { }

                #region Sectores...

                var sector = from s in consultas.Db.Sectors
                             orderby s.NomSector
                             select new
                             {
                                 s.Id_Sector,
                                 s.NomSector
                             };

                ddlsector.DataSource = sector;
                ddlsector.DataTextField = "NomSector";
                ddlsector.DataValueField = "Id_Sector";
                ddlsector.DataBind();

                try { ddlsector.SelectedValue = dt.Rows[0]["Id_Sector"].ToString(); }
                catch { }

                llenarsubsector(Convert.ToInt32(dt.Rows[0]["Id_Sector"].ToString()));

                try { ddlsubsector.SelectedValue = dt.Rows[0]["Id_Subsector"].ToString(); }
                catch { }

                #endregion
            }
        }

        protected void ddldepartament_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarddlciudad(Convert.ToInt32(ddldepartament.SelectedValue));
        }

        /// <summary>
        /// Diego QUiñonez
        /// cargue de ciudades en el dropdownlist
        /// </summary>
        /// <param name="coddepartamento"></param>
        private void llenarddlciudad(Int32 coddepartamento)
        {
            var ciudad = from c in consultas.Db.Ciudads
                         where c.CodDepartamento == coddepartamento
                         orderby c.NomCiudad
                         select new
                         {
                             c.Id_Ciudad,
                             c.NomCiudad
                         };

            ddlciudad.DataSource = ciudad;
            ddlciudad.DataTextField = "NomCiudad";
            ddlciudad.DataValueField = "Id_Ciudad";
            ddlciudad.DataBind();
        }

        protected void ddlsector_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarsubsector(Convert.ToInt32(ddlsector.SelectedValue));
        }

        /// <summary>
        /// Diego Quiñonez
        /// cargue de sectores en el dropdownlist
        /// </summary>
        /// <param name="codsector"></param>
        private void llenarsubsector(Int32 codsector)
        {
            var subsector = from s in consultas.Db.SubSectors
                            where s.CodSector == codsector
                            orderby s.NomSubSector
                            select new
                            {
                                s.Id_SubSector,
                                s.NomSubSector
                            };

            ddlsubsector.DataSource = subsector;
            ddlsubsector.DataTextField = "NomSubSector";
            ddlsubsector.DataValueField = "Id_SubSector";
            ddlsubsector.DataBind();
        }

        /// <summary>
        /// Diego Quiñonez
        /// realiza el tranlado del plan de negocio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btntrasladar_Click(object sender, EventArgs e)
        {
            txtSQL = "UPDATE proyecto SET CodInstitucion = " + ddlnombreunidad.SelectedValue + "," + "  CodCiudad = " + ddlciudad.SelectedValue +
            "  , CodSubsector = " + ddlsubsector.SelectedValue + "  WHERE Id_Proyecto= " + id_ProyectoEval;

            ejecutaReader(txtSQL, 2);

            txtSQL = " INSERT INTO LogTrasladoProyectos VALUES(getdate()," + usuario.IdContacto + "," + txtIdSubsector.Text + "," + ddlnombreunidad.SelectedValue + ",'" + txtSQL + "')";

            ejecutaReader(txtSQL, 2);

            txtSQL = "  UPDATE ProyectoContacto SET FechaFin = getdate(), " + "  Inactivo = 1 " + "  FROM rol " + "  WHERE id_rol =  CodRol " +
                "  AND	 Nombre LIKE '%Asesor%'" + "  AND   CodProyecto= " + id_ProyectoEval;

            ejecutaReader(txtSQL, 2);

            txtSQL = " SELECT Distinct CodContacto " + " FROM institucioncontacto, Institucion " + " WHERE inactivo = 0 " +
             " AND Id_Institucion = codinstitucion " + " AND codinstitucion = " + txtIdSubsector.Text;

            SqlDataReader reader = ejecutaReader(txtSQL, 1);

            if (reader.Read())
            {

                AgendarTarea agenda = new AgendarTarea
                    (Convert.ToInt32(reader["CodContacto"].ToString()),
                    "Asignar Asesor a proyecto",
                    "Debe asginar un asesor al proyecto" + lblnombreproyecto.Text + ". Número de proyecto: " + id_ProyectoEval,
                    id_ProyectoEval,
                    2,
                    "0",
                    true,
                    1,
                    true,
                    false,
                    usuario.IdContacto,
                    "",
                    "",
                    "Traslado Planes");

                agenda.Agendar();
            }

            id_ProyectoEval = null;
            Id_ciudadEval = null;

            pnlprincipal.Visible = true;
            pnlprincipal.Enabled = true;

            pnltransaldar.Visible = false;
            pnltransaldar.Enabled = false;
        }
    }
}