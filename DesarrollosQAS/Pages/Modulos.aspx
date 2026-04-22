<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Modulos.aspx.cs" Inherits="DesarrollosQAS.Pages.Modulo" MasterPageFile="~/Root.master" %>

<asp:Content ID="ContentModulo" ContentPlaceHolderID="Content" runat="server">
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
        .dxgvHeader_Office365 {
            background-color: #353943;
            color: white;
        }
    </style>

    <script type="text/javascript">
        var pendingSuccessMessage = "";

        function OnGridModuloEndCallback(s, e) {
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
                if (s.cpMessageType === "success") {
                    var successMsg = s.cpMessage;
                    delete s.cpMessageType;
                    delete s.cpMessage;
                    delete s.cpShouldCloseEdit;

                    if (gridModulo.IsEditing()) {
                        pendingSuccessMessage = successMsg;
                        gridModulo.CancelEdit();
                    } else {
                        setTimeout(function () {
                            lblMensajeExito.SetText(successMsg);
                            pcMensajeExito.Show();
                        }, 100);
                    }
                } else if (s.cpMessageType === "error") {
                    var errorMsg = s.cpMessage;
                    delete s.cpMessageType;
                    delete s.cpMessage;

                    setTimeout(function () {
                        lblMensajeError.SetText(errorMsg);
                        pcMensajeError.Show();
                    }, 100);
                }
            }
        }

        var currentDeleteIndex = -1;

        function OnCustomButtonClickModulo(s, e) {
            if (e.buttonID === 'btnDeleteModulo') {
                e.processOnServer = false;
                currentDeleteIndex = e.visibleIndex;
                pcConfirmarEliminacion.Show();
            }
        }

        function ConfirmarEliminacion() {
            if (currentDeleteIndex >= 0) {
                gridModulo.PerformCallback('DELETE|' + currentDeleteIndex);
                pcConfirmarEliminacion.Hide();
            }
        }

        function CancelarEliminacion() {
            currentDeleteIndex = -1;
            pcConfirmarEliminacion.Hide();
        }

        function GuardarModulo(s, e) {
            gridModulo.UpdateEdit();
        }

        function CancelarEdicion(s, e) {
            pendingSuccessMessage = "";
            gridModulo.CancelEdit();
        }
    </script>

    <!-- Popup de Confirmación de Eliminación -->
    <dx:ASPxPopupControl ID="pcConfirmarEliminacion" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarEliminacion"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
        <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <div style="padding: 30px; text-align: center;">
                    <dx:ASPxLabel runat="server" Text="¿Está seguro que desea eliminar este módulo/catálogo?" Font-Size="16px" Font-Bold="true" />
                    <br /><br />
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
                    <ClientSideEvents Click="function(s, e) { pcMensajeError.Hide(); }" />
                </dx:ASPxButton>
            </div>
        </FooterContentTemplate>
    </dx:ASPxPopupControl>

    <div style="padding-top: 8px">
        <dx:ASPxLabel runat="server" ID="lblTitulo" Text="Módulos / Catálogos" Font-Bold="true" Font-Size="X-Large"></dx:ASPxLabel>
    </div>
    <br />

    <%-- GRID PARA ADMINISTRAR MÓDULOS/CATÁLOGOS --%>
    <dx:ASPxGridView ID="gridModulo" runat="server" AutoGenerateColumns="False" KeyFieldName="id_modulo_catalogo"
        ClientInstanceName="gridModulo"
        OnRowUpdating="gridModulo_RowUpdating"
        OnRowInserting="gridModulo_RowInserting"
        OnDataBinding="gridModulo_DataBinding"
        OnCustomCallback="gridModulo_CustomCallback"
        OnHtmlEditFormCreated="gridModulo_HtmlEditFormCreated"
        Width="100%">
        <ClientSideEvents
            EndCallback="OnGridModuloEndCallback"
            CustomButtonClick="OnCustomButtonClickModulo" />
        <Styles>
            <Header Font-Bold="true"></Header>
        </Styles>
        <Columns>
            <dx:GridViewCommandColumn Caption="Acciones" Width="50"
                ShowNewButtonInHeader="true"
                ShowEditButton="true"
                ButtonRenderMode="Image">
                <CustomButtons>
                    <dx:GridViewCommandColumnCustomButton ID="btnDeleteModulo" Text="Eliminar">
                        <Image Url="~/Images/delete.png" Width="18px" Height="18px" ToolTip="Eliminar" />
                    </dx:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="id_modulo_catalogo" Visible="False" ReadOnly="True" />
            <dx:GridViewDataTextColumn FieldName="nombre" Caption="Nombre" CellStyle-HorizontalAlign="Center" Width="200" HeaderStyle-HorizontalAlign="Center" />
            <dx:GridViewDataTextColumn FieldName="descripcion" Caption="Descripción" HeaderStyle-HorizontalAlign="Center" />
            <dx:GridViewDataCheckColumn FieldName="activo" Caption="Activo" Width="100" HeaderStyle-HorizontalAlign="Center" />
            <dx:GridViewDataDateColumn FieldName="fecha_creacion" Caption="Fecha de creación" ReadOnly="True" CellStyle-HorizontalAlign="Center" Width="150" HeaderStyle-HorizontalAlign="Center" />
            <dx:GridViewDataTextColumn FieldName="creado_por" Visible="false" ReadOnly="True" />
            <dx:GridViewDataTextColumn FieldName="nombre_creador" Caption="Creado por" ReadOnly="True" Width="150" HeaderStyle-HorizontalAlign="Center" />
        </Columns>

        <Templates>
            <EditForm>
                <dx:ASPxFormLayout runat="server" ID="FormLayoutModulo" Width="100%" Paddings-Padding="0">
                    <Styles LayoutGroupBox-Caption-Font-Size="X-Large" LayoutGroupBox-Caption-Font-Bold="true" LayoutGroupBox-Caption-ForeColor="#353943"></Styles>
                    <Styles LayoutGroupBox-Caption-BackgroundImage-HorizontalPosition="center" LayoutItem-Caption-ForeColor="#353943" LayoutGroupBox-Caption-Paddings-Padding="0"></Styles>
                    <Items>
                        <dx:LayoutGroup Caption=" " ShowCaption="false" ColumnCount="2" SettingsItemCaptions-Location="Top" CellStyle-Font-Size="0" ParentContainerStyle-Paddings-Padding="0" Paddings-Padding="25">
                            <CellStyle Font-Size="14px" />
                            <GroupBoxStyle Border-BorderStyle="None" />
                            <Items>
                                <%-- Nombre --%>
                                <dx:LayoutItem Caption="Nombre" ColumnSpan="2" Width="100%">
                                    <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox ID="txtNombre" runat="server" Width="100%" ClientInstanceName="nombre"
                                                Text='<%# Bind("nombre") %>' AutoResizeWithContainer="true" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <%-- Descripción --%>
                                <dx:LayoutItem Caption="Descripción" ColumnSpan="2" Width="100%">
                                    <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxMemo ID="txtDescripcion" runat="server" Width="100%" Rows="2" ClientInstanceName="descripcion"
                                                Text='<%# Bind("descripcion") %>' />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <%-- Creado por (hidden) --%>
                                <dx:LayoutItem Caption="Creado por" Visible="false" ShowCaption="false" ColumnSpan="2" Width="100%">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtCreado_por" runat="server" Width="100%" ClientInstanceName="creado_por"
                                                Text='<%# Bind("creado_por") %>' Visible="false" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <%-- Activo --%>
                                <dx:LayoutItem ColumnSpan="2" FieldName="layoutItemActivo" Caption="Activo" CaptionSettings-Location="Left">
                                    <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="activo"
                                                Checked='<%# Eval("activo") == null ? true : (bool)Eval("activo") %>' />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <%-- Guardar --%>
                                <dx:LayoutItem ColumnSpan="1" ShowCaption="false" HorizontalAlign="Left" Paddings-PaddingTop="80">
                                    <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Width="120px" AutoPostBack="false"
                                                BackColor="Teal" ForeColor="White" Font-Bold="true">
                                                <ClientSideEvents Click="GuardarModulo" />
                                            </dx:ASPxButton>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <%-- Cancelar --%>
                                <dx:LayoutItem ColumnSpan="1" ShowCaption="false" HorizontalAlign="Right" Paddings-PaddingTop="80">
                                    <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxButton ID="btnCancelar" runat="server" Text="Cancelar" Width="120px" AutoPostBack="false"
                                                BackColor="DarkRed" ForeColor="White" Font-Bold="true" Style="margin-left: 150px;">
                                                <ClientSideEvents Click="CancelarEdicion" />
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
            <EditForm Modal="true" Width="650px" HorizontalAlign="WindowCenter" VerticalAlign="WindowCenter" ShowCloseButton="false" />
        </SettingsPopup>
        <StylesPopup>
            <EditForm Header-BackColor="#353943" Content-Paddings-Padding="0"></EditForm>
        </StylesPopup>
        <SettingsText PopupEditFormCaption=" " />
        <SettingsText SearchPanelEditorNullText="Ingrese el módulo/catálogo a buscar ..." />
        <SettingsSearchPanel Visible="true" />
        <SettingsPager PageSize="25" />
        <StylesPager CurrentPageNumber-BackColor="#353943" PageSizeItem-HoverStyle-BackColor="Teal" />
        <SettingsCommandButton>
            <NewButton>
                <Image Url="~/Images/add.png" Width="30px" Height="30px" ToolTip="Nuevo Módulo/Catálogo" />
            </NewButton>
            <EditButton>
                <Image Url="~/Images/edits.png" Width="18px" Height="18px" ToolTip="Editar" />
            </EditButton>
        </SettingsCommandButton>
    </dx:ASPxGridView>
</asp:Content>