<%@ Page Language="C#" MasterPageFile="~/Master.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="CatalogoProyecto.aspx.cs" Inherits="Fonade.FONADE.PlandeNegocio.CatalogoProyecto" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="head1" ContentPlaceHolderID="head" runat="server">
    <%--<script type="text/javascript">
        function SetActiveTab(tabControl, tabNumber) {
            var ctrl = $find(tabControl);
            ctrl.set_activeTab(ctrl.get_tabs()[tabNumber]);
        }
        function cambiartab() {
            SetActiveTab('<%=tc_Emprendedor.ClientID%>', 1);
            return false;
        }
    </script>--%>
    <script type="text/javascript">
        function ValidNum(e) {
            var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
            return (tecla > 47 && tecla < 58);
        }

        function alerta() {
            return confirm('¿Está seguro que desea eliminar la actividad seleccionada?');
        }
    </script>
    <style type="text/css">
        .boton_Link001
        {
            background: none;
            border: none;
            color: white;
            border-collapse: collapse;
            text-decoration: underline;
            color: blue;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:LinqDataSource ID="lds_Proyectos" runat="server" ContextTypeName="Datos.FonadeDBDataContext"
        AutoPage="false" OnSelecting="lds_Proyectos_Selecting">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="lds_Emprendedor" runat="server" ContextTypeName="Datos.FonadeDBDataContext"
        AutoPage="false" OnSelecting="lds_Emprendedor_Selecting">
    </asp:LinqDataSource>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <h1>
        <asp:Label runat="server" ID="lbl_Titulo" /></h1>
    <asp:Panel ID="pnl_Proyectos" runat="server">
        <asp:ImageButton ID="imgBtnAdd_Emrpendedor" ImageUrl="../../../Images/icoAdicionarUsuario.gif"
            runat="server" AlternateText="image" />
        <asp:HyperLink ID="AgregarProyecto" NavigateUrl="~/FONADE/PlandeNegocio/CatalogoProyecto.aspx?Accion=Crear"
            runat="server" Text="Agregar Plan de Negocio" />
        <asp:GridView ID="gv_Proyectos" runat="server" Width="100%" AutoGenerateColumns="False"
            DataKeyNames="ID_Proyecto" CssClass="Grilla" AllowPaging="false" DataSourceID="lds_Proyectos"
            AllowSorting="True" OnRowDataBound="gv_Proyectos_RowDataBound" OnDataBound="gv_Proyectos_DataBound"
            OnRowCreated="gv_Proyectos_RowCreated" OnPageIndexChanging="gv_Proyectos_PageIndexChanged">
            <Columns>
                <asp:TemplateField HeaderText="Nombre" SortExpression="NomProyecto">
                    <ItemTemplate>
                        <asp:HyperLink ID="hl_Proyecto" runat="server" NavigateUrl='<%# "CatalogoProyecto.aspx?Accion=Editar&CodProyecto="+ Eval("ID_Proyecto") %>'
                            Text='<%# Eval("Proyecto")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Ciudad" DataField="Ciudad" SortExpression="Ciudad" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="pnl_crearEditar" runat="server">
        <asp:Table ID="tbl_Proyecto" runat="server">
            <asp:TableRow>
                <asp:TableCell>Nombre:</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="tb_Nombre" runat="server" />
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell>Descripción:</asp:TableCell><asp:TableCell>
                    <asp:TextBox ID="tb_Descripcion" TextMode="MultiLine" runat="server" />
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell>Lugar de Ejecución:</asp:TableCell><asp:TableCell>
                    <asp:DropDownList ID="ddl_depto1" runat="server" OnChange="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this.name, '', true, '', '', false, true))"
                        OnSelectedIndexChanged="ddl_depto1_SelectedIndexChanged" Width="400px" />
                    <br />
                    <asp:UpdatePanel ID="panelDropDowList" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddl_ciudad1" runat="server" Width="400px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddl_depto1" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </asp:TableCell></asp:TableRow><asp:TableRow>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Sector:</asp:TableCell><asp:TableCell>
                    <asp:DropDownList ID="ddl_depto2" runat="server" OnChange="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this.name, '', true, '', '', false, true))"
                        OnSelectedIndexChanged="ddl_depto2_SelectedIndexChanged" Width="400px" />
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddl_ciudad2" runat="server" Width="400px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddl_depto2" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </asp:TableCell></asp:TableRow></asp:Table><asp:Button ID="btn_crearActualizar" OnClick="btn_crearActualizar_onclick" runat="server"
            Text="Actualizar" />
        <ajaxToolkit:ConfirmButtonExtender Enabled="false" ID="cbe1" runat="server" DisplayModalPopupID="mpe1"
            TargetControlID="btn_crearActualizar">
        </ajaxToolkit:ConfirmButtonExtender>
        <ajaxToolkit:ModalPopupExtender ID="mpe1" runat="server" PopupControlID="pnlPopup1"
            TargetControlID="btn_crearActualizar" OkControlID="btnYes" BackgroundCssClass="modalBackground">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="pnlPopup1" runat="server" CssClass="modalPopup" Style="display: none">
            <div class="header">
                Confirmación </div><div class="body">
                <asp:Label ID="lbl_popup" runat="server" />
            </div>
            <div class="footer" align="right">
                <asp:Button ID="btnYes" runat="server" Text="Aceptar" />
            </div>
        </asp:Panel>
    </asp:Panel>
    <br />
    <br />
    <asp:UpdatePanel runat="server" ID="pnl_Emprendedor" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddl_depto1" EventName="SelectedIndexChanged" />
        </Triggers>
        <ContentTemplate>
            <asp:ImageButton ID="image_i" runat="server" ImageUrl="~/Images/icoAdicionarUsuario.gif"
                OnClick="image_i_Click" />
            <asp:LinkButton ID="sd" runat="server" Text="Agregar Emprendedor" OnClick="btn_CrearEmprendedor_Click" />
            <br />
            <br />
            <asp:GridView ID="gv_emprendedor" runat="server" Width="100%" AutoGenerateColumns="False"
                DataKeyNames="" CssClass="Grilla" AllowPaging="false" DataSourceID="lds_Emprendedor"
                AllowSorting="True" OnRowCommand="gv_emprendedor_RowCommand">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="imgBtn_EliminarEmprendedor" ImageUrl="/Images/icoBorrar.gif"
                                runat="server" CausesValidation="false" CommandName="Editar" CommandArgument='<%# Eval("CodProyecto") +";"+ Eval("Id_contacto") %>'
                                OnClientClick="return alerta();" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Nombre" SortExpression="NomProyecto">
                        <ItemTemplate>
                            <asp:LinkButton ID="hl_Emprendedor" CommandArgument='<%# Eval("CodProyecto") +";"+ Eval("Id_contacto") %>'
                                Text='<%# Eval("Nombre")%>' runat="server" CommandName="Editar" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Email" SortExpression="Email">
                        <ItemTemplate>
                            <asp:HyperLink ID="hl_Email" runat="server" NavigateUrl='<%# "mailto:{"+Eval("Email")+"}"  %> '
                                Text='<%# Eval("Email") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <br />
            <br />
            <asp:Panel ID="pnl_infoAcademica" runat="server">
                <ajaxToolkit:TabContainer ID="tc_Emprendedor" runat="server" ActiveTabIndex="0" Width="100%"
                    Height="80%">
                    <ajaxToolkit:TabPanel ID="tp_perfil" runat="server" HeaderText="Asignar Perfil" Width="100%"
                        Height="100%">
                        <ContentTemplate>
                            <asp:Panel ID="Panel1" runat="server">
                                <asp:Table ID="tbl_Convenio" runat="server">
                                    <asp:TableRow>
                                        <asp:TableCell>Nombres:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox ID="tb_nombreperfil" runat="server" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Apellidos:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox ID="tb_apellidoperfil" runat="server" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Identificación:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList runat="server" ID="ddl_Tipodocumentoperfil" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>No:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox ID="tb_nodocperfil" runat="server" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Departamento expedición:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList ID="ddl_deptoexpedicionperfil" runat="server" OnChange="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this.name, '', true, '', '', false, true))"
                                                OnSelectedIndexChanged="ddl_deptoexpedicionperfil_SelectedIndexChanged" Width="400px" />
                                            <br />
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                <ContentTemplate>
                                                    <asp:DropDownList ID="ddl_ciudadexpedicionperfil" runat="server" Width="400px" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="ddl_deptoexpedicionperfil" EventName="SelectedIndexChanged" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Correo electrónico:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox ID="tb_correoperfil" runat="server" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Género:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList runat="server" ID="ddl_generoperfil">
                                                <asp:ListItem Text="Masculino" Value="M" />
                                                <asp:ListItem Text="Femenino" Value="F" />
                                            </asp:DropDownList>
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Fecha Nacimiento:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox runat="server" ID="tb_fechanacimiento" />
                                            <asp:Image runat="server" ID="Image1" AlternateText="cal2" ImageUrl="~/images/icomodificar.gif" />
                                            <ajaxToolkit:CalendarExtender runat="server" ID="Calendarextender2" PopupButtonID="Image1"
                                                CssClass="ajax__calendar" TargetControlID="tb_fechanacimiento" Format="dd/MM/yyyy" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Departamento nacimiento:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList ID="ddl_deptonacimientoperfil" runat="server" OnChange="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this.name, '', true, '', '', false, true))"
                                                OnSelectedIndexChanged="ddl_deptonacimientoperfil_SelectedIndexChanged" Width="400px" />
                                            <br />
                                            <asp:DropDownList ID="ddl_ciudadonacimientoperfil" runat="server" Width="400px" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Teléfono:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox runat="server" ID="tb_telefonoperfil" Text="" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Nivel de estudio:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList ID="ddl_nivelestudioperfil" runat="server" AutoPostBack="true" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Programa Realizado:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox ID="tb_Programarealizadoperfil" runat="server" Enabled="false" />
                                            <asp:ImageButton ID="imbtn_institucion" ImageUrl="~/Images/icoComentario.gif" runat="server"
                                                OnClick="imbtn_institucion_Click" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Institución:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox ID="tb_Institucionperfil" runat="server" Enabled="false" />
                                            <asp:ImageButton ID="imbtn_nivel" ImageUrl="~/Images/icoComentario.gif" runat="server"
                                                OnClick="imbtn_nivel_Click" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Ciudad Institución:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox runat="server" ID="tb_ciudadinstitucionperfil" Enabled="false" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Estado:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList runat="server" ID="ddl_estadoperfil">
                                                <asp:ListItem Text="Finalizado" Value="1" />
                                                <asp:ListItem Text="Actualmente cursando" Value="0" />
                                                <asp:ListItem Text="" Value="" />
                                            </asp:DropDownList>
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Fecha inicio:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox runat="server" ID="tb_fechaInicioperfil" />
                                            <asp:Image runat="server" ID="img_dateInicio" AlternateText="cal2" ImageUrl="~/images/icomodificar.gif" />
                                            <ajaxToolkit:CalendarExtender runat="server" ID="Calendarextender1" PopupButtonID="img_dateInicio"
                                                CssClass="ajax__calendar" TargetControlID="tb_fechaInicioperfil" Format="dd/MM/yyyy" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Fecha Finalización de Materias:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox runat="server" ID="id_fechafinalizacion" />
                                            <asp:Image runat="server" ID="Image3" AlternateText="cal2" ImageUrl="~/images/icomodificar.gif" />
                                            <ajaxToolkit:CalendarExtender runat="server" ID="Calendarextender4" PopupButtonID="Image3"
                                                CssClass="ajax__calendar" TargetControlID="id_fechafinalizacion" Format="dd/MM/yyyy" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Fecha Graduación:</asp:TableCell><asp:TableCell>
                                            <asp:TextBox runat="server" ID="tb_fechagraduacionperfil" />
                                            <asp:Image runat="server" ID="Image2" AlternateText="cal2" ImageUrl="~/images/icomodificar.gif" />
                                            <ajaxToolkit:CalendarExtender runat="server" ID="Calendarextender3" PopupButtonID="Image3"
                                                CssClass="ajax__calendar" TargetControlID="tb_fechagraduacionperfil" Format="dd/MM/yyyy" />
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>¿Usted tiene alguna condición especial?:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList runat="server" ID="ddl_condicionespecialperfil">
                                                <asp:ListItem Text="Sí" Value="1" />
                                                <asp:ListItem Text="No" Value="0" />
                                            </asp:DropDownList>
                                        </asp:TableCell></asp:TableRow><asp:TableRow>
                                        <asp:TableCell>Tipo de Aprendiz:</asp:TableCell><asp:TableCell>
                                            <asp:DropDownList runat="server" ID="ddl_tipoaprendizperfil" />
                                        </asp:TableCell></asp:TableRow></asp:Table><!--<asp:CheckBox  ID="chkbx_terminarPerfil" runat="server"/>--><asp:Button ID="Button1" OnClick="tb_crearPrograma_onclick" runat="server" Text="Actualizar" />
                                <asp:Button ID="Button2" OnClick="btn_vistaprevia_perfil_onclick" Text="Vista Previa"
                                    runat="server" />
                                <asp:Button ID="Button3" Text="Crear" runat="server" OnClick="Button3_Click1" />
                            </asp:Panel>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel ID="tp_infoacademica" runat="server" HeaderText="Agregar información Académica">
                        <ContentTemplate>
                            <!--Seleccionar Información Académica.-->
                            <asp:Panel ID="pnl_Convenios" runat="server">
                                <table width="100%" border="0">
                                    <tbody>
                                        <tr>
                                            <td>
                                                Buscar </td><td colspan="28">
                                                <asp:TextBox ID="txtBusqueda" runat="server" />
                                            </td>
                                            <td>
                                                Ciudad </td><td>
                                                <asp:DropDownList ID="SelCiudad" runat="server" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btn_Buscar_Programa" Text="Buscar" runat="server" OnClick="btn_Buscar_Programa_Click" />
                                            </td>
                                            <td>
                                                &nbsp; </td><td align="right">
                                                <asp:LinkButton ID="lnkBtn_CrearPrograma" Text="Crear <br /> programa <br /> académico"
                                                    runat="server" ForeColor="Black" Style="text-decoration: none;" OnClick="lnkBtn_CrearPrograma_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbl_Pagina" Text="" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="6">
                                                <asp:LinkButton ID="lnkbtn_opcion_A" Text="A" CssClass="opcionUno" runat="server"
                                                    Style="font: bold; color: Blue; text-decoration: none;" OnClick="lnkbtn_opcion_A_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_B" Text="B" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_B_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_C" Text="C" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_C_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_D" Text="D" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_D_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_E" Text="E" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_E_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_F" Text="F" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_F_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_G" Text="G" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_G_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_H" Text="H" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_H_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_I" Text="I" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_I_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_J" Text="J" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_J_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_K" Text="K" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_K_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_L" Text="L" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_L_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_M" Text="M" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_M_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_N" Text="N" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_N_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_O" Text="O" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_O_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_P" Text="P" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_P_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_Q" Text="Q" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_Q_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_R" Text="R" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_R_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_S" Text="S" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_S_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_T" Text="T" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_T_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_U" Text="U" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_U_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_V" Text="V" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_V_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_W" Text="W" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_W_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_X" Text="X" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_X_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_Y" Text="Y" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_Y_Click" />
                                                <asp:LinkButton ID="lnkbtn_opcion_Z" Text="Z" runat="server" Style="font: bold; color: Blue;
                                                    text-decoration: none;" OnClick="lnkbtn_opcion_Z_Click" />
                                                <strong style="font: bold; color: Blue; text-decoration: none;">|</strong> <asp:LinkButton ID="lnkbtn_opcion_todos" Text="Todos" runat="server" OnClick="lnkbtn_opcion_todos_Click"
                                                    Style="font: bold; color: Blue; text-decoration: none;" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <asp:GridView ID="gv_institucion" runat="server" AutoGenerateColumns="false" CssClass="Grilla"
                                    HeaderStyle-HorizontalAlign="Left" RowStyle-HorizontalAlign="Left" Width="100%"
                                    PageSize="20" ShowHeaderWhenEmpty="true" EmptyDataText="No hay datos." AllowPaging="true"
                                    OnPageIndexChanging="gv_institucion_PageIndexChanging" OnRowDataBound="gv_institucion_RowDataBound"
                                    OnRowCommand="gv_institucion_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Programa Académico">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_nom_programa_ac" runat="server" CausesValidation="false"
                                                    CommandArgument='<%# Eval("ID_PROGRAMAACADEMICO") +";"+ Eval("NOMPROGRAMAACADEMICO") +";"+ Eval("NOMCIUDAD")+";"+ Eval("NOMINSTITUCIONEDUCATIVA") %>'
                                                    CommandName="seleccionar_1" Text='<%# Eval("NOMPROGRAMAACADEMICO")%>' /></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Institución Educativa">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_inst_educativa" runat="server" CausesValidation="false" CommandArgument='<%# Eval("ID_PROGRAMAACADEMICO") +";"+ Eval("NOMPROGRAMAACADEMICO") +";"+ Eval("NOMCIUDAD")+";"+ Eval("NOMINSTITUCIONEDUCATIVA") %>'
                                                    CommandName="seleccionar_2" Text='<%# Eval("NOMINSTITUCIONEDUCATIVA")%>' /></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ciudad">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_ciudad" runat="server" CausesValidation="false" CommandArgument='<%# Eval("ID_PROGRAMAACADEMICO") +";"+ Eval("NOMPROGRAMAACADEMICO") +";"+ Eval("NOMCIUDAD")+";"+ Eval("NOMINSTITUCIONEDUCATIVA") %>'
                                                    CommandName="seleccionar_3" Text='<%# Eval("NOMCIUDAD")%>' /></ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <br />
                                <div style="float: right; height: 50px;">
                                    <asp:Button ID="btn_volver" Text="cerrar" runat="server" OnClick="btn_volver_Click" />
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="Panel2" runat="server">
                                <!--Crear Información Académica.-->
                                <table>
                                    <tbody>
                                        <tr>
                                            <td colspan="2">
                                                Institución: </td></tr><tr>
                                            <td colspan="2">
                                                <asp:DropDownList ID="SelInstitucion" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                &nbsp; </td></tr><tr id="trOtraInstitucion" style="display: none">
                                            <td nowrap="">
                                                Otra Institución </td><td>
                                                <asp:TextBox ID="txtOtraInstitucion" runat="server" MaxLength="200" size="70" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="">
                                                Nombre del Programa </td><td>
                                                <asp:TextBox ID="txtNomPrograma" runat="server" MaxLength="200" size="70" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Departamento de institución </td><td>
                                                <asp:DropDownList ID="SelDptoExpedicion" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SelDptoExpedicion_OnSelectedIndexChanged" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Ciudad de institución </td><td>
                                                <asp:UpdatePanel ID="updt_depto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                    <ContentTemplate>
                                                        <asp:DropDownList ID="SelMunExpedicion" runat="server" />
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="SelDptoExpedicion" EventName="SelectedIndexChanged" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                <center>
                                    <asp:Button ID="btn_CrearPrograma" Text="Crear Programa" runat="server" OnClick="btn_CrearPrograma_Click" />
                                    <asp:Button ID="btn_Ocultar" Text="Ocultar" runat="server" OnClick="btn_Ocultar_Click" />
                                </center>
                            </asp:Panel>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer></asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Label ID="Lbl_Resultados" CssClass="Indicador" runat="server" /></asp:Content>
