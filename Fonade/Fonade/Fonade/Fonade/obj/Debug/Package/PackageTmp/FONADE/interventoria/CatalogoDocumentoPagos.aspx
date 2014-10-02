﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoDocumentoPagos.aspx.cs"
    Inherits="Fonade.FONADE.interventoria.CatalogoDocumentoPagos" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.9.1.js"></script>
    <link href="../../Styles/Site.css" rel="stylesheet" />
    <%--<style type="text/css">
        table
        {
            width: 100%;
        }
        table table td
        {
            width: 50%;
        }
        
        tr:first-child
        {
            text-align: left;
        }
        
        .editar
        {
            display: none;
        }
        
        .Mostrar
        {
            display: block;
        }
        
        #tableeditar
        {
            width: 50%;
        }
    </style>--%>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ContentInfo" style="width: 100%; height: auto; margin-top: 10px;">
        <asp:Panel ID="contPri" runat="server" Width="100%">
            <h1>
                <label>
                    DOCUMENTOS</label>
            </h1>
            <br />
            <table width="100%">
                <tr>
                    <td>
                        <asp:GridView ID="gvpresupuesto" runat="server" CssClass="Grilla" Width="100%" AutoGenerateColumns="False"
                            OnRowCommand="gvpresupuesto_RowCommand">
                            <Columns>
                                <asp:BoundField HeaderText="" HeaderStyle-Width="3%" ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Tipo" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hlarchivo" runat="server" NavigateUrl='<%# Eval("URL") %>' Target="_blank">
                                            <asp:Image ID="tipo" runat="server" ImageUrl='<%# "~/Images/" + Eval("icono") %>' />
                                        </asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Nombre" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Button ID="btnnombreDoc" runat="server" Text='<%# Eval("NomPagoActividadArchivo") %>'
                                            CssClass="boton_Link_Grid" CommandArgument='<%# Eval("CodPagoActividad") + ";" + Eval("NomPagoActividadarchivo") + ";" + Eval("Estado") %>'
                                            CommandName="DocEditar" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Fecha" DataField="Fecha" HeaderStyle-Width="30%" ItemStyle-HorizontalAlign="Left" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">
                        <br />
                        <asp:Button ID="btnregresar" runat="server" Text="Regresar" PostBackUrl="~/FONADE/interventoria/SeguimientoPptal.aspx" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="panelEditar" runat="server" Visible="false" CssClass="">
            <h1>
                <label>
                    EDITAR DOCUMENTO</label>
            </h1>
            <br />
            <br />
            <table id="tableeditar">
                <tr>
                    <td>
                        Nombre:
                    </td>
                    <td>
                        <asp:TextBox ID="txtNomDocumento" runat="server" Text="" Width="300px" ValidationGroup="Actualizar"></asp:TextBox>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNomDocumento"
                            ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="Actualizar">Campo Requerido</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        Categoría del documento:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddltipodocumento" runat="server" Height="16px" Width="300px"
                            ValidationGroup="Actualizar">
                        </asp:DropDownList>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddltipodocumento"
                            ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="Actualizar">Campo Requerido</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" OnClick="btnActualizar_Click"
                                        ValidationGroup="Actualizar" />
                                </td>
                                <td>
                                    <asp:Button ID="btnVolver" runat="server" Text="Regresar" OnClick="btnVolver_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
