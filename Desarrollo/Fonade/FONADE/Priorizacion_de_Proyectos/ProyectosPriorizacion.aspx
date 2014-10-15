<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectosPriorizacion.aspx.cs"
    Inherits="Fonade.FONADE.Priorizacion_deProyectos.ProyectosPriorizacion" MasterPageFile="~/Master.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:LinqDataSource ID="lds_proyectospriorizar" runat="server" ContextTypeName="Datos.FonadeDBDataContext"
        AutoPage="true" OnSelecting="lds_proyectospriorizar_Selecting">
    </asp:LinqDataSource>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="PanelGrilla" runat="server" Visible="true" Width="98%" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Paneltitulo" runat="server" Width="100%">
                <table width="100%">
                    <tr>
                        <td class="style23">
                            <h1>
                                <asp:Label runat="server" ID="lbl_Titulo" Style="font-weight: 700"></asp:Label></h1>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="PanelIr_A" runat="server" Width="100%" Visible="false">
                <table width="100%">
                    <tr>
                        <td class="style20">
                            &nbsp;
                        </td>
                        <td class="style21">
                            <asp:LinkButton ID="lbtn_iraacta" runat="server" OnClick="lbtn_iraacta_Click"></asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table width="100%" border="0">
                <tr>
                    <td class="style10">
                        <asp:CheckBox ID="ChBSeleccionarTodo" runat="server" Text="Seleccionar Todo" OnCheckedChanged="ChBSeleccionarTodo_CheckedChanged"
                            AutoPostBack="true" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="gv_proyectospriorizar" CssClass="Grilla" runat="server" AllowPaging="false"
                            ShowHeaderWhenEmpty="true" AutoGenerateColumns="false" DataSourceID="lds_proyectospriorizar"
                            EmptyDataText="No hay proyectos en el estado de Asignación de Recursos" Width="100%"
                            DataKeyNames="codigoproyecto" OnLoad="gv_proyectospriorizar_Load">
                            <Columns>
                                <asp:TemplateField HeaderText="Sel.">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChBSelct" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="" />
                                <asp:TemplateField HeaderText="Id">
                                    <ItemTemplate>
                                        <asp:Label ID="codigo_proyecto" runat="server" Text='<%# Eval("codigoproyecto") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Plan de Negocio">
                                    <ItemTemplate>
                                        <asp:Label ID="nombre_proyecto" runat="server" Text='<%# Eval("nombreproyecto") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Convocatoria">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hiddenIDConvoc" runat="server" Value='<%# Eval("codigoconvocatoria") %>' />
                                        <asp:Label ID="nombre_convocatoria" runat="server" Text='<%# Eval("nombreconvocatoria") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Recursos">
                                    <ItemTemplate>
                                        <asp:Label ID="valor_recomendado" runat="server" Text='<%# Eval("valorrecomendado") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <%--          <tr>
            <td align="right" style="font-weight: 700">
                Total Recursos Seleccionados:&nbsp;<asp:Label ID="l_recursos" runat="server"></asp:Label>
            </td>
          </tr>--%>
            </table>
            <%------------------------------------------------------------------------------------%>
            <table width="100%">
                <tr>
                    <td colspan="4">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="style11">
                        &nbsp;
                    </td>
                    <td colspan="2" style="font-weight: 700;" class="style24">
                        Detalles del Acta
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="style11">
                        &nbsp;
                    </td>
                    <td class="style12" valign="baseline">
                        Número de Acta:
                    </td>
                    <td class="style13" valign="baseline">
                        <asp:TextBox ID="txt_numeroacta" runat="server" Width="100px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txt_numeroacta"
                            Display="Dynamic" ErrorMessage="RequiredFieldValidator" ForeColor="Red">* Este campo está vacío.</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="style11">
                        &nbsp;
                    </td>
                    <td class="style12" valign="baseline">
                        Nombre del Acta:
                    </td>
                    <td class="style13" valign="baseline">
                        <asp:TextBox ID="txt_nombreacta" runat="server" Width="275px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txt_nombreacta"
                            Display="Dynamic" ErrorMessage="RequiredFieldValidator" ForeColor="Red">* Este campo está vacío.</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="style11">
                        &nbsp;
                    </td>
                    <td class="style12" valign="baseline">
                        Fecha del Acta:
                    </td>
                    <td class="style13" valign="baseline">
                        <asp:TextBox ID="txt_fecha" runat="server" BackColor="White" Text="" Width="100px"
                            Enabled="False"></asp:TextBox>
                        &nbsp;
                        <asp:Image ID="btnfecha" runat="server" AlternateText="cal1" ImageAlign="AbsBottom"
                            ImageUrl="~/Images/calendar.png" Height="21px" Width="20px" />
                        <asp:CalendarExtender ID="Calendariofacta" runat="server" Format="dd/MM/yyyy" PopupButtonID="btnfecha"
                            TargetControlID="txt_fecha" />
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txt_fecha"
                            Display="Dynamic" ErrorMessage="RequiredFieldValidator" ForeColor="Red">* Este campo está vacío.</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="style11">
                        &nbsp;
                    </td>
                    <td class="style12" valign="baseline">
                        Convocatoria:
                    </td>
                    <td class="style13" valign="baseline">
                        <asp:DropDownList ID="ddl_convocatoria" runat="server" Width="275px">
                        </asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="style16">
                    </td>
                    <td class="style17" valign="bottom">
                        Observaciones:
                    </td>
                    <td class="style18">
                    </td>
                    <td class="style19">
                    </td>
                </tr>
                <tr>
                    <td class="style11">
                        &nbsp;
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="txt_observaciones" runat="server" Height="140px" TextMode="MultiLine"
                            Width="440px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txt_observaciones"
                            Display="Dynamic" ErrorMessage="RequiredFieldValidator" ForeColor="Red">* Este campo está vacío.</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="style14">
                    </td>
                    <td colspan="2" align="center" class="style15">
                        <asp:Button ID="btn_asignarRecursos" runat="server" Text="Asignar Recursos" OnClick="btn_asignarRecursos_Click" />
                        <ajaxToolkit:ConfirmButtonExtender ID="cbe" runat="server" TargetControlID="btn_asignarRecursos"
                            ConfirmText="¿Desea pasar a Legalización de Contrato los Planes de Negocio seleccionados?" />
                    </td>
                    <td class="style15">
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        &nbsp;
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .style10
        {
            height: 39px;
        }
        .style11
        {
            width: 20px;
        }
        .style12
        {
            width: 161px;
            font-weight: bold;
        }
        .style13
        {
            width: 284px;
        }
        .style14
        {
            width: 20px;
            height: 52px;
        }
        .style15
        {
            height: 52px;
        }
        .style16
        {
            width: 20px;
            height: 22px;
        }
        .style17
        {
            width: 161px;
            font-weight: bold;
            height: 22px;
        }
        .style18
        {
            width: 284px;
            height: 22px;
        }
        .style19
        {
            height: 22px;
        }
        .style20
        {
            width: 87px;
        }
        .style21
        {
            width: 509px;
        }
        .style23
        {
            width: 630px;
        }
        .style24
        {
            font-size: medium;
        }
    </style>
    <script type="text/javascript">
        function ValidNum(e) {
            var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
            if (/[^A-Za-z0-9 ]/.test(String.fromCharCode(tecla))) {
                return false;
            }
        }
    </script>
</asp:Content>
