using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoInfraestructura : System.Web.UI.Page
    {
        Consultas oConsultas = new Consultas();
        private ProyectoInfraestructura entity;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                lblfecha.Text = DateTime.Now.ToShortDateString();
                CargarTipoInfraestructura();
                ObtenerById(Request["Codigo"],Request["Accion"]);
            }
        }

        #region Metodos 

        public void CargarTipoInfraestructura()
        {
            List<TipoInfraestructura> lstInfraestructura = oConsultas.Db.TipoInfraestructuras.ToList();
            if (lstInfraestructura.Any())
            {
               
                DdlTpoInfraestructura.DataTextField = "NomTipoInfraestructura";
                DdlTpoInfraestructura.DataValueField = "Id_TipoInfraestructura";
                DdlTpoInfraestructura.DataSource = lstInfraestructura;
                DdlTpoInfraestructura.DataBind();
                DdlTpoInfraestructura.Items.Insert(0, new ListItem("Seleccione", "0"));
            }

        }

        public void Insertar()
        {
            try
            {


                entity = new ProyectoInfraestructura
                             {
                                 codProyecto = (int)(!string.IsNullOrEmpty(Request["Codigo"]) ? Convert.ToInt64(Request["Codigo"]) : 0),
                               NomInfraestructura = TxtNombre.Text.Trim(),
                               CodTipoInfraestructura = Convert.ToByte(DdlTpoInfraestructura.SelectedValue),
                               FechaCompra = Convert.ToDateTime(TxtFecha.Text),
                               Unidad = !string.IsNullOrEmpty(TxtUnidadTipo.Text) ? TxtUnidadTipo.Text.Trim() : string.Empty,
                               ValorCredito = !string.IsNullOrEmpty(TxtPorcentaje.Text) ? Convert.ToInt32(TxtPorcentaje.Text.Trim()) : 0,
                               ValorUnidad = (!string.IsNullOrEmpty(TxtValorU.Text) ? Convert.ToDecimal(TxtValorU.Text.Trim()) : 0),
                               Cantidad = (!string.IsNullOrEmpty(TxtCantidad.Text) ? Convert.ToDouble(TxtCantidad.Text.Trim()) : 0),
                               PeriodosAmortizacion =  Convert.ToByte(DdlPeriodo.SelectedValue),
                               SistemaDepreciacion = !string.IsNullOrEmpty(Txtsistema.Text) ? Txtsistema.Text.Trim() : string.Empty,

                             };

                oConsultas.Db.ProyectoInfraestructuras.InsertOnSubmit(entity);
                oConsultas.Db.SubmitChanges();
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "alert('Registro Creado Exitosamente!');", true);
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.opener.location.reload();window.close();", true);

            }
            catch (Exception ex)
            {
              string mensaje =  ex.Message;
            }
           

        }

        public void Update()
        {
            try
            {
                var consult = oConsultas.Db.ProyectoInfraestructuras.Single(
                               p => p.Id_ProyectoInfraestructura == Convert.ToInt64(Request["Codigo"]));
                if (consult != null)
                {

                     consult.NomInfraestructura = TxtNombre.Text.Trim();
                     consult.CodTipoInfraestructura = Convert.ToByte(DdlTpoInfraestructura.SelectedValue);
                      consult. FechaCompra = Convert.ToDateTime(TxtFecha.Text);
                      consult. Unidad = !string.IsNullOrEmpty(TxtUnidadTipo.Text) ? TxtUnidadTipo.Text.Trim() : string.Empty;
                      consult. ValorCredito = !string.IsNullOrEmpty(TxtPorcentaje.Text) ? Convert.ToInt32(TxtPorcentaje.Text.Trim()) : 0;
                      consult. ValorUnidad = (!string.IsNullOrEmpty(TxtValorU.Text) ? Convert.ToDecimal(TxtValorU.Text.Trim()) : 0);
                      consult. Cantidad = (!string.IsNullOrEmpty(TxtCantidad.Text) ? Convert.ToDouble(TxtCantidad.Text.Trim()) : 0);
                      consult.PeriodosAmortizacion = Convert.ToByte(DdlPeriodo.SelectedValue);
                      consult. SistemaDepreciacion = !string.IsNullOrEmpty(Txtsistema.Text) ? Txtsistema.Text.Trim() : string.Empty;
                      oConsultas.Db.SubmitChanges();
                      ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "alert('Registro Actualizado Exitosamente!');", true);
                      ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.opener.location.reload();window.close();", true);
                   

                }

               
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
            }


        }

        public void ObtenerById(string codigo,string accion)
        {
            try
            {
                if (!string.IsNullOrEmpty(codigo) && accion == "Editar")
                {
                    var proyecto =
                        oConsultas.Db.ProyectoInfraestructuras.Single(
                            p => p.Id_ProyectoInfraestructura == Convert.ToInt64(codigo));
                    if (proyecto!=null)
                    {
                        TxtNombre.Text = proyecto.NomInfraestructura;
                        TxtFecha.Text = proyecto.FechaCompra.ToShortDateString();
                        TxtCantidad.Text = proyecto.Cantidad.ToString();
                        TxtUnidadTipo.Text = proyecto.Unidad;
                        DdlTpoInfraestructura.SelectedValue =  Convert.ToString(proyecto.CodTipoInfraestructura);
                        DdlPeriodo.SelectedValue = Convert.ToString(proyecto.PeriodosAmortizacion);
                        TxtValorU.Text = proyecto.ValorUnidad.ToString();
                        TxtPorcentaje.Text =Convert.ToString( proyecto.ValorCredito);
                        Txtsistema.Text = proyecto.SistemaDepreciacion;
                    }
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
            }


        }

        #endregion

        protected void BtnCrear_Click(object sender, EventArgs e)
        {
            if (Request["Accion"]!="Editar")
            {
                Insertar();
            }else Update();
         
        }

        protected void BtnCancelar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.opener.location.reload();window.close();", true);
        }
    }
}