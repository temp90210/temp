<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrameAspectos.aspx.cs" Inherits="Fonade.FONADE.Convocatoria.FrameAspectos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        table {
            width:100%;
        }
        td {
            width: 50%;
        }
        .auto-style1 {
            width: 26%;
        }
        .sinlinea {
            border:none;
            border-collapse:collapse;
            border-bottom-color:none;
        }
    </style>


    <%--<link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />--%>
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>

    <script type="text/javascript">
        function closeWindow() {
            window.parent.opener.location.reload();
            window.parent.opener.focus();
            window.parent.close();
        }
    </script>
</head>
<body style="width:900px; height:650px">
    <form id="form1" runat="server" style="width:900px; height:650px">
    <div style="width:900px; height:650px">
        <table>
            <tr>
                <td class="auto-style1">
                    <div style="overflow-y:scroll;height:650px;width:300px; ">
                        <div style="text-align:center;">
                            Seleccione de la lista los aspectos que quiere incluir en la evaluación.
                            <br />
                            <asp:Button ID="btnexpan" runat="server" Text="Expandir todo" OnClick="btnexpan_Click" />
                        </div>
                        <br />
                        <asp:TreeView ID="tv_aspectos" runat="server" CssClass="Grilla" Width="300px" OnSelectedNodeChanged="tv_aspectos_SelectedNodeChanged">
                        </asp:TreeView>
                        <br />
                        <div style="text-align:center;">
                            <table>
                                <tr>
                                    <td><asp:Button ID="btnCerrar" runat="server" Text="Cerrar" OnClientClick="return closeWindow()" /></td>
                                    <td><asp:Button ID="btnAceptar" runat="server" Text="Aceptar" OnClick="btnAceptar_Click" /></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td>
                <td style="vertical-align:top;">
                    <asp:Panel ID="pnlinfonodo" runat="server" Width="100%">
                        <h1>EDITAR ASPECTO</h1>
                        <br />
                        <table>
                            <tr>
                                <td colspan="2">Descripción:</td>
                            </tr>
                            <tr>
                                <td style="text-align:center;" colspan="2">
                                    <asp:TextBox ID="txtdescripcion" runat="server" TextMode="MultiLine" Height="100px" Width="400px" ValidationGroup="actualizar" MaxLength="350"></asp:TextBox>
                                    <br />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtdescripcion" ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="actualizar">Cmpo reuerido</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Estado:</td>
                                <td>
                                    <asp:DropDownList ID="ddlestado" runat="server" Height="16px" Width="150px" ValidationGroup="actualizar">
                                        <asp:ListItem Value="0">Activo</asp:ListItem>
                                        <asp:ListItem Value="1">Inactivo</asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlestado" ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="actualizar">Cmpo reuerido</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2"></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Image ID="imgaspectoAgr" runat="server" ImageUrl="~/Images/add.png" Visible="false" />
                                    <asp:LinkButton ID="lnkadicionar" runat="server" OnClick="lnkadicionar_Click" Text="" Visible="false"></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:GridView ID="gv_campos" runat="server" CssClass="Grilla" AutoGenerateColumns="false" OnRowCommand="gv_campos_RowCommand">
                                       <Columns>
                                           <asp:TemplateField>
                                               <ItemTemplate>
                                                   <asp:LinkButton ID="lnkeliminar" runat="server" CommandName="eliminar" CommandArgument='<%# Eval("id_Campo") %>' Text="" OnClientClick="return confirm('Al borrar un Aspecto: este desaparecerá de los demás que lo requieren\n¿Esta seguro que desea borrar el insumo seleccionado?')" CssClass="sinlinea">
                                                        <asp:Image ID="imgeliminar" runat="server" ImageUrl="~/Images/icoBorrar.gif"  CssClass="sinlinea" />
                                                    </asp:LinkButton>
                                               </ItemTemplate>
                                           </asp:TemplateField>
                                           <asp:TemplateField HeaderText="Descripción">
                                               <ItemTemplate>
                                                   <asp:LinkButton ID="btnVerIDCampo" runat="server" CommandArgument='<%# Eval("id_Campo") %>' CommandName="ver" Text='<%# Eval("Campo1") %>'>

                                                   </asp:LinkButton>
                                               </ItemTemplate>
                                           </asp:TemplateField>
                                           <asp:BoundField HeaderText="Estado" DataField="activo" />
                                       </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align:center;" colspan="2">
                                    <br />
                                    <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" OnClick="btnActualizar_Click" ValidationGroup="actualizar" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlAgregarCampo" runat="server">
                        <h1><asp:Label ID="lbltituloAgregar" runat="server" Text=""></asp:Label></h1>
                        <br />
                        <table>
                            <tr>
                                <td colspan="2">Descripción:</td>
                            </tr>
                            <tr>
                                <td style="text-align:center;" colspan="2">
                                    <asp:TextBox ID="txtnombrecampo" runat="server" TextMode="MultiLine" Height="100px" Width="400px" ValidationGroup="crear" MaxLength="350"></asp:TextBox>
                                    <br />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtnombrecampo" ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="crear">Cmpo reuerido</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Estado:</td>
                                <td>
                                    <asp:DropDownList ID="ddlnuevoactivo" runat="server" Height="16px" Width="150px" ValidationGroup="crear">
                                        <asp:ListItem Value="0">Activo</asp:ListItem>
                                        <asp:ListItem Value="1">Inactivo</asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ddlnuevoactivo" ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="crear">Cmpo reuerido</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2"></td>
                            </tr>
                            <tr>
                                <td style="text-align:center;" colspan="2">
                                    <br />
                                    <asp:Button ID="btnAgregarCampo" runat="server" Text="Crear" OnClick="btnAgregarCampo_Click" ValidationGroup="crear" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
