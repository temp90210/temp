<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoMercadoProyecciones.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoMercadoProyecciones" %>

<%@ Register Src="../../Controles/Post_It.ascx" TagName="Post_It" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <!-- Estilos específicos para frames -->
    <link href="../../Styles/EstilosEspecificos.css" rel="stylesheet" />
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            background-color: white;
        }
        .sinlinea
        {
            border: none;
            border-collapse: collapse;
        }
        .MsoNormal
        {
            margin: 0cm 0cm 0pt 0cm !important;
            padding: 5px 15px 0px 15px;
        }
        .MsoNormalTable
        {
            margin: 6px 0px 4px 8px !important;
        }
        .childContainer
        {
            width: 100%;
            height: auto;
        }
        html, body, div, iframe
        {
            /*height: 13% !important;*/
        }
    </style>
    <script type="text/ecmascript">
        function url() {
            open("../Ayuda/Mensaje.aspx", "Proyección de ventas", "width=500,height=200");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <%--<div>
        <%= obtenerUltimaActualizacion(txtTab, codProyecto) %>
    </div>--%>
    <table width="780" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td width="12">
                    <span style="width: 12; height: 28;" />
                </td>
                <td width="150">
                    ULTIMA ACTUALIZACIÓN:
                </td>
                <td width="380">
                    <asp:Label ID="lbl_nombre_user_ult_act" Text="" runat="server" ForeColor="#CC0000" />&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lbl_fecha_formateada" Text="" runat="server" ForeColor="#CC0000" />
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
                            <asp:ImageButton ID="ImageButton11" ImageUrl="../../Images/icoClip.gif" runat="server"
                                OnClick="ImageButton11_Click" />
                        </td>
                        <td style="width: 138;">
                            <asp:ImageButton ID="ImageButton22" ImageUrl="../../Images/icoClip2.gif" runat="server"
                                OnClick="ImageButton22_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <div>
        <table style="width: 100%;">
            <tr>
                <td style="width: 100%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Proyección de ventas', texto: 'ProyeccionVentas'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasAprovisionamiento" />
                        </div>
                        &nbsp;<div>
                            Proyección de ventas:
                        </div>
                    </div>
                </td>
                <td style="width: 100%">
                    <div id="div_post_it_1" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="ProyeccionVentas" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <table width="98%" border="0" cellspacing="1" cellpadding="4">
            <tbody>
                <tr>
                    <td>
                        <asp:Label ID="L_FechIniPro" runat="server" Text="Fecha de Inicio del Proyecto" />
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="DDL_Dia" runat="server" ValidationGroup="grabar">
                            <asp:ListItem Value="01">01</asp:ListItem>
                            <asp:ListItem Value="02">02</asp:ListItem>
                            <asp:ListItem Value="03">03</asp:ListItem>
                            <asp:ListItem Value="04">04</asp:ListItem>
                            <asp:ListItem Value="05">05</asp:ListItem>
                            <asp:ListItem Value="06">06</asp:ListItem>
                            <asp:ListItem Value="07">07</asp:ListItem>
                            <asp:ListItem Value="08">08</asp:ListItem>
                            <asp:ListItem Value="09">09</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                            <asp:ListItem Value="12">12</asp:ListItem>
                            <asp:ListItem Value="13">13</asp:ListItem>
                            <asp:ListItem Value="14">14</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                            <asp:ListItem Value="16">16</asp:ListItem>
                            <asp:ListItem Value="17">17</asp:ListItem>
                            <asp:ListItem Value="18">18</asp:ListItem>
                            <asp:ListItem Value="19">19</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="21">21</asp:ListItem>
                            <asp:ListItem Value="22">22</asp:ListItem>
                            <asp:ListItem Value="23">23</asp:ListItem>
                            <asp:ListItem Value="24">24</asp:ListItem>
                            <asp:ListItem Value="25">25</asp:ListItem>
                            <asp:ListItem Value="26">26</asp:ListItem>
                            <asp:ListItem Value="27">27</asp:ListItem>
                            <asp:ListItem Value="28">28</asp:ListItem>
                            <asp:ListItem Value="29">29</asp:ListItem>
                            <asp:ListItem Value="30">30</asp:ListItem>
                            <asp:ListItem Value="31">31</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;
                        <asp:DropDownList ID="DDL_Mes" runat="server" ValidationGroup="grabar">
                            <asp:ListItem Value="01">Ene</asp:ListItem>
                            <asp:ListItem Value="02">Feb</asp:ListItem>
                            <asp:ListItem Value="03">Mar</asp:ListItem>
                            <asp:ListItem Value="04">Abr</asp:ListItem>
                            <asp:ListItem Value="05">May</asp:ListItem>
                            <asp:ListItem Value="06">Jun</asp:ListItem>
                            <asp:ListItem Value="07">Jul</asp:ListItem>
                            <asp:ListItem Value="08">Ago</asp:ListItem>
                            <asp:ListItem Value="09">Sep</asp:ListItem>
                            <asp:ListItem Value="10">Oct</asp:ListItem>
                            <asp:ListItem Value="11">Nov</asp:ListItem>
                            <asp:ListItem Value="12">Dic</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;
                        <asp:DropDownList ID="DD_Anio" runat="server" ValidationGroup="grabar">
                            <asp:ListItem Value="2004">2004</asp:ListItem>
                            <asp:ListItem Value="2005">2005</asp:ListItem>
                            <asp:ListItem Value="2006">2006</asp:ListItem>
                            <asp:ListItem Value="2007">2007</asp:ListItem>
                            <asp:ListItem Value="2008">2008</asp:ListItem>
                            <asp:ListItem Value="2009">2009</asp:ListItem>
                            <asp:ListItem Value="2010">2010</asp:ListItem>
                            <asp:ListItem Value="2011">2011</asp:ListItem>
                            <asp:ListItem Value="2012">2012</asp:ListItem>
                            <asp:ListItem Value="2013">2013</asp:ListItem>
                            <asp:ListItem Value="2014">2014</asp:ListItem>
                            <asp:ListItem Value="2015">2015</asp:ListItem>
                            <asp:ListItem Value="2016">2016</asp:ListItem>
                            <asp:ListItem Value="2017">2017</asp:ListItem>
                            <asp:ListItem Value="2018">2018</asp:ListItem>
                            <asp:ListItem Value="2019">2019</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="L_Tamperi" runat="server" Text="Tamaño del Período" />
                    </td>
                    <td>
                        <asp:DropDownList ID="DD_Periodo" runat="server" ValidationGroup="grabar">
                            <asp:ListItem Value="1">Mes</asp:ListItem>
                            <asp:ListItem Value="2">Bimestre</asp:ListItem>
                            <asp:ListItem Value="3">Trimestre</asp:ListItem>
                            <asp:ListItem Value="4">Semestre</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label ID="L_TiemPro" runat="server" Text="Tiempo de Proyección" />
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList1" runat="server" ValidationGroup="grabar" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="L_MetPro" runat="server" Text="Método de Proyección" />
                    </td>
                    <td>
                        <asp:DropDownList ID="DD_MetProy" runat="server" ValidationGroup="grabar" OnSelectedIndexChanged="DD_MetProy_SelectedIndexChanged">
                            <asp:ListItem Value="Lineal">Lineal</asp:ListItem>
                            <asp:ListItem Value="Exponencial">Exponencial</asp:ListItem>
                            <asp:ListItem Value="Logarítmico">Logarítmico</asp:ListItem>
                            <asp:ListItem Value="Promedios Móviles">Promedios Móviles</asp:ListItem>
                            <asp:ListItem Value="Promedios Móviles Suavizados">Promedios Móviles Suavizados</asp:ListItem>
                            <asp:ListItem Value="Sistema Winters">Sistema Winters</asp:ListItem>
                            <asp:ListItem Value="Otro">Otro</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td colspan="2" id="td_otroMedio" runat="server" visible="false">
                        <asp:TextBox ID="OtroMetodo" runat="server" size="30" MaxLength="30" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="L_CostoVenta" runat="server" Text="Costo de Venta" />
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="TB_CostoVenta" runat="server" Width="368px" ValidationGroup="grabar"
                            MaxLength="100" />&nbsp;
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TB_CostoVenta"
                            ErrorMessage="*" ForeColor="Red" ValidationGroup="grabar" />
                    </td>
                </tr>
            </tbody>
        </table>
        <table style="width: 100%">
            <tr>
                <td style="width: 100%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Justificación de Proyección de Ventas', texto: 'JustificaProyeccion'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasAprovisionamiento" />&nbsp;Justificación
                            de Proyección de Ventas:
                        </div>
                    </div>
                </td>
                <td style="width: 100%">
                    <div id="div_post_it_2" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It2" runat="server" _txtCampo="JustificaProyeccion" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_JusProVen" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="TB_JusProVen" runat="server" TextMode="MultiLine" Height="200px"
            ValidationGroup="grabar" CausesValidation="false" Width="100%" />
        <ajaxToolkit:HtmlEditorExtender ID="HEE_JusProVen" runat="server" Enabled="true"
            TargetControlID="TB_JusProVen" />
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Política de Cartera', texto: 'PoliticaCartera'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasAprovisionamiento" />&nbsp;Política
                            de Cartera:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_3" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It3" runat="server" _txtCampo="PoliticaCartera" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_PoliCarte" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="TB_PoliCarte" runat="server" TextMode="MultiLine" Width="100%" Height="200px"
            ValidationGroup="grabar" />
        <ajaxToolkit:HtmlEditorExtender ID="HEE_PoliCarte" runat="server" Enabled="true"
            TargetControlID="TB_PoliCarte" />
        <br />
        <asp:Button ID="B_Guardar" runat="server" Text="Guardar" OnClick="B_Guardar_Click"
            ValidationGroup="grabar" Visible="False" />
        <br />
        <br />
        <asp:ImageButton ID="IB_AgregarProductoServicio" runat="server" ImageUrl="~/Images/icoAdicionarUsuario.gif"
            OnClick="IB_AgregarProductoServicio_Click" Visible="False" />&nbsp;<asp:LinkButton
                ID="B_AgregarProductoServicio" runat="server" Text="Adicionar Producto o Servicio"
                Font-Bold="true" OnClick="B_AgregarProductoServicio_Click" Visible="False" />
        <asp:Panel ID="pnl_Datos" runat="server" Width="100%">
            <asp:GridView ID="GV_productoServicio" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                CellSpacing="1" CssClass="Grilla" DataKeyNames="Id_Producto" EmptyDataText="No hay datos."
                OnRowDataBound="GV_productoServicio_RowDataBound" Width="100%">
                <HeaderStyle HorizontalAlign="Left" />
                <Columns>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="3%" ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" OnClientClick="return confirm('Esta seguro que desea borrar el producto seleccionado?');"
                                Text="">
                                <asp:Image ID="I_imagen" runat="server" ImageUrl="~/Images/icoBorrar.gif" CssClass="sinlinea" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Producto o Servicio" ItemStyle-HorizontalAlign="Left"
                        ItemStyle-Width="38%">
                        <ItemTemplate>
                            <asp:LinkButton ID="LB_ProductoServicio" runat="server" OnClick="LB_ProductoServicio_Click"
                                Text='<%# Eval("NomProducto") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Id_Producto" HeaderText="Id_Producto" ItemStyle-HorizontalAlign="Right"
                        ItemStyle-Width="12%" Visible="false" />
                    <asp:BoundField DataField="PosicionArancelaria" HeaderText="Posición Arancelaria"
                        ItemStyle-HorizontalAlign="Right" ItemStyle-Width="12%" />
                    <asp:BoundField DataField="PorcentajeRetencion" HeaderText="RTF" ItemStyle-HorizontalAlign="Right"
                        ItemStyle-Width="5%" />
                    <asp:BoundField DataField="PorcentajeIva" HeaderText="IVA" ItemStyle-HorizontalAlign="Right"
                        ItemStyle-Width="5%" />
                    <asp:TemplateField HeaderText="Precio Inicial" ItemStyle-HorizontalAlign="Right"
                        ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="lbl_PrecioInicial" runat="server" Text='<%#String.Format("{0:N2}", Eval("PrecioLanzamiento") ) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PorcentajeVentasContado" HeaderText="%Contado" ItemStyle-HorizontalAlign="Right"
                        ItemStyle-Width="10%" />
                    <asp:BoundField DataField="PorcentajeVentasPlazo" HeaderText="%Crédito" ItemStyle-HorizontalAlign="Right"
                        ItemStyle-Width="10%" />
                    <asp:TemplateField HeaderText="Insumo" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:LinkButton ID="LB_Insumo" runat="server" OnClick="LB_Insumo_Click" Text="IR" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <br />
        </asp:Panel>
    </div>
    <br />
    <br />
    <br />
    </form>
</body>
</html>
