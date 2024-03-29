﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EvaluacionFinanciera.aspx.cs" Inherits="Fonade.FONADE.evaluacion.EvaluacionFinanciera" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }

        .lineas {
            border: none;
            border-collapse: collapse;
            border-color: none;
        }
    </style>
    <script type="text/javascript">
        function redirect() {
            window.location = "CatalogoItem.aspx";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td>
                        <br />
                        <div class="help_container">
                            <div onclick="textoAyuda({titulo: 'Desempeño del Evaluador', texto: 'DesempenoEvaluador'});">
                                <img alt="help_Objetivos" border="0" src="../../Images/imgAyuda.gif" />
                            </div>
                            <div>
                                &nbsp; <strong>Desempeño del Evaluador:</strong>&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="L_Nombre" runat="server"></asp:Label>
                            </div>
                        </div>

                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnladicionar" runat="server">
                            <div class="help_container" onclick="redirect()">
                                <div style="cursor: pointer; width: auto">
                                    <img alt="help_Objetivos" border="0" src="../../Images/icoAdicionarUsuario.gif" />
                                </div>
                                <div style="cursor: pointer; width: auto">
                                    &nbsp; <strong>Adicionar Item</strong>
                                </div>
                            </div>
                        </asp:Panel>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        
                        <asp:LinqDataSource ID="lds_item" runat="server" ContextTypeName="Datos.FonadeDBDataContext" OnSelecting="lds_item_Selecting" AutoPage="true"></asp:LinqDataSource>
                        <asp:GridView ID="GV_Item" runat="server" DataSourceID="lds_item" CssClass="Grilla" AutoGenerateColumns="False" DataKeyNames="Id_Item,Puntaje" AllowPaging="True" OnPageIndexChanging="GV_Item_PageIndexChanging" Width="100%" OnRowCreated="GV_Item_RowCreated">
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LB_EliminarItem" runat="server" Text="" CssClass="lineas" OnClick="LB_EliminarItem_Click" OnClientClick="return confirm('¿Desea eliminar este item?')">
                                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/icoBorrar.gif" CssClass="lineas" />
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Id_Item" Visible="False" />
                                <asp:TemplateField HeaderText="Item">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="lbl_nombreItem" runat="server" Text='<%# Bind("NomItem") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LB_EditarItem" runat="server" Text="" OnClick="LB_EditarItem_Click" Enabled="false">
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("NomItem") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Protegido" HeaderText="Protegido" Visible="False" />
                                <asp:TemplateField HeaderText="Puntaje">
                                    <ItemTemplate>
                                        
                                        <asp:DropDownList ID="DDL_Puntaje" runat="server" Width="250px" DataTextField="Texto" DataValueField="Puntaje">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="Grilla">
                            <thead>
                                <tr>
                                    <th>

                                        <asp:Label ID="L_PuntajeObtenido" runat="server" Text="Puntaje Obtenido:"></asp:Label>

                                    </th>
                                    <th style="text-align: center;">

                                        <asp:Label ID="L_TotalPuntajeObtenido" runat="server" Width="200px"></asp:Label>
                                    </th>
                                </tr>
                            </thead>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btn_Actualizar" runat="server" Text="Actualizar" OnClick="btn_Actualizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <br />
        </div>
    </form>
</body>
</html>
