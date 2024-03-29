﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubirRespuestaPagos.aspx.cs" Inherits="Fonade.FONADE.interventoria.SubirRespuestaPagos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        table {
            width: 100%;
        }
    </style>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="overflow-y:scroll; width:700px; height:500px;">
        <asp:Panel ID="pnlprincipal" runat="server">
        <h1>
            <label>Carga Archivo</label>
        </h1>
        <br>
        <br />
        <table>
            <tr>
                <td colspan="2">Seleccione el archivo:</td>
            </tr>
            <tr>
                <td style="Width:50%;">
                    <asp:FileUpload ID="fuarchivo" runat="server" Width="500px" />
                    <br />
                    <asp:RequiredFieldValidator ID="rfvarchivo" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="fuarchivo" Text="'Debe seleccionar un archivo!!!'" ForeColor="#ff0000" ValidationGroup="subirarc"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <asp:Button ID="btnsu8bir" runat="server" OnClick="btnsu8bir_Click" ValidationGroup="subirarc" Text="Subir Archivo" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <p>Solo se debe subir el archivo de Información de Pagos generado por la Fiduciaria. Con el siguiente formato (colocar solo las filas de valores, incluir una sola fila de titulos, el nombre de la hoja electrónica debe ser Pagos ):</p>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <div>
            <table class="Grilla">
            <thead>
                <tr>
                    <th>>Numero Campo</th>
                    <th>Nombre Camp</th>
                    <th>Ejemplo</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>1</td>
                    <td>Consecutivo</td>
                    <td>1</td>
                </tr>
                <tr>
                    <td>2</td>
                    <td># Proyecto</td>
                    <td>1001383</td>
                </tr>
                <tr>
                    <td>3</td>
                    <td>Nombre Proyecto</td>
                    <td>TOMATES DEL CAFE</td>
                </tr>
                <tr>
                    <td>4</td>
                    <td>NIT/CC</td>
                    <td>900036033</td>
                </tr>
                <tr>
                    <td>5</td>
                    <td>Beneficiario</td>
                    <td>PADILLA GALVIS UBERNEY</td>
                </tr>
                <tr>
                    <td>6</td>
                    <td>No Documentoa a Pagar</td>
                    <td>11112005Pg1264.zip</td>
                </tr>
                <tr>
                    <td>7</td>
                    <td>Tipo de Cuenta</td>
                    <td>4</td>
                </tr>
                <tr>
                    <td>8</td>
                    <td># de Cuenta</td>
                    <td>7089011625374</td>
                </tr>
                <tr>
                    <td>9</td>
                    <td># OP</td>
                    <td>586</td>
                </tr>
                <tr>
                    <td>10</td>
                    <td>Fecha OP</td>
                    <td>15/11/2005</td>
                </tr>
                <tr>
                    <td>11</td>
                    <td>Vr. Bruto</td>
                    <td>1100000</td>
                </tr>
                <tr>
                    <td>12</td>
                    <td>Vr. RteFte</td>
                    <td>50000</td>
                </tr>
                <tr>
                    <td>13</td>
                    <td>Vr. RetIVA</td>
                    <td>35000</td>
                </tr>
                <tr>
                    <td>14</td>
                    <td>VR. RetICA</td>
                    <td>5000</td>
                </tr>
                <tr>
                    <td>15</td>
                    <td>Otros Dctos</td>
                    <td>5000</td>
                </tr>
                <tr>
                    <td>16</td>
                    <td>Vr. Pagado</td>
                    <td>1005000</td>
                </tr>
                <tr>
                    <td>17</td>
                    <td># ACH</td>
                    <td>13350270</td>
                </tr>
                <tr>
                    <td>18</td>
                    <td>Fecha de Pago</td>
                    <td>15/11/2005</td>
                </tr>
                <tr>
                    <td>19</td>
                    <td>Observaciones</td>
                    <td>ACEPTADA</td>
                </tr>
                <tr>
                    <td>20</td>
                    <td>Motivo de Rechazo</td>
                    <td>3-SOPORTES ILEGIBLES</td>
                </tr>
                <tr>
                    <td>21</td>
                    <td>Fecha Rechazo</td>
                    <td>16/11/2005</td>
                </tr>
                <tr>
                    <td>22</td>
                    <td>Fecha de Movimiento</td>
                    <td>10/11/2005  11:06:00 a.m.</td>
                </tr>
                <tr>
                    <td>22</td>
                    <td>OBSERVACION CAMBIO</td>
                    <td>Si existe una modificación se agrega este campo</td>
                </tr>
            </tbody>
        </table>
            </asp:Panel>
        </div>
        <br />
        <br />
    </div>
    </form>
</body>
</html>
