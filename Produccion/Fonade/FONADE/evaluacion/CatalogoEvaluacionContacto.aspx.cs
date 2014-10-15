#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>27 - 02 - 2014</Fecha>
// <Archivo>CatalogoEvaluacionContacto.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Linq;
using System.Web.UI;
using Datos;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoEvaluacionContacto : Page
    {
        private Consultas _consultas;
        private int codconvocatoria;
        private int codproyecto;

        public int CodigoContacto
        {
            get { return (int) ViewState["codigo"]; }
            set { ViewState["codigo"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatos();
            }
        }

        public void CargarDatos()
        {
            CodigoContacto = (int)(!string.IsNullOrEmpty(Request["codContacto"])
                                            ? Convert.ToInt64(Request["codContacto"])
                                            : 0);
            codproyecto =Convert.ToInt32(Session["CodProyecto"].ToString());
            codconvocatoria = Convert.ToInt32(Session["CodConvocatoria"].ToString()); 



            _consultas = new Consultas();

            // cargamos la informacion de la evaluacion del contacto
            var evalucacion = _consultas.Db.EvaluacionContactos.FirstOrDefault(ec => ec.CodProyecto == codproyecto
                                                                            && ec.CodConvocatoria == codconvocatoria
                                                                            && ec.CodContacto == CodigoContacto);


            //  cargamos la informacion del contacto
            var query = _consultas.Db.Contactos.Join(_consultas.Db.ProyectoContactos,
                                                     c => c.Id_Contacto, pc => pc.CodContacto,
                                                     (c, pc) => new
                                                                    {
                                                                        Idcontacto = c.Id_Contacto,
                                                                        NombreCompleto =
                                                                        string.Concat(c.Nombres, c.Apellidos),
                                                                        Beneficiarios = pc.Beneficiario,
                                                                        Inactivos = c.Inactivo
                                                                    }).FirstOrDefault(
                                                                        p =>
                                                                        p.Idcontacto == CodigoContacto &&
                                                                        p.Inactivos == false);

            if (evalucacion!=null)
            {

                txtdinero.Text = evalucacion.AporteDinero.ToString();
                txtespecie.Text = evalucacion.AporteEspecie.ToString();
                txtDetalleespecie.Text = evalucacion.DetalleEspecie;
              
              
            }

            if (query.Idcontacto!=0)
            {


                lblemprendedor.Text = query.Beneficiarios == true ? "SI" : "NO";
                TxtNombre.Text = query.NombreCompleto;
            }


        }

        public void Actulizar()
        {
            CodigoContacto = (int)(!string.IsNullOrEmpty(Request["codContacto"])
                                            ? Convert.ToInt64(Request["codContacto"])
                                            : 0);
            codproyecto = Convert.ToInt32(Session["CodProyecto"].ToString());
            codconvocatoria = Convert.ToInt32(Session["CodConvocatoria"].ToString()); 

            _consultas = new Consultas();
            var evaluacionContacto = new EvaluacionContacto();

            var evalucacion = _consultas.Db.EvaluacionContactos.FirstOrDefault(ec => ec.CodProyecto == codproyecto
                                                                             && ec.CodConvocatoria == codconvocatoria
                                                                             && ec.CodContacto == CodigoContacto);

            if (evalucacion!=null)
            {

                evalucacion.AporteDinero = !string.IsNullOrEmpty(txtdinero.Text) ? Convert.ToDouble(txtdinero.Text) : 0;
                evalucacion.AporteEspecie = !string.IsNullOrEmpty(txtespecie.Text) ? Convert.ToDouble(txtespecie.Text) : 0;
                evalucacion.DetalleEspecie = txtDetalleespecie.Text;
                _consultas.Db.SubmitChanges();
                Redireccionar("Su Registro fue modificado exitosamente!");

            }else
            {

                evaluacionContacto.AporteDinero = !string.IsNullOrEmpty(txtdinero.Text) ? Convert.ToDouble(txtdinero.Text) : 0;
                evaluacionContacto.AporteEspecie = !string.IsNullOrEmpty(txtespecie.Text) ? Convert.ToDouble(txtespecie.Text) : 0;
                evaluacionContacto.DetalleEspecie = txtDetalleespecie.Text;
                evaluacionContacto.Entidades = string.Empty;
                evaluacionContacto.CodConvocatoria = codconvocatoria;
                evaluacionContacto.CodProyecto = codproyecto;
                evaluacionContacto.CuentasCorrientes = 0;
                evaluacionContacto.ValorCartera = 0;
                evaluacionContacto.ValorOtrasCarteras = 0;
                evaluacionContacto.CodContacto = CodigoContacto;
                evaluacionContacto.Entidades = string.Empty;
                _consultas.Db.EvaluacionContactos.InsertOnSubmit(evaluacionContacto);
                _consultas.Db.SubmitChanges();
                Redireccionar("Registro Exitoso!");
            }
        }
        public void Redireccionar(string mensaje)
        {
            if (!string.IsNullOrEmpty(mensaje))
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "alert('" + mensaje + "');", true);
            }
           
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.opener.location.reload();window.close();", true);
        }

        protected void BtnCrear_Click(object sender, EventArgs e)
        {
            Actulizar();

        }

        protected void BtnCancelar_Click(object sender, EventArgs e)
        {
            Redireccionar("");
        }
    }
}