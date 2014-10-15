<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/FONADE/evaluacion/Evaluacion.Master" 
AutoEventWireup="true" CodeBehind="SeguimientoFrameset.aspx.cs" Inherits="Fonade.FONADE.interventoria.SeguimientoFrameset" %>
<asp:Content ID="Content1" ContentPlaceHolderID="bodyHolder" runat="server">
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <style type="text/css">
        iframe {
            min-height: 750px;
        }
        .ContentInfo {
            margin-top: 6px !important;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            //alert("How we doing?")
        });
    </script>
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager2" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <div>
        <ajaxToolkit:TabContainer ID="tc_proyectos" runat="server" ActiveTabIndex="0" Width="100%"
            Height="100%">
            <ajaxToolkit:TabPanel ID="tc_tabla" OnDemandMode="Once" runat="server" Width="100%" 
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmtabla','ProyectoOperativoInterFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_PlanOperativoInter) %>"></li>
                        <span>Plan Operativo</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmtabla" src="ProyectoOperativoInterFrame.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            
            <ajaxToolkit:TabPanel ID="tc_modelofinanciero" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmmodelo','InterIndicadoresFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_IndicadoresGestionInter2) %>"></li>
                        <span>Indicadores De Gestión</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmmodelo" src="InterIndicadoresFrame.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            
                    
            <ajaxToolkit:TabPanel ID="tc_concepto" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmconcepto','InterRiesgos.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_RiesgosInter) %>"></li>
                        <span>Riesgos</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmconcepto" src="InterRiesgos.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>

            <ajaxToolkit:TabPanel ID="tc_plan" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmPlanOperativo','InterConceptosFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_ConceptosInter) %>"></li>
                        <span>Concepto Final y Recomendaciones</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmPlanOperativo" src="InterConceptosFrame.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="auto" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_informe" runat="server" Width="100%" Height="100%" >
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frminforme','InterContratoFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.CONST_ContratoInter) %>"></li>
                        <span>Contrato</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frminforme" src="InterContratoFrame.aspx" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>

            <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" Width="100%" Height="100%" >
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmDesempeno','SeguimientoPPtalInterTabs.aspx')">
                        <li class="<%= setTab(6) %>"></li>
                        <span>Seguimiento Presupuestal</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmDesempeno" src="SeguimientoPPtalInterTabs.aspx" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
    </div>
    <script type="text/javascript">
        $(document).on("ready", cargarTab);

        function cargarTab() {

            $("#tc_tabla");
        }
    
          </script>
</asp:Content>

