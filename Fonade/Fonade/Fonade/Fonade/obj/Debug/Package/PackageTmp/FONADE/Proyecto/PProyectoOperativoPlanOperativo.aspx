<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoOperativoPlanOperativo.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoOperativoPlanOperativo" EnableEventValidation="true" %>

<%@ Register Src="../../Controles/Alert.ascx" TagName="Alert" TagPrefix="uc1" %>
<%@ Register Src="../../Controles/Post_It.ascx" TagName="Post_It" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
    <script type="text/javascript">
        function CalcularTotal(nombreCampo, anio) {
            var totalRegistro = 0;

            for (var i = 1; i <= 12; i++) {
                var valor = document.getElementById(nombreCampo + i).value;
                if (isNaN(parseFloat(valor))) {
                    valor = 0
                    document.getElementById(nombreCampo + i).value = "";
                } else {
                    valor = parseFloat(valor)
                }
                totalRegistro = totalRegistro + valor;
            }
            document.getElementById(nombreCampo + "Total").innerHTML = "<b>" + formatCurrency(totalRegistro) + "</b>";
            //Total Columna
            var valorAporte = document.getElementById('txtAporte' + anio).value;
            var valorFondo = document.getElementById('txtFondo' + anio).value;

            valorAporte = isNaN(parseFloat(valorAporte)) ? 0 : parseFloat(valorAporte);
            valorFondo = isNaN(parseFloat(valorFondo)) ? 0 : parseFloat(valorFondo);

            document.getElementById('TotalMes' + anio).innerHTML = "<b>" + formatCurrency(parseFloat(valorAporte) + parseFloat(valorFondo)) + "</b>";

        }

        function formatCurrency(strValue) {
            strValue = strValue.toString().replace(/\$|\,/g, '');
            dblValue = parseFloat(strValue);

            blnSign = (dblValue == (dblValue = Math.abs(dblValue)));
            dblValue = Math.floor(dblValue * 100 + 0.50000000001);
            intCents = dblValue % 100;
            strCents = intCents.toString();
            dblValue = Math.floor(dblValue / 100).toString();
            if (intCents < 10)
                strCents = "0" + strCents;
            for (var i = 0; i < Math.floor((dblValue.length - (1 + i)) / 3); i++)
                dblValue = dblValue.substring(0, dblValue.length - (4 * i + 3)) + ',' +
			dblValue.substring(dblValue.length - (4 * i + 3));
            return (((blnSign) ? '' : '-') + '$' + dblValue + '.' + strCents);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:Alert ID="Alert1" runat="server" />
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
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Cronograma de Actividades', texto: 'PlanOperativo'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasAprovisionamiento" />
                        </div>
                        <div>
                            Cronograma de Actividades:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_Post_It_1" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="PlanOperativo" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <br />
        <table width='100%' align="Center" border='0' cellpadding='0' cellspacing='0'>
            <tr>
                <td align='left' valign='top' width='98%'>
                    <table width='100%' border='0' cellspacing='1' cellpadding='4'>
                        <tr>
                            <td align='left'>
                                <asp:Panel ID="pnlAdicionarActividadPlanOperativo" runat="server" Visible="false">
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/icoAdicionarUsuario.gif" />
                                    <asp:Button ID="btnAdicionarActividadPlan" runat="server" Text="Adicionar Actividad al Plan Operativo"
                                        BorderStyle="None" OnClick="btnAdicionarActividadPlan_Click" CssClass="boton_Link" />
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    <table cellpadding="0px" cellspacing="0px">
                        <tr>
                            <td valign="Top">
                                <div style="width: 300px; overflow: auto; border-right: silver 1px solid">
                                    <table class="Grilla" cellpadding="0" cellspacing="0" width="300px" border="0">
                                        <tr>
                                            <th style="width: 264px; text-align: center">
                                                &nbsp;
                                            </th>
                                        </tr>
                                    </table>
                                    <asp:GridView ID="gw_Anexos" runat="server" Width="300px" AutoGenerateColumns="false"
                                        CssClass="Grilla" OnRowCommand="gw_Anexos_RowCommand" RowStyle-Height="35px"
                                        CellPadding="2" CellSpacing="2" GridLines="None" BorderColor="#ffffff">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="btn_Borrar" CommandName="Borrar" CommandArgument='<%# Eval("Id_Actividad") %>'
                                                        runat="server" ImageUrl="/Images/icoBorrar.gif" Visible="false" OnClientClick="return Confirmacion('Esta seguro que desea borrar la actividad seleccionada?')" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Item" HeaderText="Item" />
                                            <asp:TemplateField HeaderText="Actividad">
                                                <ItemTemplate>
                                                    <asp:Button ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id_Actividad") %>'
                                                        Text='<%# Eval("Actividad") %>' Visible="false" CssClass="boton_Link_Grid" />
                                                    <asp:Label ID="lblEditar" runat="server" Text='<%# Eval("Actividad") %>' Visible="true" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </td>
                            <td valign="Top">
                                <div style="width: 560px; overflow: auto">
                                    <table class="Grilla" cellpadding="2" cellspacing="2" width="3380px" style="border-color: #ffffff">
                                        <tr>
                                            <th style="width: 264px; text-align: center">
                                                Mes 1
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 2
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 3
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 4
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 5
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 6
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 7
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 8
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 9
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 10
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 11
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Mes 12
                                            </th>
                                            <th style="width: 264px; text-align: center">
                                                Costo Total
                                            </th>
                                        </tr>
                                    </table>
                                    <asp:GridView ID="gw_AnexosActividad" runat="server" Width="3380px" AutoGenerateColumns="false"
                                        CssClass="Grilla" OnRowCommand="gw_Anexos_RowCommand" RowStyle-Height="35px"
                                        CellPadding="2" CellSpacing="2" GridLines="None" BorderColor="#ffffff">
                                        <Columns>
                                            <asp:BoundField DataField="fondo1" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor1" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo2" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor2" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo3" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor3" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo4" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor4" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo5" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor5" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo6" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor6" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo7" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor7" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo8" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor8" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo9" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor9" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo10" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor10" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo11" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor11" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondo12" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedor12" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="fondoTotal" HeaderText="Fondo" ItemStyle-Width="130px" />
                                            <asp:BoundField DataField="emprendedorTotal" HeaderText="Emprendedor" ItemStyle-Width="130px" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!--  Nuevo Panel -->
    <asp:Panel ID="pnlCrearActividad" Visible="false" runat="server">
        <asp:Label ID="lblTitulo" runat="server" Text="NUEVA ACTIVIDAD"></asp:Label>
        <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
        <table width='1000px' border='0' cellspacing='0' cellpadding='2'>
            <tr valign="top">
                <td align="left">
                    <b>Item:</b>
                </td>
                <td>
                    <asp:TextBox ID="txtItem" runat="server" MaxLength="3" Width="40px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="El No. de Item es requerido"
                        Display="Dynamic" ControlToValidate="txtItem" ValidationGroup="ValidadorCrearActividad"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="El No. de Item es un valor numérico"
                        ControlToValidate="txtItem" ValidationExpression="[0-9]*" ValidationGroup="ValidadorCrearActividad"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr valign="top">
                <td align="Left">
                    <b>Actividad:</b>
                </td>
                <td>
                    <asp:TextBox ID="txtNombreActividad" runat="server" MaxLength="150" Width="350px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="La actividad es requerida"
                        ControlToValidate="txtNombreActividad" ValidationGroup="ValidadorCrearActividad"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr valign="top">
                <td align="Left">
                    <b>Metas:</b>
                </td>
                <td>
                    <asp:TextBox ID="txtMetas" runat="server" Width="500px" Height="100px" TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlAprobacion" runat="server" Visible="false">
            <table width='1000px' border='0' cellspacing='0' cellpadding='2'>
                <tr valign="top">
                    <td align="Left">
                        <b>Aprobar:</b><br>
                        <br>
                        <b>Observaciones:</b>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAprobar" runat="server" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlAprobado" runat="server">
                            <asp:ListItem Value="No" Text="No"></asp:ListItem>
                            <asp:ListItem Value="Si" Text="Si"></asp:ListItem>
                        </asp:DropDownList>
                        <br>
                        <asp:TextBox ID="txtObservaciones" runat="server" Width="300px" Height="100px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br>
        <table width='98%' border='0' cellspacing='0' cellpadding='2'>
            <tr valign="top">
                <td colspan="14">
                    REQUERIMIENTOS DE RECURSOS POR MES
                </td>
            </tr>
            <tr valign="top">
                <td>
                    <b>&nbsp;</b>
                </td>
                <td>
                    <b>Mes 1</b>
                </td>
                <td>
                    <b>Mes 2</b>
                </td>
                <td>
                    <b>Mes 3</b>
                </td>
                <td>
                    <b>Mes 4</b>
                </td>
                <td>
                    <b>Mes 5</b>
                </td>
                <td>
                    <b>Mes 6</b>
                </td>
                <td>
                    <b>Mes 7</b>
                </td>
                <td>
                    <b>Mes 8</b>
                </td>
                <td>
                    <b>Mes 9</b>
                </td>
                <td>
                    <b>Mes 10</b>
                </td>
                <td>
                    <b>Mes 11</b>
                </td>
                <td>
                    <b>Mes 12</b>
                </td>
                <td>
                    <b>Costo Total</b>
                </td>
            </tr>
            <tr valign="top">
                <td valign="middle" width='8%'>
                    Fondo Emprender
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo1" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','1')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo2" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','2')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo3" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','3')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo4" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','4')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo5" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','5')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo6" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','6')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo7" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','7')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo8" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','8')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo9" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','9')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo10" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','10')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo11" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','11')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtFondo12" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtFondo','12')"> </asp:TextBox>
                </td>
                <td align="right" id='txtFondoTotal' runat="server">
                </td>
            </tr>
            <tr valign="top">
                <td valign="middle" width='8%'>
                    Aporte Emprendedor
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte1" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','1')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte2" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','2')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte3" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','3')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte4" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','4')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte5" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','5')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte6" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','6')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte7" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','7')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte8" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','8')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte9" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','0')"> </asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte10" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','10')"></asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte11" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','11')"></asp:TextBox>
                </td>
                <td align="left" valign="middle">
                    <asp:TextBox ID="txtAporte12" runat="server" MaxLength="10" Width="50px" ClientIDMode="Static"
                        onBlur=" CalcularTotal('txtAporte','12')"></asp:TextBox>
                </td>
                <td align="right" id='txtAporteTotal' runat="server">
                </td>
            </tr>
            <tr valign="top">
                <td>
                    <b>Total</b>
                </td>
                <td align="right" id='TotalMes1' runat="server">
                </td>
                <td align="right" id='TotalMes2' runat="server">
                </td>
                <td align="right" id='TotalMes3' runat="server">
                </td>
                <td align="right" id='TotalMes4' runat="server">
                </td>
                <td align="right" id='TotalMes5' runat="server">
                </td>
                <td align="right" id='TotalMes6' runat="server">
                </td>
                <td align="right" id='TotalMes7' runat="server">
                </td>
                <td align="right" id='TotalMes8' runat="server">
                </td>
                <td align="right" id='TotalMes9' runat="server">
                </td>
                <td align="right" id='TotalMes10' runat="server">
                </td>
                <td align="right" id='TotalMes11' runat="server">
                </td>
                <td align="right" id='TotalMes12' runat="server">
                </td>
                <td align="right" id='TotalMesTotal' runat="server">
                </td>
            </tr>
            <tr valign="top">
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr valign="top">
                <td colspan="11" align="right" class="Titdestacado">
                    <asp:HiddenField ID="hddIdActividad" runat="server" />
                    <asp:Button ID="btnCrearActividad" runat="server" Text="Crear" OnClick="btnCrearActividad_Click"
                        ValidationGroup="ValidadorCrearActividad" />
                    <asp:Button ID="btnCerrar" runat="server" Text="Cerrar" OnClick="btnCerrar_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
