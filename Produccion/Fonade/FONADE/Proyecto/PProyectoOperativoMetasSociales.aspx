<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoOperativoMetasSociales.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoOperativoMetasSociales" EnableEventValidation="true" %>

<%@ Register Src="../../Controles/CargarArchivos.ascx" TagName="CargarArchivos" TagPrefix="uc1" %>
<%@ Register Src="../../Controles/Alert.ascx" TagName="Alert" TagPrefix="uc2" %>
<%@ Register Src="../../Controles/Post_It.ascx" TagName="Post_It" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
</head>
<body style="background-color:white;background-image:none">
    <form id="form1" runat="server" style="background-color:white;background-image:none">
    <uc2:Alert ID="Alert1" runat="server" />
    <asp:Panel ID="pnlPrincipal" Visible="true" runat="server">
        <table>
            <tbody>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        ULTIMA ACTUALIZACIÓN:&nbsp;
                    </td>
                    <td>
                        <asp:Label ID="lbl_nombre_user_ult_act" Text="" runat="server" ForeColor="#CC0000" />&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="lbl_fecha_formateada" Text="" runat="server" ForeColor="#CC0000" />
                    </td>
                    <td style="width: 100px;">
                    </td>
                    <td>
                        <asp:CheckBox ID="chk_realizado" Text="MARCAR COMO REALIZADO:&nbsp;&nbsp;&nbsp;&nbsp;"
                            runat="server" TextAlign="Left" />
                        &nbsp;<asp:Button ID="btn_guardar_ultima_actualizacion" Text="Guardar" runat="server"
                            ToolTip="Guardar" OnClick="btn_guardar_ultima_actualizacion_Click" Visible="false" />
                    </td>
                </tr>
            </tbody>
        </table>
        <table id="tabla_docs" runat="server" visible="false" width="780" border="0" cellspacing="0"
            cellpadding="0">
            <tr>
                <td align="right">
                    <table width="52" border="0" cellspacing="0" cellpadding="0">
                        <tr align="center">
                            <td style="width: 50;">
                                <asp:ImageButton ID="ImageButton1" ImageUrl="../../Images/icoClip.gif" runat="server"
                                    ToolTip="Nuevo Documento" OnClick="ImageButton1_Click" />
                            </td>
                            <td style="width: 138;">
                                <asp:ImageButton ID="ImageButton2" ImageUrl="../../Images/icoClip2.gif" runat="server"
                                    ToolTip="Ver Documentos" OnClick="ImageButton2_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <table width="780" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td width="19">
                    &nbsp;
                </td>
                <td width="761">
                    <table width='95%' border='0' cellspacing='0' cellpadding='0'>
                        <tr>
                            <td colspan="2">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 50%">
                                            <div class="help_container">
                                                <div onclick="textoAyuda({titulo: 'Metas Sociales del Plan de Negocio', texto: 'MetaSocial'});">
                                                    <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasAprovisionamiento" />
                                                </div>
                                                <div>
                                                    Metas Sociales del Plan de Negocio
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            <div id="div_Post_It_1" runat="server" visible="false">
                                                <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="MetaSocial" _txtTab="1" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <table border="0">
                                    <tr>
                                        <td width='18' align='left'>
                                            <div class="help_container">
                                                <div onclick="textoAyuda({titulo: 'Plan Nacional de Desarrollo', texto: 'PlanNacional'});">
                                                    <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            Plan Nacional de Desarrollo
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtPlanNacional" runat="server" Width="810px" Height="100px" TextMode="MultiLine"
                                    MaxLength="450" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <table border="0">
                                    <tr>
                                        <td width='18' align='left'>
                                            <div class="help_container">
                                                <div onclick="textoAyuda({titulo: 'Plan Regional de Desarrollo', texto: 'PlanRegional'});">
                                                    <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                                </div>
                                            </div>
                                        </td>
                                        <td class='Titulo'>
                                            Plan Regional de Desarrollo
                                        </td>
                                    </tr>
                                </table>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtPlanRegional" runat="server" Width="810px" Height="100px" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <table border="0">
                                    <tr>
                                        <td width='18' align='left'>
                                            <a href="#">
                                                <div class="help_container">
                                                    <div onclick="textoAyuda({titulo: 'Cluster o Cadena Productiva', texto: 'Cluster'});">
                                                        <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                                    </div>
                                                </div>
                                        </td>
                                        <td class='Titulo'>
                                            Cluster o Cadena Productiva
                                        </td>
                                    </tr>
                                </table>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtCluster" runat="server" Width="810px" Height="100px" TextMode="MultiLine" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <table border="0">
                                    <tr>
                                        <td width='18' align='left'>
                                            <a href="#">
                                                <div class="help_container">
                                                    <div onclick="textoAyuda({titulo: 'Empleo', texto: 'Empleo'});">
                                                        <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                                    </div>
                                                </div>
                                        </td>
                                        <td width='350' class='Titulo'>
                                            Empleo
                                        </td>
                                    </tr>
                                </table>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td colspan="2">
                                <table id="tabla_1" runat="server" width='850px' border='0' cellspacing='1' cellpadding='4'>
                                    <tr bgcolor='#3D5A87'>
                                        <td style="color: White;" align="center">
                                            Empleos Directos
                                        </td>
                                        <td colspan="2">
                                            &nbsp;
                                        </td>
                                        <td align="center" style="color: White;">
                                            Jovenes
                                        </td>
                                        <td colspan="7" align="center" style="color: White;">
                                            Población Vulnerable
                                        </td>
                                    </tr>
                                    <tr bgcolor='#3D5A87'>
                                        <td style="color: White; width: 140px; font-size: 10px">
                                            Cargo
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Sueldo Mes
                                        </td>
                                        <td style="color: White; width: 100px; font-size: 10px">
                                            Generado en el Primer Año
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Edad entre 18 y 24 años
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Desplazado por la violencia
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Madre Cabeza de Familia
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Minoría Etnica (Indigena o Negritud)
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Recluido Carceles INPEC
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Desmovilizado o Reinsertado
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Discapacitado
                                        </td>
                                        <td style="color: White; width: 60px; font-size: 10px">
                                            Desvinculado de Entidades del Estado
                                        </td>
                                    </tr>
                                </table>
                                <asp:Label ID="Label1" runat="server" Text="Personal Calificado" CssClass="TitulosRegistrosGrilla"></asp:Label>
                                <asp:GridView ID="gw_Empleos" runat="server" Width="850px" AutoGenerateColumns="false"
                                    CssClass="Grilla" ShowHeader="false">
                                    <Columns>
                                        <asp:BoundField DataField="Cargo" HeaderText="Cargo" ItemStyle-Width="140px" />
                                        <asp:BoundField DataField="ValorMensual" HeaderText="Sueldo Mes" ItemStyle-Width="60px" />
                                        <asp:TemplateField HeaderText="Generado en el Primer Año" ItemStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="txtCodCargo" runat="server" Value='<%#Eval("IdCargo")%>' />
                                                <asp:DropDownList ID="ddlGeneradoMes" runat="server" SelectedValue='<%#Eval("GeneradoPrimerAnio")%>'>
                                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                                    <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                                    <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                                    <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                                    <asp:ListItem Value="6" Text="6"></asp:ListItem>
                                                    <asp:ListItem Value="7" Text="7"></asp:ListItem>
                                                    <asp:ListItem Value="8" Text="8"></asp:ListItem>
                                                    <asp:ListItem Value="9" Text="9"></asp:ListItem>
                                                    <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                                    <asp:ListItem Value="11" Text="11"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Edad entre 18 y 24 años">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkEdad18_24" Checked='<%# Eval("EsJoven") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desplazado por la violencia">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDesplazado" Checked='<%# Eval("EsDesplazado") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Madre Cabeza de Familia">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkMadreCabeza" Checked='<%# Eval("EsMadre") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Minoría Etnica (Indigena o Negritud)	">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkMinoriaEtnica" Checked='<%# Eval("EsMinoria") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Recluido Carceles INPEC">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkRecluidoCarceles" Checked='<%# Eval("EsRecluido") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desmovilizado o Reinsertado">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDesmovilizado" Checked='<%# Eval("EsDesmovilizado") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discapacitado">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDiscapacitado" Checked='<%# Eval("EsDiscapacitado") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desvinculado de Entidades del Estado">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDesvinculado" Checked='<%# Eval("EsDesvinculado") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:Label runat="server" Text="Mano de Obra Directa" CssClass="TitulosRegistrosGrilla"></asp:Label>
                                <asp:GridView ID="gw_ManoObra" runat="server" Width="850px" AutoGenerateColumns="false"
                                    CssClass="Grilla" ShowHeader="false">
                                    <Columns>
                                        <asp:BoundField DataField="Cargo" HeaderText="Cargo" ItemStyle-Width="140px" />
                                        <asp:TemplateField HeaderText="Sueldo" ItemStyle-Width="60px">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="txtSueldo" value='<%# Eval("ValorMensual") %>' MaxLength="8"
                                                    Width="50px" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Generado en el Primer Año" ItemStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="txtCodInsumo" runat="server" Value='<%#Eval("IdCargo")%>' />
                                                <asp:DropDownList ID="ddlGeneradoMes" runat="server" SelectedValue='<%#Eval("GeneradoPrimerAnio")%>'>
                                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Mes 1"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Mes 2"></asp:ListItem>
                                                    <asp:ListItem Value="3" Text="Mes 3"></asp:ListItem>
                                                    <asp:ListItem Value="4" Text="Mes 4"></asp:ListItem>
                                                    <asp:ListItem Value="5" Text="Mes 5"></asp:ListItem>
                                                    <asp:ListItem Value="6" Text="Mes 6"></asp:ListItem>
                                                    <asp:ListItem Value="7" Text="Mes 7"></asp:ListItem>
                                                    <asp:ListItem Value="8" Text="Mes 8"></asp:ListItem>
                                                    <asp:ListItem Value="9" Text="Mes 9"></asp:ListItem>
                                                    <asp:ListItem Value="10" Text="Mes 10"></asp:ListItem>
                                                    <asp:ListItem Value="11" Text="Mes 11"></asp:ListItem>
                                                    <asp:ListItem Value="12" Text="Mes 12"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Edad entre 18 y 24 años">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkEdad18_24" Checked='<%# Eval("EsJoven")  %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desplazado por la violencia">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDesplazado" Checked='<%# Eval("EsDesplazado") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Madre Cabeza de Familia">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkMadreCabeza" Checked='<%# Eval("EsMadre") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Minoría Etnica (Indigena o Negritud)	">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkMinoriaEtnica" Checked='<%# Eval("EsMinoria")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Recluido Carceles INPEC">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkRecluidoCarceles" Checked='<%# Eval("EsRecluido")  %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desmovilizado o Reinsertado">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDesmovilizado" Checked='<%# Eval("EsDesmovilizado")  %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discapacitado">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDiscapacitado" Checked='<%# Eval("EsDiscapacitado")  %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desvinculado de Entidades del Estado">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkDesvinculado" Checked='<%# Eval("EsDesvinculado")  %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <br />
                                Empleos Indirectos:
                                <asp:TextBox runat="server" ID="txtEmpleosIndirectos" Width="30px" />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td colspan="2">
                                <table border="0">
                                    <tr>
                                        <td width='18' align='left'>
                                            <a href="#">
                                                <div class="help_container">
                                                    <div onclick="textoAyuda({titulo: 'Emprendedores', texto: 'Emprendedores'});">
                                                        <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                                    </div>
                                                </div>
                                        </td>
                                        <td class='Titulo'>
                                            Emprendedores
                                        </td>
                                    </tr>
                                </table>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td colspan="2">
                                <asp:GridView ID="gw_emprendedores" runat="server" Width="600px" AutoGenerateColumns="false" CssClass="Grilla"
                                    RowStyle-Height="35px" DataKeyNames="Id_Contacto" OnRowDataBound="gw_emprendedores_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="nombres" HeaderText="Nombre" />
                                        <asp:TemplateField HeaderText="Beneficiario del Fondo emprender" ItemStyle-HorizontalAlign="Center"
                                            HeaderStyle-Width="70" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkBeneficiario" Checked='<%# Eval("Beneficiario") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Participación Accionaria (%)" HeaderStyle-Width="70"
                                            HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtParticipacion" runat="server" Text='<%# Eval("Participacion") %>'
                                                    Style="text-align: right;" Width="71px" />
                                                <asp:Label ID="lblParticipacion" runat="server" Text='<%# Eval("Participacion") %>'
                                                    Visible="false"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="right">
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click"
                                    Visible="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
    </asp:Panel>
    </form>
</body>
</html>
