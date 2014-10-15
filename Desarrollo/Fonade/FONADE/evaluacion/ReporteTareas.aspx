<%@ Page Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="ReporteTareas.aspx.cs"
    Inherits="Fonade.FONADE.evaluacion.ReporteTareas1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1
        {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
    <table class="auto-style1">
        <thead>
            <tr>
                <th colspan="2" style="text-align: left; padding-left: 50px">
                    <h1>
                        <asp:Label ID="L_ReportesEvaluacion" runat="server" Text="MIS TAREAS" Width="260px"></asp:Label>
                    </h1>
                </th>
            </tr>
        </thead>
        <tr>
            <td>
                <br />
                <br />
                <div style="width: 700px; overflow-x: scroll;">
                    <asp:GridView ID="gvrtareas" runat="server" CssClass="Grilla" AutoGenerateColumns="False"
                        Width="100%" EmptyDataText="No hay tareas registradas" AllowSorting="true" AllowPaging="true"
                        OnPageIndexChanging="GridView1_PageIndexChanging" 
                        OnRowDataBound="gvrtareas_RowDataBound" onsorting="gvrtareas_Sorting">
                        <Columns>
                            <%--<asp:BoundField DataField="nomproyecto" HeaderText="Plan de Negocio" SortExpression="nomproyecto" />--%>
                            <asp:TemplateField HeaderText="Plan de Negocio" SortExpression="nomproyecto">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_nomProyecto" Text='<%# Eval("nomproyecto") %>' runat="server" />
                                    <asp:HiddenField ID="hdf_codcontactoagendo" runat="server" Value='<%# Eval("codcontactoagendo") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" DataFormatString="{0:MMM dd 'de' yyyy}"
                                HtmlEncode="false" />
                            <asp:BoundField DataField="nomtareaprograma" HeaderText="Tipo" SortExpression="nomtareaprograma" />
                            <asp:BoundField DataField="nomtareausuario" HeaderText="Tarea" SortExpression="nomtareausuario" />
                            <asp:BoundField DataField="descripcion" HeaderText="Descripcion" SortExpression="descripcion"
                                HtmlEncode="false" />
                            <%--<asp:BoundField DataField="agendo" HeaderText="Agendó" SortExpression="agendo" />--%>
                            <asp:TemplateField HeaderText="Agendó" SortExpression="nomproyecto">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_agendo_" Text='<%# Eval("agendo") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="fechacierre" HeaderText="Fecha Cierre" SortExpression="fechacierre"
                                ConvertEmptyStringToNull="true" DataFormatString="{0:MMM dd 'de' yyyy}" HtmlEncode="false" />
                            <asp:BoundField DataField="respuesta" HeaderText="Respuesta" SortExpression="respuesta" />
                        </Columns>
                    </asp:GridView>
                </div>
                <br />
                <br />
                <br />
            </td>
        </tr>
    </table>
</asp:Content>
