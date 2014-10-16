<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectoMercadoFrame.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.ProyectoMercadoFrame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.8.21.custom.min.js" type="text/javascript"></script>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </ajaxToolkit:ToolkitScriptManager>
        <div>
            <ajaxToolkit:TabContainer ID="tbc_mercado" runat="server" ActiveTabIndex="0" Width="100%"
                Height="661px">
                <ajaxToolkit:TabPanel ID="tb_mercadoInvestigacion" runat="server" Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmInvestigacionMercados','PProyectoMercadoInvestigacion.aspx')">
                            <span>Investigación de Mercados</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmInvestigacionMercados" src="PProyectoMercadoInvestigacion.aspx"
                            marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                            height="550px"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tb_mercadoEstrategia" OnDemandMode="Once" runat="server"
                    Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmEstrategiaMercado','PProyectoMercadoEstrategia.aspx')">
                            <span>Estrategias de Mercado</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmEstrategiaMercado" src="" marginwidth="0" marginheight="0" frameborder="0"
                            scrolling="auto" width="100%" height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tb_mercadoProyecciones" OnDemandMode="Once" runat="server"
                    HeaderText="" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmProyeccionVentas','PProyectoMercadoProyecciones.aspx')">
                        <span>Proyecciones de Ventas</span>
                    </div>
                </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmProyeccionVentas" src="" marginwidth="0"
                            marginheight="0" frameborder="0" scrolling="auto" width="100%" height="100%">
                        </iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </div>
    </div>
    </form>
</body>
</html>
