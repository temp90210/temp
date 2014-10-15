using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Globalization;
using System.Configuration;
using System.IO;
using System.Text;

namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoFinanzasIngreso : Negocio.Base_Page
    {
        public string codProyecto;
        public string codConvocatoria;
        public int txtTab = Constantes.CONST_Ingresos;
        public string idAporteEdita;
        public string idRecursoEdita;
        public string accion;
        public string controlAccion;
        public int txtTiempoProyeccion;
        public string NumeroSMLVNV;
        private Datos.ProyectoFinanzasIngreso registroActual;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si está o no "realizado"...
        /// </summary>
        public Boolean bRealizado;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["codProyecto"].ToString() != string.Empty)
                codProyecto = Session["codProyecto"].ToString();

            if (!IsPostBack)
            {
                inicioEncabezado(codProyecto, codConvocatoria, txtTab);

                //Consultar si es miembro.
                esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

                //Consultar si está "realizado".
                bRealizado = esRealizado(txtTab.ToString(), codProyecto, "");

                //if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_AdministradorFonade)
                if (esMiembro && !bRealizado)//!chk_realizado.Checked)
                {
                    this.div_post_it_1.Visible = true;
                    this.div_post_it_2.Visible = true;
                    btnGuardar.Visible = true;
                    txtRecursosSolicitados.Enabled = true;
                    //Comprobar si el archivo existe.
                    //btnLinkVerModeloFinanciero.Visible = true;
                }

                if (Request.QueryString["Accion"] == "Editar")
                {
                    accion = Request.QueryString["Accion"].ToString();
                    controlAccion = Request.QueryString["controlAccion"].ToString();
                    if (controlAccion == "Aporte")
                    {
                        idAporteEdita = Request.QueryString["idAporte"].ToString();
                        CargarControlesEditarAporte();
                    }
                    else if (controlAccion == "Recurso")
                    {
                        idRecursoEdita = Request.QueryString["idRecurso"].ToString();
                        CargarControlesEditarRecurso();
                    }
                }

                if (!(esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor) || bRealizado)
                    txtRecursosSolicitados.Enabled = false;

                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
                {
                    #region Habiliatar campos.

                    Image1.Visible = true;
                    btnAdicionarAporte.Visible = true;

                    Image2.Visible = true;
                    btnAdicionarRecurso.Visible = true;

                    btnLinkBajarModeloFinanciero.Visible = true;
                    btnLinkSubirModeloFinanciero.Visible = true;

                    btnLinkGuiaLlenarModeloFinanciero.Visible = true;

                    #endregion

                    //Habilitar mas campos.
                    btnGuardar.Visible = true;
                    pnlBotonAdicionarAporte.Visible = true;
                    pnlBotonAdicionarRecurso.Visible = true;
                    btnLinkBajarModeloFinanciero.Visible = true;
                    btnLinkSubirModeloFinanciero.Visible = true;
                }


                string ruta = ConfigurationManager.AppSettings.Get("RutaDocumentosProyecto") + Math.Abs(Convert.ToInt32(codProyecto) / 2000) + @"\Proyecto_" + codProyecto + @"\FORMATOSFINANCIEROS" + codProyecto + ".xls";
                if (File.Exists(ruta))
                {
                    //btnLinkVerModeloFinanciero.Visible = true;
                    HyperLink1.NavigateUrl = ruta;
                    HyperLink1.Visible = true;
                }
                else
                {
                    HyperLink1.NavigateUrl = "#";
                    HyperLink1.Visible = false;
                }

                ((Button)CargarArchivos1.FindControl("btnSubirDocumento")).Command += new CommandEventHandler(SubirArchivo_Click);
                ((Button)CargarArchivos1.FindControl("btnCancelar")).Command += new CommandEventHandler(CancelarArchivo_Click);

                lblErrorRecursos.Text = "";
                CargarTiempoProyeccion();
                CargarNumeroSMLVNV();
                if (accion == "Editar")
                {
                    if (controlAccion == "Aporte")
                        CargarControlesEditarAporte();
                    else if (controlAccion == "Recurso")
                        CargarControlesEditarRecurso();
                }

                CargarTxtRecursosSolicitados();
                CargarListadoTipoRecurso();
                CargarRecursosCapital();

                CargarAportesEmprendedores();
                CargarListadoTipoAporte();

                CargarIngresosVentas();

                ObtenerDatosUltimaActualizacion();
            }
        }

        #region General

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarRecursos(txtRecursosSolicitados, lblErrorRecursos))
            { return; }

            var query = (from p in consultas.Db.ProyectoFinanzasIngresos
                         where p.CodProyecto == Convert.ToInt32(codProyecto)
                         select new { p.CodProyecto }).ToList();
            if (query.Count == 0)
            {
                //inserta
                Datos.ProyectoFinanzasIngreso datosNuevos = new ProyectoFinanzasIngreso()
                {
                    CodProyecto = Convert.ToInt32(codProyecto),
                    Recursos = Convert.ToByte(txtRecursosSolicitados.Text)
                };
                consultas.Db.ProyectoFinanzasIngresos.InsertOnSubmit(datosNuevos);
                ObtenerDatosUltimaActualizacion();
            }
            else
            {
                //update
                registroActual = getProyectoFinanzasIngreso();
                registroActual.Recursos = Convert.ToByte(txtRecursosSolicitados.Text);

            }
            consultas.Db.SubmitChanges();

            if (txtRecursosSolicitados.Text.Trim() != "")
            {
                //Actualizar fecha modificación del tab.
                prActualizarTab(txtTab.ToString(), codProyecto);
                ObtenerDatosUltimaActualizacion();
            }
        }

        protected void CargarNumeroSMLVNV()
        {
            int numeroEmpleosNV = 0;
            int codigoConvocatoria = 0;
            var queryCant = (from p in consultas.Db.ProyectoGastosPersonals
                             where p.CodProyecto == Convert.ToInt32(codProyecto)
                             select new { p.CodProyecto });

            var queryCant2 = (from p in consultas.Db.ProyectoInsumos
                              where p.CodProyecto == Convert.ToInt32(codProyecto)
                              select new { p.CodProyecto });

            numeroEmpleosNV = queryCant.ToList().Count + queryCant2.ToList().Count;

            try
            {
                var queryConvoca = (from cp in consultas.Db.ConvocatoriaProyectos
                                    where cp.CodProyecto == Convert.ToInt32(codProyecto)
                                    select new { cp.CodConvocatoria }
                                        ).FirstOrDefault();
                codigoConvocatoria = queryConvoca.CodConvocatoria;
            }
            catch
            {
                codigoConvocatoria = 0;
            }

            ConsultarSalarioSMLVNV(1, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(2, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(3, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(4, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(5, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(6, numeroEmpleosNV, codigoConvocatoria);

        }

        private void ConsultarSalarioSMLVNV(int regla, int numeroEmpleosNV, int codigoConvocatoria)
        {

            try
            {
                var queryRegla = (from p in consultas.Db.ConvocatoriaReglaSalarios
                                  where p.NoRegla == regla && p.CodConvocatoria == codigoConvocatoria
                                  select p).FirstOrDefault();

                int empv1 = queryRegla.EmpleosGenerados1;
                int? empv11 = queryRegla.EmpleosGenerados2;
                string lista1 = queryRegla.ExpresionLogica;
                int Salmin1 = queryRegla.SalariosAPrestar;

                switch (lista1)
                {
                    case "=":
                        if (numeroEmpleosNV == empv1)
                            NumeroSMLVNV = Salmin1.ToString();
                        break;
                    case "<":
                        if (numeroEmpleosNV < empv1)
                            NumeroSMLVNV = Salmin1.ToString();
                        break;
                    case ">":
                        if (numeroEmpleosNV > empv1)
                            NumeroSMLVNV = Salmin1.ToString();

                        break;
                    case "<=":
                        if (numeroEmpleosNV <= empv1)
                            NumeroSMLVNV = Salmin1.ToString();
                        break;
                    case ">=":
                        if (numeroEmpleosNV >= empv1)
                            NumeroSMLVNV = Salmin1.ToString();
                        break;
                    case "Entre":
                        if (numeroEmpleosNV >= empv1 && numeroEmpleosNV <= empv11)
                            NumeroSMLVNV = Salmin1.ToString();
                        break;
                }
            }
            catch { }
        }

        private bool ValidarRecursos(TextBox campo, Label lblError)
        {
            if (campo.Text == "" || campo.Text == "0")
            {
                lblError.Text = "Debe ingresar los Recursos solicitados al Fondo Emprender";
                return false;
            }
            else
            {
                int respeusta = 0;
                if (!int.TryParse(campo.Text, out respeusta))
                {
                    lblError.Text = "El valor debe ser numérico";
                    return false;
                }
                else if (Convert.ToInt32(campo.Text) > Convert.ToInt32(NumeroSMLVNV))
                {
                    lblError.Text = "Los Recursos solicitados al Fondo Emprender no pueden ser mayores a " + NumeroSMLVNV + " smlv";
                    campo.Text = NumeroSMLVNV;
                    return false;
                }
            }
            return true;
        }

        private void CargarIngresosVentas()
        {
            #region Versión anterior.
            ////Inicializar variables.
            //DataTable datos = new DataTable();

            ////Consulta LINQ.
            //var query = (from p in consultas.Db.ProyectoProductos
            //             where p.CodProyecto == Convert.ToInt32(codProyecto)
            //             select new { p.Id_Producto, p.NomProducto, p.PorcentajeIva });

            ////Añadir columna a la tabla.
            //datos.Columns.Add("Producto");

            ////Ciclo que agrega las tablas de años de acuerdo al tiempo de proyección.
            //for (int i = 1; i <= txtTiempoProyeccion; i++)
            //{ datos.Columns.Add("Año " + i); }

            //decimal[] totalPt = new decimal[txtTiempoProyeccion + 1];
            //decimal[] totalIvaPt = new decimal[txtTiempoProyeccion + 1];

            //foreach (var item in query)
            //{
            //    DataRow dr = datos.NewRow();

            //    dr["Producto"] = item.NomProducto;

            //    IEnumerable<BORespuestaDetalleProducto> respuesta = consultas.Db.ExecuteQuery<BORespuestaDetalleProducto>("select cast(sum(unidades)as decimal) as unidades, cast(dbo.RemoveChars([Precio]) as decimal) as precio, cast((sum(unidades)*dbo.RemoveChars([Precio])) as decimal) as total , cast(ano as int) as anio from proyectoproductounidadesventas u, proyectoproductoprecio p where p.codproducto=u.codproducto and periodo=ano and p.codproducto=" + item.Id_Producto + " group by ano,precio order by ano");
            //    int i = 0;
            //    foreach (BORespuestaDetalleProducto registro in respuesta)
            //    {
            //        try
            //        {
            //            dr["Año " + (i + 1)] = (registro.Unidades * registro.Precio).ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));

            //            totalPt[registro.anio - 1] += registro.Total;
            //            totalIvaPt[registro.anio - 1] += Convert.ToDecimal(registro.anio + registro.Total * Convert.ToDecimal(item.PorcentajeIva) / 100);
            //        }
            //        catch { }

            //        //Incremento.
            //        i++;

            //    }

            //    //Añadir la fila a la tabla.
            //    datos.Rows.Add(dr);
            //}

            //DataRow drTotal = datos.NewRow();
            //DataRow drTotalIva = datos.NewRow();
            //DataRow drTotalMasIva = datos.NewRow();
            //drTotal["Producto"] = "Total";
            //drTotalIva["Producto"] = "Iva";
            //drTotalMasIva["Producto"] = "Total con Iva";

            //for (int i = 1; i <= txtTiempoProyeccion; i++)
            //{
            //    drTotal["Año " + i] = totalPt[i].ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
            //    //drTotalIva["Año " + i] = totalIvaPt[i].ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
            //    //drTotalIva["Año " + i] = "0.00";
            //    drTotalIva["Año " + i] = totalIvaPt[i].ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));

            //    // drTotalMasIva["Año " + i] = (totalPt[i] + totalIvaPt[i]).ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
            //    //drTotalMasIva["Año " + i] = totalPt[i] + " 0.00";
            //    drTotalIva["Año " + i] = (totalPt[i] + totalIvaPt[i]).ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
            //}

            //datos.Rows.Add(drTotal);
            //datos.Rows.Add(drTotalIva);
            //datos.Rows.Add(drTotalMasIva);

            //gw_IngresosVentas.DataSource = datos;
            //gw_IngresosVentas.DataBind();
            //PintarFilasGrid(gw_IngresosVentas, 0, new string[] { "Total", "Iva", "Total con Iva" }); 
            #endregion

            #region Versión de Mauricio Arias Olave.

            //Inicializar variables
            String txtSQL;
            DataTable rsProducto = new DataTable();
            DataTable datos = new DataTable();
            DataTable rsUnidades = new DataTable();
            double[] totalPt = new double[txtTiempoProyeccion + 1];
            double[] totalIvaPt = new double[txtTiempoProyeccion + 1];

            txtSQL = " select id_producto, nomproducto, porcentajeiva " +
                     " from proyectoproducto " +
                     " where codproyecto = " + codProyecto;

            rsProducto = consultas.ObtenerDataTable(txtSQL, "text");

            //Añadir columna a la tabla.
            datos.Columns.Add("Producto");

            //Ciclo que agrega las tablas de años de acuerdo al tiempo de proyección.
            for (int i = 1; i <= txtTiempoProyeccion; i++)
            { datos.Columns.Add("Año " + i); }

            foreach (DataRow row in rsProducto.Rows)
            {
                DataRow dr = datos.NewRow();
                dr["producto"] = row["nomproducto"].ToString();

                txtSQL = " select sum(unidades) as unidades, precio, sum(unidades)*precio as total, ano " +
                         " from proyectoproductounidadesventas u, proyectoproductoprecio p " +
                         " where p.codproducto=u.codproducto and periodo=ano and p.codproducto = " + row["id_Producto"].ToString() +
                         " group by ano,precio order by ano";

                rsUnidades = consultas.ObtenerDataTable(txtSQL, "text");

                int incr = 1;
                foreach (DataRow row_unidades in rsUnidades.Rows)
                {
                    try
                    {
                        double unidades = Double.Parse(row_unidades["Unidades"].ToString());
                        double precio = Double.Parse(row_unidades["Precio"].ToString());
                        if (incr == 0)
                        {
                            dr["Producto"] = (unidades * precio).ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
                        }
                        else
                        {
                            dr["Año " + incr.ToString()] = (unidades * precio).ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
                        }


                        double total = Double.Parse(row_unidades["total"].ToString());
                        double anio = Double.Parse(row_unidades["ano"].ToString());
                        double porcentajeIva = Double.Parse(row["porcentajeiva"].ToString());
                        totalPt[incr] += total;
                        totalIvaPt[incr] += (total * porcentajeIva / 100);
                        //Incremento.
                        incr++;
                    }
                    catch { }
                }

                //Añadir la fila a la tabla.
                datos.Rows.Add(dr);
            }

            //Agregar nuevaas filas.
            DataRow drTotal = datos.NewRow();
            DataRow drTotalIva = datos.NewRow();
            DataRow drTotalMasIva = datos.NewRow();
            drTotal["Producto"] = "Total";
            drTotalIva["Producto"] = "Iva";
            drTotalMasIva["Producto"] = "Total con Iva";

            for (int i = 0; i <= txtTiempoProyeccion + 1; i++)
            {
                if (i != 0)
                {
                    if (i <= txtTiempoProyeccion)
                    {
                        drTotal["Año " + i] = totalPt[i].ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
                        drTotalIva["Año " + i] = totalIvaPt[i].ToString("N2", CultureInfo.CreateSpecificCulture("es-CO"));
                        drTotalMasIva["Año " + i] = (totalPt[i] + totalIvaPt[i]).ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
                    }
                }
            }

            //Agregar las filas a la tabla.
            datos.Rows.Add(drTotal);
            datos.Rows.Add(drTotalIva);
            datos.Rows.Add(drTotalMasIva);

            //Bindear la tabla en el GridView.
            gw_IngresosVentas.DataSource = datos;
            gw_IngresosVentas.DataBind();
            PintarFilasGrid(gw_IngresosVentas, 0, new string[] { "Total", "Iva", "Total con Iva" });

            #endregion
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

        private ProyectoFinanzasIngreso getProyectoFinanzasIngreso()
        {
            var query = (from p in consultas.Db.ProyectoFinanzasIngresos
                         where p.CodProyecto == Convert.ToInt32(codProyecto)
                         select p).First();

            return query;
        }

        #endregion General

        #region Aportes

        protected void CargarListadoTipoAporte()
        {
            ddlTipoAporte.Items.Add(new ListItem() { Value = Constantes.CONST_Dinero, Text = Constantes.CONST_Dinero });
            ddlTipoAporte.Items.Add(new ListItem() { Value = Constantes.CONST_Bien, Text = Constantes.CONST_Bien });
            ddlTipoAporte.Items.Add(new ListItem() { Value = Constantes.CONST_Servicio, Text = Constantes.CONST_Servicio });
        }

        protected void CargarAportesEmprendedores()
        {
            var query = (from p in consultas.Db.ProyectoAportes
                         where p.CodProyecto == Convert.ToInt32(this.codProyecto)
                         orderby p.TipoAporte, p.Nombre ascending
                         select new { p.Id_Aporte, p.Nombre, p.Valor, p.TipoAporte, p.Detalle });

            DataTable datos = new DataTable();
            datos.Columns.Add("nombre");
            datos.Columns.Add("valor");
            datos.Columns.Add("detalle");
            datos.Columns.Add("");
            datos.Columns.Add("Id_Aporte");

            string aporteActual = "";
            double total = 0;
            foreach (var item in query)
            {

                if (aporteActual != item.TipoAporte)
                {
                    DataRow drTitulo = datos.NewRow();
                    drTitulo["nombre"] = item.TipoAporte;
                    aporteActual = item.TipoAporte;
                    datos.Rows.Add(drTitulo);
                }

                DataRow dr = datos.NewRow();
                dr["nombre"] = item.Nombre;
                dr["valor"] = item.Valor.ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO")); ;
                dr["detalle"] = item.Detalle;
                dr["Id_Aporte"] = item.Id_Aporte;
                total += Convert.ToDouble(item.Valor);
                datos.Rows.Add(dr);

            }

            DataRow drTotal = datos.NewRow();
            drTotal["nombre"] = "Total";
            drTotal["valor"] = total.ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
            datos.Rows.Add(drTotal);

            gw_AporteEmprendedores.DataSource = datos;
            gw_AporteEmprendedores.DataBind();

            for (int i = 0; i < gw_AporteEmprendedores.Rows.Count; i++)
            {
                // add negrita caso ERROR-JEF-UNID-40 (f3l)
                if (gw_AporteEmprendedores.Rows[i].Cells[1].Text == "Total")
                {
                
                    gw_AporteEmprendedores.Rows[i].Cells[2].Text = "<b>" + gw_AporteEmprendedores.Rows[i].Cells[2].Text + "</b>";
                }
                //f3l end
                if (gw_AporteEmprendedores.Rows[i].Cells[3].Text != "&nbsp;" && gw_AporteEmprendedores.Rows[i].Cells[1].Text != "Total")
                {
                    if (esMiembro == true && usuario.CodGrupo == Constantes.CONST_Emprendedor && bRealizado == false)
                    {
                        string Id_Aporte = gw_AporteEmprendedores.DataKeys[i].Value.ToString();
                        gw_AporteEmprendedores.Rows[i].Cells[1].Text = "<a href='PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "&idAporte=" + Id_Aporte + "&Accion=Editar&controlAccion=Aporte'>" + gw_AporteEmprendedores.Rows[i].Cells[1].Text + "</a>";
                    }
                    else
                    {
                        gw_AporteEmprendedores.Rows[i].Cells[0].Text = "";
                    }
                }
                else
                {
                    gw_AporteEmprendedores.Rows[i].Cells[0].Text = "";
                    gw_AporteEmprendedores.Rows[i].Cells[1].Text = "<span class='TitulosRegistrosGrilla'>" + gw_AporteEmprendedores.Rows[i].Cells[1].Text + "</span>";
                }
            }
        }

        protected void btnAdicionarAporte_Click(object sender, EventArgs e)
        {

            pnlPrincipal.Visible = false;
            pnlAporte.Visible = true;

        }

        //Borrar Aporte
        protected void gw_AporteEmprendedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Borrar")
            {
                string idAporte = e.CommandArgument.ToString();
                consultas.Db.ExecuteCommand("delete from ProyectoAporte where Id_Aporte={0}", Convert.ToInt32(idAporte));
                ObtenerDatosUltimaActualizacion();
                CargarAportesEmprendedores();
                CargarRecursosCapital();

            }
        }

        protected void btnCancelarAporte_Click(object sender, EventArgs e)
        {
            if (accion == "Editar")
            {
                Response.Redirect("PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "");
            }
            else
            {
                pnlPrincipal.Visible = true;
                pnlAporte.Visible = false;
            }
        }

        private void CargarControlesEditarAporte()
        {
            pnlPrincipal.Visible = false;
            pnlAporte.Visible = true;

            var query = (from p in consultas.Db.ProyectoAportes
                         where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                         p.Id_Aporte == Convert.ToInt32(idAporteEdita)
                         select p
                             ).First();
            txtNombreAporte.Text = query.Nombre;
            txtValorAporte.Text = query.Valor.ToString();
            ddlTipoAporte.SelectedValue = query.TipoAporte;
            txtDetalleAporte.Text = query.Detalle;
            btnCrearAporte.Text = "Actualizar";

        }

        protected void btnCrearAporte_Click(object sender, EventArgs e)
        {
            if (accion == "Editar")
            {
                var query = (from p in consultas.Db.ProyectoAportes
                             where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                             p.Id_Aporte == Convert.ToInt32(idAporteEdita)
                             select p
                      ).First();

                query.Nombre = txtNombreAporte.Text;
                query.Valor = Convert.ToDecimal(txtValorAporte.Text);
                query.TipoAporte = ddlTipoAporte.SelectedValue;
                query.Detalle = txtDetalleAporte.Text;
                consultas.Db.ExecuteCommand(UsuarioActual());
                consultas.Db.SubmitChanges();
                ObtenerDatosUltimaActualizacion();
                Response.Redirect("PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "");
            }
            else
            {

                Datos.ProyectoAporte datosNuevos = new ProyectoAporte()
                {
                    CodProyecto = Convert.ToInt32(codProyecto),
                    Nombre = txtNombreAporte.Text,
                    Valor = Convert.ToDecimal(txtValorAporte.Text),
                    TipoAporte = ddlTipoAporte.SelectedValue,
                    Detalle = txtDetalleAporte.Text
                };

                consultas.Db.ProyectoAportes.InsertOnSubmit(datosNuevos);
                consultas.Db.SubmitChanges();
                ObtenerDatosUltimaActualizacion();
                Response.Redirect("PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "");

            }

        }

        #endregion Aportes

        #region Recursos

        protected void CargarListadoTipoRecurso()
        {
            ddlTipoRecurso.Items.Add(new ListItem() { Value = Constantes.CONST_Credito, Text = Constantes.CONST_Dinero });
            ddlTipoRecurso.Items.Add(new ListItem() { Value = Constantes.CONST_Donancion, Text = Constantes.CONST_Bien });
        }

        protected void CargarTxtRecursosSolicitados()
        {

            try
            {
                var query = (from p in consultas.Db.ProyectoFinanzasIngresos
                             where p.CodProyecto == Convert.ToInt32(codProyecto)
                             select new { p.Recursos }).First();
                txtRecursosSolicitados.Text = query.Recursos.ToString();
            }
            catch
            {
                txtRecursosSolicitados.Text = "";
            }

        }

        protected void CargarRecursosCapital()
        {

            var query = (from p in consultas.Db.ProyectoRecursos
                         where p.CodProyecto == Convert.ToInt32(this.codProyecto)
                         orderby p.Tipo ascending
                         select new { p.Id_Recurso, p.Tipo, p.Cuantia, p.Plazo, p.Formapago, p.Interes, p.Destinacion });

            DataTable datos = new DataTable();
            datos.Columns.Add("cuantia");
            datos.Columns.Add("plazo");
            datos.Columns.Add("formaPago");
            datos.Columns.Add("intereses");
            datos.Columns.Add("destinacion");
            datos.Columns.Add("Id_Recurso");

            string tipoActual = "";
            double total = 0;
            foreach (var item in query)
            {
                if (tipoActual != item.Tipo)
                {
                    DataRow drTitulo = datos.NewRow();
                    drTitulo["cuantia"] = item.Tipo;
                    tipoActual = item.Tipo;
                    datos.Rows.Add(drTitulo);
                }

                DataRow dr = datos.NewRow();
                dr["cuantia"] = item.Cuantia.ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO")); ;
                dr["plazo"] = item.Plazo;
                dr["formaPago"] = item.Formapago;
                dr["intereses"] = item.Interes;
                dr["destinacion"] = item.Destinacion;
                dr["Id_Recurso"] = item.Id_Recurso;
                total += Convert.ToDouble(item.Cuantia);
                datos.Rows.Add(dr);

            }

            DataRow drTotal = datos.NewRow();
            drTotal["cuantia"] = "Total";
            drTotal["plazo"] = total.ToString("0,0.00", CultureInfo.CreateSpecificCulture("es-CO"));
            datos.Rows.Add(drTotal);

            gw_RecursosCapital.DataSource = datos;
            gw_RecursosCapital.DataBind();

            for (int i = 0; i < gw_RecursosCapital.Rows.Count; i++)
            {
                if (gw_RecursosCapital.Rows[i].Cells[4].Text != "&nbsp;" && gw_RecursosCapital.Rows[i].Cells[1].Text != "Total")
                {
                    if (esMiembro == true && usuario.CodGrupo == Constantes.CONST_Emprendedor && bRealizado == false)
                    {
                        string id_Recurso = gw_RecursosCapital.DataKeys[i].Value.ToString();
                        gw_RecursosCapital.Rows[i].Cells[1].Text = "<a href='PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "&idRecurso=" + id_Recurso + "&Accion=Editar&controlAccion=Recurso'>" + gw_RecursosCapital.Rows[i].Cells[1].Text + "</a>";
                    }
                    else
                    {
                        gw_RecursosCapital.Rows[i].Cells[0].Text = "";
                    }
                }
                else
                {
                    gw_RecursosCapital.Rows[i].Cells[0].Text = "";
                    gw_RecursosCapital.Rows[i].Cells[1].Text = "<span class='TitulosRegistrosGrilla'>" + gw_RecursosCapital.Rows[i].Cells[1].Text + "</span>";
                }
            }
        }

        protected void btnAdicionarRecurso_Click(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = false;
            pnlRecurso.Visible = true;
        }

        //Borrar Recurso
        protected void gw_RecursosCapital_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Borrar")
            {
                string idRecurso = e.CommandArgument.ToString();
                consultas.Db.ExecuteCommand("delete from ProyectoRecurso where Id_Recurso={0}", Convert.ToInt32(idRecurso));
                ObtenerDatosUltimaActualizacion();
                CargarRecursosCapital();
                CargarAportesEmprendedores();
            }
        }

        protected void btnCancelarRecurso_Click(object sender, EventArgs e)
        {
            if (accion == "Editar")
            {
                Response.Redirect("PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "");
            }
            else
            {
                pnlPrincipal.Visible = true;
                pnlRecurso.Visible = false;
            }
        }

        private void CargarControlesEditarRecurso()
        {

            pnlPrincipal.Visible = false;
            pnlRecurso.Visible = true;

            var query = (from p in consultas.Db.ProyectoRecursos
                         where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                         p.Id_Recurso == Convert.ToInt32(idRecursoEdita)
                         select p
                             ).First();

            ddlTipoRecurso.SelectedValue = query.Tipo;
            txtCuantiaRecurso.Text = query.Cuantia.ToString(); ;
            txtPlazoRecurso.Text = query.Plazo;
            txtFormaPagoRecurso.Text = query.Formapago;
            txtInteresRecurso.Text = query.Interes.ToString(); ;
            txtDestinacionRecurso.Text = query.Destinacion;
            btnCrearRecurso.Text = "Actualizar";
        }

        protected void btnCrearRecurso_Click(object sender, EventArgs e)
        {
            if (accion == "Editar")
            {
                var query = (from p in consultas.Db.ProyectoRecursos
                             where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                             p.Id_Recurso == Convert.ToInt32(idRecursoEdita)
                             select p
                      ).First();

                query.Tipo = ddlTipoRecurso.SelectedValue;
                query.Cuantia = Convert.ToDecimal(txtCuantiaRecurso.Text);
                query.Plazo = txtPlazoRecurso.Text;
                query.Formapago = txtFormaPagoRecurso.Text;
                query.Interes = Convert.ToInt16(txtInteresRecurso.Text);
                query.Destinacion = txtDestinacionRecurso.Text;

                consultas.Db.SubmitChanges();
                Response.Redirect("PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "");
            }
            else
            {
                #region LINQ Defectuoso.
                //Datos.ProyectoRecurso datosNuevos = new ProyectoRecurso()
                //{
                //    CodProyecto = Convert.ToInt32(codProyecto),
                //    Tipo = ddlTipoRecurso.SelectedValue,
                //    Cuantia = Convert.ToDecimal(txtCuantiaRecurso.Text),
                //    Plazo = txtPlazoRecurso.Text,
                //    Formapago = txtFormaPagoRecurso.Text,
                //    Interes = Convert.ToInt16(txtInteresRecurso.Text),
                //    Destinacion = txtDestinacionRecurso.Text
                //};

                //consultas.Db.ProyectoRecursos.InsertOnSubmit(datosNuevos);
                //consultas.Db.SubmitChanges(); 
                #endregion

                #region Versión SQL.

                if (txtInteresRecurso.Text.Trim() == "") { txtInteresRecurso.Text = "0"; }
                if (codProyecto == "") { return; }

                string txtSQL = " INSERT INTO ProyectoRecurso (Tipo, Cuantia, Plazo, Formapago, Interes, Destinacion, CodProyecto) " +
                                " VALUES ('" + ddlTipoRecurso.SelectedValue + "', " + txtCuantiaRecurso.Text.Trim() + ", '"
                                + txtPlazoRecurso.Text.Trim() + "', '" + txtFormaPagoRecurso.Text.Trim() + "', " + txtInteresRecurso.Text + ", '" +
                                txtDestinacionRecurso.Text + "', " + codProyecto + ") ";
                ejecutaReader(txtSQL, 2);

                #endregion

                Response.Redirect("PProyectoFinanzasIngreso.aspx?CodProyecto=" + codProyecto + "");

            }
        }

        #endregion Recursos

        #region Links.

        /// <summary>
        /// Descargar modelo financiero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLinkBajarModeloFinanciero_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            ClientScriptManager cm = this.ClientScript;

            //Instanciar la ruta.
            string ruta = ConfigurationManager.AppSettings.Get("RutaDocumentos") + @"\FORMATOSFINANCIEROS.xls";

            if (File.Exists(ruta))
            { DescargarArchivo(ruta); }
            else
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El archivo no existe.');</script>");
                return;
            }
        }

        protected void btnLinkSubirModeloFinanciero_Click(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = false;
            pnlCargueDocumento.Visible = true;
            string ruta = ConfigurationManager.AppSettings.Get("RutaDocumentosProyecto") + Math.Abs(Convert.ToInt32(codProyecto) / 2000) + @"\Proyecto_" + codProyecto + @"\";
            string archivo = @"\FORMATOSFINANCIEROS" + codProyecto;
            CargarArchivos1.show(new string[] { "xls" }, ruta, archivo);
        }

        /// <summary>
        /// Ver el modelo financiero (archivo en Excel).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLinkVerModeloFinanciero_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            ClientScriptManager cm = this.ClientScript;

            string ruta = ConfigurationManager.AppSettings.Get("RutaDocumentosProyecto") + Math.Abs(Convert.ToInt32(codProyecto) / 2000) + @"\Proyecto_" + codProyecto + @"\FORMATOSFINANCIEROS" + codProyecto + ".xls";
            if (File.Exists(ruta))
            {
                DescargarArchivo(ruta);
            }
            else
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El archivo no existe.');</script>");
                return;
            }
        }

        /// <summary>
        /// Descargar la guía para el modelo financiero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLinkGuiaLlenarModeloFinanciero_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            ClientScriptManager cm = this.ClientScript;

            //Instancia la ruta.
            string ruta = ConfigurationManager.AppSettings.Get("RutaDocumentos") + "GuiaModelo.doc";

            if (File.Exists(ruta))
            { DescargarArchivo(ruta); }
            else
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El archivo no existe.');</script>");
                return;
            }
        }

        #endregion links

        #region Handlers Control Cargar

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/05/2014.
        /// Subir archivo, dependiendo del resultado almacenado en la variable "respuesta.Mensaje" procesará la información del
        /// formulario y del archivo adjunto, si el valor en la variable "respuesta.Mensaje" es "OK", lo procesará, de lo contrario,
        /// mostrará el valor guardado en la variable "respuesta.Mensaje".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SubirArchivo_Click(object sender, CommandEventArgs e)
        {
            //Inicializar variables.
            ClientScriptManager cm = this.ClientScript;

            string rutaHttpDestino = "Documentos/Proyecto/" + Math.Abs(Convert.ToInt32(codProyecto) / 2000) + "/" + "Proyecto_" + codProyecto + "/" + "FORMATOSFINANCIEROS" + codProyecto + ".xls";

            Controles.RespuestaCargue respuesta = CargarArchivos1.Resultado();
            if (respuesta.Mensaje == "OK")
            {
                #region Procesar la infomación del formulario y del archivo adjunto.

                var query = (from d in consultas.Db.Documentos
                             where d.CodTab == txtTab &&
                             d.CodProyecto == Convert.ToInt32(codProyecto) &&
                            d.NomDocumento.Contains("Formato Financieros")
                             select d
                                 );
                if (query.ToList().Count == 0)
                {
                    Datos.Documento datosNuevos = new Documento()
                    {
                        CodProyecto = Convert.ToInt32(codProyecto),
                        NomDocumento = "Formato Financieros",
                        CodDocumentoFormato = 3,
                        URL = rutaHttpDestino,
                        Comentario = "Formato Financieros",
                        Fecha = DateTime.Now,
                        CodContacto = usuario.IdContacto,
                        CodTab = (Int16?)txtTab
                    };

                    consultas.Db.Documentos.InsertOnSubmit(datosNuevos);
                    consultas.Db.SubmitChanges();
                }
                else
                {
                    var queryUpdate = query.First();
                    queryUpdate.Fecha = DateTime.Now;
                    queryUpdate.URL = rutaHttpDestino;
                    queryUpdate.CodDocumentoFormato = 3;
                    queryUpdate.Borrado = false;
                    queryUpdate.CodContacto = usuario.IdContacto;
                    consultas.Db.SubmitChanges();

                }
                pnlPrincipal.Visible = true;
                pnlCargueDocumento.Visible = false;

                #endregion
            }
            else
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Atención: " + respuesta.Mensaje + "');</script>");
                return;
            }
        }

        void CancelarArchivo_Click(object sender, CommandEventArgs e)
        {
            pnlPrincipal.Visible = true;
            pnlCargueDocumento.Visible = false;
        }
        #endregion Handlers Control Cargar

        #region Otros métodos.

        protected void gw_IngresosVentas_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Establecer diseño en code-behind.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gw_IngresosVentas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
                for (int i = 1; i < txtTiempoProyeccion + 1; i++)
                { e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right; }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
                for (int i = 1; i < txtTiempoProyeccion + 1; i++)
                { e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right; }
            }
        }

        #endregion

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

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + codProyecto + " And CodContacto = " + usuario.IdContacto +
                         " and inactivo=0 and FechaInicio<=getdate() and FechaFin is null ";

                //Asignar variables a DataTable.
                var rs = consultas.ObtenerDataTable(txtSQL, "text");

                //Crear la variable de sesión.
                if (rs.Rows.Count > 0) { Session["CodRol"] = rs.Rows[0]["CodRol"].ToString(); }
                else { Session["CodRol"] = ""; }

                //Destruir la variable.
                rs = null;

                #endregion

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
                    string hora = fecha.ToString("hh:mm:ss tt", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")).ToLowerInvariant();

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
                return;
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
        /// Mauricio Arias Olave.
        /// 11/09/2014.
        /// Eliminar emprendedores.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gw_AporteEmprendedores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var btn = e.Row.FindControl("btn_BorrarEmprendedor") as ImageButton;

                if (btn != null)
                {
                    if (!(esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado))
                    { btn.Style.Add(HtmlTextWriterStyle.Display, "none"); }
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 11/09/2014.
        /// Eliminar recursos de capital.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gw_RecursosCapital_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var btn = e.Row.FindControl("btn_BorrarRecursosCapital") as ImageButton;

                if (btn != null)
                {
                    if (!(esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado))
                    { btn.Style.Add(HtmlTextWriterStyle.Display, "none"); }
                }
            }
        }
    }

    public class BORespuestaDetalleProducto
    {
        public decimal Unidades { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public int anio { get; set; }
    }
}
