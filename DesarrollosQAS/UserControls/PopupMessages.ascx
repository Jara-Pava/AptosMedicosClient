
<%@  Control Language="C#" AutoEventWireup="true" CodeBehind="PopupMessages.ascx.cs" Inherits="DesarrollosQAS.UserControls.PopupMessages" %>

<!-- Popup de Confirmación -->
<dx:ASPxPopupControl ID="pcConfirmarAccion" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
    PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarAccion"
    HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
    <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <div style="padding: 30px; text-align: center;">
                <dx:ASPxLabel ID="lblMensajeConfirmacion" runat="server" ClientInstanceName="lblMensajeConfirmacion" Font-Size="16px" Font-Bold="true" />
                <br />
                <br />
            </div>
        </dx:PopupControlContentControl>
    </ContentCollection>
    <FooterContentTemplate>
        <div style="text-align: center; padding: 10px;">
            <dx:ASPxButton ID="btnConfirmar" runat="server" Text="Sí" Width="120px" AutoPostBack="False"
                BackColor="Teal" ForeColor="White" Font-Bold="true" Style="margin-left: 10px;"
                ClientInstanceName="btnConfirmar">
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnCancelar" runat="server" Text="No" Width="120px" AutoPostBack="False"
                BackColor="DarkRed" ForeColor="White" Font-Bold="true" Style="margin-left: 80px;">
                <ClientSideEvents Click="function(s, e) { pcConfirmarAccion.Hide(); }" />
            </dx:ASPxButton>
        </div>
    </FooterContentTemplate>
</dx:ASPxPopupControl>

<!-- Popup de Éxito -->
<dx:ASPxPopupControl ID="pcMensajeExito" runat="server" Width="400" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
    PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcMensajeExito"
    HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
    <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <div style="padding: 20px; text-align: center;">
                <dx:ASPxLabel ID="lblMensajeExito" runat="server" Font-Size="14px" ClientInstanceName="lblMensajeExito" />
            </div>
        </dx:PopupControlContentControl>
    </ContentCollection>
    <FooterContentTemplate>
        <div style="text-align: center; padding: 10px;">
            <dx:ASPxButton ID="btnCerrarExito" runat="server" Text="OK" Width="100px" AutoPostBack="False"
                BackColor="Teal" ForeColor="White" Font-Bold="true" ClientInstanceName="btnCerrarExito">
                <ClientSideEvents Click="function(s, e) { pcMensajeExito.Hide(); }" />
            </dx:ASPxButton>
        </div>
    </FooterContentTemplate>
</dx:ASPxPopupControl>

<!-- Popup de Error -->
<dx:ASPxPopupControl ID="pcMensajeError" runat="server" Width="400" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
    PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcMensajeError"
    HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
    <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <div style="padding: 20px; text-align: center;">
                <dx:ASPxLabel ID="lblMensajeError" runat="server" Font-Size="14px" ClientInstanceName="lblMensajeError" />
            </div>
        </dx:PopupControlContentControl>
    </ContentCollection>
    <FooterContentTemplate>
        <div style="text-align: center; padding: 10px;">
            <dx:ASPxButton ID="btnCerrarError" runat="server" Text="OK" Width="100px" AutoPostBack="False"
                BackColor="Teal" ForeColor="White" Font-Bold="true">
                <ClientSideEvents Click="function(s, e) { pcMensajeError.Hide(); }" />
            </dx:ASPxButton>
        </div>
    </FooterContentTemplate>
</dx:ASPxPopupControl>

