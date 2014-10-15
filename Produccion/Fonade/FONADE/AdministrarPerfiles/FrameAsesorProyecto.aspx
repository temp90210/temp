<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/Emergente.Master" AutoEventWireup="true" CodeBehind="FrameAsesorProyecto.aspx.cs" Inherits="Fonade.FrameAsesorProyecto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .pagina1 {
            width: 30%;
            height: auto;
            border: 1px solid black;
            position: relative;
            float: left;
            overflow: scroll;
            height: 660px;
        }
        
        .pagina2 {
            width: 65%;
            height: auto;
            border: 1px solid black;
            position: relative;
            float: right;
            overflow: scroll;
            height: 620px;
            padding: 20px;
        }

        #pnlPrincipal {
            display: inline-block;
            width: 100%;
        }

        .ContentInfo {
            width: 100%;
        }

        body, #form1 {
            width: 1000px;
            height: 660px;
        }

        .asig {
            margin: 0px auto;
            width: 100%;
            text-align: center;
        }

        .seciones {
            padding-top: 25px;
            padding-bottom: 25px;
        }
        .aspNetHidden {
            display: none;
        }
        .seciones div{
            height: inherit !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
    <div>
        <div class="pagina1">
            <br />
            <h1 style="text-align:center;">
                <label>PLANES DE NEGOCIO</label>
            </h1>
            <asp:GridView ID="gv_proyectos" runat="server" AutoGenerateColumns="false" CssClass="Grilla" DataSourceID="ldsproyectos" AllowPaging="True" PageSize="20" OnPageIndexChanging="gv_proyectos_PageIndexChanging" AllowSorting="true" OnRowCreated="gv_proyectos_RowCreated" DataKeyNames="Id_Proyecto" OnRowCommand="gv_proyectos_RowCommand" Width="100%">
                <Columns>
                    <asp:TemplateField SortExpression="NomProyecto">
                        <ItemTemplate>
                            <asp:Button ID="btnproyecto" runat="server" CssClass="boton_Link_Grid" Text='<%# ">> " + Eval("Id_Proyecto") + " - " + Eval("NomProyecto") %>' CommandArgument='<%# Eval("Id_Proyecto") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="imgadmiracion" runat="server" ImageUrl="~/Images/admiracion.gif" Visible="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblcontactos" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <div class="pagina2">
            <asp:Label ID="lbltitulo" runat="server" Text=""></asp:Label>
            <span class="seciones">
                <asp:GridView ID="gvrasesorlider" runat="server" CssClass="Grilla" Width="90%" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" AllowSorting="true" DataSourceID="ldsasesorlider">
                    <Columns>
                        <asp:BoundField HeaderText="Asesor Lider" DataField="Nombre" SortExpression="Nombre" />
                        <asp:TemplateField HeaderText="Email Asesor" SortExpression="Email">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkemail" runat="server" Text='<%# Eval("Email") %>' PostBackUrl='<%# "mailto:" + Eval("Email") %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </span>
            <span class="seciones">
                <asp:GridView ID="gvrasesores" runat="server" CssClass="Grilla" Width="90%" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" AllowSorting="true" DataSourceID="ldsasesores">
                    <Columns>
                        <asp:BoundField HeaderText="Asesor" DataField="Nombre" SortExpression="Nombre" />
                        <asp:TemplateField HeaderText="Email Asesor" SortExpression="Email">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkemail1" runat="server" Text='<%# Eval("Email") %>' PostBackUrl='<%# "mailto:" + Eval("Email") %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:LinkButton ID="lnkasignacionasesores" runat="server" Text=">> ASIGNACIÓN DE ASESORES <<" OnClick="lnkasignacionasesores_Click" CssClass="asig" Width="100%"></asp:LinkButton>
            </span>
            <span class="seciones">
                <asp:GridView ID="gvrasignarasesores" runat="server" CssClass="Grilla" Width="90%" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" AllowSorting="true" DataSourceID="ldsasesoresasignar" DataKeyNames="Id_Contacto" OnRowCreated="gvrasignarasesores_RowCreated" Visible="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Asesor" SortExpression="Nombre">
                            <ItemTemplate>
                                <asp:CheckBox ID="cbxasesor" runat="server" Text='<%# Eval("Nombre") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Asesor Lider" SortExpression="Nombre">
                            <ItemTemplate>
                                <asp:RadioButton ID="rbasesorlider" runat="server" Text='<%# Eval("Nombre") %>' GroupName="lider" OnCheckedChanged="rbasesorlider_CheckedChanged" AutoPostBack="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </span>
            <br />
            <asp:Button ID="btnactualizar" runat="server" Text="Actualizar" OnClick="btnactualizar_Click" />
        </div>
    </div>
    <asp:LinqDataSource ID="ldsproyectos" runat="server" ContextTypeName="Datos.FonadeDBDataContext" OnSelecting="ldsproyectos_Selecting" AutoPage="true"></asp:LinqDataSource>
    <asp:LinqDataSource ID="ldsasesorlider" runat="server" ContextTypeName="Datos.FonadeDBDataContext" OnSelecting="ldsasesorlider_Selecting"></asp:LinqDataSource>
    <asp:LinqDataSource ID="ldsasesores" runat="server" ContextTypeName="Datos.FonadeDBDataContext" OnSelecting="ldsasesores_Selecting"></asp:LinqDataSource>
    <asp:LinqDataSource ID="ldsasesoresasignar" runat="server" ContextTypeName="Datos.FonadeDBDataContext" OnSelecting="ldsasesoresasignar_Selecting"></asp:LinqDataSource>
    <asp:HiddenField ID="hdproyecto" runat="server" />
</asp:Content>
