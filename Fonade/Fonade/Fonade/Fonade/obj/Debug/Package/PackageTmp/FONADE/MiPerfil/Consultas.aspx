<%@ Page MasterPageFile="~/Master.master" Language="C#" AutoEventWireup="true" CodeBehind="Consultas.aspx.cs"
    Inherits="Fonade.FONADE.MiPerfil.Consultas" Title="FONADE - " %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <script src="../../Scripts/jquery-1.10.2.min.js"></script>
    <script src="../../Scripts/ScriptsEspecificos.js"></script>
    <script type="text/javascript">
        function ValidNum(e) {
            var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
            return (tecla > 47 && tecla < 58);
        }
    </script>
    <script>
        $(function () {
            $(".divA").scroll(function () {
                $(".divB").scrollLeft($(".divA").scrollLeft());
            });
            $(".divB").scroll(function () {
                $(".divA").scrollLeft($(".divB").scrollLeft());
            });
        });
    </script>
    <style type="text/css">
        .tablaPaginador
        {
            bottom: 36px !important;
        }
        .PagerControl
        {
            text-decoration: none;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:LinqDataSource ID="lds_Consultar" runat="server" ContextTypeName="Datos.FonadeDBDataContext"
        AutoPage="false" OnSelecting="lds_Consultar_Selecting">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="lds_Consultarporrol" runat="server" ContextTypeName="Datos.FonadeDBDataContext"
        AutoPage="false" OnSelecting="lds_Consultarporrol_Selecting">
    </asp:LinqDataSource>
    <h1>
        <asp:Label runat="server" ID="lbl_Titulo" /></h1>
    <asp:Panel ID="Panel1" runat="server">
        <table width="98%" border="0" cellspacing="1" cellpadding="4">
            <tbody>
                <tr>
                    <td align="left">
                        <span>Planes de Negocio</span><br />
                    </td>
                </tr>
                <tr bgcolor="#3D5A87">
                    <td width="40%" align="left" style="color: White;">
                        Búsqueda por palabra
                    </td>
                </tr>
                <tr valign="top">
                    <td align="left">
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <strong>Por palabra:</strong>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_porPalabra" runat="server" Width="156px" MaxLength="80" />
                                    </td>
                                    <td>
                                        <asp:Button runat="server" ID="btn_buscar" OnClick="btn_buscar_onclick" Text="Buscar" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="40%" align="left">
                        &nbsp;
                    </td>
                </tr>
                <tr bgcolor="#3D5A87">
                    <td width="40%" align="left" style="color: White;">
                        Búsqueda avanzada
                    </td>
                </tr>
                <tr valign="top">
                    <td align="left">
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <b>Código:</b>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_codigo" runat="server" Width="106px" MaxLength="80" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Departamento:</b>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddl_departamento" AutoPostBack="true" OnSelectedIndexChanged="ddl_departamento_OnSelectedIndexChanged"
                                            runat="server" Width="100%" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Municipio:</b>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddl_municipio" AutoPostBack="true" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Sector:</b>
                                    </td>
                                    <td>
                                        <asp:ListBox ID="lb_sector" runat="server" Width="300px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Estado:</b>
                                    </td>
                                    <td>
                                        <asp:ListBox ID="lb_estados" runat="server" Width="300px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Unidad Emprendimiento:</b>
                                    </td>
                                    <td>
                                        <asp:ListBox ID="lb_unidadEmprendimiento" runat="server" Width="410px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <b>Asesor:</b>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_asesor" runat="server" Width="150px" />
                                        <asp:HiddenField ID="hdf_CodContacto" runat="server" />
                                        <asp:Button ID="btn_buscarAsesor" OnClick="btn_buscarAsesor_onclick" runat="server"
                                            Text="Buscar Asesor" />
                                        <br />
                                        <asp:LinkButton ID="lnk_Limpiar" Text="Limpiar" runat="server" OnClick="lnk_Limpiar_Click" />
                                        <asp:ListBox ID="lb_asesores" runat="server" Width="300px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Incluir Descripción:</b>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CheckBox1" runat="server" Text="Incluir descripción" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td align="right">
                                        <asp:Button ID="btn_BusquedaAvanzada" OnClick="btn_BusquedaAvanzada_onclick" runat="server"
                                            Text="Buscar" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="40%" align="left">
                        &nbsp;
                    </td>
                </tr>
                <tr id="tr_part_I" runat="server" visible="false">
                    <td align="left">
                        <span>Emprendedores, asesores</span><br />
                    </td>
                </tr>
                <tr id="tr_part_II" runat="server" visible="false" bgcolor="#3D5A87">
                    <td width="40%" align="left" style="color: White;">
                        Búsqueda por palabra
                    </td>
                </tr>
                <tr id="tr_part_III" runat="server" visible="false" valign="top">
                    <td align="left">
                        <table width="100%">
                            <tbody>
                                <tr>
                                    <td>
                                        <b>Por Cédula o Palabra:</b>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tb_cedulaPalabra" runat="server" Width="256px" MaxLength="80" />
                                    </td>
                                </tr>
                                <tr valign="middle">
                                    <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal"
                                        Width="260px">
                                        <asp:ListItem Value="3">Emprendedor</asp:ListItem>
                                        <asp:ListItem Value="1,2">Asesor</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="1,2,3">Todos</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <caption>
                                        <tr>
                                            <td align="right" width="250">
                                                <asp:Button ID="btn_buscartodos" runat="server" OnClick="tb_cedulaPalabra_onclick"
                                                    Text="Buscar" />
                                            </td>
                                        </tr>
                                    </caption>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
        <table id="vercoor1" runat="server" width="98%" border="0" cellspacing="1" cellpadding="4"
            visible="false">
            <tbody>
                <%--JEFES UNIDAD--%>
                <tr>
                    <td align="left">
                        <span>Jefes de unidad</span><br />
                    </td>
                </tr>
                <tr bgcolor="#3D5A87">
                    <td width="40%" align="left" style="color: White;">
                        Búsqueda por palabra
                    </td>
                </tr>
                <tr valign="top">
                    <td align="left">
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <b>Por Cédula o Palabra:</b>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tctcedulaopalabra" runat="server" Width="156px" MaxLength="80" />
                                    </td>
                                    <td>
                                        <asp:Button runat="server" OnClick="btnbuscarcedulaopalabra_Click" ID="btnbuscarcedulaopalabra"
                                            Text="Buscar" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <%--JEFES UNIDAD--%>
            </tbody>
        </table>
        <table id="vercoor2" runat="server" width="98%" border="0" cellspacing="1" cellpadding="4"
            visible="false">
            <tbody>
                <%--UNIDADES DE EMPRENDIMIENTO--%>
                <tr>
                    <td align="left">
                        <span>Unidades de Emprendimiento</span><br />
                    </td>
                </tr>
                <tr bgcolor="#3D5A87">
                    <td width="100%" align="left" style="color: White;">
                        Búsqueda por palabra
                    </td>
                </tr>
                <tr valign="top">
                    <td align="left">
                        <table>
                            <tbody>
                                <tr>
                                    <td>
                                        <strong>Por Palabra:</strong>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtpalabra" runat="server" Width="156px" MaxLength="80" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr valign="middle">
                    <td valign="middle">
                        <table width="95%" border="0">
                            <tbody>
                                <tr>
                                    <asp:RadioButtonList ID="radiobutonunidades" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="1">Sena</asp:ListItem>
                                        <asp:ListItem Value="3">Externa</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="2">Todos</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <td align="right">
                                        <asp:Button ID="btnbuscarcedulaopalabra0" runat="server" OnClick="btnbuscarcedulaopalabra0_Click"
                                            Text="Buscar" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="40%" align="left">
                        &nbsp;
                    </td>
                </tr>
                <%--UNIDADES DE EMPRENDIMIENTO--%>
            </tbody>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlInfoResultados" runat="server" Height="50px">
        <table width="95%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center" colspan="3">
                    <asp:Label ID="lblResults" Text="Se han encontrado planes de <br/> negocio buscando ''"
                        runat="server" Font-Bold="true" />
                </td>
                <td align="left" colspan="3">
                    <asp:LinkButton runat="server" ID="hpl_nueva" Text="Realizar otra búsqueda..." OnClick="hpl_nueva_onclick"
                        ForeColor="#CC0000" Style="text-decoration: none;" />
                </td>
            </tr>
            <tr>
                <td colspan="7">
                    <table border="0" width="100%">
                        <tbody>
                            <tr>
                                <td align="left" width="120">
                                    <asp:Label ID="lbl_NumeroPaginas" Text="página 1 de 3976" runat="server" />
                                </td>
                                <td align="right" width="200">
                                    Planes por página
                                    <asp:DropDownList ID="numRegPorPagina" runat="server" AutoPostBack="True" OnSelectedIndexChanged="numRegPorPagina_SelectedIndexChanged">
                                        <asp:ListItem Value="10" Selected="True">10</asp:ListItem>
                                        <asp:ListItem Value="20">20</asp:ListItem>
                                        <asp:ListItem Value="30">30</asp:ListItem>
                                        <asp:ListItem Value="40">40</asp:ListItem>
                                        <asp:ListItem Value="50">50</asp:ListItem>
                                        <asp:ListItem Value="60">60</asp:ListItem>
                                        <asp:ListItem Value="70">70</asp:ListItem>
                                        <asp:ListItem Value="80">80</asp:ListItem>
                                        <asp:ListItem Value="90">90</asp:ListItem>
                                        <asp:ListItem Value="100">100</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel2" runat="server">
        <asp:GridView ID="grdMain" runat="server" AutoGenerateColumns="false" CssClass="Grilla"
            AllowPaging="true" OnRowCommand="grdMain_RowCommand" AllowSorting="True" PageSize="10"
            OnPageIndexChanging="grdMain_PageIndexChanging" RowStyle-HorizontalAlign="Center"
            OnSorting="grdMain_Sorting" PagerSettings-Position="Top" PagerStyle-HorizontalAlign="Center"
            OnRowDataBound="grdMain_RowDataBound" PagerStyle-ForeColor="#CC0000" PagerSettings-NextPageText=">"
            PagerSettings-LastPageText=">>" PagerSettings-PreviousPageImageUrl="<" PagerSettings-Mode="NumericFirstLast"
            PagerStyle-Font-Underline="false" PagerStyle-CssClass="PagerControl">
            <Columns>
                <asp:BoundField DataField="Id_Proyecto" HeaderText="Código" SortExpression="Id_Proyecto"
                    HeaderStyle-Width="2%" />
                <asp:BoundField DataField="NomSubSector" HeaderText="Sector" SortExpression="NomSubSector"
                    HeaderStyle-Width="15%" />
                <asp:BoundField DataField="NomCiudad" HeaderText="Ciudad" SortExpression="NomCiudad"
                    HeaderStyle-Width="8%" />
                <asp:BoundField DataField="NomDepartamento" HeaderText="Departamento" SortExpression="NomDepartamento"
                    HeaderStyle-Width="8%"></asp:BoundField>
                <asp:TemplateField SortExpression="NomProyecto" HeaderText="Plan de Negocio" HeaderStyle-Width="15%">
                    <ItemTemplate>
                        <asp:LinkButton ID="Lnkbtnplan" runat="server" Text='<%# Eval("NomProyecto") %>'
                            CommandArgument='<%# Eval("Id_Proyecto") %>' CssClass="boton_Link_Grid" CommandName="plan"
                            ForeColor="#CC0000" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="Descripcion" HeaderText="Descripción" HeaderStyle-Width="22%">
                    <ItemTemplate>
                        <asp:Label ID="tb_descripcion" runat="server" Text='<%# Eval("Sumario")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="NomEstado" HeaderText="Estado" SortExpression="NomEstado"
                    HeaderStyle-Width="10%" />
                <asp:BoundField DataField="Unidad" HeaderText="Unidad" SortExpression="Unidad" HeaderStyle-Width="10%" />
                <asp:BoundField DataField="N_NomConvocatoria" HeaderText="Nombre Ultima Convocatoria"
                    HeaderStyle-Width="10%" />
                <asp:BoundField DataField="N_Fecha" HeaderText="Fecha de Creación" HeaderStyle-Width="10%"
                    SortExpression="N_Fecha" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="Panel3" runat="server">
        <asp:GridView ID="gv_busquedaporrol" runat="server" Width="100%" AutoGenerateColumns="false"
            CssClass="Grilla" AllowPaging="true" OnRowCommand="gv_busquedaporrol_RowCommand"
            PageSize="10" AllowSorting="True" OnPageIndexChanging="gv_busquedaporrol_PageIndexChanging"
            PagerSettings-Position="Top" PagerStyle-HorizontalAlign="Center" OnRowDataBound="gv_busquedaporrol_RowDataBound"
            PagerStyle-ForeColor="#CC0000" PagerSettings-NextPageText=">" PagerSettings-LastPageText=">>"
            PagerSettings-PreviousPageImageUrl="<" PagerSettings-Mode="NumericFirstLast"
            PagerStyle-Font-Underline="false" PagerStyle-CssClass="PagerControl">
            <Columns>
                <asp:BoundField DataField="Id_Proyecto" HeaderText="Código Plan" SortExpression="Id_Proyecto"
                    HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left" />
                <asp:TemplateField SortExpression="NomProyecto" HeaderText="Plan de Negocio" HeaderStyle-Width="5%"
                    ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnk_planNegocio" Text='<%# Eval("NomProyecto") %>' runat="server"
                            CausesValidation="false" CommandArgument='<%# Eval("Id_Proyecto") %>' CommandName="planes"
                            Style="text-decoration: none;" ForeColor="#CC0000" Width="94px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="NomTipoIdentificacion" HeaderText="Tipo Documento" SortExpression="NomTipoIdentificacion"
                    HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="Identificacion" HeaderText="Número Documento" SortExpression="Identificacion"
                    HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left" />
                <asp:TemplateField HeaderText="Nombres" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnk_nombres" Text='<%# Eval("Nombres") %>' runat="server" Style="text-decoration: none;"
                            Width="46px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField SortExpression="Email" HeaderText="Email" HeaderStyle-Width="5%"
                    ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <asp:Button ID="btnemal" runat="server" Text='<%# Eval("Email") %>' CommandArgument='<%# Eval("Id_Proyecto") %>'
                            CommandName="email" CssClass="boton_Link_Grid" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Nombre" HeaderText="Rol" SortExpression="Nombre" HeaderStyle-Width="5%"
                    ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="nomInstitucion" HeaderText="Unidad de Emprendimiento"
                    SortExpression="nomInstitucion" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="NomTipoInstitucion" HeaderText="Tipo de Institución" SortExpression="NomTipoInstitucion"
                    HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="CodConvocatoria" HeaderText="Número Ultima Convocatoria"
                    HeaderStyle-Width="10%" SortExpression="CodConvocatoria" />
                <asp:BoundField DataField="N_NomConvocatoria" HeaderText="Nombre Ultima Convocatoria"
                    HeaderStyle-Width="10%" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="Panel4" runat="server">
        <div style="padding: 20px 0px;">
            <asp:GridView ID="gv_busquedaavanzada" runat="server" Width="100%" AutoGenerateColumns="false"
                DataKeyNames="" CssClass="Grilla" AllowPaging="true" OnRowCommand="gv_busquedaavanzada_RowCommand"
                FooterStyle-CssClass="PaginadorUno" AllowSorting="True" HeaderStyle-HorizontalAlign="Left"
                RowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_busquedaavanzada_PageIndexChanging"
                PagerSettings-Position="Top" PagerStyle-HorizontalAlign="Center" OnRowDataBound="gv_busquedaavanzada_RowDataBound"
                PagerStyle-ForeColor="#CC0000" PagerSettings-NextPageText=">" PagerSettings-LastPageText=">>"
                PagerSettings-PreviousPageImageUrl="<" PagerSettings-Mode="NumericFirstLast"
                PagerStyle-CssClass="PagerControl">
                <Columns>
                    <asp:BoundField DataField="Id_Proyecto" HeaderText="Código" SortExpression="Id_Proyecto"
                        HeaderStyle-Width="5%" />
                    <asp:BoundField DataField="NomSubSector" HeaderText="Sector" SortExpression="NomSubSector"
                        HeaderStyle-Width="20%" />
                    <asp:BoundField DataField="NomCiudad" HeaderText="Localización Proyecto" SortExpression="NomCiudad"
                        HeaderStyle-Width="20%" />
                    <asp:TemplateField SortExpression="NomProyecto" HeaderText="Plan de Negocio" HeaderStyle-Width="20%"
                        ControlStyle-Width="50px">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnidproyecto" runat="server" CommandArgument='<%# Eval("Id_Proyecto") %>'
                                Text='<%# Eval("NomProyecto") %>' CommandName="rdkproyecto" ForeColor="#CC0000" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CiudadUnidad" HeaderText="Localización Unidad" SortExpression="CiudadUnidad"
                        HeaderStyle-Width="20%" />
                    <asp:BoundField DataField="Nomunidad" HeaderText="Unidad" SortExpression="Nomunidad"
                        HeaderStyle-Width="20%" />
                    <asp:BoundField DataField="Sumario" HeaderText="Descripción" HeaderStyle-Width="25%" />
                    <asp:BoundField DataField="NomEstado" HeaderText="Estado" SortExpression="NomEstado"
                        HeaderStyle-Width="10%" />
                    <asp:BoundField DataField="Asesor" HeaderText="Asesor" SortExpression="Asesor" HeaderStyle-Width="10%"
                        ConvertEmptyStringToNull="true" />
                    <asp:BoundField DataField="Lider" HeaderText="Lider" SortExpression="Lider" HeaderStyle-Width="10%"
                        ConvertEmptyStringToNull="true" />
                    <asp:BoundField DataField="CodConvocatoria" HeaderText="Número Ultima Convocatoria"
                        HeaderStyle-Width="10%" SortExpression="CodConvocatoria" />
                    <asp:BoundField DataField="N_NomConvocatoria" HeaderText="Nombre Ultima Convocatoria"
                        HeaderStyle-Width="10%" />
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlpanel5" runat="server" Visible="false">
        <asp:GridView ID="gvcontacto" runat="server" AutoGenerateColumns="false" CssClass="Grilla"
            Width="100%" OnSorting="gvcontacto_Sorting">
            <Columns>
                <asp:BoundField HeaderText="Tipo Documento" DataField="NomTipoIdentificacion" SortExpression="NomTipoIdentificacion" />
                <asp:BoundField HeaderText="Número Documento" DataField="Identificacion" SortExpression="Identificacion" />
                <asp:BoundField HeaderText="Nombres" DataField="Nombre" SortExpression="Nombre" />
                <asp:TemplateField HeaderText="Email" SortExpression="Email">
                    <ItemTemplate>
                        <asp:HyperLink ID="hl_Email" runat="server" NavigateUrl='<%# "mailto:{"+Eval("Email")+"}"  %> '
                            Text='<%# Eval("Email") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Unidad Emprendimiento" DataField="Unidad" SortExpression="Unidad" />
                <asp:BoundField HeaderText="Ciudad" DataField="ciudad" SortExpression="Ciudad" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="pnlpanel6" runat="server" Visible="false">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="Grilla"
            Width="100%" OnSorting="GridView1_Sorting">
            <Columns>
                <asp:BoundField HeaderText="Tipo Institución" DataField="NomTipoInstitucion" SortExpression="NomTipoInstitucion" />
                <asp:BoundField HeaderText="Unidad" DataField="unidad" SortExpression="unidad" />
                <asp:BoundField HeaderText="Ciudad" DataField="ciudad" SortExpression="ciudad" />
                <asp:BoundField HeaderText="Nombre Jefe Unidad" DataField="Nombre" SortExpression="Nombre" />
                <asp:TemplateField HeaderText="Email Jefe Unidad" SortExpression="Email">
                    <ItemTemplate>
                        <asp:HyperLink ID="hl_Email" runat="server" NavigateUrl='<%# "mailto:{"+Eval("Email")+"}"  %> '
                            Text='<%# Eval("Email") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Teléfono" DataField="Telefono" SortExpression="Telefono" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
