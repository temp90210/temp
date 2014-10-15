<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/Emergente.Master" AutoEventWireup="true"
    CodeBehind="ReportePuntajeDetallado.aspx.cs" Inherits="Fonade.FONADE.evaluacion.ReportePuntajeDetallado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/Site.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
    <br />
    <table class="style10" align="center">
        <tr>
            <td>
                <h1>
                    <asp:Label ID="lbltitulo" runat="server" Style="font-weight: 700" Text="Reporte de Puntaje de Evaluación" />
                </h1>
                <asp:Label ID="bltituloConvocatoria" runat="server" Style="font-weight: 700" />
            </td>
        </tr>
    </table>
    <br />
    <div style="overflow: scroll; width: 100%;" align="center">
        <asp:DataList runat="server" ID="DtlReporteDetallado" CssClass="Grilla" OnItemDataBound="DtlReporteDetallado_ItemDataBound">
            <HeaderTemplate>
                <table border='0' cellspacing='1' cellpadding='4' width="100%">
                    <tr>
                        <th>
                            Municipio
                        </th>
                        <th>
                            Unidad de Emprendimiento
                        </th>
                        <th>
                            ID
                        </th>
                        <th>
                            Plan de Negocio
                        </th>
                        <th>
                            Viable
                        </th>
                        <th>
                            Valor Solicitado
                        </th>
                        <th>
                            Valor Recomendado
                        </th>
                        <th>
                            A
                        </th>
                        <th>
                            B
                        </th>
                        <th>
                            C
                        </th>
                        <th>
                            D
                        </th>
                        <th>
                            E
                        </th>
                        <th>
                            Generales
                        </th>
                        <th>
                            F
                        </th>
                        <th>
                            G
                        </th>
                        <th>
                            H
                        </th>
                        <th>
                            I
                        </th>
                        <th>
                            J
                        </th>
                        <th>
                            K
                        </th>
                        <th>
                            L
                        </th>
                        <th>
                            M
                        </th>
                        <th>
                            N
                        </th>
                        <th>
                            O
                        </th>
                        <th>
                            P
                        </th>
                        <th>
                            Q
                        </th>
                        <th>
                            R
                        </th>
                        <th>
                            S
                        </th>
                        <th>
                            Comerciales
                        </th>
                        <th>
                            T
                        </th>
                        <th>
                            U
                        </th>
                        <th>
                            V
                        </th>
                        <th>
                            W
                        </th>
                        <th>
                            X
                        </th>
                        <th>
                            Y
                        </th>
                        <th>
                            Técnicos
                        </th>
                        <th>
                            Z
                        </th>
                        <th>
                            AA
                        </th>
                        <th>
                            AB
                        </th>
                        <th>
                            AC
                        </th>
                        <th>
                            AD
                        </th>
                        <th>
                            AE
                        </th>
                        <th>
                            AF
                        </th>
                        <th>
                            Organizacionales
                        </th>
                        <th>
                            AG
                        </th>
                        <th>
                            AH
                        </th>
                        <th>
                            AI
                        </th>
                        <th>
                            AJ
                        </th>
                        <th>
                            AK
                        </th>
                        <th>
                            Financieros
                        </th>
                        <th>
                            AL
                        </th>
                        <th>
                            Medio Ambiente
                        </th>
                        <th>
                            PuntajeTotal
                        </th>
                    </tr>
                </table>
            </HeaderTemplate>
            <ItemTemplate>
                <table border='0' cellspacing='1' cellpadding='4'>
                    <tr>
                        <td>
                            <asp:Label ID="lblciuadd" runat="server" Text=' <%#Eval("nomciudad")%>' />'
                        </td>
                        <td>
                            <asp:Label ID="lblinstitucion" runat="server" Text='<%#Eval("nominstitucion")%>' />
                        </td>
                        <td>
                            <asp:Label ID="lblidproyecto" runat="server" Text=' <%#Eval("Id_Proyecto")%>' />
                        </td>
                        <td>
                            <asp:Label ID="lblnombreproyecto" runat="server" Text=' <%#Eval("nomproyecto")%>' />
                        </td>
                        <td>
                            <asp:Label ID="lblviable" runat="server" Text=' <%#Eval("viable")%>' />
                        </td>
                        <td>
                            <asp:Label ID="lblsolicitado" runat="server" Text=' <%#Eval("montosolicitado")%>' />
                        </td>
                        <td>
                            <asp:Label ID="lblrecomendado" runat="server" Text=' <%#Eval("montorecomendado")%>' />
                        </td>
                        <!-- INCIO  DEL TABL GENERALES -->
                        <td>
                            <asp:Label ID="lblga" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblgb" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblgc" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblgd" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblge" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbltotalG" runat="server" />
                        </td>
                        <!-- FIN GENERALES -->

                        <!-- INCIO COMERCIALES -->
                        <td>
                            <asp:Label ID="lblcc" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcg" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblch" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblci" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcj" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblck" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcl" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcm" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcn" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblco" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcp" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcq" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcr" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblcs" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbltotalC" runat="server" />
                        </td>
                        <!-- FIN DEL TABL COMERCIALES -->
                        <!-- INCIO DEL TAB TECNICOS -->
                        <td>
                            <asp:Label ID="lbltt" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbltu" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbltv" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbltw" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbltx" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblty" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalT" runat="server" />
                        </td>
                        <!-- FIN DEL TECNICOS -->
                        <!--  INICIO TAB ORGANIZACIONALES -->
                        <td>
                            <asp:Label ID="lbloz" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbloaa" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbloab" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbloac" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbload" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbloae" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lbloaf" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalO" runat="server" />
                        </td>
                        <!--  FIN  ORGANIZACIONALES -->
                        <!--  INICIO  FINANCIEROS -->
                        <td>
                            <asp:Label ID="lblfag" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblfah" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblfai" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblfaj" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblfak" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalF" runat="server" />
                        </td>
                        <!--  FIN  FINANCIEROS -->
                        <td>
                            <asp:Label ID="lblTotalM" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lpuntajetotal" runat="server" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:DataList>
        <br />
    </div>
</asp:Content>
