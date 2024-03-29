﻿using System;
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

namespace Fonade.FONADE.Beneficiario
{
    public partial class Beneficiario : Negocio.Base_Page
    {
        public int codigo;
        public string sucur1 = "0";
        string txtSQL;
        Int32 CodEstadoProyecto;
        Int64 CodProyecto;

        protected void Page_Load(object sender, EventArgs e)
        {
            string tipoid1 = "0", tiposoc1 = "0", tiporet1 = "0", ciud1 = "0", banc1 = "0", cuent1 = "0";
            codigo = Convert.ToInt32(Request.QueryString["LoadCode"]);

            if (!IsPostBack)
            {
                txtSQL = "SELECT CodEstado, CodProyecto FROM Proyecto P, ProyectoContacto PC WHERE P.id_proyecto = PC.CodProyecto AND PC.Inactivo = 0 AND PC.CodContacto = " + usuario.IdContacto;
                var RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    CodEstadoProyecto = Int32.Parse(RS.Rows[0]["CodEstado"].ToString());
                    CodProyecto = Int64.Parse(RS.Rows[0]["CodProyecto"].ToString());
                }

                if (CodEstadoProyecto < Constantes.CONST_Ejecucion || CodEstadoProyecto >= Constantes.CONST_Asignado_para_acreditacion)
                {
                    pnl_Creacion.Visible = false;
                    pnl_Validacion.Visible = true;
                }
                else
                {
                    pnl_Creacion.Visible = true;
                    pnl_Validacion.Visible = false;

                    if (codigo == 0)
                    {
                        btn_crear.Visible = true;
                        btn_crear.Enabled = true;
                        lbl_Titulo.Text = void_establecerTitulo("CREAR BENEFICIARIO");
                    }
                    else
                    {
                        btn_Actualizar.Visible = true;
                        btn_Actualizar.Enabled = true;
                        lbl_Titulo.Text = void_establecerTitulo("ACTUALIZAR BENEFICIARIO");

                        var query = (from pBenef in consultas.Db.PagoBeneficiarios
                                     where pBenef.Id_PagoBeneficiario == codigo
                                     select new
                                     {
                                         pBenef
                                     }).FirstOrDefault();
                        tipoid1 = query.pBenef.CodTipoIdentificacion.ToString();
                        tiposoc1 = query.pBenef.TipoRazonSocial.ToString();
                        tiporet1 = query.pBenef.CodPagoTipoRetencion.ToString();
                        ciud1 = query.pBenef.CodCiudad.ToString();
                        banc1 = query.pBenef.CodPagoBanco.ToString();
                        cuent1 = query.pBenef.TipoCuenta.ToString();
                        sucur1 = query.pBenef.CodPagoSucursal.ToString();
                        txt_apellidos.Text = query.pBenef.Apellido;
                        txt_direccion.Text = query.pBenef.Direccion;
                        txt_email.Text = query.pBenef.Email;
                        txt_fax.Text = query.pBenef.Fax;
                        txt_ncuenta.Text = query.pBenef.NumCuenta;
                        txt_nombres.Text = query.pBenef.Nombre;
                        txt_numdocumento.Text = query.pBenef.NumIdentificacion;
                        txt_rsocial.Text = query.pBenef.RazonSocial;
                        txt_telefono.Text = query.pBenef.Telefono;
                    }

                    traerTipoIdentific(tipoid1);
                    traertsociedad(tiposoc1);
                    traertretencion(tiporet1);
                    traerCiudad(ciud1);
                    traerbanco(banc1, sucur1);
                    ddl_tcuenta.SelectedValue = cuent1;
                }
            }
        }

        protected void traerTipoIdentific(string seleccion)
        {
            var query = (from TipoId in consultas.Db.TipoIdentificacions
                         select new
                         {
                             id_ident = TipoId.Id_TipoIdentificacion,
                             nomb_id = TipoId.NomTipoIdentificacion,
                         });
            ddl_tipoid.DataSource = query.ToList();
            ddl_tipoid.DataTextField = "nomb_id";
            ddl_tipoid.DataValueField = "id_ident";
            ddl_tipoid.DataBind();
            ddl_tipoid.SelectedValue = seleccion;
        }

        protected void traertsociedad(string seleccion)
        {
            var query = (from tabla in consultas.Db.TipoSociedads
                         select new
                         {
                             id_Tabla = tabla.Id_TipoSociedad,
                             nomb_tabla = tabla.NomTipoSociedad,
                         });
            ddl_tsociedad.DataSource = query.ToList();
            ddl_tsociedad.DataTextField = "nomb_tabla";
            ddl_tsociedad.DataValueField = "id_Tabla";
            ddl_tsociedad.DataBind();
            ddl_tsociedad.SelectedValue = seleccion;
        }

        protected int codigoempresa()
        {
            var query = (from empr in consultas.Db.Empresas
                         join PrC in consultas.Db.ProyectoContactos
                         on empr.codproyecto equals PrC.CodProyecto
                         where PrC.Inactivo == Convert.ToBoolean(0)
                         & PrC.CodContacto == usuario.IdContacto
                         select new
                         {
                             idEmpresa = empr.id_empresa,
                         }).FirstOrDefault();
            int empresa = query.idEmpresa;
            return empresa;
        }

        protected void traertretencion(string seleccion)
        {
            var query = (from tabla in consultas.Db.PagoTipoRetencions
                         select new
                         {
                             id_Tabla = tabla.Id_PagoTipoRetencion,
                             nomb_tabla = tabla.NomPagoTipoRetencion,
                         });
            ddl_tretencion.DataSource = query.ToList();
            ddl_tretencion.DataTextField = "nomb_tabla";
            ddl_tretencion.DataValueField = "id_Tabla";
            ddl_tretencion.DataBind();
            ddl_tretencion.SelectedValue = seleccion;
        }

        protected void traerCiudad(string seleccion)
        {
            var query = (from Ciud in consultas.Db.Ciudads
                         from depto in consultas.Db.departamentos
                         where Ciud.CodFiduciaria != null
                         & depto.Id_Departamento == Ciud.CodDepartamento
                         select new
                         {
                             Id_Ciud = Ciud.Id_Ciudad,
                             Nombre_Ciud = Ciud.NomCiudad + " (" + depto.NomDepartamento + ")",
                         }).OrderBy(x => x.Nombre_Ciud); ;
            ddl_ciudadpago.DataSource = query.ToList();
            ddl_ciudadpago.DataTextField = "Nombre_Ciud";
            ddl_ciudadpago.DataValueField = "Id_Ciud";
            ddl_ciudadpago.DataBind();
            ddl_ciudadpago.SelectedValue = seleccion;
        }

        protected void traerbanco(string seleccion, string seleccionSucursal)
        {
            var query = (from tabla in consultas.Db.PagoBancos
                         where tabla.Activo == Convert.ToBoolean(1)
                         select new
                         {
                             id_Tabla = tabla.Id_Banco,
                             nomb_tabla = tabla.NomBanco,
                         });
            ddl_banco.DataSource = query.ToList();
            ddl_banco.DataTextField = "nomb_tabla";
            ddl_banco.DataValueField = "id_Tabla";
            ddl_banco.DataBind();
            ddl_banco.SelectedValue = seleccion;
            traersucursal(seleccionSucursal, Convert.ToInt32(ddl_banco.SelectedValue));
        }

        protected void traersucursal(string seleccion, int codigoBanco)
        {
            try
            {
                var query = from tabla in consultas.Db.PagoSucursals
                            where tabla.CodPagoBanco == codigoBanco
                            select new
                            {
                                id_Tabla = tabla.Id_PagoSucursal,
                                nomb_tabla = tabla.NomPagoSucursal + " (" + tabla.CodPagoSucursal + ")",
                            };
                ddl_sucursal.DataSource = query.ToList();
                ddl_sucursal.DataTextField = "nomb_tabla";
                ddl_sucursal.DataValueField = "id_Tabla";
                ddl_sucursal.DataBind();
                ddl_sucursal.SelectedValue = seleccion;
            }
            catch { }
        }

        protected void ddl_banco_SelectedIndexChanged1(object sender, EventArgs e)
        {
            traersucursal("0", Convert.ToInt32(ddl_banco.SelectedValue));
        }

        protected void btn_crear_Click(object sender, EventArgs e)
        {
            try
            {
                var query = (from pagos in consultas.Db.PagoBeneficiarios
                             where pagos.CodTipoIdentificacion == Convert.ToInt32(ddl_tipoid.SelectedValue)
                             & pagos.NumIdentificacion == txt_numdocumento.Text
                             & pagos.CodEmpresa == codigoempresa()
                             select new
                             {
                                 pagos.Id_PagoBeneficiario,
                             }).Count();

                if (query == 0)
                {
                    //Inicializar variables.
                    String msg = "";

                    msg = EvaluarCampos();

                    if (msg != "")
                    { insertarOmodificar(codigoempresa(), 0, "Create"); }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + msg + "')", true);
                        return;
                    }

                    //Destruir la variable.
                    msg = null;
                }
                else
                { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Este beneficiario ya está registrado en su empresa.!')", true); }
            }
            catch { }
        }

        /// <summary>
        /// Método que contiene el procedimiento almacenado para insertar / actualizar beneficiario.
        /// </summary>
        /// <param name="codigoEmpr"></param>
        /// <param name="Idbenefic"></param>
        /// <param name="tipoCaso"></param>
        protected void insertarOmodificar(int codigoEmpr, int Idbenefic, string tipoCaso)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_Insert_Update_Beneficiario", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TipoIdentificacion", Convert.ToInt32(ddl_banco.SelectedValue));
            cmd.Parameters.AddWithValue("@numIdentificacion", txt_numdocumento.Text);
            cmd.Parameters.AddWithValue("@txtNombres", txt_nombres.Text);
            cmd.Parameters.AddWithValue("@txtApellidos", txt_apellidos.Text);
            cmd.Parameters.AddWithValue("@txtRazonSocial", txt_rsocial.Text);
            cmd.Parameters.AddWithValue("@TipoSociedad", Convert.ToInt32(ddl_tsociedad.SelectedValue));
            cmd.Parameters.AddWithValue("@Retencion", Convert.ToInt32(ddl_tretencion.SelectedValue));
            cmd.Parameters.AddWithValue("@Ciudad", Convert.ToInt32(ddl_ciudadpago.SelectedValue));
            cmd.Parameters.AddWithValue("@txtTelefono", txt_telefono.Text);
            cmd.Parameters.AddWithValue("@txtDireccion", txt_direccion.Text);
            cmd.Parameters.AddWithValue("@txtFax", txt_fax.Text);
            cmd.Parameters.AddWithValue("@txtEmail", txt_email.Text);
            cmd.Parameters.AddWithValue("@Banco", Convert.ToInt32(ddl_banco.SelectedValue));
            cmd.Parameters.AddWithValue("@Sucursal", Convert.ToInt32(ddl_sucursal.SelectedValue));
            cmd.Parameters.AddWithValue("@TipoCuenta", Convert.ToInt32(ddl_tcuenta.SelectedValue));
            cmd.Parameters.AddWithValue("@txtNumCuenta", txt_ncuenta.Text);
            cmd.Parameters.AddWithValue("@CodEmpresa", codigoEmpr);
            cmd.Parameters.AddWithValue("@IdBeneficiario", Idbenefic);
            cmd.Parameters.AddWithValue("@caso", tipoCaso);
            try
            {
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }


            //string mensajeAlert;
            //if (Idbenefic == 0)
            //{
            //    mensajeAlert = "El beneficiario se ha creado exitosamente!";
            //}
            //else
            //{
            //    mensajeAlert = "El beneficiario ha sido actualizado exitosamente!";
            //}
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + mensajeAlert + "')", true);

            Response.Redirect("CatalogoBeneficiario.aspx");
        }

        /// <summary>
        /// Actualizar Beneficiario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Actualizar_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            String msg = "";

            msg = EvaluarCampos();

            if (msg != "")
            { insertarOmodificar(codigoempresa(), codigo, "Update"); }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + msg + "')", true);
                return;
            }

            //Destruir la variable.
            msg = null;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/09/2014.
        /// Seegún FONADE clásico, el nombre de la función es "Evaluate", pero 
        /// para evitar conflictos de nombres, se ha agregado este nombre para el método.
        /// El método evalúa que los campos contenga la información requerida para
        /// el registro del beneficiario.
        /// </summary>
        /// <returns>string vacío = no hay errores. // string con datos = error.</returns>
        private string EvaluarCampos()
        {
            //Inicializar variables.
            String msg = "";
            Int32 NumIdent = 0;
            char caracter = '0';

            try
            {
                if (ddl_tipoid.SelectedValue == "" || ddl_tipoid.SelectedValue == null)
                { msg = "Llene el campo de Tipo de Identificación"; }

                try
                {
                    if (txt_numdocumento.Text.Trim() == "" || !Int32.TryParse(txt_numdocumento.Text, out NumIdent) || txt_numdocumento.Text.Trim().Length != 9)
                    { msg = "Llene el campo de Número de Identificación, sin dígito de verificación, ni ceros al inicio"; }
                }
                catch (Exception)
                { msg = "Llene el campo de Número de Identificación, sin dígito de verificación, ni ceros al inicio"; }

                //Validar que el primer caracter NO sea un cero.
                caracter = txt_numdocumento.Text[0];

                if (caracter == '0')
                { msg = "El campo Número de Identificación sin dígito de verificación, ni ceros al inicio"; }

                if (txt_rsocial.Text.Trim() == "")
                { msg = "Llene el campo de Razon Social."; }

                if (ddl_tsociedad.SelectedValue == "" || ddl_tsociedad.SelectedValue == null)
                { msg = "Llene el campo de Tipo Sociedad."; }

                if (ddl_tretencion.SelectedValue == "" || ddl_tretencion.SelectedValue == null)
                { msg = "Llene el campo de Tipo de Retención."; }
                else
                {
                    //Validar que si es nit no puede ser Régimen simplificado.
                    if (ddl_tipoid.SelectedValue == "5")
                    {
                        if (ddl_tretencion.SelectedValue == "2")
                        {
                            msg = "Regimen simplificado no se puede asociar a un Tipo de Documento NIT.";
                        }
                    }
                }

                if (ddl_ciudadpago.SelectedValue == "" || ddl_ciudadpago.SelectedValue == null)
                { msg = "Llene el campo de Ciudad de Pago."; }

                if (txt_direccion.Text.Trim() == "")
                { msg = "Llene el campo Dirección."; }

                if (ddl_banco.SelectedValue == "" || ddl_banco.SelectedValue == null)
                { msg = "Llene el campo Banco."; }

                if (ddl_tcuenta.SelectedValue == "" || ddl_tcuenta.SelectedValue == null)
                { msg = "Llene el campo Tipo de Cuenta."; }

                if (txt_ncuenta.Text.Trim() == "")
                { msg = "Llene el campo Número de Cuenta."; }

                try
                {
                    if (txt_ncuenta.Text.Trim() == "" || !Int32.TryParse(txt_ncuenta.Text, out NumIdent))
                    { msg = "Llene el campo de Numero de Cuenta con datos numericos, sin guiones, sin espacios."; }
                }
                catch (Exception)
                { msg = "Llene el campo de Numero de Cuenta con datos numericos, sin guiones, sin espacios."; }

                //Retornar "mensaje".
                return msg;
            }
            catch (Exception ex) { msg = "Error: " + ex.Message; return msg; }
        }
    }
}