#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>08 - 07 - 2014</Fecha>
// <Archivo>InfoAsesor.cs</Archivo>

#endregion

#region using 

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace Fonade.FONADE.Administracion
{
    public partial class InfoAsesor : Negocio.Base_Page
    {
        #region variables globales

        string ID_PROYECTOAcreditar;
        string CODCONVOCATORIAAcreditar;
        string mTipoConsulta;

        //vaiable que contiene las sentencias SQL que se ejecutan en la BD
        string txtSQL;

        #endregion

        /// <summary>
        /// Diego Quiñonez
        /// metodo de carga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlDataReader reader;//lectura de la BD

            //recoge los datos de session utilizados para la consulta
            ID_PROYECTOAcreditar = Session["ID_PROYECTOAcreditar"] != null && !string.IsNullOrEmpty(Session["ID_PROYECTOAcreditar"].ToString()) ? Session["ID_PROYECTOAcreditar"].ToString() : "0";
            CODCONVOCATORIAAcreditar = Session["CODCONVOCATORIAAcreditar"] != null && !string.IsNullOrEmpty(Session["CODCONVOCATORIAAcreditar"].ToString()) ? Session["CODCONVOCATORIAAcreditar"].ToString() : "0";
            mTipoConsulta = Session["mTipoConsulta"] != null && !string.IsNullOrEmpty(Session["mTipoConsulta"].ToString()) ? Session["mTipoConsulta"].ToString() : "0";

            //valida que tipo de consulta realiza de acuerdo al rol del usuario
            switch (mTipoConsulta)
            {
                case "1":
                    txtSQL = "SELECT DISTINCT (C.NOMBRES + ' ' + C.APELLIDOS) 'NOMBRE',C.EMAIL, C.TELEFONO  FROM CONTACTO C JOIN PROYECTOCONTACTO PC ON (PC.CODROL=1 AND PC.INACTIVO=0 AND PC.CODCONTACTO=C.ID_CONTACTO) WHERE PC.CODPROYECTO =" + ID_PROYECTOAcreditar;
                    break;
                case "2":
                    txtSQL = "SELECT DISTINCT (C.NOMBRES + ' ' + C.APELLIDOS) 'NOMBRE',C.EMAIL, C.TELEFONO  FROM CONTACTO C JOIN PROYECTOCONTACTO PC ON (PC.CODROL=2 AND PC.INACTIVO=0 AND PC.CODCONTACTO=C.ID_CONTACTO AND (PC.ACREDITADOR IS NULL OR PC.ACREDITADOR=0  )) WHERE PC.CODPROYECTO =" + ID_PROYECTOAcreditar;
                    break;
            }

            if (!string.IsNullOrEmpty(txtSQL))
            {
                //se trae la informacion
                reader = ejecutaReader(txtSQL, 1);

                //si trae la informacion correcta
                //la muestra en los label
                if (reader.Read())
                {
                    lblnombre.Text = reader["Nombre"].ToString();
                    lblcorreo.Text = reader["Email"].ToString();
                    lblnumero.Text = reader["Telefono"].ToString();
                }
            }
        }
    }
}