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

namespace Fonade.FONADE.MiPerfil
{
    public partial class IngresarInformacionAcademica : Negocio.Base_Page
    {
        public int codigo;
        protected void Page_Load(object sender, EventArgs e)
        {
            codigo = Convert.ToInt32(Request.QueryString["LoadCode"]);
            if (!IsPostBack)
            {
                l_fechaActual.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
                //codigo = Convert.ToInt32(Request.QueryString["LoadCode"]);
                
                if (codigo == 0)
                {
                    llenarCiudad();
                    llenarNiveles();
                    LlenarInstituciones(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue));
                    lbl_Titulo.Text = void_establecerTitulo("ADICIONAR INFORMACIÓN ACADÉMICA");
                    mostrarPaneles(Convert.ToInt32(ddl_nivelEdu.SelectedValue), Convert.ToInt32(ddl_estadoEstudio.SelectedValue));
                    btn_CrearEstudio.Enabled = true;
                    btn_CrearEstudio.Visible = true;
                }
                else
                {
                    lbl_Titulo.Text = void_establecerTitulo("MODIFICAR INFORMACIÓN ACADÉMICA");
                    btn_modificarEstudio.Enabled = true;
                    btn_modificarEstudio.Visible = true;
                    var query = (from Estud in consultas.Db.ContactoEstudios
                                 where Estud.Id_ContactoEstudio == codigo
                                 select new
                                 {
                                     Estud
                                 }).FirstOrDefault();
                    llenarCiudad();
                    ddl_ciudad.SelectedValue = query.Estud.CodCiudad.ToString();
                    llenarNiveles();
                    ddl_nivelEdu.SelectedValue = query.Estud.CodNivelEstudio.ToString();
                    if (query.Estud.Finalizado == null)
                    {
                        ddl_estadoEstudio.SelectedValue = "0";
                    }
                    LlenarInstituciones(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue));
                    ddl_institucion.SelectedValue = query.Estud.Institucion;
                    llenarProgramas(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue), ddl_institucion.SelectedValue, tx_palabraClave.Text);
                    rbl_programasAcad.SelectedValue = query.Estud.CodProgramaAcademico.ToString();
                    mostrarPaneles(Convert.ToInt32(ddl_nivelEdu.SelectedValue), Convert.ToInt32(ddl_estadoEstudio.SelectedValue));
                    if (query.Estud.FechaInicio != null)
                    {
                        tx_frechaInicio.Text = Convert.ToDateTime(query.Estud.FechaInicio).ToString("dd/MM/yyyy");
                    }
                    if (query.Estud.FechaFinMaterias != null)
                    {
                        txt_finmaterias.Text = Convert.ToDateTime(query.Estud.FechaFinMaterias).ToString("dd/MM/yyyy");
                    }
                    if (query.Estud.FechaGrado != null)
                    {
                        txt_graduacion.Text = Convert.ToDateTime(query.Estud.FechaGrado).ToString("dd/MM/yyyy");
                    }
                    if (query.Estud.FechaGrado != null)
                    {
                        txt_fechafin2.Text = Convert.ToDateTime(query.Estud.FechaGrado).ToString("dd/MM/yyyy");
                    }
                    if (query.Estud.SemestresCursados != null)
                    {
                        txt_semestreactual1.Text = query.Estud.SemestresCursados.ToString();
                    }
                    if (query.Estud.FechaUltimoCorte != null)
                    {
                        txt_fechaCursando2.Text = Convert.ToDateTime(query.Estud.FechaUltimoCorte).ToString("dd/MM/yyyy");
                    }
                    if (query.Estud.SemestresCursados != null)
                    {
                        txt_semestreactual2.Text = query.Estud.SemestresCursados.ToString();
                    }
                }

                

            }
        }

        protected void llenarCiudad()
        {
            var query = (from Ciud in consultas.Db.Ciudads
                         from depto in consultas.Db.departamentos
                         where depto.Id_Departamento == Ciud.CodDepartamento
                        select new
                        {
                            Id_Ciud = Ciud.Id_Ciudad,
                            Nombre_Ciud = Ciud.NomCiudad + " (" + depto.NomDepartamento + ")",
                        }).OrderBy(x => x.Nombre_Ciud);
            ddl_ciudad.DataSource = query.ToList();
            ddl_ciudad.DataTextField = "Nombre_Ciud";
            ddl_ciudad.DataValueField = "Id_Ciud";
            ddl_ciudad.DataBind();
            
        }
        protected void llenarNiveles()
        {
            var query2 = from Nivel in consultas.Db.NivelEstudios
                         select new
                         {
                             Id_nivel = Nivel.Id_NivelEstudio,
                             Nombre_Nivel = Nivel.NomNivelEstudio,
                         };
            ddl_nivelEdu.DataSource = query2.ToList();
            ddl_nivelEdu.DataTextField = "Nombre_Nivel";
            ddl_nivelEdu.DataValueField = "Id_nivel";
            ddl_nivelEdu.DataBind();
        }

        protected void LlenarInstituciones(int ciudad, int nivel)
        {
            
            var query = from instit in consultas.Db.MD_MostrarInstituciones(ciudad, nivel)
                        select new
                        {
                            nombre_inst = instit.NomInstitucionEducativa,
                            //id_inst = instit.
                        };
            ddl_institucion.DataSource = query.ToList();
            ddl_institucion.DataTextField = "nombre_inst";
            ddl_institucion.DataValueField = "nombre_inst";
            ddl_institucion.DataBind();
             
        }

        protected void llenarProgramas(int ciudad, int nivel, string institucion, string programaT)
        {
            
            rbl_programasAcad.ClearSelection();
            rbl_programasAcad.Items.Clear();
            rbl_programasAcad.Dispose();
            
            var query = from Prog in consultas.Db.MD_MostrarProgramasAcademicos(ciudad, nivel, institucion, programaT)
                        select new
                        {
                            nombre_Prog = Prog.NomProgramaAcademico,
                            id_Prog = Prog.Id_ProgramaAcademico,
                        };
            rbl_programasAcad.DataSource = query.ToList();
            rbl_programasAcad.DataTextField = "nombre_Prog";
            rbl_programasAcad.DataValueField = "id_Prog";
            rbl_programasAcad.DataBind();
            
        }

        protected void ddl_ciudad_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenarInstituciones(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue));
            llenarProgramas(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue), ddl_institucion.SelectedValue, tx_palabraClave.Text);
        }

        protected void ddl_nivelEdu_SelectedIndexChanged(object sender, EventArgs e)
        {
            LlenarInstituciones(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue));
            llenarProgramas(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue), ddl_institucion.SelectedValue, tx_palabraClave.Text);
            mostrarPaneles(Convert.ToInt32(ddl_nivelEdu.SelectedValue), Convert.ToInt32(ddl_estadoEstudio.SelectedValue));
        }

        protected void btn_buscar_Click(object sender, ImageClickEventArgs e)
        {
            llenarProgramas(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue), ddl_institucion.SelectedValue, tx_palabraClave.Text);
        }

        protected void ddl_institucion_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarProgramas(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue), ddl_institucion.SelectedValue, tx_palabraClave.Text);
        }

        public void ddl_estadoEstudio_SelectedIndexChanged(object sender, EventArgs e)
        {
            mostrarPaneles(Convert.ToInt32(ddl_nivelEdu.SelectedValue), Convert.ToInt32(ddl_estadoEstudio.SelectedValue)); 
        }

        private void mostrarPaneles(int seleccionEstudios, int seleccionEstado)
        {
            switch (seleccionEstado)
            {
                case 1:

                    if (seleccionEstudios == 9 || seleccionEstudios == 10 || seleccionEstudios == 11 || seleccionEstudios == 12)
                    {
                        PanelFinalizado1.Visible = false;
                        PanelFinalizado2.Visible = true;
                        PanelCursando1.Visible = false;
                        PanelCursando2.Visible = false;
                    }
                    else
                    {
                        PanelFinalizado1.Visible = true;
                        PanelFinalizado2.Visible = false;
                        PanelCursando1.Visible = false;
                        PanelCursando2.Visible = false;
                    }

                    break;
                case 0:

                    if (seleccionEstudios == 11)
                    {
                        PanelFinalizado1.Visible = false;
                        PanelFinalizado2.Visible = false;
                        PanelCursando1.Visible = false;
                        PanelCursando2.Visible = true;
                    }
                    else 
                    {
                        PanelFinalizado1.Visible = false;
                        PanelFinalizado2.Visible = false;
                        PanelCursando1.Visible = true;
                        PanelCursando2.Visible = false;                
                    }

                    break;
            }  
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            pnlOcultar.Visible = true;
            pnlAlerta.Visible = true;
            l_ciudadP.Text = ddl_ciudad.SelectedItem.ToString();
            l_institucionP.Text = ddl_institucion.SelectedValue;
            l_NivelP.Text = ddl_nivelEdu.SelectedItem.ToString();
        }

        protected void btn_crearPrograma_Click(object sender, EventArgs e)
        {
            try
            {
                var query = (from instit in consultas.Db.MD_MostrarIdInstitucionAct(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue), ddl_institucion.SelectedValue)
                             select new
                             {
                                 nombre_inst = instit.Id_InstitucionEducativa,
                                 //id_inst = instit.
                             }).FirstOrDefault();
                int id_institucion = query.nombre_inst;

                var query2 = consultas.Db.ProgramaAcademicos.Max(x => x.Id_ProgramaAcademico);

                int codigo = query2 + 1;

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_InsertNuevoProgramaAcademico", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodProgramaAcademico", codigo);
                cmd.Parameters.AddWithValue("@nombreprograma", txt_ProgramaP.Text);
                cmd.Parameters.AddWithValue("@CodInstitucion", id_institucion);
                cmd.Parameters.AddWithValue("@CodNivelEstudio", Convert.ToInt32(ddl_nivelEdu.SelectedValue));
                cmd.Parameters.AddWithValue("@CiudadInstitucion", Convert.ToInt32(ddl_ciudad.SelectedValue));
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
                llenarProgramas(Convert.ToInt32(ddl_ciudad.SelectedValue), Convert.ToInt32(ddl_nivelEdu.SelectedValue), ddl_institucion.SelectedValue, tx_palabraClave.Text);
                rbl_programasAcad.SelectedValue = codigo.ToString();
                linkFInal.Focus();
                pnlOcultar.Visible = false;
                pnlAlerta.Visible = false;
            }
            catch (Exception)
            { }
        }

        protected void btn_cancelarPrograma_Click(object sender, EventArgs e)
        {
            pnlOcultar.Visible = false;
            pnlAlerta.Visible = false;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            pnlOcultar.Visible = true;
            pnlAlerta.Visible = true;
            l_ciudadP.Text = ddl_ciudad.SelectedItem.ToString();
            l_institucionP.Text = ddl_institucion.SelectedValue;
            l_NivelP.Text = ddl_nivelEdu.SelectedItem.ToString();
        }

        protected void btn_CrearEstudio_Click(object sender, EventArgs e)
        {
            validarInsertUpdate(Convert.ToInt32(ddl_nivelEdu.SelectedValue), Convert.ToInt32(ddl_estadoEstudio.SelectedValue), "Create");            
        }

        protected void btn_modificarEstudio_Click(object sender, EventArgs e)
        {
            validarInsertUpdate(Convert.ToInt32(ddl_nivelEdu.SelectedValue), Convert.ToInt32(ddl_estadoEstudio.SelectedValue), "Update"); 
        }

        private void validarInsertUpdate(int seleccionEstudios, int seleccionEstado, string caso)
        {
            
            switch (seleccionEstado)
            {
                case 1:

                    if (seleccionEstudios == 9 || seleccionEstudios == 10 || seleccionEstudios == 11 || seleccionEstudios == 12)
                    {
                        if (txt_fechafin2.Text == "" || tx_frechaInicio.Text=="")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: Algunos campos están vacíos!')", true);
                        }
                        else 
                        {
                            if (Convert.ToDateTime(txt_fechafin2.Text) >= DateTime.Now)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La fecha de graduación no puede ser mayor o igual a la actual!')", true);
                            }
                            else 
                            {
                                insertarOmodificar(codigo, caso, txt_fechafin2.Text, "");
                            }
                        }
                    }
                    else
                    {

                        if (txt_finmaterias.Text == "" || txt_graduacion.Text == "" || tx_frechaInicio.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: Algunos campos están vacíos!')", true);
                        }
                        else
                        {
                            if (Convert.ToDateTime(tx_frechaInicio.Text) >= Convert.ToDateTime(txt_finmaterias.Text))
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: La fecha de Inicio no puede ser mayor a la fecha de finalizacion de materias!')", true);
                                break;
                            }
                            if (Convert.ToDateTime(txt_finmaterias.Text) > Convert.ToDateTime(txt_graduacion.Text))
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: La fecha de finalizacion de materias no puede ser mayor a la fecha de Graduación!')", true);
                                break;
                            }
                            if (Convert.ToDateTime(txt_graduacion.Text) >= DateTime.Now)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: La fecha de Graduaciónn no puede ser mayor o igual al dia de hoy!')", true);
                                break;
                            }
                            else
                            {
                                insertarOmodificar(codigo, caso, txt_graduacion.Text, "");
                            }
                        }
                    }

                    break;
                case 0:

                    if (seleccionEstudios == 11)
                    {
                        if (tx_frechaInicio.Text == "" || txt_fechaCursando2.Text == "" || txt_semestreactual2.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: Algunos campos están vacíos!')", true);
                        }
                        else
                        {
                            if (Convert.ToDateTime(txt_fechaCursando2.Text) <= DateTime.Now || Convert.ToDateTime(tx_frechaInicio.Text) >= DateTime.Now)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: Algunas fechas no coinciden!')", true);
                            }
                            else 
                            {
                                insertarOmodificar(codigo, caso, "", txt_semestreactual2.Text);
                            }
                        }
                    }
                    else
                    {
                        if (tx_frechaInicio.Text == "" || txt_semestreactual1.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al validar fechas: Algunos campos están vacíos!')", true);
                        }
                        else
                        {
                            if (Convert.ToDateTime(tx_frechaInicio.Text) >= DateTime.Now)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error de validación: Algunas fechas no coinciden!')", true);
                            }
                            else
                            {
                                insertarOmodificar(codigo, caso, "", txt_semestreactual1.Text);
                            }
                        }
                    }

                    break;
            }
        }

        protected void insertarOmodificar(int IdInfoAcademica, string tipoCaso, string fechaGraduacion, string semestactualUHoras)
        {
            if (String.IsNullOrEmpty(rbl_programasAcad.SelectedValue))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Programa Realizado Es Requerido')", true);
                return;
            }

            if (String.IsNullOrEmpty(ddl_institucion.SelectedValue))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La Institucion Es Un Requerido')", true);
                return;
            }

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_Insert_Update_Estudios", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CODCONTACTO", usuario.IdContacto);
                cmd.Parameters.AddWithValue("@CODPROGRAMAACADEMICO", Convert.ToInt32(rbl_programasAcad.SelectedValue));
                cmd.Parameters.AddWithValue("@TITULOOBTENIDO", rbl_programasAcad.SelectedItem.ToString());
                if (fechaGraduacion == "")
                {
                    cmd.Parameters.AddWithValue("@ANOTITULO", null);
                    cmd.Parameters.AddWithValue("@FECHAGRADO", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ANOTITULO", Convert.ToDateTime(fechaGraduacion).Year);
                    cmd.Parameters.AddWithValue("@FECHAGRADO", Convert.ToDateTime(fechaGraduacion).ToString("yyyy-MM-dd"));
                }

                cmd.Parameters.AddWithValue("@INSTITUCION", ddl_institucion.SelectedValue);
                cmd.Parameters.AddWithValue("@CODCIUDAD", Convert.ToInt32(ddl_ciudad.SelectedValue));
                cmd.Parameters.AddWithValue("@CODNIVELESTUDIO", Convert.ToInt32(ddl_nivelEdu.SelectedValue));
                cmd.Parameters.AddWithValue("@FINALIZADO", Convert.ToInt32(ddl_estadoEstudio.SelectedValue));
                cmd.Parameters.AddWithValue("@FECHAINICIO", Convert.ToDateTime(tx_frechaInicio.Text).ToString("yyyy-MM-dd"));
                if (txt_finmaterias.Text == "")
                {
                    cmd.Parameters.AddWithValue("@FECHAFINMATERIA", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FECHAFINMATERIA", Convert.ToDateTime(txt_finmaterias.Text).ToString("yyyy-MM-dd"));
                }
                if (txt_fechaCursando2.Text == "")
                {
                    cmd.Parameters.AddWithValue("@FECHAULTIMOCORTE", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FECHAULTIMOCORTE", Convert.ToDateTime(txt_fechaCursando2.Text).ToString("yyyy-MM-dd"));
                }
                if (semestactualUHoras == "")
                {
                    cmd.Parameters.AddWithValue("@SEMESTRESCURSADOS", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SEMESTRESCURSADOS", Convert.ToInt32(semestactualUHoras));
                }

                cmd.Parameters.AddWithValue("@IDCONTACTOESTUDIO", IdInfoAcademica);
                cmd.Parameters.AddWithValue("@caso", tipoCaso);
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
                if (IdInfoAcademica == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información académica creada exitosamente!')", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información académica modificada exitosamente!')", true);
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.opener.location.reload(); window.close();", true);
                
            }
            catch (Exception)
            {}

            
        }

    }
}