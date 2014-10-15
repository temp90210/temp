using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Convocatoria
{
    public partial class FrameAspectos : Negocio.Base_Page
    {
        string Id_EvalConvocatoriaAS;
        string txtSQL;

        string codPadre;
        string codCampo;

        delegate string del(bool x);

        protected void Page_Load(object sender, EventArgs e)
        {
            Id_EvalConvocatoriaAS = Session["Id_EvalConvocatoriaAS"] != null && !string.IsNullOrEmpty(Session["Id_EvalConvocatoriaAS"].ToString()) ? Session["Id_EvalConvocatoriaAS"].ToString() : "0";
            codCampo = Session["codCampoAspecto"] != null && !string.IsNullOrEmpty(Session["codCampoAspecto"].ToString()) ? Session["codCampoAspecto"].ToString() : "0";

            if (!IsPostBack)
            {
                tv_aspectos.Nodes.Clear();
                pnlAgregarCampo.Visible = false;
                pnlinfonodo.Visible = false;
                crearLista();
            }
        }

        private void crearLista()
        {
            txtSQL = "SELECT id_campo, campo, codConvocatoria FROM Campo C LEFT JOIN convocatoriaCampo CC ON C.id_campo = CC.codcampo and codConvocatoria = " + Id_EvalConvocatoriaAS + " WHERE c.codcampo is null ";

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            List<Lista1> lista1 = new List<Lista1>();

            foreach (DataRow dr in dt.Rows)
            {
                lista1.Add(new Lista1(){
                    id_campo=Convert.ToInt32(dr["id_campo"].ToString()),
                    campo=dr["campo"].ToString(),
                    codConvocatoria=!string.IsNullOrEmpty(dr["codConvocatoria"].ToString()) ? Convert.ToInt32(dr["codConvocatoria"].ToString()) : 0,
                    nivel = 0
                });

                txtSQL = "SELECT id_campo, campo FROM Campo WHERE codCampo = " + dr["id_campo"].ToString();

                var dtH1 = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow drH1 in dtH1.Rows)
                {
                    lista1.Add(new Lista1()
                    {
                        id_campo = Convert.ToInt32(drH1["id_campo"].ToString()),
                        campo = drH1["campo"].ToString(),
                        nivel = 1
                    });

                    txtSQL = "SELECT id_campo, campo, codConvocatoria " +
                        "FROM Campo C LEFT JOIN convocatoriaCampo CC ON C.id_campo = CC.codcampo and codConvocatoria = " + Id_EvalConvocatoriaAS + " " +
                        "WHERE C.codCampo = " + drH1["id_campo"].ToString();

                    var dtH2 = consultas.ObtenerDataTable(txtSQL, "text");

                    foreach (DataRow drH2 in dtH2.Rows)
                    {
                        lista1.Add(new Lista1()
                        {
                            id_campo = Convert.ToInt32(drH2["id_campo"].ToString()),
                            campo = drH2["campo"].ToString(),
                            codConvocatoria = !string.IsNullOrEmpty(dr["codConvocatoria"].ToString()) ? Convert.ToInt32(dr["codConvocatoria"].ToString()) : 0,
                            nivel = 2
                        });
                    }
                }
            }

            BindTree(lista1);

            tv_aspectos.CollapseAll();
        }

        private void BindTree(IEnumerable<Lista1> list)
        {
            var nodes = list;

            foreach (var node in nodes)
            {
                TreeNode newNode = new TreeNode(node.campo, Convert.ToString(node.id_campo));
                if (node.nivel == 0)
                {
                    tv_aspectos.Nodes.Add(newNode);
                    newNode.ShowCheckBox = true;

                    if (node.codConvocatoria != 0)
                    {
                        newNode.Checked = true;
                    }
                }
                else
                {
                    TreeNode nodeAux = tv_aspectos.Nodes[tv_aspectos.Nodes.Count - 1];
                    if (node.nivel == 1)
                    {
                        nodeAux.ChildNodes.Add(newNode);
                    }
                    else
                    {
                        if (node.nivel == 2)
                        {
                            TreeNode nodeUH = nodeAux.ChildNodes[nodeAux.ChildNodes.Count - 1];
                            nodeUH.ChildNodes.Add(newNode);
                            newNode.ShowCheckBox = true;

                            if (node.codConvocatoria != 0)
                            {
                                newNode.Checked = true;
                            }
                        }
                    }
                }
            }
        }

        protected void btnexpan_Click(object sender, EventArgs e)
        {
            if (btnexpan.Text.Equals("Expandir todo"))
            {
                tv_aspectos.ExpandAll();
                btnexpan.Text = "Contraer todo";
            }
            else
            {
                tv_aspectos.CollapseAll();
                btnexpan.Text = "Expandir todo";
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            foreach (TreeNode nodo in tv_aspectos.Nodes)
            {
                string value = nodo.Value;

                if (nodo.Checked)
                {
                    txtSQL = "SELECT codCampo FROM ConvocatoriaCampo WHERE codConvocatoria = " + Id_EvalConvocatoriaAS + " AND codCampo = " + value;

                    SqlDataReader reader = ejecutaReader(txtSQL, 1);

                    if (reader != null)
                    {
                        if (reader.Read()) { }
                        else
                        {
                            txtSQL = "INSERT INTO ConvocatoriaCampo (CodConvocatoria, codCampo, Puntaje) VALUES (" + Id_EvalConvocatoriaAS + ", " + value + ", 0)";
                            ejecutaReader(txtSQL, 2);
                        }
                    }
                    else
                    {
                        txtSQL = "INSERT INTO ConvocatoriaCampo (CodConvocatoria, codCampo, Puntaje) VALUES (" + Id_EvalConvocatoriaAS + ", " + value + ", 0)";
                        ejecutaReader(txtSQL, 2);
                    }
                    if (nodo.ChildNodes.Count > 0)
                        foreach (TreeNode item in nodo.ChildNodes)
                        {
                            validarNodo(item);    
                        }
                        
                }
                else
                {
                    txtSQL = "DELETE FROM ConvocatoriaCampo WHERE codConvocatoria = " + Id_EvalConvocatoriaAS + " AND codCampo = " + value;
                    ejecutaReader(txtSQL, 2);
                    txtSQL = "DELETE FROM ConvocatoriaCampo " +
                    " WHERE codConvocatoria = " + Id_EvalConvocatoriaAS + " AND codCampo in (SELECT id_campo FROM Campo WHERE codCampo in (SELECT id_campo FROM Campo WHERE codCampo = " + value + "))";
                    ejecutaReader(txtSQL, 2);
                }
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.parent.opener.location.reload();window.parent.opener.focus(); window.parent.close();", true);
            return;
        }

        public void validarNodo(TreeNode nodo)
        {
            string value = nodo.Value;

            foreach (TreeNode nodo2 in nodo.ChildNodes)
            {
                value = nodo2.Value;
                if (nodo2.Checked)
                {
                    txtSQL = "SELECT codCampo FROM ConvocatoriaCampo WHERE codConvocatoria = " + Id_EvalConvocatoriaAS + " AND codCampo = " + value;

                    SqlDataReader reader = ejecutaReader(txtSQL, 1);

                    if (reader != null)
                    {
                        if (reader.Read()) { }
                        else
                        {
                            txtSQL = "INSERT INTO ConvocatoriaCampo (CodConvocatoria, codCampo, Puntaje) VALUES (" + Id_EvalConvocatoriaAS + ", " + value + ", 0)";
                            ejecutaReader(txtSQL, 2);
                        }
                    }
                    else
                    {
                        txtSQL = "INSERT INTO ConvocatoriaCampo (CodConvocatoria, codCampo, Puntaje) VALUES (" + Id_EvalConvocatoriaAS + ", " + value + ", 0)";
                        ejecutaReader(txtSQL, 2);
                    }
                    if (nodo2.ChildNodes.Count > 0)
                        validarNodo(nodo2);
                }
                else
                {
                    txtSQL = "DELETE FROM ConvocatoriaCampo WHERE codConvocatoria = " + Id_EvalConvocatoriaAS + " AND codCampo = " + value;
                    ejecutaReader(txtSQL, 2);
                    txtSQL = "DELETE FROM ConvocatoriaCampo " +
                    " WHERE codConvocatoria = " + Id_EvalConvocatoriaAS + " AND codCampo in (SELECT id_campo FROM Campo WHERE codCampo in (SELECT id_campo FROM Campo WHERE codCampo = " + value + "))";
                    ejecutaReader(txtSQL, 2);
                }
            }
        }

        protected void tv_aspectos_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode node = tv_aspectos.SelectedNode;

            string valor = node.Value;
            Session["codCampoAspecto"] = valor;
            seleccionGCCampos(valor);
        }

        private void seleccionGCCampos(string valor)
        {
            pnlinfonodo.Visible = true;
            pnlAgregarCampo.Visible = false;

            if (!string.IsNullOrEmpty(valor) && !valor.Equals("0"))
            {
                txtSQL = "SELECT codCampo FROM Campo WHERE id_campo = " + valor;
                SqlDataReader reader = ejecutaReader(txtSQL, 1);

                if (reader.Read())
                {
                    if (string.IsNullOrEmpty(reader[0].ToString()))
                    {
                        lnkadicionar.Text = "Nuevo Variable";
                        lnkadicionar.Visible = true;
                        lnkadicionar.Enabled = true;
                        imgaspectoAgr.Visible = true;
                    }
                    else
                    {
                        txtSQL = "SELECT codCampo FROM Campo WHERE id_campo = " + reader[0].ToString();
                        reader = ejecutaReader(txtSQL, 1);
                        if (reader.Read())
                        {
                            if (string.IsNullOrEmpty(reader[0].ToString()))
                            {
                                lnkadicionar.Text = "Nuevo Campo";
                                lnkadicionar.Visible = true;
                                lnkadicionar.Enabled = true;
                                imgaspectoAgr.Visible = true;
                            }
                            else
                            {
                                lnkadicionar.Text = string.Empty;
                                lnkadicionar.Visible = false;
                                lnkadicionar.Enabled = false;
                                imgaspectoAgr.Visible = false;
                            }
                        }
                        else
                        {
                            lnkadicionar.Text = "Nuevo Campo";
                            lnkadicionar.Visible = true;
                            lnkadicionar.Enabled = true;
                            imgaspectoAgr.Visible = true;
                        }
                    }
                }
                else
                {
                    lnkadicionar.Text = "Nuevo Variable";
                    lnkadicionar.Visible = true;
                    lnkadicionar.Enabled = true;
                    imgaspectoAgr.Visible = true;
                }
            }

            txtSQL = "SELECT * FROM Campo WHERE id_campo=" + valor;

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            if (dt.Rows.Count > 0)
            {
                txtdescripcion.Text = dt.Rows[0]["Campo"].ToString();
                try
                {
                    ddlestado.SelectedValue = dt.Rows[0]["Inactivo"].ToString();
                }
                catch (ArgumentOutOfRangeException) { }
            }

            codPadre = dt.Rows[0]["codCampo"].ToString();
            codCampo = dt.Rows[0]["id_Campo"].ToString();

            del myDelegate = (x) =>
            {
                if (x)
                    return "Inactivo";
                else
                    return "Activo";
            };

            var result = from c in consultas.Db.Campos
                         where c.codCampo == Convert.ToInt32(valor)
                         select new
                         {
                             c.id_Campo,
                             c.Campo1,
                             activo = myDelegate(bool.Parse("" + c.Inactivo))
                         };

            gv_campos.DataSource = result;
            gv_campos.DataBind();
        }

        protected void gv_campos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("ver"))
            {
                string valor = e.CommandArgument.ToString();

                seleccionGCCampos(valor);
            }
            else
            {
                if (e.CommandName.Equals("eliminar"))
                {
                    txtSQL = "DELETE FROM Campo WHERE id_campo = " + e.CommandArgument.ToString();
                    ejecutaReader(txtSQL, 2);

                    tv_aspectos.Nodes.Clear();

                    crearLista();

                    pnlinfonodo.Visible = false;
                    pnlAgregarCampo.Visible = false;
                }
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if(!codCampo.Equals("0"))
                txtSQL = "SELECT codCampo FROM Campo WHERE Campo = '" + txtdescripcion.Text + "' and id_campo <> " + codCampo;

            SqlDataReader reader = ejecutaReader(txtSQL, 1);

            if (reader != null)
            {
                if (reader.Read())
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe un campo con ese Nombre.')", true);
                    return;
                }
                else
                {
                    txtSQL = " UPDATE Campo SET Campo = '" + txtdescripcion.Text + "', Inactivo = " + ddlestado.SelectedValue + " WHERE Id_Campo =" + codCampo;
                    ejecutaReader(txtSQL, 2);
                }
            }
            else
            {
                txtSQL = " UPDATE Campo SET Campo = '" + txtdescripcion.Text + "', Inactivo = " + ddlestado.SelectedValue + " WHERE Id_Campo =" + codCampo;
                ejecutaReader(txtSQL, 2);
            }

            tv_aspectos.Nodes.Clear();

            crearLista();

            pnlinfonodo.Visible = false;
            pnlAgregarCampo.Visible = false;
        }

        protected void lnkadicionar_Click(object sender, EventArgs e)
        {
            lbltituloAgregar.Text = lnkadicionar.Text;
            pnlAgregarCampo.Visible = true;
            pnlinfonodo.Visible = false;
        }

        protected void btnAgregarCampo_Click(object sender, EventArgs e)
        {
            txtSQL = "SELECT * FROM Campo WHERE Campo = '" + txtnombrecampo.Text + "'";

            SqlDataReader reader = ejecutaReader(txtSQL, 1);

            if (reader != null)
            {
                if (reader.Read())
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe un campo con ese Nombre.')", true);
                    return;
                }
                else
                {
                    txtSQL = "INSERT INTO Campo (Campo, codCampo, Inactivo) VALUES ('" + txtnombrecampo.Text + "', " + codCampo + ", " + ddlnuevoactivo.SelectedValue + ")";
                    ejecutaReader(txtSQL, 2);
                }
            }
            else
            {
                txtSQL = "INSERT INTO Campo (Campo, codCampo, Inactivo) VALUES ('" + txtnombrecampo.Text + "', " + codCampo + ", " + ddlnuevoactivo.SelectedValue + ")";
                ejecutaReader(txtSQL, 2);
            }

            tv_aspectos.Nodes.Clear();

            crearLista();

            pnlinfonodo.Visible = false;
            pnlAgregarCampo.Visible = false;
        }
    }

    public class Lista1
    {
        public int id_campo;
        public string campo;
        public int codConvocatoria;
        public int nivel;
    }
}