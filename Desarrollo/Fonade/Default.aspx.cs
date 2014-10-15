using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace Fonade
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("/FONADE/MiPerfil/Home.aspx");
        }

        [WebMethod]
        public static string textoAyuda(string texto)
        {
            Datos.FonadeDBDataContext db = new Datos.FonadeDBDataContext();
            var sql_texto = (from txt in db.Textos
                            where txt.NomTexto.ToLower() == texto.ToLower()
                            select txt).FirstOrDefault();
            string response = (sql_texto != null) ? sql_texto.Texto1 : "";
            return response;
        }
    }
}
