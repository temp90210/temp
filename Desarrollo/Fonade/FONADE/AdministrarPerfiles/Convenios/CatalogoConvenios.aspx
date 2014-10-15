<%@ Page Language="C#"  MasterPageFile="~/Master.master"  Title="Fonade-Administrar Convenios" AutoEventWireup="true" CodeBehind="CatalogoConvenios.aspx.cs" Inherits="Fonade.FONADE.AdministrarPerfiles.Convenios.CatalogoConvenios" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:LinqDataSource ID="lds_Convenios" runat="server" 
        ContextTypeName="Datos.FonadeDBDataContext" AutoPage="false" 
        onselecting="lds_Convenios_Selecting"  >        
 </asp:LinqDataSource>

 <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager> 
       
 <h1><asp:Label runat="server" ID="lbl_Titulo"></asp:Label></h1>
   <asp:Panel  ID="pnl_Convenios" runat="server">
   <asp:HyperLink ID="AgregarConvenio"    NavigateUrl="~/FONADE/AdministrarPerfiles/Convenios/CatalogoConvenios.aspx?Accion=Crear" runat="server">
 <img alt="" src="../../../Images/icoAdicionarUsuario.gif" />
 Agregar Convenios</asp:HyperLink>
     <asp:GridView ID="gv_Convenios" runat="server" Width="100%" AutoGenerateColumns="False"
        DataKeyNames="" CssClass="Grilla" AllowPaging="false" 
        DataSourceID="lds_Convenios" 
        AllowSorting="True" onrowdatabound="gv_Convenios_RowDataBound" 
        OnDataBound="gv_Convenios_DataBound" 
        OnRowCreated ="gv_Convenios_RowCreated"       
        OnPageIndexChanging="gv_Convenios_PageIndexChanged">
          
        <Columns>   
        <asp:TemplateField>
                <ItemTemplate> 
                             
                    <asp:ImageButton ID="btn_Inactivar" CommandArgument='<%# Bind("Id_convenio")%>'  OnCommand= "btn_Inactivar_click"  runat="server" ImageUrl="/Images/icoBorrar.gif" Visible="true"/>
                    <ajaxToolkit:ConfirmButtonExtender ID="cbe" runat="server" DisplayModalPopupID="mpe" TargetControlID="btn_Inactivar">
                    </ajaxToolkit:ConfirmButtonExtender>
                    <ajaxToolkit:ModalPopupExtender ID="mpe" runat="server" PopupControlID="pnlPopup"  TargetControlID="btn_Inactivar" OkControlID = "btnYes"
                    CancelControlID="btnNo" BackgroundCssClass="modalBackground"></ajaxToolkit:ModalPopupExtender>
                     <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">
                    <div class="header">
                        Confirmación
                    </div>
                    <div class="body">
                        Esta seguro de borrar este registro?
                    </div>
                    <div class="footer" align="right">
                        <asp:Button ID="btnYes" runat="server" Text="Sí" />
                        <asp:Button ID="btnNo" runat="server" Text="No" />
                    </div>
                </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField> 
                   
            <asp:TemplateField HeaderText="Convenio" SortExpression="Convenio">
                <ItemTemplate>
                    <asp:HyperLink ID="hl_Convenio" runat="server" NavigateUrl='<%# "CatalogoConvenios.aspx?Accion=Editar&CodCriterio="+ Eval("Id_convenio") %>' Text='<%# Eval("nomconvenio")%>'></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>  
            <asp:BoundField HeaderText="Fecha Inicio" DataField ="FechaInicio" SortExpression="FechaInicio" />
            <asp:BoundField HeaderText="Fecha Fin" DataField ="FechaFin" SortExpression="FechaFin" />
            <asp:BoundField HeaderText="Email Fiduciaria" DataField ="EmailFiduciaria" SortExpression="EmailFiduciaria" />
            
            
        </Columns>
    </asp:GridView>
    </asp:Panel>
    <asp:Panel  ID="pnl_crearEditar"  runat="server">
    <asp:Table ID="tbl_Convenio" runat="server">
      <asp:TableRow>
            <asp:TableCell>Convenio:</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="tb_Convenio" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator  CssClass="ErrorValidacion" ForeColor="Red" ID="RequiredFieldValidator1" runat="server" ControlToValidate="tb_Convenio" Display="Dynamic" ErrorMessage="* Este campo no puede estar en blanco"></asp:RequiredFieldValidator>
                </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>Descripción:</asp:TableCell>
            <asp:TableCell><asp:TextBox ID="tb_Descripcion" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator  CssClass="ErrorValidacion" ForeColor="Red" ID="RequiredFieldValidator2" runat="server" ControlToValidate="tb_Descripcion" Display="Dynamic" ErrorMessage="* Este campo no puede estar en blanco"></asp:RequiredFieldValidator>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>Fecha inicio:</asp:TableCell>
            
            <asp:TableCell>
            
            <asp:TextBox runat="server" ID="tb_fechaInicio"  Text="11/01/2006" />
                        <asp:Image runat="server" ID="img_dateInicio" AlternateText="cal2" ImageUrl="~/images/icomodificar.gif" />
                        <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" PopupButtonID="img_dateInicio"  CssClass="ajax__calendar" TargetControlID="tb_fechaInicio" Format="MMMM d, yy" />
            </asp:TableCell>
            
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell>Fecha fin:</asp:TableCell>            
            <asp:TableCell>
            <asp:TextBox runat="server" ID="tb_fechaFin"  Text="11/01/2006" />
                        <asp:Image runat="server" ID="img_dateFin" AlternateText="cal2" ImageUrl="~/images/icomodificar.gif" />
                        <ajaxtoolkit:calendarextender runat="server"   ID="calExtender2" PopupButtonID="img_dateFin"  CssClass="ajax__calendar" TargetControlID="tb_fechaFin" Format="MMMM d, yy" />
            </asp:TableCell>            
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>Fiduciaria:</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="ddl_fiduciaria" runat="server">
                </asp:DropDownList> 
                <asp:RequiredFieldValidator  CssClass="ErrorValidacion" ForeColor="Red" ID="RequiredFieldValidator6" runat="server" ControlToValidate="ddl_fiduciaria" Display="Dynamic" ErrorMessage="* Este campo no puede estar en blanco"></asp:RequiredFieldValidator>
                </asp:TableCell>
        </asp:TableRow>
        
    </asp:Table>
     <asp:Button ID="btn_crearActualizar"    OnClick="btn_crearActualizar_onclick"  runat="server" Text="Actualizar" />
     
                <ajaxToolkit:ConfirmButtonExtender Enabled=false ID="cbe1" runat="server" DisplayModalPopupID="mpe1" TargetControlID="btn_crearActualizar">
                    </ajaxToolkit:ConfirmButtonExtender>
                    <ajaxToolkit:ModalPopupExtender ID="mpe1" runat="server" PopupControlID="pnlPopup1"  TargetControlID="btn_crearActualizar" OkControlID = "btnYes"
                    BackgroundCssClass="modalBackground"></ajaxToolkit:ModalPopupExtender>
                     <asp:Panel ID="pnlPopup1" runat="server" CssClass="modalPopup" Style="display: none">
                    <div class="header">
                        Confirmación
                    </div>
                    <div class="body">
                        <asp:Label ID="lbl_popup" runat="server"></asp:Label>
                    </div>
                    <div class="footer" align="right">
                        <asp:Button ID="btnYes" runat="server" Text="Aceptar" />
                        
                    </div>
                </asp:Panel>
    </asp:Panel>
    <asp:Label ID="Lbl_Resultados" CssClass="Indicador" runat="server"></asp:Label>
</asp:Content>
