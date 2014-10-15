<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdicionarVisita.aspx.cs"
    Inherits="Fonade.FONADE.interventoria.AdicionarVisita" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Agendar Visita</title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.9.1.js"></script>
    <link href="../../Styles/Site.css" rel="stylesheet" />
    <script src="../../Scripts/ScriptsGenerales.js"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
        table
        {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ContentInfo" style="width: 995px; height: auto;">
        <ajaxToolkit:ToolkitScriptManager ID="tk_1" runat="server">
        </ajaxToolkit:ToolkitScriptManager>
        <asp:UpdatePanel ID="updt_add_visita" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnl_tarea_agendada" runat="server" Visible="true">
                    <h1>
                        <label>
                            AGENDAR VISITA</label>
                    </h1>
                    <br />
                    <br />
                    <table>
                        <tr>
                            <td>
                                Nombre:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlempresa" runat="server" Width="450px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Fecha Inicio:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblFechaInicio" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Fecha Fin:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblFechaFin" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Nit:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblnitempresa" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Ciudad:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblciudad" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Objeto:
                                        </td>
                                        <td>
                                            <asp:Label ID="lblobjeto" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnl_tarea_a_crear" runat="server" Visible="false">
                    <h1>
                        AGENDAR VISITA
                    </h1>
                    <table>
                        <tr>
                            <td>
                                Nombre:
                                <asp:DropDownList ID="DD_Empresas" runat="server" AutoPostBack="true" Width="450px"
                                    OnSelectedIndexChanged="DD_Empresas_SelectedIndexChanged" />
                                <br />
                                <br />
                                Nit:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:TextBox ID="TXT_nit" runat="server" Enabled="false" Width="119px" Height="18px" />
                                <br />
                                <br />
                                Objeto:&nbsp;
                                <asp:TextBox ID="TXT_objeto" runat="server" Width="269px" Height="18px" />
                            </td>
                            <td>
                                Fecha Inicio:
                                <br />
                                <asp:TextBox runat="server" ID="txtDate" Text="" ValidationGroup="crearTarea" Enabled="False" />
                                <asp:Image runat="server" ID="btnDate" AlternateText="cal1" ImageUrl="~/images/icomodificar.gif" />
                                <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender1" PopupButtonID="btnDate"
                                    CssClass="ajax__calendar" TargetControlID="txtDate" Format="dd/MMM/yyyy" />
                                <br />
                                <br />
                                Ciudad:
                                <br />
                                <asp:TextBox ID="TXT_ciudad" runat="server" Enabled="false" />
                            </td>
                            <td style="padding-top: 10px;">
                                Fecha Fin:
                                <br />
                                <asp:HiddenField ID="hdt_t2_NumVisita" runat="server" />
                                <asp:TextBox runat="server" ID="txtDate2" Text="" ValidationGroup="crearTarea" Enabled="False" />
                                <asp:Image runat="server" ID="btnDate2" AlternateText="cal2" ImageUrl="~/images/icomodificar.gif" />
                                <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender2" PopupButtonID="btnDate2"
                                    CssClass="ajax__calendar" TargetControlID="txtDate2" Format="dd/MMM/yyyy" />
                                <br />
                                <br />
                                <br />
                                <%--Ciudad:--%>
                                <br />
                                <br />
                                <input type="text" name="name" value="" readonly disabled style="display: none;" />
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="pnl_btns" runat="server" HorizontalAlign="Center">
                        <asp:Button ID="btn_agendar" Text="Agendar" runat="server" OnClick="btn_agendar_Click" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="Button2" Text="Cerrar" runat="server" OnClientClick="window.close();" />
                    </asp:Panel>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
