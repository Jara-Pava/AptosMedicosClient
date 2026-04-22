<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitudEspecial.aspx.cs" Inherits="DesarrollosQAS.SolicitudEspecial" MasterPageFile="~/Root.master" %>

<asp:Content ID="contentSolicitudEspecial" ContentPlaceHolderID="Content" runat="Server">
    <style type="text/css">
        tr > .dxflCaptionCell_Office365 {
            padding-bottom: 15px !important;
        }
        /*Centrar el caption del LayoutGroup*/
        .dxflGroupBox_Office365 {
            margin-bottom: 0px;
            padding: 0 0 12px;
            text-align: center;
        }
    </style>

    <script type="text/javascript">
        // Variable para identificar el modo (edición o creación)
        var esEdicion = <%= Request.QueryString["id"] != null ? "true" : "false" %>;

        // Variables para almacenar valores originales en modo edición
        var valoresOriginales = {
            tipoSolicitud: null,
            proyecto: null,
            visitante: null,
            actividad: null
        };

        // Función que se ejecuta cuando la página está completamente cargada
        function OnPageLoad() {
            // Si estamos en modo edición, guardar los valores iniciales
            if (esEdicion) {
                GuardarValoresOriginales();
            }
        }

        // Guardar los valores originales de los campos críticos
        function GuardarValoresOriginales() {
            valoresOriginales.tipoSolicitud = cboTipoSolicitud.GetValue();
            valoresOriginales.proyecto = cboProyecto.GetValue();
            valoresOriginales.visitante = txtVisitante.GetText();
            valoresOriginales.actividad = txtActividad.GetText();
        }

        // Verificar si se han modificado los campos críticos en modo edición
        function CamposModificados() {
            if (!esEdicion) {
                return false;
            }

            var tipoActual = cboTipoSolicitud.GetValue();
            var proyectoActual = cboProyecto.GetValue();
            var visitanteActual = txtVisitante.GetText();
            var actividadActual = txtActividad.GetText();

            // Comparar valores actuales con originales
            return (tipoActual !== valoresOriginales.tipoSolicitud) ||
                (proyectoActual !== valoresOriginales.proyecto) ||
                (visitanteActual !== valoresOriginales.visitante) ||
                (actividadActual !== valoresOriginales.actividad);
        }

        // Función de validación antes de guardar (ahora solo se procesa en servidor)
        function ValidarYGuardar(s, e) {
            // Permitir el postback - la validación se hará en el servidor
            e.processOnServer = true;
        }

        // Función para verificar si hay datos en el formulario (modo creación)
        function FormularioTieneDatos() {
            return (cboTipoSolicitud.GetValue() != null) ||
                (cboProyecto.GetValue() != null) ||
                (txtVisitante.GetText().trim() != '') ||
                (txtActividad.GetText().trim() != '');
        }

        // Función para cancelar con confirmación
        function CancelarFormulario(s, e) {
            if (FormularioTieneDatos()) {
                e.processOnServer = false;
                pcConfirmarCancelacion.Show();
            } else {
                window.location.href = 'SolicitudesEspeciales.aspx';
            }
        }

        // Confirmar cancelación desde el popup
        function ConfirmarCancelacion() {
            pcConfirmarCancelacion.Hide();
            window.location.href = 'SolicitudesEspeciales.aspx';
        }

        // Cancelar la cancelación (quedarse en el formulario)
        function CancelarLaCancelacion() {
            pcConfirmarCancelacion.Hide();
        }

        // Función para regresar (comportamiento diferente según el modo)
        function RegresarFormulario(s, e) {
            // Si está en modo edición, verificar si hay cambios
            if (esEdicion) {
                if (CamposModificados()) {
                    e.processOnServer = false;
                    pcConfirmarCancelacion.Show();
                } else {
                    window.location.href = 'SolicitudesEspeciales.aspx';
                }
            } else {
                // Si está en modo creación, verificar si hay datos
                if (FormularioTieneDatos()) {
                    e.processOnServer = false;
                    pcConfirmarCancelacion.Show();
                } else {
                    window.location.href = 'SolicitudesEspeciales.aspx';
                }
            }
        }

        // Ejecutar cuando se carga la página
        window.onload = function () {
            OnPageLoad();
        };
    </script>

    <!-- Campos ocultos para comunicación entre servidor y cliente -->
    <asp:HiddenField ID="hfMostrarMensaje" runat="server" Value="false" />
    <asp:HiddenField ID="hfTipoMensaje" runat="server" Value="" />
    <asp:HiddenField ID="hfTextoMensaje" runat="server" Value="" />

    <!-- Popup de Confirmación de Cancelación -->
    <dx:ASPxPopupControl ID="pcConfirmarCancelacion" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarCancelacion"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false">
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
                <dx:ASPxButton ID="btnConfirmarCancelar" runat="server" Text="Sí" Width="120px" AutoPostBack="False"
                    BackColor="Teal" ForeColor="White" Font-Bold="true" Style="margin-left: 10px;">
                    <ClientSideEvents Click="ConfirmarCancelacion" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNoNoCancelar" runat="server" Text="No" Width="120px" AutoPostBack="False"
                    BackColor="DarkRed" ForeColor="White" Font-Bold="true" Style="margin-left: 80px;">
                    <ClientSideEvents Click="CancelarLaCancelacion" />
                </dx:ASPxButton>
            </div>
        </FooterContentTemplate>
    </dx:ASPxPopupControl>

    <!-- Popup de Éxito -->
    <dx:ASPxPopupControl ID="pcMensajeExitoSolicitud" runat="server" Width="400" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcMensajeExitoSolicitud"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
        <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <div style="padding: 20px; text-align: center;">
                    <dx:ASPxLabel ID="lblMensajeExitoSolicitud" runat="server" Font-Size="14px" ClientInstanceName="lblMensajeExitoSolicitud" />
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterContentTemplate>
            <div style="text-align: center; padding: 10px;">
                <dx:ASPxButton ID="btnCerrarExitoSolicitud" runat="server" Text="OK" Width="100px" AutoPostBack="False" BackColor="Teal" ForeColor="White" Font-Bold="true">
                    <ClientSideEvents Click="function(s, e) { window.location.href='SolicitudesEspeciales.aspx'; }" />
                </dx:ASPxButton>
            </div>
        </FooterContentTemplate>
    </dx:ASPxPopupControl>

    <!-- Popup de Error -->
    <dx:ASPxPopupControl ID="pcMensajeErrorSolicitud" runat="server" Width="400" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcMensajeErrorSolicitud"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
        <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <div style="padding: 20px; text-align: center;">
                    <dx:ASPxLabel ID="lblMensajeErrorSolicitud" runat="server" Font-Size="14px" ClientInstanceName="lblMensajeErrorSolicitud" />
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterContentTemplate>
            <div style="text-align: center; padding: 10px;">
                <dx:ASPxButton ID="btnCerrarErrorSolicitud" runat="server" Text="OK" Width="100px" AutoPostBack="False" BackColor="Teal" ForeColor="White" Font-Bold="true">
                    <ClientSideEvents Click="function(s, e) { pcMensajeErrorSolicitud.Hide(); }" />
                </dx:ASPxButton>
            </div>
        </FooterContentTemplate>
    </dx:ASPxPopupControl>

    <%--Template para solicitud especial--%>
    <dx:ASPxFormLayout runat="server" ID="FormLayoutSolicitudEspecial" Paddings-PaddingTop="20px" RequiredMarkDisplayMode="None" EnableViewState="true" EncodeHtml="false" UseDefaultPaddings="false" Width="100%">
        <Styles LayoutGroupBox-Caption-Font-Size="X-Large" LayoutGroupBox-Caption-Font-Bold="true" LayoutGroupBox-Caption-ForeColor="#353943"></Styles>
        <Styles LayoutGroupBox-Caption-BackgroundImage-HorizontalPosition="center" LayoutItem-Caption-ForeColor="#353943"></Styles>
        <Items>
            <dx:LayoutGroup Caption="Solicitud" ColumnCount="4" SettingsItemCaptions-Location="Top" CellStyle-Font-Size="14px">
                <CellStyle Font-Size="14px" />
                <GroupBoxStyle Border-BorderStyle="None" />
                <Items>
                    <dx:LayoutItem Caption=" " ColumnSpan="4">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <div style="padding-left: 20px; text-align: right">
                                    <dx:ASPxButton runat="server" ID="btnRegresar" Text="Regresar"
                                        Width="200px" CssClass="btn" BackColor="#353943" ForeColor="White" Font-Bold="true" AutoPostBack="false">
                                        <ClientSideEvents Click="RegresarFormulario" />
                                    </dx:ASPxButton>
                                </div>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <%--Combobox Tipo Solicitud--%>
                    <dx:LayoutItem Caption="Tipo Solicitud" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="cboTipoSolicitud" TextField="Visita" ValueField="ID_TipoVisita" ValueType="System.Int32" runat="server" Width="100%" ClientInstanceName="cboTipoSolicitud" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Combobox Proyecto--%>
                    <dx:LayoutItem Caption="Proyecto" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="cboProyecto" TextField="NombreProyecto" ValueField="ID_Proyecto" ValueType="System.Int32" runat="server" Width="100%" ClientInstanceName="cboProyecto" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Textbox Visitante--%>
                    <dx:LayoutItem Caption="Visitante" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox ID="txtVisitante" runat="server" Width="100%" ClientInstanceName="txtVisitante" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--ComboBox Planta--%>
                    <dx:LayoutItem Caption="Planta" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="cboPlanta" TextField="NombrePlanta" ValueField="ID_Planta" ValueType="System.Int32" runat="server" Width="100%" ClientInstanceName="cboPlanta" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--ComboBox Contratista--%>
                    <dx:LayoutItem Caption="Contratista" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxComboBox ID="cboContratista" TextField="Responsable" ValueField="id_contratista" ValueType="System.Int32" runat="server" Width="100%" ClientInstanceName="cboContratista" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Textbox Area de Trabajo--%>
                    <dx:LayoutItem Caption="Área de Trabajo" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox ID="txtAreaTrabajo" runat="server" Width="100%" ClientInstanceName="txtAreaTrabajo" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Memo Actividad--%>
                    <dx:LayoutItem Caption="Actividad" ColumnSpan="4">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server">
                                <dx:ASPxMemo ID="txtActividad" runat="server" Width="100%" Rows="3" ClientInstanceName="txtActividad" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Textbox Responsable--%>
                    <dx:LayoutItem Caption="Responsable" ColumnSpan="4">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox ID="txtResponsable" runat="server" Width="100%" ClientInstanceName="txtResponsable" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Textbox Estancia--%>
                    <dx:LayoutItem Caption="Estancia" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox ID="txtEstancia" runat="server" Width="100%" ClientInstanceName="txtEstancia" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Textbox RFC--%>
                    <dx:LayoutItem Caption="RFC" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxTextBox ID="txtRFC" runat="server" Width="100%" ClientInstanceName="txtRFC">
                                    <ClientSideEvents TextChanged="function(s, e) { 
                                        // Convertir a mayúsculas
                                        var valor = s.GetText().toUpperCase();
                                        s.SetText(valor);
                                    }" />
                                </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--DateEdit Fecha Inicio--%>
                    <dx:LayoutItem Caption="Fecha Inicio" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxDateEdit ID="dteFechaInicio" runat="server" EditFormat="Date" Width="100%" ClientInstanceName="dteFechaInicio" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--DateEdit Fecha Fin--%>
                    <dx:LayoutItem Caption="Fecha Fin" ColumnSpan="2">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxDateEdit ID="dteFechaFin" runat="server" EditFormat="Date" Width="100%" ClientInstanceName="dteFechaFin" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--CheckBox Aprobado--%>
                    <dx:LayoutItem FieldName="layoutItemAprobado" Caption="Aprobado" ColumnSpan="4" CaptionSettings-Location="Left">
                        <ParentContainerStyle Paddings-PaddingRight="12"></ParentContainerStyle>
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxCheckBox ID="chkAprobado" runat="server" ClientInstanceName="chkAprobado" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>

                    <%--Botón Guardar--%>
                    <dx:LayoutItem HorizontalAlign="Left" ShowCaption="False" ColumnSpan="2">
                        <Paddings PaddingTop="50" />
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxButton runat="server" ID="btnGuardar" Text="Guardar"
                                    Width="130" Height="40" CssClass="btn" BackColor="Teal" ForeColor="White" Font-Bold="true"
                                    AutoPostBack="true" ValidationGroup="Items" ClientInstanceName="btnGuardar">
                                    <ClientSideEvents Click="ValidarYGuardar" />
                                </dx:ASPxButton>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>


                    <%--Botón Cancelar--%>
                    <dx:LayoutItem HorizontalAlign="Right" ShowCaption="False" ColumnSpan="2">
                        <Paddings PaddingTop="50" PaddingRight="12" />
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer>
                                <dx:ASPxButton runat="server" ID="btnCancelar" Text="Cancelar"
                                    Width="130px" Height="40" CssClass="btn" BackColor="DarkRed" ForeColor="White" Font-Bold="true"
                                    AutoPostBack="false">
                                    <ClientSideEvents Click="CancelarFormulario" />
                                </dx:ASPxButton>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                </Items>
            </dx:LayoutGroup>
        </Items>
    </dx:ASPxFormLayout>
</asp:Content>
