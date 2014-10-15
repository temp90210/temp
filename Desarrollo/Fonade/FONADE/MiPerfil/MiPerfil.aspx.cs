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


namespace Fonade.FONADE.MiPerfil
{
    public partial class MiPerfil : Negocio.Base_Page
    {
        String CodTipoInstitucion;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("MI PERFIL");
                switch (usuario.CodGrupo)
                {
                    case 1:
                        //gerente Admin
                        ingresarGerenteAdmin();
                        PanelGerenteAdmin.Visible = true;

                        break;
                    case 8:/* Call Center*/
                        if (!IsPostBack)
                            Redirect(null, "~/FONADE/MiPerfil/CambiarClave.aspx", "_Blank", "width=580,height=300,toolbar=no, scrollbars=no, resizable=no");
                        //return;
                        break;
                    case 2:
                        if (!IsPostBack)
                            Redirect(null, "~/FONADE/MiPerfil/CambiarClave.aspx", "_Blank", "width=580,height=300,toolbar=no, scrollbars=no, resizable=no");
                        //return;
                        break;
                    case 4:
                        //Jefe de Unidad
                        ingresarDatosJefeUnidad();
                        PanelJefeUnidad.Visible = true;
                        string txtsql = "SELECT CodTipoInstitucion, NomInstitucion, Nit, RegistroIcfes, FechaRegistro, CodCiudad, Direccion, Telefono, Fax, Website FROM Institucion WHERE Id_Institucion = " + usuario.CodInstitucion;
                        var dt = consultas.ObtenerDataTable(txtsql, "text");
                        if (dt.Rows.Count > 0) { CodTipoInstitucion = dt.Rows[0]["CodTipoInstitucion"].ToString(); }
                        dt = null;
                        txtsql = null;
                        if (CodTipoInstitucion == "33") // si es 33, NO se muestran los campos "ICFES, etc".
                        {
                            tr_ICFES.Visible = false;
                            tr_FECHA_REGISTRO.Visible = false;
                        }
                        //else
                        //{
                        //    tr_ICFES.Visible = tr_cargo;
                        //    tr_FECHA_REGISTRO.Visible = tr_cargo;
                        //}
                        break;
                    case 5:
                        //asesor
                        ingresarDatosAsesor();
                        PanelAsesor.Visible = true;
                        break;
                    case 6:
                        //Emprendedor
                        ingresarDatosEmprendedor();
                        PanelEmprendedor.Visible = true;
                        break;
                    case 9: /*A*/
                        //gerente evaluador 
                        //ingresardatosUsGeneral();
                        ingresardatosUs_GruposSelectos();
                        PanelGeneral.Visible = true;
                        break;
                    case 10: /*B*/
                        //coordinador evaluador
                        //ingresardatosUsGeneral();
                        ingresardatosUs_GruposSelectos();
                        PanelGeneral.Visible = true;
                        break;
                    case 11:
                        //Evaluador
                        ingresarDatosEvaluador();
                        PanelEvaluador.Visible = true;
                        break;
                    case 12: /*C*/
                        //gerente interventor
                        //ingresardatosUsGeneral();
                        ingresardatosUs_GruposSelectos();
                        PanelGeneral.Visible = true;
                        break;
                    case 13: /*D*/
                        //coordinador interventor
                        //ingresardatosUsGeneral();
                        ingresardatosUs_GruposSelectos();
                        PanelGeneral.Visible = true;
                        break;
                    case 14:
                        //Interventor.

                        #region Leer líneas de código.

                        //Mauricio Arias Olave.(13/08/2014): Se ajusta el código existente para agreagr campos reportados
                        //como faltantes, así como ajustar el perfil del "interventor".

                        tr_cedula.Visible = true;
                        tr_numIdentificacion.Visible = false;
                        tr_cargo.Visible = true;//estaba en "false".
                        tr_direccion.Visible = true;
                        tr_Celular.Visible = true;
                        tr_Departamento.Visible = true;
                        tr_Ciudad.Visible = true;
                        pnl_exp_interventor.Visible = true;

                        //Cargar los estudios del inteventor.
                        CargarEstudiosInterventor();

                        #endregion

                        //ingresarDatosAsesor();
                        //PanelAsesor.Visible = true;
                        ingresardatosUsGeneral();
                        PanelGeneral.Visible = true;

                        break;
                    case 15:
                        //Fiduciaria.
                        ////ingresarDatosAsesor();
                        //PanelGeneral.Visible = true;
                        //ingresarUsuarioFiduciaria();
                        //PanelGeneral.Visible = true; 
                        if (!IsPostBack)
                            Redirect(null, "~/FONADE/MiPerfil/CambiarClave.aspx", "_Blank", "width=580,height=300,toolbar=no, scrollbars=no, resizable=no");
                        //return;
                        break;
                }
            }
        }

        protected void ingresarDatosAsesor()
        {
            try
            {
                int CodigoCiudad = 111;
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_cargarAsesorMiPerfil", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_contacto", usuario.IdContacto);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                r.Read();
                l_nombre2.Text = Convert.ToString(r["Nombres"]);
                l_apellido2.Text = Convert.ToString(r["Apellidos"]);
                l_identificacion2.Text = Convert.ToString(r["Identificacion"]);
                l_email2.Text = Convert.ToString(r["Email"]);
                tx_experiencia2.Text = Convert.ToString(r["Experiencia"]);
                ddl_decalracion2.SelectedValue = Convert.ToString(r["Dedicacion"]);
                tx_hojadevida2.Text = Convert.ToString(r["HojaVida"]);
                tx_interes2.Text = Convert.ToString(r["Intereses"]);
                try
                {
                    CodigoCiudad = Convert.ToInt32(r["LugarExpedicionDI"]);
                }
                catch (Exception)
                { }

                con.Close();
                con.Dispose();
                cmd.Dispose();


                var query = from dept in consultas.Db.departamentos
                            select new
                            {
                                Id_dpto = dept.Id_Departamento,
                                Nombre_dpto = dept.NomDepartamento,
                            };
                ddl_departamento2.DataSource = query.ToList();
                ddl_departamento2.DataTextField = "Nombre_dpto";
                ddl_departamento2.DataValueField = "Id_dpto";
                ddl_departamento2.DataBind();
                seleccionarDepartamento(ddl_departamento2, CodigoCiudad);
                llenarCiudad(ddl_ciudad2, ddl_departamento2);
                ddl_ciudad2.SelectedValue = CodigoCiudad.ToString();

            }
            catch (Exception)
            { }

        }

        protected void ingresarDatosEvaluador()
        {
            string banco = "";
            string tipocuenta = "";
            try
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_cargarMiPerfilEvaluador", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_contacto", usuario.IdContacto);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                r.Read();
                l_nombre6.Text = Convert.ToString(r["Nombres"]);
                l_apellidos6.Text = Convert.ToString(r["Apellidos"]);
                l_identificacion6.Text = Convert.ToString(r["identificacion"]);
                l_persona6.Text = Convert.ToString(r["Persona"]);
                l_email6.Text = Convert.ToString(r["email"]);
                txt_direccion6.Text = Convert.ToString(r["Direccion"]);
                txt_telefono6.Text = Convert.ToString(r["Telefono"]);
                txt_fax6.Text = Convert.ToString(r["fax"]);
                txt_numcuenta6.Text = Convert.ToString(r["Cuenta"]);
                txt_planes6.Text = Convert.ToString(r["MaximoPlanes"]);
                txt_expgeneral6.Text = Convert.ToString(r["Experiencia"]);
                txt_expintereses6.Text = Convert.ToString(r["Intereses"]);
                txt_secprincipal6.Text = Convert.ToString(r["ExperienciaPrincipal"]);
                txt_secsecundario6.Text = Convert.ToString(r["ExperienciaSecundaria"]);
                txt_hojadevida6.Text = Convert.ToString(r["HojaVida"]);
                banco = Convert.ToString(r["codBanco"]);
                tipocuenta = Convert.ToString(r["CodTipoCuenta"]);
                con.Close();
                con.Dispose();
                cmd.Dispose();
            }
            catch (Exception)
            { }
            llenarbancos(ddl_banco6);
            ddl_banco6.SelectedValue = banco;
            llenartipocuenta(ddl_tipocuenta6);
            ddl_tipocuenta6.SelectedValue = tipocuenta;
            llenarsectores(ddl_secprincipal6);
            ddl_secprincipal6.SelectedValue = llenarddlsector("P");
            llenarsectores(ddl_secsecundario6);
            ddl_secsecundario6.SelectedValue = llenarddlsector("S");

        }

        protected void llenarbancos(DropDownList lista)
        {
            var query = from bn in consultas.Db.Bancos
                        select new
                        {
                            Id_banco = bn.Id_Banco,
                            Nombre_banco = bn.nomBanco,
                        };
            lista.DataSource = query.ToList();
            lista.DataTextField = "Nombre_banco";
            lista.DataValueField = "Id_banco";
            lista.DataBind();
        }

        protected string llenarddlsector(string tiposector)
        {
            string retorno = "11";
            try
            {
                var query = (from x in consultas.Db.EvaluadorSectors
                             where x.CodContacto == usuario.IdContacto
                             && x.Experiencia == tiposector
                             select new
                             {
                                 x.CodSector,
                             }).FirstOrDefault();
                retorno = query.CodSector.ToString();
            }
            catch (Exception)
            { }

            return retorno;
        }

        protected void llenartipocuenta(DropDownList lista)
        {
            var query = from tc in consultas.Db.TipoCuentas
                        select new
                        {
                            Id_tc = tc.Id_TipoCuenta,
                            Nombre_tc = tc.NomTipoCuenta,
                        };
            lista.DataSource = query.ToList();
            lista.DataTextField = "Nombre_tc";
            lista.DataValueField = "Id_tc";
            lista.DataBind();
        }

        protected void llenarsectores(DropDownList lista)
        {
            var query = (from sc in consultas.Db.Sectors
                         select new
                         {
                             Id_sec = sc.Id_Sector,
                             Nombre_sec = sc.NomSector,
                         }).OrderBy(e => e.Nombre_sec);

            lista.DataSource = query.ToList();
            lista.DataTextField = "Nombre_sec";
            lista.DataValueField = "Id_sec";
            lista.DataBind();
        }

        protected void ingresarDatosJefeUnidad()
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_CargarJefeUnidadMiPerfil", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_contacto", usuario.IdContacto);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                r.Read();
                int CodigoCiudadUnidad = 111;
                int CodigoCiudadEmprendedor = 111;
                try
                {
                    l_nomcentro3.Text = Convert.ToString(r["NomInstitucionI"]);
                    l_nit3.Text = Convert.ToString(r["NitI"]);
                    if (tr_ICFES.Visible && tr_FECHA_REGISTRO.Visible)
                    {
                        tx_icfes3.Text = Convert.ToString(r["RegistroIcfesI"]);
                        tx_fregistro2.Text = Convert.ToDateTime(r["FechaRegistroI"]).ToString("dd/MM/yyyy");
                    }
                    l_correspondencia3.Text = Convert.ToString(r["DireccionI"]);
                    tx_telunidad3.Text = Convert.ToString(r["TelefonoI"]);
                    tx_faxunidad3.Text = Convert.ToString(r["FaxI"]);
                    tx_sitioweb3.Text = Convert.ToString(r["WebsiteI"]);
                    l_nombre3.Text = Convert.ToString(r["NombresC"]);
                    l_apellido3.Text = Convert.ToString(r["Apellidos"]);
                    l_identificacion3.Text = Convert.ToString(r["IdentificacionC"]);
                    l_email3.Text = Convert.ToString(r["EmailC"]);
                    tx_cargo3.Text = Convert.ToString(r["CargoC"]);
                    tx_telefono3.Text = Convert.ToString(r["TelefonoC"]);
                    tx_fax3.Text = Convert.ToString(r["FaxC"]);
                    try
                    {
                        CodigoCiudadUnidad = Convert.ToInt32(r["CodCiudadI"]);
                    }
                    catch (Exception)
                    { }
                    try
                    {
                        CodigoCiudadEmprendedor = Convert.ToInt32(r["LugarExpedicionDIC"]);
                    }
                    catch (Exception)
                    { }

                }
                catch (Exception)
                { }

                con.Close();
                con.Dispose();
                cmd.Dispose();


                var query = from dept in consultas.Db.departamentos
                            select new
                            {
                                Id_dpto1 = dept.Id_Departamento,
                                Nombre_dpto1 = dept.NomDepartamento,
                            };
                ddl_deparunidad3.DataSource = query.ToList();
                ddl_deparunidad3.DataTextField = "Nombre_dpto1";
                ddl_deparunidad3.DataValueField = "Id_dpto1";
                ddl_deparunidad3.DataBind();
                seleccionarDepartamento(ddl_deparunidad3, CodigoCiudadUnidad);
                llenarCiudad(ddl_ciudadunidad3, ddl_deparunidad3);
                ddl_ciudadunidad3.SelectedValue = CodigoCiudadUnidad.ToString();

                var query2 = from dept2 in consultas.Db.departamentos
                             select new
                             {
                                 Id_dpto2 = dept2.Id_Departamento,
                                 Nombre_dpto2 = dept2.NomDepartamento,
                             };
                ddl_departamento3.DataSource = query2.ToList();
                ddl_departamento3.DataTextField = "Nombre_dpto2";
                ddl_departamento3.DataValueField = "Id_dpto2";
                ddl_departamento3.DataBind();
                seleccionarDepartamento(ddl_departamento3, CodigoCiudadEmprendedor);
                llenarCiudad(ddl_ciudad3, ddl_departamento3);
                ddl_ciudad3.SelectedValue = CodigoCiudadEmprendedor.ToString();

            }
            catch (Exception)
            { }

        }

        protected void ingresarGerenteAdmin()
        {
            l_nombres5.Text = usuario.Nombres;
            l_apellidos5.Text = usuario.Apellidos;
            l_email5.Text = usuario.Email;
            l_identificacion5.Text = usuario.Identificacion.ToString();
        }

        protected void ingresardatosUsGeneral()
        {
            var query = (from c in consultas.Db.Contactos
                         where c.Id_Contacto == usuario.IdContacto
                         select new
                         {
                             c,
                         }).FirstOrDefault();

            l_nombre1.Text = usuario.Nombres;
            l_apellido1.Text = usuario.Apellidos;
            l_email1.Visible = true;
            l_email1.Text = usuario.Email;
            l_identificacion1.Text = usuario.Identificacion.ToString();
            tx_cargo1.Text = query.c.Cargo;
            tx_telefono1.Text = query.c.Telefono;
            tx_fax1.Text = query.c.Fax;

            #region Campos agregados/ajustes según información reportada en el Excel "Checklist_PerfilInterventor".

            lbl_Cedula.Text = query.c.Identificacion.ToString();
            lbl_Email.Visible = true;
            lbl_Email.Text = "Email:";
            txtDireccion.Text = query.c.Direccion;

            #region Departamento y ciudad.

            var queryDeptos = (from dept in consultas.Db.departamentos
                               select new
                               {
                                   Id_dpto1 = dept.Id_Departamento,
                                   Nombre_dpto1 = dept.NomDepartamento,
                               }).ToList();
            dd_deptos.DataSource = queryDeptos;
            dd_deptos.DataTextField = "Nombre_dpto1";
            dd_deptos.DataValueField = "Id_dpto1";
            dd_deptos.DataBind();
            llenarCiudad(dd_ciudad, dd_deptos);
            dd_deptos.SelectedValue = query.c.Ciudad.CodDepartamento.ToString();
            seleccionarDepartamento(dd_ciudad, query.c.Ciudad.Id_Ciudad);

            #endregion

            #region Cargar celular y otros datos.

            var rstContacto = consultas.ObtenerDataTable("SELECT * FROM Interventor WHERE codContacto = " + usuario.IdContacto, "text");

            if (rstContacto.Rows.Count > 0)
            {
                try
                {
                    txtCelular.Text = rstContacto.Rows[0]["Celular"].ToString();
                    txt_exp_sector_principal.Text = rstContacto.Rows[0]["ExperienciaPrincipal"].ToString();
                    txt_exp_sector_secundario.Text = rstContacto.Rows[0]["ExperienciaSecundaria"].ToString();
                }
                catch { }
            }

            rstContacto = null;

            #endregion

            #endregion

            txt_exp_int_profesional.Text = query.c.Experiencia;
            txt_int_res_HV.Text = query.c.HojaVida;
            txt_exp_int_experi_intere.Text = query.c.Intereses;

            llenarsectores(dd_sector_princ_int);
            //dd_sector_princ_int.SelectedValue = llenarddlsector("P");
            llenarsectores(dd_sector_second_int);
            //dd_sector_second_int.SelectedValue = llenarddlsector("S");

            //Sector Principal de interventor.
            var a = consultas.ObtenerDataTable("SELECT codSector FROM InterventorSector WHERE codContacto='" + usuario.IdContacto + "' and Experiencia='P'", "text");
            if (a.Rows.Count > 0) { dd_sector_princ_int.SelectedValue = a.Rows[0]["codSector"].ToString(); }
            a = null;

            //Sector Principal de interventor.
            var b = consultas.ObtenerDataTable("SELECT codSector FROM InterventorSector WHERE codContacto='" + usuario.IdContacto + "' and Experiencia='S'", "text");
            if (b.Rows.Count > 0) { dd_sector_second_int.SelectedValue = b.Rows[0]["codSector"].ToString(); }
            b = null;
        }

        protected void ingresarDatosEmprendedor()
        {
            try
            {
                int CodigoCiudadNacimiento = 111;
                int CodigoCiudadexped = 111;
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_cargarMiPerfilEmprendedor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_contacto", usuario.IdContacto);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();
                r.Read();
                l_nombre4.Text = Convert.ToString(r["Nombres"]);
                l_apellidos4.Text = Convert.ToString(r["Apellidos"]);
                l_identificacion4.Text = Convert.ToString(r["Identificacion"]);
                l_email4.Text = Convert.ToString(r["Email"]);
                tx_fechanacimiento4.Text = Convert.ToDateTime(r["FechaNacimiento"]).ToString("dd/MM/yyyy");
                tx_telefono4.Text = Convert.ToString(r["Telefono"]);
                ddl_genero4.SelectedValue = Convert.ToString(r["Genero"]);
                try
                {
                    CodigoCiudadNacimiento = Convert.ToInt32(r["CodCiudad"]);

                }
                catch (Exception)
                { }
                try
                {
                    CodigoCiudadexped = Convert.ToInt32(r["LugarExpedicionDI"]);
                }
                catch (Exception)
                { }

                con.Close();
                con.Dispose();
                cmd.Dispose();


                var query = from dept in consultas.Db.departamentos
                            select new
                            {
                                Id_dpto1 = dept.Id_Departamento,
                                Nombre_dpto1 = dept.NomDepartamento,
                            };
                ddl_depexped4.DataSource = query.ToList();
                ddl_depexped4.DataTextField = "Nombre_dpto1";
                ddl_depexped4.DataValueField = "Id_dpto1";
                ddl_depexped4.DataBind();
                seleccionarDepartamento(ddl_depexped4, CodigoCiudadexped);
                llenarCiudad(dd_ciuexp4, ddl_depexped4);
                dd_ciuexp4.SelectedValue = CodigoCiudadexped.ToString();

                var query2 = from dept2 in consultas.Db.departamentos
                             select new
                             {
                                 Id_dpto2 = dept2.Id_Departamento,
                                 Nombre_dpto2 = dept2.NomDepartamento,
                             };
                ddl_departamento4.DataSource = query2.ToList();
                ddl_departamento4.DataTextField = "Nombre_dpto2";
                ddl_departamento4.DataValueField = "Id_dpto2";
                ddl_departamento4.DataBind();
                seleccionarDepartamento(ddl_departamento4, CodigoCiudadNacimiento);
                llenarCiudad(ddl_ciudad4, ddl_departamento4);
                ddl_ciudad4.SelectedValue = CodigoCiudadNacimiento.ToString();

            }
            catch (Exception)
            { }

        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 08/04/2014: Este método se aplica a los siguientes grupos:
        /// 9 = Gerente Evaluador
        /// 10 = Coordinador de Evaluadores
        /// 12 = Gerente Interventor
        /// 13 = Coordinar de Interventor
        /// </summary>
        protected void ingresardatosUs_GruposSelectos()
        {
            //Ocultar los demás campos.
            tb_PanelAsesor.Visible = false;
            tb_PanelJefeUnidad.Visible = false;
            tb_PanelEmprendedor.Visible = false;
            tb_PanelGerenteAdmin.Visible = false;
            tb_PanelEvaluador.Visible = false;

            //Consulta.
            var query = (from c in consultas.Db.Contactos
                         where c.Id_Contacto == usuario.IdContacto
                         select new
                         {
                             c,
                         }).FirstOrDefault();

            l_nombre1.Text = usuario.Nombres;
            l_apellido1.Text = usuario.Apellidos;
            l_identificacion1.Text = usuario.Identificacion.ToString();
            l_email1.Text = usuario.Email;
            tx_cargo1.Text = query.c.Cargo;
            tx_telefono1.Text = query.c.Telefono;
            tx_fax1.Text = query.c.Fax;

        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 08/04/2014: Este método se aplica a los siguientes grupos:
        /// 9 = Gerente Evaluador
        /// 10 = Coordinador de Evaluadores
        /// 12 = Gerente Interventor
        /// 13 = Coordinar de Interventor
        /// </summary>
        protected void ingresarUsuarioFiduciaria()
        {
            //Ocultar los demás campos.
            tb_PanelAsesor.Visible = false;
            tb_PanelJefeUnidad.Visible = false;
            tb_PanelEmprendedor.Visible = false;
            tb_PanelGerenteAdmin.Visible = false;
            tb_PanelEvaluador.Visible = false;

            //Consulta.
            var query = (from c in consultas.Db.Contactos
                         where c.Id_Contacto == usuario.IdContacto
                         select new
                         {
                             c,
                         }).FirstOrDefault();

            l_nombre1.Text = usuario.Nombres;
            l_apellido1.Text = usuario.Apellidos;
            l_identificacion1.Text = usuario.Identificacion.ToString();
            l_email1.Text = usuario.Email;
            tx_cargo1.Text = query.c.Cargo;
            tx_telefono1.Text = query.c.Telefono;
            tx_fax1.Text = query.c.Fax;

        }

        protected void ddl_departamento2_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarCiudad(ddl_ciudad2, ddl_departamento2);
        }

        protected void seleccionarDepartamento(DropDownList dtoaSeleccionar, int codigoCiudad)
        {
            try
            {
                var query = (from CIud in consultas.Db.Ciudads
                             where CIud.Id_Ciudad == codigoCiudad
                             select new
                             {
                                 Id_departamento = CIud.CodDepartamento,
                             }).FirstOrDefault();
                dtoaSeleccionar.SelectedValue = Convert.ToString(query.Id_departamento);
            }
            catch (Exception)
            { }

        }

        protected void llenarCiudad(DropDownList listaciudad, DropDownList departamentolist)
        {
            try
            {
                var query = from CIud in consultas.Db.Ciudads
                            where CIud.CodDepartamento == Convert.ToInt32(departamentolist.SelectedValue)
                            select new
                            {
                                Id_ciudad = CIud.Id_Ciudad,
                                Nombre_ciudad = CIud.NomCiudad,
                            };
                listaciudad.DataSource = query.ToList();
                listaciudad.DataTextField = "Nombre_ciudad";
                listaciudad.DataValueField = "Id_ciudad";
                listaciudad.DataBind();
            }
            catch (Exception)
            { }

        }

        protected void lds_estudios_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var query = from P in consultas.Db.MD_VerEstudiosAsesor(usuario.IdContacto)
                            select P;
                e.Result = query;
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/08/2014.
        /// Cargar el GridView "gv_infoAcademic_Interventor" con los estudios del inteventor.
        /// </summary>
        private void CargarEstudiosInterventor()
        {
            //Inicializar variables.
            String txtSQL;
            DataTable RS = new DataTable();

            try
            {
                txtSQL = " SELECT CE.Id_ContactoEstudio, CE.TituloObtenido, CE.AnoTitulo,CE.Finalizado, CE.Institucion, CE.CodCiudad, C.NomCiudad, D.NomDepartamento, NE.NomNivelEstudio" +
                         " FROM ContactoEstudio CE, Ciudad C, Departamento D, NivelEstudio NE" +
                         " WHERE CE.CodCiudad = C.ID_Ciudad" +
                         " AND C.CodDepartamento = D.Id_Departamento" +
                         " AND CE.CodNivelEstudio = NE.Id_NivelEstudio" +
                         " AND codcontacto = " + usuario.IdContacto +
                         " ORDER BY CE.Finalizado,CE.AnoTitulo Desc";

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                this.gv_infoAcademic_Interventor.DataSource = RS;
                this.gv_infoAcademic_Interventor.DataBind();
            }
            catch { }
        }

        protected void btn_actualizar2_Click(object sender, EventArgs e)
        {
            updateAsesor();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información actualizada exitosamente!'); document.location=('MiPerfil.aspx');", true);
        }

        protected void Eliminar_Estudios_Realizados(object sender, CommandEventArgs e)
        {
            try
            {
                int codigo_estudio = Convert.ToInt32(e.CommandArgument);
                var query = (from Estudio in consultas.Db.ContactoEstudios
                             where Estudio.FlagIngresadoAsesor == 1
                             & Estudio.Id_ContactoEstudio == codigo_estudio
                             select Estudio.FlagIngresadoAsesor).Count();

                if (query == 0)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("MD_EliminarEstudioRealizado", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudio", codigo_estudio);
                    SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                    con.Open();
                    cmd2.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd2.Dispose();
                    cmd.Dispose();
                    this.gvestudiosrealizadosasesor.DataBind();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No ha sido posible eliminar esta información académica')", true);
                }
            }
            catch (Exception)
            { }
        }

        protected void Eliminar_Estudios_Emprendedor(object sender, CommandEventArgs e)
        {
            try
            {
                int codigo_estudio = Convert.ToInt32(e.CommandArgument);
                var query = (from Estudio in consultas.Db.ContactoEstudios
                             where Estudio.FlagIngresadoAsesor == 1
                             & Estudio.Id_ContactoEstudio == codigo_estudio
                             select Estudio.FlagIngresadoAsesor).Count();

                if (query == 0)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("MD_EliminarEstudioRealizado", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_estudio", codigo_estudio);
                    SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                    con.Open();
                    cmd2.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd2.Dispose();
                    cmd.Dispose();
                    this.gvestudiosemprendedor.DataBind();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No ha sido posible eliminar esta información académica')", true);
                }
            }
            catch (Exception)
            { }
        }

        protected void eliminararchivos()
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Funciona!')", true);
        }

        protected void btn_actualizar4_Click(object sender, EventArgs e)
        {
            updateEmprendedor();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información actualizada exitosamente!'); document.location=('MiPerfil.aspx');", true);
            //Response.Redirect("MiPerfil.aspx");
        }

        protected void ddl_departamento3_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarCiudad(ddl_ciudad3, ddl_departamento3);
        }

        protected void ddl_deparunidad3_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarCiudad(ddl_ciudadunidad3, ddl_deparunidad3);
        }

        protected void ddl_depexped4_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarCiudad(dd_ciuexp4, ddl_depexped4);
        }

        protected void dd_deptos_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarCiudad(dd_ciudad, dd_deptos);
        }

        protected void ddl_departamento4_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarCiudad(ddl_ciudad4, ddl_departamento4);
        }

        protected void Image1_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void btn_actualizar3_Click(object sender, EventArgs e)
        {
            updateJefeUnidad();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información actualizada exitosamente!'); document.location=('MiPerfil.aspx');", true);
            //Response.Redirect("MiPerfil.aspx");
        }

        protected void updateJefeUnidad()
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_Update_JefeUnidad", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@icfes", tx_icfes3.Text);
                cmd.Parameters.AddWithValue("@fechaderegistro", Convert.ToDateTime(tx_fregistro2.Text).ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@telefonoInst", tx_telunidad3.Text);
                cmd.Parameters.AddWithValue("@faxInst", tx_faxunidad3.Text);
                cmd.Parameters.AddWithValue("@web", tx_sitioweb3.Text);
                cmd.Parameters.AddWithValue("@ciudadInst", Convert.ToInt32(ddl_ciudadunidad3.SelectedValue));
                cmd.Parameters.AddWithValue("@idInstitucion", usuario.CodInstitucion);
                cmd.Parameters.AddWithValue("@cargo", tx_cargo3.Text);
                cmd.Parameters.AddWithValue("@telefono", tx_telefono3.Text);
                cmd.Parameters.AddWithValue("@fax", tx_fax3.Text);
                cmd.Parameters.AddWithValue("@ciudad", Convert.ToInt32(ddl_ciudad3.SelectedValue));
                cmd.Parameters.AddWithValue("@idcontacto", usuario.IdContacto);
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();

            }
            catch (Exception)
            { }
        }

        protected void updateEmprendedor()
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_Update_Emprendedor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ciudadexpedicion", Convert.ToInt32(dd_ciuexp4.SelectedValue));
                cmd.Parameters.AddWithValue("@telefono", tx_telefono4.Text);
                cmd.Parameters.AddWithValue("@ciudad", Convert.ToInt32(ddl_ciudad4.SelectedValue));
                cmd.Parameters.AddWithValue("@idcontacto", usuario.IdContacto);
                cmd.Parameters.AddWithValue("@genero", ddl_genero4.SelectedValue);
                cmd.Parameters.AddWithValue("@fechanacimiento", Convert.ToDateTime(tx_fechanacimiento4.Text).ToString("yyyy-MM-dd"));
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();

            }
            catch (Exception)
            { }
        }

        protected void updateAsesor()
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_Update_Asesor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dedicacion", ddl_decalracion2.SelectedValue);
                cmd.Parameters.AddWithValue("@experiencia", tx_experiencia2.Text);
                cmd.Parameters.AddWithValue("@hojadevida", tx_hojadevida2.Text);
                cmd.Parameters.AddWithValue("@idcontacto", usuario.IdContacto);
                cmd.Parameters.AddWithValue("@intereses", tx_interes2.Text);
                cmd.Parameters.AddWithValue("@ciudad", Convert.ToInt32(ddl_ciudad2.SelectedValue));
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();

            }
            catch (Exception)
            { }
        }

        protected void updateUsGeneral()
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_Update_UsGeneral", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cargo", tx_cargo1.Text);
                cmd.Parameters.AddWithValue("@telefono", tx_telefono1.Text);
                cmd.Parameters.AddWithValue("@fax", tx_fax1.Text);
                cmd.Parameters.AddWithValue("@idcontacto", usuario.IdContacto);
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
            }
            catch (Exception)
            { }
        }

        protected void updateEvaluador()
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_Update_Evaluador", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@telefono", txt_telefono6.Text);
                cmd.Parameters.AddWithValue("@direccion", txt_direccion6.Text);
                cmd.Parameters.AddWithValue("@experiencia", txt_expgeneral6.Text);
                cmd.Parameters.AddWithValue("@intereses", txt_expintereses6.Text);
                cmd.Parameters.AddWithValue("@hojavida", txt_hojadevida6.Text);
                cmd.Parameters.AddWithValue("@fax", txt_fax6.Text);
                cmd.Parameters.AddWithValue("@codBanco", Convert.ToInt32(ddl_banco6.SelectedValue));
                cmd.Parameters.AddWithValue("@CodTipocuenta", Convert.ToInt32(ddl_tipocuenta6.SelectedValue));
                cmd.Parameters.AddWithValue("@txtNumCuenta", txt_numcuenta6.Text);
                cmd.Parameters.AddWithValue("@MaximoPlanes", Convert.ToInt16(txt_planes6.Text));
                cmd.Parameters.AddWithValue("@txtExpPrincipal", txt_secprincipal6.Text);
                cmd.Parameters.AddWithValue("@txtExpSecundaria", txt_secsecundario6.Text);
                cmd.Parameters.AddWithValue("@codSectorPri", Convert.ToInt32(ddl_secprincipal6.SelectedValue));
                cmd.Parameters.AddWithValue("@codSectorSec", Convert.ToInt32(ddl_secsecundario6.SelectedValue));
                cmd.Parameters.AddWithValue("@idcontacto", usuario.IdContacto);
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
            }
            catch (Exception)
            { }
        }

        protected void btn_actualizar1_Click(object sender, EventArgs e)
        {
            updateUsGeneral();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información actualizada exitosamente!'); document.location=('MiPerfil.aspx');", true);
        }

        protected void btn_actualizar6_Click(object sender, EventArgs e)
        {
            updateEvaluador();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información actualizada exitosamente!'); document.location=('MiPerfil.aspx');", true);
        }

        protected void Img_Btn_Nuevo_Doc_interventor_Click(object sender, ImageClickEventArgs e)
        {
            Session["Accion_Docs"] = "Crear";
            Redirect(null, "CatalogoAnexarInterventor.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        protected void Img_Btn_Ver_Doc_interventor_Click(object sender, ImageClickEventArgs e)
        {
            Session["Accion_Docs"] = "Vista";
            Redirect(null, "CatalogoAnexarInterventor.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }
    }
}