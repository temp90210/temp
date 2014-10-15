<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterContratoFrame.aspx.cs"
    Inherits="Fonade.FONADE.interventoria.InterContratoFrame" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.9.1.js"></script>
    <link href="../../Styles/Site.css" rel="stylesheet" />
    <script src="../../Scripts/ScriptsGenerales.js"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
        table
        {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ContentInfo" style="width: 995px; height: auto;">
        <div>
            <asp:Panel ID="panexosagre" runat="server">
                <h1>
                    <label>
                        INFORMACIÓN DEL CONTRATO</label>
                </h1>
                <br />
                <br />
                <table class="Grilla">
                    <tr>
                        <td>
                            No Contrato de Colaboración Empresarial:
                        </td>
                        <td>
                            <asp:Label ID="lblNumContrato" runat="server"></asp:Label>
                        </td>
                        <td>
                            Plazo en meses del ctto:
                        </td>
                        <td>
                            <asp:Label ID="lblplazoMeses" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Fecha de Acta de Inicio:
                        </td>
                        <td>
                            <asp:Label ID="lblFechaActa" runat="server"></asp:Label>
                        </td>
                        <td>
                            Numero del ap presupuestal:
                        </td>
                        <td>
                            <asp:Label ID="lblNumAppresupuestal" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Objeto:
                        </td>
                        <td colspan="3">
                            <asp:Label ID="lblObjeto" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Fecha del ap:
                        </td>
                        <td>
                            <asp:Label ID="lblFechaAp" runat="server"></asp:Label>
                        </td>
                        <td>
                            Fecha Firma Del Contrato:
                        </td>
                        <td>
                            <asp:Label ID="lblFechaFirmaContrato" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            No Póliza de Seguro de Vida:
                        </td>
                        <td>
                            <asp:Label ID="lblPolizaSeguro" runat="server"></asp:Label>
                        </td>
                        <td>
                            Compañía Seguro de Vida:
                        </td>
                        <td>
                            <asp:Label ID="lblCompaniaSeguroVida" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Valor inicial en pesos:
                        </td>
                        <td>
                            <asp:Label ID="lblValorInicial" runat="server"></asp:Label>
                        </td>
                        <td colspan="2">
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table class="Grilla">
                    <tr>
                        <td>
                            Archivos Adjuntos:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="panexos" runat="server">
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <asp:Button ID="lnkSuberArchivo" runat="server" Text="Cargar Archivo Adjunto" OnClick="lnkSuberArchivo_Click1"
                    CssClass="boton_Link_Grid"></asp:Button>
            </asp:Panel>
            <asp:Panel ID="Adjunto" runat="server">
                Carga Archivo
                <table>
                    <tr>
                        <td>
                            Seleccione el archivo:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblErrorDocumento" runat="server" CssClass="failureNotification"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:FileUpload ID="fuArchivo" runat="server" Width="240px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnSubirDocumento" runat="server" Text="Cargar Archivo" OnClick="btnSubirDocumento_Click" />
                            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Por favor de click en examinar para escoger los archivos Adjuntos
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
    </div>
    </form>
</body>
</html>
