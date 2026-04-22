<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitudesEspeciales.aspx.cs" Inherits="DesarrollosQAS.SolicitudesEspeciales" MasterPageFile="~/Root.master" %>

<%@ Register Src="~/UserControls/PopupMessages.ascx" TagPrefix="uc" TagName="PopupMessages" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <style type="text/css">
        tr > .dxflCaptionCell_Office365 {
            padding-bottom: 15px !important;
        }

        .dxflGroupBox_Office365 {
            margin-bottom: 0px;
            padding: 0 0 12px;
            text-align: center;
        }

        .dxpLite_Office365 .dxp-button:not(.dxp-disabledButton):hover {
            background-color: teal;
        }
    </style>

    <script type="text/javascript">
        // Manejar mensajes después de operaciones del grid
        function OnGridSolicitudesEndCallback(s, e) {
            if (s.cpMessageType && s.cpMessage) {
                if (s.cpMessageType === "success") {
                    lblMensajeExito.SetText(s.cpMessage);
                    pcMensajeExito.Show();
                } else if (s.cpMessageType === "error") {
                    lblMensajeError.SetText(s.cpMessage);
                    pcMensajeError.Show();
                }

                delete s.cpMessageType;
                delete s.cpMessage;
            }
        }



        // Redirigir a nueva solicitud
        function NuevaSolicitud() {
            window.location.href = 'SolicitudEspecial.aspx';
        }

        // Manejar botones personalizados (Editar y Eliminar)
        function OnCustomButtonClickSolicitud(s, e) {
            // Botón Editar
            if (e.buttonID === 'btnEditSolicitud') {
                e.processOnServer = false;
                var idSolicitud = s.GetRowKey(e.visibleIndex);

                if (idSolicitud) {
                    window.location.href = 'SolicitudEspecial.aspx?id=' + idSolicitud;
                } else {
                    alert('No se pudo obtener el ID de la solicitud');
                }
                return;
            }

            // Botón Eliminar
            if (e.buttonID === 'btnDeleteSolicitud') {
                e.processOnServer = false;
                currentDeleteIndex = e.visibleIndex;
                pcConfirmarEliminacion.Show();
            }
        }

        // Permitir edición con doble clic
        function OnRowDblClickSolicitud(s, e) {
            if (e.visibleIndex >= 0) {
                var idSolicitud = s.GetRowKey(e.visibleIndex);
                if (idSolicitud) {
                    window.location.href = 'SolicitudEspecial.aspx?id=' + idSolicitud;
                }
            }
        }

        // Variable global para guardar el índice de la fila a eliminar
        var currentDeleteIndex = -1;

        // Confirmar eliminación desde el popup
        function ConfirmarEliminacion() {
            if (currentDeleteIndex >= 0) {
                gridSolicitudesEspeciales.PerformCallback('DELETE|' + currentDeleteIndex);
                pcConfirmarEliminacion.Hide();
            }
        }

        // Cancelar eliminación
        function CancelarEliminacion() {
            currentDeleteIndex = -1;
            pcConfirmarEliminacion.Hide();
        }
    </script>

    <!-- Incluir el User Control de Popups -->
    <uc:PopupMessages ID="popupMessages" runat="server" />
    <!-- Popup de Confirmación de Eliminación (específico de esta página) -->
    <dx:ASPxPopupControl ID="pcConfirmarEliminacion" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarEliminacion"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
        <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <div style="padding: 30px; text-align: center;">
                    <dx:ASPxLabel runat="server" Text="¿Está seguro que desea eliminar esta solicitud?" Font-Size="16px" Font-Bold="true" />
                    <br />
                    <br />
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterContentTemplate>
            <div style="text-align: center; padding: 10px;">
                <dx:ASPxButton ID="btnConfirmarEliminar" runat="server" Text="Sí" Width="120px" AutoPostBack="False"
                    BackColor="Teal" ForeColor="White" Font-Bold="true" Style="margin-left: 10px;">
                    <ClientSideEvents Click="ConfirmarEliminacion" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnCancelarEliminar" runat="server" Text="No" Width="120px" AutoPostBack="False"
                    BackColor="DarkRed" ForeColor="White" Font-Bold="true" Style="margin-left: 90px;">
                    <ClientSideEvents Click="CancelarEliminacion" />
                </dx:ASPxButton>
            </div>
        </FooterContentTemplate>
    </dx:ASPxPopupControl>

    <div style="padding-top: 8px">
        <dx:ASPxLabel runat="server" ID="ASPxLabel7" Text="Solicitudes" Font-Bold="true" Font-Size="X-Large"></dx:ASPxLabel>
    </div>
    <br />

    <!-- Grid de Solicitudes RH -->
    <dx:ASPxGridView ID="gridSolicitudesEspeciales" runat="server"
        KeyFieldName="ID_Solicitud"
        ForeColor="Black"
        ClientInstanceName="gridSolicitudesEspeciales"
        OnDataBinding="gridSolicitudesEspeciales_DataBinding"
        OnCustomButtonCallback="gridSolicitudesEspeciales_CustomButtonCallback"
        OnCustomCallback="gridSolicitudesEspeciales_CustomCallback"
        EnableRowsCache="false"
        OnDataBound="gridSolicitudesEspeciales_DataBound">
        <ClientSideEvents
            EndCallback="OnGridSolicitudesEndCallback"
            CustomButtonClick="function(s, e) { OnCustomButtonClickSolicitud(s, e); }" />
        <Styles>
            <Header BackColor="#353943" ForeColor="White" Font-Bold="true"></Header>
        </Styles>
        <Settings ShowColumnHeaders="true"/>
        <Columns>
            <dx:GridViewCommandColumn Caption="Acciones" Width="80" ButtonRenderMode="Image">
                <HeaderTemplate>
                    <div style="text-align: center;">
                        <dx:ASPxButton runat="server" ID="btnNuevoHeader" Text="" AutoPostBack="false"
                            ToolTip="Nueva Solicitud" RenderMode="Link">
                            <Image Url="~/Images/add.png" Width="30px" Height="30px" />
                            <ClientSideEvents Click="NuevaSolicitud" />
                        </dx:ASPxButton>
                    </div>
                </HeaderTemplate>
                <CustomButtons>
                    <dx:GridViewCommandColumnCustomButton ID="btnEditSolicitud" Text="Editar">
                        <Image Url="~/Images/edits.png" Width="18px" Height="18px" ToolTip="Editar" />
                    </dx:GridViewCommandColumnCustomButton>
                    <dx:GridViewCommandColumnCustomButton ID="btnDeleteSolicitud" Text="Eliminar">
                        <Image Url="~/Images/delete.png" Width="18px" Height="18px" ToolTip="Eliminar" />
                    </dx:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dx:GridViewCommandColumn>

            <dx:GridViewDataTextColumn FieldName="ID_Solicitud" Caption="Folio" Visible="true" ReadOnly="true" Width="80" CellStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />

            <dx:GridViewDataComboBoxColumn FieldName="id_TipoSolicitud" Caption="Tipo <br/> Solicitud" Width="105" HeaderStyle-HorizontalAlign="Center">
                <PropertiesComboBox TextField="Visita" ValueField="ID_TipoVisita" ValueType="System.Int32"></PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>

            <dx:GridViewDataTextColumn FieldName="Visitante" Caption="Visitante" HeaderStyle-HorizontalAlign="Center" Width="150">
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataComboBoxColumn FieldName="id_Proyecto" Caption="Proyecto" Width="100" HeaderStyle-HorizontalAlign="Center">
                <PropertiesComboBox TextField="NombreProyecto" ValueField="ID_Proyecto" ValueType="System.Int32" />
            </dx:GridViewDataComboBoxColumn>

            <dx:GridViewDataComboBoxColumn FieldName="id_Planta" Caption="Planta" Width="120" HeaderStyle-HorizontalAlign="Center">
                <PropertiesComboBox TextField="NombrePlanta" ValueField="ID_Planta" ValueType="System.Int32" />
            </dx:GridViewDataComboBoxColumn>

            <dx:GridViewDataComboBoxColumn FieldName="id_Contratista" Caption="Contratista" Width="100" HeaderStyle-HorizontalAlign="Center">
                <PropertiesComboBox TextField="Responsable" ValueField="id_contratista" ValueType="System.Int32" />
            </dx:GridViewDataComboBoxColumn>

            <dx:GridViewDataTextColumn FieldName="AreaTrabajo" Caption="Área<br/>de Trabajo" Width="100" HeaderStyle-HorizontalAlign="Center" />

            <dx:GridViewDataTextColumn FieldName="Actividad" Caption="Actividad" Width="300" HeaderStyle-HorizontalAlign="Center" />

            <dx:GridViewDataTextColumn FieldName="Estancia" Caption="Estancia" Width="80" />

            <dx:GridViewDataDateColumn FieldName="FechaInicio" Caption="Fecha Inicio" Width="110" HeaderStyle-HorizontalAlign="Center">
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
            </dx:GridViewDataDateColumn>

            <dx:GridViewDataDateColumn FieldName="FechaFin" Caption="Fecha Fin" Width="110" HeaderStyle-HorizontalAlign="Center">
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
            </dx:GridViewDataDateColumn>

            <dx:GridViewDataDateColumn FieldName="FechaSolicitud" Caption="Fecha<br/>Solicitud" Width="100" ReadOnly="true" HeaderStyle-HorizontalAlign="Center">
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
            </dx:GridViewDataDateColumn>

            <dx:GridViewDataTextColumn FieldName="RFC" Caption="RFC" Width="120" HeaderStyle-HorizontalAlign="Center" />

            <dx:GridViewDataTextColumn FieldName="Responsable" Caption="Responsable" Width="180" HeaderStyle-HorizontalAlign="Center" />

            <dx:GridViewDataCheckColumn FieldName="aprobado" Caption="Aprobado" Width="10" CellStyle-HorizontalAlign="Center">
                <HeaderStyle Wrap="False" />
            </dx:GridViewDataCheckColumn>

        </Columns>
        <SettingsText SearchPanelEditorNullText="Ingrese el valor a buscar ..." />
        <SettingsSearchPanel Visible="true" />
        <SettingsPager PageSize="25">
        </SettingsPager>
        
        <StylesPager CurrentPageNumber-BackColor="#353943" PageSizeItem-HoverStyle-BackColor="Teal"></StylesPager>
    </dx:ASPxGridView>

</asp:Content>
