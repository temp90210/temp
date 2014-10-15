using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Globalization;

namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoFinanzasEgreso : Negocio.Base_Page
    {
        public string codProyecto;
        public string codConvocatoria;
        public int txtTab = Constantes.CONST_Egresos;
        public string idInversionEdita;
        public string accion;
        public int txtTiempoProyeccion;
        private Datos.ProyectoFinanzasEgreso registroActual;
        private Datos.ProyectoInversion registroInversion;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si está o no "realizado"...
        /// </summary>
        public Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["codProyecto"].ToString() != string.Empty)
                codProyecto = Session["codProyecto"].ToString();

            if (Request.QueryString["Accion"] == "Editar")
            {
                accion = Request.QueryString["Accion"].ToString();
                idInversionEdita = Request.QueryString["idInversion"].ToString();
            }

            inicioEncabezado(codProyecto, codConvocatoria, txtTab);

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto, "");

            //if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_AdministradorFonade)
            if (esMiembro && !bRealizado)//!chk_realizado.Checked)
            { this.div_Post_It_2.Visible = true; }

            if (!(esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor) || bRealizado)
            { txtActualizacionMonetaria.Enabled = false; }

            if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
            {
                btnGuardar.Visible = true;
                pnlAdicionar.Visible = true;
                btnGuardar.Visible = true;
            }

            CargarTiempoProyeccion();

            if (!IsPostBack)
            {
                if (accion == "Editar")
                { EditarInversion(codProyecto, idInversionEdita); }

                CargarTipoFuenteInversion();
                CargarInversionesFijas();
                CargarActualizacionMonetaria();
                CargarCostosPuestaEnMarca();
                CargarCostosAnualizados();
                CargarGastosPersonales();

                ObtenerDatosUltimaActualizacion();
            }
        }

        protected void CargarTipoFuenteInversion()
        {

            ddlTipoFuente.Items.Add(new ListItem() { Value = Constantes.CONST_FondoEmprender, Text = Constantes.CONST_FondoEmprender });
            ddlTipoFuente.Items.Add(new ListItem() { Value = Constantes.CONST_AporteEmprendedores, Text = Constantes.CONST_AporteEmprendedores });
            ddlTipoFuente.Items.Add(new ListItem() { Value = Constantes.CONST_RecursosCapital, Text = Constantes.CONST_RecursosCapital });
            ddlTipoFuente.Items.Add(new ListItem() { Value = Constantes.CONST_IngresosVentas, Text = Constantes.CONST_IngresosVentas });
        }

        protected void CargarActualizacionMonetaria()
        {
            try
            {
                var query = (from p in consultas.Db.ProyectoFinanzasEgresos
                             where p.CodProyecto == Convert.ToInt32(codProyecto)
                             select new { p.ActualizacionMonetaria }).First();
                txtActualizacionMonetaria.Text = query.ActualizacionMonetaria.ToString();
            }
            catch
            {
                txtActualizacionMonetaria.Text = "1";
            }
        }

        protected void CargarTiempoProyeccion()
        {
            try
            {
                var query = (from p in consultas.Db.ProyectoMercadoProyeccionVentas
                             where p.CodProyecto == Convert.ToInt32(codProyecto)
                             select new { p.TiempoProyeccion }).First();
                txtTiempoProyeccion = Convert.ToInt32(query.TiempoProyeccion);
            }
            catch
            {
                txtTiempoProyeccion = 3;
            }
        }

        protected void CargarInversionesFijas()
        {
            var query = (from final in
                             ((
                                 from b in
                                     (from a in consultas.Db.ProyectoInfraestructuras select new { a.CodTipoInfraestructura, a.codProyecto })
                                 from p in consultas.Db.ProyectoInversions
                                 from t in consultas.Db.TipoInfraestructuras
                                 where p.Concepto == t.NomTipoInfraestructura &&
                                        p.CodProyecto == b.codProyecto &&
                                        t.Id_TipoInfraestructura == b.CodTipoInfraestructura &&
                                        b.codProyecto == Convert.ToInt32(codProyecto)

                                 select new
                                 {
                                     p.Id_Inversion,
                                     p.Concepto,
                                     p.Valor,
                                     p.Semanas,
                                     p.AportadoPor,
                                     p.TipoInversion
                                 }).Union(
                                 from pr in consultas.Db.ProyectoInversions
                                 where pr.CodProyecto == Convert.ToInt32(codProyecto) &&
                                 pr.TipoInversion == "Diferida"
                                 select new
                                 {
                                     pr.Id_Inversion,
                                     pr.Concepto,
                                     pr.Valor,
                                     pr.Semanas,
                                     pr.AportadoPor,
                                     pr.TipoInversion
                                 }
                                 ))
                         orderby final.TipoInversion descending
                         select new
                         {
                             final.Id_Inversion,
                             final.Concepto,
                             final.Valor,
                             final.Semanas,
                             final.AportadoPor,
                             final.TipoInversion
                         }
                         );

            DataTable datos = new DataTable();
            datos.Columns.Add("CodProyecto");
            datos.Columns.Add("Id_Inversion");
            datos.Columns.Add("concepto");
            datos.Columns.Add("valor");
            datos.Columns.Add("mes");
            datos.Columns.Add("tipoFuente");

            string inversionActual = "";
            double total = 0;
            foreach (var item in query)
            {

                if (inversionActual != item.TipoInversion)
                {
                    DataRow drTitulo = datos.NewRow();
                    drTitulo["concepto"] = item.TipoInversion;
                    inversionActual = item.TipoInversion;
                    datos.Rows.Add(drTitulo);
                }

                DataRow dr = datos.NewRow();
                dr["CodProyecto"] = codProyecto;
                dr["Id_Inversion"] = item.Id_Inversion;
                dr["concepto"] = item.Concepto;
                dr["valor"] = item.Valor.ToString("0,0.00", CultureInfo.InvariantCulture); ;
                dr["mes"] = item.Semanas;
                dr["tipoFuente"] = item.AportadoPor;
                total += Convert.ToDouble(item.Valor);
                datos.Rows.Add(dr);
            }

            DataRow drTotal = datos.NewRow();
            drTotal["concepto"] = "Total";
            drTotal["valor"] = total.ToString("0,0.00", CultureInfo.InvariantCulture);
            datos.Rows.Add(drTotal);

            gw_InversionesFijas.DataSource = datos;
            gw_InversionesFijas.DataBind();

            for (int i = 0; i < gw_InversionesFijas.Rows.Count; i++)
            {
                if (gw_InversionesFijas.Rows[i].Cells[2].Text != "&nbsp;" && gw_InversionesFijas.Rows[i].Cells[1].Text != "Total")
                {
                    if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
                    {
                        if (gw_InversionesFijas.Rows[i].Cells[4].Text != "Diferida")
                        {
                            gw_InversionesFijas.Rows[i].Cells[0].Text = "";
                        }
                        string id_Inventario = gw_InversionesFijas.DataKeys[i].Value.ToString();
                        gw_InversionesFijas.Rows[i].Cells[1].Text = "<a href='PProyectoFinanzasEgreso.aspx?CodProyecto=" + codProyecto + "&idInversion=" + id_Inventario + "&Accion=Editar'>" + gw_InversionesFijas.Rows[i].Cells[1].Text + "</a>";
                    }
                    else
                    {
                        gw_InversionesFijas.Rows[i].Cells[0].Text = "";
                    }
                }
                else
                {
                    gw_InversionesFijas.Rows[i].Cells[0].Text = "";
                    gw_InversionesFijas.Rows[i].Cells[1].Text = "<span class='TitulosRegistrosGrilla'>" + gw_InversionesFijas.Rows[i].Cells[1].Text + "</span>";
                }
            }
        }

        private void CargarCostosPuestaEnMarca()
        {
            decimal total = 0;

            var query = (from p in consultas.Db.ProyectoGastos
                         where p.Tipo == "Arranque"
                         && p.CodProyecto == Convert.ToInt32(codProyecto)
                         orderby p.Descripcion ascending
                         select new { p.Id_Gasto, p.Descripcion, p.Valor, p.Protegido });

            DataTable datos = new DataTable();
            datos.Columns.Add("Descripcion");
            datos.Columns.Add("Valor");

            foreach (var item in query)
            {
                decimal valor = item.Valor;

                DataRow dr = datos.NewRow();
                dr["Descripcion"] = item.Descripcion;
                dr["Valor"] = valor.ToString("0,0.00", CultureInfo.InvariantCulture);

                total += valor;

                datos.Rows.Add(dr);
            }

            DataRow drTotal = datos.NewRow();

            drTotal["Descripcion"] = "Total";
            drTotal["Valor"] = total.ToString("0,0.00", CultureInfo.InvariantCulture);

            datos.Rows.Add(drTotal);

            gw_CostosPuestaMarcha.DataSource = datos;
            gw_CostosPuestaMarcha.DataBind();

            //Ajustar la alineación de los registros que muestra la grilla.
            if (gw_CostosPuestaMarcha.Columns.Count > 0)
            {
                gw_CostosPuestaMarcha.Columns[0].ItemStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            PintarFilasGrid(gw_CostosPuestaMarcha, 0, new string[] { "Total" });
        }

        private void CargarCostosAnualizados()
        {
            var query = (from p in consultas.Db.ProyectoGastos
                         where p.Tipo == "Anual"
                         && p.CodProyecto == Convert.ToInt32(codProyecto)
                         orderby p.Descripcion ascending
                         select new { p.Id_Gasto, p.Descripcion, p.Valor, p.Protegido });

            DataTable datos = new DataTable();
            datos.Columns.Add("Descripcion");
            for (int i = 1; i <= txtTiempoProyeccion; i++)
            {
                datos.Columns.Add("Año " + i);
            }

            decimal[] total = new decimal[txtTiempoProyeccion + 1];
            foreach (var item in query)
            {
                DataRow dr = datos.NewRow();

                dr["Descripcion"] = item.Descripcion;

                decimal valor = item.Valor;
                for (int i = 1; i <= txtTiempoProyeccion; i++)
                {
                    dr["Año " + i] = valor.ToString("0,0.00", CultureInfo.InvariantCulture); ;
                    total[i] += valor;
                    valor = Convert.ToDecimal(txtActualizacionMonetaria.Text) * valor;
                }
                datos.Rows.Add(dr);
            }

            DataRow drTotal = datos.NewRow();
            drTotal["Descripcion"] = "Total";
            for (int i = 1; i <= txtTiempoProyeccion; i++)
            {
                drTotal["Año " + i] = total[i].ToString("0,0.00", CultureInfo.InvariantCulture);
            }
            datos.Rows.Add(drTotal);

            gw_CostosAnualizados.DataSource = datos;
            gw_CostosAnualizados.DataBind();

            //Ajustar la alineación de los registros que muestra la grilla.
            if (gw_CostosAnualizados.Columns.Count > 0)
            {
                gw_CostosAnualizados.Columns[0].ItemStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            PintarFilasGrid(gw_CostosAnualizados, 0, new string[] { "Total" });
        }

        private void CargarGastosPersonales()
        {
            var query = (from p in consultas.Db.ProyectoGastosPersonals
                         where p.CodProyecto == Convert.ToInt32(codProyecto)
                         orderby p.Cargo ascending
                         select new { p.Cargo, p.ValorAnual });

            DataTable datos = new DataTable();
            datos.Columns.Add("Cargo");
            for (int i = 1; i <= txtTiempoProyeccion; i++)
            {
                datos.Columns.Add("Año " + i);
            }

            decimal[] total = new decimal[txtTiempoProyeccion + 1];
            foreach (var item in query)
            {
                DataRow dr = datos.NewRow();

                dr["Cargo"] = item.Cargo;

                decimal valor = item.ValorAnual;
                for (int i = 1; i <= txtTiempoProyeccion; i++)
                {
                    dr["Año " + i] = valor.ToString("0,0.00", CultureInfo.InvariantCulture); ;
                    total[i] += valor;
                    valor = Convert.ToDecimal(txtActualizacionMonetaria.Text) * valor;
                }
                datos.Rows.Add(dr);
            }

            DataRow drTotal = datos.NewRow();
            drTotal["Cargo"] = "Total";
            for (int i = 1; i <= txtTiempoProyeccion; i++)
            {
                drTotal["Año " + i] = total[i].ToString("0,0.00", CultureInfo.InvariantCulture);
            }
            datos.Rows.Add(drTotal);

            gw_GastosPersonales.DataSource = datos;
            gw_GastosPersonales.DataBind();

            //Ajustar la alineación de los registros que muestra la grilla.
            if (gw_GastosPersonales.Columns.Count > 0)
            { gw_GastosPersonales.Columns[0].ItemStyle.HorizontalAlign = HorizontalAlign.Right; }

            PintarFilasGrid(gw_GastosPersonales, 0, new string[] { "Total" });
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            var query = (from p in consultas.Db.ProyectoFinanzasEgresos
                         where p.CodProyecto == Convert.ToInt32(codProyecto)
                         select new { p.CodProyecto }).ToList();
            if (query.Count == 0)
            {
                //inserta
                Datos.ProyectoFinanzasEgreso datosNuevos = new ProyectoFinanzasEgreso()
                {
                    CodProyecto = Convert.ToInt32(codProyecto),
                    ActualizacionMonetaria = Convert.ToDouble(txtActualizacionMonetaria.Text)
                };
                consultas.Db.ProyectoFinanzasEgresos.InsertOnSubmit(datosNuevos);
            }
            else
            {
                //update
                registroActual = getProyectoFinanzasEgreso();
                registroActual.ActualizacionMonetaria = Convert.ToDouble(txtActualizacionMonetaria.Text);

            }
            consultas.Db.SubmitChanges();
            //Actualizar fecha modificación del tab.
            prActualizarTab(txtTab.ToString(), codProyecto);
            ObtenerDatosUltimaActualizacion();
            CargarActualizacionMonetaria();
            CargarInversionesFijas();
            CargarCostosPuestaEnMarca();
            CargarCostosAnualizados();
            CargarGastosPersonales();

        }

        private ProyectoFinanzasEgreso getProyectoFinanzasEgreso()
        {
            var query = (from p in consultas.Db.ProyectoFinanzasEgresos
                         where p.CodProyecto == Convert.ToInt32(codProyecto)
                         select p).First();

            return query;

        }

        protected void gw_InversionesFijas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Borrar")
            {
                string idInversion = e.CommandArgument.ToString();
                consultas.Db.ExecuteCommand("delete from ProyectoInversion where Id_Inversion={0}", Convert.ToInt32(idInversion));
                CargarInversionesFijas();
            }
        }

        protected void btnAdicionarInversion_Click(object sender, EventArgs e)
        {
            CargarInversionesFijas();
            pnlEgresos.Visible = false;
            pnlCrearInversion.Visible = true;

        }

        protected void btnCancelarNuevaInversion_Click(object sender, EventArgs e)
        {
            if (accion == "Editar")
            {
                Response.Redirect("PProyectoFinanzasEgreso.aspx?CodProyecto=" + codProyecto + "");
            }
            else
            {
                CargarInversionesFijas();
                pnlEgresos.Visible = true;
                pnlCrearInversion.Visible = false;
            }
        }

        protected void btnCrearInversion_Click(object sender, EventArgs e)
        {

            if (accion == "Editar")
            {

                var query = (from p in consultas.Db.ProyectoInversions
                             where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                             p.Id_Inversion == Convert.ToInt32(idInversionEdita)
                             select p
                      ).First();



                query.Concepto = txtConcepto.Text;
                query.Valor = Convert.ToDecimal(txtValor.Text);
                query.Semanas = Convert.ToInt16(txtSemana.Text);
                query.AportadoPor = ddlTipoFuente.SelectedValue;
                consultas.Db.SubmitChanges();
                //ObtenerDatosUltimaActualizacion();
                Response.Redirect("PProyectoFinanzasEgreso.aspx?CodProyecto=" + codProyecto + "");
            }
            else
            {

                Datos.ProyectoInversion datosNuevos = new ProyectoInversion()
                {
                    CodProyecto = Convert.ToInt32(codProyecto),
                    Concepto = txtConcepto.Text,
                    Valor = Convert.ToDecimal(txtValor.Text),
                    Semanas = Convert.ToInt16(txtSemana.Text),
                    AportadoPor = ddlTipoFuente.SelectedValue,
                    TipoInversion = "Diferida"
                };
                consultas.Db.ProyectoInversions.InsertOnSubmit(datosNuevos);

                consultas.Db.SubmitChanges();
                //ObtenerDatosUltimaActualizacion();
                CargarInversionesFijas();

                txtConcepto.Text = "";
                txtValor.Text = "0";
                txtSemana.Text = "0";

                pnlEgresos.Visible = true;
                pnlCrearInversion.Visible = false;
            }
        }

        private void EditarInversion(string codProyecto, string idInversionEdita)
        {
            CargarInversionesFijas();
            pnlEgresos.Visible = false;
            pnlCrearInversion.Visible = true;

            var query = (from p in consultas.Db.ProyectoInversions
                         where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                         p.Id_Inversion == Convert.ToInt32(idInversionEdita)
                         select p
                             ).First();

            txtConcepto.Text = query.Concepto;
            txtValor.Text = query.Valor.ToString();
            txtSemana.Text = query.Semanas.ToString();
            ddlTipoFuente.SelectedValue = query.AportadoPor.ToString();

        }

        #region Métodos de Mauricio Arias Olave.

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

        /// <summary>
        /// Mauricio Arias Olave.
        /// 06/06/2014.
        /// Obtener la información acerca de la última actualización realizada, ási como la habilitación del 
        /// CheckBox para el usuario dependiendo de su grupo / rol.
        /// </summary>
        private void ObtenerDatosUltimaActualizacion()
        {
            //Inicializar variables.
            String txtSQL;
            DateTime fecha = new DateTime();
            DataTable tabla = new DataTable();
            bool bRealizado = false;
            bool EsMiembro = false;
            Int32 numPostIt = 0;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si es miembro.
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

                //Obtener número "numPostIt".
                numPostIt = Obtener_numPostIt();

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(txtTab.ToString(), codProyecto, ""); //codConvocatoria);

                //Consultar los datos a mostrar en los campos correspondientes a la actualización.
                txtSQL = " SELECT Nombres + ' ' + Apellidos AS nombre, FechaModificacion, Realizado " +
                         " FROM TabProyecto, Contacto " +
                         " where Id_Contacto = CodContacto AND CodTab = " + txtTab +
                         " AND CodProyecto = " + codProyecto;

                //Asignar resultados de la consulta a variable DataTable.
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                //Si tiene datos "y debe tenerlos" ejecuta el siguiente código.
                if (tabla.Rows.Count > 0)
                {
                    //Nombre del usuario quien hizo la actualización.
                    lbl_nombre_user_ult_act.Text = tabla.Rows[0]["nombre"].ToString().ToUpperInvariant();

                    #region Formatear la fecha.

                    //Convertir fecha.
                    try { fecha = Convert.ToDateTime(tabla.Rows[0]["FechaModificacion"].ToString()); }
                    catch { fecha = DateTime.Today; }

                    //Obtener el nombre del mes (las primeras tres letras).
                    string sMes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                    //Obtener la hora en minúscula.
                    string hora = fecha.ToString("hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant();

                    //Reemplazar el valor "am" o "pm" por "a.m" o "p.m" respectivamente.
                    if (hora.Contains("am")) { hora = hora.Replace("am", "a.m"); } if (hora.Contains("pm")) { hora = hora.Replace("pm", "p.m"); }

                    //Formatear la fecha según manejo de FONADE clásico. "Ej: Nov 19 de 2013 07:36:26 p.m.".
                    lbl_fecha_formateada.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year + " " + hora + ".";

                    #endregion

                    //Valor "bRealziado".
                    bRealizado = Convert.ToBoolean(tabla.Rows[0]["Realizado"].ToString());
                }

                //Asignar check de acuerdo al valor obtenido en "bRealizado".
                chk_realizado.Checked = bRealizado;

                //Determinar si el usuario actual puede o no "chequear" la actualización.
                //if (!(EsMiembro && numPostIt == 0 && ((usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && usuario.CodGrupo == Constantes.CONST_RolEvaluador && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                if (!(EsMiembro && numPostIt == 0 && ((Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                { chk_realizado.Enabled = false; }

                //Mostrar el botón de guardar.
                //if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (usuario.CodGrupo == Constantes.CONST_RolEvaluador && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Mostrar los enlaces para adjuntar documentos.
                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolEmprendedor.ToString() && !bRealizado)
                {
                    tabla_docs.Visible = true;
                }

                //Destruir variables.
                tabla = null;
                txtSQL = null;
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                //Destruir variables.
                //tabla = null;
                //txtSQL = null;
                //return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 06/06/2014.
        /// Obtener el número "numPostIt" usado en la condicional de "obtener última actualización".
        /// El código se encuentra en "Base_Page" línea "116", método "inicioEncabezado".
        /// Ya se le están enviado por parámetro en el método el código del proyecto y la constante "CONST_PostIt".
        /// </summary>
        /// <returns>numPostIt.</returns>
        private int Obtener_numPostIt()
        {
            Int32 numPosIt = 0;

            //Hallar numero de post it por tab
            var query = from tur in consultas.Db.TareaUsuarioRepeticions
                        from tu in consultas.Db.TareaUsuarios
                        from tp in consultas.Db.TareaProgramas
                        where tp.Id_TareaPrograma == tu.CodTareaPrograma
                        && tu.Id_TareaUsuario == tur.CodTareaUsuario
                        && tu.CodProyecto == Convert.ToInt32(codProyecto)
                        && tp.Id_TareaPrograma == Constantes.CONST_PostIt
                        && tur.FechaCierre == null
                        select tur;

            numPosIt = query.Count();

            return numPosIt;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/06/2014.
        /// Guardar la información "Ultima Actualización".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_guardar_ultima_actualizacion_Click(object sender, EventArgs e)
        { prActualizarTab(txtTab.ToString(), codProyecto.ToString()); Marcar(txtTab.ToString(), codProyecto, "", chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Nuevo";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Vista";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        #endregion

        /// <summary>
        /// Alinear valores.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gw_InversionesFijas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Alinear a la derecha los valores de moneda.
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                }
            }
        }
    }
}