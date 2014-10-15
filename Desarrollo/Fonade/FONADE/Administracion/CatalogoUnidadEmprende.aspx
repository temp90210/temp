<%@ Page Language="C#" Title="FONDO EMPRENDER - Administrar Unidades de Emprendimiento"
    MasterPageFile="~/Master.master" AutoEventWireup="true" CodeBehind="~/FONADE/Administracion/CatalogoUnidadEmprende.aspx.cs"
    Inherits="Fonade.FONADE.Administracion.CatalogoUnidadEmprende" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:ScriptManager ID="scr_1" runat="server">
    </asp:ScriptManager>
    <script src="../../Scripts/jquery-1.10.2.min.js"></script>
    <script>
        $(document).ready(function () {
            var $elBoton = $('.miPanel');
            //$('body').append('<div id="miPantalla" style="width: 100%; height: 100%; background: rgba(0,0,0,0.5); position: fixed; margin: auto; top: 0; bottom: 0; left: 0; right: 0;"><div style="position: absolute; margin: auto; top: 0; bottom: 0; left: 0; right: 0; width: 200px; height:130px; text-align: center; background: white; border-radius: 50px;"><h1>CARGANDO</h1><img src="https://mitoyotapr.com/Content/images/loading.gif" width="70" /></div></div>')
            $('.miClase').click(function () {
                setTimeout(function () {
                    $elBoton = $('.miPanel').find('input#bodyContentPlace_btn_modificarUnidad');
                    console.log($elBoton);
                }, 3000);
                //alert('entra');
            });
            
            $elBoton.bind('click', function () {
                alert("se hizo click");
            });
            
        });
    </script>
    <%--<div id="miPantalla" style="width: 100%; height: 100%; background: rgba(0,0,0,0.5); position: fixed; margin: auto; top: 0; bottom: 0; left: 0; right: 0;"><div style="position: absolute; margin: auto; top: 0; bottom: 0; left: 0; right: 0; width: 170px; height:130px; text-align: center; background: white; border-radius: 50px;"><h1>CARGANDO</h1><img src="https://mitoyotapr.com/Content/images/loading.gif" width="70" /></div></div>--%>
    <asp:UpdatePanel ID="updt_1" runat="server">
        <ContentTemplate>
            <h1>
                <asp:Label ID="lbl_enunciado" runat="server" Text="" />
            </h1>
            <asp:Panel ID="pnlPrincipal" runat="server" Visible="true">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="text-align: left">
                            <asp:Label ID="lblvalidador" runat="server" Style="display: none" />
                            <asp:ImageButton ID="Adicionar" runat="server" ImageUrl="../../Images/icoAdicionarUsuario.gif"
                                Style="cursor: pointer;" OnClick="Adicionar_Click" />
                            &nbsp;
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click"> Adicionar Unidad</asp:LinkButton>
                            <asp:HiddenField ID="hdf_id" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="4">
                            <asp:LinkButton ID="lnkbtn_opcion_A" Text="A" CssClass="opcionUno" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_A_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_B" Text="B" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_B_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_C" Text="C" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_C_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_D" Text="D" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_D_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_E" Text="E" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_E_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_F" Text="F" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_F_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_G" Text="G" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_G_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_H" Text="H" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_H_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_I" Text="I" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_I_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_J" Text="J" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_J_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_K" Text="K" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_K_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_L" Text="L" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_L_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_M" Text="M" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_M_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_N" Text="N" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_N_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_O" Text="O" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_O_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_P" Text="P" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_P_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_Q" Text="Q" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_Q_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_R" Text="R" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_R_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_S" Text="S" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_S_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_T" Text="T" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_T_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_U" Text="U" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_U_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_V" Text="V" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_V_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_W" Text="W" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_W_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_X" Text="X" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_X_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_Y" Text="Y" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_Y_Click" />
                            <asp:LinkButton ID="lnkbtn_opcion_Z" Text="Z" runat="server" Style="font: bold; color: Blue;
                                text-decoration: none;" OnClick="lnkbtn_opcion_Z_Click" />
                            <strong style="font: bold; color: Blue; text-decoration: none;">|</strong>
                            <asp:LinkButton ID="lnkbtn_opcion_todos" Text="Todos" runat="server" OnClick="lnkbtn_opcion_todos_Click"
                                Style="font: bold; color: Blue; text-decoration: none;" />
                        </td>
                    </tr>
                    <%--Grilla--%>
                    <tr>
                        <td style="text-align: center">
                            <asp:GridView ID="gv_UnidadesEmprendimiento" runat="server" Width="98%" AutoGenerateColumns="false"
                                CssClass="Grilla" AllowPaging="True" AllowSorting="true" ShowHeaderWhenEmpty="true"
                                BorderWidth="1" CellSpacing="1" CellPadding="4" PageSize="30" OnRowDataBound="gv_UnidadesEmprendimiento_RowDataBound"
                                OnRowCommand="gv_UnidadesEmprendimiento_RowCommand" OnSorting="gv_UnidadesEmprendimiento_Sorting"
                                OnPageIndexChanging="gv_UnidadesEmprendimiento_PageIndexChanging">
                                <PagerStyle CssClass="Paginador" />
                                <RowStyle HorizontalAlign="Left" VerticalAlign="Top" />
                                <Columns>
                                    <asp:BoundField HeaderText="Id_Institucion" DataField="Id_Institucion" Visible="false" />
                                    <asp:TemplateField HeaderStyle-Width="3%">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="img_btn" ImageUrl="../../Images/icoBorrar.gif" runat="server"
                                                CausesValidation="false" CommandName="eliminar" CommandArgument='<%# Eval("Id_Institucion") + ";"+Eval("NomUnidad") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nombre Unidad" ControlStyle-CssClass="miClase" SortExpression="NomUnidad" HeaderStyle-Width="40%">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkeditar" runat="server" CausesValidation="False" CommandArgument='<%# Eval("Id_Institucion")+ ";" + Eval("NomUnidad") %>'
                                                CommandName="editar" Text='<%#Eval("NomUnidad")+ " "  +"("+Eval("NomInstitucion")+")"%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Tipo" DataField="NomTipoInstitucion" SortExpression="NomTipoInstitucion"
                                        HeaderStyle-Width="30%" />
                                    <asp:TemplateField HeaderText="Estado" ItemStyle-ForeColor="Blue" HeaderStyle-Width="27%">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_id_inst" Text='<%# Eval("Id_Institucion") %>' runat="server" Visible="false" />
                                            <asp:Label ID="lbl_estado" Text='<%# Eval("Inactivo") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="CodCiudad" DataField="CodCiudad" Visible="false" />
                                </Columns>
                                <PagerStyle CssClass="Paginador" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnl_detalles" runat="server" Visible="false" CssClass="miPanel">
                <table width="95%" border="1" cellpadding="0" cellspacing="0" style="border-color: #4E77AF;">
                    <tbody>
                        <tr>
                            <td align="center" valign="top" width="98%">
                                <table width="98%" border="0" cellspacing="0" cellpadding="3">
                                    <tbody>
                                        <tr>
                                            <td align="right">
                                                <b>Tipo de Unidad:</b>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="dd_CodTipoInstitucion" runat="server" AutoPostBack="true" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Nombre Unidad:</b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txt_nombreUnidad" runat="server" MaxLength="80" Width="219px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Nombre Centro o Institución:</b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txt_NombreCentroInstitucion" runat="server" MaxLength="80" Width="219px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>NIT Centro o Institución:</b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txt_NitCentroInstitucion" runat="server" MaxLength="80" Width="219px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Departamento:</b>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="dd_SelDpto2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dd_SelDpto2_SelectedIndexChanged"
                                                    Width="179px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Ciudad:</b>
                                            </td>
                                            <td height="25" align="left">
                                                <asp:DropDownList ID="dd_Ciudades" runat="server" AutoPostBack="true" Width="75px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Dirección de Correpondencia:</b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txt_Direccion" runat="server" MaxLength="100" Width="219px" />
                                            </td>
                                        </tr>
                                        <asp:Panel ID="pnl_detalles_unk" runat="server" Visible="false">
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lbl_telefonoUnidad" Text="Teléfono:" runat="server" />
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtTelefonoUnidad" runat="server" Width="219px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lbl_faxUnidad" Text="Fax:" runat="server" />
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtFaxUnidad" runat="server" Width="219px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lbl_websiteUnidad" Text="Website:" runat="server" />
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtWebsite" runat="server" Width="219px" />
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                        <tr>
                                            <td align="right">
                                                <b>Criterios de Selección:</b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtCriterios" runat="server" TextMode="MultiLine" Rows="5" Columns="35" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Jefe de Unidad:</b>
                                            </td>
                                            <td align="left">
                                                <asp:LinkButton ID="lnk_invocar_ventanaModal" runat="server" Width="219px" Style="text-decoration: none;"
                                                    OnClick="lnk_invocar_ventanaModal_Click">
                                                    <asp:TextBox ID="txt_jefeUnidad" runat="server" Enabled="false" Width="219px" />
                                                </asp:LinkButton><asp:Button ID="btn_buscarJefeUnidad" Text="Buscar" runat="server"
                                                    OnClick="btn_buscarJefeUnidad_Click" /><br />
                                                <asp:Button ID="btn_cambiarDatosJefe" Text="Cambiar Datos Jefe" runat="server" Visible="false"
                                                    OnClick="btn_cambiarDatosJefe_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lbl_razonCambio" Text="Razón de cambio de Jefe de Unidad:" runat="server"
                                                    Visible="false" />
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtCambioJefe" runat="server" TextMode="MultiLine" Rows="5" Columns="35" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="right">
                                                <asp:Label ID="txtCargando" runat="server" ForeColor="Red" Text="Cargando..." Visible="False"></asp:Label><asp:Button ID="btn_actualizarUnidad" CssClass="botonParaClick" Text="Actualizar" runat="server" Visible="false" />
                                                <asp:Button ID="btn_crearUnidad" CssClass="botonParaClick" Text="Crear" runat="server" OnClick="btn_crearUnidad_Click" />
                                                <asp:Button ID="btn_modificarUnidad" CssClass="openCargando" Text="Modificar Unidad" runat="server" 
                                                    Visible="false" OnClick="btn_modificarUnidad_Click" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
