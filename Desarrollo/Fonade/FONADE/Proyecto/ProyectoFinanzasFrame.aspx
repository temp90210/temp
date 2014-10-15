<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectoFinanzasFrame.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.ProyectoFinanzasFrame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </ajaxToolkit:ToolkitScriptManager>
        <div>
            <ajaxToolkit:TabContainer ID="tbc_finanzas" runat="server" ActiveTabIndex="0" Width="100%"
                Height="660px">
                <ajaxToolkit:TabPanel ID="tb_finanzasIngreso" OnDemandMode="Once" runat="server"
                    Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmFinanzasIngreso','PProyectoFinanzasIngreso.aspx')">
                            <span>Ingresos</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmFinanzasIngreso" src="PProyectoFinanzasIngreso.aspx" marginwidth="0"
                            marginheight="0" frameborder="0" scrolling="auto" width="100%" height="550px">
                        </iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tb_finanzasEgreso" runat="server" Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmFinanzasEgreso','PProyectoFinanzasEgreso.aspx')">
                            <span>Egresos</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmFinanzasEgreso" src="" marginwidth="0" marginheight="0" frameborder="0"
                            scrolling="auto" width="100%" height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tb_finanzasCapital" OnDemandMode="Once" runat="server"
                    HeaderText="" Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmFinanzasCapital','PProyectoFinanzasCapitalTrabajo.aspx')">
                            <span>Capital de Trabajo</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmFinanzasCapital" src="" marginwidth="0" marginheight="0" frameborder="0"
                            scrolling="auto" width="100%" height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </div>
    </div>
    </form>
</body>
</html>
