using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fonade.Account;
using Datos;
using System.Web.Security;

namespace Fonade.Negocio
{
    public class Base_Master : System.Web.UI.MasterPage
    {
        protected FonadeUser usuario;
        protected Consultas consultas;

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                usuario = (FonadeUser)Membership.GetUser(HttpContext.Current.User.Identity.Name, true);
                consultas = new Consultas();
            }
            catch
            {
                throw new Exception("the user session doesnt exist");
            };
            base.OnLoad(e);
        }
    }
}