
#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Negocio;
using System.Configuration;


#endregion

namespace Fonade.FONADE.interventoria
{
    public partial class FramePlanOperativoInterventoria : Base_Page
    {
        public const int Mes = 12;
        public int prorroga;
        public int prorrogaTotal;
        /// <summary>
        /// Valor que define el estado de la página actual, así como el tipo de pago "variable"
        /// a enviar a las páginas "PagosActividad.aspx" y "PagosActividadInter.aspx".
        /// </summary>
        public int Estado;
        /// <summary>
        /// Obtiene el valor establecido según la pestaña activa.
        /// </summary>
        public int txtTab;

        #region propiedades

        //private int CodEmpresa
        //{
        //    get { return (int)ViewState["empresa"]; }
        //    set { ViewState["empresa"] = value; }
        //}
        private int codActividad
        {
            get { return (int)ViewState["CodActividad"]; }
            set { ViewState["CodActividad"] = value; }
        }
        private string NomActividad
        {
            get { return (string)ViewState["NomActividad"]; }
            set { ViewState["NomActividad"] = value; }
        }
        private int Prorroga
        {
            get { return (int)ViewState["prorroga"]; }
            set { ViewState["prorroga"] = value; }
        }
        /* private Int32 Prorroga
         {
             get { return prorroga; }
             set { prorroga = value; }
         }*/
        private int CodProyecto
        {
            get { return (int)ViewState["proyecto"]; }
            set { ViewState["proyecto"] = value; }
        }

        public string Sfondo1F
        {
            get { return ViewState["sfondo1f"].ToString(); }
            set { ViewState["sfondo1f"] = value; }
        }

        public string Sfondo2F
        {
            get { return ViewState["sfondo2F"].ToString(); }
            set { ViewState["sfondo2F"] = value; }
        }

        public string Sfondo3F
        {
            get { return ViewState["sfondo3F"].ToString(); }
            set { ViewState["sfondo3F"] = value; }
        }

        public string Sfondo4F
        {
            get { return ViewState["sfondo4F"].ToString(); }
            set { ViewState["sfondo4F"] = value; }
        }

        public string Sfondo5F
        {
            get { return ViewState["sfondo5F"].ToString(); }
            set { ViewState["sfondo5F"] = value; }
        }
        public string Sfondo6F
        {
            get { return ViewState["sfondo6F"].ToString(); }
            set { ViewState["sfondo6F"] = value; }
        }
        public string Sfondo7F
        {
            get { return ViewState["sfondo7F"].ToString(); }
            set { ViewState["sfondo7F"] = value; }
        }
        public string Sfondo8F
        {
            get { return ViewState["sfondo8F"].ToString(); }
            set { ViewState["sfondo8F"] = value; }
        }
        public string Sfondo9F
        {
            get { return ViewState["sfondo9F"].ToString(); }
            set { ViewState["sfondo9F"] = value; }
        }

        public string Sfondo10F
        {
            get { return ViewState["sfondo10F"].ToString(); }
            set { ViewState["sfondo10F"] = value; }
        }
        public string Sfondo11F
        {
            get { return ViewState["sfondo11F"].ToString(); }
            set { ViewState["sfondo11F"] = value; }
        }

        public string Sfondo12F
        {
            get { return ViewState["sfondo12F"].ToString(); }
            set { ViewState["sfondo12F"] = value; }
        }
        public string Sfondo13F
        {
            get { return ViewState["sfondo13F"].ToString(); }
            set { ViewState["sfondo13F"] = value; }
        }
        public string Sfondo14F
        {
            get { return ViewState["sfondo14F"].ToString(); }
            set { ViewState["sfondo14F"] = value; }
        }


        private int Mostrar { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                CodProyecto =
                            (int)
                            (!string.IsNullOrEmpty(Session["CodProyecto"].ToString())
                                 ? Convert.ToInt64(Session["CodProyecto"])
                                 : 0);
                //NO SE USA!
                //CodEmpresa =
                //    (int)
                //    (!string.IsNullOrEmpty(Session["CodEmpresa"].ToString())
                //         ? Convert.ToInt64(Session["CodEmpresa"])
                //         : 0);

                #region GENERAR GRILLA DINÁMICA DESCOMENTADO.
                //Nómina seleccionada.
                string interno_Codactividad = Session["interno_Codactividad"] != null && !string.IsNullOrEmpty(Session["interno_Codactividad"].ToString()) ? Session["interno_Codactividad"].ToString() : "0";
                if (interno_Codactividad != "0")
                {
                    GenerarTabla(interno_Codactividad, CodProyecto.ToString());
                }
                #endregion

                #region Establecer el "Estado" y el "txtTab" de la ventana actual.

                switch (1) //Plan Operativo / Default.
                {
                    case 2:
                        txtTab = Constantes.CONST_NominaInter;
                        Estado = 2;
                        break;
                    case 3:
                        txtTab = Constantes.CONST_ProduccionInter;
                        Estado = 3;
                        break;
                    case 4:
                        txtTab = Constantes.CONST_VentasInter;
                        Estado = 4;
                        break;
                    default:
                        txtTab = Constantes.CONST_PlanOperativoInter2;
                        Estado = 1;
                        break;
                }

                #endregion

                if (!IsPostBack)
                {
                    lblvalidador.Text = CodProyecto.ToString();
                    ValidarPerfil();
                    CargarGridActividades();
                }
                else
                {
                    //Evaluar si ciertos campos se mostrarán.
                    EvaluarCampos(usuario.CodGrupo);
                }

                //CargarConteoActividadesNoAprobadas();

                //Evaluar si ciertos campos se mostrarán.
                EvaluarCampos(usuario.CodGrupo);
            }
            catch (Exception)
            {
                //Response.Redirect("~/Account/Login.aspx");
            }
        }

        private void ValidarPerfil()
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
                //divadicionar.Visible = true;
                LinkButton1.Visible = true;
                lnkPagos.Visible = true;
                pnlActividades.Visible = true;
            }
            else
            {
                //divadicionar.Visible = false;
                LinkButton1.Visible = false;
                //lnkPagos.Visible = false;
                pnlActividades.Visible = false;
            }

            var prorrogaC = consultas.Db.ProyectoProrrogas.FirstOrDefault(pr => pr.CodProyecto == CodProyecto);

            if (prorrogaC != null)
            { Prorroga = prorrogaC.Prorroga; }
            else
            { Prorroga = 0; }
        }

        protected void CargarGridActividades()
        {
            try
            {
                consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@codProyecto",
                                                       Value = CodProyecto
                                                   }
                                           };
                DataSet query = consultas.ObtenerDataSet("MD_ObtenerItems_Interventor");

                if (query.Tables.Count != 0)
                {
                    Session["dtitems"] = query.Tables[0];
                    gw_Anexos.DataSource = query.Tables[0];
                    gw_Anexos.DataBind();

                    if (query.Tables[1].Rows.Count != 0)
                    {
                        Session["dtActividades"] = query.Tables[1];
                        GrvActividadesNoAprovadas.DataSource = query.Tables[1];
                        lblpuestosPendientesConteo.Text = "Actividades Pendientes de Aprobar: " + query.Tables[1].Rows.Count;
                        GrvActividadesNoAprovadas.DataBind();
                    }
                }
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        private void CargarDetalle(string parametros)
        {
            var variables = new string[] { };

            if (!string.IsNullOrEmpty(parametros))
            {
                variables = parametros.Split(';');
            }

            if (!string.IsNullOrEmpty(variables.ToString()))
            {
                lblnomactividad.Text = variables[1];

                var respuestaDetalle = new DataTable();
                var prorroga = new DataTable();
                try
                {
                    #region GENERAR GRILLA DINÁMICA DESCOMENTADO.
                    //Llamar a método que generará la tabla.
                    //Se generan las variables en sesion para llamar a este método 
                    Session["interno_Codactividad"] = variables[0].ToString();
                    GenerarTabla(variables[0], CodProyecto.ToString());

                    #endregion
                }
                catch { return; }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/09/2014.
        /// Método dinámico que de acuerdo a la acción "actualizar, reportar", invoca la ventana
        /// emergente del catálogo de la actividad del plan operativo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DynamicCommand_VerAvance(Object sender, CommandEventArgs e)
        {
            try
            {
                //Inicializar variables.
                var valores_command = new string[] { };
                valores_command = e.CommandArgument.ToString().Split(';');

                #region Comentarios NO BORRAR!
                //System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Usted seleccionó: \nCommandName: " + e.CommandName + ".\n0: " + valores_command[0] + ".\n 1: " + valores_command[1] + ".\n 2: " + valores_command[2] + ".\n 3: " + valores_command[3] + "')", true);
                //return; 
                #endregion

                Session["Accion"] = valores_command[0].ToString(); //"actualizar";
                Session["proyecto"] = valores_command[1].ToString();
                Session["CodActividad"] = valores_command[2].ToString();
                //Ya se le está enviado la nómina seleccionada en una variable de sesión.
                Session["linkid"] = valores_command[3].ToString();
                //Ya se le está enviando el nombre del cargo seleccionado en una variable de sesión.
                Session["pagina"] = "Nomina";
                Redirect(null, "../interventoria/CatalogoActividadPOInterventor.aspx", "_blank",
                           "menubar=0,scrollbars=1,width=980,height=400,top=100");
            }
            catch { }
        }

        #region NO BORRAR
        //public void GenerarTabla(String Cod_Actividad, String Cod_Proyecto)
        //{
        //    //Inicializar variables.
        //    String txtSQL = "";
        //    String nomCargo;
        //    Double TotalFE = 0;
        //    Double TotalEmp = 0;
        //    DataTable rsActividad = new DataTable();
        //    DataTable contador = new DataTable();
        //    DataTable rsTipo1 = new DataTable();
        //    DataTable rsTipo2 = new DataTable();
        //    DataTable rsPagoActividad = new DataTable();
        //    int ejecutar = 0;
        //    #region Obtener el valor de la prórroga para sumarla a la constante de meses generar la tabla.
        //    prorroga = 0;
        //    prorrogaTotal = 0;
        //    prorroga = ObtenerProrroga(CodProyecto.ToString());
        //    if (prorroga == 0)
        //    { prorrogaTotal = prorroga + Constantes.CONST_Meses; /*El +1 es para evitar modificar aún mas el for...*/ }
        //    else { prorrogaTotal = prorroga + Constantes.CONST_Meses; }
        //    /*int prorroga = 0;
        //    prorroga = ObtenerProrroga(CodProyecto.ToString());
        //    int prorrogaTotal = prorroga + Constantes.CONST_Meses; */
        //    //El +1 es paar evitar modificar aún mas el for...
        //    #endregion

        //    try
        //    {
        //        //Inicializar tabla.
        //        t_anexos.Rows.Clear();

        //        //Inicializar la fila.
        //        TableRow fila = new TableRow();
        //        fila.Style.Add("text-align", "center");

        //        #region Generar la primera fila con los meses que tiene la nómina seleccionada.
        //        for (int i = 1; i <= prorrogaTotal; i++)
        //        {
        //            TableHeaderCell celda = new TableHeaderCell();
        //            celda.Style.Add("text-align", "center");
        //            celda.ColumnSpan = 2;
        //            celda.Text = "Mes " + i;
        //            fila.Cells.Add(celda);
        //            t_anexos.Rows.Add(fila);
        //            celda = null;
        //        }
        //        #endregion

        //        #region Crear una nueva celda que contiene el valor "Costo Total".
        //        TableHeaderCell celdaCostoTotal = new TableHeaderCell();
        //        celdaCostoTotal.Text = "Costo Total";
        //        celdaCostoTotal.Style.Add("text-align", "center");
        //        celdaCostoTotal.ColumnSpan = 2;
        //        fila.Cells.Add(celdaCostoTotal);
        //        t_anexos.Rows.Add(fila);
        //        celdaCostoTotal = null;
        //        #endregion

        //        #region Agregar nueva fila (para adicionar las celdas "Sueldo" y "Prestaciones").
        //        //Se obtiene la cantidad de celdas que tiene la primera fila para generar los Sueldos y las Prestaciones.
        //        int conteo_celdas = fila.Cells.Count + 1; //El +1 es para contar también la celda "Costo Total".
        //        //Se inicializa la variable para generar una nueva fila.
        //        fila = new TableRow();

        //        //Generar las celdas "Sueldo" y "Prestaciones".
        //        for (int i = 1; i < conteo_celdas; i++)
        //        {
        //            //Celdas "Sueldo" y "Prestaciones Sociales".
        //            TableHeaderCell celdaSueldo = new TableHeaderCell();
        //            celdaSueldo.Style.Add("text-align", "left");
        //            TableHeaderCell celdaPrestaciones = new TableHeaderCell();
        //            celdaPrestaciones.Style.Add("text-align", "left");

        //            //Agregar datos a la celda de Sueldo.
        //            celdaSueldo.Text = "fondo";
        //            fila.Cells.Add(celdaSueldo);
        //            t_anexos.Rows.Add(fila);
        //            celdaSueldo = null;

        //            //Agregar datos a la celda de Prestaciones Sociales.
        //            celdaPrestaciones.Text = "Emprendedor";
        //            fila.Cells.Add(celdaPrestaciones);
        //            t_anexos.Rows.Add(fila);
        //            celdaPrestaciones = null;
        //        }
        //        #endregion

        //        #region Personal calificado - Cargos.
        //        //Personal calificado - Cargos.
        //        txtSQL = " SELECT DISTINCT ISNULL(pm.CodActividad,0) CodActividad,ISNULL(pm.Mes,0) Mes,ISNULL(pm.CodTipoFinanciacion,0) as CodTipoFinanciacion,ISNULL(CONVERT(VARCHAR(2000),CAST(pm.Valor AS DECIMAL),1) ,0.0) Valor,pin.Id_Actividad,pin.NomActividad,pin.CodProyecto,pin.Item,pin.Metas " +
        //                                  " FROM ProyectoActividadPOMesInterventor pm RIGHT OUTER JOIN proyectoactividadPOInterventor pin On pin.id_actividad= pm.CodActividad " +
        //                                  " Where pin.codproyecto=" + Cod_Proyecto + "  AND pin.Id_actividad = " + Cod_Actividad +
        //                                  " ORDER BY item, mes,codtipofinanciacion ";


        //        /* " WHERE a.Tipo='Cargo' AND a.Id_Nomina = b.CodCargo AND a.CodProyecto = " + Cod_Proyecto +
        //           " AND b.Mes <> 0 and a.Id_Nomina = " + codActividad + " " +
        //           " ORDER BY a.Id_Nomina, b.Mes, b.Tipo";*/


        //        //Asignar resultados de la consulta anterior a variable DataTable.
        //        rsActividad = consultas.ObtenerDataTable(txtSQL, "text");

        //        #endregion

        //        #region Contador...
        //        //Contador...
        //        /*  txtSQL = "SELECT count(*) AS contador " +
        //                      "FROM InterventorNomina " +
        //                      "WHERE tipo='Cargo'";

        //          //Asignar resultados de la consulta anterior a variable DataTable.
        //          contador = consultas.ObtenerDataTable(txtSQL, "text");*/
        //        contador = consultas.ObtenerDataTable(txtSQL, "text");
        //        #endregion

        //        //Crear variable temporal que contiene el código del cargo "asignable mas adelante".
        //        string CodCargo = "";

        //        if (rsActividad.Rows.Count > 0)
        //        {
        //            #region Agregar nueva fila con espacio separador "igual como lo deja FONADE clásico.

        //            //Inicializar la fila.
        //            fila = new TableRow();
        //            TableCell celdaEspacio = new TableCell();
        //            celdaEspacio.Text = "&nbsp;";
        //            fila.Cells.Add(celdaEspacio);
        //            t_anexos.Rows.Add(fila);

        //            #endregion


        //            //TableRow fila1 = new TableRow();
        //            //fila.Attributes.Add("align", "left");
        //            //fila.Attributes.Add("valign", "top");
        //            //Mientras que la consulta del personal calificado - Cargo - NO esté vacío.
        //            foreach (DataRow row in rsActividad.Rows)
        //            {

        //                //Asinar nombre del cargo a variable.
        //                nomCargo = row["NomActividad"].ToString();

        //                if (CodCargo != row["CodActividad"].ToString())
        //                {
        //                    #region Asigno la primera fila.
        //                    //Asignar valor de la fila a variable.
        //                    CodCargo = row["CodActividad"].ToString();
        //                }
        //                //Inicializar la nueva fila.

        //                fila = new TableRow();
        //                fila.Attributes.Add("align", "left");
        //                fila.Attributes.Add("valign", "top");

        //                if (row["Mes"].ToString() != null)
        //                {
        //                    #region Crear la primera fila que contiene valores numéricos.
        //                    /*for (int j = 1; j < Constantes.CONST_Meses + prorrogaTotal; j++) //Se coloca -1 para que descuente una celda.
        //                            {*/
        //                    if (row != null)
        //                    {
        //                        if (row["Mes"].ToString() != null)
        //                        {
        //                            #region Formatear el valor y agregarlo a la fila.
        //                            //Formatear el valor.
        //                            decimal valor = 0;
        //                            string valor_formateado = "";

        //                            valor = decimal.Parse(row["Valor"].ToString());
        //                            valor_formateado = "$" + valor.ToString("0,0.00", CultureInfo.InvariantCulture);



        //                            //Agregar celda con valor formateado.
        //                            TableCell celdaValor = new TableCell();
        //                            celdaValor.Text = valor_formateado;
        //                            celdaValor.Attributes.Add("align", "right");
        //                            fila.Cells.Add(celdaValor);
        //                            t_anexos.Rows.Add(fila);

        //                            #region NO BORRAR Código comentado, se reemplaza por funcionalidad multiplicada.
        //                            //switch (row["Tipo1"].ToString())
        //                            //{
        //                            //    case "1":
        //                            //        //TotalFE = TotalFE + Double.Parse(row["Valor"].ToString());
        //                            //        TotalFE = int.Parse(row["Mes"].ToString()) * double.Parse(row["Valor"].ToString());
        //                            //        break;
        //                            //    case "2":
        //                            //        TotalEmp = TotalEmp + Double.Parse(row["Valor"].ToString());
        //                            //        break;
        //                            //    default:
        //                            //        break;
        //                            //} 
        //                            #endregion

        //                            #region Generar celda vacía.
        //                            TableCell celdaVacia = new TableCell();
        //                            celdaVacia.Text = "&nbsp;";
        //                            celdaValor.Attributes.Add("align", "right");
        //                            fila.Cells.Add(celdaVacia);
        //                            t_anexos.Rows.Add(fila);
        //                            #endregion
        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            #region Generar celda vacía.
        //                            //TableCell celdaVacia = new TableCell();
        //                            //celdaVacia.Text = "&nbsp;";
        //                            //fila.Cells.Add(celdaVacia);
        //                            t_anexos.Rows.Add(fila);
        //                            #endregion
        //                        }

        //                    }
        //                    #region Costo total y cerrar fila.
        //                    //Formatear el valor de Fondo Emprender.
        //                    decimal valor_FE = 0;
        //                    string valor_formateado_FE = "";
        //                    DataRow lastRow = rsActividad.Rows[rsActividad.Rows.Count - 1];
        //                    #region Leer comentarios.
        //                    ////Porqué se tendrá que restar dos números???, si algo, toca cambiarlo por la consulta ya creada.
        //                    //CAMBIADO; se consulta aquí http://stackoverflow.com/questions/18528736/how-to-retrieve-values-from-the-last-row-in-a-datatable
        //                    //y se realiza el siguiente código:
        //                    //TotalFE = TotalFE - Double.Parse(rsCargo.Rows[0]["Valor"].ToString()) - Double.Parse(rsCargo.Rows[0]["Valor"].ToString()); 
        //                    #endregion
        //                    //Obtener el Total Emprendedor.
        //                    double multiplicar = Double.Parse(lastRow["Mes"].ToString()) * Double.Parse(lastRow["Valor"].ToString());
        //                    TotalFE = multiplicar;
        //                    valor_FE = decimal.Parse(TotalFE.ToString());
        //                    valor_formateado_FE = "$" + valor_FE.ToString("0,0.00", CultureInfo.InvariantCulture);

        //                    //Formatear el valor de Emprendimiento.
        //                    decimal valor_Emp = 0;
        //                    string valor_formateado_Emp = "";
        //                    valor_Emp = decimal.Parse(TotalEmp.ToString());
        //                    valor_formateado_Emp = "$" + valor_Emp.ToString("0,0.00", CultureInfo.InvariantCulture);


        //                    TableCell celdaCostoTotal_FE = new TableCell();
        //                    celdaCostoTotal_FE.Text = valor_formateado_FE;
        //                    celdaCostoTotal_FE.Attributes.Add("align", "right");
        //                    fila.Cells.Add(celdaCostoTotal_FE);
        //                    t_anexos.Rows.Add(fila);

        //                    //Emprendimiento.
        //                    TableCell celdaCostoTotal_Emp = new TableCell();
        //                    celdaCostoTotal_Emp.Text = valor_formateado_Emp;
        //                    celdaCostoTotal_Emp.Attributes.Add("align", "right");
        //                    fila.Cells.Add(celdaCostoTotal_Emp);
        //                    t_anexos.Rows.Add(fila);
        //                    #endregion

        //                    #endregion

        //                }
        //                else
        //                {
        //                    #region prorrogaTotal * Constantes.CONST_Fuentes
        //                    //for (int j = 0; j < prorrogaTotal * Constantes.CONST_Fuentes; j++)
        //                    //{
        //                    //    #region Generar celda vacía.
        //                    //    TableCell celdaVacia = new TableCell();
        //                    //    celdaVacia.Attributes.Add("align", "right");
        //                    //    celdaVacia.Text = "&nbsp;";
        //                    //    fila.Cells.Add(celdaVacia);
        //                    //t_anexos.Rows.Add(fila);
        //                    //    #endregion
        //                    //}

        //                    #region Costo total y cerrar fila.
        //                    //Formatear el valor de Fondo Emprender.
        //                    decimal valor_FE = 0;
        //                    string valor_formateado_FE = "";
        //                    valor_FE = decimal.Parse(TotalFE.ToString());
        //                    valor_formateado_FE = "$" + valor_FE.ToString("0,0.00", CultureInfo.InvariantCulture);

        //                    //Formatear el valor de Emprendimiento.
        //                    decimal valor_Emp = 0;
        //                    string valor_formateado_Emp = "";
        //                    valor_Emp = decimal.Parse(TotalEmp.ToString());
        //                    valor_formateado_Emp = "$" + valor_Emp.ToString("0,0.00", CultureInfo.InvariantCulture);

        //                    //Fondo Emprender.
        //                    TableCell celdaCostoTotal_FE = new TableCell();
        //                    celdaCostoTotal_FE.Attributes.Add("align", "right");
        //                    celdaCostoTotal_FE.Text = valor_formateado_FE;
        //                    fila.Cells.Add(celdaCostoTotal_FE);
        //                    t_anexos.Rows.Add(fila);

        //                    //Emprendimiento.
        //                    TableCell celdaCostoTotal_Emp = new TableCell();
        //                    celdaCostoTotal_Emp.Text = valor_formateado_Emp;
        //                    celdaCostoTotal_Emp.Attributes.Add("align", "right");
        //                    fila.Cells.Add(celdaCostoTotal_Emp);
        //                    t_anexos.Rows.Add(fila);
        //                    #endregion
        //                    #endregion
        //                }
        //                    #endregion

        //                #region Asigno la segunda fila.

        //                //Avances reportados.

        //                //Inicializar la nueva fila.
        //                //fila = new TableRow();
        //                fila.Attributes.Add("bgcolor", "#EDF1F8");

        //                //Inicializar variables internas.
        //                double TotalTipo1 = 0;
        //                double TotalTipo2 = 0;
        //                double d_Tipo1 = 0;
        //                double d_Tipo2 = 0;
        //                string Tipo1 = "";
        //                string Tipo2 = "";

        //                for (int j = 1; j < conteo_celdas - 1; j++) //Se coloca -1 para que descuente una celda.
        //                {
        //                    #region Tipo 1.
        //                    //Consulta de tipo 1.
        //                    txtSQL = " select * " +
        //                             " from AvanceActividadPOMes " +
        //                             " where codactividad = " + Cod_Actividad +
        //                             " and Mes = " + j + " and codtipofinanciacion = 1 ";


        //                    //Asignar resultados de la consulta de tipo 1.
        //                    rsTipo1 = consultas.ObtenerDataTable(txtSQL, "text");

        //                    //Si la consulta tiene datos.
        //                    if (rsTipo1.Rows.Count > 0)
        //                    {
        //                        d_Tipo1 = Double.Parse(rsTipo1.Rows[0]["Valor"].ToString());
        //                        Tipo1 = "$" + d_Tipo1.ToString("0,0.00", CultureInfo.InvariantCulture);
        //                        TotalTipo1 = TotalTipo1 + d_Tipo1;
        //                    }
        //                    else
        //                    {
        //                        d_Tipo1 = 0;
        //                        Tipo1 = "&nbsp;";
        //                    }

        //                    //Generar la celda.
        //                    TableCell celdaInterna_Tipo1 = new TableCell();
        //                    celdaInterna_Tipo1.Attributes.Add("align", "right");
        //                    celdaInterna_Tipo1.Text = "<font color=\"#CC0000\">" + Tipo1 + "</font>";
        //                    fila.Cells.Add(celdaInterna_Tipo1);

        //                    #endregion

        //                    #region Tipo 2.
        //                    //Consulta de Tipo 2.
        //                    txtSQL = " select * " +
        //                             " from AvanceActividadPOMes " +
        //                             " where codactividad = " + Cod_Actividad +
        //                             " and Mes = " + j + " and codtipofinanciacion = 2 ";

        //                    //Asignar resultados de la consulta de Tipo 2.
        //                    rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");

        //                    //Si la consulta tiene datos.
        //                    if (rsTipo2.Rows.Count > 0)
        //                    {
        //                        d_Tipo2 = Double.Parse(rsTipo2.Rows[0]["Valor"].ToString());
        //                        Tipo2 = "$" + d_Tipo2.ToString("0,0.00", CultureInfo.InvariantCulture);
        //                        TotalTipo2 = TotalTipo2 + d_Tipo2;
        //                    }
        //                    else
        //                    {
        //                        d_Tipo2 = 0;
        //                        Tipo2 = "&nbsp;";
        //                    }

        //                    //Generar la celda.
        //                    TableCell celdaInterna_Tipo2 = new TableCell();
        //                    celdaInterna_Tipo2.Attributes.Add("align", "right");
        //                    celdaInterna_Tipo2.Text = "<font color=\"#CC0000\">" + Tipo2 + "</font>";
        //                    fila.Cells.Add(celdaInterna_Tipo2);

        //                    #endregion
        //                }

        //                #region Costo Total de Avances reportados.

        //                //Formatear el valor de Avance de Tipo 1.
        //                decimal valor_Tipo1 = 0;
        //                string valor_formateado_Tipo1 = "";
        //                //TotalTipo1 = TotalTipo1 - Double.Parse(rsTipo1.Rows[0]["Valor"].ToString());
        //                valor_Tipo1 = decimal.Parse(TotalTipo1.ToString());
        //                valor_formateado_Tipo1 = "$" + valor_Tipo1.ToString("0,0.00", CultureInfo.InvariantCulture);

        //                //Formatear el valor de Avance de Tipo 2.
        //                decimal valor_Tipo2 = 0;
        //                string valor_formateado_Tipo2 = "";
        //                valor_Tipo2 = decimal.Parse(TotalTipo2.ToString());
        //                valor_formateado_Tipo2 = "$" + valor_Tipo2.ToString("0,0.00", CultureInfo.InvariantCulture);

        //                //Tipo 1.
        //                TableCell celdaCostoTotal_Tipo1 = new TableCell();
        //                celdaCostoTotal_Tipo1.Attributes.Add("align", "right");
        //                celdaCostoTotal_Tipo1.Text = "<font color=\"#CC0000\">" + valor_formateado_Tipo1 + "</font>";
        //                fila.Cells.Add(celdaCostoTotal_Tipo1);
        //                t_anexos.Rows.Add(fila);

        //                //Tipo 2.
        //                TableCell celdaCostoTotal_Tipo2 = new TableCell();
        //                celdaCostoTotal_Tipo2.Attributes.Add("align", "right");
        //                celdaCostoTotal_Tipo2.Text = "<font color=\"#CC0000\">" + valor_formateado_Tipo2 + "</font>";
        //                fila.Cells.Add(celdaCostoTotal_Tipo2);
        //                t_anexos.Rows.Add(fila);
        //                #endregion

        //                //Añadir la fila a la tabla.
        //                t_anexos.Rows.Add(fila);

        //                #endregion

        //                #region Asigno la tercera fila.

        //                //Reportar avances.

        //                //Inicializar la nueva fila.
        //                //fila = new TableRow();
        //                fila.Attributes.Add("bgcolor", "#EDF1F8");

        //                int mes = 0;

        //                for (int j = 1; j < conteo_celdas - 1; j++) //Se coloca -1 para que descuente una celda.
        //                {
        //                    #region Consultar AvanceCargoPOMes
        //                    mes = j;

        //                    txtSQL = " SELECT * " +
        //                             " FROM AvanceActividadPOMes " +
        //                             " WHERE codactividad=" + Cod_Actividad +
        //                             " AND mes=" + j;

        //                    rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");
        //                    #endregion

        //                    #region Establecer si se puede o no ejecutar.
        //                    if (rsTipo2.Rows.Count > 0)
        //                    {
        //                        ejecutar = 1; //Si existe se coloca la opción de editar y borrar.
        //                    }
        //                    else
        //                    {
        //                        ejecutar = 2; //Si NO existe se coloca la opción de adicionar.
        //                    }
        //                    #endregion

        //                    //Agregar celda.
        //                    TableCell celdaCentrada = new TableCell();
        //                    celdaCentrada.Attributes.Add("colspan", "2");
        //                    celdaCentrada.Attributes.Add("align", "center");

        //                    try { nomCargo = nomCargo.Replace("+", "$"); }
        //                    catch { }

        //                    if (ejecutar == 1)
        //                    {
        //                        #region Condición de "ObservacionesInterventor". Aquí es donde se agrega los botones de "Ver Avance".
        //                        if (!String.IsNullOrEmpty(rsTipo2.Rows[0]["ObservacionesInterventor"].ToString()))
        //                        {
        //                            #region Reportar avance.//Ver avance.
        //                            ImageButton img_VerAvance = new ImageButton();
        //                            LinkButton lnk_VerAvance = new LinkButton();

        //                            //ImageButton.
        //                            img_VerAvance.ID = "img_VerAvance_" + j.ToString();
        //                            img_VerAvance.ImageUrl = "~/Images/icoAdicionarUsuario.gif";
        //                            img_VerAvance.AlternateText = "Avance";
        //                            img_VerAvance.CommandName = "VerAvance";
        //                            img_VerAvance.CommandArgument = "Editar" + ";" + Cod_Proyecto + ";" + Cod_Actividad + ";" + j + ";" + nomCargo;
        //                            img_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
        //                            if (usuario.CodGrupo != Constantes.CONST_Interventor)
        //                            { img_VerAvance = null; }
        //                            else
        //                            { celdaCentrada.Controls.Add(img_VerAvance); }

        //                            //LinkButton.
        //                            lnk_VerAvance.ID = "lnk_VerAvance_" + j.ToString();
        //                            lnk_VerAvance.Text = "<b>&nbsp;Ver Avance</b>";
        //                            lnk_VerAvance.Style.Add("text-decoration", "none");
        //                            lnk_VerAvance.CommandName = "VerAvance";
        //                            lnk_VerAvance.CommandArgument = "Editar" + ";" + Cod_Proyecto + ";" + Cod_Actividad + ";" + j + ";" + nomCargo;
        //                            lnk_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
        //                            if (usuario.CodGrupo != Constantes.CONST_Interventor)
        //                            { lnk_VerAvance = null; }
        //                            else
        //                            { celdaCentrada.Controls.Add(lnk_VerAvance); }
        //                            #endregion
        //                        }

        //                        #endregion
        //                    }
        //                    if (celdaCentrada.Controls.Count == 0) { celdaCentrada.Text = "&nbsp;"; }
        //                    fila.Cells.Add(celdaCentrada);
        //                    t_anexos.Rows.Add(fila);
        //                    celdaCentrada = null;
        //                }

        //                //Al terminar el for...
        //                //Agregar celda con espacio.
        //                TableCell celda_Espacio = new TableCell();
        //                celda_Espacio.Attributes.Add("colspan", "2");
        //                celda_Espacio.Text = "&nbsp;";
        //                fila.Cells.Add(celda_Espacio);
        //                t_anexos.Rows.Add(fila);
        //                celda_Espacio = null;

        //                //Final.

        //                #endregion

        //            }

        //        }

        //        //Bindear finalmente la grilla.
        //        t_anexos.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
        //        t_anexos.Visible = false;
        //    }
        //} 
        #endregion

        private void GenerarTabla(String codActividad, String Cod_Proyecto)
        {
            //Inicializar variables.
            String txtSQL = "";
            String nomCargo = "";
            Double TotalFE = 0;
            Double TotalEmp = 0;
            DataTable rsCargo = new DataTable();
            DataTable contador = new DataTable();
            DataTable rsTipo1 = new DataTable();
            DataTable rsTipo2 = new DataTable();
            DataTable rsPagoActividad = new DataTable();
            double TotalTipo1 = 0;
            double TotalTipo2 = 0;
            int ejecutar = 0;
            double costo_total = 0;
            decimal valor_FE = 0;
            string valor_formateado_FE = "";
            Double Valor = 0;
            String Valor_FE = "&nbsp;";
            String Valor_Emp = "&nbsp;";

            #region Obtener el valor de la prórroga para sumarla a la constante de meses generar la tabla.
            int prorroga = 0;
            prorroga = ObtenerProrroga(CodProyecto.ToString());
            int prorrogaTotal = prorroga + Constantes.CONST_Meses;
            #endregion

            try
            {
                //Inicializar tabla.
                t_anexos.Rows.Clear();

                //Inicializar la fila.
                TableRow fila = new TableRow();
                fila.Style.Add("text-align", "center");

                #region Generar la primera fila con los meses que tiene la nómina seleccionada.
                for (int i = 1; i <= prorrogaTotal; i++)
                {
                    TableHeaderCell celda = new TableHeaderCell();
                    celda.Style.Add("text-align", "center");
                    celda.ColumnSpan = 2;
                    celda.Text = "Mes " + i;
                    fila.Cells.Add(celda);
                    t_anexos.Rows.Add(fila);
                    celda = null;
                }
                #endregion

                #region Crear una nueva celda que contiene el valor "Costo Total".
                TableHeaderCell celdaCostoTotal = new TableHeaderCell();
                celdaCostoTotal.Text = "Costo Total";
                celdaCostoTotal.Style.Add("text-align", "center");
                celdaCostoTotal.ColumnSpan = 2;
                fila.Cells.Add(celdaCostoTotal);
                t_anexos.Rows.Add(fila);
                celdaCostoTotal = null;
                #endregion

                #region Agregar nueva fila (para adicionar las celdas "Fondo" y "Emprendedor").
                //Se obtiene la cantidad de celdas que tiene la primera fila para generar los Sueldos y las Prestaciones.
                int conteo_celdas = fila.Cells.Count + 1; //El +1 es para contar también la celda "Costo Total".
                //Se inicializa la variable para generar una nueva fila.
                fila = new TableRow();

                //Generar las celdas "Sueldo" y "Prestaciones".
                for (int i = 1; i < conteo_celdas; i++)
                {
                    //Celdas "Sueldo" y "Prestaciones Sociales".
                    TableHeaderCell celdaSueldo = new TableHeaderCell();
                    celdaSueldo.Style.Add("text-align", "left");
                    TableHeaderCell celdaPrestaciones = new TableHeaderCell();
                    celdaPrestaciones.Style.Add("text-align", "left");

                    //Agregar datos a la celda de Sueldo.
                    celdaSueldo.Text = "Fondo";
                    celdaSueldo.Attributes.Add("title", "Fondo Emprender");
                    fila.Cells.Add(celdaSueldo);
                    t_anexos.Rows.Add(fila);
                    celdaSueldo = null;

                    //Agregar datos a la celda de Prestaciones Sociales.
                    celdaPrestaciones.Text = "Emprendedor";
                    celdaPrestaciones.Attributes.Add("title", "Aporte Emprendedor");
                    fila.Cells.Add(celdaPrestaciones);
                    t_anexos.Rows.Add(fila);
                    celdaPrestaciones = null;
                }
                #endregion

                #region Personal calificado - Cargos.

                //txtSQL = " SELECT DISTINCT * " +
                //         " FROM InterventorNomina a,InterventorNominaMes b " +
                //         " WHERE a.tipo='Cargo' AND id_nomina = codcargo AND codproyecto= " + Cod_Proyecto +
                //         " AND mes <> 0 and Id_Nomina = " + codActividad + " " +
                //         " ORDER BY id_nomina, mes, b.tipo ";

                txtSQL = " SELECT DISTINCT ISNULL(pm.CodActividad,0) CodActividad,ISNULL(pm.Mes,0) Mes,ISNULL(pm.CodTipoFinanciacion,0) as CodTipoFinanciacion,ISNULL(CONVERT(VARCHAR(2000),CAST(pm.Valor AS DECIMAL),1) ,0.0) Valor,pin.Id_Actividad,pin.NomActividad,pin.CodProyecto,pin.Item,pin.Metas " +
                                                      " FROM ProyectoActividadPOMesInterventor pm RIGHT OUTER JOIN proyectoactividadPOInterventor pin On pin.id_actividad= pm.CodActividad " +
                                                      " Where pin.codproyecto=" + Cod_Proyecto + "  AND pin.Id_actividad = " + codActividad +
                                                      " ORDER BY item, mes,codtipofinanciacion ";

                //Asignar resultados de la consulta anterior a variable DataTable.
                rsCargo = consultas.ObtenerDataTable(txtSQL, "text");

                #endregion

                #region Generar tres filas con sus respectivas celdas.

                //Conteo de las celdas anteriores DEBEN ser obtenidas de nuevo para generar correctamente las celdas.
                conteo_celdas = fila.Cells.Count + 1;
                int mes_data = 1;

                //Generar tres celdas.
                for (int i = 0; i < 4; i++)
                {
                    //Si es cero, es la primera fila, que por defecto es una fila vacía.
                    if (i == 0)
                    {
                        #region Agregar nueva fila con espacio separador "igual como lo deja FONADE clásico.

                        //Inicializar la fila.
                        fila = new TableRow();
                        TableCell celdaEspacio = new TableCell();
                        celdaEspacio.Text = "&nbsp;";
                        fila.Cells.Add(celdaEspacio);
                        t_anexos.Rows.Add(fila);

                        #endregion
                    }
                    if (i == 1)
                    {
                        #region Agregar la fila "con los valores de meses (Cargos)".

                        //Inicializar la fila.
                        fila = new TableRow();
                        //Recorrer las celdas.
                        for (int j = 1; j < conteo_celdas; j++)
                        {
                            //Si el mes es menor o igual a los meses totales (mes+prorroga) + 1 (fila Costo Total).
                            if (mes_data <= prorrogaTotal + 1) //+ 1 indicando la "fila" "Costo Total".
                            {
                                //Inicializar variable para obtener el valor de acuerdo al mes.
                                DataRow[] result = rsCargo.Select("Mes = " + mes_data);

                                //Si encuentra datos.
                                if (result.Count() != 0)
                                {
                                    #region Consultar el campo "Valor" y operarlo en las variables correspondientes.

                                    foreach (DataRow row in result)
                                    {
                                        //Obtener el campos "Valor".
                                        Valor = Double.Parse(row["Valor"].ToString());

                                        //Si es de tipo "1", lo añade a variable "FE", si es "2", lo agrega a variable "Emp".
                                        if (row["CodTipoFinanciacion"].ToString() == "1")
                                        {
                                            TotalFE = TotalFE + Valor;
                                            Valor_FE = Valor.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                        }
                                        if (row["CodTipoFinanciacion"].ToString() == "2")
                                        {
                                            TotalEmp = TotalEmp + Valor;
                                            Valor_Emp = Valor.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                        }
                                    }

                                    #endregion

                                    if (mes_data == prorrogaTotal + 1)
                                    {
                                        TableCell celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = TotalFE.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                        fila.Cells.Add(celdaEspacio);

                                        celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = TotalEmp.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                        fila.Cells.Add(celdaEspacio);
                                    }
                                    else
                                    {
                                        TableCell celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = Valor_FE;
                                        fila.Cells.Add(celdaEspacio);

                                        celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = Valor_Emp; //"&nbsp;";
                                        fila.Cells.Add(celdaEspacio);
                                    }
                                }
                                else
                                {
                                    if (mes_data == prorrogaTotal + 1)
                                    {
                                        TableCell celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = TotalFE.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                        fila.Cells.Add(celdaEspacio);

                                        celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = TotalEmp.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                        fila.Cells.Add(celdaEspacio);
                                    }
                                    else
                                    {
                                        #region Añadir espacios.

                                        TableCell celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = "&nbsp;";
                                        fila.Cells.Add(celdaEspacio);

                                        celdaEspacio = new TableCell();
                                        celdaEspacio.Attributes.Add("align", "right");
                                        celdaEspacio.Text = "&nbsp;";
                                        fila.Cells.Add(celdaEspacio);

                                        #endregion
                                    }
                                }
                                //Incrementar variable mes.
                                mes_data++;
                            }
                        }
                        //Añadir la fila a la tabla.
                        t_anexos.Rows.Add(fila);

                        #endregion
                    }
                    if (i == 2)
                    {
                        #region Agregar la fila "con valores en (rojo)".
                        mes_data = 1;
                        fila = new TableRow();
                        for (int j = 1; j < conteo_celdas; j++)
                        {
                            //Si el mes es menor o igual a los meses totales (mes+prorroga) + 1 (fila Costo Total).
                            if (mes_data <= prorrogaTotal + 1) //+ 1 indicando la "fila" "Costo Total".
                            {
                                #region Tipo 1.
                                string tipo1 = "&nbsp;";
                                double valor1 = 0;

                                txtSQL = "select *  from AvanceActividadPOMes where codactividad=" + codActividad +
                                    " and Mes=" + mes_data + " and codtipofinanciacion=1";
                                var datable1 = consultas.ObtenerDataTable(txtSQL, "text");

                                if (datable1.Rows.Count > 0)
                                {
                                    valor1 = double.Parse(datable1.Rows[0]["valor"].ToString());
                                    TotalTipo1 += valor1;
                                    tipo1 = valor1.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                }
                                if (mes_data == prorrogaTotal + 1)
                                {
                                    TableCell celdaEspacio = new TableCell();
                                    celdaEspacio.Attributes.Add("align", "right");
                                    celdaEspacio.Text = "<font color='#CC0000'>" + TotalTipo1.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</font>";
                                    fila.Cells.Add(celdaEspacio);
                                }
                                else
                                {
                                    TableCell celdaEspacio = new TableCell();
                                    celdaEspacio.Attributes.Add("align", "right");
                                    celdaEspacio.Text = "<font color='#CC0000'>" + tipo1 + "</font>";
                                    fila.Cells.Add(celdaEspacio);
                                }

                                #endregion

                                #region Tipo 2.
                                string tipo2 = "&nbsp;";
                                double valor2 = 0;

                                txtSQL = "select *  from AvanceActividadPOMes where codactividad=" + codActividad +
                                    " and Mes=" + mes_data + " and codtipofinanciacion=2";
                                var datable2 = consultas.ObtenerDataTable(txtSQL, "text");

                                if (datable2.Rows.Count > 0)
                                {
                                    valor2 = double.Parse(datable2.Rows[0]["valor"].ToString());
                                    TotalTipo2 += valor2;
                                    tipo2 = valor2.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                                }
                                if (mes_data == prorrogaTotal + 1)
                                {
                                    TableCell celdaEspacio = new TableCell();
                                    celdaEspacio.Attributes.Add("align", "right");
                                    celdaEspacio.Text = "<font color='#CC0000'>" + TotalTipo2.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</font>";
                                    fila.Cells.Add(celdaEspacio);
                                }

                                else
                                {
                                    TableCell celdaEspacio = new TableCell();
                                    celdaEspacio.Attributes.Add("align", "right");
                                    celdaEspacio.Text = "<font color='#CC0000'>" + tipo2 + "</font>";
                                    fila.Cells.Add(celdaEspacio);
                                }

                                #endregion
                            }
                            mes_data++;
                        }
                        t_anexos.Rows.Add(fila);
                        #endregion
                    }
                    if (i == 3)
                    {
                        #region Agregar la fila "con los controles dinámicos".

                        //Inicializar celda.
                        fila = new TableRow();
                        //Re-inicializar la variable de "Mes" a 1.
                        mes_data = 1;
                        //for (int m = 1; m <= prorrogaTotal + 1; m++)
                        for (int m = 1; m <= prorrogaTotal + 1; m++)
                        {
                            //Generar celda.
                            TableCell celda = new TableCell();
                            celda.Style.Add("text-align", "center");
                            celda.ColumnSpan = 2;

                            //Si el mes es menor o igual a los meses totales (mes+prorroga) + 1 (fila Costo Total).
                            if (mes_data <= prorrogaTotal + 1) //+ 1 indicando la "fila" "Costo Total".
                            {
                                //Consulta SQL.
                                txtSQL = " SELECT * FROM AvanceActividadPOMes " +
                                         " WHERE codactividad = " + codActividad + " AND mes = " + mes_data;

                                //Asignar resultados a la variable DataTable.
                                rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");

                                //Determinar el valor de la vairiable "ejecutar" de acuerdo a la tabla.
                                if (rsTipo2.Rows.Count > 0)
                                { ejecutar = 1; /*Si existe se coloca la opción de editar y borrar*/ }
                                else
                                { ejecutar = 2; /*Si NO existe se coloca la opción de adicionar*/ }

                                if (ejecutar == 1)
                                {
                                    if (usuario.CodGrupo == Constantes.CONST_Emprendedor)
                                    {
                                        rsPagoActividad = new DataTable();

                                        //Se debe verificar que si la actividad ya tiene un PagoActividad, el emprendedor solo puede editar si el pago esta en 0
                                        txtSQL = " SELECT DISTINCT ProyectoActividadPOInterventor.Id_Actividad, PagoActividad.Estado FROM PagoActividad " +
                                                 " INNER JOIN ProyectoActividadPOInterventor ON PagoActividad.CodActividad = ProyectoActividadPOInterventor.Id_Actividad " +
                                                 " WHERE (ProyectoActividadPOInterventor.CodProyecto = " + CodProyecto + ") " +
                                                 " AND (PagoActividad.CodProyecto = " + CodProyecto + ") " +
                                                 " AND (ProyectoActividadPOInterventor.Id_Actividad = " + codActividad + ") " +
                                                 " AND (PagoActividad.Mes = " + mes_data + ") AND (PagoActividad.Estado <> 0)";

                                        rsPagoActividad = consultas.ObtenerDataTable(txtSQL, "text");


                                        //if (rsTipo2.Rows[0]["Aprobada"].ToString() == "False" || rsTipo2.Rows[0]["Aprobada"].ToString() == "0")
                                        if (rsTipo2.Rows[0]["Aprobada"].ToString() == "True" || rsTipo2.Rows[0]["Aprobada"].ToString() == "1")
                                        {
                                            #region Ver avance.

                                            if (mes_data <= prorrogaTotal)
                                            {
                                                ImageButton img_VerAvance = new ImageButton();
                                                LinkButton lnk_VerAvance = new LinkButton();

                                                //ImageButton.
                                                img_VerAvance.ID = "img_VerAvance_" + mes_data.ToString();
                                                img_VerAvance.ImageUrl = "~/Images/icoAdicionarUsuario.gif";
                                                img_VerAvance.AlternateText = "Avance";
                                                img_VerAvance.CommandName = "actualizar";
                                                img_VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                                img_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                                //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                                //{ img_VerAvance = null; }
                                                //else
                                                //{  }
                                                celda.Controls.Add(img_VerAvance);

                                                //LinkButton.
                                                lnk_VerAvance.ID = "lnk_VerAvance_" + mes_data.ToString();
                                                lnk_VerAvance.Text = "<b>&nbsp;Ver Avance</b>";
                                                lnk_VerAvance.Style.Add("text-decoration", "none");
                                                lnk_VerAvance.CommandName = "actualizar";
                                                lnk_VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                                lnk_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                                //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                                //{ lnk_VerAvance = null; }
                                                //else
                                                //{  }
                                                celda.Controls.Add(lnk_VerAvance);
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            if (rsPagoActividad.Rows.Count == 0)
                                            {
                                                if (String.IsNullOrEmpty(rsTipo2.Rows[0]["ObservacionesInterventor"].ToString()))
                                                {
                                                    #region Eliminar avance.

                                                    if (mes_data <= prorrogaTotal)
                                                    {
                                                        ImageButton img_EliminarAvance = new ImageButton();

                                                        //ImageButton.
                                                        img_EliminarAvance.ID = "img_EliminarAvance_" + mes_data.ToString();
                                                        img_EliminarAvance.ImageUrl = "~/Images/icoBorrar.gif";
                                                        img_EliminarAvance.AlternateText = "Avance";
                                                        img_EliminarAvance.CommandName = "borrar";
                                                        img_EliminarAvance.ToolTip = "Eliminar Avance";
                                                        img_EliminarAvance.OnClientClick = "return borrar();";
                                                        img_EliminarAvance.Style.Add(System.Web.UI.HtmlTextWriterStyle.MarginRight, "5px");
                                                        img_EliminarAvance.CommandArgument = "borrar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                                        img_EliminarAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                                        //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                                        //{ img_EliminarAvance = null; }
                                                        //else
                                                        //{  }
                                                        celda.Controls.Add(img_EliminarAvance);
                                                    }

                                                    #endregion
                                                }

                                                #region Editar avance.

                                                if (mes_data <= prorrogaTotal)
                                                {
                                                    ImageButton img_VerAvance = new ImageButton();
                                                    LinkButton lnk_VerAvance = new LinkButton();

                                                    //ImageButton.
                                                    img_VerAvance.ID = "img_VerAvance_" + mes_data.ToString();
                                                    img_VerAvance.ImageUrl = "~/Images/icoAdicionarUsuario.gif";
                                                    img_VerAvance.AlternateText = "Avance";
                                                    img_VerAvance.CommandName = "actualizar";
                                                    img_VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                                    img_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                                    //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                                    //{ img_VerAvance = null; }
                                                    //else
                                                    //{ }
                                                    celda.Controls.Add(img_VerAvance);

                                                    //LinkButton.
                                                    lnk_VerAvance.ID = "lnk_VerAvance_" + mes_data.ToString();
                                                    lnk_VerAvance.Text = "<b>&nbsp;Editar Avance</b>";
                                                    lnk_VerAvance.Style.Add("text-decoration", "none");
                                                    lnk_VerAvance.CommandName = "actualizar";
                                                    lnk_VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                                    lnk_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                                    //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                                    //{ lnk_VerAvance = null; }
                                                    //else
                                                    //{  }
                                                    celda.Controls.Add(lnk_VerAvance);
                                                }

                                                #endregion
                                            }
                                            else
                                            {
                                                #region Ver avance.

                                                if (mes_data <= prorrogaTotal)
                                                {
                                                    ImageButton img_____VerAvance = new ImageButton();
                                                    LinkButton lnk_____VerAvance = new LinkButton();

                                                    //ImageButton.
                                                    img_____VerAvance.ID = "img_____VerAvance_____" + mes_data.ToString();
                                                    img_____VerAvance.ImageUrl = "~/Images/icoAdicionarUsuario.gif";
                                                    img_____VerAvance.AlternateText = "Avance";
                                                    img_____VerAvance.CommandName = "actualizar";
                                                    img_____VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                                    img_____VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                                    //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                                    //{ img_____VerAvance = null; }
                                                    //else
                                                    //{  }
                                                    celda.Controls.Add(img_____VerAvance);

                                                    //LinkButton.
                                                    lnk_____VerAvance.ID = "lnk_____VerAvance_____" + mes_data.ToString();
                                                    lnk_____VerAvance.Text = "<b>&nbsp;Ver Avance</b>";
                                                    lnk_____VerAvance.Style.Add("text-decoration", "none");
                                                    lnk_____VerAvance.CommandName = "actualizar";
                                                    lnk_____VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                                    lnk_____VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                                    //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                                    //{ lnk_____VerAvance = null; }
                                                    //else
                                                    //{  }
                                                    celda.Controls.Add(lnk_____VerAvance);
                                                }

                                                #endregion
                                            }
                                        }
                                    }
                                    else if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_Asesor || usuario.CodGrupo == Constantes.CONST_JefeUnidad || usuario.CodGrupo == Constantes.CONST_CallCenter)
                                    {
                                        #region Ver avance.

                                        if (mes_data <= prorrogaTotal)
                                        {
                                            ImageButton img__VerAvance = new ImageButton();
                                            LinkButton lnk__VerAvance = new LinkButton();

                                            //ImageButton.
                                            img__VerAvance.ID = "img__VerAvance__" + mes_data.ToString();
                                            img__VerAvance.ImageUrl = "~/Images/icoAdicionarUsuario.gif";
                                            img__VerAvance.AlternateText = "Avance";
                                            img__VerAvance.CommandName = "actualizar";
                                            img__VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                            img__VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                            //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                            //{ img__VerAvance = null; }
                                            //else
                                            //{  }
                                            celda.Controls.Add(img__VerAvance);

                                            //LinkButton.
                                            lnk__VerAvance.ID = "lnk__VerAvance__" + mes_data.ToString();
                                            lnk__VerAvance.Text = "<b>&nbsp;Ver Avance</b>";
                                            lnk__VerAvance.Style.Add("text-decoration", "none");
                                            lnk__VerAvance.CommandName = "actualizar";
                                            lnk__VerAvance.CommandArgument = "actualizar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                            lnk__VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                            //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                            //{ lnk__VerAvance = null; }
                                            //else
                                            //{  }
                                            celda.Controls.Add(lnk__VerAvance);
                                        }

                                        #endregion
                                    }
                                }
                                else
                                {
                                    if (usuario.CodGrupo == Constantes.CONST_Emprendedor)
                                    {
                                        #region Reportar avance.

                                        if (mes_data <= prorrogaTotal)
                                        {
                                            ImageButton img__ra__ReportarAvance = new ImageButton();
                                            LinkButton lnk__ra__ReportarAvance = new LinkButton();

                                            //ImageButton.
                                            img__ra__ReportarAvance.ID = "img__ra__ReportarAvance__ra__" + mes_data.ToString();
                                            img__ra__ReportarAvance.ImageUrl = "~/Images/icoAdicionarUsuario.gif";
                                            img__ra__ReportarAvance.AlternateText = "Reportar";
                                            img__ra__ReportarAvance.CommandName = "Reportar";
                                            img__ra__ReportarAvance.CommandArgument = "Reportar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                            img__ra__ReportarAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                            //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                            //{ img__ra__ReportarAvance = null; }
                                            //else
                                            //{  }
                                            celda.Controls.Add(img__ra__ReportarAvance);

                                            //LinkButton.
                                            lnk__ra__ReportarAvance.ID = "lnk__ra__ReportarAvance__ra__" + mes_data.ToString();
                                            lnk__ra__ReportarAvance.Text = "<b>&nbsp;Reportar Avance</b>";
                                            lnk__ra__ReportarAvance.Style.Add("text-decoration", "none");
                                            lnk__ra__ReportarAvance.CommandName = "Reportar";
                                            lnk__ra__ReportarAvance.CommandArgument = "Reportar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                                            lnk__ra__ReportarAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                                            //if (usuario.CodGrupo != Constantes.CONST_Interventor)
                                            //{ lnk__ra__ReportarAvance = null; }
                                            //else
                                            //{  }
                                            celda.Controls.Add(lnk__ra__ReportarAvance);
                                        }

                                        #endregion
                                    }
                                }

                            }

                            //Incrementa el mes 
                            mes_data++;

                            //Añadir la celda a la fila y la fila a la tabla.
                            fila.Cells.Add(celda);
                            t_anexos.Rows.Add(fila);
                            celda = null;

                            #region NO BORRAR!
                            ////TableCell celda = new TableCell();
                            ////celda.Style.Add("text-align", "center");
                            ////celda.ColumnSpan = 2;

                            //if (mes_data <= prorrogaTotal)
                            //{
                            //    #region Consultar AvanceCargoPOMes

                            //    txtSQL = " SELECT * " +
                            //             " FROM AvanceActividadPOMes " +
                            //             " WHERE codactividad=" + codActividad +
                            //             " AND mes=" + mes_data;

                            //    rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");
                            //    #endregion

                            //    #region Establecer si se puede o no ejecutar.
                            //    if (rsTipo2.Rows.Count > 0)
                            //    {
                            //        ejecutar = 1; //Si existe se coloca la opción de editar y borrar.
                            //    }
                            //    else
                            //    {
                            //        ejecutar = 2; //Si NO existe se coloca la opción de adicionar.
                            //    }
                            //    #endregion

                            //    //Agregar celda.
                            //    TableCell celdaCentrada = new TableCell();
                            //    celdaCentrada.Attributes.Add("colspan", "2");
                            //    celdaCentrada.Attributes.Add("align", "center");

                            //    try { nomCargo = nomCargo.Replace("+", "$"); }
                            //    catch { }

                            //    if (ejecutar == 1)
                            //    {
                            //        #region Reportar avance.//Ver avance.
                            //        if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_Asesor || usuario.CodGrupo == Constantes.CONST_JefeUnidad || usuario.CodGrupo == Constantes.CONST_CallCenter || usuario.CodGrupo == Constantes.CONST_Emprendedor)
                            //        {
                            //            ImageButton img_VerAvance = new ImageButton();
                            //            LinkButton lnk_VerAvance = new LinkButton();

                            //            //ImageButton.
                            //            img_VerAvance.ID = "img_VerAvance_" + mes_data.ToString();
                            //            img_VerAvance.ImageUrl = "~/Images/icoAdicionarUsuario.gif";
                            //            img_VerAvance.AlternateText = "Avance";
                            //            img_VerAvance.CommandName = "VerAvance";
                            //            img_VerAvance.CommandArgument = "Editar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                            //            img_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                            //            if (usuario.CodGrupo == Constantes.CONST_Interventor)
                            //            { img_VerAvance = null; }
                            //            else
                            //            { celdaCentrada.Controls.Add(img_VerAvance); }

                            //            //LinkButton.
                            //            lnk_VerAvance.ID = "lnk_VerAvance_" + mes_data.ToString();
                            //            lnk_VerAvance.Text = "<b>&nbsp;Ver Avance</b>";
                            //            lnk_VerAvance.Style.Add("text-decoration", "none");
                            //            lnk_VerAvance.CommandName = "VerAvance";
                            //            lnk_VerAvance.CommandArgument = "Editar" + ";" + Cod_Proyecto + ";" + codActividad + ";" + mes_data + ";" + nomCargo;
                            //            lnk_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                            //            if (usuario.CodGrupo == Constantes.CONST_Interventor)
                            //            { lnk_VerAvance = null; }
                            //            else
                            //            { celdaCentrada.Controls.Add(lnk_VerAvance); }
                            //        }
                            //        #endregion
                            //    }
                            //    if (celdaCentrada.Controls.Count == 0) { celdaCentrada.Text = "&nbsp;"; }
                            //    fila.Cells.Add(celdaCentrada);
                            //    t_anexos.Rows.Add(fila);
                            //    celdaCentrada = null;
                            //}

                            ////Incrementa el mes 
                            //mes_data++; 
                            #endregion
                        }
                        #endregion
                    }
                    t_anexos.Rows.Add(fila);
                }

                #endregion

                //Bindear finalmente la grilla.
                t_anexos.DataBind();
            }
            catch (Exception ex)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                //t_anexos.Visible = false;
            }
        }

        public decimal Avance(int codactividad, int mes, int tipo)
        {
            decimal dtAvance = 0;

            try
            {
                var query = consultas.Db.AvanceActividadPOMes
                    .FirstOrDefault(am => am.CodActividad == codactividad && am.Mes == mes && am.CodTipoFinanciacion == tipo);

                if (query != null && query.CodActividad != 0)
                {
                    dtAvance = query.Valor;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dtAvance;
        }

        void Cargar()
        {
            Session["Accion"] = "crear";
            Session["codProyecto"] = CodProyecto;
            Session["FramePO"] = "FramePO";
            Redirect(null, "../evaluacion/CatalogoActividadPO.aspx", "_blank", "menubar=0,scrollbars=1,width=1009,height=511,top=50");

        }

        protected void Adicionar_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Cargar();
        }

        #region eventos Gridview

        protected void GwAnexosRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session["Accion"] = "actualizar";
                Session["CodActividad"] = e.CommandArgument.ToString();

                Redirect(null, "../evaluacion/CatalogoActividadPO.aspx", "_blank",
                         "menubar=0,scrollbars=1,width=960,height=525,top=100");
            }
            if (e.CommandName == "eliminar")
            {
                EliminarActividadSeleccionada(Convert.ToInt32(e.CommandArgument.ToString()));
            }
            else if (e.CommandName == "mostrar")
            {

                Session["CodActividad"] = e.CommandArgument.ToString();
                CargarDetalle(e.CommandArgument.ToString());
            }
        }

        protected void GwAnexosRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var labelActividadPo = e.Row.FindControl("lblactividaPOI") as Label;
                var imgEditar = e.Row.FindControl("lnkeliminar") as LinkButton;
                var lnk_ver = e.Row.FindControl("lnkeditar") as LinkButton;

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Procesar para el caso de que el usuario sea un "Interventor".
                    if (labelActividadPo != null)
                    {
                        if (labelActividadPo.Text.Equals("0"))
                        {
                            if (imgEditar != null)
                            {
                                imgEditar.Visible = true;
                                imgEditar.ToolTip = "Eliminar Actividad.";
                            }
                        }
                        else
                        {
                            if (imgEditar != null)
                            {
                                imgEditar.Visible = false;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Inhaibilitar LinkButton.

                    lnk_ver.Enabled = false;
                    lnk_ver.Style.Add("text-decoration", "none");
                    lnk_ver.ForeColor = System.Drawing.Color.Black;

                    #endregion

                    #region Procesar para cualquier otro "Rol". = Se debe dejar invisible el botón de eliminación.

                    if (labelActividadPo != null)
                    {
                        if (imgEditar != null)
                        {
                            imgEditar.Visible = false;
                        }
                    }

                    #endregion

                    #region Comentarios NO BORRAR!.

                    //if (labelActividadPo != null)
                    //{
                    //    if (labelActividadPo.Text.Equals("0"))
                    //    {
                    //        if (imgEditar != null)
                    //        {
                    //            imgEditar.Visible = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (imgEditar != null)
                    //        {
                    //            imgEditar.Visible = false;
                    //        }
                    //    }
                    //}

                    #endregion
                }
            }
        }

        protected void GwAnexosPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gw_Anexos.PageIndex = e.NewPageIndex;
            gw_Anexos.DataSource = Session["dtitems"];
            gw_Anexos.DataBind();
        }

        protected void GrvActividadesNoAprovadasRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session["Accion"] = "actualizar";
                Session["CodActividad"] = e.CommandArgument.ToString();
                Redirect(null, "../evaluacion/CatalogoActividadPOTMP.aspx", "_blank",
                         "menubar=0,scrollbars=1,width=980,height=400,top=100");
            }
        }

        protected void GrvActividadesNoAprovadas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvActividadesNoAprovadas.PageIndex = e.NewPageIndex;
            GrvActividadesNoAprovadas.DataSource = Session["dtActividades"];
            GrvActividadesNoAprovadas.DataBind();
        }

        protected void gw_AnexosActividad_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 1)
                {
                    //Colocar los valores de avances de color rojo.
                    e.Row.ForeColor = System.Drawing.Color.Red;

                    #region Label
                    var fondo1F = e.Row.FindControl("fondo1F") as Label;
                    var fondo2F = e.Row.FindControl("fondo2F") as Label;
                    var fondo3F = e.Row.FindControl("fondo3F") as Label;
                    var fondo4F = e.Row.FindControl("fondo4F") as Label;
                    var fondo5F = e.Row.FindControl("fondo5F") as Label;
                    var fondo6F = e.Row.FindControl("fondo6F") as Label;
                    var fondo7F = e.Row.FindControl("fondo7F") as Label;
                    var fondo8F = e.Row.FindControl("fondo8F") as Label;
                    var fondo9F = e.Row.FindControl("fondo9F") as Label;
                    var fondo10F = e.Row.FindControl("fondo10F") as Label;
                    var fondo11F = e.Row.FindControl("fondo11F") as Label;
                    var fondo12F = e.Row.FindControl("fondo12F") as Label;
                    var fondo13F = e.Row.FindControl("fondo13F") as Label;
                    var fondo14F = e.Row.FindControl("fondo14F") as Label;
                    #endregion

                    #region Validacion Mostrar Avances

                    if (fondo1F != null)
                    {

                        Sfondo1F = fondo1F.Text;

                    }
                    if (fondo2F != null)
                    {
                        Sfondo2F = fondo2F.Text;

                    }
                    if (fondo3F != null)
                    {
                        Sfondo3F = fondo3F.Text;


                    }
                    if (fondo4F != null)
                    {
                        Sfondo4F = fondo4F.Text;

                    }
                    if (fondo5F != null)
                    {
                        Sfondo5F = fondo5F.Text;


                    }
                    if (fondo6F != null)
                    {
                        Sfondo6F = fondo6F.Text;


                    }
                    if (fondo7F != null)
                    {

                        Sfondo7F = fondo7F.Text;


                    }
                    if (fondo8F != null)
                    {

                        Sfondo8F = fondo8F.Text;


                    }
                    if (fondo9F != null)
                    {

                        Sfondo9F = fondo9F.Text;


                    }

                    if (fondo10F != null)
                    {
                        Sfondo10F = fondo10F.Text;


                    }

                    if (fondo11F != null)
                    {

                        Sfondo11F = fondo11F.Text;


                    }

                    if (fondo12F != null)
                    {

                        Sfondo12F = fondo12F.Text;


                    }

                    if (fondo13F != null)
                    {

                        Sfondo13F = fondo13F.Text;


                    }

                    if (fondo14F != null)
                    {

                        Sfondo14F = fondo14F.Text;


                    }

                    #endregion
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                if (e.Row.RowIndex == -1)
                {
                    #region imagenes

                    var imgAvance1F = e.Row.FindControl("imgAvance1") as Image;
                    var imgAvance2F = e.Row.FindControl("imgAvance2") as Image;
                    var imgAvance3F = e.Row.FindControl("imgAvance3") as Image;
                    var imgAvance4F = e.Row.FindControl("imgAvance4") as Image;
                    var imgAvance5F = e.Row.FindControl("imgAvance5") as Image;
                    var imgAvance6F = e.Row.FindControl("imgAvance6") as Image;
                    var imgAvance7F = e.Row.FindControl("imgAvance7") as Image;
                    var imgAvance8F = e.Row.FindControl("imgAvance8") as Image;
                    var imgAvance9F = e.Row.FindControl("imgAvance9") as Image;
                    var imgAvance10F = e.Row.FindControl("imgAvance10") as Image;
                    var imgAvance11F = e.Row.FindControl("imgAvance11") as Image;
                    var imgAvance12F = e.Row.FindControl("imgAvance12") as Image;
                    var imgAvance13F = e.Row.FindControl("imgAvance13") as Image;
                    var imgAvance14F = e.Row.FindControl("imgAvance14") as Image;

                    #endregion

                    #region link

                    var lnkactividad1F = e.Row.FindControl("lnkactividad1") as LinkButton;
                    lnkactividad1F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad2F = e.Row.FindControl("lnkactividad2") as LinkButton;
                    lnkactividad2F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad3F = e.Row.FindControl("lnkactividad3") as LinkButton;
                    lnkactividad3F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad4F = e.Row.FindControl("lnkactividad4") as LinkButton;
                    lnkactividad4F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad5F = e.Row.FindControl("lnkactividad5") as LinkButton;
                    lnkactividad5F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad6F = e.Row.FindControl("lnkactividad6") as LinkButton;
                    lnkactividad6F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad7F = e.Row.FindControl("lnkactividad7") as LinkButton;
                    lnkactividad7F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad8F = e.Row.FindControl("lnkactividad8") as LinkButton;
                    lnkactividad8F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad9F = e.Row.FindControl("lnkactividad9") as LinkButton;
                    lnkactividad9F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad10F = e.Row.FindControl("lnkactividad10") as LinkButton;
                    lnkactividad10F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad11F = e.Row.FindControl("lnkactividad11") as LinkButton;
                    lnkactividad11F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad12F = e.Row.FindControl("lnkactividad12") as LinkButton;
                    lnkactividad12F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad13F = e.Row.FindControl("lnkactividad13") as LinkButton;
                    lnkactividad13F.Click += new EventHandler(LinkButtonClick);
                    var lnkactividad14F = e.Row.FindControl("lnkactividad14") as LinkButton;
                    lnkactividad14F.Click += new EventHandler(LinkButtonClick);

                    #endregion

                    #region Si el valor empieza con "0" o está vacío, se muestran los avances.
                    if (string.IsNullOrEmpty(Sfondo1F) || Sfondo1F.StartsWith("$0"))
                    {
                        imgAvance1F.Visible = false;
                        lnkactividad1F.Visible = false;
                    }



                    if (string.IsNullOrEmpty(Sfondo2F) || Sfondo2F.StartsWith("$0"))
                    {
                        imgAvance2F.Visible = false;
                        lnkactividad2F.Visible = false;
                    }



                    if (string.IsNullOrEmpty(Sfondo3F) || Sfondo3F.StartsWith("$0"))
                    {
                        imgAvance3F.Visible = false;
                        lnkactividad3F.Visible = false;
                    }


                    if (string.IsNullOrEmpty(Sfondo4F) || Sfondo4F.StartsWith("$0"))
                    {
                        imgAvance4F.Visible = false;
                        lnkactividad4F.Visible = false;
                    }

                    if (string.IsNullOrEmpty(Sfondo5F) || Sfondo5F.StartsWith("$0"))
                    {
                        imgAvance5F.Visible = false;
                        lnkactividad5F.Visible = false;
                    }


                    if (string.IsNullOrEmpty(Sfondo6F) || Sfondo6F.StartsWith("$0"))
                    {
                        imgAvance6F.Visible = false;
                        lnkactividad6F.Visible = false;
                    }

                    if (string.IsNullOrEmpty(Sfondo7F) || Sfondo7F.StartsWith("$0"))
                    {
                        imgAvance7F.Visible = false;
                        lnkactividad7F.Visible = false;
                    }
                    if (string.IsNullOrEmpty(Sfondo8F) || Sfondo8F.StartsWith("$0"))
                    {
                        imgAvance8F.Visible = false;
                        lnkactividad8F.Visible = false;
                    }

                    if (string.IsNullOrEmpty(Sfondo9F) || Sfondo9F.StartsWith("$0"))
                    {
                        imgAvance9F.Visible = false;
                        lnkactividad9F.Visible = false;
                    }


                    if (string.IsNullOrEmpty(Sfondo10F) || Sfondo10F.StartsWith("$0"))
                    {
                        imgAvance10F.Visible = false;
                        lnkactividad10F.Visible = false;
                    }

                    if (string.IsNullOrEmpty(Sfondo11F) || Sfondo11F.StartsWith("$0"))
                    {
                        imgAvance11F.Visible = false;
                        lnkactividad11F.Visible = false;
                    }

                    if (string.IsNullOrEmpty(Sfondo12F) || Sfondo12F.StartsWith("$0"))
                    {
                        imgAvance12F.Visible = false;
                        lnkactividad12F.Visible = false;
                    }

                    if (string.IsNullOrEmpty(Sfondo13F) || Sfondo13F.StartsWith("$0"))
                    {
                        imgAvance13F.Visible = false;
                        lnkactividad13F.Visible = false;
                    }

                    if (string.IsNullOrEmpty(Sfondo14F) || Sfondo14F.StartsWith("$0"))
                    {
                        imgAvance14F.Visible = false;
                        lnkactividad14F.Visible = false;
                    }
                    #endregion

                    //Nuevo = Si NO es "CONST_Interventor", no podrá ver avances.
                    if (usuario.CodGrupo != Constantes.CONST_Interventor)
                    {
                        imgAvance1F.Visible = false;
                        lnkactividad1F.Visible = false;
                        imgAvance2F.Visible = false;
                        lnkactividad2F.Visible = false;
                        imgAvance3F.Visible = false;
                        lnkactividad3F.Visible = false;
                        imgAvance4F.Visible = false;
                        lnkactividad4F.Visible = false;
                        imgAvance5F.Visible = false;
                        lnkactividad5F.Visible = false;
                        imgAvance6F.Visible = false;
                        lnkactividad6F.Visible = false;
                        imgAvance7F.Visible = false;
                        lnkactividad7F.Visible = false;
                        imgAvance8F.Visible = false;
                        lnkactividad8F.Visible = false;
                        imgAvance9F.Visible = false;
                        lnkactividad9F.Visible = false;
                        imgAvance10F.Visible = false;
                        lnkactividad10F.Visible = false;
                        imgAvance11F.Visible = false;
                        lnkactividad11F.Visible = false;
                        imgAvance12F.Visible = false;
                        lnkactividad12F.Visible = false;
                        imgAvance13F.Visible = false;
                        lnkactividad13F.Visible = false;
                        imgAvance14F.Visible = false;
                        lnkactividad14F.Visible = false;
                    }
                }
            }
        }

        #endregion

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Cargar();
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/04/2014: Eliminar la actividad seleccionada ya hacer las funciones que dicta FONADE clásico.
        /// </summary>
        /// <param name="P_CodActividad">Código de la actividad seleccionada.</param>
        private void EliminarActividadSeleccionada(Int32 P_CodActividad)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta;
            bool procesado = false;
            String txtSQL = "";

            try
            {
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Si es interventor inserta los registros a borrar en tablas temporales para la aprobación del coordinador y el gerente

                    //Asigna la tarea al coordinador
                    txtSQL = "select CodCoordinador  from interventor where codcontacto = " + usuario.IdContacto;
                    var Rs = consultas.ObtenerDataTable(txtSQL, "text");

                    //Verifica si el interventor tiene un coordinador asignado			            
                    if (Rs.Rows.Count > 0)
                    {
                        #region Inserción.

                        txtSQL = " Insert into proyectoactividadPOInterventorTMP (id_actividad,CodProyecto,Tarea) " +
                                 " values (" + P_CodActividad + "," + CodProyecto + ",'Borrar')";

                        try
                        {
                            //NEW RESULTS:
                            cmd = new SqlCommand(txtSQL, con);

                            if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Dispose();
                            cmd.Dispose();
                            procesado = true;
                        }
                        catch { procesado = false; }

                        #endregion

                        #region Inserción #2.

                        txtSQL = "Insert into proyectoactividadPOMesInterventorTMP (CodActividad) " +
                                 "values (" + P_CodActividad + ")";

                        #region COMENTARIOS NO BORRAR.
                        //try
                        //{
                        //    //NEW RESULTS:
                        //    con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                        //    cmd = new SqlCommand(txtSQL, con);

                        //    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        //    cmd.CommandType = CommandType.Text;
                        //    cmd.ExecuteNonQuery();
                        //    con.Close();
                        //    con.Dispose();
                        //    cmd.Dispose();
                        //    procesado = true;
                        //}
                        //catch { procesado = false; } //Aquí sale el error de "no PK on table ProyectoActividadPOMesInterventorTMP". 
                        #endregion

                        //Ejecutar setencia.
                        ejecutaReader(txtSQL, 2);

                        #endregion

                        if (procesado)
                        {
                            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Actividad eliminada.')", true);
                            CargarGridActividades();
                            return;
                        }
                    }
                    else
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
                        return;
                    }
                    //Destruir la variable temporal.
                    Rs = null;

                    #endregion
                }
                else
                {
                    #region Ejecuta sentencia DELETE.

                    #region Versión 1.0 COMENTADA.
                    ////Ejecutar Delete
                    //sqlConsulta = "DELETE FROM ProyectoactividadPO WHERE Id_Actividad = " + P_CodActividad;

                    //cmd = new SqlCommand(sqlConsulta, con);
                    //procesado = EjecutarSQL(con, cmd);

                    //if (procesado)
                    //{
                    //    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Actividad eliminada.')", true);
                    //    CargarGridActividades();
                    //    return;
                    //}
                    //else
                    //{
                    //    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo eliminar la actividad seleccionada.')", true);
                    //    return;
                    //} 
                    #endregion

                    #region Versión 2.0

                    sqlConsulta = "DELETE FROM ProyectoactividadPO WHERE Id_Actividad = " + P_CodActividad;

                    //Ejecutar setencia.
                    ejecutaReader(txtSQL, 2);

                    //Cargar la actividades.
                    CargarGridActividades();
                    return;

                    #endregion

                    #endregion
                }
            }
            catch (Exception)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo eliminar la actividad seleccionada.')", true);
                return;
            }
        }

        public event EventHandler Click;

        protected void gw_AnexosActividad_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            switch (e.CommandName.ToString())
            {
                case "editar":
                    Session["CodActividad"] = codActividad;
                    string linkid = "";

                    using (GridViewRow rowSelect = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer))
                    {
                        LinkButton ln = (LinkButton)rowSelect.FindControl((((LinkButton)e.CommandSource).ID).ToString());

                        linkid = ln.ID;
                        int limit = 0;
                        if (linkid.Length == 14) limit = 2;
                        else limit = 1;
                        linkid = linkid.Substring(12, limit);
                        Session["linkid"] = linkid;

                    }

                    Redirect(null, "CatalogoActividadPOInterventor.aspx", "_blank", "menubar=0,scrollbars=1,width=980,height=400,top=100");
                    /*diana*/
                    break;
            }
        }

        public void LinkButtonClick(object sender, EventArgs e)
        {
            switch ((sender as LinkButton).ID)
            {
                case "lnkactividad1":

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Botón "Pagos".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkPagos_Click(object sender, EventArgs e)
        {
            if (Estado == 1)
            {
                Session["TipoPago"] = "1";
                Session["CodProyecto"] = CodProyecto;

                if (usuario.CodGrupo == Constantes.CONST_Emprendedor)
                { Redirect(null, "PagosActividad.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100"); }

                else if (usuario.CodGrupo == Constantes.CONST_Interventor)
                { Redirect(null, "PagosActividadInter.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100"); }
            }
            else if (Estado == 2)
            {
                Session["TipoPago"] = "2";
                Session["CodProyecto"] = CodProyecto;

                if (usuario.CodGrupo == Constantes.CONST_Emprendedor)
                { Redirect(null, "PagosActividad.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100"); }

                else if (usuario.CodGrupo == Constantes.CONST_Interventor)
                { Redirect(null, "PagosActividadInter.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100"); }
            }
        }

        protected void lnkImprimir_Click(object sender, EventArgs e)
        {
            Redirect(null, "ImprimirPlanOperativos.aspx", "_blank", "width=640,height=480,scrollbars=yes,resizable=no");
        }

        #region Métodos varios.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 05/05/2014.
        /// Evaluar "dependiendo del rol del usuario logueado" la habilitación de ciertos
        /// componentes visuales "controles" para acceder a las funcionalidades del sistema.
        /// </summary>
        /// <param name="CodGrupo_Contacto">CodGrupo del contacto = "Rol del usuario logueado".</param>
        private void EvaluarCampos(Int32 CodGrupo_Contacto)
        {
            try
            {
                if (CodGrupo_Contacto == Constantes.CONST_Interventor)
                {
                    #region Habilitar campos que el Interventor puede operar.

                    //Controles para "Adicionar Actividad al Plan Operativo".
                    lblvalidador.Visible = true;
                    Adicionar.Visible = true;
                    LinkButton1.Visible = true;

                    //Botones de "Pagos".
                    lnkPagos.Visible = true;

                    #endregion
                }
                else
                {
                    #region Deshabilitar/Ocultar campos que el usuario logueado NO puede operar.

                    //Controles para "Adicionar Actividad al Plan Operativo".
                    lblvalidador.Visible = false;
                    Adicionar.Visible = false;
                    LinkButton1.Visible = false;

                    //Control "Puestos Pendientes de Aprobar:".
                    lblpuestosPendientesConteo.Visible = false;

                    //Botones de "Pagos".
                    //lnkPagos.Visible = false;

                    #endregion
                }
            }
            catch { }
        }

        #endregion
    }
}