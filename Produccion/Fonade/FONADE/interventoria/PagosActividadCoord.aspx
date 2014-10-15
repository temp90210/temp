<%@ Page Language="C#" MasterPageFile="~/Master.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="PagosActividadCoord.aspx.cs" Inherits="Fonade.FONADE.interventoria.PagosActividadCoord" %>

<asp:Content ID="head1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        table
        {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:Panel ID="pnlprincipal" runat="server">
        <h1>
            <span>APROBACION DE SOLICITUDES DE PAGO</span>
        </h1>
        <asp:GridView ID="gvsolicitudes" runat="server" CssClass="Grilla" AutoGenerateColumns="False"
            OnRowCommand="gvsolicitudes_RowCommand" OnRowDataBound="gvsolicitudes_RowDataBound"
            AllowPaging="True" OnPageIndexChanging="gvsolicitudes_PageIndexChanging" EmptyDataText="No hay Solicitudes de pago registradas">
            <Columns>
                <asp:TemplateField HeaderText="Solicitud No.">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnk_btn_Id_PagoActividad" Text='<%# Eval("Id_PagoActividad") %>'
                            runat="server" CausesValidation="false" CommandName="mostrar_coordinadorPago"
                            CommandArgument='<%# Eval("Id_PagoActividad") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Fecha">
                    <ItemTemplate>
                        <asp:Label ID="lbl_fecha" Text='<%# Eval("Fecha") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Empresa" DataField="RazonSocial" />
                <asp:TemplateField HeaderText="Agendó">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Intervemtor" runat="server" />
                        <asp:HiddenField ID="hdf_RazonSocial" runat="server" Value='<%# Eval("RazonSocial") %>' />
                        <asp:HiddenField ID="hdf_codProyecto" runat="server" Value='<%# Eval("CodProyecto") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Valor" HeaderStyle-Width="100px">
                    <ItemTemplate>
                        <asp:Label ID="lbl_valor" Text='<%# Eval("Valor") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Observación Interventor">
                    <ItemTemplate>
                        <asp:Label ID="lbl_observ_interv" Text='<%# Eval("ObservaInterventor") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Aprobado">
                    <ItemTemplate>
                        <asp:HiddenField ID="hdf_codactafonade" runat="server" Value='<%# Eval("codactafonade") %>' />
                        <asp:HiddenField ID="hdf_CodBeneficiario" runat="server" Value='<%# Eval("numIdentificacion") %>' />
                        <asp:HiddenField ID="hdf_empresa" runat="server" Value='<%# Eval("Id_Empresa") %>' />
                        <asp:Label ID="lbl_displayText" Text="" runat="server" Visible="false" />
                        <asp:RadioButtonList ID="rb_lst_aprobado" runat="server" RepeatDirection="Vertical">
                            <asp:ListItem Text="Si" Value="opcion_SI" />
                            <asp:ListItem Text="No" Value="opcion_NO" />
                            <asp:ListItem Text="Pendiente" Value="opcion_Pendiente" />
                        </asp:RadioButtonList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Observaciones">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_observ" runat="server" TextMode="MultiLine" Columns="25" Rows="10" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                No hay Solicitudes de pago registradas.</EmptyDataTemplate>
        </asp:GridView>
        <asp:Panel ID="resTotal" runat="server">
            <table>
                <tr>
                    <td style="text-align: center;">
                        <br />
                        <p>
                            Al hacer clic sobre el botón Enviar Datos, el programa le solicitará ingresar la
                            firma digital, y por medio de esta acción usted estará adquiriendo la responsabilidad
                            legal sobre los datos consignados en el formulario.
                        </p>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;">
                        <br />
                        <br />
                        <asp:Button ID="btnenviardatos" runat="server" Text="Enviar Datos" OnClick="btnenviardatos_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
    <!--Esta instrucción detecta si capicom se encuentra instalada en la máquina. Si no lo está la instala de manera automática-->
    <object id="oCAPICOM" codebase="http://download.microsoft.com/download/E/1/8/E18ED994-8005-4377-A7D7-0A8E13025B94/capicom.cab#version=2,0,0,3"
        classid="clsid:A996E48C-D3DC-4244-89F7-AFA33EC60679" viewastext>
    </object>
    <object width="0" height="0" classid="CLSID:BD31F075-A010-44FE-816D-6781BA976DDC"
        id="verifier1" codebase="RTE_3_52.cab#version=3,52,22,0">
    </object>
</asp:Content>
