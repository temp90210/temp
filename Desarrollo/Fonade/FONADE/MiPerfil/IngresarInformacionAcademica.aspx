<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IngresarInformacionAcademica.aspx.cs" Inherits="Fonade.FONADE.MiPerfil.IngresarInformacionAcademica" MasterPageFile="~/Emergente.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="BodyContent"  runat="server" ContentPlaceHolderID="bodyContentPlace">

    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"> </asp:ToolkitScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" Visible="true" Width="100%" UpdateMode="Conditional">
        <ContentTemplate>
        
            <table width="580px" border="0" style="height: 51px">
              <tr>
                <td class="style47"><h1><asp:Label runat="server" ID="lbl_Titulo" style="font-weight: 700"></asp:Label></h1></td>
                <td align="right">
                    <asp:Label ID="l_fechaActual" runat="server" style="font-weight: 700"></asp:Label>
                </td>
                <td class="style28"></td>
              </tr>
            </table>
            <table border="0" style="width: 580px">
              <tr>
                <td class="style28"></td>
                <td class="style29" valign="baseline">Ciudad:</td>
                <td class="style30" valign="baseline">
                    <asp:DropDownList ID="ddl_ciudad" runat="server" Height="20px" Width="98%" 
                        AutoPostBack="True" 
                        onselectedindexchanged="ddl_ciudad_SelectedIndexChanged">
                    </asp:DropDownList>
                  </td>
                <td class="style31"></td>
              </tr>
              <tr>
                <td class="style28"></td>
                <td class="style29" valign="baseline">Nivel Educativo:</td>
                <td class="style30" valign="baseline">
                    <asp:DropDownList ID="ddl_nivelEdu" runat="server" Height="20px" Width="98%" 
                        AutoPostBack="True" onselectedindexchanged="ddl_nivelEdu_SelectedIndexChanged">
                    </asp:DropDownList>
                  </td>
                <td class="style31"></td>
              </tr>
              <tr>
                <td class="style28"></td>
                <td class="style29" valign="baseline">Institución:</td>
                <td class="style30" valign="baseline">
                    <asp:DropDownList ID="ddl_institucion" runat="server" Height="20px" 
                        Width="98%" AutoPostBack="True" 
                        onselectedindexchanged="ddl_institucion_SelectedIndexChanged">
                    </asp:DropDownList>
                  </td>
                <td class="style31"></td>
              </tr>
              <tr >
                <td class="style28"></td>
                <td class="style29" valign="baseline">Palabra clave (Programa Académico):</td>
                <td class="style30" valign="baseline">
                    <asp:TextBox ID="tx_palabraClave" runat="server" Width="150px" 
                        AutoPostBack="True"></asp:TextBox>&nbsp;&nbsp;
                    <asp:ImageButton ID="btn_buscar" runat="server" Height="23px" 
                        ImageAlign="AbsBottom" ImageUrl="~/Images/buscarrr.png" 
                        onclick="btn_buscar_Click" Width="23px" />
                  </td>
                <td class="style31"></td>
              </tr>
              <tr>
                <td colspan="3" align="center">
                    </td>
                <td></td>
              </tr>
              <tr>
                <td class="style37"></td>
                <td colspan="2" style="background-color:#00468F" class="style38">&nbsp;&nbsp;&nbsp;&nbsp; <strong>&nbsp;</strong><span 
                        class="style39"><strong>Programas Académicos</strong></span></td>
                <td class="style38"></td>
                  <tr>
                      <td class="style40">
                          </td>
                      <td colspan="2" class="style41">
                          <asp:Panel ID="Panel1" runat="server" Height="116px" ScrollBars="Auto" 
                              Width="99%" BorderColor="#BFBFBF" BorderStyle="Ridge">
                              <asp:RadioButtonList ID="rbl_programasAcad" runat="server" Height="16px" 
                                  Width="100%">
                              </asp:RadioButtonList>
                           <a id="linkFInal" runat=server> </a> 
                          </asp:Panel>
                      </td>
                      <td class="style41">
                          </td>
                  </tr>
              </tr>
              <tr>
                <td class="style28"></td>
                <td colspan="2">
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/add.png" 
                        onclick="ImageButton1_Click" />
                    <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">Crear Programa Académico</asp:LinkButton>   
                  </td>

                  </td>
                <td class="style31"></td>
              </tr>
            </table>
<%-- ------------------------------------------------------------------------------------------------- --%>
            <asp:Panel ID="PanelEstado" runat="server" Visible="true" Height="65px">
                <table border="0" style="width: 580px">
                    <tr>
                    <td class="style43"></td>
                    <td class="style49" valign="baseline">Estado:</td>
                    <td class="style45" valign="baseline">
                          <asp:DropDownList ID="ddl_estadoEstudio" runat="server" Height="20px" 
                              Width="150px" AutoPostBack="True" 
                              onselectedindexchanged="ddl_estadoEstudio_SelectedIndexChanged">
                              <asp:ListItem Value="1">Finalizado</asp:ListItem>
                              <asp:ListItem Value="0">Cursando Actualmente</asp:ListItem>
                          </asp:DropDownList>
                    </td>
                    <td class="style46"></td>
                    </tr>
                    <tr>
                    <td class="style43">&nbsp;</td>
                    <td class="style49" valign="baseline">Fecha de Inicio:</td>
                    <td class="style45" valign="baseline">
                         <asp:TextBox ID="tx_frechaInicio" runat="server" BackColor="White" 
                              Enabled="False" Text=""></asp:TextBox>
                          &nbsp;
                          <asp:Image ID="btnDate1" runat="server" AlternateText="cal1" 
                              ImageAlign="AbsBottom" ImageUrl="~/Images/calendar.png" Height="21px" 
                              Width="20px" />
                          <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd/MM/yyyy" 
                              PopupButtonID="btnDate1" TargetControlID="tx_frechaInicio" />
                    </td>
                    <td class="style46">&nbsp;</td>
                    </tr>
                </table>
            </asp:Panel>
<%-- ------------------------------------------------------------------------------------------------- --%>
            <asp:Panel ID="PanelFinalizado1" runat="server" Visible="false" Height="65px">
                <table border="0" style="width: 580px">
                    <tr>
                    <td class="style43"></td>
                    <td class="style44" valign="baseline">Fecha de finalización de Materias:</td>
                    <td class="style45" valign="baseline">
                        <asp:TextBox ID="txt_finmaterias" runat="server" BackColor="White" 
                            Enabled="False" Text=""></asp:TextBox>
                        &nbsp;
                        <asp:Image ID="btnfinmaterias" runat="server" AlternateText="cal1" 
                            ImageAlign="AbsBottom" ImageUrl="~/Images/calendar.png" Height="21px" 
                            Width="20px" />
                        <asp:CalendarExtender ID="Calendariofinmatearias" runat="server" Format="dd/MM/yyyy" 
                            PopupButtonID="btnfinmaterias" TargetControlID="txt_finmaterias" />
                    </td>
                    <td class="style46"></td>
                    </tr>
                    <tr>
                    <td class="style43">&nbsp;</td>
                    <td class="style44" valign="baseline">Fecha de Graduación:</td>
                    <td class="style45" valign="baseline">
                        <asp:TextBox ID="txt_graduacion" runat="server" BackColor="White" 
                            Enabled="False" Text=""></asp:TextBox>
                        &nbsp;
                        <asp:Image ID="btngraduacion" runat="server" AlternateText="cal1" 
                            ImageAlign="AbsBottom" ImageUrl="~/Images/calendar.png" Height="21px" 
                            Width="20px" />
                        <asp:CalendarExtender ID="Calendariograduacion" runat="server" Format="dd/MM/yyyy" 
                            PopupButtonID="btngraduacion" TargetControlID="txt_graduacion" />
                    </td>
                    <td class="style46">&nbsp;</td>
                    </tr>
                </table>
            </asp:Panel>
 <%-- ------------------------------------------------------------------------------------------------- --%>
            <asp:Panel ID="PanelFinalizado2" runat="server" Visible="false" Height="65px">
                <table border="0" style="width: 580px">
                    <tr>
                        <td class="style43">&nbsp;</td>
                        <td class="style44" valign="baseline">Fecha de Graduación:</td>
                        <td class="style45" valign="baseline">
                            <asp:TextBox ID="txt_fechafin2" runat="server" BackColor="White" 
                                Enabled="False" Text=""></asp:TextBox>
                            &nbsp;
                            <asp:Image ID="imfinalizado2" runat="server" AlternateText="" 
                                ImageAlign="AbsBottom" ImageUrl="~/Images/calendar.png" Height="21px" 
                                Width="20px" />
                            <asp:CalendarExtender ID="Calendariofin2" runat="server" Format="dd/MM/yyyy" 
                                PopupButtonID="imfinalizado2" TargetControlID="txt_fechafin2" />
                        </td>
                        <td class="style46">&nbsp;</td>
                    </tr>

                </table>

            </asp:Panel>
<%-- ------------------------------------------------------------------------------------------------- --%>
            
            <asp:Panel ID="PanelCursando1" runat="server" Visible="false" Height="65px">
                <table border="0" style="width: 580px">
                    <tr>
                    <td class="style43"></td>
                    <td class="style44" valign="baseline">Semestre actual u horas:</td>
                    <td class="style45" valign="baseline">
                        <asp:TextBox ID="txt_semestreactual1" runat="server" BackColor="White" 
                            Enabled="true" Text="" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3568" ValidationExpression="^[0-9]*" 
                                ControlToValidate="txt_semestreactual1" runat="server"  ForeColor="Red"  Display="Dynamic" 
                                ErrorMessage="* Este campo es numérico.">
                            </asp:RegularExpressionValidator>
                    </td>
                    <td class="style46"></td>
                    </tr>

                </table>

            </asp:Panel>



<%-- ------------------------------------------------------------------------------------------------- --%>
            <asp:Panel ID="PanelCursando2" runat="server" Visible="false" Height="65px">
                <table border="0" style="width: 580px">
                    <tr>
                    <td class="style43"></td>
                    <td class="style44" valign="baseline">Fecha de finalización de Etapa Productiva:</td>
                    <td class="style45" valign="baseline">
                        <asp:TextBox ID="txt_fechaCursando2" runat="server" BackColor="White" 
                            Enabled="False" Text=""></asp:TextBox>
                        &nbsp;
                        <asp:Image ID="imCursando2" runat="server" AlternateText="" 
                            ImageAlign="AbsBottom" ImageUrl="~/Images/calendar.png" Height="21px" 
                            Width="20px" />
                        <asp:CalendarExtender ID="CalendarExtender3" runat="server" Format="dd/MM/yyyy" 
                            PopupButtonID="imCursando2" TargetControlID="txt_fechaCursando2" />
                    </td>
                    <td class="style46"></td>
                    </tr>
                    <tr>
                    <td class="style43"></td>
                    <td class="style44" valign="baseline">Semestre actual u horas:</td>
                    <td class="style45" valign="baseline">
                        <asp:TextBox ID="txt_semestreactual2" runat="server" BackColor="White" 
                            Enabled="true" Text="" MaxLength="4"></asp:TextBox>
                    </td>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator154" ValidationExpression="^[0-9]*" 
                                ControlToValidate="txt_semestreactual2" runat="server"  ForeColor="Red"  Display="Dynamic" 
                                ErrorMessage="* Este campo es numérico.">
                            </asp:RegularExpressionValidator>
                    <td class="style46"></td>
                    </tr>
                </table>
            </asp:Panel>

<%-- ------------------------------------------------------------------------------------------------- --%>
            <asp:Panel ID="PanelBotones" runat="server" Visible="true" Height="66px">
                <table border="0" style="width: 580px; height: 56px;">
                    <tr>
                        <td class="style43"></td>
                        <td colspan="2" class="style53" align="center">
                            
                            <asp:Button ID="btn_CrearEstudio" runat="server" Enabled="False" Text="Crear" 
                                Visible="False" onclick="btn_CrearEstudio_Click" />
                            &nbsp;
                            <asp:Button ID="btn_modificarEstudio" runat="server" Enabled="False" 
                                Text="Modificar" Visible="False" onclick="btn_modificarEstudio_Click" />
                            &nbsp;&nbsp;&nbsp;
                            
                        </td>

                        <td class="style46"></td>
                    </tr>

                </table>
            </asp:Panel>

<%-- ------------------------------------------------------------------------------------------------- --%>
<%-- ------------------------------------------------------------------------------------------------- --%>
<%-- ------------------------------------------------------------------------------------------------- --%>

            <asp:Panel ID="pnlOcultar" runat="server" Visible="False" CssClass="pnlOcultar">
            </asp:Panel>
            <asp:Panel ID="pnlAlerta" runat="server" Visible="false" CssClass="pnlConfirmacion" BackColor="White">
                <table border="0" width="500px" class="fondoBlanco tablaEmergente" style="text-align: center" cellpadding="0" cellspacing="0">
                  <tr>
                    <td class="style59">&nbsp;</td>
                    <td class="style64">&nbsp;</td>
                    <td class="style65">&nbsp;</td>
                    <td>&nbsp;</td>
                  </tr>
                  <tr>
                    <td class="style73"></td>
                    <td class="style74">Ciudad:</td>
                    <td class="style75">
                        <asp:Label ID="l_ciudadP" runat="server"></asp:Label>
                      </td>
                    <td class="style76"></td>
                  </tr>
                  <tr>
                    <td class="style73"></td>
                    <td class="style74">Nivel Educativo:</td>
                    <td class="style75">
                        <asp:Label ID="l_NivelP" runat="server"></asp:Label>
                      </td>
                    <td class="style76"></td>
                  </tr>
                  <tr>
                    <td class="style73"></td>
                    <td class="style74">Institución:</td>
                    <td class="style75">
                        <asp:Label ID="l_institucionP" runat="server"></asp:Label>
                      </td>
                    <td class="style76"></td>
                  </tr>
                  <tr>
                    <td class="style69"></td>
                    <td class="style70">Programa Académico:</td>
                    <td class="style71">
                        <asp:TextBox ID="txt_ProgramaP" runat="server" Width="313px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                            BackColor="White" ControlToValidate="txt_ProgramaP" 
                            ErrorMessage="* Este campo está vacío" Display="Dynamic"
                            style="font-size: small; color: #FF0000;"></asp:RequiredFieldValidator>
                      </td>
                    <td class="style72"></td>
                  </tr>
                  <tr>
                    <td class="style67"></td>
                    <td colspan="2" class="style68">
                        <asp:Button ID="btn_crearPrograma" runat="server" Text="Crear" 
                            onclick="btn_crearPrograma_Click" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btn_cancelarPrograma" runat="server" Text="Cancelar" 
                            onclick="btn_cancelarPrograma_Click" CausesValidation="False" />
                      </td>
                    <td class="style68"></td>
                  </tr>

                </table>
            </asp:Panel>

<%-- ------------------------------------------------------------------------------------------------- --%>
<%-- ------------------------------------------------------------------------------------------------- --%>
<%-- ------------------------------------------------------------------------------------------------- --%>

        </ContentTemplate>

    </asp:UpdatePanel>



 </asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .style28
        {
            width: 17px;
            height: 26px;
        }
        .style29
        {
            width: 215px;
            font-weight: bold;
            height: 26px;
        }
        .style30
        {
            width: 314px;
            height: 26px;
        }
        .style31
        {
            height: 26px;
        }
        .style37
        {
            width: 17px;
            height: 32px;
        }
        .style38
        {
            height: 32px;
        }
        .style39
        {
            color: #FFFFFF;
        }
        .style40
        {
            width: 17px;
            height: 100px;
        }
        .style41
        {
            height: 100px;
        }
        .style43
        {
            width: 17px;
            height: 22px;
        }
        .style44
        {
            width: 274px;
            font-weight: bold;
            height: 22px;
        }
        .style45
        {
            width: 314px;
            height: 22px;
        }
        .style46
        {
            height: 22px;
        }
        .style47
        {
            width: 408px;
        }
        .style49
        {
            width: 279px;
            font-weight: bold;
            height: 22px;
        }
        .pnlConfirmacion
        {
          display:block !important;
          position:fixed !important;
          padding: 2px 3px !important;
          z-index:6 !important;
          left:40px!important;
          width:500px!important;
          height:187px!important;
          top:35%!important;
        }
 
        .tablaEmergente
        {
                        
                        width:500px !important;
                        -moz-border-radius: 15px !important;
        }
        .tablaEmergente1
        {
                        
                        width:500px !important;
                        -moz-border-radius: 15px !important;
        }
 
 
        .headConfirm
        {
                        background:#DF090C !important;
        }
        .textoConfirm
        {
                        font-family:Arial !important;
                        font-size:10px !important;
            padding:5px 5px 5px 5px !important;
        }
 
        .pnlOcultar{
                        position:absolute !important;
            top:0px !important;
            left:0px !important;
            width:100% !important;
            height:100% !important;
            background:silver !important;
            opacity:0.5 !important;
            -moz-opacity:0.5 !important;
            filter:Alpha(Opacity=50) !important;
        }
 
        .style53
        {
            width: 585px;
        }
 
        .style59
        {
            width: 28px;
        }
         
        .style64
        {
            width: 130px;
            text-align: left;
        }
        .style65
        {
            width: 322px;
            text-align: left;
        }
         
        .style67
        {
            width: 28px;
            height: 59px;
        }
        .style68
        {
            height: 59px;
        }
        .style69
        {
            width: 28px;
            height: 30px;
        }
        .style70
        {
            width: 130px;
            text-align: left;
            height: 30px;
        }
        .style71
        {
            width: 322px;
            text-align: left;
            height: 30px;
        }
        .style72
        {
            height: 30px;
        }
 
        .style73
        {
            width: 28px;
            height: 25px;
        }
        .style74
        {
            width: 130px;
            text-align: left;
            height: 25px;
        }
        .style75
        {
            height: 25px;
            width: 322px;
            text-align: left;
        }
        .style76
        {
            height: 25px;
        }
 
        </style>
</asp:Content>
