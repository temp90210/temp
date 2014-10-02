<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true"
    CodeBehind="ConsultaPrueba.aspx.cs" Inherits="Fonade.FONADE.Prueba.ConsultaPrueba" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .divTitulo1
        {
            color: Blue;
            font-family: @Arial Unicode MS;
        }
        .divTitulo2
        {
            color:White;
            background-color:Blue;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
    <div class="divTitulo1">
        <asp:Label ID="lblPlanes" runat="server">Planes de Negocio</asp:Label>
    </div>
    <div >
        <table width="100%">
            <tr class="divTitulo2">
                <td  colspan="2">
                    B&uacute;squeda por palabra
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="lblBusquedaPalabra" runat="server" >Por Palabra:</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtBusquedaPalabra" runat="server" Width="180px"></asp:TextBox>
                    <asp:Button ID="btnBuscarPorPalabra" runat="server"  Text="Buscar" />
                </td>
            </tr>
        </table>
    </div>
    <br />

    <div >
        <table width="100%">
            <tr class="divTitulo2">
                <td  colspan="2">
                    B&uacute;squeda avanzada
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="Label1" runat="server" >C&oacute;digo:</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtCodigo" runat="server" Width="180px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="lblDepartamento" runat="server" >Departamento:</asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlDepartamento"  runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="lblMunicipio" runat="server" >Municipio:</asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlMunicipio"  runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="lblSector" runat="server" >Sector:</asp:Label>
                </td>
                <td>
                    <asp:ListBox ID="lsb_sector" runat="server"></asp:ListBox>
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="lblEstado" runat="server" >Estado:</asp:Label>
                </td>
                <td>
                    <asp:ListBox ID="lsbEstado" runat="server"></asp:ListBox>
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="lblUnidad" runat="server" >Unidad Emprendimiento:</asp:Label>
                </td>
                <td>
                    <asp:ListBox ID="lsbEmprendimiento" runat="server"></asp:ListBox>
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="lblAsesor" runat="server" >Asesor:</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtAsesor" runat="server"></asp:TextBox>
                    <asp:Button ID="btnBuscarAsesores" runat="server" Text="BuscarAsesor"  />
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    &nbsp;
                </td>
                <td>
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar"  />
                </td>
            </tr>
            <tr>
                <td style="width:100px">
                    <asp:Label ID="Label2" runat="server" >Incluir Descripcion:</asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkIncluir" runat="server" Text="Incluir descripcion"  />          
                </td>
            </tr>
            <tr>
                <td colspan=2 align="right">
                       <asp:Button ID="btnBuscarAvanzada" runat="server"  Text="Buscar" />
                </td>
            </tr>
        </table>
    </div>
<br />
    <div class="divTitulo1">
        <asp:Label ID="Label4" runat="server">Emprendedore, asesores</asp:Label>
    </div>

    <div >
        <table width="100%">
            <tr class="divTitulo2">
                <td  colspan="2">
                    B&uacute;squeda por palabra
                </td>
            </tr>
            <tr>
                <td style="width:200px">
                    <asp:Label ID="Label3" runat="server" >Por C&eacute;dula o Palabra:</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox1" runat="server" Width="180px"></asp:TextBox>
                    <asp:Button ID="Button1" runat="server"  Text="Buscar" />
                </td>
            </tr>
            <tr>
                <td  colspan="2">
                    <asp:RadioButtonList ID="rdbOpciones" runat="server" 
                        RepeatDirection="Horizontal" >
                        <asp:ListItem Text="Emprendedor" Value="E"></asp:ListItem>
                        <asp:ListItem Text="Asesor" Value="A"></asp:ListItem>
                        <asp:ListItem Text="Todos" Value="T"></asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Button ID="btnBuscarEmprendedores" runat="server"  Text="Buscar" />
                </td>
            </tr>

        </table>
    </div>
    <div class="divTitulo1">
        <asp:Label ID="Label5" runat="server">Jefes de unidad</asp:Label>
    </div>
    <div >
        <table width="100%">
            <tr class="divTitulo2">
                <td  colspan="2">
                    B&uacute;squeda por palabra
                </td>
            </tr>
            <tr>
                <td style="width:200px">
                    <asp:Label ID="Label6" runat="server" >Por C&eacute;dula o Palabra:</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox2" runat="server" Width="180px"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="divTitulo1">
        <asp:Label ID="Label7" runat="server">Jefes de unidad</asp:Label>
    </div>
    <div >
        <table width="100%">
            <tr class="divTitulo2">
                <td  colspan="2">
                    B&uacute;squeda por palabra
                </td>
            </tr>
            <tr>
                <td style="width:200px">
                    <asp:Label ID="Label8" runat="server" >Por C&eacute;dula o Palabra:</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox3" runat="server" Width="180px"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
