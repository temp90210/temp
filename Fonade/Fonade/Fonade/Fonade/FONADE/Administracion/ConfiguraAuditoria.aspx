<%@ Page Language="C#" MasterPageFile="~/Master.master"  EnableEventValidation="false" AutoEventWireup="true" CodeBehind="ConfiguraAuditoria.aspx.cs" Inherits="Fonade.FONADE.Administracion.ConfiguraAuditoria" %>

 <asp:Content  ID="head1"   ContentPlaceHolderID="head" runat="server">
     <style type="text/css">
         table {
             width:100%;
         }
         td {
             vertical-align:top;
         }
     </style>
    </asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <h1>
        <label>CONFIGURAR CAMPOS A AUDITAR</label>
    </h1>
    <br />
    <br />
    <table>
        <tr>
            <td>Tabla:</td>
            <td>
                <asp:DropDownList ID="ddltablas" runat="server" DataTextField="TABLE_NAME" DataValueField="TABLE_NAME" Height="16px" Width="500px" OnSelectedIndexChanged="ddltablas_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="vertical-align:top;">Campos</td>
            <td>

                <asp:Panel ID="pnlcampos" runat="server">
                    <asp:Table ID="tblcampos" runat="server"></asp:Table>
                </asp:Panel>

            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:right;">
                <asp:Button ID="btnguardar" runat="server" Text="Guardar Configuración" OnClick="btnguardar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>