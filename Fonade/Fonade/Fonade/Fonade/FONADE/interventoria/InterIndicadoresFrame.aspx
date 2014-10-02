<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterIndicadoresFrame.aspx.cs" Inherits="Fonade.FONADE.interventoria.InterIndicadoresFrame" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.9.1.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
            </ajaxToolkit:ToolkitScriptManager>
            <ajaxToolkit:TabContainer ID="tc_proyectos" runat="server" ActiveTabIndex="0" Width="100%"
                Height="80%">
                <ajaxToolkit:TabPanel ID="tbaportes" OnDemandMode="Once" runat="server" Width="100%"
                    Height="100%">
                    <HeaderTemplate>
                        <div id="aportesdiv" class="tab_header" onclick="CargarPestana('frmEvalAportes','InterIndicadoresGenericos.aspx')">
                            <span>Indicadores Genéricos</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmEvalAportes" src="InterIndicadoresGenericos.aspx"
                            marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                            height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbpresupuesto" runat="server" Width="100%" Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmEvalPresupuesto','InterIndicadores.aspx')">
                            <span>Indicadores Específicos</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmEvalPresupuesto" src="" marginwidth="0" marginheight="0" frameborder="0"
                            scrolling="auto" width="100%" height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>

            </ajaxToolkit:TabContainer>
        </div>
    </form>
</body>
</html>
