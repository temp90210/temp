<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImprimirActaAsignacion.aspx.cs" Inherits="Fonade.FONADE.Priorizacion_de_Proyectos.ImprimirActaAsignacion"  MasterPageFile="~/MasterImpr.Master"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="BodyContent"  runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

    <asp:LinqDataSource ID="lds_ActaAsignacion" runat="server" 
        ContextTypeName="Datos.FonadeDBDataContext" AutoPage="true" 
        onselecting="lds_ActaAsignacion_Selecting" >
    </asp:LinqDataSource>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"> </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="PanelInfo" runat="server" Visible="true" Width="660px" 
        UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>

        <table width="650px">
              <tr>
                <td class="style37">
                    <h1><asp:Label runat="server" ID="lbl_Titulo" style="font-weight: 700"></asp:Label></h1>
                </td>
                <td align="right">
                    <asp:Label ID="l_fechaActual" runat="server" style="font-weight: 700"></asp:Label>
                </td>
              </tr>
            </table>
            <table width="650px">
              <tr>
                <td colspan="4">&nbsp;</td>
              </tr>

              <tr>
                <td class="style36">&nbsp;</td>
                <td class="style33" valign="baseline">Número del Acta:</td>
                <td class="style35" valign="baseline" colspan="2">
                    <asp:Label ID="l_numActa" runat="server"></asp:Label>
                  </td>

              </tr>
              <tr>
                <td class="style36">&nbsp;</td>
                <td class="style33" valign="baseline">Nombre del Acta:</td>
                <td class="style35" valign="baseline" colspan="2">
                    <asp:Label ID="l_nombreActa" runat="server"></asp:Label>
                  </td>

              </tr>
              <tr>
                <td class="style36">&nbsp;</td>
                <td class="style33" valign="baseline">Fecha del Acta:</td>
                <td class="style35" valign="baseline" colspan="2">
                    <asp:Label ID="l_fecha" runat="server"></asp:Label>
                </td>

              </tr>
              <tr>
                <td class="style36">&nbsp;</td>
                <td class="style33" valign="top">Convocatoria:</td>
                <td class="style28" valign="baseline" colspan="2">
                    <asp:Label ID="l_convocatoria" runat="server"></asp:Label>
                </td>
              </tr>
              <tr>
                <td class="style36"></td>
                <td class="style33" valign="top">Observaciones:</td>
                <td class="style28" colspan="2">
                    <asp:Label ID="l_observaciones" runat="server"></asp:Label>
                  </td>

              </tr>
              <tr>
                <td colspan="4">&nbsp;</td>
              </tr>
            </table>
            <table width="650px">
               <tr>
                <td class="style30">&nbsp;</td>
                <td class="style31" ><strong>Planes de Negocio Incluidos:</strong> 
                  </td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style30">&nbsp;</td>
                <td class="style31">
                <asp:GridView ID="gv_ActaAsignacion"  CssClass="Grilla2" runat="server" 
                AllowPaging="false" AutoGenerateColumns="false" DataSourceID="lds_ActaAsignacion"
                EmptyDataText="No hay información disponible." Width="100%" 
                        onload="gv_ActaAsignacion_Load">
                    <Columns>
                        <asp:TemplateField HeaderText="Id">
                            <ItemTemplate>
                                <asp:Label ID="lId" runat="server" Text='<%# Eval("Id_Proyecto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plan de Negocio">
                            <ItemTemplate>
                                <asp:Label ID="LNombre" runat="server" Text='<%# Eval("NomProyecto") %>'></asp:Label>
                            </ItemTemplate> 
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Recursos (SMMLV)" >
                            <ItemTemplate>
                                <asp:HiddenField ID="hValor" runat="server" Value='<%# Eval("ValorRecomendado") %>' />
                                <asp:Label ID="lSalarios" runat="server" Text='<%# Eval("salarios") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Asignado" >
                            <ItemTemplate>
                                <asp:Label ID="lAsignados" runat="server" Text='<%# Eval("Asignado") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                </td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style30">&nbsp;</td>
                <td class="style31" align="right"><strong>Total (SMMLV):</strong>&nbsp;<asp:Label 
                        ID="l_totalsalarios" runat="server" Text=""></asp:Label>
                  </td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style30">&nbsp;</td>
                <td class="style31" align="right"><strong>Total:</strong>&nbsp;<asp:Label 
                        ID="l_Total" runat="server" Text=""></asp:Label></td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style30">&nbsp;</td>
                <td class="style31" align="center">
                    <hr />
                  </td>
                <td>&nbsp;</td>
              </tr>
            </table>

            <table width="650px">
              <tr>
                <td class="style11">&nbsp;</td>
                <td class="style18"><strong>Aprobó:</strong></td>
                <td class="style18">&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style21"></td>
                <td class="style22" valign="bottom">___________________________________</td>
                <td class="style22" valign="bottom">___________________________________</td>
                <td class="style23"></td>
              </tr>
              <tr>
                <td class="style11">&nbsp;</td>
                <td class="style20">Subgerente Financiero</td>
                <td class="style20">Subgerente Técnico</td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style21"></td>
                <td class="style29" valign="bottom">______________________________</td>
                <td class="style29" valign="bottom">______________________________</td>
                <td class="style23"></td>
              </tr>
              <tr>
                <td class="style11">&nbsp;</td>
                <td class="style20">Coordinador Grupo de Ejecución y Liquidación de Convenios</td>
                <td class="style20" valign="top">Gerente Unidad Crédito y Cartera</td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style21"></td>
                <td class="style29" valign="bottom">______________________________</td>
                <td class="style29"></td>
                <td class="style23"></td>
              </tr>
              <tr>
                <td class="style11">&nbsp;</td>
                <td class="style20">Gerente de Convenio Fondo Emprender</td>
                <td class="style20">&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style11">&nbsp;</td>
                <td class="style18">&nbsp;</td>
                <td class="style18">&nbsp;</td>
                <td>&nbsp;</td>
              </tr>
            </table>

    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .style11
        {
            width: 16px;
        }
        .style18
        {
            width: 395px;
        }
        .style20
        {
            width: 395px;
            font-weight: bold;
        }
        .style21
        {
            width: 16px;
            height: 80px;
        }
        .style22
        {
            width: 395px;
            height: 80px;
        }
        .style23
        {
            height: 80px;
        }
        .style28
        {
            width: 395px;
            height: 27px;
        }
        .style29
        {
            width: 395px;
            font-weight: bold;
            height: 80px;
        }
        .style30
        {
            height: 32px;
        }
        .style31
        {
            height: 35px;
        }
        .style33
        {
            width: 141px;
            font-weight: bold;
            height: 24px;
        }
        .style35
        {
            width: 12px;
            height: 27px;
        }
        .style36
        {
            width: 20px;
            font-weight: bold;
            height: 26px;
        }
        .style37
        {
            width: 364px;
        }
    </style>
</asp:Content>
