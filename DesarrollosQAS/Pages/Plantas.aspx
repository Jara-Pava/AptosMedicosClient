<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Plantas.aspx.cs" Inherits="DesarrollosQAS.Pages.Plantas" MasterPageFile="~/Root.master" %>

<asp:Content ID="ContentModulo" ContentPlaceHolderID="Content" runat="server">
    <style>
        .dxflLastChildInRowSys {
            align-content: baseline;
        }
    </style>

    <script type="text/javascript">
        var pendingSuccessMessage = "";

        // Manejar mensajes después de operaciones del grid
        function OnGridPlantasEndCallback(s, e) {
            // Si hay un mensaje de éxito pendiente (viene de un CancelEdit anterior), mostrarlo
            if (pendingSuccessMessage) {
                var msg = pendingSuccessMessage;
                pendingSuccessMessage = "";
                setTimeout(function () {
                    lblMensajeExito.SetText(msg);
                    pcMensajeExito.Show();
                }, 100);
                return;
            }

            if (s.cpMessageType && s.cpMessage) {
                console.log("llegando al metodo de EndCallback", s.cpMessage)
                if (s.cpMessageType === "success") {
                    var successMsg = s.cpMessage;

                    delete s.cpMessageType;
                    delete s.cpMessage;
                    delete s.cpShouldCloseEdit;

                    if (gridPlantas.IsEditing()) {
                        pendingSuccessMessage = successMsg;
                        gridPlantas.CancelEdit();
                    } else {
                        setTimeout(function () {
                            lblMensajeExito.SetText(successMsg);
                            pcMensajeExito.Show();
                        }, 100);
                    }

                } else if (s.cpMessageType === "error") {
                    // Guardar mensaje y limpiar propiedades
                    var errorMsg = s.cpMessage;

                    delete s.cpMessageType;
                    delete s.cpMessage;
                    delete s.cpShouldReopenEdit;
                    delete s.cpIsNewRow;
                    delete s.cpEditIndex;

                    // NO cerrar el formulario — mostrar el error encima del EditForm abierto
                    setTimeout(function () {
                        lblMensajeError.SetText(errorMsg);
                        pcMensajeError.Show();
                    }, 100);
                }
            }
        }

        // Variable global para guardar el índice de la fila a eliminar
        var currentDeleteIndex = -1;

        // Manejar botón personalizado de eliminar
        function OnCustomButtonClickPlantas(s, e) {
            if (e.buttonID === 'btnDeletePlanta') {
                e.processOnServer = false;
                currentDeleteIndex = e.visibleIndex;
                pcConfirmarEliminacion.Show();
            }
        }

        // Confirmar eliminación desde el popup
        function ConfirmarEliminacion() {
            if (currentDeleteIndex >= 0) {
                gridPlantas.PerformCallback('DELETE|' + currentDeleteIndex);
                pcConfirmarEliminacion.Hide();
            }
        }

        // Cancelar eliminación
        function CancelarEliminacion() {
            currentDeleteIndex = -1;
            pcConfirmarEliminacion.Hide();
        }

        // Guardar cambios del formulario de edición
        function GuardarPlanta(s, e) {
            //Console para verificar que se está llamando a la función
            console.log("Guardando contratista...");

            gridPlantas.UpdateEdit();
        }

        // Cancelar edición
        function CancelarEdicion(s, e) {
            pendingSuccessMessage = "";
            gridPlantas.CancelEdit();
        }
    </script>

    <!-- Popup de Confirmación de Eliminación -->
    <dx:ASPxPopupControl ID="pcConfirmarEliminacion" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarEliminacion"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="true">
        <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <div style="padding: 30px; text-align: center;">
                    <dx:ASPxLabel runat="server" Text="¿Está seguro que desea eliminar esta Planta?" Font-Size="16px" Font-Bold="true" />
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
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="true">
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
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="true">
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
                    <ClientSideEvents Click="function(s, e) { pcMensajeError.Hide(); }" />
                </dx:ASPxButton>
            </div>
        </FooterContentTemplate>
    </dx:ASPxPopupControl>

    <div style="padding-top: 8px; padding-left: 4%; padding-right: 4%">
        <dx:ASPxLabel runat="server" ID="lblTitulo" Text="Plantas" Font-Bold="true" Font-Size="X-Large"></dx:ASPxLabel>
    </div>
    <br />

    <dx:ASPxGridView ID="gridPlantas" runat="server" KeyFieldName="ID_Planta" AutoGenerateColumns="False"
        ClientInstanceName="gridPlantas"
        OnDataBinding="gridPlantas_DataBinding"
        OnRowUpdating="gridPlantas_RowUpdating"
        OnRowInserting="gridPlantas_RowInserting"
        OnCustomCallback="gridPlantas_CustomCallback"
        OnHtmlEditFormCreated="gridPlantas_HtmlEditFormCreated"
        Width="100%" Paddings-PaddingLeft="4%" Paddings-PaddingRight="4%">
        <ClientSideEvents
            EndCallback="OnGridPlantasEndCallback"
            CustomButtonClick="OnCustomButtonClickPlantas" />
        <Styles>
            <Header Font-Bold="true"></Header>
        </Styles>
        <Columns>
            <dx:GridViewCommandColumn Caption="Acciones" Width="50"
                ShowNewButtonInHeader="true"
                ShowEditButton="true"
                ButtonRenderMode="Image">
                <CustomButtons>
                    <dx:GridViewCommandColumnCustomButton ID="btnDeletePlanta" Text="Eliminar">
                        <Image Url="~/Images/delete.png" Width="18px" Height="18px" ToolTip="Eliminar" />
                    </dx:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="ID_Planta" Visible="False" ReadOnly="True" />
            <dx:GridViewDataTextColumn FieldName="NombrePlanta" Caption="Nombre" HeaderStyle-HorizontalAlign="Center" />
            <dx:GridViewDataCheckColumn FieldName="Activo" Caption="Activo" HeaderStyle-HorizontalAlign="Center" />
        </Columns>

        <Templates>
            <EditForm>
                <dx:ASPxFormLayout runat="server" ID="FormLayoutPlanta" Width="100%" Paddings-Padding="0">
                    <Styles LayoutGroupBox-Caption-Font-Size="X-Large" LayoutGroupBox-Caption-Font-Bold="true" LayoutGroupBox-Caption-ForeColor="#353943"></Styles>
                    <Styles LayoutGroupBox-Caption-BackgroundImage-HorizontalPosition="center" LayoutItem-Caption-ForeColor="#353943" LayoutGroupBox-Caption-Paddings-Padding="0"></Styles>
                    <Items>
                        <dx:LayoutGroup ShowCaption="false" ColumnCount="2" SettingsItemCaptions-Location="Top" ParentContainerStyle-Paddings-Padding="0" Paddings-PaddingLeft="25" Paddings-PaddingRight="25">
                            <CellStyle Font-Size="14px" />
                            <GroupBoxStyle Border-BorderStyle="None" />
                            <Items>
                                <%-- TxtBox Nombre --%>
                                <dx:LayoutItem Caption="Planta" ColumnSpan="1">
                                    <%--<ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>--%>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox ID="txtPlanta" runat="server" Width="100%" ClientInstanceName="txtPlanta"
                                                Text='<%# Bind("NombrePlanta") %>' AutoResizeWithContainer="true" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <%-- Activo --%>
                                <dx:LayoutItem ColumnSpan="1" FieldName="layoutItemActivo" Caption="Activo" CaptionSettings-Location="Left">
                                    <NestedControlCellStyle></NestedControlCellStyle>
                                    <%--<CaptionSettings Location="Top"/>--%>
                                    <%--<ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>--%>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo"
                                                Checked='<%# Eval("Activo") == null ? true : (bool)Eval("Activo") %>' />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <%-- Guardar --%>
                                <%--                                <dx:LayoutItem ColumnSpan="1" ShowCaption="false" Paddings-PaddingTop="30">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <div style="text-align: left;">
                                                <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Width="120px" AutoPostBack="false"
                                                    BackColor="Teal" ForeColor="White" Font-Bold="true">
                                                    <ClientSideEvents Click="GuardarPlanta" />
                                                </dx:ASPxButton>
                                            </div>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>--%>

                                <%-- BOTONES DE EDICIÓN POP --%>
                                <dx:LayoutItem ColumnSpan="2" ShowCaption="false" Paddings-PaddingTop="40" HorizontalAlign="Right">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>

                                            <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Width="120px" AutoPostBack="false"
                                                BackColor="Teal" ForeColor="White" Font-Bold="true" >
                                                <ClientSideEvents Click="GuardarPlanta" />
                                            </dx:ASPxButton>

                                            <dx:ASPxButton ID="btnCancelar" runat="server" Text="Cancelar" Width="120px" AutoPostBack="false"
                                                BackColor="DarkRed" ForeColor="White" Font-Bold="true" Style="margin-left: 8%;">
                                                <ClientSideEvents Click="CancelarEdicion"/>
                                            </dx:ASPxButton>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                    </Items>
                </dx:ASPxFormLayout>
            </EditForm>
        </Templates>

        <SettingsEditing Mode="PopupEditForm" />
        <SettingsPopup>
            <EditForm Modal="true"
                HorizontalAlign="WindowCenter"
                VerticalAlign="WindowCenter" ShowCloseButton="true">
            </EditForm>
        </SettingsPopup>
        <StylesPopup>
            <EditForm Header-BackColor="#353943" MainArea-Paddings-Padding="0"></EditForm>
        </StylesPopup>
        <SettingsText PopupEditFormCaption=" " />
        <SettingsText SearchPanelEditorNullText="Ingrese la planta a buscar ..." />
        <SettingsSearchPanel Visible="true" />
        <SettingsPager PageSize="25" />
        <StylesPager CurrentPageNumber-BackColor="#353943" PageSizeItem-HoverStyle-BackColor="Teal" />
        <SettingsCommandButton>
            <NewButton>
                <Image Url="~/Images/add.png" Width="30px" Height="30px" ToolTip="Nuevo Rol" />
            </NewButton>
            <EditButton>
                <Image Url="~/Images/edits.png" Width="18px" Height="18px" ToolTip="Editar" />
            </EditButton>
        </SettingsCommandButton>
    </dx:ASPxGridView>
</asp:Content>
