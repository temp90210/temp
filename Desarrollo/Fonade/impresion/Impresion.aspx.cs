using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.impresion
{
    public partial class Impresion : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //tv_tab.Nodes.Clear();
            if (!IsPostBack)
            {
                llenarTreeView();
                tv_tab.Attributes.Add("onclick", "postBackByObject()");
                enlaceproyecto(txt_codProyecto.Text.Trim());
            }
        }

        public void enlaceproyecto(string param)
        {
            String txtSQL = "SELECT id_proyecto, nomproyecto, nomciudad, nomdepartamento, inactivo FROM proyecto , ciudad, departamento WHERE id_ciudad=codciudad and id_departamento = coddepartamento ";
            DataTable datatable = new DataTable();

            switch (usuario.CodGrupo)
            {
                case Constantes.CONST_AdministradorSena:
                    txtSQL = txtSQL + " and inactivo=0 ";
                    break;
                case Constantes.CONST_JefeUnidad:
                    txtSQL = txtSQL + " and codinstitucion = " + usuario.CodInstitucion;
                    break;
                case Constantes.CONST_Asesor:
                    txtSQL = txtSQL + " and inactivo=0 and codinstitucion = " + usuario.CodInstitucion + " and  exists (select codproyecto from proyectocontacto pc  where id_proyecto=codproyecto and pc.codcontacto=" + usuario.IdContacto + " and pc.inactivo=0)";
                    break;
                case Constantes.CONST_Emprendedor:
                    txtSQL = txtSQL + " and inactivo=0 and codinstitucion = " + usuario.CodInstitucion + " and  exists (select codproyecto from proyectocontacto pc  where id_proyecto=codproyecto and pc.codcontacto=" + usuario.IdContacto + " and pc.inactivo=0)";
                    break;
                case Constantes.CONST_Evaluador:
                    txtSQL = txtSQL + " and inactivo=0  and  exists (select codproyecto from proyectocontacto pc  where id_proyecto=codproyecto and pc.codcontacto=" + usuario.IdContacto + " and pc.inactivo=0)";
                    break;
                case Constantes.CONST_CoordinadorEvaluador:
                    txtSQL = txtSQL + " and inactivo=0  and  exists (select codproyecto from proyectocontacto pc  where id_proyecto=codproyecto and pc.codcontacto=" + usuario.IdContacto + " and pc.inactivo=0)";
                    break;
                case Constantes.CONST_GerenteEvaluador:
                    txtSQL = txtSQL + " and inactivo=0 and codestado>=" + Constantes.CONST_Convocatoria;
                    break;
                case Constantes.CONST_Interventor:
                    txtSQL = txtSQL + " and inactivo=0 and id_proyecto in  (select distinct Codproyecto from empresaInterventor EI, empresa E  where id_empresa=codempresa and EI.inactivo=0 and EI.CodContacto = " + usuario.IdContacto + ")";
                    break;
            }

            //Si el parámetro "es decir, el código del proyecto" tiene datos, lo agrega a la consulta.
            if (param.Trim() != "") { txtSQL = txtSQL + "  and Id_proyecto = " + param; }

            txtSQL = txtSQL + " order by nomProyecto";

            datatable = consultas.ObtenerDataTable(txtSQL, "text");

            DDL_proyecto.Items.Clear();

            foreach (DataRow row in datatable.Rows)
            {
                ListItem item = new ListItem();
                item.Text = row["nomproyecto"].ToString();
                item.Value = row["id_proyecto"].ToString();
                DDL_proyecto.Items.Add(item);
            }
        }

        private void llenarTreeView()
        {
            String txtSQL = "SELECT  isnull(codtab, id_tab) orden, id_tab, nomTab FROM Tab WHERE isnull(codtab, id_tab) not in(" + Constantes.CONST_PlanOperativo + ", " + Constantes.CONST_Anexos + ")  ORDER BY orden, id_tab";

            DataTable datatable = new DataTable();
            datatable = consultas.ObtenerDataTable(txtSQL, "text");

            List<Tab> lista1 = new List<Tab>();

            foreach (DataRow fila in datatable.Rows)
            {
                lista1.Add(new Tab()
                {
                    orden = Convert.ToInt32(fila["orden"].ToString()),
                    id_tab = Convert.ToInt32(fila["id_tab"].ToString()),
                    nomTab = fila["nomTab"].ToString()
                });
            }

            BindTree(lista1);
        }

        private void BindTree(IEnumerable<Tab> list)
        {
            var nodes = list;

            foreach (var node in nodes)
            {
                TreeNode newNode = new TreeNode(node.nomTab, Convert.ToString(node.id_tab));

                if (node.id_tab != node.orden)
                {
                    TreeNode nodeAux = tv_tab.Nodes[tv_tab.Nodes.Count - 1];
                    nodeAux.ChildNodes.Add(newNode);
                }
                else
                {
                    tv_tab.Nodes.Add(newNode);
                }

                newNode.SelectAction = TreeNodeSelectAction.None;
                newNode.ShowCheckBox = true;
            }
        }

        protected void btnimpresion_Click(object sender, EventArgs e)
        {
            ClientScriptManager cm = this.ClientScript;


            if (string.IsNullOrEmpty(DDL_proyecto.SelectedValue))
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('No ha subido ningun archivo');</script>");
                return;
            }

            Session["codProye"] = DDL_proyecto.SelectedValue;
            Session["listaTap"] = tv_tab;

            Response.Redirect("VerImpresion.aspx");
        }

        protected void tv_tab_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            foreach (TreeNode nodeHijo in e.Node.ChildNodes)
            {
                nodeHijo.Checked = e.Node.Checked;
            }
        }

        protected void lnk_buscarProyectos_Click(object sender, EventArgs e)
        {
            DDL_proyecto.Items.Clear();
            enlaceproyecto(txt_codProyecto.Text.Trim());
            txt_codProyecto.Text = "";
        }
    }

    public class Tab
    {
        public int orden;
        public int id_tab;
        public string nomTab;
    }
}