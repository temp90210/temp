<%@ Page Title="FONDO EMPRENDER" Language="C#" AutoEventWireup="true" CodeBehind="VerInterventoresCoordinador.aspx.cs" Inherits="Fonade.FONADE.interventoria.VerInterventoresCoordinador" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>



<body>
       <form id="form1" runat="server">
           
    <asp:LinqDataSource ID="lds_eval" runat="server" 
        ContextTypeName="Datos.FonadeDBDataContext" AutoPage="true" 
        onselecting="lds_eval_Selecting">
    </asp:LinqDataSource>

    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"> </asp:ToolkitScriptManager>
    <div class="ContentInfo" style="width:995px; height:auto;">
            <table width="600px">
              <tr>
                <td class="auto-style1" >
                    <h1><asp:Label runat="server" ID="lbl_Titulo" style="font-weight: 700"></asp:Label></h1>
                </td>
                <td align="right">
                    <asp:Label ID="l_fechaActual" runat="server" style="font-weight: 700"></asp:Label>
                </td>
              </tr>
            </table>

            <table width="600">
              <tr>
                <td class="style10">&nbsp;</td>
                <td class="style11">                   
                <asp:Panel ID="Panel1" runat="server" Width="100%" Height="250px" ScrollBars="Vertical">
                <asp:GridView ID="gvevaluadores"  CssClass="Grilla" runat="server" DataSourceID="lds_eval" 
                   AllowPaging="false" AutoGenerateColumns="false" Width="100%" >
                        <Columns>
                           <asp:TemplateField HeaderText="Nombre">
                                <ItemTemplate> 
                                    <asp:Label ID="lnombre" runat="server" Text='<%# Eval("nombre") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField> 
                            <asp:TemplateField HeaderText="Empresa">
                                <ItemTemplate>
                                    <asp:Label ID="lplanes" runat="server" Text='<%# Eval("nomproyecto") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </asp:Panel>
                    </td>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style12"></td>
                <td class="style13" align="center">
                    <asp:Button ID="btn_cerrar" runat="server" onclick="btn_cerrar_Click" 
                        Text="Cerrar" />
                  </td>
                <td class="style14"></td>
              </tr>

            </table>

    </div>
    </form>

   </body>

<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <style type="text/css">
         table {
             width: 100%;
         }
        .celdaest {
            text-align:center;
        }
     </style>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" />
    <div id="contentPrincipal">
        </div>
    </head>
