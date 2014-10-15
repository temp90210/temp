#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Archivo>CargarContratos1.cs</Archivo>

#endregion

using Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Administracion
{
    public partial class CargarContratos1 : Negocio.Base_Page
    {
        string txtSQL;
        string error;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btncargar_Click(object sender, EventArgs e)
        {
            ClientScriptManager cm = this.ClientScript;

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

                    string nombreArchivo;
                    string extencionArchivo;

                    nombreArchivo = System.IO.Path.GetFileName(fld_cargar.PostedFile.FileName);
                    extencionArchivo = System.IO.Path.GetExtension(fld_cargar.PostedFile.FileName);

                    if (string.IsNullOrEmpty(nombreArchivo))
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Debe seleccionar un archivo!!!');</script>");
                        return;
                    }
                    if (!(extencionArchivo.Equals(".zip") || extencionArchivo.Equals(".ZIP")))
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El archivo seleccionado no es valido');</script>");
                        return;
                    }

                    string saveLocation = Server.MapPath("\\FonadeDocumentos\\Contratos") + "\\" + nombreArchivo;

                    if ((System.IO.File.Exists(saveLocation)))
                    {
                        System.IO.File.Delete(saveLocation);
                    }

                    if (!(File.Exists(saveLocation)))
                    {
                        fld_cargar.PostedFile.SaveAs(saveLocation);
                    }
                    else
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Ya se encuantra almacenado un archivo con este nombre');</script>");
                        return;
                    }

                    string destino = Server.MapPath("\\FonadeDocumentos\\Contratos") + "\\" + "ZIP" + usuario.IdContacto;

                    try
                    {
                        if (!(Directory.Exists(destino)))
                        {
                            Directory.CreateDirectory(destino);
                        }
                    }
                    catch (IOException)
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Error No se pudo crear la carpeta');</script>");
                        return;
                    }

                    try
                    {
                        DescomprimirFicheros(saveLocation, destino);
                    }
                    catch (ZipException)
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Error No se pudo descomprimir archivo');</script>");
                        return;
                    }

                    string[] files;

                    files = Directory.GetFiles(destino);

                    foreach (string file in files)
                    {
                        FileInfo arch = new FileInfo(file);
                        StreamReader objReader = new StreamReader(destino + arch.Name);
                        string sLine = "";
                        ArrayList arrText = new ArrayList();

                        while (sLine != null)
                        {
                            sLine = objReader.ReadLine();
                            if (sLine != null)
                            {
                                arrText.Add(sLine);
                                string[] atrib = sLine.Split('_');
                                try
                                {
                                    string CodProyecto = atrib[0];
                                    string CodContrato = atrib[1];
                                    string NombreArchivoContrato = atrib[2];

                                    txtSQL = "SELECT Id_Empresa FROM Empresa WHERE CodProyecto = " + CodProyecto;

                                    SqlDataReader reader = instruccion(txtSQL, 1);

                                    if (reader != null)
                                    {
                                        if (reader.Read())
                                        {
                                            string CodEmpresa = reader["Id_Empresa"].ToString();

                                            txtSQL = "UPDATE ContratoEmpresa SET NumeroContrato='" + CodContrato + "' WHERE CodEmpresa = " + CodEmpresa;
                                            instruccion(txtSQL, 2);
                                            txtSQL = "insert into ContratosArchivosAnexos values (" + CodProyecto + ",'" + destino + arch.Name + "','" + arch.Name + "')";
                                            instruccion(txtSQL, 2);
                                        }
                                    }
                                }
                                catch (IndexOutOfRangeException) { }
                            }
                        }
                        objReader.Close();
                    }

                    if ((File.Exists(saveLocation)))
                    {
                        File.Delete(saveLocation);
                    }

                    if ((Directory.Exists(destino)))
                    {
                        Directory.Delete(destino);
                    }

                    #endregion
                }
            }
        }

        public static void DescomprimirFicheros(string Zip, string RutaDestino)
        {
            using (ZipFile FicheroComprimido = ZipFile.Read(Zip))
            {
                FicheroComprimido.ExtractAll(RutaDestino);
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