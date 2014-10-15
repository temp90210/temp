using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoProducto : System.Web.UI.Page
    {
        public String codProyecto;
        public int txtTab = Constantes.CONST_ProyeccionesVentas;
        public String codConvocatoria;
        private ProyectoMercadoProyeccionVenta pm;

        EventArgs eee;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                codProyecto = Session["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
                codConvocatoria = Session["codConvocatoria"].ToString();

                codProyecto = Request.QueryString["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
            }
            catch (Exception) { }

            if (Request.QueryString["codConvocatoria"] != null)
                codConvocatoria = Request.QueryString["codConvocatoria"].ToString();

            try
            {
                TB_PosicionArancelariacodigo.Text = Session["txtcodigo"].ToString();
                TB_PosicionArancelariadescripcion.Text = Session["desccripcion"].ToString();
            }
            catch (Exception) { }

            try
            {
                if ("agregar".Equals(Session["OpcionMercadoProyecciones"].ToString()))
                {
                    Session["OpcionMercadoProyecciones"] = "";
                    B_Crear.Text = "Crear";
                }
                if ("actualizar".Equals(Session["OpcionMercadoProyecciones"].ToString()))
                {
                    Session["OpcionMercadoProyecciones"] = "";
                    B_Crear.Text = "Actualizar";
                    llenarCampos();
                    cacillas();
                    precios();
                }
                
            }
            catch (Exception) { }
        }

        private void precios()
        {
            String sql;
            sql = "SELECT * FROM [Fonade].[dbo].[ProyectoProductoPrecio] WHERE [CodProducto] = " + Session["valordeId_Producto"].ToString();

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int cont = 1;
                while (reader.Read())
                {
                    String objeto = "L_PrecioAnio"  + cont;
                    cont++;
                    TextBox textbox;
                    textbox = (TextBox)this.FindControl(objeto);
                    textbox.Text = reader["Precio"].ToString();
                }
            }
            catch (SqlException) { }
            finally
            {
                conn.Close();
            }

            object sender = new object();

            camino1(sender, eee);
            camino2(sender, eee);
            camino3(sender, eee);
        }

        private void cacillas()
        {
            String sql;
            sql = "SELECT * FROM [Fonade].[dbo].[ProyectoProductoUnidadesVentas] WHERE [CodProducto] = " + Session["valordeId_Producto"].ToString();

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (Int32.Parse(reader["Ano"].ToString()) < 4 && Int32.Parse(reader["Mes"].ToString()) < 13)
                    {
                        String objeto = "CantAnio" + reader["Ano"].ToString() + "Mes" + reader["Mes"].ToString();
                        TextBox textbox;
                        textbox = (TextBox)this.FindControl(objeto);
                        textbox.Text = reader["Unidades"].ToString();
                    }
                }
            }
            catch (SqlException) { }
            finally
            {
                conn.Close();
            }
        }

        private void llenarCampos()
        {
            String sql;
            sql = "SELECT P.*, A.[Descripcion] FROM [Fonade].[dbo].[ProyectoProducto] AS P, [Fonade].[dbo].[PosicionArancelaria] AS A  WHERE [Id_Producto] = " + Session["valordeId_Producto"].ToString() + " AND A.[PosicionArancelaria] = P.[PosicionArancelaria]";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                TB_NombreProductoServicio.Text = reader["NomProducto"].ToString();
                TB_PosicionArancelariacodigo.Text = reader["PosicionArancelaria"].ToString();
                TB_PosicionArancelariadescripcion.Text = reader["Descripcion"].ToString();
                TB_PrecioLanzamiento.Text = reader["PrecioLanzamiento"].ToString();
                TB_IVA.Text = reader["PorcentajeIva"].ToString();
                TB_Retencionfuente.Text = reader["PorcentajeRetencion"].ToString();
                TB_VentasCredito.Text = reader["PorcentajeVentasPlazo"].ToString();
            }
            catch (SqlException) { }
            finally
            {
                conn.Close();
            }
        }

        private void insertarproyectoProducto(Int32 codigo, String nombreproducto, String iva, String retencioFuente, String VentaCredito, String posicionArancelarCodigo, Double precioLanzamiento, String accion)
        {
            string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            using (var con = new SqlConnection(conexionStr))
            {
                using (var com = con.CreateCommand())
                {
                    com.CommandText = "MD_Insertar_Actualizar_ProyectoProducto";
                    com.CommandType = System.Data.CommandType.StoredProcedure;

                    com.Parameters.AddWithValue("@_Id_Producto", codigo);

                    if (accion == "CREATE") com.Parameters.AddWithValue("@_CodProyecto", codProyecto);
                    else com.Parameters.AddWithValue("@_CodProyecto", 0);

                    com.Parameters.AddWithValue("@_NomProducto", nombreproducto);
                    com.Parameters.AddWithValue("@_PorcentajeIva", iva);
                    com.Parameters.AddWithValue("@_PorcentajeRetencion", retencioFuente);
                    com.Parameters.AddWithValue("@_PorcentajeVentasContado", (100 - Int32.Parse(VentaCredito)));
                    com.Parameters.AddWithValue("@_PorcentajeVentasPlazo", VentaCredito);
                    com.Parameters.AddWithValue("@_PosicionArancelaria", posicionArancelarCodigo);
                    com.Parameters.AddWithValue("@_PrecioLanzamiento", precioLanzamiento);
                    if (accion == "CREATE")
                        com.Parameters.AddWithValue("@_caso", "CREATE");
                    if (accion == "UPDATE")
                        com.Parameters.AddWithValue("@_caso", "UPDATE");
                    try
                    {
                        con.Open();
                        com.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        private void insertarProyectoProductoPrecio(Int32 Id_Producto, String accion)
        {
            string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            using (var con = new SqlConnection(conexionStr))
            {
                using (var com = con.CreateCommand())
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        com.CommandText = "MD_Insertar_Actualizar_ProyectoProductoPrecio";
                        com.CommandType = System.Data.CommandType.StoredProcedure;

                        String objeto = "L_PrecioAnio" + i;
                        TextBox textbox;
                        textbox = (TextBox)this.FindControl(objeto);

                        com.Parameters.AddWithValue("@_CodProducto", Id_Producto);
                        com.Parameters.AddWithValue("@_Periodo", i);
                        com.Parameters.AddWithValue("@_Precio", textbox.Text);
                        if (accion == "CREATE")
                            com.Parameters.AddWithValue("@_caso", "CREATE");
                        if (accion == "UPDATE")
                            com.Parameters.AddWithValue("@_caso", "UPDATE");

                        try
                        {
                            con.Open();
                            com.ExecuteReader();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
        }

        private void insertarproyectoProductoUnidades(Int32 Id_Producto, String accion)
        {
            string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            using (var con = new SqlConnection(conexionStr))
            {
                using (var com = con.CreateCommand())
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        for (int j = 1; j <= 12; j++)
                        {
                            com.CommandText = "MD_Insertar_Actualizar_ProyectoProductoUnidadesVentas";
                            com.CommandType = System.Data.CommandType.StoredProcedure;

                            String objeto = "CantAnio" + i + "Mes" + j;
                            TextBox textbox;
                            textbox = (TextBox)this.FindControl(objeto);

                            com.Parameters.AddWithValue("@_CodProducto", Id_Producto);
                            com.Parameters.AddWithValue("@_Unidades", textbox.Text);
                            com.Parameters.AddWithValue("@_Mes", j);
                            com.Parameters.AddWithValue("@_Ano", i);
                            if (accion == "CREATE")
                                com.Parameters.AddWithValue("@_caso", "CREATE");
                            if (accion == "UPDATE")
                                com.Parameters.AddWithValue("@_caso", "UPDATE");

                            try
                            {
                                con.Open();
                                com.ExecuteReader();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }
        }

        private void CrearProductoServicio()
        {
            ClientScriptManager cm = this.ClientScript;

            String nombreproducto = TB_NombreProductoServicio.Text;
            String posicionArancelarCodigo = TB_PosicionArancelariacodigo.Text;
            String posicionArancelarDescripcion = TB_PosicionArancelariadescripcion.Text;
            //String precioLanzamiento = TB_PrecioLanzamiento.Text;
            String iva = TB_IVA.Text;
            String retencioFuente = TB_Retencionfuente.Text;
            String VentaCredito = TB_VentasCredito.Text;

            Double precioLanzamiento;

            try
            {
                precioLanzamiento = Double.Parse(TB_PrecioLanzamiento.Text);
            }
            catch (FormatException)
            {

                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Precio de lanzamiento incorrecto');</script>");
                return;
            }

            String sql;
            sql = "SELECT COUNT([Id_Producto]) AS VER FROM [Fonade].[dbo].[ProyectoProducto] WHERE  WHERE [CodProyecto] = " + codProyecto + " AND [NomProducto] = '" + nombreproducto + "'";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                Int32 suma = Int32.Parse(reader["VER"].ToString());

                if (suma > 0)
                {
                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Ya existe un Producto con ese Nombre.');</script>");
                    return;
                }
                else
                {
                    conn.Close();

                    insertarproyectoProducto(0, nombreproducto, iva, retencioFuente, VentaCredito, posicionArancelarCodigo, precioLanzamiento, "CREATE");

                    sql = "SELECT [Id_Producto] FROM [Fonade].[dbo].[ProyectoProducto] WHERE [NomProducto] = '" + nombreproducto + "' AND [CodProyecto] = " + codProyecto;
                    cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    Int32 Id_Producto = Int32.Parse(reader["Id_Producto"].ToString());
                    conn.Close();

                    insertarproyectoProductoUnidades(Id_Producto, "CREATE");
                    insertarProyectoProductoPrecio(Id_Producto, "CREATE");
                }
            }
            catch (SqlException) { }
            finally
            {
                conn.Close();
            }

            Response.Redirect("~/FONADE/Proyecto/PProyectoMercadoProyecciones.aspx");
        }

        private void ActualizarProductoServicio()
        {
            ClientScriptManager cm = this.ClientScript;

            String nombreproducto = TB_NombreProductoServicio.Text;
            String posicionArancelarCodigo = TB_PosicionArancelariacodigo.Text;
            String posicionArancelarDescripcion = TB_PosicionArancelariadescripcion.Text;
            //String precioLanzamiento = TB_PrecioLanzamiento.Text;
            String iva = TB_IVA.Text;
            String retencioFuente = TB_Retencionfuente.Text;
            String VentaCredito = TB_VentasCredito.Text;

            Double precioLanzamiento;

            try
            {
                precioLanzamiento = Double.Parse(TB_PrecioLanzamiento.Text);
            }
            catch (FormatException)
            {

                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Precio de lanzamiento incorrecto');</script>");
                return;
            }

            insertarproyectoProducto(Int32.Parse(Session["valordeId_Producto"].ToString()), nombreproducto, iva, retencioFuente, VentaCredito, posicionArancelarCodigo, precioLanzamiento, "UPDATE");
            insertarproyectoProductoUnidades(Int32.Parse(Session["valordeId_Producto"].ToString()), "UPDATE");
            insertarProyectoProductoPrecio(Int32.Parse(Session["valordeId_Producto"].ToString()), "UPDATE");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            if ("Crear".Equals(B_Crear.Text)) CrearProductoServicio();
            if ("Actualizar".Equals(B_Crear.Text)) ActualizarProductoServicio();
            
        }

        private void suma1(String valor, int anio)
        {
            try
            {
                if (!String.IsNullOrEmpty(valor))
                {
                    if (anio == 1)
                    {
                        Double cantidadSumada = Double.Parse(L_totalVentas1.Text);
                        L_totalVentas1.Text = "" + (cantidadSumada + (Double.Parse(valor) * Double.Parse(L_PrecioAnio1.Text)));
                    }
                    if (anio == 2)
                    {
                        Double cantidadSumada = Double.Parse(L_totalVentas2.Text);
                        L_totalVentas2.Text = "" + (cantidadSumada + (Double.Parse(valor) * Double.Parse(L_PrecioAnio2.Text)));
                    }
                    if (anio == 3)
                    {
                        Double cantidadSumada = Double.Parse(L_totalVentas3.Text);
                        L_totalVentas3.Text = "" + (cantidadSumada + (Double.Parse(valor) * Double.Parse(L_PrecioAnio3.Text)));
                    }
                }
                L_totalVentas3.DataBind();
            }
            catch (FormatException)
            {

            }
            catch (Exception)
            {
                try
                {
                    if (anio == 1) L_totalVentas1.Text = "" + (Double.Parse(valor) * Double.Parse(L_PrecioAnio1.Text));
                    if (anio == 2) L_totalVentas1.Text = "" + (Double.Parse(valor) * Double.Parse(L_PrecioAnio2.Text));
                    if (anio == 3) L_totalVentas1.Text = "" + (Double.Parse(valor) * Double.Parse(L_PrecioAnio3.Text));
                }
                catch (FormatException) { }
            }
            finally
            {
                L_totalVentas1.DataBind();
                L_totalVentas2.DataBind();
                L_totalVentas3.DataBind();
            }
        }

        protected void CantAnio1Mes1_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes1.Text, 1);
        }

        protected void CantAnio1Mes2_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes2.Text, 1);
        }

        protected void CantAnio1Mes3_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes3.Text, 1);
        }

        protected void CantAnio1Mes4_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes4.Text, 1);
        }

        protected void CantAnio1Mes5_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes5.Text, 1);
        }

        protected void CantAnio1Mes6_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes6.Text, 1);
        }

        protected void CantAnio1Mes7_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes7.Text, 1);
        }

        protected void CantAnio1Mes8_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes8.Text, 1);
        }

        protected void CantAnio1Mes9_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes9.Text, 1);
        }

        protected void CantAnio1Mes10_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes10.Text, 1);
        }

        protected void CantAnio1Mes11_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes11.Text, 1);
        }

        protected void CantAnio1Mes12_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio1Mes12.Text, 1);
        }

        protected void CantAnio2Mes1_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes1.Text, 2);
        }

        protected void CantAnio2Mes2_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes2.Text, 2);
        }

        protected void CantAnio2Mes3_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes3.Text, 2);
        }

        protected void CantAnio2Mes4_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes4.Text, 2);
        }

        protected void CantAnio2Mes5_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes5.Text, 2);
        }

        protected void CantAnio2Mes6_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes6.Text, 2);
        }

        protected void CantAnio2Mes7_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes7.Text, 2);
        }

        protected void CantAnio2Mes8_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes8.Text, 2);
        }

        protected void CantAnio2Mes9_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes9.Text, 2);
        }

        protected void CantAnio2Mes10_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes10.Text, 2);
        }

        protected void CantAnio2Mes11_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes11.Text, 2);
        }

        protected void CantAnio2Mes12_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio2Mes12.Text, 2);
        }

        protected void CantAnio3Mes1_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes1.Text, 3);
        }

        protected void CantAnio3Mes2_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes2.Text, 3);
        }

        protected void CantAnio3Mes3_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes3.Text, 3);
        }

        protected void CantAnio3Mes4_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes4.Text, 3);
        }

        protected void CantAnio3Mes5_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes5.Text, 3);
        }

        protected void CantAnio3Mes6_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes6.Text, 3);
        }

        protected void CantAnio3Mes7_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes7.Text, 3);
        }

        protected void CantAnio3Mes8_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes8.Text, 3);
        }

        protected void CantAnio3Mes9_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes9.Text, 3);
        }

        protected void CantAnio3Mes10_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes10.Text, 3);
        }

        protected void CantAnio3Mes11_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes11.Text, 3);
        }

        protected void CantAnio3Mes12_TextChanged(object sender, EventArgs e)
        {
            suma1(CantAnio3Mes12.Text, 3);
        }

        private void camino1(object sender, EventArgs e)
        {
            L_totalVentas1.Text = "0.0";
            CantAnio1Mes1_TextChanged(sender, e);
            CantAnio1Mes2_TextChanged(sender, e);
            CantAnio1Mes3_TextChanged(sender, e);
            CantAnio1Mes4_TextChanged(sender, e);
            CantAnio1Mes5_TextChanged(sender, e);
            CantAnio1Mes6_TextChanged(sender, e);
            CantAnio1Mes7_TextChanged(sender, e);
            CantAnio1Mes8_TextChanged(sender, e);
            CantAnio1Mes9_TextChanged(sender, e);
            CantAnio1Mes10_TextChanged(sender, e);
            CantAnio1Mes11_TextChanged(sender, e);
            CantAnio1Mes12_TextChanged(sender, e);
        }

        private void camino2(object sender, EventArgs e)
        {
            L_totalVentas2.Text = "0.0";
            CantAnio2Mes1_TextChanged(sender, e);
            CantAnio2Mes2_TextChanged(sender, e);
            CantAnio2Mes3_TextChanged(sender, e);
            CantAnio2Mes4_TextChanged(sender, e);
            CantAnio2Mes5_TextChanged(sender, e);
            CantAnio2Mes6_TextChanged(sender, e);
            CantAnio2Mes7_TextChanged(sender, e);
            CantAnio2Mes8_TextChanged(sender, e);
            CantAnio2Mes9_TextChanged(sender, e);
            CantAnio2Mes10_TextChanged(sender, e);
            CantAnio2Mes11_TextChanged(sender, e);
            CantAnio2Mes12_TextChanged(sender, e);
        }

        private void camino3(object sender, EventArgs e)
        {
            L_totalVentas3.Text = "0.0";
            CantAnio3Mes1_TextChanged(sender, e);
            CantAnio3Mes2_TextChanged(sender, e);
            CantAnio3Mes3_TextChanged(sender, e);
            CantAnio3Mes4_TextChanged(sender, e);
            CantAnio3Mes5_TextChanged(sender, e);
            CantAnio3Mes6_TextChanged(sender, e);
            CantAnio3Mes7_TextChanged(sender, e);
            CantAnio3Mes8_TextChanged(sender, e);
            CantAnio3Mes9_TextChanged(sender, e);
            CantAnio3Mes10_TextChanged(sender, e);
            CantAnio3Mes11_TextChanged(sender, e);
            CantAnio3Mes12_TextChanged(sender, e);
        }

        protected void L_PrecioAnio1_TextChanged(object sender, EventArgs e)
        {
            camino1(sender, e);
        }

        protected void L_PrecioAnio2_TextChanged(object sender, EventArgs e)
        {
            camino2(sender, e);
        }

        protected void L_PrecioAnio3_TextChanged(object sender, EventArgs e)
        {
            camino3(sender, e);
        }

        protected void LB_Buscar_Click(object sender, EventArgs e)
        {

        }
    }
}