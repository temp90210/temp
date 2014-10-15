<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectoFrameSet.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.ProyectoFrameSet" MasterPageFile="~/FONADE/Proyecto/Proyecto.Master" %>

<asp:Content runat="server" ID="ProyectosFrameBodyHolder" ContentPlaceHolderID="bodyHolder">
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //$("#padre #bodyHolder_tc_proyectos #bodyHolder_tc_proyectos_header #bodyHolder_tc_proyectos_tc_mercado_tab .ajax__tab_outer .ajax__tab_inner .ajax__tab_tab span .tab_header").on("click", function () {
            //    alert();
            //});
            //$("iframe").attr("height", "550px !important");
            //$("#padre").attr("height", "550px !important");
            function myFunction() {
                //$("#padre div div:nth-child(2) div:nth-child(1) iframe").css("height", "550px !important");
                //alert($("#padre div div:nth-child(2) div:nth-child(1) iframe #document html").css("height"));
                //var resultado = $("#padre #" + filtro1 + ":nth-child(2)").attr("id");
                //console.log("Este es el id del filtro 2 "+resultado);
            }
            //setInterval(myFunction, 2000);
        });
    </script>
    <style type="text/css">
        iframe
        {
            min-height: 700px;
        }
        .ContentInfo
        {
            margin-top: 6px !important;
        }
        .ajax__tab_xp .ajax__tab_header
        {
            font-size: 9px !important;
        }
        .tab_header .tab_aprobado:before
        {
            font-size: 12px !important;
        }
    </style>
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <div id="padre">
        <ajaxToolkit:TabContainer ID="tc_proyectos" runat="server" ActiveTabIndex="0" Width="100%"
            Height="100%">
            <ajaxToolkit:TabPanel ID="tc_mercado" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmMercado','ProyectoMercadoFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_Mercado, "Mercado") %>"></li>
                        <span>Mercado</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_Mercado, "Mercado") %>"></img>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmMercado" src="ProyectoMercadoFrame.aspx" marginwidth="0" marginheight="0"
                        frameborder="0" scrolling="no" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_operacion" runat="server" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmProyectoOperacion','ProyectoOperacionFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_Operacion, "Operacion") %>"></li>
                        <span>Operacion</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_Operacion, "Operacion") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmProyectoOperacion" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_organizacion" runat="server" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmProyectoOrganizacion','ProyectoOrganizacionFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_Organizacion, "Organizacion") %>"></li>
                        <span>Organización</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_Organizacion, "Organizacion") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmProyectoOrganizacion" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_finanzas" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmProyectoFinanzas','ProyectoFinanzasFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_Finanzas, "Finanzas") %>"></li>
                        <span>Finanzas</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_Finanzas, "Finanzas") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmProyectoFinanzas" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_planOperativo" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmPlanOperativo','ProyectoOperativoFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_PlanOperativo, "Operativo") %>"></li>
                        <span>Plan Operativo</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_PlanOperativo, "Operativo") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <!-- Este contenedor de iframes de plan Operativo -->
                    <iframe id="frmPlanOperativo" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" style="height: 685px !important;"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_impacto" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmProyectoImpacto','ProyectoImpacto.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_Impacto, "Impacto") %>"></li>
                        <span>Impacto</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_Impacto, "Impacto") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmProyectoImpacto" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_resumen" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmProyectoResumen','ProyectoResumenFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_ResumenEjecutivo, "Resumen") %>"></li>
                        <span>Resumen Ejecutivo</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_ResumenEjecutivo, "Resumen") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmProyectoResumen" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_anexos" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmAnexos','ProyectoAnexos.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_Anexos, "Anexos") %>"></li>
                        <span>Anexos</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_Anexos, "Anexos") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmAnexos" src="" marginwidth="0" marginheight="0" frameborder="0" scrolling="no"
                        width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_empresa" OnDemandMode="Once" runat="server" Visible="false"
                Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmProyectoEmpresa','ProyectoEmpresaFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_Empresa, "Empresa") %>"></li>
                        <span>Empresa</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_Empresa, "Empresa") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmProyectoEmpresa" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_seguimiento" OnDemandMode="Once" runat="server" Visible="false"
                Width="100%" Height="800px">
                <HeaderTemplate>
                    <div class="tab_header">
                        <li class="<%= setTab(10, "Seguimiento") %>" onclick="CargarPestana('frmContrato','/Fonade/interventoria/SeguimientoPPtalInterTabs.aspx')">
                        </li>
                        <span>Seguimiento Presupuestal</span>
                        <%--<img alt="img" src="<%= setTab(10, "Seguimiento") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmSeguimiento" src="/Fonade/interventoria/SeguimientoPPtalInterTabs.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="no" width="100%"
                        height="800px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_contrato" OnDemandMode="Once" runat="server" HeaderText=""
                Visible="false" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmContrato','/Fonade/interventoria/InterContratoFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_ContratoInter, "Seguimiento") %>"></li>
                        <span>Contrato</span>
                        <%--<img alt="img" src="<%= setTab(Datos.Constantes.CONST_ContratoInter, "Contrato") %>"></img></li>--%>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmContrato" src="" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="550px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>
</asp:Content>
