<%@ Page Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="EvaluadorHojaAvanceGerenteDetalle.aspx.cs" Inherits="Fonade.FONADE.evaluacion.EvaluadorHojaAvanceGerenteDetalle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="LB_Volver" runat="server" Text="Regresar a la Lista de Coordinadores" PostBackUrl="~/FONADE/evaluacion/EvaluadorHojaAvanceGerente.aspx"></asp:LinkButton>
    <br />
    <br />
    <div style="overflow: scroll; padding-left: 20px; padding-right: 20px; padding-bottom: 20px;
        padding-top: 20px; margin: 20px; width:650px;">
        <asp:DataList ID="DtSeguimiento" runat="server" onitemdatabound="DtSeguimientoItemDataBound" ShowFooter="False" Style="font-weight: 700" Width="88%">
            <HeaderTemplate>
                <table border="0" width="100%">
                    <tr align="center" bgcolor="#D1D8E2" class="Titulo">
                        <td>No. </td>
                        <td>Plan de Negocio </td>
                        <td>Evaluador </td>
                        <td>Lectura del Plan de negocio </td>
                        <td>Solicitud de información al emprendedor </td>
                        <td colspan="23">
                            <div align="center">
                                Tabla de evaluación</div>
                        </td>
                        <td>Modelo financiero </td>
                        <td>Indices de rentabilidad </td>
                        <td colspan="2">Concepto y recomendaciones </td>
                        <td>Plan operativo </td>
                        <td>Informe de evaluacion </td>
                    </tr>
                    <tr align="center" bgcolor="#EDEFF3" class="Titulo">
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td colspan="5">Generales </td>
                        <td colspan="5">Comerciales </td>
                        <td colspan="4">Técnico </td>
                        <td colspan="3">Organizacionales </td>
                        <td colspan="5">Financiero </td>
                        <td>Medio Ambiente </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>Viabilidad </td>
                        <td>Indicadores de gestión </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                    </tr>
                    <tr align="center" bgcolor="#EDEFF3" class="Titulo">
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>Antecedentes </td>
                        <td>Definición Objetivos </td>
                        <td>Equipo de Trabajo </td>
                        <td>Justificacion de proyecto </td>
                        <td>resumen ejecutivo </td>
                        <td>Caracterización del Producto </td>
                        <td>Estrategias y garantias de comercialización </td>
                        <td>Identificación Mercado Objetivo </td>
                        <td>Identificacion y evaluacion de canales </td>
                        <td>Proyeccion de ventas </td>
                        <td>Caracterizacion Técnica del Producto o Servicio </td>
                        <td>Definición del proceso de producción a implementar e indices técnicos </td>
                        <td>Identificación y valoración de los requerimientos en equipamiento y materiales y suministros </td>
                        <td>Programa de producción </td>
                        <td>Analisis en los tramites y requisitos legales para la puesta en marcha de la empresa </td>
                        <td>Compromisos institucionales privados o publicos </td>
                        <td>Organización empresarial propuesta </td>
                        <td>Cunatificacion de la inversion requerida </td>
                        <td>Perspectivas de rentabilidad </td>
                        <td>Estados financieros </td>
                        <td>Presupuestos de costos de produccion </td>
                        <td>Presupuesto de ingresos de operación </td>
                        <td>contempla o no el manejo ambiental </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                        <td colspan="2">&nbsp; </td>
                        <td>&nbsp; </td>
                        <td>&nbsp; </td>
                    </tr>
                </table>
            </HeaderTemplate>
            <ItemTemplate>
                <table border="1" cellpadding="0" cellspacing="0" class="TableEvaluador" style="width: 100%; text-align: center">
                    <%--<tr align="center">
                        <td style="width: 21px">&nbsp; </td>
                        <td style="width: 67px">&nbsp; </td>
                        <td style="width: 78px">&nbsp; </td>
                        <td style="width: 53px">&nbsp; </td>
                        <td style="width: 86px">&nbsp; </td>
                        <td style="width: 85px">&nbsp; </td>
                        <td style="width: 60px">&nbsp; </td>
                        <td style="width: 48px">&nbsp; </td>
                        <td style="width: 72px">&nbsp; </td>
                        <td style="width: 54px">&nbsp; </td>
                        <td style="width: 94px">&nbsp; </td>
                        <td style="width: 99px">&nbsp; </td>
                        <td style="width: 80px">&nbsp; </td>
                        <td style="width: 80px">&nbsp; </td>
                        <td style="width: 68px">&nbsp; </td>
                        <td style="width: 94px">&nbsp; </td>
                        <td style="width: 73px">&nbsp; </td>
                        <td style="width: 80px">&nbsp; </td>
                        <td style="width: 68px">&nbsp; </td>
                        <td style="width: 51px">&nbsp; </td>
                        <td style="width: 85px">&nbsp; </td>
                        <td style="width: 79px">&nbsp; </td>
                        <td style="width: 85px">&nbsp; </td>
                        <td style="width: 76px">&nbsp; </td>
                        <td style="width: 64px">&nbsp; </td>
                        <td style="width: 80px">&nbsp; </td>
                        <td style="width: 74px">&nbsp; </td>
                        <td style="width: 62px">&nbsp; </td>
                        <td style="width: 58px">&nbsp; </td>
                        <td style="width: 55px">&nbsp; </td>
                        <td style="width: 61px">&nbsp; </td>
                        <td style="width: 71px">&nbsp; </td>
                        <td style="width: 49px">&nbsp; </td>
                        <td style="width: 63px">&nbsp; </td>
                    </tr>--%>
                    <tr align="center" bgcolor="#EDEFF3" class="Antetitulo">
                        <td style="width: 21px">&nbsp; </td>
                        <td style="width: 67px">&nbsp; </td>
                        <td style="width: 78px">&nbsp; </td>
                        <td style="width: 53px" text-align: center">&nbsp; </td>
                        <td style="width: 86px" text-align: center">&nbsp; </td>
                        <td style="width: 85px" text-align: center">&nbsp; </td>
                        <td style="width: 60px" text-align: center">&nbsp; </td>
                        <td style="width: 48px" text-align: center">&nbsp; </td>
                        <td style="width: 72px" text-align: center">&nbsp; </td>
                        <td style="width: 54px" text-align: center">&nbsp; </td>
                        <td style="width: 94px" text-align: center">&nbsp; </td>
                        <td style="width: 99px" text-align: center">&nbsp; </td>
                        <td style="width: 80px" text-align: center">&nbsp; </td>
                        <td style="width: 80px" text-align: center">&nbsp; </td>
                        <td style="width: 68px" text-align: center">&nbsp; </td>
                        <td style="width: 94px" text-align: center">&nbsp; </td>
                        <td style="width: 73px" text-align: center">&nbsp; </td>
                        <td style="width: 80px" text-align: center">&nbsp; </td>
                        <td style="width: 68px" text-align: center">&nbsp; </td>
                        <td style="width: 51px" text-align: center">&nbsp; </td>
                        <td style="width: 85px" text-align: center">&nbsp; </td>
                        <td style="width: 79px" text-align: center">&nbsp; </td>
                        <td style="width: 85px" text-align: center">&nbsp; </td>
                        <td style="width: 76px" text-align: center">&nbsp; </td>
                        <td style="width: 64px" text-align: center">&nbsp; </td>
                        <td style="width: 80px" text-align: center">&nbsp; </td>
                        <td style="width: 74px" text-align: center">&nbsp; </td>
                        <td style="width: 62px" text-align: center">&nbsp; </td>
                        <td style="width: 58px" text-align: center">&nbsp; </td>
                        <td style="width: 55px" text-align: center">&nbsp; </td>
                        <td style="width: 61px" text-align: center">&nbsp; </td>
                        <td style="width: 71px" text-align: center">&nbsp; </td>
                        <td style="width: 49px" text-align: center">&nbsp; </td>
                        <td style="width: 63px" text-align: center">&nbsp; </td>
                    </tr>
                    <!--coso-->
                    <tr>
                        <td align="center">
                            <asp:Label ID="lproyecto" runat="server" Text='<%# Eval("CodProyecto") %>' />
                            <asp:Label ID="lcontacto" runat="server" Text='<%# Eval("CodContacto") %>' Visible="False" />
                            <td style="width: 21px">
                                <asp:Label ID="lplannegocio" runat="server" Text='<%# Eval("NomProyecto") %>' />
                            </td>
                            <td style="width: 67px">
                                <asp:Label ID="levaluador" runat="server" />
                            </td>
                            <td style="width: 78px">&nbsp; </td>
                            <td style="width: 53px">
                                <asp:CheckBox ID="cLecturaPlanNegocio" runat="server" Text='<%# Eval("LecturaPlanNegocio") %>' />
                            </td>
                            <td style="width: 86px">
                                <asp:CheckBox ID="cSolicitudInformacionEmprendedor" runat="server" Text='<%# Eval("SolicitudInformacionEmprendedor") %>' />
                            </td>
                            <td style="width: 85px">
                                <asp:CheckBox ID="cAntecedentes" runat="server" Text='<%# Eval("Antecedentes") %>' />
                            </td>
                            <td style="width: 60px">
                                <asp:CheckBox ID="cDefinicionObjetivos" runat="server" Text='<%# Eval("DefinicionObjetivos") %>' />
                            </td>
                            <td style="width: 48px">
                                <asp:CheckBox ID="cEquipoTrabajo" runat="server" Text='<%# Eval("EquipoTrabajo") %>' />
                            </td>
                            <td style="width: 72px">
                                <asp:CheckBox ID="cJustificacionProyecto" runat="server" Text='<%# Eval("JustificacionProyecto") %>' />
                            </td>
                            <td style="width: 54px">
                                <asp:CheckBox ID="cResumenEjecutivo" runat="server" Text='<%# Eval("ResumenEjecutivo") %>' />
                            </td>
                            <td style="width: 94px">
                                <asp:CheckBox ID="cCaracterizacionProducto" runat="server" Text='<%# Eval("CaracterizacionProducto") %>' />
                            </td>
                            <td style="width: 99px">
                                <asp:CheckBox ID="cEstrategiasGarantiasComercializacion" runat="server" Text='<%# Eval("EstrategiasGarantiasComercializacion") %>' />
                            </td>
                            <td style="width: 80px">
                                <asp:CheckBox ID="cIdentificacionMercadoObjetivo" runat="server" Text='<%# Eval("IdentificacionMercadoObjetivo") %>' />
                            </td>
                            <td style="width: 80px">
                                <asp:CheckBox ID="cIdentificacionEvaluacionCanales" runat="server" Text='<%# Eval("IdentificacionEvaluacionCanales") %>' />
                            </td>
                            <td style="width: 68px">
                                <asp:CheckBox ID="cProyeccionVentas" runat="server" Text='<%# Eval("ProyeccionVentas") %>' />
                            </td>
                            <td style="width: 94px">
                                <asp:CheckBox ID="cCaracterizacionTecnicaProductoServicio" runat="server" Text='<%# Eval("CaracterizacionTecnicaProductoServicio") %>' />
                            </td>
                            <td style="width: 73px">
                                <asp:CheckBox ID="cDefinicionProcesoProduccionImplementarIndicesTecnicos" runat="server" Text='<%# Eval("DefinicionProcesoProduccionImplementarIndicesTecnicos") %>' />
                            </td>
                            <td style="width: 80px">
                                <asp:CheckBox ID="cIdentificacionValoracionRequerimientosEquipamiento" runat="server" Text='<%# Eval("IdentificacionValoracionRequerimientosEquipamiento") %>' />
                            </td>
                            <td style="width: 68px">
                                <asp:CheckBox ID="cProgramaProduccion" runat="server" Text='<%# Eval("ProgramaProduccion") %>' />
                            </td>
                            <td style="width: 51px">
                                <asp:CheckBox ID="cAnalisisTramitesRequisitosLegales" runat="server" Text='<%# Eval("AnalisisTramitesRequisitosLegales") %>' />
                            </td>
                            <td style="width: 85px">
                                <asp:CheckBox ID="cCompromisosInstitucionalesPrivadosPublicos" runat="server" Text='<%# Eval("CompromisosInstitucionalesPrivadosPublicos") %>' />
                            </td>
                            <td style="width: 79px">
                                <asp:CheckBox ID="cOrganizacionEmpresarialPropuesta" runat="server" Text='<%# Eval("OrganizacionEmpresarialPropuesta") %>' />
                            </td>
                            <td style="width: 85px">
                                <asp:CheckBox ID="cCuantificacionInversionRequerida" runat="server" Text='<%# Eval("CuantificacionInversionRequerida") %>' />
                            </td>
                            <td style="width: 76px">
                                <asp:CheckBox ID="cPerspectivasRentabilidad" runat="server" Text='<%# Eval("PerspectivasRentabilidad") %>' />
                            </td>
                            <td style="width: 64px">
                                <asp:CheckBox ID="cEstadosFinancieros" runat="server" Text='<%# Eval("EstadosFinancieros") %>' />
                            </td>
                            <td style="width: 80px">
                                <asp:CheckBox ID="cPresupuestosCostosProduccion" runat="server" Text='<%# Eval("PresupuestosCostosProduccion") %>' />
                            </td>
                            <td style="width: 74px">
                                <asp:CheckBox ID="cPresupuestoIngresosOperacion" runat="server" Text='<%# Eval("PresupuestoIngresosOperacion") %>' />
                            </td>
                            <td style="width: 62px">
                                <asp:CheckBox ID="cContemplaManejoAmbiental" runat="server" Text='<%# Eval("ContemplaManejoAmbiental") %>' />
                            </td>
                            <td style="width: 58px">
                                <asp:CheckBox ID="cModeloFinanciera" runat="server" Text='<%# Eval("ModeloFinanciera") %>' />
                            </td>
                            <td style="width: 55px">
                                <asp:CheckBox ID="cIndicesRentabilidad" runat="server" Text='<%# Eval("IndicesRentabilidad") %>' />
                            </td>
                            <td style="width: 61px">
                                <asp:CheckBox ID="cViabilidad" runat="server" Text='<%# Eval("Viabilidad") %>' />
                            </td>
                            <td style="width: 71px">
                                <asp:CheckBox ID="cIndicadoresGestion" runat="server" Text='<%# Eval("IndicadoresGestion") %>' />
                            </td>
                            <td style="width: 49px">
                                <asp:CheckBox ID="cPlanOperativo" runat="server" Text='<%# Eval("PlanOperativo") %>' />
                            </td>
                            <td style="width: 63px">
                                <asp:CheckBox ID="cInformeEvaluacion" runat="server" Text='<%# Eval("InformeEvaluacion") %>' />
                            </td>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:DataList>
        <p align="center">
            <asp:Label ID="lblmensaje" runat="server" Font-Size="XX-Large" Text="No se encontraron  Datos" Visible="False" />
        </p>
    </div>
    
</asp:Content>