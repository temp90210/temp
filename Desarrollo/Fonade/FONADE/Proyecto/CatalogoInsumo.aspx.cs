using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Fonade.Account;

namespace Fonade.FONADE.Convocatoria
{
    public partial class CatalogoInsumo : Negocio.Base_Page
    {
        public int proyecto;
        public int producto;
        public int insumo;
        public int carguetxt;
        public int tiempo_Proyeccion;

        protected void Page_Load(object sender, EventArgs e)
        {       
            proyecto = Convert.ToInt32(Session["CodProyecto"]);
            producto = Convert.ToInt32(Session["Id_Producto"]);
            insumo = Convert.ToInt32(Session["Insumo"]);
            if (!IsPostBack)
            {
                if (insumo == 0)
                {
                    btn_crear.Visible = true;
                }
                else
                {
                    btn_Actualizar.Visible = true;
                }
            }
        }

        protected void llenarTipoInsumo(DropDownList dll_lista)
        {
            var query = from c in consultas.Db.TipoInsumos
                        select new
                        {
                            idtipo = c.Id_TipoInsumo,
                            nomtipo = c.NomTipoInsumo,
                        };
            dll_lista.DataSource = query.ToList();
            dll_lista.DataTextField = "nomtipo";
            dll_lista.DataValueField = "idtipo";
            dll_lista.DataBind();
        }

        protected void lds_cargartxt_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {

                var query = from P in consultas.txt_insumo()
                            select P;
                e.Result = query;

            }
            catch (Exception)
            { }
        }

        protected void gvProyeccionVentas_Load(object sender, EventArgs e)
        {
            llenarTipoInsumo(ddl_insumotipos);
            var query = (from pmpv in consultas.Db.ProyectoMercadoProyeccionVentas
                             where pmpv.CodProyecto == proyecto
                             select new
                             {
                                 pmpv.CodPeriodo,
                                 pmpv.TiempoProyeccion,
                             }).FirstOrDefault();

                for (int i = 0; i < query.TiempoProyeccion; i++)
                {
                    TemplateField nuevaColumna = new TemplateField();
                    nuevaColumna.HeaderText = "Año " + (i + 1).ToString();
                    //nuevaColumna.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    //nuevaColumna.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    nuevaColumna.ItemTemplate = new labelasigado();
                    gvProyeccionVentas.Columns.Add(nuevaColumna);
                }
                tiempo_Proyeccion = Convert.ToInt32(query.TiempoProyeccion);
                llenarcolumnas();   
        }

        protected void llenarcolumnas()
        {
            for (int i = 1; i <= tiempo_Proyeccion; i++)
            {
                foreach (GridViewRow grd_Row in this.gvProyeccionVentas.Rows)
                {
                    try
                    {
                        string nombre2 = "txt_valor" + i.ToString();
                        TextBox tx1 = new TextBox();
                        tx1.ID = nombre2;
                        tx1.Text = "";
                        tx1.Width = 70;
                        grd_Row.Cells[i].Controls.Add(tx1);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
                
        protected void btn_iralista_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.opener.location.reload(); window.close();", true);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.close();", true);
        }

        protected void btn_crear_Click(object sender, EventArgs e)
        {
            var query = (from p in consultas.Db.ProyectoInsumos
                         where p.nomInsumo == txt_nombreinsumo.Text
                         && p.CodProyecto == proyecto
                         select new
                         {
                             p
                         }).Count();
            if (query == 1)
            {
                Alert1.Ver("Ya existe un Insumo con ese Nombre!", true);
                
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe un Insumo con ese Nombre!')", true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "location.reload();", true);
            }
            else 
            {
                //InsertUpdateDelete(
                //    0, proyecto, Convert.ToInt32(ddl_insumotipos.SelectedValue), txt_nombreinsumo.Text,
                //    Convert.ToInt32(txt_ivainsumo.Text), txt_unidadinsumo.Text, txt_presentacioninsumo.Text,
                //    Convert.ToInt32(txt_creditoinsumo.Text), "Create");
                insertInsumoPrecio();
            }

        }

        protected void btn_Actualizar_Click(object sender, EventArgs e)
        {

        }

        protected void InsertUpdateDelete(int c_insumo, int c_proyecto, int c_tipo, string nom_insumo, int iva, string unidad, string presentacion, int credito, string caso)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_Insert_Update_Delete_ProyectoInsumo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodInsumo", c_insumo);
            cmd.Parameters.AddWithValue("@codProyecto", c_proyecto);
            cmd.Parameters.AddWithValue("@Tipo", c_tipo);
            cmd.Parameters.AddWithValue("@NomInsumo", nom_insumo);
            cmd.Parameters.AddWithValue("@IVA", iva);
            cmd.Parameters.AddWithValue("@Unidad", unidad);
            cmd.Parameters.AddWithValue("@Presentacion", presentacion);
            cmd.Parameters.AddWithValue("@Credito", credito);
            cmd.Parameters.AddWithValue("@caso", caso);
            SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
            con.Open();
            cmd2.ExecuteNonQuery();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
            cmd2.Dispose();
            cmd.Dispose();
        }
        protected void insertInsumoPrecio()
        {
            try
            {
                string valores = "";
                foreach (GridViewRow grd_Row in this.gvProyeccionVentas.Rows)
                {
                    
                    for (int i = 1; i <= tiempo_Proyeccion; i++)
                    {
                        valores += ((TextBox)grd_Row.FindControl("txt_valor" + i.ToString())).Text + "//";
                    }
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('datos: "+valores+"!')", true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.opener.location.reload(); window.close();", true);
            }
            catch (Exception exc)
            {
                Alert1.Ver(exc.Message + "---" + exc.StackTrace, true);  
            }
            
        }

        protected void gvProyeccionVentas_Unload(object sender, EventArgs e)
        {
        }
    }

    class labelasigado : ITemplate
    {
        public void InstantiateIn(System.Web.UI.Control container)
        {
            //TextBox AAsignar = new TextBox();
            //AAsignar.ID = "l_asignar";
            //AAsignar.Width = 70;
            //container.Controls.Add(AAsignar);
        }
    }


}