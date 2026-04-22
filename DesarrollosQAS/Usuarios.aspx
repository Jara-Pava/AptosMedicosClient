<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" MasterPageFile="~/Root.master" Inherits="DesarrollosQAS.Usuarios" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <script type="text/javascript">

        // Variables globales para guardar el estado de reapertura del formulario
        var shouldReopenForm = false;
        var isNewRowForm = false;
        var editRowIndex = -1;
        var formDataToRestore = null;
        var errorMessage = ""; // Variable para guardar el mensaje de error

        // Manejar mensajes después de operaciones del grid
        function OnGridEndCallback(s, e) {
            if (s.cpMessageType && s.cpMessage) {
                if (s.cpMessageType === "success") {
                    lblMensajeExito.SetText(s.cpMessage);
                    pcMensajeExito.Show();

                    // Resetear variables en caso de éxito
                    shouldReopenForm = false;
                    isNewRowForm = false;
                    editRowIndex = -1;
                    formDataToRestore = null;
                } else if (s.cpMessageType === "error") {
                    // Guardar el mensaje de error
                    errorMessage = s.cpMessage;

                    // Guardar información sobre si se debe reabrir el formulario
                    if (s.cpShouldReopenEdit) {
                        shouldReopenForm = true;
                        isNewRowForm = s.cpIsNewRow || false;
                        editRowIndex = s.cpEditIndex || -1;

                        // Guardar los valores del formulario antes de que se cierre
                        GuardarDatosFormulario();
                    }

                    // IMPORTANTE: Cerrar el formulario antes de mostrar el error
                    if (gridUsuarios.IsEditing()) {
                        gridUsuarios.CancelEdit();
                    }

                    // Mostrar el popup de error DESPUÉS de cerrar el formulario
                    setTimeout(function () {
                        lblMensajeError.SetText(errorMessage);
                        pcMensajeError.Show();
                    }, 250);
                }

                delete s.cpMessageType;
                delete s.cpMessage;
                delete s.cpShouldReopenEdit;
                delete s.cpIsNewRow;
                delete s.cpEditIndex;
            }
        }

        // Función para guardar los datos del formulario antes de cerrarlo
        function GuardarDatosFormulario() {
            try {
                var txtSiglaRed = ASPxClientControl.GetControlCollection().GetByName("txtSiglaRed");
                var txtNombre = ASPxClientControl.GetControlCollection().GetByName("txtNombre");
                var txtEmail = ASPxClientControl.GetControlCollection().GetByName("txtEmail");
                var chkActivo = ASPxClientControl.GetControlCollection().GetByName("chkActivo");

                if (txtSiglaRed && txtNombre && txtEmail) {
                    formDataToRestore = {
                        siglaRed: txtSiglaRed.GetValue(),
                        nombre: txtNombre.GetValue(),
                        email: txtEmail.GetValue(),
                        activo: chkActivo ? chkActivo.GetChecked() : true
                    };
                    console.log("Datos guardados:", formDataToRestore);
                } else {
                    console.log("No se encontraron todos los controles");
                }
            } catch (e) {
                console.log("Error guardando datos del formulario: " + e);
            }
        }

        // Función para restaurar los datos del formulario
        function RestaurarDatosFormulario() {
            if (formDataToRestore) {
                setTimeout(function () {
                    try {
                        var txtSiglaRed = ASPxClientControl.GetControlCollection().GetByName("txtSiglaRed");
                        var txtNombre = ASPxClientControl.GetControlCollection().GetByName("txtNombre");
                        var txtEmail = ASPxClientControl.GetControlCollection().GetByName("txtEmail");
                        var chkActivo = ASPxClientControl.GetControlCollection().GetByName("chkActivo");

                        if (txtSiglaRed) txtSiglaRed.SetValue(formDataToRestore.siglaRed);
                        if (txtNombre) txtNombre.SetValue(formDataToRestore.nombre);
                        if (txtEmail) txtEmail.SetValue(formDataToRestore.email);
                        if (chkActivo) chkActivo.SetChecked(formDataToRestore.activo);

                        console.log("Datos restaurados correctamente");
                        console.log("txtSiglaRed: ", txtSiglaRed);
                        console.log("txtNombre: ", txtNombre);
                        console.log("txtEmail: ", txtEmail);
                        console.log("chkActivo: ", chkActivo);
                    } catch (e) {
                        console.log("Error restaurando datos del formulario: " + e);
                    }
                }, 300);
            }
        }

        // Función para reabrir el formulario después de cerrar el popup de error
        function CerrarErrorYReopenForm() {
            pcMensajeError.Hide();

            if (shouldReopenForm) {
                // Reabrir el formulario después de un breve delay
                setTimeout(function () {
                    if (isNewRowForm) {
                        gridUsuarios.AddNewRow();
                    } else {
                        gridUsuarios.StartEditRow(editRowIndex);
                    }

                    // Restaurar los datos del formulario
                    RestaurarDatosFormulario();

                    // Resetear las variables
                    shouldReopenForm = false;
                    isNewRowForm = false;
                    editRowIndex = -1;
                }, 200);
            }
        }

        // Variable global para guardar el índice de la fila a eliminar
        var currentDeleteIndex = -1;

        // Confirmar antes de eliminar - Mostrar popup personalizado
        function OnCustomButtonClick(s, e) {
            if (e.buttonID === 'btnDelete') {
                e.processOnServer = false; // Cancelar el proceso del servidor
                currentDeleteIndex = e.visibleIndex; // Guardar el índice de la fila
                pcConfirmarEliminacion.Show(); // Mostrar popup de confirmación
            }
        }

        // Confirmar eliminación desde el popup
        function ConfirmarEliminacion() {
            if (currentDeleteIndex >= 0) {
                gridUsuarios.PerformCallback('DELETE|' + currentDeleteIndex);
                pcConfirmarEliminacion.Hide();
            }
        }

        // Cancelar eliminación
        function CancelarEliminacion() {
            currentDeleteIndex = -1;
            pcConfirmarEliminacion.Hide();
        }

        // Prevenir Enter en el formulario de edición
        function OnEditFormKeyDown(s, e) {
            if (e.htmlEvent.keyCode === 13) {
                e.htmlEvent.preventDefault();
                return false;
            }
        }

        // Guardar cambios del formulario de edición
        function GuardarUsuario(s, e) {
            if (ASPxClientEdit.ValidateGroup('EditForm')) {
                gridUsuarios.UpdateEdit();
            }
        }

        // Cancelar edición
        function CancelarEdicion(s, e) {
            // Resetear las variables cuando se cancela manualmente
            shouldReopenForm = false;
            isNewRowForm = false;
            editRowIndex = -1;
            formDataToRestore = null;

            gridUsuarios.CancelEdit();
        }

        // Filtrar grid por estado de usuario
        function OnFiltroEstadoChanged(s, e) {
            gridUsuarios.PerformCallback('FILTER|' + s.GetValue());
        }

    </script>

    <div style="padding-top: 8px">
        <dx:ASPxLabel runat="server" ID="ASPxLabel1" Text="Usuarios" Font-Bold="true" Font-Size="X-Large"></dx:ASPxLabel>
    </div>
    <br />

    <!-- Filtro de estado -->
    <div style="margin-bottom: 3%; display: flex; align-items: center; flex-direction: row; justify-content: flex-end">
        <dx:ASPxLabel runat="server" Text="Estatus:" Font-Bold="true" Style="margin-right: 8px;" />
        <dx:ASPxComboBox ID="cmbFiltroEstado" runat="server" Width="200px"
            ClientInstanceName="cmbFiltroEstado"
            ValueType="System.String"
            SelectedIndex="0">
            <Items>
                <dx:ListEditItem Text="Todos" Value="todos" />
                <dx:ListEditItem Text="Activos" Value="activos" />
                <dx:ListEditItem Text="Inactivos" Value="inactivos" />
            </Items>
            <ClientSideEvents SelectedIndexChanged="OnFiltroEstadoChanged" />
        </dx:ASPxComboBox>
    </div>

    <!-- Popup de Confirmación de Eliminación -->
    <dx:ASPxPopupControl ID="pcConfirmarEliminacion" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarEliminacion"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
        <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <div style="padding: 30px; text-align: center;">
                    <dx:ASPxLabel runat="server" Text="¿Está seguro que desea eliminar este usuario?" Font-Size="16px" Font-Bold="true" />
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
                <dx:ASPxButton ID="btnCerrarExito" runat="server" Text="OK" Width="100px" AutoPostBack="False" BackColor="Teal" ForeColor="White" Font-Bold="true">
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
                <dx:ASPxButton ID="btnCerrarError" runat="server" Text="OK" Width="100px" AutoPostBack="False" BackColor="Teal" ForeColor="White" Font-Bold="true">
                    <ClientSideEvents Click="CerrarErrorYReopenForm" />
                </dx:ASPxButton>
            </div>
        </FooterContentTemplate>
    </dx:ASPxPopupControl>

    <dx:ASPxGridView ID="gridUsuarios" runat="server"
        KeyFieldName="id_usuario"
        Width="100%"
        ForeColor="Black"
        ClientInstanceName="gridUsuarios"
        OnDataBinding="gridUsuarios_DataBinding"
        OnRowInserting="gridUsuarios_RowInserting"
        OnRowUpdating="gridUsuarios_RowUpdating"
        OnCustomButtonCallback="gridUsuarios_CustomButtonCallback"
        OnCustomCallback="gridUsuarios_CustomCallback"
        OnHtmlEditFormCreated="gridUsuarios_HtmlEditFormCreated">
        <ClientSideEvents
            EndCallback="OnGridEndCallback"
            CustomButtonClick="OnCustomButtonClick" />
        <Styles>
            <Header BackColor="#353943" ForeColor="White" Font-Bold="true"></Header>
        </Styles>
        <Columns>
            <dx:GridViewCommandColumn Caption="Acciones" Width="50"
                ShowNewButtonInHeader="true"
                ShowEditButton="true"
                ButtonRenderMode="Image">
                <CustomButtons>
                    <dx:GridViewCommandColumnCustomButton ID="btnDelete" Text="Eliminar">
                        <Image Url="~/Images/delete.png" Width="18px" Height="18px" ToolTip="Eliminar" />
                    </dx:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="id_usuario" Caption="ID" Visible="false" ReadOnly="true" />
            <dx:GridViewDataTextColumn FieldName="sigla_red" Caption="Sigla Red" HeaderStyle-HorizontalAlign="Center">
                <PropertiesTextEdit NullText="Ingrese sigla de red">
                    <ClientSideEvents KeyDown="OnEditFormKeyDown" />
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="nombre" Caption="Nombre" HeaderStyle-HorizontalAlign="Center">
                <PropertiesTextEdit NullText="Ingrese nombre completo">
                    <ClientSideEvents KeyDown="OnEditFormKeyDown" />
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Email" Caption="Email" HeaderStyle-HorizontalAlign="Center">
                <PropertiesTextEdit NullText="ejemplo@correo.com">
                    <ClientSideEvents KeyDown="OnEditFormKeyDown" />
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataCheckColumn FieldName="activo" Caption="Activo" HeaderStyle-HorizontalAlign="Center" />
            <dx:GridViewDataTextColumn FieldName="creado_por" Caption="Creado Por" HeaderStyle-HorizontalAlign="Center" ReadOnly="true" CellStyle-HorizontalAlign="Center"/>
            <dx:GridViewDataTextColumn FieldName="modificado_por" Caption="Modificado Por" HeaderStyle-HorizontalAlign="Center" ReadOnly="true" CellStyle-HorizontalAlign="Center" />
        </Columns>
        <Settings />
        <Templates>
            <EditForm>
                <div style="padding: 20px;">
                    <table style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td colspan="1" style="padding: 10px; vertical-align: top;">
                                <dx:ASPxLabel ID="lblSiglaRed" runat="server" Text="Sigla Red:" AssociatedControlID="txtSiglaRed" />
                                <span style="color: red;">*</span>
                                <br />
                                <dx:ASPxTextBox ID="txtSiglaRed" runat="server" Width="95%"
                                    ClientInstanceName="txtSiglaRed"
                                    Text='<%# Bind("sigla_red") %>'
                                    NullText="Ingrese sigla de red">
                                    <ClientSideEvents KeyDown="OnEditFormKeyDown" />
                                </dx:ASPxTextBox>
                            </td>
                            <td colspan="1" style="padding: 10px; vertical-align: top;">
                                <dx:ASPxLabel ID="lblNombre" runat="server" Text="Nombre:" AssociatedControlID="txtNombre" />
                                <span style="color: red;">*</span>
                                <br />
                                <dx:ASPxTextBox ID="txtNombre" runat="server" Width="95%"
                                    Text='<%# Bind("nombre") %>'
                                    NullText="Ingrese nombre completo"
                                    ClientInstanceName="txtNombre">
                                    <ClientSideEvents KeyDown="OnEditFormKeyDown" />
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding: 10px; vertical-align: top;">
                                <dx:ASPxLabel ID="lblEmail" runat="server" Text="Email:" AssociatedControlID="txtEmail" />
                                <span style="color: red;">*</span>
                                <br />
                                <dx:ASPxTextBox ID="txtEmail" runat="server" Width="97.5%"
                                    Text='<%# Bind("Email") %>'
                                    NullText="ejemplo@correo.com"
                                    ClientInstanceName="txtEmail">
                                    <ClientSideEvents KeyDown="OnEditFormKeyDown" />
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr id="trActivo" runat="server">
                            <td style="width: 50%; padding: 10px; vertical-align: top">
                                <dx:ASPxLabel ID="lblActivo" runat="server" Text="Activo:" AssociatedControlID="chkActivo" />
                                <dx:ASPxCheckBox ID="chkActivo" runat="server"
                                    Checked='<%# Eval("activo") == null ? true : (bool)Eval("activo") %>'
                                    Text="" RootStyle-HoverStyle-BackColor="Teal"
                                    ClientInstanceName="chkActivo" />
                            </td>
                            <td style="width: 50%; padding: 10px; vertical-align: top;">
                                <!-- Celda vacía para mantener el diseño balanceado -->
                            </td>
                        </tr>
                    </table>

                    <div style="text-align: center; margin-top: 20px; padding-top: 15px; border-top: 1px solid #ddd;">
                        <dx:ASPxButton ID="btnUpdate" runat="server" Text="Guardar" Width="120px" ToolTip="Guardar" AutoPostBack="false" BackColor="Teal" ForeColor="White" Font-Bold="true">
                            <ClientSideEvents Click="GuardarUsuario" />
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnCancel" runat="server" Text="Cancelar" Width="120px" ToolTip="Cancelar" AutoPostBack="false" Style="margin-left: 150px;" BackColor="DarkRed" ForeColor="White" Font-Bold="true">
                            <ClientSideEvents Click="CancelarEdicion" />
                        </dx:ASPxButton>
                    </div>
                </div>
            </EditForm>
        </Templates>

        <SettingsPager PageSize="10" />
        <SettingsEditing Mode="PopupEditForm" />
        <SettingsPopup>
            <EditForm Modal="true"
                Width="650px"
                HorizontalAlign="WindowCenter"
                VerticalAlign="WindowCenter" ShowCloseButton="false" />
        </SettingsPopup>
        <SettingsText PopupEditFormCaption=" " />

        <SettingsCommandButton>
            <NewButton>
                <Image Url="~/Images/add.png" Width="30px" Height="30px" ToolTip="Nueva Solicitud" />
            </NewButton>
            <EditButton>
                <Image Url="~/Images/edits.png" Width="18px" Height="18px" ToolTip="Editar" />
            </EditButton>
            <UpdateButton>
                <Image Url="~/Images/comprobar.png" Width="25px" Height="25px" ToolTip="Guardar" />
            </UpdateButton>
            <CancelButton>
                <Image Url="~/Images/cancel.png" Width="18px" Height="18px" ToolTip="Cancelar" />
            </CancelButton>
        </SettingsCommandButton>
    </dx:ASPxGridView>
</asp:Content>
