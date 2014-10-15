<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Alert.ascx.cs" Inherits="Fonade.Controles.Alert" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager> 
    <asp:Button ID="btn_crearActualizar"      runat="server" Text="Actualizar" Visible="false" />
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