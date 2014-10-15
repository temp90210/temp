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
using Datos;
namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoOperativoMetasSociales : Negocio.Base_Page
    {
        public string codProyecto;
        public string codConvocatoria;
        public int txtTab = Constantes.CONST_SubMetas;
        public bool habilitado = false;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si está o no "realizado"...
        /// </summary>
        public Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            codProyecto = Session["codProyecto"] != null && !string.IsNullOrEmpty(Session["codProyecto"].ToString()) ? Session["codProyecto"].ToString() : "";
            codConvocatoria = Session["codConvocatoria"] != null && !string.IsNullOrEmpty(Session["codConvocatoria"].ToString()) ? Session["codConvocatoria"].ToString() : "";

            inicioEncabezado(codProyecto, codConvocatoria, txtTab);

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto, "");

            //if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_AdministradorFonade)
            if (esMiembro && !bRealizado)//!chk_realizado.Checked)
            { this.div_Post_It_1.Visible = true; }

            if (esMiembro == true && !bRealizado)
            { habilitado = true; }

            if (esMiembro == true && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
            { btnGuardar.Visible = true; }

            if (!IsPostBack)
            {
                ObtenerDatosUltimaActualizacion();
                CargarTextArea();
                CargarGridEmpleos();
                CargarGridEmprendedores();
                HabilitarCampos();
                HabilitarCampos_Texto();
            }
        }

        #region General

        protected void CargarTextArea()
        {
            try
            {
                var query = (from p in consultas.Db.ProyectoMetaSocials
                             where p.CodProyecto == Convert.ToInt32(codProyecto)
                             select new { p.PlanNacional, p.PlanRegional, p.Cluster, p.EmpleoIndirecto }).FirstOrDefault();

                txtPlanRegional.Text = query.PlanRegional;
                txtPlanNacional.Text = query.PlanNacional;
                txtCluster.Text = query.Cluster;
                txtEmpleosIndirectos.Text = query.EmpleoIndirecto.ToString();
            }
            catch
            {
                txtPlanRegional.Text = "";
                txtPlanNacional.Text = "";
                txtCluster.Text = "";
                txtEmpleosIndirectos.Text = "";
            }

        }

        private void CargarGridEmpleos()
        {
            try
            {
                DataTable respuesta = new DataTable();
                DataTable respuesta2 = new DataTable();
                string consulta = " select cast(id_cargo as int) as IdCargo, cast(cargo as varchar(100)) as Cargo, cast(valormensual as decimal) as ValorMensual, ";
                consulta += " cast(GeneradoPrimerAno as varchar) as GeneradoPrimerAnio, Joven as EsJoven, Desplazado as EsDesplazado, Madre as EsMadre, ";
                consulta += " Minoria as EsMinoria, Recluido as EsRecluido, Desmovilizado as EsDesmovilizado, Discapacitado as EsDiscapacitado,  Desvinculado as EsDesvinculado ";
                consulta += " from  proyectoempleocargo right OUTER JOIN proyectogastospersonal ";
                consulta += "on id_cargo=codcargo where codproyecto= " + codProyecto;

                respuesta = consultas.ObtenerDataTable(consulta, "text");
                //IEnumerable<BORespuestaEmpleos> respuesta = consultas.Db.ExecuteQuery<BORespuestaEmpleos>(consulta, codProyecto);

                string consulta2 = " select cast(id_insumo as int) as IdCargo, cast(nominsumo as varchar(100)) as Cargo, cast(sueldomes as decimal) as ValorMensual, ";
                consulta2 += " cast(GeneradoPrimerAno as varchar) as GeneradoPrimerAnio, Joven as EsJoven, Desplazado as EsDesplazado,Madre as EsMadre, ";
                consulta2 += " Minoria as EsMinoria, Recluido as EsRecluido,Desmovilizado as EsDesmovilizado, Discapacitado as EsDiscapacitado, Desvinculado as EsDesvinculado ";
                consulta2 += " from  proyectoinsumo inner join ProyectoProductoInsumo  on CodInsumo = Id_Insumo LEFT OUTER JOIN proyectoempleomanoobra ";
                consulta2 += " on id_insumo=codmanoobra where codtipoinsumo=2 and codproyecto= " + codProyecto;

                respuesta2 = consultas.ObtenerDataTable(consulta2, "text");
                //IEnumerable<BORespuestaEmpleos> respuesta2 = consultas.Db.ExecuteQuery<BORespuestaEmpleos>(consulta2, codProyecto);


                gw_Empleos.DataSource = respuesta;
                gw_ManoObra.DataSource = respuesta2;
                gw_Empleos.DataBind();
                gw_ManoObra.DataBind();

            }
            catch (ArgumentException)
            {
                /*gw_Empleos.DataBind();
                gw_ManoObra.DataBind();*/
            }
        }

        private void CargarGridEmprendedores()
        {

            var query = (from p in consultas.Db.ProyectoContactos
                         from c in consultas.Db.Contactos
                         where p.CodContacto == c.Id_Contacto &&
                          p.CodProyecto == Convert.ToInt32(codProyecto) &&
                          p.CodRol == Constantes.CONST_RolEmprendedor &&
                          p.Inactivo == false
                         orderby c.Nombres, c.Apellidos ascending
                         select new { c.Id_Contacto, nombres = c.Nombres + " " + c.Apellidos, p.Beneficiario, p.Participacion });

            gw_emprendedores.DataSource = query;
            gw_emprendedores.DataBind();

            for (int i = 0; i < gw_emprendedores.Rows.Count; i++)
            {

                if (gw_emprendedores.DataKeys[i].Value.ToString() != usuario.IdContacto.ToString())
                {
                    ((CheckBox)gw_emprendedores.Rows[i].FindControl("chkBeneficiario")).Checked = true;
                    ((CheckBox)gw_emprendedores.Rows[i].FindControl("chkBeneficiario")).Enabled = false;
                    ((TextBox)gw_emprendedores.Rows[i].FindControl("txtParticipacion")).Visible = false;
                    ((Label)gw_emprendedores.Rows[i].FindControl("lblParticipacion")).Visible = true;

                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarFormulario())
            {
                try
                {
                    var query = (from p in consultas.Db.ProyectoMetaSocials
                                 where p.CodProyecto == Convert.ToInt32(codProyecto)
                                 select p).FirstOrDefault();
                    //update
                    query.PlanNacional = txtPlanNacional.Text;
                    query.PlanRegional = txtPlanRegional.Text;
                    query.Cluster = txtCluster.Text;
                    query.EmpleoIndirecto = Convert.ToInt16(txtEmpleosIndirectos.Text);
                }
                catch
                {
                    //inserta
                    Datos.ProyectoMetaSocial datosNuevos = new ProyectoMetaSocial()
                    {
                        CodProyecto = Convert.ToInt32(codProyecto),
                        PlanNacional = txtPlanNacional.Text,
                        PlanRegional = txtPlanRegional.Text,
                        Cluster = txtCluster.Text,
                        EmpleoIndirecto = Convert.ToInt16(txtEmpleosIndirectos.Text)
                    };
                    consultas.Db.ProyectoMetaSocials.InsertOnSubmit(datosNuevos);

                }
                //consultas.Db.ExecuteCommand(UsuarioActual());
                consultas.Db.SubmitChanges();
                //Actualizar fecha modificación del tab.
                prActualizarTab(txtTab.ToString(), codProyecto);
                ObtenerDatosUltimaActualizacion();
                RegistrarEmpleos();
                RegistrarManoObra();
                RegistrarParticipacion();

                CargarTextArea();
                CargarGridEmpleos();
                CargarGridEmprendedores();
            }
        }

        protected bool ValidarFormulario()
        {
            if (txtPlanNacional.Text.Trim() == "")
            {
                Alert1.Ver("El Plan Nacional es requerido", true);
                txtPlanNacional.Focus();
                return false;
            }
            if (txtPlanRegional.Text.Trim() == "")
            {
                Alert1.Ver("El Plan Regional es requerido", true);
                txtPlanRegional.Focus();
                return false;
            }
            if (txtCluster.Text.Trim() == "")
            {
                Alert1.Ver("El Plan Regional es requerido", true);
                txtCluster.Focus();
                return false;
            }

            if (txtPlanNacional.Text.Trim().Length > 450)
            {
                Alert1.Ver("El campo excede el tamaño permitido", true);
                txtPlanNacional.Focus();
                return false;
            }
            if (txtPlanRegional.Text.Trim().Length > 450)
            {
                Alert1.Ver("El campo excede el tamaño permitido", true);
                txtPlanRegional.Focus();
                return false;
            }
            if (txtCluster.Text.Trim().Length > 450)
            {
                Alert1.Ver("El campo excede el tamaño permitido", true);
                txtCluster.Focus();
                return false;
            }

            for (int i = 0; i < gw_ManoObra.Rows.Count; i++)
            {
                TextBox sueldo = ((TextBox)gw_ManoObra.Rows[i].FindControl("txtSueldo"));
                if (sueldo.Text.Trim() == "")
                {
                    sueldo.Text = "0";
                }
                else
                {
                    decimal valor = 0;
                    if (!decimal.TryParse(sueldo.Text, out valor))
                    {
                        Alert1.Ver("El valor debe ser numérico", true);
                        sueldo.Focus();
                        return false;
                    }
                }
            }

            TextBox participacion = ((TextBox)gw_emprendedores.Rows[0].FindControl("txtParticipacion"));
            if (participacion.Visible == true)
            {
                if (participacion.Text.Trim() == "")
                {
                    participacion.Text = "0";
                }
                else
                {
                    decimal valor = 0;
                    if (!decimal.TryParse(participacion.Text, out valor))
                    {
                        Alert1.Ver("El valor debe ser numérico", true);
                        participacion.Focus();
                        return false;
                    }
                    if (Convert.ToInt32(participacion.Text) > 100 || Convert.ToInt32(participacion.Text) < 0)
                    {
                        Alert1.Ver("Porcentaje no vlaido", true);
                        participacion.Focus();
                        return false;
                    }
                }
            }

            return true;
        }

        protected void RegistrarEmpleos()
        {
            for (int i = 0; i < gw_Empleos.Rows.Count; i++)
            {
                int codCargo = Convert.ToInt32(((HiddenField)gw_Empleos.Rows[i].FindControl("txtCodCargo")).Value);

                try
                {
                    var queryEmpleo = (from p in consultas.Db.ProyectoEmpleoCargos
                                       where p.CodCargo == codCargo
                                       select p).First();

                    queryEmpleo.GeneradoPrimerAno = Convert.ToByte(((DropDownList)gw_Empleos.Rows[i].FindControl("ddlGeneradoMes")).SelectedValue);
                    queryEmpleo.Joven = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkEdad18_24")).Checked;
                    queryEmpleo.Desplazado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesplazado")).Checked;
                    queryEmpleo.Madre = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkMadreCabeza")).Checked;
                    queryEmpleo.Minoria = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkMinoriaEtnica")).Checked;
                    queryEmpleo.Recluido = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkRecluidoCarceles")).Checked;
                    queryEmpleo.Desmovilizado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesmovilizado")).Checked;
                    queryEmpleo.Discapacitado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDiscapacitado")).Checked;
                    queryEmpleo.Desvinculado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesvinculado")).Checked;
                }
                catch
                {
                    ProyectoEmpleoCargo datoNuevo = new ProyectoEmpleoCargo()
                    {
                        CodCargo = codCargo,
                        GeneradoPrimerAno = Convert.ToByte(((DropDownList)gw_Empleos.Rows[i].FindControl("ddlGeneradoMes")).SelectedValue),
                        Joven = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkEdad18_24")).Checked,
                        Desplazado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesplazado")).Checked,
                        Madre = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkMadreCabeza")).Checked,
                        Minoria = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkMinoriaEtnica")).Checked,
                        Recluido = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkRecluidoCarceles")).Checked,
                        Desmovilizado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesmovilizado")).Checked,
                        Discapacitado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDiscapacitado")).Checked,
                        Desvinculado = ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesvinculado")).Checked
                    };
                    consultas.Db.ProyectoEmpleoCargos.InsertOnSubmit(datoNuevo);
                }
            }
            consultas.Db.SubmitChanges();
        }

        protected void RegistrarManoObra()
        {
            for (int i = 0; i < gw_ManoObra.Rows.Count; i++)
            {
                int codInsumo = Convert.ToInt32(((HiddenField)gw_ManoObra.Rows[i].FindControl("txtCodInsumo")).Value);

                try
                {
                    var queryEmpleo = (from p in consultas.Db.ProyectoEmpleoManoObras
                                       where p.CodManoObra == codInsumo
                                       select p).FirstOrDefault();

                    queryEmpleo.SueldoMes = Convert.ToDecimal(((TextBox)gw_ManoObra.Rows[i].FindControl("txtSueldo")).Text);
                    queryEmpleo.GeneradoPrimerAno = Convert.ToByte(((DropDownList)gw_ManoObra.Rows[i].FindControl("ddlGeneradoMes")).SelectedValue);
                    queryEmpleo.Joven = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkEdad18_24")).Checked;
                    queryEmpleo.Desplazado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesplazado")).Checked;
                    queryEmpleo.Madre = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkMadreCabeza")).Checked;
                    queryEmpleo.Minoria = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkMinoriaEtnica")).Checked;
                    queryEmpleo.Recluido = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkRecluidoCarceles")).Checked;
                    queryEmpleo.Desmovilizado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesmovilizado")).Checked;
                    queryEmpleo.Discapacitado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDiscapacitado")).Checked;
                    queryEmpleo.Desvinculado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesvinculado")).Checked;
                }
                catch
                {
                    ProyectoEmpleoManoObra datoNuevo = new ProyectoEmpleoManoObra()
                    {
                        CodManoObra = codInsumo,
                        SueldoMes = Convert.ToDecimal(((TextBox)gw_ManoObra.Rows[i].FindControl("txtSueldo")).Text),
                        GeneradoPrimerAno = Convert.ToByte(((DropDownList)gw_ManoObra.Rows[i].FindControl("ddlGeneradoMes")).SelectedValue),
                        Joven = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkEdad18_24")).Checked,
                        Desplazado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesplazado")).Checked,
                        Madre = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkMadreCabeza")).Checked,
                        Minoria = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkMinoriaEtnica")).Checked,
                        Recluido = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkRecluidoCarceles")).Checked,
                        Desmovilizado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesmovilizado")).Checked,
                        Discapacitado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDiscapacitado")).Checked,
                        Desvinculado = ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesvinculado")).Checked
                    };
                    consultas.Db.ProyectoEmpleoManoObras.InsertOnSubmit(datoNuevo);
                }
            }
            consultas.Db.SubmitChanges();
        }

        protected void RegistrarParticipacion()
        {
            try
            {
                var queryEmpleo = (from p in consultas.Db.ProyectoContactos
                                   where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                                   p.CodContacto == usuario.IdContacto &&
                                   p.Inactivo == false &&
                                   p.FechaFin == null
                                   select p).FirstOrDefault();

                string participacion = ((TextBox)gw_emprendedores.Rows[0].FindControl("txtParticipacion")).Text;
                bool benficiario = ((CheckBox)gw_emprendedores.Rows[0].FindControl("chkBeneficiario")).Checked;

                queryEmpleo.Beneficiario = benficiario;
                queryEmpleo.Participacion = Convert.ToDouble(participacion);

            }
            catch
            {
            }

            consultas.Db.SubmitChanges();
        }

        protected void HabilitarCampos()
        {
            txtPlanRegional.Enabled = habilitado;
            txtPlanNacional.Enabled = habilitado;
            txtEmpleosIndirectos.Enabled = habilitado;
            txtCluster.Enabled = habilitado;

            for (int i = 0; i < gw_Empleos.Rows.Count; i++)
            {

                ((DropDownList)gw_Empleos.Rows[i].FindControl("ddlGeneradoMes")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkEdad18_24")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesplazado")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkMadreCabeza")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkMinoriaEtnica")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkRecluidoCarceles")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesmovilizado")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDiscapacitado")).Enabled = habilitado;
                ((CheckBox)gw_Empleos.Rows[i].FindControl("chkDesvinculado")).Enabled = habilitado;
            }


            for (int i = 0; i < gw_ManoObra.Rows.Count; i++)
            {


                ((TextBox)gw_ManoObra.Rows[i].FindControl("txtSueldo")).Enabled = habilitado; ;
                ((DropDownList)gw_ManoObra.Rows[i].FindControl("ddlGeneradoMes")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkEdad18_24")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesplazado")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkMadreCabeza")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkMinoriaEtnica")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkRecluidoCarceles")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesmovilizado")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDiscapacitado")).Enabled = habilitado;
                ((CheckBox)gw_ManoObra.Rows[i].FindControl("chkDesvinculado")).Enabled = habilitado;
            }

        }
        #endregion General

        #region Métodos de Mauricio Arias Olave.

        public void HabilitarCampos_Texto()
        {
            //Inicializar variables.
            bool EsMiembro = false;
            bool bRealizado = true;

            try
            {
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

                if (EsMiembro && !bRealizado)
                {
                    #region Deshabilitar campos...

                    txtPlanNacional.Enabled = false;
                    txtPlanRegional.Enabled = false;
                    txtCluster.Enabled = false;
                    tabla_1.Attributes.Add("", "disabled");

                    #endregion
                }
            }
            catch { }
        }

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
            bool EsNuevo = true;
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
        /// Mauricio Arias Olave.
        /// 11/09/2014.
        /// Des/habilitar el CheckBox del beneficiario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gw_emprendedores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var chk = e.Row.FindControl("chkBeneficiario") as CheckBox;
                var txt = e.Row.FindControl("txtParticipacion") as TextBox;

                if (chk != null && txt != null)
                { chk.Enabled = habilitado; txt.Enabled = habilitado; }
            }
        }
    }

    public class BORespuestaEmpleos
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public decimal ValorMensual { get; set; }
        public int GeneradoPrimerAnio { get; set; }
        public int EsJoven { get; set; }
        public int EsDesplazado { get; set; }
        public int EsMadre { get; set; }
        public int EsMinoria { get; set; }
        public int EsRecluido { get; set; }
        public int EsDesmovilizado { get; set; }
        public int EsDiscapacitado { get; set; }
        public int EsDesvinculado { get; set; }
    }
}
