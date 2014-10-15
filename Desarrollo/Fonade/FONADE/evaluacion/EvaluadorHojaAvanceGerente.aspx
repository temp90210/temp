<%@ Page Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="EvaluadorHojaAvanceGerente.aspx.cs" Inherits="Fonade.FONADE.evaluacion.EvaluadorHojaAvanceGerente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
    .auto-style1 {
        width: 100%;
    }
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">

    <table class="auto-style1">
        <thead>
            <tr>
                <th colspan="2" style="background-color:#00468f; text-align:left; padding-left:50px">
                    <asp:Label ID="L_ReportesEvaluacion" runat="server" ForeColor="White" Text="Hoja de Avance Evaluación" Width="260px"></asp:Label>
                </th>
            </tr>
        </thead>
        
        <tr>
            <td>
                <br />
                <asp:GridView ID="GV_Reporte" runat="server" CssClass="Grilla" AutoGenerateColumns="False" AllowPaging="True" DataSourceID="ODS_Contacto" DataKeyNames="Id">
                    <Columns>

                        <asp:BoundField DataField="Id" HeaderText="Id" />
                        <asp:TemplateField HeaderText="Nombre">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Nombre") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="LB_Contacto" runat="server" OnClick="LB_Contacto_Click">
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Nombre") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                    </Columns>
                </asp:GridView>
                <asp:ObjectDataSource ID="ODS_Contacto" runat="server" SelectMethod="contacto" TypeName="Fonade.FONADE.evaluacion.EvaluadorHojaAvanceGerente"></asp:ObjectDataSource>
                <br />
            </td>
        </tr>
    </table>

</asp:Content>