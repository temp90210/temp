<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterIndicadoresGenericos.aspx.cs" Inherits="Fonade.FONADE.interventoria.InterIndicadoresGenericos" %>

<%@ Register src="../../Controles/Post_It.ascx" tagname="Post_It" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.9.1.js"></script>
    <script type="text/ecmascript">
        function url() {
            open("agregarRiesgo.aspx", "Agregar Riesgo", "width=800,height=600");
        }
</script>
    <script type="text/javascript">
        function ValidNum(e) {
            var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
            return (tecla != 13);
        }
    </script>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
        .sinlinea {
            border:none;
            border-collapse:collapse;
            border-bottom-color:none;
        }
        table {
            width:100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <br />
        <table class="style1">
            <tr>
                <td>
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Indicadores Genéricos', texto: 'IndicadoresInter'});">
                            <img alt="help_Objetivos" border="0" src="../../Images/imgAyuda.gif" />
                        </div>
                        <div>
                            &nbsp; <strong>Indicadores Genéricos</strong>
                        </div>
                    </div>
                    <br />
                </td>
                <td>
                    <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="IndicadoresInter" _txtTab="1" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:GridView ID="gvindicadoresgenericos" runat="server" CssClass="Grilla" AutoGenerateColumns="false" EmptyDataText="No se han establecido Indicadores Genéricos para esta empresa." DataKeyNames="Id_IndicadorGenerico">
                        <Columns>
                            <asp:BoundField HeaderText="" DataField="Id_IndicadorGenerico" Visible="false"/>
                            <asp:BoundField HeaderText="Nombre del Indicador" DataField="NombreIndicador" />
                            <asp:TemplateField HeaderText="Descripción">
                                <ItemTemplate>
                                    <div style="text-align:center;">
                                        <asp:Label ID="lblNumDescripcion" runat="server" Text='<%# Eval("NueradorDescripcion") %>'></asp:Label>
                                        <hr />
                                        <asp:Label ID="lblDenDescripcion" runat="server" Text='<%# Eval("DenominadorDescripcion") %>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <div style="text-align:center;">
                                        <asp:TextBox ID="txtNumerador" runat="server" Text='<%# Eval("Numerador") %>'></asp:TextBox>
                                        <hr />
                                        <asp:TextBox ID="txtDenominador" runat="server" Text='<%# Eval("Denominador") %>'></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Evaluación" DataField="Evaluacion" />
                            <asp:TemplateField HeaderText="Observación">
                                <ItemTemplate>
                                    <div style="width:300px; text-align:center;">
                                        <asp:TextBox ID="txtObservacion" runat="server" Text='<%# Eval("Observacion") %>' TextMode="MultiLine" Width="300px" Height="150px" MaxLength="8000"></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td style="text-align:center;" colspan="2">
                    <br />
                    <br />
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
