using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.Controles
{
    public partial class CatalogoGasto : System.Web.UI.UserControl
    {

        public enum Accion { Nuevo,Editar,Borrar };
        protected Consultas consulta = new Consultas();
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
        public string Cargar(Accion accion, string codProyecto, string idGasto,string tipo)
        {
            hddAccion.Value = accion.ToString();
            hddCodProyecto.Value = codProyecto;
            hddIdGasto.Value = idGasto;
            hddTipo.Value = tipo;

            if (accion == Accion.Nuevo)
            {
                btnGasto.Text = "Crear";
                LimpiarCampos();
            }

            if (accion == Accion.Editar)
            {
                btnGasto.Text = "Actualizar";
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
            txtDescripcion.Text = "";
            txtValor.Text = "";
            txtDescripcion.ReadOnly = false;
        }

        protected void llenarCampos()
        {
            ProyectoGasto dato = RegistroActual();
            txtDescripcion.Text = dato.Descripcion;
            txtValor.Text = Convert.ToInt32(dato.Valor).ToString();;
            if (dato.Protegido)
            {
                txtDescripcion.ReadOnly = true;
            }
        }

        protected ProyectoGasto RegistroActual()
        {
            var query = (from p in consulta.Db.ProyectoGastos
                         where p.Id_Gasto == Convert.ToInt32(hddIdGasto.Value)
                         select p).First();

            return query;
        }

        protected void btnGasto_Click(object sender, EventArgs e)
        {
            try
            {
                if (hddAccion.Value == Accion.Nuevo.ToString())
                {
                    CrearNuevo();
                }
                else if (hddAccion.Value == Accion.Editar.ToString())
                {
                    ActualizarRegistro();
                }

                consulta.Db.SubmitChanges();
                Error = "OK";
            }
            catch
            {
                Error = "ERROR";
            }
        }

        protected void CrearNuevo()
        {

            var query= (from p in consulta.Db.ProyectoGastos 
                            where p.Descripcion==txtDescripcion.Text &&
                            p.CodProyecto==Convert.ToInt32(hddCodProyecto.Value)
                            select new{p.Descripcion});
            if (query.Count() == 0)
            {
                ProyectoGasto dato = new ProyectoGasto();
                dato.CodProyecto = Convert.ToInt32(hddCodProyecto.Value);
                dato.Descripcion = txtDescripcion.Text;
                dato.Valor = Convert.ToDecimal(txtValor.Text);
                dato.Tipo = hddTipo.Value;
                consulta.Db.ProyectoGastos.InsertOnSubmit(dato);
                Error = "OK";
            }
            else
            {
                Error = "Registro Duplicado";
            }   
        }

        protected void ActualizarRegistro()
        {
            var query = (from p in consulta.Db.ProyectoGastos 
                            where p.Descripcion==txtDescripcion.Text &&
                            p.CodProyecto==Convert.ToInt32(hddCodProyecto.Value) &&
                            p.Id_Gasto != Convert.ToInt32(hddIdGasto.Value)
                            select new{p.Descripcion});
              if (query.Count() == 0)
              {
                  ProyectoGasto dato = RegistroActual();
                  dato.Descripcion = txtDescripcion.Text;
                  dato.Valor = Convert.ToDecimal(txtValor.Text);
                  Error = "OK";
              }
              else
              {
                  Error = "Registro Duplicado";
              }
        }


        protected void btnCancelarGasto_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            hddAccion.Value = "";
            hddCodProyecto.Value = "";
            hddIdGasto.Value = "";
            hddTipo.Value = "";
        }

        protected string BorrarCargo()
        {

            consulta.Db.ExecuteCommand("Delete from ProyectoGastos where protegido=0 and Id_Gasto={0}", Convert.ToInt32(hddIdGasto.Value));
            consulta.Db.SubmitChanges();
            string respuesta = "OK";

            return respuesta;
        }


    }
}