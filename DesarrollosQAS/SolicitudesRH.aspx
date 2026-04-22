<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitudesRH.aspx.cs" MasterPageFile="~/Root.master" Inherits="DesarrollosQAS.SolicitudesRH" %>

<%@ Register Src="~/UserControls/PopupMessages.ascx" TagPrefix="uc" TagName="PopupMessages" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <script type="text/javascript">

        // Manejar mensajes después de operaciones del grid
        function OnGridSolicitudesEndCallback(s, e) {
            console.log('EndCallback disparado'); // Para debug
            console.log('cpMessageType:', s.cpMessageType);
            console.log('cpMessage:', s.cpMessage);

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

        // Confirmar antes de eliminar - Mostrar popup personalizado
        function OnCustomButtonClickSolicitud(s, e) {
            if (e.buttonID === 'btnDeleteSolicitud') {
                e.processOnServer = false; // Cancelar el proceso del servidor
                currentDeleteIndex = e.visibleIndex; // Guardar el índice de la fila
                pcConfirmarEliminacion.Show(); // Mostrar popup de confirmación
            }
        }

        // Variable global para guardar el índice de la fila a eliminar
        var currentDeleteIndex = -1;

        // Confirmar eliminación desde el popup
        function ConfirmarEliminacion() {
            if (currentDeleteIndex >= 0) {
                gridSolicitudesRH.PerformCallback('DELETE|' + currentDeleteIndex);
                pcConfirmarEliminacion.Hide();
            }
        }

        // Cancelar eliminación
        function CancelarEliminacion() {
            currentDeleteIndex = -1;
            pcConfirmarEliminacion.Hide();
        }

        // Validar antes de guardar en el EditForm
        function ValidarYGuardarEditForm(s, e) {
            // Validar todos los controles del grupo "EditForm"
            if (!ASPxClientEdit.ValidateGroup('EditForm')) {
                e.processOnServer = false;
                return false;
            }

            // Permitir el postback
            e.processOnServer = true;
        }

        // Función para verificar si hay datos en el EditForm
        function EditFormTieneDatos() {
            try {
                // Verificar si los controles existen antes de intentar acceder a ellos
                var tieneDatos = false;

                if (typeof cboTipoSolicitudEdit !== 'undefined' && cboTipoSolicitudEdit.GetValue() != null) tieneDatos = true;
                if (typeof txtVisitanteEdit !== 'undefined' && txtVisitanteEdit.GetText().trim() != '') tieneDatos = true;
                if (typeof cboProyectoEdit !== 'undefined' && cboProyectoEdit.GetValue() != null) tieneDatos = true;
                if (typeof cboPlantaEdit !== 'undefined' && cboPlantaEdit.GetValue() != null) tieneDatos = true;
                if (typeof cboContratistaEdit !== 'undefined' && cboContratistaEdit.GetValue() != null) tieneDatos = true;
                if (typeof txtAreaTrabajoEdit !== 'undefined' && txtAreaTrabajoEdit.GetText().trim() != '') tieneDatos = true;
                if (typeof txtActividadEdit !== 'undefined' && txtActividadEdit.GetText().trim() != '') tieneDatos = true;
                if (typeof txtResponsableEdit !== 'undefined' && txtResponsableEdit.GetText().trim() != '') tieneDatos = true;
                if (typeof txtRFCEdit !== 'undefined' && txtRFCEdit.GetText().trim() != '') tieneDatos = true;
                if (typeof txtEstanciaEdit !== 'undefined' && txtEstanciaEdit.GetText().trim() != '') tieneDatos = true;
                if (typeof dteFechaInicioEdit !== 'undefined' && dteFechaInicioEdit.GetValue() != null) tieneDatos = true;
                if (typeof dteFechaFinEdit !== 'undefined' && dteFechaFinEdit.GetValue() != null) tieneDatos = true;

                return tieneDatos;
            } catch (ex) {
                console.error('Error al verificar datos del formulario:', ex);
                return false;
            }
        }

        // Cancelar edición con confirmación
        function CancelarEdicion(s, e) {
            e.processOnServer = false;

            if (EditFormTieneDatos()) {
                pcConfirmarCancelacionEdit.Show();
            } else {
                gridSolicitudesRH.CancelEdit();
            }
        }

        // Confirmar cancelación del EditForm
        function ConfirmarCancelacionEdit() {
            pcConfirmarCancelacionEdit.Hide();
            gridSolicitudesRH.CancelEdit();
        }

        // Cancelar la cancelación del EditForm (quedarse en el formulario)
        function CancelarLaCancelacionEdit() {
            pcConfirmarCancelacionEdit.Hide();
        }

    </script>

    <!-- Grid de Solicitudes RH -->
    <asp:Table runat="server" Width="90%" HorizontalAlign="Center">
        <asp:TableRow>
            <asp:TableCell>
                <div style="padding-top: 8px">
                    <dx:ASPxLabel runat="server" ID="ASPxLabel7" Text="Solicitudes de Visitas" Font-Bold="true" Font-Size="X-Large"></dx:ASPxLabel>
                </div>
                <br />

                <!-- Incluir el User Control de Popups -->
                <uc:PopupMessages ID="popupMessages" runat="server" />

                <!-- Popup de Confirmación de Eliminación -->
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

                <!-- Popup de Confirmación de Cancelación del EditForm -->
                <dx:ASPxPopupControl ID="pcConfirmarCancelacionEdit" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
                    PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarCancelacionEdit"
                    HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
                    <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
                    <ContentCollection>
                        <dx:PopupControlContentControl runat="server">
                            <div style="padding: 30px; text-align: center;">
                                <dx:ASPxLabel runat="server" Text="¿Está seguro que desea cancelar? Se perderán los cambios no guardados." Font-Size="16px" Font-Bold="true" />
                                <br />
                                <br />
                            </div>
                        </dx:PopupControlContentControl>
                    </ContentCollection>
                    <FooterContentTemplate>
                        <div style="text-align: center; padding: 10px;">
                            <dx:ASPxButton ID="btnConfirmarCancelarEdit" runat="server" Text="Sí" Width="120px" AutoPostBack="False"
                                BackColor="Teal" ForeColor="White" Font-Bold="true" Style="margin-left: 10px;">
                                <ClientSideEvents Click="ConfirmarCancelacionEdit" />
                            </dx:ASPxButton>
                            <dx:ASPxButton ID="btnNoCancelarEdit" runat="server" Text="No" Width="120px" AutoPostBack="False"
                                BackColor="DarkRed" ForeColor="White" Font-Bold="true" Style="margin-left: 90px;">
                                <ClientSideEvents Click="CancelarLaCancelacionEdit" />
                            </dx:ASPxButton>
                        </div>
                    </FooterContentTemplate>
                </dx:ASPxPopupControl>

                <dx:ASPxGridView ID="gridSolicitudesRH" runat="server"
                    KeyFieldName="ID_Solicitud"
                    Width="100%"
                    ForeColor="Black"
                    ClientInstanceName="gridSolicitudesRH"
                    OnDataBinding="gridSolicitudesRH_DataBinding"
                    OnRowInserting="gridSolicitudesRH_RowInserting"
                    OnRowUpdating="gridSolicitudesRH_RowUpdating"
                    OnCustomButtonCallback="gridSolicitudesRH_CustomButtonCallback"
                    OnCustomCallback="gridSolicitudesRH_CustomCallback"
                    OnHtmlEditFormCreated="gridUsuarios_HtmlEditFormCreated">
                    <ClientSideEvents
                        EndCallback="OnGridSolicitudesEndCallback"
                        CustomButtonClick="OnCustomButtonClickSolicitud" />
                    <Styles>
                        <Header BackColor="#353943" ForeColor="White" Font-Bold="true"></Header>
                    </Styles>
                    <Columns>
                        <dx:GridViewCommandColumn Caption="Acciones" Width="100px"
                            ShowNewButtonInHeader="true"
                            ShowEditButton="true"
                            ButtonRenderMode="Image">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDeleteSolicitud" Text="Eliminar">
                                    <Image Url="~/Images/delete.png" Width="25px" Height="25px" ToolTip="Eliminar" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataTextColumn FieldName="ID_Solicitud" Caption="ID" Visible="false" ReadOnly="true" />

                        <dx:GridViewDataComboBoxColumn FieldName="id_TipoSolicitud" Caption="Tipo Solicitud" Width="120px">
                            <PropertiesComboBox TextField="Visita" ValueField="ID_TipoVisita" ValueType="System.Int32" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="Visitante" Caption="Visitante" Width="50%" />

                        <dx:GridViewDataComboBoxColumn FieldName="id_Proyecto" Caption="Proyecto" Width="150px">
                            <PropertiesComboBox TextField="NombreProyecto" ValueField="ID_Proyecto" ValueType="System.Int32" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="id_Planta" Caption="Planta" Width="120px">
                            <PropertiesComboBox TextField="NombrePlanta" ValueField="ID_Planta" ValueType="System.Int32" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="id_Contratista" Caption="Contratista" Width="150px">
                            <PropertiesComboBox TextField="Responsable" ValueField="id_contratista" ValueType="System.Int32" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="AreaTrabajo" Caption="Área de Trabajo" Width="150px" />

                        <dx:GridViewDataTextColumn FieldName="Actividad" Caption="Actividad" Width="150px" />

                        <dx:GridViewDataTextColumn FieldName="Estancia" Caption="Estancia" Width="100px" />

                        <dx:GridViewDataDateColumn FieldName="FechaInicio" Caption="Fecha Inicio" Width="110px">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                        </dx:GridViewDataDateColumn>

                        <dx:GridViewDataDateColumn FieldName="FechaFin" Caption="Fecha Fin" Width="110px">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                        </dx:GridViewDataDateColumn>

                        <dx:GridViewDataDateColumn FieldName="FechaSolicitud" Caption="Fecha Solicitud" Width="110px" ReadOnly="true">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                        </dx:GridViewDataDateColumn>

                        <dx:GridViewDataTextColumn FieldName="RFC" Caption="RFC" Width="120px" />

                        <dx:GridViewDataTextColumn FieldName="Responsable" Caption="Responsable" Width="150px" />

                        <dx:GridViewDataCheckColumn FieldName="aprobado" Caption="Aprobado" Width="80px" />
                    </Columns>
                    <Templates>
                        <EditForm>
                            <div style="padding: 20px;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="padding: 10px; width: 50%;">
                                            <dx:ASPxLabel runat="server" Text="Tipo Solicitud:" Font-Bold="true" />
                                            <dx:ASPxComboBox ID="cboTipoSolicitudEdit" runat="server" Width="100%"
                                                Value='<%# Bind("id_TipoSolicitud") %>'
                                                TextField="Visita"
                                                ValueField="ID_TipoVisita"
                                                ValueType="System.Int32"
                                                ClientInstanceName="cboTipoSolicitudEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxComboBox>
                                        </td>
                                        <td style="padding: 10px; width: 50%;">
                                            <dx:ASPxLabel runat="server" Text="Visitante:" Font-Bold="true" />
                                            <dx:ASPxTextBox ID="txtVisitanteEdit" runat="server" Width="100%"
                                                Text='<%# Bind("Visitante") %>'
                                                ClientInstanceName="txtVisitanteEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Proyecto:" Font-Bold="true" />
                                            <dx:ASPxComboBox ID="cboProyectoEdit" runat="server" Width="100%"
                                                Value='<%# Bind("id_Proyecto") %>'
                                                TextField="NombreProyecto"
                                                ValueField="ID_Proyecto"
                                                ValueType="System.Int32"
                                                ClientInstanceName="cboProyectoEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxComboBox>
                                        </td>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Planta:" Font-Bold="true" />
                                            <dx:ASPxComboBox ID="cboPlantaEdit" runat="server" Width="100%"
                                                Value='<%# Bind("id_Planta") %>'
                                                TextField="NombrePlanta"
                                                ValueField="ID_Planta"
                                                ValueType="System.Int32"
                                                ClientInstanceName="cboPlantaEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Contratista:" Font-Bold="true" />
                                            <dx:ASPxComboBox ID="cboContratistaEdit" runat="server" Width="100%"
                                                Value='<%# Bind("id_Contratista") %>'
                                                TextField="Responsable"
                                                ValueField="id_contratista"
                                                ValueType="System.Int32"
                                                ClientInstanceName="cboContratistaEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxComboBox>
                                        </td>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Área de Trabajo:" Font-Bold="true" />
                                            <dx:ASPxTextBox ID="txtAreaTrabajoEdit" runat="server" Width="100%"
                                                Text='<%# Bind("AreaTrabajo") %>'
                                                ClientInstanceName="txtAreaTrabajoEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Actividad:" Font-Bold="true" />
                                            <dx:ASPxMemo ID="txtActividadEdit" runat="server" Width="100%" Rows="3"
                                                Text='<%# Bind("Actividad") %>'
                                                ClientInstanceName="txtActividadEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxMemo>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Responsable:" Font-Bold="true" />
                                            <dx:ASPxTextBox ID="txtResponsableEdit" runat="server" Width="100%"
                                                Text='<%# Bind("Responsable") %>'
                                                ClientInstanceName="txtResponsableEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="RFC:" Font-Bold="true" />
                                            <dx:ASPxTextBox ID="txtRFCEdit" runat="server" Width="100%"
                                                Text='<%# Bind("RFC") %>'
                                                ClientInstanceName="txtRFCEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                    <RegularExpression ValidationExpression="^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Estancia:" Font-Bold="true" />
                                            <dx:ASPxTextBox ID="txtEstanciaEdit" runat="server" Width="100%"
                                                Text='<%# Bind("Estancia") %>'
                                                ClientInstanceName="txtEstanciaEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Fecha Inicio:" Font-Bold="true" />
                                            <dx:ASPxDateEdit ID="dteFechaInicioEdit" runat="server" Width="100%"
                                                Value='<%# Bind("FechaInicio") %>'
                                                DisplayFormatString="dd/MM/yyyy"
                                                EditFormat="Date"
                                                ClientInstanceName="dteFechaInicioEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel runat="server" Text="Fecha Fin:" Font-Bold="true" />
                                            <dx:ASPxDateEdit ID="dteFechaFinEdit" runat="server" Width="100%"
                                                Value='<%# Bind("FechaFin") %>'
                                                DisplayFormatString="dd/MM/yyyy"
                                                EditFormat="Date"
                                                ClientInstanceName="dteFechaFinEdit">
                                                <ValidationSettings ValidationGroup="EditForm" Display="Dynamic" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText=" " />
                                                </ValidationSettings>
                                            </dx:ASPxDateEdit>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px;">
                                            <dx:ASPxLabel ID="lblAprobadoEdit" runat="server" Text="Aprobado:" Font-Bold="true" />
                                            <dx:ASPxCheckBox ID="chkAprobadoEdit" runat="server"
                                                Checked='<%# Eval("aprobado") != null ? (bool)Eval("aprobado") : false %>' />
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                                <div style="padding-top: 20px;">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td style="text-align: left; width: 50%;">
                                                <dx:ASPxButton ID="btnUpdate" runat="server" Text="Guardar"
                                                    ValidationGroup="EditForm"
                                                    CausesValidation="true"
                                                    AutoPostBack="true"
                                                    BackColor="Teal" ForeColor="White" Font-Bold="true">
                                                    <ClientSideEvents Click="ValidarYGuardarEditForm" />
                                                </dx:ASPxButton>
                                            </td>
                                            <td style="text-align: right; width: 50%;">
                                                <dx:ASPxButton ID="btnCancel" runat="server" Text="Cancelar"
                                                    CausesValidation="false"
                                                    AutoPostBack="false"
                                                    BackColor="DarkRed" ForeColor="White" Font-Bold="true">
                                                    <ClientSideEvents Click="CancelarEdicion" />
                                                </dx:ASPxButton>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </EditForm>
                    </Templates>

                    <SettingsPager PageSize="10">
                        <PageSizeItemSettings ShowAllItem="true" Visible="true"></PageSizeItemSettings>
                    </SettingsPager>
                    <SettingsEditing Mode="PopupEditForm" />
                    <SettingsPopup>
                        <EditForm Modal="true"
                            Width="700" Height="700"
                            HorizontalAlign="WindowCenter"
                            VerticalAlign="WindowCenter"
                            ShowCloseButton="false" />
                    </SettingsPopup>
                    <SettingsText PopupEditFormCaption=" " />
                    <SettingsCommandButton>
                        <NewButton>
                            <Image Url="~/Images/add.png" Width="30px" Height="30px" ToolTip="Nueva Solicitud" />
                        </NewButton>
                        <EditButton>
                            <Image Url="~/Images/edits.png" Width="25px" Height="25px" ToolTip="Editar" />
                        </EditButton>
                        <UpdateButton Text="Guardar" RenderMode="Button" Styles-Style-HorizontalAlign="left">
                            <Styles>
                                <Style BackColor="Teal" ForeColor="White" Font-Bold="true" CssClass="btn" />
                            </Styles>
                        </UpdateButton>
                        <CancelButton Text="Cancelar" RenderMode="Button">
                            <Styles>
                                <Style BackColor="DarkRed" ForeColor="White" Font-Bold="true" CssClass="btn" />
                            </Styles>
                        </CancelButton>
                    </SettingsCommandButton>
                </dx:ASPxGridView>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>