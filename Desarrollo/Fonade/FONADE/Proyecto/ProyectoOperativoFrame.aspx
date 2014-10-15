<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectoOperativoFrame.aspx.cs" Inherits="Fonade.FONADE.Proyecto.ProyectoOperativoFrame" %>

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
        
        <ajaxToolkit:TabContainer ID="tbc_Operativo" runat="server" ActiveTabIndex="0" Width="100%" Height="650px">
            <ajaxToolkit:TabPanel ID="tbc_OperativoPlanOperativo" OnDemandMode="Once" runat="server" Height="100%" >
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmOperativoPlanOperativo','PProyectoOperativoPlanOperativo.aspx')">
                        <span>Plan Operativo</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate >
                  <iframe id="frmOperativoPlanOperativo" src="PProyectoOperativoPlanOperativo.aspx" marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tbc_OperativoMetasSociales"  runat="server"   Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmOperativoMetasSociaes','PProyectoOperativoMetasSociales.aspx')">
                        <span>Metas Sociales</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmOperativoMetasSociaes" src="" marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>
    </div>
    </form>
</body>
</html>
