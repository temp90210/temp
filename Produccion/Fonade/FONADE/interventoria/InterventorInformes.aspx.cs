using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class InterventorInformes : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (usuario.CodGrupo)
            {
                case Constantes.CONST_Interventor:
                case Constantes.CONST_CoordinadorInterventor:
                case Constantes.CONST_GerenteInterventor:
                    L_titulo.Text = "INFORMES DE INTERVENTORÍA";
                    break;
                default:
                    L_titulo.Text = "VER PLANES DE NEGOCIO";
                    break;
            }
        }
    }
}