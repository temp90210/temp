#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>10 - 06 - 2014</Fecha>
// <Archivo>Consultas.cs</Archivo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;
using Datos;

using Datos.DataType;
using System.Xml;
using System.Data;

namespace Fonade.FONADE.MiPerfil
{
    public partial class Consultas : Negocio.Base_Page
    {
        #region Variables globales.

        public const int PAGE_SIZE = 10;
        public int tiporol1;
        public int tiporol2;
        public int tiporol3;
        /// <summary>
        /// Contiene las consultas SQL.
        /// </summary>
        String txtSQL;
        /// <summary>
        /// Conteo de registros obtenidos de la consulta de planes de negocio por palabra.
        /// </summary>
        Int32 conteo_grvMain;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            tb_codigo.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            if (usuario.CodGrupo != Constantes.CONST_AdministradorFonade)
            {
                vercoor1.Visible = false;
                try { vercoor1.Attributes.Add("", "disabled"); /*vercoor1.Enabled = false;*/ }
                catch { }
                vercoor2.Visible = false;
                try { vercoor2.Attributes.Add("", "disabled");/*vercoor2.Enabled = false;*/ }
                catch { }
            }
            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "move")
            {
                tb_asesor.Text = lb_asesores.SelectedItem.Text;
                lb_asesores.Visible = false;
            }
            lb_asesores.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(lb_asesores, "move"));

            if (!IsPostBack)
            {
                //Determinar la visiblidad de ciertos campos según FONADE clásico.
                if (usuario.CodGrupo == Constantes.CONST_AdministradorFonade || usuario.CodGrupo == Constantes.CONST_AdministradorSena || usuario.CodGrupo == Constantes.CONST_JefeUnidad || usuario.CodGrupo == Constantes.CONST_CallCenter)
                {
                    tr_part_I.Visible = true;
                    tr_part_II.Visible = true;
                    tr_part_III.Visible = true;

                    if (usuario.CodGrupo == Constantes.CONST_AdministradorFonade || usuario.CodGrupo == Constantes.CONST_AdministradorSena || usuario.CodGrupo == Constantes.CONST_CallCenter)
                    { vercoor1.Visible = true; vercoor2.Visible = true; }
                }

                if (usuario.CodGrupo == 1 || usuario.CodGrupo == 4 || usuario.CodGrupo == 2 || usuario.CodGrupo == 8) //Agregado el Rol "Call Center".
                {
                    //registrar doble click

                    // fin registro doble click
                    lb_asesores.Visible = false;
                    Panel1.Visible = true;
                    Panel2.Visible = false;
                    Panel3.Visible = false;
                    Panel4.Visible = false;
                    pnlpanel6.Visible = false;
                    pnlpanel5.Visible = false;
                    pnlInfoResultados.Visible = false;
                    //Se muestra el panel que contiene el buscador de Unidades de Emprendimiento.
                    //El usuario 8 es Constantes.CONST_CallCenter.
                    if (usuario.CodGrupo == 8)
                    {
                        vercoor1.Visible = true;
                        vercoor2.Visible = true;
                    }

                    #region Carga de departamentos.

                    //SELECT id_Departamento, nomDepartamento FROM Departamento WHERE codPais=1 ORDER BY nomDepartamento"
                    var departamentos = from d in consultas.Db.departamentos
                                        where d.CodPais == 1
                                        orderby d.NomDepartamento ascending
                                        select new
                                        {
                                            id_Departamento = d.Id_Departamento,
                                            nomDepartamento = d.NomDepartamento
                                        };

                    ddl_departamento.DataSource = departamentos;
                    ddl_departamento.DataTextField = "nomDepartamento";
                    ddl_departamento.DataValueField = "id_Departamento";
                    ddl_departamento.DataBind();
                    ddl_departamento.Items.Insert(0, new ListItem("(Todos los Departamentos)", ""));

                    #endregion

                    #region Carga de estados.

                    var estados = from es in consultas.Db.Estados
                                  orderby es.NomEstado
                                  select new
                                  {
                                      Id_Estado = es.Id_Estado,
                                      NomEstado = es.NomEstado
                                  };
                    lb_estados.DataSource = estados.ToList();
                    lb_estados.DataTextField = "NomEstado";
                    lb_estados.DataValueField = "Id_Estado";
                    lb_estados.DataBind();
                    lb_estados.Items.Insert(0, new ListItem("                              ", ""));

                    #endregion

                    #region Carga unidad emprendimiento.

                    var unidad = from i in consultas.Db.Institucions
                                 where i.Inactivo == false
                                 orderby i.NomUnidad
                                 select new
                                 {
                                     Id_Institucion = i.Id_Institucion,
                                     NomInstitucion = i.NomInstitucion,
                                     NomUnidad = i.NomUnidad

                                 };
                    if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                    {
                        unidad.Where(i => i.Id_Institucion == usuario.CodInstitucion);
                    }

                    lb_unidadEmprendimiento.DataSource = unidad.ToList();
                    lb_unidadEmprendimiento.DataTextField = "NomUnidad";
                    lb_unidadEmprendimiento.DataValueField = "Id_Institucion";
                    lb_unidadEmprendimiento.DataBind();
                    lb_unidadEmprendimiento.Items.Insert(0, new ListItem("                              ", ""));

                    #endregion

                    #region Carga sectores.

                    var sectores = from s in consultas.Db.Sectors

                                   orderby s.NomSector
                                   select new
                                   {
                                       Id_Sector = s.Id_Sector,
                                       NomSector = s.NomSector
                                   };
                    lb_sector.DataSource = sectores.ToList();
                    lb_sector.DataTextField = "NomSector";
                    lb_sector.DataValueField = "Id_Sector";
                    lb_sector.DataBind();
                    lb_sector.Items.Insert(0, new ListItem("                              ", ""));
                    lbl_Titulo.Text = void_establecerTitulo("CONSULTAS");

                    #endregion

                    try
                    {
                        if (!string.IsNullOrEmpty(Session["consultarMaster"].ToString()))
                        {
                            //codProyecto = Session["CodProyecto"].ToString();
                            tb_porPalabra.Text = Session["consultarMaster"].ToString();
                            Session["consultarMaster"] = null;
                            grdMain.DataSource = null;
                            grdMain.DataBind();
                        }
                    }
                    catch { }
                }
                else { Response.Redirect("Home.aspx"); }
            }
        }

        /// <summary>
        /// Buscar por palabra.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_buscar_onclick(object sender, EventArgs e)
        {
            //Inicializar variables.
            DataTable rstProyecto = new DataTable();

            try
            {
                txtSQL = " SELECT P.Id_Proyecto, P.NomProyecto, P.Sumario, S.NomSubSector, E.NomEstado, C.NomCiudad, D.NomDepartamento, P.FechaCreacion, I.nomUnidad + ' ('+I.nomInstitucion+')' Unidad " +
                         " ,  (Select Max(CodConvocatoria)  from ConvocatoriaProyecto where CodProyecto = P.Id_Proyecto) as CodConvocatoria" +
                         " FROM Proyecto P, SubSector S, Estado E, Ciudad C, Departamento D, Institucion I " +
                         " WHERE " +
                         " (P.NomProyecto like '%" + tb_porPalabra.Text.Trim() + "%' " +
                         "  OR P.Sumario like '%" + tb_porPalabra.Text.Trim() + "%' " +
                         "  OR S.NomSubSector like '%" + tb_porPalabra.Text.Trim() + "%' " +
                         "  OR D.NomDepartamento like '%" + tb_porPalabra.Text.Trim() + "%' " +
                         "  OR C.NomCiudad like '%" + tb_porPalabra.Text.Trim() + "%') " +
                         " AND S.Id_SubSector = P.CodSubSector " +
                         " AND I.Id_Institucion = P.CodInstitucion " +
                         " AND E.Id_estado = P.CodEstado " +
                         " AND C.Id_Ciudad = P.CodCiudad " +
                         " AND C.CodDepartamento = D.Id_Departamento " +
                         " and p.inactivo=0";

                //"Jefe de Unidad"
                if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                { txtSQL = txtSQL + " AND P.CodInstitucion = " + usuario.CodInstitucion; }

                rstProyecto = consultas.ObtenerDataTable(txtSQL, "text");
                conteo_grvMain = rstProyecto.Rows.Count;

                //Ocultar columna "Nombre Ultima Convocatoria" si NO es Administardor FONADE.
                if (usuario.CodGrupo != Constantes.CONST_AdministradorFonade) { this.grdMain.Columns[8].Visible = false; }

                ///Añadir columna para mostrar la columna "Nombre Ultima Convocatoria", consultando
                ///su nombre en un ciclo foreach que llama al método que consulta dicho nombre basado en el código.
                rstProyecto.Columns.Add("N_NomConvocatoria", typeof(System.String));
                rstProyecto.Columns.Add("N_Fecha", typeof(System.String));
                foreach (DataRow dr in rstProyecto.Rows)
                {
                    //Nombre de la convocatoria.
                    dr["N_NomConvocatoria"] = getNomConvocatoria(dr["CodConvocatoria"].ToString());

                    //Fecha formateada según FONADE clásico.
                    try { dr["N_Fecha"] = DateTime.Parse(dr["FechaCreacion"].ToString()).ToString(" d MMM yyyy"); }
                    catch { dr["N_Fecha"] = dr["FechaCreacion"].ToString(); }
                }

                //Bindear los datos...
                Session["tablaConsultas_palabra"] = rstProyecto;

                //Mostrar los resultados obtenidos.
                grdMain.DataSource = rstProyecto;
                grdMain.DataBind();
                try { lblResults.Text = "Se han encontrado " + conteo_grvMain + " Planes de negocio de <br/> buscando ''" + tb_porPalabra.Text + "''"; }
                catch { lblResults.Text = "Se han encontrado planes de <br/> negocio buscando ''"; }

                //Mostrar paneles de los resultados...
                Panel3.Visible = false;
                Panel1.Visible = false;
                Panel2.Visible = true;
                pnlInfoResultados.Visible = true;
            }
            catch
            { }
        }

        protected void hpl_nueva_onclick(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
            Panel3.Visible = false;
            Panel4.Visible = false;
            pnlInfoResultados.Visible = false;
            pnlpanel5.Visible = false;
            pnlpanel6.Visible = false;

            //Se muestra el panel que contiene el buscador de Unidades de Emprendimiento.
            //El usuario 8 es Constantes.CONST_CallCenter.
            if (usuario.CodGrupo == 8)
            {
                vercoor1.Visible = true;
                vercoor2.Visible = true;
            }
        }

        protected void btn_buscarAsesor_onclick(object sender, EventArgs e)
        {
            #region LINQ Defectuoso.
            //var asesores = (from c in consultas.Db.Contactos
            //                from gc in consultas.Db.GrupoContactos
            //                where gc.CodGrupo == Constantes.CONST_Asesor &
            //                gc.CodContacto == c.Id_Contacto
            //                select new
            //                {
            //                    Nombres_Asesores = double.Parse(c.Identificacion.ToString()).ToString() + " - " + c.Nombres + " " + c.Apellidos,
            //                    id_contactos = c.Id_Contacto,
            //                    c.Nombres,
            //                    c.Apellidos

            //                }).OrderBy(d => d.Nombres).ThenByDescending(n => n.Apellidos);

            //lb_asesores.Visible = true;
            //lb_asesores.DataSource = asesores;
            //lb_asesores.DataTextField = "Nombres_Asesores";
            //lb_asesores.DataValueField = "id_contactos";
            //lb_asesores.DataBind();
            #endregion

            #region Versión de Mauricio Arias Olave.

            txtSQL = " SELECT * FROM Contacto C, GrupoContacto GC WHERE GC.CodGrupo = 5 AND GC.CodContacto = Id_Contacto ORDER BY Nombres, Apellidos ";
            var rs = consultas.ObtenerDataTable(txtSQL, "text");

            lb_asesores.Items.Clear();

            foreach (DataRow row in rs.Rows)
            {
                ListItem item = new ListItem();
                item.Value = row["Id_Contacto"].ToString();
                item.Text = row["Identificacion"].ToString() + " - " + row["Nombres"].ToString() + " " + row["Apellidos"].ToString();
                lb_asesores.Items.Add(item);
            }

            lb_asesores.Visible = true;
            rs = null;

            #endregion
        }

        protected void btn_BusquedaAvanzada_onclick(object sender, EventArgs e)
        {
            #region Comentarios NO BORRAR.
            ////Debería llamar al método "lds_Consultaravanzada_Selecting", pero se 
            ////usarán consultas SQL como en FONADE clásico.
            //gv_busquedaavanzada.DataBind(); 
            #endregion

            #region Versión SQL.
            try
            {
                //Tabla que contendrá la información consultada.
                DataTable rstProyecto = new DataTable();
                //Variable que indica si ha seleccionado el asesor para así ocultar "también" las columas "Asesor" y "Lider".
                bool tieneAsesor;

                #region Variables de consultas SQL temporales.

                string txtSQLTemp = "";
                string txtSQLTemp1 = "";

                #endregion

                #region Variables a emplear en la generación de la consulta.

                string txtCodigo = "";
                string CodSector = "";
                string CodEstado = "";
                string CodUnidad = "";
                string CodContacto = "";
                string CodCiudad = "";
                string CodDepartamento = "";
                bool bolIncluye = CheckBox1.Checked;

                #endregion

                #region Asignación de valores en las variables y construcción de la consulta SQL.

                if (tb_codigo.Text.Trim() != "")
                {
                    txtCodigo = tb_codigo.Text.Trim();
                    txtSQL = txtSQL + " AND Id_Proyecto = " + txtCodigo;
                }

                if (lb_sector.SelectedValue != "")
                {
                    CodSector = lb_sector.SelectedValue;
                    txtSQL = txtSQL + " AND SC.Id_Sector IN (" + CodSector + ")";
                }

                if (lb_estados.SelectedValue != "")
                {
                    CodEstado = lb_estados.SelectedValue;
                    txtSQL = txtSQL + " AND E.Id_Estado IN(" + CodEstado + ")";
                }

                if (lb_unidadEmprendimiento.SelectedValue != "")
                {
                    CodUnidad = lb_unidadEmprendimiento.SelectedValue;
                    txtSQL = txtSQL + " AND P.CodInstitucion IN(" + CodUnidad + ")";
                }

                //Asesor seleccionado.
                hdf_CodContacto.Value = lb_asesores.SelectedValue;
                CodContacto = hdf_CodContacto.Value;

                //if (hdf_CodContacto.Value != "")
                //{ CodContacto = hdf_CodContacto.Value; }

                if (ddl_municipio.SelectedValue != "")
                {
                    CodCiudad = ddl_municipio.SelectedValue;
                    txtSQL = txtSQL + " AND P.CodCiudad IN(" + CodCiudad + ")";
                }

                if (ddl_departamento.SelectedValue != "")
                {
                    CodDepartamento = ddl_departamento.SelectedValue;
                    txtSQL = txtSQL + " AND CP.CodDepartamento IN(" + CodDepartamento + ")";
                }

                #endregion

                if (CodContacto != "")
                {
                    #region Si ha seleccionado asesor(es)...

                    txtSQL = txtSQL + " AND PC.CodContacto = " + CodContacto;
                    txtSQLTemp = " SELECT Id_Proyecto, NomProyecto, Sumario, S.NomSubSector, E.NomEstado, CP.NomCiudad, DP.NomDepartamento, Nomunidad, NomInstitucion, CI.NomCiudad +','+ DI.NomDepartamento as CiudadUnidad, A.Nombres + ' ' + A.Apellidos Asesor, case PC.CodRol when 1 then 'Lider' else '' end Lider " +
                                 " ,  (Select Max(CodConvocatoria)  from ConvocatoriaProyecto where CodProyecto = P.Id_Proyecto) as CodConvocatoria" +
                                 " FROM Proyecto P, SubSector S, Estado E, Ciudad CP, Departamento DP, Sector SC, Institucion I, ProyectoContacto PC, Contacto A, Ciudad CI, Departamento DI " +
                                 " WHERE S.Id_SubSector = P.CodSubSector " + txtSQL +
                                 " AND PC.CodProyecto = P.Id_Proyecto" +
                                 " AND PC.CodRol IN(1,2)" +
                                 " AND PC.Codcontacto=A.id_contacto" +
                                 " AND PC.Inactivo = 0 " +
                                 " AND P.CodSubSector = S.Id_SubSector" +
                                 " AND SC.Id_Sector = S.CodSector" +
                                 " AND E.Id_Estado = P.CodEstado " +
                                 " AND CP.Id_Ciudad = P.CodCiudad " +
                                 " AND CP.CodDepartamento = DP.Id_Departamento" +
                                 " AND CI.Id_Ciudad = I.CodCiudad " +
                                 " AND CI.CodDepartamento = DI.Id_Departamento" +
                                 " AND P.CodInstitucion = Id_Institucion " +
                                 " AND P.inactivo=0";

                    txtSQLTemp1 = " SELECT Count(Id_Proyecto) " +
                                  " FROM Proyecto P, SubSector S, Estado E, Ciudad C, Departamento D, Sector SC, Institucion, ProyectoContacto PC, Contacto A " +
                                  " WHERE S.Id_SubSector = P.CodSubSector " + txtSQL +
                                  " AND PC.CodProyecto = P.Id_Proyecto" +
                                  " AND PC.CodRol IN(1,2)" +
                                  " AND PC.Codcontacto=A.id_contacto" +
                                  " AND PC.Inactivo = 0 " +
                                  " AND P.CodSubSector = S.Id_SubSector" +
                                  " AND SC.Id_Sector = S.CodSector" +
                                  " AND E.Id_Estado = P.CodEstado " +
                                  " AND C.Id_Ciudad = P.CodCiudad " +
                                  " AND C.CodDepartamento = D.Id_Departamento" +
                                  " AND P.CodInstitucion = Id_Institucion " +
                                  " AND P.inactivo=0";

                    //Si tiene asesor seleccionado.
                    tieneAsesor = true;

                    #endregion
                }
                else
                {
                    #region No ha seleccionado asesor(es)...

                    txtSQLTemp = " SELECT Id_Proyecto, NomProyecto, Sumario, S.NomSubSector, E.NomEstado, CP.NomCiudad, DP.NomDepartamento, NomUnidad, NomInstitucion, CI.NomCiudad +','+ DI.NomDepartamento as CiudadUnidad  " +
                                         ",  (Select Max(CodConvocatoria)  from ConvocatoriaProyecto where CodProyecto = P.Id_Proyecto) as CodConvocatoria" +
                                         " , '' AS [Asesor] , '' AS [Lider] " +
                                         " FROM Proyecto P, SubSector S, Estado E, Ciudad CP, Departamento DP, Sector SC, Institucion I, Ciudad CI, Departamento DI" +
                                         " WHERE S.Id_SubSector = P.CodSubSector " + txtSQL +
                                         " AND P.CodSubSector = S.Id_SubSector" +
                                         " AND SC.Id_Sector = S.CodSector" +
                                         " AND E.Id_Estado = P.CodEstado " +
                                         " AND CP.Id_Ciudad = P.CodCiudad " +
                                         " AND CP.CodDepartamento = DP.Id_Departamento" +
                                         " AND CI.Id_Ciudad = I.CodCiudad " +
                                         " AND CI.CodDepartamento = DI.Id_Departamento" +
                                         " AND P.CodInstitucion = Id_Institucion" +
                                         " AND P.inactivo=0";

                    txtSQLTemp1 = " SELECT Count(Id_Proyecto) " +
                                  " FROM Proyecto P, SubSector S, Estado E, Ciudad CP, Departamento DP, Sector SC, Institucion I" +
                                  " WHERE S.Id_SubSector = P.CodSubSector " + txtSQL +
                                  " AND P.CodSubSector = S.Id_SubSector" +
                                  " AND SC.Id_Sector = S.CodSector" +
                                  " AND E.Id_Estado = P.CodEstado " +
                                  " AND CP.Id_Ciudad = P.CodCiudad " +
                                  " AND CP.CodDepartamento = DP.Id_Departamento" +
                                  " AND P.CodInstitucion = Id_Institucion" +
                                  " AND P.inactivo=0";

                    //No tiene un asesor seleccionado.
                    tieneAsesor = false;

                    #endregion
                }

                #region Si el usuario en sesión es jefe de unidad...

                if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                {
                    txtSQLTemp = txtSQLTemp + " AND P.CodInstitucion = " + usuario.CodInstitucion;
                    txtSQLTemp1 = txtSQLTemp1 + " AND P.CodInstitucion = " + usuario.CodInstitucion;
                }

                #endregion

                rstProyecto = consultas.ObtenerDataTable(txtSQLTemp, "text");

                //Mostrar columna "Descripción", si ha seleccionado el checkbox "Incluir Descripción".
                this.gv_busquedaavanzada.Columns[6].Visible = bolIncluye;

                //Mostrar columnas "Asesor" y "Lider". (oculta las columas si NO ha seleccionado un asesor).
                //Es decir, si sí ha seleccionado asesor!
                if (CodContacto != "" || tieneAsesor)
                {
                    this.gv_busquedaavanzada.Columns[8].Visible = true;
                    this.gv_busquedaavanzada.Columns[9].Visible = true;
                }
                else
                {
                    this.gv_busquedaavanzada.Columns[8].Visible = false;
                    this.gv_busquedaavanzada.Columns[9].Visible = false;
                }

                //Ocultar columna "Número Ultima Convocatoria" si NO es Administardor FONADE.
                if (usuario.CodGrupo != Constantes.CONST_AdministradorFonade) { this.gv_busquedaavanzada.Columns[10].Visible = false; } //7

                ///Añadir columna para mostrar la columna "Nombre Ultima Convocatoria", consultando
                ///su nombre en un ciclo foreach que llama al método que consulta dicho nombre basado en el código.
                rstProyecto.Columns.Add("N_NomConvocatoria", typeof(System.String));
                foreach (DataRow dr in rstProyecto.Rows)
                {
                    //Localización Proyecto.
                    dr["NomCiudad"] = dr["NomCiudad"].ToString() + ", " + dr["NomDepartamento"].ToString();

                    //Localización Unidad.
                    dr["NomUnidad"] = dr["NomUnidad"].ToString() + " (" + dr["NomInstitucion"].ToString() + ")";

                    //Nombre de la convocatoria.
                    dr["N_NomConvocatoria"] = getNomConvocatoria(dr["CodConvocatoria"].ToString());
                }

                //Bindear los datos...
                Session["tablaConsultas"] = rstProyecto;
                gv_busquedaavanzada.PageSize = 10;
                gv_busquedaavanzada.DataSource = rstProyecto;
                gv_busquedaavanzada.DataBind();

                //Mostrar la información.
                Panel1.Visible = false;
                Panel2.Visible = false;
                Panel3.Visible = false;
                Panel4.Visible = true;
                lblResults.Text = "Se han encontrado " + rstProyecto.Rows.Count + " Planes de negocio de <br/> acuerdo a los criterios seleccionados";
                pnlInfoResultados.Visible = true;
            }
            catch
            {
                Panel1.Visible = true;
                Panel2.Visible = true;
                Panel3.Visible = true;
                Panel4.Visible = false;
                pnlInfoResultados.Visible = false;
                lblResults.Text = "Se han encontrado planes de <br/> negocio buscando ''";
            }

            #endregion
        }

        /// <summary>
        /// Buscar por Emprendedores y/o asesores.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void tb_cedulaPalabra_onclick(object sender, EventArgs e)
        {
            #region COMENTARIOS DE DESARROLLO ANTERIOR.
            //string valor = RadioButtonList1.SelectedValue.ToString();

            //tiporol1 = 0;
            //tiporol2 = 0;
            //tiporol3 = 0;

            //switch (valor)
            //{
            //    case "1":
            //        tiporol1 = 3;
            //        tiporol2 = 0;
            //        tiporol3 = 0;
            //        break;

            //    case "2":
            //        tiporol1 = 1;
            //        tiporol2 = 2;
            //        tiporol3 = 0;
            //        break;
            //    case "3":
            //        tiporol1 = 1;
            //        tiporol2 = 2;
            //        tiporol3 = 3;
            //        break;
            //    default:
            //        return;
            //}; 
            #endregion

            try
            {
                //Inicializar variables.
                DataTable rstContacto = new DataTable();
                string CodTipoContacto = RadioButtonList1.SelectedValue;

                txtSQL = "SELECT P.Id_Proyecto, P.NomProyecto, T.NomTipoIdentificacion, C.Identificacion, C.Id_contacto, C.Nombres, C.Apellidos, C.Email, R.Id_Rol, R.Nombre, nomUnidad+' ('+nomInstitucion+')' nomInstitucion, nomTipoInstitucion " +
                         " ,  (Select Max(CodConvocatoria)  from ConvocatoriaProyecto where CodProyecto = P.Id_Proyecto) as CodConvocatoria" +
                         " FROM Contacto C, TipoIdentificacion T, Proyecto P, ProyectoContacto PC, Rol R, Institucion I, TipoInstitucion TI" +
                         " WHERE C.CodTipoIdentificacion = T.Id_TipoIdentificacion" +
                         " AND PC.CodContacto = C.Id_Contacto" +
                         " AND C.CodInstitucion = I.Id_Institucion" +
                         " AND I.CodTipoInstitucion = TI.Id_TipoInstitucion" +
                         " AND PC.Inactivo = 0 AND P.inactivo=0 " +
                         " AND P.Id_Proyecto = PC.CodProyecto" +
                         " AND PC.CodRol = R.Id_Rol AND PC.codRol in (" + CodTipoContacto + ")" +
                         " AND (C.Nombres LIKE '%" + tb_cedulaPalabra.Text.Trim() + "%' OR C.Apellidos LIKE '%" + tb_cedulaPalabra.Text.Trim() + "%') ORDER BY P.Id_Proyecto";

                //"Jefe de Unidad"
                if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
                { txtSQL = txtSQL + " AND P.CodInstitucion = " + usuario.CodInstitucion; }

                rstContacto = consultas.ObtenerDataTable(txtSQL, "text");
                conteo_grvMain = rstContacto.Rows.Count;

                //Ocultar columna "Número Ultima Convocatoria" si NO es Administardor FONADE.
                if (usuario.CodGrupo != Constantes.CONST_AdministradorFonade) { this.gv_busquedaporrol.Columns[9].Visible = false; }

                ///Añadir columna para mostrar la columna "Nombre Ultima Convocatoria", consultando
                ///su nombre en un ciclo foreach que llama al método que consulta dicho nombre basado en el código.
                rstContacto.Columns.Add("N_NomConvocatoria", typeof(System.String));
                foreach (DataRow dr in rstContacto.Rows)
                {
                    //Nombre de la convocatoria.
                    dr["N_NomConvocatoria"] = getNomConvocatoria(dr["CodConvocatoria"].ToString());
                }

                //Bindear los datos...
                Session["tablaConsultas_Rol"] = rstContacto;

                gv_busquedaporrol.DataSource = rstContacto;
                gv_busquedaporrol.DataBind();

                //Mostrar paneles de los resultados...
                Panel1.Visible = false;
                Panel2.Visible = false;
                Panel3.Visible = true;
                pnlInfoResultados.Visible = true;

                try { lblResults.Text = "Se han encontrado " + conteo_grvMain + " usuarios<br/> buscando ''" + tb_porPalabra.Text + "''"; }
                catch { lblResults.Text = "Se han encontrado usuarios buscando ''"; }
            }
            catch { }
        }

        protected void ddl_departamento_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_departamento.SelectedValue != "")
            {
                var municipios = (from c in consultas.Db.Ciudads
                                  where c.CodDepartamento == Int32.Parse(ddl_departamento.SelectedValue)
                                  orderby c.NomCiudad ascending
                                  select new
                                  {
                                      Ciudad = c.NomCiudad,
                                      ID_Ciudad = c.Id_Ciudad
                                  });
                ddl_municipio.DataSource = municipios;
                ddl_municipio.DataTextField = "Ciudad";
                ddl_municipio.DataValueField = "ID_Ciudad";
                ddl_municipio.DataBind();
                ddl_municipio.Items.Insert(0, new ListItem("(Todos los Municipios)", ""));
            }
        }

        protected void lds_Consultarporrol_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            int resultadoConversion;
            bool esnumero = Int32.TryParse(tb_cedulaPalabra.Text, out resultadoConversion);
            if (esnumero) { }
            else { resultadoConversion = 0; }

            var contactos = (from c in consultas.Db.Contactos
                             from t in consultas.Db.TipoIdentificacions
                             from p in consultas.Db.Proyectos
                             from pc in consultas.Db.ProyectoContactos
                             from r in consultas.Db.Rols
                             from i in consultas.Db.Institucions
                             from ti in consultas.Db.TipoInstitucions
                             orderby c.Id_Contacto
                             where (
                                 c.CodTipoIdentificacion == t.Id_TipoIdentificacion &
                                  pc.CodContacto == c.Id_Contacto &
                                  c.CodInstitucion == i.Id_Institucion &
                                  i.CodTipoInstitucion == ti.Id_TipoInstitucion
                                  & pc.Inactivo == false & p.Inactivo == false
                                  & p.Id_Proyecto == pc.CodProyecto
                                  & (pc.CodRol == tiporol1 || pc.CodRol == tiporol2 || pc.CodRol == tiporol3)
                                  & pc.CodRol == r.Id_Rol & (c.Nombres.Contains(tb_cedulaPalabra.Text) || c.Apellidos.Contains(tb_cedulaPalabra.Text) || c.Identificacion == resultadoConversion))

                             select new
                             {
                                 Id_Proyecto = p.Id_Proyecto,
                                 codRol = r.Id_Rol,
                                 Unidad = i.NomUnidad,
                                 codUnidad = i.Id_Institucion,
                                 NomProyecto = p.NomProyecto,
                                 NomTipoIdentificacion = t.NomTipoIdentificacion,
                                 Identificacion = c.Identificacion,
                                 Id_contacto = c.Id_Contacto,
                                 Nombres = c.Nombres + " " + c.Apellidos,
                                 Apellidos = c.Apellidos,
                                 Email = c.Email,
                                 Id_Rol = r.Id_Rol,
                                 Rol = r.Nombre,
                                 Nombre = r.Nombre,
                                 NomTipoInstitucion = ti.NomTipoInstitucion
                             });

            var conteo = (from c in consultas.Db.Contactos
                          from t in consultas.Db.TipoIdentificacions
                          from p in consultas.Db.Proyectos
                          from pc in consultas.Db.ProyectoContactos
                          from r in consultas.Db.Rols
                          from i in consultas.Db.Institucions
                          from ti in consultas.Db.TipoInstitucions
                          orderby c.Id_Contacto
                          where (
                              c.CodTipoIdentificacion == t.Id_TipoIdentificacion &
                              pc.CodContacto == c.Id_Contacto &
                              c.CodInstitucion == i.Id_Institucion &
                              i.CodTipoInstitucion == ti.Id_TipoInstitucion
                              & pc.Inactivo == false & p.Inactivo == false
                              & p.Id_Proyecto == pc.CodProyecto
                              & (pc.CodRol == tiporol1 || pc.CodRol == tiporol2 || pc.CodRol == tiporol3)
                              & pc.CodRol == r.Id_Rol & (c.Nombres.Contains(tb_cedulaPalabra.Text) || c.Apellidos.Contains(tb_cedulaPalabra.Text) || c.Identificacion == resultadoConversion))
                          select new
                          {
                              Id_Proyecto = p.Id_Proyecto,
                              codRol = r.Id_Rol
                          }).Count();

            if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
            {
                contactos = contactos.Where(x => x.codUnidad == usuario.CodInstitucion);
            }

            double numero;
            bool validaelnumero;

            validaelnumero = Double.TryParse(tb_cedulaPalabra.Text, out numero);

            if (validaelnumero)
                contactos = contactos.Where(re => re.Identificacion == Convert.ToDouble(tb_cedulaPalabra.Text));
            else
            {
                contactos = contactos.Where(re => (re.Nombres + " " + re.Apellidos).Contains(tb_cedulaPalabra.Text));
            }

            #region COMENTARIOS ANTES DE LAS MODIFICACIONES DEL 20/08/2014.
            //if (tiporol1 != 0 && tiporol1 != null)
            //{
            //    contactos = contactos.Where(re => re.codRol == tiporol1);
            //}


            //if (tiporol2 != 0 && tiporol2 != null)
            //{
            //    contactos = contactos.Where(re => re.codRol == tiporol2);
            //}

            //if (tiporol3 != 0 && tiporol3 != null)
            //{
            //    contactos = contactos.Where(re => re.codRol == tiporol3);
            //} 
            #endregion

            e.Arguments.TotalRowCount = conteo;
            conteo_grvMain = conteo;
            contactos = contactos.Skip(grdMain.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            e.Result = contactos;

            Panel1.Visible = false;
            Panel2.Visible = false;
            Panel3.Visible = true;
            lblResults.Text = "Se han encontrado " + conteo + " Planes de <br/> negocio buscando '" + tb_porPalabra.Text + "'";
            pnlInfoResultados.Visible = true;
        }

        /// <summary>
        /// LINQ de grvMain...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lds_Consultar_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var Consulta = from P in consultas.Consultar(usuario.IdContacto, usuario.CodGrupo, usuario.CodInstitucion, "palabra", tb_porPalabra.Text)
                               select P;

                e.Arguments.TotalRowCount = Consulta.Count();

                try
                { Consulta = Consulta.Skip(grdMain.PageIndex * PAGE_SIZE).Take(PAGE_SIZE).ToList(); }
                catch { }

                e.Result = Consulta;

                Panel3.Visible = false;
                Panel1.Visible = false;
                Panel2.Visible = true;
                pnlInfoResultados.Visible = true;
            }
            catch { }
        }

        /// <summary>
        /// LINQ de la consulta avanzada...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lds_Consultaravanzada_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var consultaav1 = (from p in consultas.Db.Proyectos
                               from s in consultas.Db.SubSectors
                               from es in consultas.Db.Estados
                               from cp in consultas.Db.Ciudads
                               from dp in consultas.Db.departamentos
                               from sc in consultas.Db.Sectors
                               from i in consultas.Db.Institucions
                               from pc in consultas.Db.ProyectoContactos
                               from c in consultas.Db.Contactos
                               from ci in consultas.Db.Ciudads
                               from di in consultas.Db.departamentos
                               where (s.Id_SubSector == p.CodSubSector & pc.CodProyecto == p.Id_Proyecto & (pc.CodRol == 1 | pc.CodRol == 2)
                                     & pc.CodContacto == c.Id_Contacto
                                     & pc.Inactivo == false
                                     & p.CodSubSector == s.Id_SubSector
                                     & sc.Id_Sector == s.CodSector
                                     & es.Id_Estado == p.CodEstado
                                     & cp.Id_Ciudad == p.CodCiudad
                                     & cp.CodDepartamento == dp.Id_Departamento
                                     & ci.Id_Ciudad == i.CodCiudad
                                     & ci.CodDepartamento == di.Id_Departamento
                                     & p.CodInstitucion == i.Id_Institucion
                                     & p.Inactivo == false)
                               //group p by p.Id_Proyecto into grouping
                               select new
                               {
                                   Id_Proyecto = p.Id_Proyecto,
                                   NomSubSector = s.NomSubSector,
                                   NomCiudad = cp.NomCiudad,
                                   NomProyecto = p.NomProyecto,
                                   CiudadUnidad = ci.NomCiudad + ',' + di.NomDepartamento,
                                   Nomunidad = i.NomUnidad,
                                   NomEstado = es.NomEstado,

                                   CodSector = sc.Id_Sector,
                                   CodEstados = es.Id_Estado,
                                   CodInstitucion = p.CodInstitucion,
                                   CodCiudad = p.CodCiudad,
                                   CodDepartamento = cp.CodDepartamento,
                                   CodContacto = pc.CodContacto,

                                   Sumario = p.Sumario,
                                   NomDepartamento = dp.NomDepartamento,
                                   NomInstitucion = i.NomInstitucion,
                                   Asesor = c.Nombres + ' ' + c.Apellidos,
                                   CodRol = pc.CodRol
                               }).Distinct();

            if (!string.IsNullOrEmpty(tb_codigo.Text))
            {
                //consultaav1 = consultaav1.AsQueryable().GroupBy(x => x.Id_Proyecto).SelectMany(x => x);

                consultaav1 = consultaav1.Where(x => x.Id_Proyecto == Int32.Parse(tb_codigo.Text)).Take(1);

            }

            if (!string.IsNullOrEmpty(lb_sector.SelectedValue) && !lb_sector.SelectedValue.Equals(""))
            {
                consultaav1 = consultaav1.Where(x => x.CodSector.ToString().Contains(lb_sector.SelectedValue));
            }

            if (!string.IsNullOrEmpty(lb_estados.SelectedValue) && !lb_estados.SelectedValue.Equals(""))
            {
                consultaav1 = consultaav1.Where(x => x.CodEstados.ToString().Contains(lb_estados.SelectedValue));
            }

            if (!string.IsNullOrEmpty(ddl_municipio.SelectedValue) && !ddl_municipio.SelectedValue.Equals(""))
            {
                consultaav1 = consultaav1.Where(x => x.CodCiudad.ToString().Contains(ddl_municipio.SelectedValue));
            }

            if (!string.IsNullOrEmpty(ddl_departamento.SelectedValue) && !ddl_departamento.SelectedValue.Equals(""))
            {
                consultaav1 = consultaav1.Where(x => x.CodDepartamento.ToString().Contains(ddl_departamento.SelectedValue));
            }

            if (!string.IsNullOrEmpty(lb_unidadEmprendimiento.SelectedValue) && !lb_unidadEmprendimiento.SelectedValue.Equals(""))
            {
                consultaav1 = consultaav1.Where(x => x.CodInstitucion.ToString().Contains(lb_unidadEmprendimiento.SelectedValue));
            }

            if (!string.IsNullOrEmpty(lb_asesores.SelectedValue) && !lb_asesores.SelectedValue.Equals(""))
            {
                consultaav1 = consultaav1.Where(x => x.CodContacto == Int32.Parse(lb_asesores.SelectedValue));
            }

            if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
            {
                consultaav1.Where(y => usuario.CodInstitucion == y.CodInstitucion);
            }

            var lista = consultaav1.OrderBy(y => y.Id_Proyecto).ToList();

            int idpro = 0;

            try
            {
                foreach (var res in lista)
                {
                    if (idpro != res.Id_Proyecto)
                        idpro = res.Id_Proyecto;
                    else
                        lista.Remove(res);
                }
            }
            catch (InvalidOperationException) { }

            consultaav1 = lista.AsQueryable();

            e.Arguments.TotalRowCount = consultaav1.Count();
            consultaav1 = consultaav1.Skip(gv_busquedaavanzada.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            e.Result = consultaav1;

            Panel1.Visible = false;
            Panel2.Visible = false;
            Panel3.Visible = false;
            Panel4.Visible = true;
            pnlInfoResultados.Visible = true;
        }

        protected void btnbuscarcedulaopalabra_Click(object sender, EventArgs e)
        {
            var sesult = (from ju in consultas.Db.MD_Consultarjefe()
                          select new
                          {
                              ju.NomTipoIdentificacion,
                              ju.Identificacion,
                              Nombre = ju.Nombres + " " + ju.Apellidos,
                              ju.Email,
                              Unidad = ju.NomUnidad + " (" + ju.NomInstitucion + ")",
                              ciudad = ju.NomCiudad + " (" + ju.NomDepartamento + ")"
                          });

            if (!string.IsNullOrEmpty(tctcedulaopalabra.Text))
            {
                //var contados = sesult.Count();
                Int32 number1 = 0;
                bool canConvert = Int32.TryParse(tctcedulaopalabra.Text, out number1);

                if (canConvert)
                    sesult = sesult.Where(ju => ju.Identificacion == number1);
                else
                    sesult = sesult.Where(ju => ju.Nombre.ToString().ToLower().Contains(tctcedulaopalabra.Text.ToLower()));
            }

            Panel1.Visible = false;
            Panel2.Visible = false;
            Panel3.Visible = false;
            Panel4.Visible = false;
            pnlpanel5.Visible = true;
            pnlInfoResultados.Visible = true;

            Session["result_contacto"] = sesult;
            gvcontacto.DataSource = sesult;
            gvcontacto.DataBind();
        }

        protected void btnbuscarcedulaopalabra0_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel2.Visible = false;
            Panel3.Visible = false;
            Panel4.Visible = false;
            pnlpanel5.Visible = false;
            pnlpanel6.Visible = true;
            pnlInfoResultados.Visible = true;

            var result = from ue in consultas.Db.MD_Consultar_UnidadesdeEmprendimiento(byte.Parse(radiobutonunidades.SelectedValue), txtpalabra.Text)
                         select new
                         {
                             ue.NomTipoInstitucion,
                             unidad = ue.NomUnidad + " (" + ue.NomTipoInstitucion + ")",
                             ciudad = ue.NomCiudad + " (" + ue.NomDepartamento + ")",
                             Nombre = ue.Nombres + " " + ue.Apellidos,
                             ue.Email,
                             ue.Telefono
                         };

            Session["data_gridview1"] = result;
            GridView1.DataSource = result;
            GridView1.DataBind();
        }

        protected void lnk_Limpiar_Click(object sender, EventArgs e)
        { tb_asesor.Text = ""; hdf_CodContacto.Value = ""; }

        protected void gv_busquedaavanzada_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("rdkproyecto"))
            {
                Session["CodProyecto"] = e.CommandArgument.ToString();
                Response.Redirect("~/Fonade/Proyecto/ProyectoFrameSet.aspx");
            }
        }

        protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("plan"))
            {
                Session["CodProyecto"] = e.CommandArgument.ToString();
                Response.Redirect("~/Fonade/Proyecto/ProyectoFrameSet.aspx");
            }
        }

        protected void gv_busquedaporrol_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("planes"))
            {
                Session["CodProyecto"] = e.CommandArgument.ToString();
                Response.Redirect("~/Fonade/Proyecto/ProyectoFrameSet.aspx");
            }
            else
            {
                if (e.CommandArgument.Equals("email"))
                {
                    Session["CodProyecto"] = e.CommandArgument.ToString();
                    Response.Redirect("~/Fonade/Proyecto/ProyectoFrameSet.aspx");
                }
            }
        }

        /// <summary>
        /// Paginación del GridView de búsqueda avanzada...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_busquedaavanzada_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["tablaConsultas"] as DataTable;

            if (dt != null)
            {
                gv_busquedaavanzada.PageIndex = e.NewPageIndex;
                gv_busquedaavanzada.DataSource = dt;
                gv_busquedaavanzada.DataBind();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 08/09/2014.
        /// Obtener el nombre de la convocatoria "tal como se hace en FONADE clásico".
        /// </summary>
        /// <param name="pCodConvocatoria">Código de la convocatoria.</param>
        /// <returns>string</returns>
        private string getNomConvocatoria(string pCodConvocatoria)
        {
            //Inicializar variables.
            string lSentencia;
            DataTable rs = new DataTable();

            try
            {
                lSentencia = "Select NomConvocatoria from Convocatoria where Id_Convocatoria = '" + pCodConvocatoria + "'";
                rs = consultas.ObtenerDataTable(lSentencia, "text");

                if (rs.Rows.Count > 0) { return rs.Rows[0]["NomConvocatoria"].ToString(); } else { return ""; }
            }
            catch { return ""; }
        }

        /// <summary>
        /// Mauricio Arias Olave-
        /// 09/09/2014.
        /// Paginación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdMain_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["tablaConsultas_palabra"] as DataTable;

            if (dt != null)
            {
                grdMain.PageIndex = e.NewPageIndex;
                grdMain.DataSource = dt;
                grdMain.DataBind();
            }
        }

        /// <summary>
        /// Se debe enviar la información de la tabla en uan variable se sesión
        /// para poder sortearlo.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            var sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;

                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        /// <summary>
        /// Sortear la grilla "grdMain".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdMain_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["tablaConsultas_palabra"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                grdMain.DataSource = dt;
                grdMain.DataBind();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave-
        /// 09/09/2014.
        /// Paginación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_busquedaporrol_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["tablaConsultas_Rol"] as DataTable;

            if (dt != null)
            {
                gv_busquedaporrol.PageIndex = e.NewPageIndex;
                gv_busquedaporrol.DataSource = dt;
                gv_busquedaporrol.DataBind();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/09/2014.
        /// De acuerdo a la grilla visible, se DEBE cambiar el número de planes por 
        /// página como se muestra en FOANDE clásico.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void numRegPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Inicializar variables.
            var dt = new DataTable();
            Int32 PlanesPorPagina = 10;

            try
            {
                //Convertir el valor para aplicarlo a la grilla visible.
                PlanesPorPagina = Int32.Parse(numRegPorPagina.SelectedValue);

                if (grdMain.Visible)
                {
                    dt = Session["tablaConsultas_palabra"] as DataTable;

                    if (dt != null)
                    {
                        grdMain.PageSize = PlanesPorPagina;
                        grdMain.DataSource = dt;
                        grdMain.DataBind();
                    }
                }

                if (gv_busquedaporrol.Visible)
                {
                    dt = Session["tablaConsultas_Rol"] as DataTable;

                    if (dt != null)
                    {
                        gv_busquedaporrol.PageSize = PlanesPorPagina;
                        gv_busquedaporrol.DataSource = dt;
                        gv_busquedaporrol.DataBind();
                    }
                }

                if (gv_busquedaavanzada.Visible)
                {
                    dt = Session["tablaConsultas"] as DataTable;

                    if (dt != null)
                    {
                        gv_busquedaavanzada.PageSize = PlanesPorPagina;
                        gv_busquedaavanzada.DataSource = dt;
                        gv_busquedaavanzada.DataBind();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/09/2014.
        /// Mostrar el número de páginas de la grilla "grdMain".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Calculate the current page number.
            int currentPage = grdMain.PageIndex + 1;

            // Update the Label control with the current page information.
            lbl_NumeroPaginas.Text = "Página " + currentPage.ToString() + " de " + grdMain.PageCount.ToString();
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/09/2014.
        /// Mostrar el número de páginas de la grilla "gv_busquedaporrol".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_busquedaporrol_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Calculate the current page number.
            int currentPage = gv_busquedaporrol.PageIndex + 1;

            // Update the Label control with the current page information.
            lbl_NumeroPaginas.Text = "Página " + currentPage.ToString() + " de " + gv_busquedaporrol.PageCount.ToString();
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/09/2014.
        /// Mostrar el número de páginas de la grilla "gv_busquedaavanzada".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_busquedaavanzada_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Calculate the current page number.
            int currentPage = gv_busquedaavanzada.PageIndex + 1;

            // Update the Label control with the current page information.
            lbl_NumeroPaginas.Text = "Página " + currentPage.ToString() + " de " + gv_busquedaavanzada.PageCount.ToString();
        }

        /// <summary>
        /// Ordernas los resultados ascendente o descendentemente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcontacto_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["result_contacto"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gvcontacto.DataSource = dt;
                gvcontacto.DataBind();
            }
        }

        /// <summary>
        /// Ordernas los resultados ascendente o descendentemente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["data_gridview1"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        /// <summary>
        /// Sortear la grilla "gv_busquedaporrol".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_busquedaporrol_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["tablaConsultas_Rol"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gv_busquedaporrol.DataSource = dt;
                gv_busquedaporrol.DataBind();
            }
        }

        /// <summary>
        /// Sortear la grilla "gv_busquedaavanzada".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_busquedaavanzada_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["tablaConsultas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gv_busquedaavanzada.DataSource = dt;
                gv_busquedaavanzada.DataBind();
            }
        }
    }
}

