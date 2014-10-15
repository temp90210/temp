#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Archivo>CarguesContratos.cs</Archivo>

#endregion

using Fonade.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Administracion
{
    public partial class CarguesContratos : Negocio.Base_Page
    {
        string txtSQL;
        string error;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btncargar_Click(object sender, EventArgs e)
        {
            ClientScriptManager cm = this.ClientScript;

            string nombreArchivo;
            string extencionArchivo;

            if (!fld_cargar.HasFile)
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('No ha subido ningun archivo');</script>");
                return;
            }
            else
            {
                if (fld_cargar.PostedFile.ContentLength > 10485760)
                {
                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El tamaño del archivo debe ser menor a 10 Mb.');</script>");
                    return;
                }
                else
                {
                    #region Iniciar con el procesamiento del archivo.

                    nombreArchivo = System.IO.Path.GetFileName(fld_cargar.PostedFile.FileName);
                    extencionArchivo = System.IO.Path.GetExtension(fld_cargar.PostedFile.FileName);

                    if (string.IsNullOrEmpty(nombreArchivo))
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('No ha subido ningun archivo');</script>");
                        return;
                    }
                    if (!(extencionArchivo.Equals(".xls") || extencionArchivo.Equals(".XLS") || extencionArchivo.Equals(".xlsx") || extencionArchivo.Equals(".XLSX")))
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Tipo De Archivo No Valido');</script>");
                        return;
                    }

                    string saveLocation = Server.MapPath("\\FonadeDocumentos\\CargueMasivo") + "\\" + nombreArchivo;

                    if ((System.IO.File.Exists(saveLocation)))
                    {
                        System.IO.File.Delete(saveLocation);
                    }

                    if (!(System.IO.File.Exists(saveLocation)))
                    {
                        fld_cargar.PostedFile.SaveAs(saveLocation);
                    }
                    else
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Ya se encuantra almacenado un archivo con este nombre');</script>");
                        return;
                    }

                    DataTable dt = Excel.recogerExcel(saveLocation, "cargue");

                    if (dt.Rows.Count > 0)
                    {
                        error = string.Empty;

                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr[0].Equals("1"))
                            {
                                txtSQL = "SELECT id_empresa FROM empresa where codproyecto=" + dr[0];

                                SqlDataReader reader = instruccion(txtSQL, 1);

                                if (reader != null)
                                {
                                    if (reader.Read())
                                    {
                                        string idempresa = reader[0].ToString();

                                        txtSQL = "select * from contratoempresa where codempresa=" + reader[0].ToString();
                                        reader = instruccion(txtSQL, 1);

                                        if (reader.Read())
                                        {
                                            txtSQL = "Update contratoempresa set Numerocontrato=" + dr[2].ToString();
                                            txtSQL = txtSQL + ", ObjetoContrato='" + dr[3].ToString() + "'";
                                            txtSQL = txtSQL + ", ValorInicialEnPesos=" + dr[4].ToString();
                                            txtSQL = txtSQL + ", PlazoContratoMeses=" + dr[5].ToString();
                                            txtSQL = txtSQL + ", NumeroAPContrato=" + dr[6].ToString();
                                            txtSQL = txtSQL + ", FechaAP='" + dr[7].ToString() + "'";
                                            txtSQL = txtSQL + ", FechaFirmaDelContrato='" + dr[8].ToString() + "'";
                                            txtSQL = txtSQL + ", NumeroPoliza='" + dr[9].ToString() + "'";
                                            txtSQL = txtSQL + ", CompaniaSeguros='" + dr[10].ToString() + "'";
                                            txtSQL = txtSQL + ", FechaDeInicioContrato='" + dr[11].ToString() + "'";
                                            txtSQL = txtSQL + " where codempresa=" + idempresa;

                                            instruccion(txtSQL, 2);
                                        }
                                        else
                                        {
                                            txtSQL = "insert into contratoempresa (codempresa,Numerocontrato,ObjetoContrato,ValorInicialEnPesos,PlazoContratoMeses,";
                                            txtSQL = txtSQL + " NumeroAPContrato,FechaAP,FechaFirmaDelContrato,NumeroPoliza,CompaniaSeguros,FechaDeInicioContrato) values";
                                            txtSQL = txtSQL + "(" + idempresa + "," + dr[2].ToString() + ",'" + dr[3].ToString() + "',";

                                            if (string.IsNullOrEmpty(dr[4].ToString()))
                                                txtSQL = txtSQL + "null";
                                            else
                                                txtSQL = txtSQL + dr[4].ToString();

                                            if (string.IsNullOrEmpty(dr[5].ToString()))
                                                txtSQL = txtSQL + ",null";
                                            else
                                                txtSQL = txtSQL + dr[5].ToString();

                                            if (string.IsNullOrEmpty(dr[6].ToString()))
                                                txtSQL = txtSQL + ",null";
                                            else
                                                txtSQL = txtSQL + dr[6].ToString();

                                            if (string.IsNullOrEmpty(dr[7].ToString()))
                                                txtSQL = txtSQL + ",null";
                                            else
                                                txtSQL = txtSQL + dr[7].ToString();

                                            if (string.IsNullOrEmpty(dr[8].ToString()))
                                                txtSQL = txtSQL + ",null";
                                            else
                                                txtSQL = txtSQL + dr[8].ToString();

                                            if (string.IsNullOrEmpty(dr[9].ToString()))
                                                txtSQL = txtSQL + ",null";
                                            else
                                                txtSQL = txtSQL + dr[9].ToString();

                                            if (string.IsNullOrEmpty(dr[10].ToString()))
                                                txtSQL = txtSQL + ",null";
                                            else
                                                txtSQL = txtSQL + dr[10].ToString();

                                            if (string.IsNullOrEmpty(dr[11].ToString()))
                                                txtSQL = txtSQL + ",null";
                                            else
                                                txtSQL = txtSQL + dr[11].ToString();

                                            txtSQL = txtSQL + ")";

                                            instruccion(txtSQL, 2);
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(error))
                                            error = dr[0].ToString() + " - " + dr[1].ToString();
                                        else
                                            error += dr[0].ToString() + " - " + dr[1].ToString();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('el archivo de excel no posee la informacion de la manera correcta');</script>");
                        return;
                    }

                    if ((System.IO.File.Exists(saveLocation)))
                    {
                        System.IO.File.Delete(saveLocation);
                    }

                    if (string.IsNullOrEmpty(error))
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El archivo ha sido cargado Satisfactoriamente');</script>");
                        return;
                    }
                    else
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Los Siguientes Contratos: \n" + error + " no han sido cargado debido a no estar asociados a una empresa. Por favor Verificar');</script>");
                        return;
                    }

                    #endregion
                }
            }
        }

        public SqlDataReader instruccion(String sql, int obj)
        {
            SqlDataReader reader = null;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                        reader.Close();
                }

                if (conn != null)
                    conn.Close();

                conn.Open();

                if (obj == 1)
                    reader = cmd.ExecuteReader();
                else
                    cmd.ExecuteReader();
            }
            catch (SqlException se) { }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

            return reader;
        }
    }
}