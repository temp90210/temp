<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SeguimientoPptal.aspx.cs"
    Inherits="Fonade.FONADE.interventoria.SeguimientoPptal" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <!--<script src="../../Scripts/jquery-1.9.1.js"></script>-->
    <link href="../../Styles/Site.css" rel="stylesheet" />
    <script src="../../Scripts/ScriptsGenerales.js"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
        table
        {
            width: 100%;
        }
        table table td
        {
            width: 50%;
        }
        table.Grilla th{
            text-align: center;
        }
        .valor{
            text-align: right;
        }
        tr:first-child
        {
            text-align: left;
        }
        .help{
            width: 100%;
            height: 30px;
        }
    </style>
</head>
<body style="overflow-x: hidden;">
    <form id="form1" runat="server">
    <div class="ContentInfo" style="width: 995px; height: auto;">
        <div class="help">

        </div>
        <table>
            <tr>
                <td colspan="2">
                    <div class="help_container">
                        
                        <div>
                            <strong>Seguimiento Presupuestal</strong>
                        </div>
                        <div onclick="textoAyuda({titulo: 'Presupuesto', texto: 'Presupuesto'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;" colspan="2">
                    <table>
                        <tr>
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Presupuesto Recomendado por Fondo Emprender:
                            </td>
                            <td style="text-align: center;">
                                <asp:Label ID="lblemprender" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Presupuesto Aprobado por Interventoria:
                            </td>
                            <td style="text-align: center;">
                                <asp:Label ID="lblinterventoria" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Presupuesto Disponible:
                            </td>
                            <td style="text-align: center;">
                                <asp:Label ID="lbldisponible" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <br />
                    <div style="overflow: scroll; width: 900px; height: 570px !important;">
                        <asp:GridView ID="gvpresupuesto" runat="server" CssClass="Grilla" AutoGenerateColumns="False"
                            OnRowCommand="gvpresupuesto_RowCommand">
                            <Columns>
                                <asp:BoundField HeaderText="Id" DataField="Id_PagoActividad" ></asp:BoundField>
                                <asp:BoundField HeaderText="Nombre" DataField="NomPagoActividad" />
                                <asp:BoundField HeaderText="Fecha Interventor" DataField="FechaInterventor"/>
                                <asp:BoundField HeaderText="Valor" DataField="CantidadDinero" DataFormatString="{0:C}"/>
                                <asp:BoundField HeaderText="Estado" DataField="Estado" />
                                <asp:TemplateField HeaderText="Beneficiario">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("NomTipoIdentificacion") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("NomTipoIdentificacion") + " No. " + Eval("NumIdentificacion") + "<br>" + Eval("Nombre") + " " + Eval("Apellido") + " - " + Eval("RazonSocial") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Fecha Respuesta" DataField="FechaRtaFA" DataFormatString="{0:g}"
                                    HtmlEncode="false" />
                                <asp:BoundField HeaderText="Valor ReteFuente" DataField="ValorReteFuente" DataFormatString="${0}">
                                    <ItemStyle CssClass="valor"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Valor ReteIVA" DataField="ValorReteIVA" DataFormatString="${0}"><ItemStyle CssClass="valor"/></asp:BoundField>
                                <asp:BoundField HeaderText="Valor ReteICA" DataField="ValorReteICA" DataFormatString="${0}"><ItemStyle CssClass="valor"/></asp:BoundField>
                                <asp:BoundField HeaderText="Otros Descuentos" DataField="OtrosDescuentos" DataFormatString="${0}"><ItemStyle CssClass="valor"/></asp:BoundField>
                                <asp:BoundField HeaderText="Valor Pagado" DataField="ValorPagado" DataFormatString="${0}"><ItemStyle CssClass="valor"/></asp:BoundField>
                                <asp:BoundField HeaderText="Codigo Pago" DataField="CodigoPago" DataFormatString="${0}"><ItemStyle CssClass="valor"/></asp:BoundField>
                                <asp:TemplateField HeaderText="Anexos del Pago">
                                    <ItemTemplate>
                                        <asp:Button ID="btnverdocumentsopagos" runat="server" Text="Ver" CommandArgument='<%# Eval("Id_PagoActividad") %>'
                                            CssClass="boton_Link_Grid" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Observaciones Fiduciaria" DataField="ObservacionesFA" />
                            </Columns>
                        </asp:GridView>
                        <br />
                        <br />
                    </div>
                    <br />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
