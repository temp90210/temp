<%@ Page Language="C#" Title="FONDO EMPRENDER - " MasterPageFile="~/Master.master"
    AutoEventWireup="true" CodeBehind="~/FONADE/Administracion/AcreditacionActa.aspx.cs"
    Inherits="Fonade.FONADE.Administracion.AcreditacionActa" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <script type="text/javascript">
        function alerta() {
            return confirm('¿Está seguro que eliminar esta Acta?');
        }

        function alerta_detalle() {
            return confirm('Esta seguro de eliminar este proyecto del Acta?');
        }

        //Función imprimir.
        function imprimir() {
            document.getElementById("oculto").style.display = "block";

            var divToPrint = document.getElementById('oculto');
            var newWin = window.open('', 'Print-Window', 'width=1000,height=500');
            newWin.document.open();
            newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
            document.getElementById("oculto").style.display = "none";
            newWin.document.close();
            setTimeout(function () { newWin.close(); }, 1000);

            document.getElementById("oculto").style.display = "none";
        }

    </script>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.9.1.js" type="text/javascript"></script>
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <h1>
        <asp:Label ID="lbl_enunciado" runat="server" Text="ACTAS PARCIALES DE ACREDITACIÓN" />
    </h1>
    <asp:Panel ID="pnlPrincipal" runat="server" Visible="true">
        <table width="100%" align="center" border="0" cellspacing="0">
            <tbody>
                <tr>
                    <td align="left">
                        <b>Tipo Acta:</b>
                        <asp:DropDownList ID="dd_tiposActa" runat="server" OnSelectedIndexChanged="dd_tiposActa_SelectedIndexChanged"
                            AutoPostBack="true">
                            <asp:ListItem Text="Listar todas las Actas" Value="" />
                            <asp:ListItem Text="Actas Acreditadas" Value="1" />
                            <asp:ListItem Text="Actas NO Acreditadas" Value="0" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </tbody>
        </table>
        <table width="100%" align="center" border="0" cellspacing="0">
            <tbody>
                <tr>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:ImageButton ID="img_btn_AddActaAcreditacion" ImageUrl="../../Images/icoAdicionarUsuario.gif"
                            runat="server" OnClick="img_btn_AddActaAcreditacion_Click" />
                        <asp:LinkButton ID="lnk_btn_AddActaAcreditacion" Text="Adicionar Acta de Acreditación"
                            runat="server" OnClick="lnk_btn_AddActaAcreditacion_Click" />
                    </td>
                    <td align="left">
                        <asp:ImageButton ID="img_btn_AddActa_NO_Acreditacion" ImageUrl="../../Images/icoAdicionarUsuario.gif"
                            runat="server" OnClick="img_btn_AddActa_NO_Acreditacion_Click" />
                        <asp:LinkButton ID="lnk_btn_AddActa_NO_Acreditacion" Text="Adicionar Acta de NO Acreditación"
                            runat="server" OnClick="lnk_btn_AddActa_NO_Acreditacion_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
        <table border="0" cellpadding="0" cellspacing="0">
            <tbody>
                <tr>
                    <td>
                        <asp:GridView ID="gv_resultadosActas" runat="server" AutoGenerateColumns="false"
                            AllowSorting="true" CssClass="Grilla" OnRowDataBound="gv_resultadosActas_RowDataBound"
                            OnSorting="gv_resultadosActas_Sorting" OnRowCommand="gv_resultadosActas_RowCommand"
                            AllowPaging="True" OnPageIndexChanging="gv_resultadosActas_PageIndexChanging"
                            PageSize="30">
                            <PagerStyle CssClass="Paginador" />
                            <RowStyle HorizontalAlign="Left" />
                            <Columns>
                                <asp:BoundField HeaderText="" DataField="Id_Acta" Visible="false" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkeliminar" runat="server" OnClientClick="return alerta()" CommandArgument='<%# Eval("publicado") %>'
                                            CommandName="eliminar" CausesValidation="false">
                                            <asp:Label runat="server" ID="lblactividaPOI" Visible="False" Text='<%# Eval("publicado") %>' />
                                            <asp:Image ID="imgeditar" ImageUrl="../../Images/icoBorrar.gif" runat="server" Style="cursor: pointer;" />
                                        </asp:LinkButton></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="No Acta" SortExpression="NumActa">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnk_id_memorando" runat="server" ForeColor="Black" CausesValidation="False"
                                            CommandArgument='<%# Eval("Id_Acta")+ ";" + Eval("publicado") %>' CommandName="mostrar"
                                            Text='<%#Eval("NumActa")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Nombre" DataField="NomActa" SortExpression="NomActa" />
                                <asp:BoundField HeaderText="Convocatoria" DataField="NomConvocatoria" SortExpression="NomConvocatoria" />
                                <asp:BoundField HeaderText="Fecha" DataField="FechaActa" HtmlEncode="false" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:TemplateField HeaderText="Acta acreditada" SortExpression="NumActa">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_siNoActa" runat="server" ForeColor="Black" CausesValidation="False"
                                            CommandArgument='<%# Eval("Id_Acta")+ ";" + Eval("publicado")  %>' CommandName="mostrar"
                                            Text='<%#Eval("ActaAcreditada")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnl_detalles" runat="server" Visible="false">
        <table width="100%" align="center" border="0" cellspacing="0">
            <tbody>
                <tr valign="top">
                    <td class="titulosCentro" colspan="2">
                        <asp:Label ID="lbl_enunciado_acta" Text="ACTA DE ACREDITACIÓN" runat="server" ForeColor="Red" />
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <b>No Acta:</b> </td><td>
                        <asp:TextBox ID="txt_noActaSeleccionado" runat="server" MaxLength="10" Enabled="false"
                            Width="94px" />
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <b>Nombre:</b> </td><td>
                        <asp:TextBox ID="txt_NomActaSeleccionado" runat="server" MaxLength="80" Enabled="false"
                            Width="430px" />
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <b>Fecha:</b> </td><td>
                        <asp:DropDownList ID="dd_fecha_dias_Memorando" runat="server">
                            <asp:ListItem Text="1" Value="1" />
                            <asp:ListItem Text="2" Value="2" />
                            <asp:ListItem Text="3" Value="3" />
                            <asp:ListItem Text="4" Value="4" />
                            <asp:ListItem Text="5" Value="5" />
                            <asp:ListItem Text="6" Value="6" />
                            <asp:ListItem Text="7" Value="7" />
                            <asp:ListItem Text="8" Value="8" />
                            <asp:ListItem Text="9" Value="9" />
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="11" Value="11" />
                            <asp:ListItem Text="12" Value="12" />
                            <asp:ListItem Text="13" Value="13" />
                            <asp:ListItem Text="14" Value="14" />
                            <asp:ListItem Text="15" Value="15" />
                            <asp:ListItem Text="16" Value="16" />
                            <asp:ListItem Text="17" Value="17" />
                            <asp:ListItem Text="18" Value="18" />
                            <asp:ListItem Text="19" Value="19" />
                            <asp:ListItem Text="20" Value="20" />
                            <asp:ListItem Text="20" Value="20" />
                            <asp:ListItem Text="21" Value="21" />
                            <asp:ListItem Text="22" Value="22" />
                            <asp:ListItem Text="23" Value="23" />
                            <asp:ListItem Text="24" Value="24" />
                            <asp:ListItem Text="25" Value="25" />
                            <asp:ListItem Text="26" Value="26" />
                            <asp:ListItem Text="27" Value="27" />
                            <asp:ListItem Text="28" Value="28" />
                            <asp:ListItem Text="29" Value="29" />
                            <asp:ListItem Text="30" Value="30" />
                            <asp:ListItem Text="31" Value="31" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="dd_fecha_mes_Memorando" runat="server">
                            <asp:ListItem Text="Ene" Value="1" />
                            <asp:ListItem Text="Feb" Value="2" />
                            <asp:ListItem Text="Mar" Value="3" />
                            <asp:ListItem Text="Abr" Value="4" />
                            <asp:ListItem Text="May" Value="5" />
                            <asp:ListItem Text="Jun" Value="6" />
                            <asp:ListItem Text="Jul" Value="7" />
                            <asp:ListItem Text="Ago" Value="8" />
                            <asp:ListItem Text="Sep" Value="9" />
                            <asp:ListItem Text="Oct" Value="10" />
                            <asp:ListItem Text="Nov" Value="11" />
                            <asp:ListItem Text="Dic" Value="12" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="dd_fecha_year_Memorando" runat="server" />
                    </td>
                    <td>
                        <span id="sp_FechaFormateada" runat="server" visible="false" />
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <b>Observaciones:</b> </td><td>
                        <asp:TextBox ID="txt_observaciones" runat="server" Enabled="false" TextMode="MultiLine"
                            Rows="8" Columns="60" />
                        <span id="sp_observaciones" runat="server" visible="false" />
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <b>Convocatoria:</b> </td><td>
                        <span id="sp_convocatoria" runat="server" visible="false"></span>
                        <asp:DropDownList ID="dd_convocatoriasSeleccionables" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <asp:Panel ID="panel_publicar" runat="server" Visible="false">
                    <tr valign="top">
                        <td>
                            <b>Publicar:</b> </td><td>
                            <asp:CheckBox ID="Publicar" runat="server" />
                        </td>
                    </tr>
                </asp:Panel>
                <tr valign="top">
                    <td>
                    </td>
                    <td align="right">
                        <asp:Button ID="Btn_crearActa" runat="server" Text="Nuevo" Visible="false" OnClick="Btn_crearActa_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
        <br />
        <table width="100%">
            <tbody>
                <tr id="panel_AddPlanes" runat="server" visible="false">
                    <td>
                        <asp:ImageButton ID="img_btn_addPlanes" ImageUrl="../../Images/icoAdicionarUsuario.gif"
                            runat="server" OnClick="img_btn_addPlanes_Click" />&nbsp; <asp:LinkButton ID="lnk_btn_addPlanes" Text="Adicionar Planes de Negocio al Acta"
                            runat="server" OnClick="lnk_btn_addPlanes_Click" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="gv_DetallesActa" runat="server" AutoGenerateColumns="false" CssClass="Grilla"
                            Enabled="true" OnRowCommand="gv_DetallesActa_RowCommand" OnRowDataBound="gv_DetallesActa_RowDataBound"
                            ShowHeaderWhenEmpty="true" ShowHeader="true" Width="100%" GridLines="Both">
                            <PagerStyle CssClass="Paginador" />
                            <RowStyle HorizontalAlign="Left" />
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="3%" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkeliminar_detalle" runat="server" OnClientClick="return alerta_detalle()"
                                            CommandArgument='<%# Eval("id_proyecto")+ ";" + Eval("idAcreditador") %>' CommandName="eliminar_detalle"
                                            CausesValidation="false" Visible="false">
                                            <asp:Image ID="imgeliminar" ImageUrl="../../Images/icoBorrar.gif" runat="server"
                                                Style="cursor: pointer;" />
                                        </asp:LinkButton></ItemTemplate></asp:TemplateField><asp:BoundField HeaderText="Id" DataField="Id_Proyecto" HeaderStyle-Width="3%" ItemStyle-HorizontalAlign="Left" />
                                <asp:TemplateField HeaderText="Plan de Negocio" HeaderStyle-Width="50%" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnk_detalleActaSeleccionado" runat="server" CausesValidation="False"
                                            CommandArgument='<%# Eval("id_proyecto")+ ";" + Eval("NomProyecto")  %>' CommandName="mostrar_proyecto"
                                            Text='<%#Eval("NomProyecto")%>' ForeColor="Black" Style="text-decoration: underline;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="" DataField="viable" Visible="false" ItemStyle-HorizontalAlign="Left" />
                                <asp:TemplateField HeaderText="Acreditador" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnk_acreditador" runat="server" CausesValidation="False" CommandArgument='<%# Eval("idAcreditador")+ ";" + Eval("Acreditador") %>'
                                            CommandName="mostrar_acreditador" Text='<%#Eval("Acreditador")%>' Style="text-decoration: none;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Viable Acreditación" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_dt_ViableSiNo" Text='<%# Eval("viableAcreditador") %>' runat="server" />
                                        <asp:Label ID="lbl_viable" Text='<%# Eval("viable") %>' runat="server" Visible="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btn_imprimirMemorando" Text="Imprimir" runat="server" Visible="false"
                            OnClientClick="imprimir()" />
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <div id="oculto" style="display: none;">
        <table width="95%" border="0" cellspacing="0" cellpadding="2">
            <tbody>
                <tr>
                    <td width="50%" align="center" valign="baseline" bgcolor="#000000" class="Blanca">
                        <b>
                            <asp:Label ID="lbl_actaTitulo" Text="ACTA DE ACREDITACIÓN" runat="server" ForeColor="White" /></b>
                    </td>
                    <td width="30%" align="right" class="titulo">
                        &nbsp; </td><td width="20%" align="right" class="titulo">
                        &nbsp; </td></tr></tbody></table><table width="100%" border="0" cellpadding="4" cellspacing="1">
            <tbody>
                <tr>
                    <td width="30%">
                        <b>No Acta:</b> </td><td>
                        <span id="sp_noActa" runat="server" visible="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Nombre:</b> </td><td>
                        <span id="sp_Nombre" runat="server" visible="false" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Planes de Negocio Incluidos <asp:GridView ID="gv_imprimir_planesNegocio" runat="server" AutoGenerateColumns="false"
                            CssClass="Grilla" Enabled="false" OnRowDataBound="gv_imprimir_planesNegocio_RowDataBound">
                            <PagerStyle CssClass="Paginador" />
                            <RowStyle HorizontalAlign="Left" />
                            <HeaderStyle BackColor="Gray" ForeColor="White" />
                            <Columns>
                                <asp:BoundField HeaderText="Id" DataField="Id_Proyecto" Visible="false" />
                                <asp:BoundField HeaderText="Plan de Negocio" DataField="NomProyecto" />
                                <asp:BoundField HeaderText="" DataField="viable" Visible="false" />
                                <asp:BoundField HeaderText="" DataField="idAcreditador" Visible="false" />
                                <asp:BoundField HeaderText="Acreditador" DataField="Acreditador" />
                                <asp:TemplateField HeaderText="Viable Acreditación" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_ViableSiNo" Text='<%# Eval("viable") %>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        </p>
                        <br />
                        <hr />
                        <p>
                            Aprobó: <asp:BoundField HeaderText="Plan de Negocio" DataField="NomProyecto" />
                            <asp:BoundField HeaderText="" DataField="viable" Visible="false" />
                            <asp:BoundField HeaderText="" DataField="idAcreditador" Visible="false" />
                            <asp:BoundField HeaderText="Acreditador" DataField="Acreditador" />
                            <asp:TemplateField HeaderText="Viable Acreditación" ItemStyle-HorizontalAlign="Center">
                                <itemtemplate>
                            <asp:Label ID="lbl_ViableSiNo" Text='<%# Eval("viable") %>' runat="server" />
                        </itemtemplate>
                            </asp:TemplateField>
                            </Columns> </asp:GridView>
                        </p>
                        <br />
                        <hr />
                        <p>
                            Aprobó: </p><br />
                        <br />
                        ______________________________________<br />
                        Gerente de Convenio Fondo Emprender<br />
                        <br />
                        <br />
                        <br />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
