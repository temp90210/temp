<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true"
    CodeBehind="SubirCifin.aspx.cs" Inherits="Fonade.FONADE.evaluacion.SubirCifin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style10
        {
            height: 44px;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function CheckFileExtension(sender, args) {

            /*Validation for file extension (csv)*/

            var inputObj = document.getElementById('<%=FileUploadArchive.ClientID %>').value;
            var fileExtension = inputObj.substring(inputObj.lastIndexOf('.') + 1).toLowerCase();

            if (!(fileExtension == 'xls')) {
                args.IsValid = false;
                document.getElementById('<%=FileUploadArchive.ClientID %>').value = "";
            }
            else {
                if (inputObj == "Cifin" || inputObj == "CIFIN") {
                    args.IsValid = true;
                } else {
                    args.IsValid = true;
                }

            }

        }
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
    <br />
    <table id="Table1" border="0" cellpadding="5" cellspacing="0" width="100%">
        <tr>
            <td class="style10">
                <h1>
                    <asp:Label ID="lbltitulo" runat="server" Style="font-weight: 700" Text="Cargue  Cifin" />
                </h1>
            </td>
        </tr>
        <tr>
            <td>
                <table id="_ctl5_rblSearch" border="0" cellpadding="0" cellspacing="0" class="cssLabelForm">
                    <tr>
                        <td style="width: 10%; height: 30px;">
                            <asp:FileUpload ID="FileUploadArchive" runat="server" Width="300px" />
                        </td>
                        <td style="width: 5%;">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="FileUploadArchive"
                                ErrorMessage="Seleccione un archivo" Font-Size="Small" ForeColor="Red" ValidationGroup="Grupo2">*</asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 15%;">
                            <asp:CustomValidator ID="CustomValidator2" ForeColor="Red" runat="server" ErrorMessage="Nombre incorrecto o extensión inválida"
                                Font-Size="Small" ClientValidationFunction="CheckFileExtension" OnServerValidate="ValidateNombre"
                                ValidationGroup="Grupo2">*</asp:CustomValidator>
                        </td>
                        <td style="width: 15%;">
                            &nbsp;
                        </td>
                        <td style="width: 15%;">
                            &nbsp;&nbsp;
                            <asp:HyperLink ID="HyperLink1" runat="server" ForeColor="Red" Target="_blank" NavigateUrl="~/FONADE/Plantillas/Cifin.xls">Formato Ejemplo</asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="left" style="width: 70%;">
                <asp:Button ID="btnsubir" runat="server" Text="Subir" ValidationGroup="Grupo2" OnClick="btnsubir_Click" />
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table style="height: 70px; width: 100%;">
                    <tr>
                        <td style="width: 100%; color: #CC0000;" align="left" >
                            Solo se debe subir el archivo de Información de Cartera generado por la CIFIN. Con
                            el siguiente formato (Descargue el Formato de Ejemplo, el nombre tiene que ser el
                            mismo (Cifin) y Extensión:
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100%" align="center">
                            <asp:ValidationSummary ID="ValidationSummary6" runat="server" ValidationGroup="Grupo2"
                                Font-Size="Small" ForeColor="Red" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100%" align="center">
                            <asp:Button ID="volver" runat="server" Text="Volver" Style="display: none" ValidationGroup="Grupo4" />
                            <asp:ValidationSummary ID="ValidationSummary7" runat="server" ValidationGroup="Grupo1"
                                Font-Size="Small" ForeColor="Green" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
