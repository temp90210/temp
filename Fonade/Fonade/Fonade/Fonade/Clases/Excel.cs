#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>08 - 07 - 2014</Fecha>
// <Archivo>Excel.cs</Archivo>

#endregion

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;

#endregion

namespace Fonade.Clases
{
    /// <summary>
    /// Diego Quiñonez
    /// 08 - 07 - 2014
    /// Clase que permite leer un Excel
    /// </summary>
    public class Excel
    {

        #region metodo general

        /// <summary>
        /// Diego Quiñonez
        /// 08 - 07 - 2014
        /// devuelve informacion de un excel dentro de un datatable
        /// </summary>
        /// <param name="archivo"></param>
        /// <param name="NomHoja"></param>
        /// <returns></returns>
        public static DataTable recogerExcel(string archivo, string NomHoja)
        {
            DataTable dt = new DataTable();//datatable que retorna con informacion del excel

            OleDbConnection conexion = null;//conexion con el paquete de offimatica
            OleDbDataAdapter dataAdapter = null;//comando de datos
            string consultaHojaExcel = "Select * from [" + NomHoja + "$]";//de donde obtiene datos
            string cadenaConexionArchivoExcel = "provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + archivo + "';Extended Properties=Excel 12.0;";//cadena que permite realizar una conexion con un libro excel

            try
            {
                conexion = new OleDbConnection(cadenaConexionArchivoExcel);//parametros de conexion
                conexion.Open();//abre la conexion
                dataAdapter = new OleDbDataAdapter(consultaHojaExcel, conexion);//realiza la consulta de datos

                dataAdapter.Fill(dt);//devuelve la infomacion y la almacena en un datatable
            }
            //excepciones de tipo conexion o de datos
            catch (OleDbException) { }
            //excepcion no controlada
            catch (Exception) { }
            finally
            {

                if (conexion != null)//si existio alguna conexion
                    conexion.Close();//finaliza la conexion
            }
            return dt;//devulta del tatable
        }

        #endregion

    }
}