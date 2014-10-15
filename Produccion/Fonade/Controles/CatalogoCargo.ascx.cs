using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.Controles
{
    public partial class CatalogoCargo : System.Web.UI.UserControl
    {

        public enum Accion { Nuevo,Editar,Borrar };
        protected Consultas consulta = new Consultas();
        protected string NumeroSMLVNV;
        public string Error { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Metodo utilizado para diligenciar el fomulario, si la accion es Borrar el metodo le retornar la confirmacion.
        /// </summary>
        /// <param name="accion"></param>
        /// <param name="codProyecto"></param>
        /// <param name="idCargo"></param>
        public string Cargar(Accion accion, string codProyecto, string idCargo)
        {
            hddAccion.Value = accion.ToString();
            hddCodProyecto.Value = codProyecto;
            hddIdCargo.Value = idCargo;

            if (accion == Accion.Nuevo)
            {
                btnCargo.Text = "Crear";
                LimpiarCampos();
            }

            if (accion == Accion.Editar)
            {
                btnCargo.Text = "Actualizar";
                LimpiarCampos();
                llenarCampos();
            }

            if (accion == Accion.Borrar)
            {
                return BorrarCargo();
            }
            return "OK";
        }

        protected void LimpiarCampos()
        {
            txtCargo.Text = "";
            ddlDedicacion.SelectedIndex = 0;
            ddlTipoContratacion.SelectedIndex = 0;
            txtValorMensual.Text = "";
            txtValorAnual.Text = "";
            txtOtrosGastos.Text = "";
            txtObservacion.Text = "";
        }

        protected void llenarCampos()
        {
            ProyectoGastosPersonal dato = RegistroActual();

            txtCargo.Text = dato.Cargo;
            ddlDedicacion.SelectedValue = dato.Dedicacion;
            ddlTipoContratacion.SelectedValue = dato.TipoContratacion;
            txtValorMensual.Text = Convert.ToInt32(dato.ValorMensual).ToString();
            txtValorAnual.Text = Convert.ToInt32(dato.ValorAnual).ToString();
            txtOtrosGastos.Text = Convert.ToInt32(dato.OtrosGastos).ToString();
            txtObservacion.Text = dato.Observacion;
        }

        protected ProyectoGastosPersonal RegistroActual()
        {
            var query = (from p in consulta.Db.ProyectoGastosPersonals
                         where p.Id_Cargo == Convert.ToInt32(hddIdCargo.Value)
                         select p).First();

            return query;
        }

        protected void btnCargo_Click(object sender, EventArgs e)
        {
            try
            {
                if (hddAccion.Value == Accion.Nuevo.ToString())
                {
                    ProyectoGastosPersonal dato = new ProyectoGastosPersonal();
                    dato.CodProyecto = Convert.ToInt32(hddCodProyecto.Value);
                    dato.Cargo = txtCargo.Text;
                    dato.Dedicacion = ddlDedicacion.SelectedValue;
                    dato.TipoContratacion = ddlTipoContratacion.SelectedValue;
                    dato.ValorMensual = Convert.ToDecimal(txtValorMensual.Text);
                    dato.ValorAnual = Convert.ToDecimal(txtValorAnual.Text);
                    dato.OtrosGastos = Convert.ToDecimal(txtOtrosGastos.Text);
                    dato.Observacion = txtObservacion.Text;
                    consulta.Db.ProyectoGastosPersonals.InsertOnSubmit(dato);
                }
                else if (hddAccion.Value == Accion.Editar.ToString())
                {
                    ProyectoGastosPersonal dato = RegistroActual();
                    dato.Cargo = txtCargo.Text;
                    dato.Dedicacion = ddlDedicacion.SelectedValue;
                    dato.TipoContratacion = ddlTipoContratacion.SelectedValue;
                    dato.ValorMensual = Convert.ToDecimal(txtValorMensual.Text);
                    dato.ValorAnual = Convert.ToDecimal(txtValorAnual.Text);
                    dato.OtrosGastos = Convert.ToDecimal(txtOtrosGastos.Text);
                    dato.Observacion = txtObservacion.Text;
                }

                consulta.Db.SubmitChanges();
                Error = "OK";
            }
            catch
            {
                Error = "ERROR";
            }
        }

        protected void btnCancelarCargo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            hddAccion.Value = "";
            hddCodProyecto.Value = "";
            hddIdCargo.Value = "";
        }

        protected string BorrarCargo()
        {
            int numeroEmpleosNV = 0;
            int codigoConvocatoria = 0;
            int recauctual = 0;
            string respuesta = "";

            try
            {
                var queryConvoca = (from cp in consulta.Db.ProyectoFinanzasIngresos
                                    where cp.CodProyecto == Convert.ToInt32(hddCodProyecto.Value)
                                    select new { cp.Recursos }
                                        ).First();
                recauctual = queryConvoca.Recursos;
            }
            catch
            {
                recauctual = 0;
            }


            try
            {
                var queryConvoca = (from cp in consulta.Db.ConvocatoriaProyectos
                                    where cp.CodProyecto == Convert.ToInt32(hddCodProyecto.Value)
                                    select new { cp.CodConvocatoria }
                                        ).First();
                codigoConvocatoria = queryConvoca.CodConvocatoria;
            }
            catch
            {
                codigoConvocatoria = 0;
            }

            try
            {
                var queryCant = (from p in consulta.Db.ProyectoGastosPersonals
                                 where p.CodProyecto == Convert.ToInt32(hddCodProyecto.Value)
                                 select new { p.CodProyecto });

                var queryCant2 = (from p in consulta.Db.ProyectoInsumos
                                  from pi in consulta.Db.ProyectoProductoInsumos
                                  where p.Id_Insumo == pi.CodInsumo &&
                                  p.codTipoInsumo == 2 &&
                                  p.CodProyecto == Convert.ToInt32(hddCodProyecto.Value)
                                  select new { p.CodProyecto });

                numeroEmpleosNV = (queryCant.ToList().Count + queryCant2.ToList().Count) - 1;
            }
            catch
            {
                numeroEmpleosNV = 0;
            }
            ConsultarSalarioSMLVNV(1, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(2, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(3, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(4, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(5, numeroEmpleosNV, codigoConvocatoria);
            ConsultarSalarioSMLVNV(6, numeroEmpleosNV, codigoConvocatoria);


            if (recauctual > Convert.ToInt32(NumeroSMLVNV))
            {
                respuesta = "No se puede borrar. La cantidad de recursos solicitados (smlv) son superiores a los permitidos según la cantidad de empleos generados. Modifíquelos y asegúrese que sea menor o igual a " + NumeroSMLVNV + " (smlv)";
            }
            else
            {
                consulta.Db.ExecuteCommand("Delete from ProyectoEmpleoCargo where codCargo={0}", Convert.ToInt32(hddIdCargo.Value));
                consulta.Db.ExecuteCommand("Delete from ProyectoGastosPersonal where Id_Cargo={0}", Convert.ToInt32(hddIdCargo.Value));
                consulta.Db.SubmitChanges();
                respuesta = "OK";
            }
            return respuesta;
        }

        private void ConsultarSalarioSMLVNV(int regla, int numeroEmpleosNV, int codigoConvocatoria)
        {

            try
            {
                var queryRegla = (from p in consulta.Db.ConvocatoriaReglaSalarios
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

    }
}