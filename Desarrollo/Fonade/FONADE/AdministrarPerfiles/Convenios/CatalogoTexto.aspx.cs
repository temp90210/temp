using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.AdministrarPerfiles.Convenios
{
    public partial class CatalogoTexto :   Negocio.Base_Page
    {

        string crearAdmin = "Crear Administrador";
        string crearGerenteEvaluador = "Crear GerenteEvaluador";
        string crearCallCenter = "Crear Call Center";
        string crearGerenteInterventor = "Crear Gerente Interventor";
        string crearPerfilFiduciario = "Crear Perfil Fiduciario";
        String grupos1;
        int idusuario;
        string[] codgrupousuario;
        public String[] grupos = { "0" };        
        protected void void_ModificarDatos(string[] codgrupo, int Grupocontacto)
        {
            if (Request.QueryString["CodAnexo"] != null)
            {
                int codigocontacto = Int32.Parse(Request.QueryString["CodAnexo"]);
                var query = (from t in consultas.Db.Textos
                             where t.Id_Texto == codigocontacto
                             select t).FirstOrDefault();
                query.Texto1 = tb_Descripción.Text;
                try
                {
                    void_show("el texto ha sido actualizado correctamente", true);
                    consultas.Db.SubmitChanges();                   
                }
                catch ( Exception ex)
                {}
            }
        }
        protected void void_traerDatos(string[] codgrupo, int codusuario)
        {
            if (Request.QueryString["CodAnexo"] != null)
            {
            int codigoContacto = Int32.Parse(Request.QueryString["CodAnexo"]);
            var query = (from c in consultas.Db.Textos
                         where c.Id_Texto == codusuario
                         select c).FirstOrDefault();
            lbl_tituloanexo.Text=query.NomTexto;
            tb_Descripción.Text= query.Texto1;
            }
        }
        protected void void_ObtenerParametros()
        {
            if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
            {
                pnl_Anexos.Visible = false;
                pnl_crearEditar.Visible = true;
                pnl_Anexos.Visible = false;
            }
        }
        protected void void_show(string texto, bool mostrar)
        {
            //lbl_popup.Visible = mostrar;
            //lbl_popup.Text = texto;
            //mpe1.Enabled = mostrar;
            //mpe1.Show();

            string queryRedir = Request.Url.Query;

            queryRedir = queryRedir.Substring(0, queryRedir.IndexOf('&'));

            string redirePagina = string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Request.Url.AbsolutePath, queryRedir);

            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('" + texto + "');location.href='" + redirePagina + "'</script>");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string accion="";
                int codigoContacto = 0;
                if (!String.IsNullOrEmpty(Request.QueryString["CodAnexo"]))
                {
                    codigoContacto = Int32.Parse(Request.QueryString["CodAnexo"]);
                }
                if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
                {
                    accion = Request.QueryString["Accion"].ToString();

                };
                void_traerDatos(grupos, codigoContacto);
                pnl_crearEditar.Visible = false;
                void_ObtenerParametros();
                lbl_Titulo.Text= void_establecerTitulo(grupos, accion, "Texto");
            }
        }
        protected void gv_Anexos_DataBound(object sender, EventArgs e)
        {

        }
        protected void gv_Anexos_RowDataBound(object sender, GridViewRowEventArgs e)
        {}
        protected void gv_Anexos_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {

        }
        protected void lds_Anexos_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var anexos = (from tx in consultas.Db.Textos  
                          orderby  tx.NomTexto                           
                             select new
                             {
                                 Id_anexo = tx.Id_Texto,
                                 nomtexto = tx.NomTexto,                                 
                             });
            e.Arguments.TotalRowCount = anexos.Count();
            if (e.Arguments.TotalRowCount == 0)
            {
                Lbl_Resultados.Visible = true;
                Lbl_Resultados.Text = "No tiene actividades  para este rango de fechas";
            }
            else
            {
                Lbl_Resultados.Visible = false;
            }
            e.Result = anexos.ToList();
        }
        protected void btn_Inactivar_click(object sender, CommandEventArgs e)
        {}
        protected void btn_crearActualizar_onclick(object sender, EventArgs e)
        {
            int codigoContacto = 0;
            if (!String.IsNullOrEmpty(Request.QueryString["CodAnexo"]))
            {
                codigoContacto = Int32.Parse(Request.QueryString["CodAnexo"]);
            }
            string accion = Request.QueryString["Accion"].ToString();
            switch (accion)
            {
                case "Editar":
                    void_ModificarDatos(grupos, codigoContacto);
                    break;
                default: break;
            }
        }
        protected void gv_Anexos_RowCreated(object sender, GridViewRowEventArgs e)
        {
           
        }
    }
}