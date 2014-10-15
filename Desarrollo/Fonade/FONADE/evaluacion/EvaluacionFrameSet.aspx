<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/FONADE/evaluacion/Evaluacion.Master" AutoEventWireup="true" 
CodeBehind="EvaluacionFrameSet.aspx.cs" Inherits="Fonade.FONADE.evaluacion.EvaluacionFrameSet" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyHolder" runat="server">
    <script src="../../Scripts/jquery-1.9.1.js" type="text/javascript"></script>
     <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //alert($("imframe"));
        });
    </script>
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager2" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <div>
        <ajaxToolkit:TabContainer ID="tc_proyectos" runat="server" ActiveTabIndex="0" Width="100%"
            Height="530px">
            <ajaxToolkit:TabPanel ID="tc_tabla" OnDemandMode="Once" runat="server" Width="100%" 
                Height="530px">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmtabla','EvaluacionTablaFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.ConstTablaEvaluacion) %>"></li>
                        <span>Tabla de Evaluación</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmtabla" src="EvaluacionTablaFrame.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="no" width="100%"
                        height="530px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            
            <ajaxToolkit:TabPanel ID="tc_modelofinanciero" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmmodelo','EvaluacionModeloFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.ConstModeloFinanciero) %>"></li>
                        <span>Evaluación Financiera</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmmodelo" src="EvaluacionModeloFrame.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="no" width="100%"
                        height="700px"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            
                    
            <ajaxToolkit:TabPanel ID="tc_concepto" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmconcepto','EvaluacionIndicadoresFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.ConstModeloFinanciero) %>"></li>
                        <span>Concepto Final y Recomendaciones</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmconcepto" src="EvaluacionIndicadoresFrame.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="no" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>

            <ajaxToolkit:TabPanel ID="tc_plan" OnDemandMode="Once" runat="server" Width="100%"
                Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmPlanOperativo','EvaluacionOperativoFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.ConstPlanOperativoEval) %>"></li>
                        <span>Plan Operativo</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmPlanOperativo" src="EvaluacionOperativoFrame.aspx"
                        marginwidth="0" marginheight="0" frameborder="0" scrolling="no" width="100%"
                        height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel ID="tc_informe" runat="server" Width="100%" Height="100%" >
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frminforme','EvaluacionReportes.aspx')">
                        <li class="<%= setTab(Datos.Constantes.ConstInformes) %>"></li>
                        <span>Informes</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frminforme" src="EvaluacionReportes.aspx" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>

            <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" Width="100%" Height="100%" >
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmDesempeno','EvaluacionDesempenoFrame.aspx')">
                        <li class="<%= setTab(Datos.Constantes.ConstDesempenoEvaluador) %>"></li>
                        <span>Desempeño Evaluador</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="Iframe1" src="EvaluacionDesempenoFrame.aspx" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>

            <ajaxToolkit:TabPanel ID="tc_hoja" runat="server" Width="100%" Height="100%">
                <HeaderTemplate>
                    <div class="tab_header" onclick="CargarPestana('frmhoja','EvaluacionHojaAvance.aspx')">
                        <li class="<%= setTab(Datos.Constantes.ConstHojaAvance) %>"></li>
                        <span>Hoja De Avance</span>
                    </div>
                </HeaderTemplate>
                <ContentTemplate>
                    <iframe id="frmhoja" src="EvaluacionHojaAvance.aspx" marginwidth="0" marginheight="0" frameborder="0"
                        scrolling="no" width="100%" height="100%"></iframe>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
           
        </ajaxToolkit:TabContainer>
    </div>
</asp:Content>
