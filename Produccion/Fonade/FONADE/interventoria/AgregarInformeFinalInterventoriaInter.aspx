<%@ Page Language="C#" MasterPageFile="~/Master.master"  EnableEventValidation="false" AutoEventWireup="true" CodeBehind="AgregarInformeFinalInterventoriaInter.aspx.cs" Inherits="Fonade.FONADE.interventoria.AgregarInformeFinalInterventoriaInter" %>

 <asp:Content  ID="head1"   ContentPlaceHolderID="head" runat="server">
     <style type="text/css">
         table {
             width:100%;
         }
         td {
             vertical-align:top;
         }
     </style>
     <script type="text/javascript">

         function imprimir() {
             document.getElementById("oculto").style.visibility = "visible";

             var divToPrint = document.getElementById('contentPrincipal');
             var newWin = window.open('', 'Print-Window', 'width=1000,height=500');
             newWin.document.open();
             newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
             newWin.document.close();
             setTimeout(function () { newWin.close(); }, 1000);

             document.getElementById("oculto").style.visibility = "hidden";
         }

    </script>
    </asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <div id="contentPrincipal">
    <asp:Panel ID="principal" runat="server">
        <h1>
            <asp:Label ID="L_titulo" runat="server" Text="INFORMES DE INTERVENTORÍA">

            </asp:Label>
        </h1>
        <br />
        <br />
        <div id="contenido">
            <table>
                <thead>
                    <tr><th colspan="4"><asp:label ID="lblinforme" runat="server" Text="Interventor "></asp:label></th></tr>
                    <tr><th colspan="4"><asp:label ID="L_TituloNombre" runat="server" Text="Interventor "></asp:label></th></tr>
                    <tr><th colspan="4"><asp:label ID="lblnomcoordinador" runat="server" Text="Interventor "></asp:label></th></tr>
                </thead>
                <tbody>
                    <tr>
                        <td colspan="4"><br /><br /></td>
                    </tr>
                    <tr>
                        <td>Número Contrato:</td>
                        <td><asp:Label ID="lblnumContrato" runat="server" Text=""></asp:Label></td>
                        <td>
                            Fecha Informe:
                        </td>
                        <td><asp:Label ID="lblfechainforme" runat="server" Text=""></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="2">Empresa</td>
                        <td colspan="2">
                            <asp:Label ID="lblEmpresa" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">Teléfono</td>
                        <td colspan="2">
                            <asp:Label ID="lblTelefono" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">Dirección</td>
                        <td colspan="2">
                            <asp:Label ID="lblDireccion" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">Socios</td>
                        <td colspan="2">
                            <asp:Panel ID="pSocios" runat="server">
                                <asp:Table ID="t_table" runat="server">

                                </asp:Table>
                            </asp:Panel>
                        </td>
                    </tr>
                </tbody>
            </table>

            <br />
            <br />
            <asp:Panel ID="p_iB" runat="server" Width="100%">
                <asp:Table ID="t_variable" runat="server" class="Grilla">
                    <asp:TableHeaderRow>
                            <asp:TableHeaderCell>CRITERIO</asp:TableHeaderCell>
                            <asp:TableHeaderCell>CUMPLIMIENTO A VERIFICAR</asp:TableHeaderCell>
                            <asp:TableHeaderCell>OBSERVACIÓN INTERVENTOR</asp:TableHeaderCell>
                    </asp:TableHeaderRow>

                    <asp:TableRow>
                        <asp:TableCell ColumnSpan="3"></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
            <br />
            <br />
            <br />
            <div id="dvianexos">
                <asp:Panel ID="p_Anexos" runat="server">
                    <asp:Table ID="t_anexos" runat="server" class="Grilla">
                        <asp:TableHeaderRow>
                                <asp:TableHeaderCell ColumnSpan="2">ANEXOS</asp:TableHeaderCell>
                        </asp:TableHeaderRow>

                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="2"></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>

                <br />
                <br />
                
        </div>
            </div>

        <div id="oculto" style="visibility:hidden;">
            <p>Dadas las condiciones en que el Contratista se viene cumpliendo o incumpliendo, con las obligaciones del contrato, el INTERVENTOR recomienda FONADE:</p>
            <br />
            <br />
            <p>Para constancia firman:</p>
            <br />
            <br />
            ________________________________&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;________________________________
            <br />
            Interventor&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Contratista
        </div>
    </asp:Panel>
        </div>

    <div id="imprimir" style="text-align:center; width:100%;">
                    <asp:Button ID="btn_imprimir" runat="server" Text="Imprimir" OnClientClick="imprimir()" />
                </div>
</asp:Content>