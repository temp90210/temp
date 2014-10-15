<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectoOperacionFrame.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.ProyectoOperacionFrame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </ajaxToolkit:ToolkitScriptManager>
        <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Width="100%"
            Height="660px">
            <ajaxToolkit:TabPanel ID="tb_operacionOperacion" runat="server" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmOperacion','PProyectoOperacion.aspx')">
                        <span>Operaci&oacute;n</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmOperacion" src="PProyectoOperacion.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                        height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tb_operacionCompras" OnDemandMode="Once" runat="server"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmPlanCompras','ProyectoOperacionCompras.aspx')">
                        <span>Plan de Compras</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmPlanCompras" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="auto" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tb_operacionCostos" OnDemandMode="Once" runat="server"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmCostosProduccion','PProyectoOperacionCostos.aspx')">
                        <span>Costos de Producci&oacute;n</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmCostosProduccion" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="auto" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tb_operacionInfraestructura" OnDemandMode="Once" runat="server"
               Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmInfraestructura','PProyectoOperacionInfraestructura.aspx')">
                        <span>Infraestructura</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmInfraestructura" src=""
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>
    </form>
</body>
</html>
