﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EvaluacionModeloFrame.aspx.cs" Inherits="Fonade.FONADE.evaluacion.EvaluacionModeloFrame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>


</head>
<body>
    <form id="form1" runat="server">
    <div>
           <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </ajaxToolkit:ToolkitScriptManager>
        <ajaxToolkit:TabContainer ID="tc_proyectos" runat="server" ActiveTabIndex="0" Width="100%"
            Height="480px">
            <ajaxToolkit:TabPanel ID="tc_observaciones" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div id="indicadorfinanciero" class="tab_header" onclick="CargarPestana('frmIndicadores','EvaluacionIndicadoresFinancieros.aspx')">
                        <span>Indicadores Financieros</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmIndicadores" src=""
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_modelo" runat="server" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmModelo','EvaluacionModeloFinanciero.aspx')">
                      <span>Cargue Modelo Financiero</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmModelo" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="auto" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_centrales" runat="server" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmcentral','EvaluacionCentrales.aspx')">
                      <span>Centrales de Riesgo</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmcentral" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="auto" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_evalua" runat="server" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmevalua','EvaluacionProyecto.aspx')">
                      <span>Evaluación del Proyecto</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmevalua" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="auto" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            
               <ajaxToolkit:TabPanel ID="tc_flujo" runat="server" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmflujo','EvaluacionFlujoCaja.aspx')">
                      <span>Flujo Caja</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmflujo" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="auto" width="100%" height="530px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>
    </form>
    <script type="text/javascript">
        $(document).on("ready", cargarTab);

        function cargarTab() {
            
            $("#indicadorfinanciero").click();
        }
    </script>
</body>
</html>
