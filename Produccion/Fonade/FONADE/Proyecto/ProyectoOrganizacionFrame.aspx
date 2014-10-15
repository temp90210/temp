﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectoOrganizacionFrame.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.ProyectoOrganizacionFrame" %>

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
            <ajaxToolkit:TabContainer ID="tbc_Organizacion" runat="server" ActiveTabIndex="0"
                Width="100%" Height="660px">
                <ajaxToolkit:TabPanel ID="tbc_OrganizacionEstrategia" OnDemandMode="Once" runat="server"
                    Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmOrganizacionEstrategia','PProyectoOrganizacionEstrategia.aspx')">
                            <span>Estrategia Organizacional</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmOrganizacionEstrategia" src="PProyectoOrganizacionEstrategia.aspx"
                            marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                            height="550px"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbc_OrganizacionEstructura" runat="server" Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmOrganizacionEstructura','PProyectoOrganizacionEstructura.aspx')">
                            <span>Estructura Organizacional</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmOrganizacionEstructura" src="" marginwidth="0" marginheight="0" frameborder="0"
                            scrolling="auto" width="100%" height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbc_OrganizacionAspectos" runat="server" Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmOrganizacionAspectos','PProyectoOrganizacionAspectos.aspx')">
                            <span>Aspectos Legales</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmOrganizacionAspectos" src="" marginwidth="0" marginheight="0" frameborder="0"
                            scrolling="auto" width="100%" height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel ID="tbc_OrganizacionCostos" runat="server" Height="100%">
                    <HeaderTemplate>
                        <div class="tab_header" onclick="CargarPestana('frmOrganizacionCostos','PProyectoOrganizacionCostos.aspx')">
                            <span>Costos Administrativos</span>
                        </div>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <iframe id="frmOrganizacionCostos" src="" marginwidth="0" marginheight="0" frameborder="0" scrolling="auto"
                            width="100%" height="100%"></iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </div>
    </div>
    </form>
</body>
</html>