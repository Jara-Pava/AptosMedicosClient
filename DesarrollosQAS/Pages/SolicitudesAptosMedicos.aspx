<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitudesAptosMedicos.aspx.cs" Inherits="DesarrollosQAS.Pages.SolicitudesAptosMedicos" MasterPageFile="~/Root.master" %>

<%@ Register Src="~/UserControls/PopupMessages.ascx" TagPrefix="uc" TagName="PopupMessages" %>

<asp:Content ID="ContentSolicitudesAptos" ContentPlaceHolderID="Content" runat="server">
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

        .grid-scroll-container {
            width: 100%;
            overflow-x: auto;
            overflow-y: visible;
        }

            .grid-scroll-container .dxgvControl {
                min-width: max-content;
            }

        .search-panel {
            display: flex;
            flex-direction: row;
            align-items: flex-end;
            flex-wrap: nowrap;
            gap: 12px;
            padding: 14px 16px;
            background-color: #F5F5F5;
            border: 1px solid #D0D0D0;
            border-radius: 4px;
            margin-bottom: 12px;
        }

        .search-field {
            display: flex;
            flex-direction: column;
        }

        .search-field-label {
            font-weight: bold;
            color: #353943;
            font-size: 13px;
            margin-bottom: 4px;
            white-space: nowrap;
        }

        .search-buttons {
            display: flex;
            flex-direction: row;
            align-items: flex-end;
            gap: 8px;
            margin-left: 4px;
        }
    </style>
        <script type="text/javascript">

            // ── Visibilidad dinámica ───────────────────────────────────────────────────
            var camposDinamicos = ['divIdSolicitud', 'divIdGlobal', 'divFechaSolicitud', 'divFechaInicio', 'divFechaFin'];

            function ocultarTodosLosCampos() {
                camposDinamicos.forEach(function (id) {
                    var el = document.getElementById(id);
                    if (el) el.style.display = 'none';
                });
            }

            function mostrarCampo(id) {
                var el = document.getElementById(id);
                if (el) el.style.display = 'flex';
            }

            function onTipoBusquedaChanged(s, e) {
                var valor = s.GetValue();
                var contenedor = document.getElementById('divSearchContent');
                if (contenedor) {
                    contenedor.style.display = valor ? 'contents' : 'none';
                }
                ocultarTodosLosCampos();
                switch (valor) {
                    case 'ID_SOLICITUD': mostrarCampo('divIdSolicitud'); break;
                    case 'ID_GLOBAL': mostrarCampo('divIdGlobal'); break;
                    case 'FECHA_SOLICITUD': mostrarCampo('divFechaSolicitud'); break;
                    case 'RANGO_FECHAS': mostrarCampo('divFechaInicio'); mostrarCampo('divFechaFin'); break;
                }
            }

            // ── Almacén de valores crudos tipeados (antes de que DevExpress los borre) ─
            var _rawFecha = { sol: '', ini: '', fin: '' };

            function iniciarSeguimientoFecha(control, key) {
                // Esperar a que DevExpress inicialice el input
                var intentos = 0;
                var intervalo = setInterval(function () {
                    var inputEl = control.GetInputElement();
                    if (inputEl) {
                        clearInterval(intervalo);
                        inputEl.addEventListener('input', function () {
                            _rawFecha[key] = inputEl.value.trim();
                        });
                        // También al pegar con el mouse
                        inputEl.addEventListener('paste', function () {
                            setTimeout(function () {
                                _rawFecha[key] = inputEl.value.trim();
                            }, 50);
                        });
                    } else if (++intentos > 20) {
                        clearInterval(intervalo);
                    }
                }, 100);
            }

            // ── Formatear Date a yyyy-MM-dd sin desplazamiento UTC ────────────────────
            function formatearFechaLocal(date) {
                var y = date.getFullYear();
                var m = date.getMonth() + 1;
                var d = date.getDate();
                return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d);
            }

            // ── Parsear fecha: primero el control, luego el valor crudo guardado ──────
            function parsearFecha(control, rawKey) {
                // 1. Si DevExpress reconoció la fecha correctamente
                var dateObj = control.GetValue();
                if (dateObj instanceof Date && !isNaN(dateObj)) {
                    return formatearFechaLocal(dateObj);
                }

                // 2. Usar el valor crudo capturado antes de que DevExpress lo borrara
                var raw = _rawFecha[rawKey] || '';
                if (!raw) return '';

                var d, m, y;

                // yyyy-MM-dd o yyyy/MM/dd
                var isoMatch = raw.match(/^(\d{4})[-\/](\d{1,2})[-\/](\d{1,2})$/);
                if (isoMatch) {
                    y = parseInt(isoMatch[1], 10);
                    m = parseInt(isoMatch[2], 10);
                    d = parseInt(isoMatch[3], 10);
                }

                // dd/MM/yyyy o dd-MM-yyyy
                var dmyMatch = raw.match(/^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/);
                if (!isoMatch && dmyMatch) {
                    d = parseInt(dmyMatch[1], 10);
                    m = parseInt(dmyMatch[2], 10);
                    y = parseInt(dmyMatch[3], 10);
                }

                if (!d || !m || !y) return raw;

                var fecha = new Date(y, m - 1, d);
                if (fecha.getFullYear() !== y || fecha.getMonth() !== m - 1 || fecha.getDate() !== d) {
                    return raw;
                }

                return formatearFechaLocal(fecha);
            }

            // ── Búsqueda ──────────────────────────────────────────────────────────────
            function EjecutarBusqueda() {
                var tipo = cbTipoBusqueda.GetValue();
                var valorTexto = '';
                var fechaSol = '';
                var fechaInicio = '';
                var fechaFin = '';

                switch (tipo) {
                    case 'ID_SOLICITUD':
                        valorTexto = txtBuscarIdSolicitud.GetValue() || '';
                        break;
                    case 'ID_GLOBAL':
                        valorTexto = txtBuscarIdGlobal.GetValue() || '';
                        break;
                    case 'FECHA_SOLICITUD':
                        fechaSol = parsearFecha(deBuscarFechaSolicitud, 'sol');
                        break;
                    case 'RANGO_FECHAS':
                        fechaInicio = parsearFecha(deBuscarFechaInicio, 'ini');
                        fechaFin = parsearFecha(deBuscarFechaFin, 'fin');
                        break;
                }

                gridSolicitudesAptosMedicos.PerformCallback(
                    'SEARCH|' + tipo + '|' + valorTexto + '|' + fechaSol + '|' + fechaInicio + '|' + fechaFin
                );
            }

            function LimpiarBusqueda() {
                txtBuscarIdSolicitud.SetValue('');
                txtBuscarIdGlobal.SetValue('');
                deBuscarFechaSolicitud.SetValue(null);
                deBuscarFechaInicio.SetValue(null);
                deBuscarFechaFin.SetValue(null);
                _rawFecha.sol = '';
                _rawFecha.ini = '';
                _rawFecha.fin = '';
            }

            // ── Estado inicial ────────────────────────────────────────────────────────
            window.addEventListener('load', function () {
                var contenedor = document.getElementById('divSearchContent');
                if (contenedor) contenedor.style.display = 'none';
                ocultarTodosLosCampos();

                // Iniciar seguimiento de valores crudos en los campos de fecha
                iniciarSeguimientoFecha(deBuscarFechaSolicitud, 'sol');
                iniciarSeguimientoFecha(deBuscarFechaInicio, 'ini');
                iniciarSeguimientoFecha(deBuscarFechaFin, 'fin');
            });

            // ── Callbacks del grid ────────────────────────────────────────────────────
            function OnGridSolicitudesEndCallback(s, e) {
                if (s.cpMessageType && s.cpMessage) {
                    if (s.cpMessageType === 'success') {
                        lblMensajeExito.SetText(s.cpMessage);
                        pcMensajeExito.Show();
                    } else if (s.cpMessageType === 'error') {
                        lblMensajeError.SetText(s.cpMessage);
                        pcMensajeError.Show();
                    }
                    delete s.cpMessageType;
                    delete s.cpMessage;
                }
            }

            function OnCustomButtonClickSolicitud(s, e) {
                if (e.buttonID === 'btnEditSolicitud') {
                    e.processOnServer = false;
                    var id = s.GetRowKey(e.visibleIndex);
                    if (id) {
                        window.location.href = 'SolicitudEspecial.aspx?id=' + id;
                    } else {
                        alert('No se pudo obtener el ID de la solicitud');
                    }
                    return;
                }
                if (e.buttonID === 'btnDeleteSolicitud') {
                    e.processOnServer = false;
                    currentDeleteIndex = e.visibleIndex;
                    pcConfirmarEliminacion.Show();
                }
            }

            var currentDeleteIndex = -1;

            function ConfirmarEliminacion() {
                if (currentDeleteIndex >= 0) {
                    gridSolicitudesAptosMedicos.PerformCallback('DELETE|' + currentDeleteIndex);
                    pcConfirmarEliminacion.Hide();
                }
            }

            function CancelarEliminacion() {
                currentDeleteIndex = -1;
                pcConfirmarEliminacion.Hide();
            }
            // ── Enter en campos de búsqueda: ejecutar búsqueda y cancelar postback ────
            function onBusquedaKeyDown(s, e) {
                var keyCode = e.htmlEvent.keyCode || e.htmlEvent.which;
                if (keyCode === 13) {
                    e.htmlEvent.preventDefault();
                    e.htmlEvent.stopPropagation();
                    EjecutarBusqueda();
                }
            }

            // ── Al salir del campo: restaurar la fecha si DevExpress la borró ─────────
            function onFechaLostFocus(control, rawKey) {
                // Si DevExpress ya reconoció la fecha, no hacer nada
                var dateObj = control.GetValue();
                if (dateObj instanceof Date && !isNaN(dateObj)) return;

                var raw = _rawFecha[rawKey] || '';
                if (!raw) return;

                var d, m, y;

                // yyyy-MM-dd o yyyy/MM/dd
                var isoMatch = raw.match(/^(\d{4})[-\/](\d{1,2})[-\/](\d{1,2})$/);
                if (isoMatch) {
                    y = parseInt(isoMatch[1], 10);
                    m = parseInt(isoMatch[2], 10);
                    d = parseInt(isoMatch[3], 10);
                }

                // dd/MM/yyyy o dd-MM-yyyy
                var dmyMatch = raw.match(/^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/);
                if (!isoMatch && dmyMatch) {
                    d = parseInt(dmyMatch[1], 10);
                    m = parseInt(dmyMatch[2], 10);
                    y = parseInt(dmyMatch[3], 10);
                }

                if (!d || !m || !y) return;

                var fecha = new Date(y, m - 1, d);
                if (fecha.getFullYear() !== y || fecha.getMonth() !== m - 1 || fecha.getDate() !== d) return;

                // Restaurar la fecha al control → DevExpress la muestra con DisplayFormatString (dd/MM/yyyy)
                control.SetValue(fecha);
            }
    </script>

    <uc:PopupMessages ID="popupMessages" runat="server" />

    <dx:ASPxPopupControl ID="pcConfirmarEliminacion" runat="server" Width="450" CloseAction="CloseButton" CloseOnEscape="true" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pcConfirmarEliminacion"
        HeaderText=" " PopupAnimationType="Fade" ShowFooter="true" ShowOnPageLoad="false" ShowCloseButton="false">
        <HeaderStyle BackColor="#353943" ForeColor="White" Font-Bold="true" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <div style="padding: 30px; text-align: center;">
                    <dx:ASPxLabel runat="server" Text="¿Está seguro que desea eliminar esta solicitud?" Font-Size="16px" Font-Bold="true" />
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

    <div style="padding-top: 8px">
        <dx:ASPxLabel runat="server" ID="ASPxLabel7" Text="Solicitudes" Font-Bold="true" Font-Size="X-Large" />
    </div>
    <br />

    <!-- ── Panel de búsqueda ─────────────────────────────────────────────────── -->
    <div class="search-panel">

        <!-- Tipo de búsqueda: SIEMPRE visible -->
        <div class="search-field">
            <span class="search-field-label">Tipo Búsqueda</span>
            <dx:ASPxComboBox ID="cbTipoBusqueda" runat="server" ClientInstanceName="cbTipoBusqueda" Width="160px">
                <Items>
                    <dx:ListEditItem Text="Seleccione..."   Value="" />
                    <dx:ListEditItem Text="ID Solicitud"    Value="ID_SOLICITUD" />
                    <dx:ListEditItem Text="ID Global"       Value="ID_GLOBAL" />
                    <dx:ListEditItem Text="Fecha Solicitud" Value="FECHA_SOLICITUD" />
                    <dx:ListEditItem Text="Rango de fechas" Value="RANGO_FECHAS" />
                </Items>
                <ClientSideEvents SelectedIndexChanged="onTipoBusquedaChanged" />
            </dx:ASPxComboBox>
        </div>

        <!-- Campos dinámicos + botones: ocultos hasta seleccionar tipo -->
        <div id="divSearchContent" style="display: none; contents: none;">

                        <!-- Campo dinámico: ID Solicitud -->
            <div id="divIdSolicitud" class="search-field" style="display: none;">
                <span class="search-field-label">ID Solicitud</span>
                <dx:ASPxTextBox ID="txtBuscarIdSolicitud" runat="server"
                    ClientInstanceName="txtBuscarIdSolicitud" Width="160px" NullText="ID Solicitud...">
                    <ClientSideEvents KeyDown="onBusquedaKeyDown" />
                </dx:ASPxTextBox>
            </div>

            <!-- Campo dinámico: ID Global -->
            <div id="divIdGlobal" class="search-field" style="display: none;">
                <span class="search-field-label">ID Global</span>
                <dx:ASPxTextBox ID="txtBuscarIdGlobal" runat="server"
                    ClientInstanceName="txtBuscarIdGlobal" Width="200px" NullText="ID Global...">
                    <ClientSideEvents KeyDown="onBusquedaKeyDown" />
                </dx:ASPxTextBox>
            </div>

            <!-- Campo dinámico: Fecha Solicitud -->
            <div id="divFechaSolicitud" class="search-field" style="display: none;">
                <span class="search-field-label">Fecha Solicitud</span>
                <dx:ASPxDateEdit ID="deBuscarFechaSolicitud" runat="server"
                    ClientInstanceName="deBuscarFechaSolicitud" Width="150px"
                    DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy"
                    NullText="dd/MM/yyyy o yyyy-MM-dd">
                    <ClientSideEvents KeyDown="onBusquedaKeyDown"
                                      LostFocus="function(s,e){ onFechaLostFocus(s, 'sol'); }" />
                </dx:ASPxDateEdit>
            </div>

            <!-- Campos dinámicos: Rango de fechas -->
            <div id="divFechaInicio" class="search-field" style="display: none;">
                <span class="search-field-label">Fecha Inicio</span>
                <dx:ASPxDateEdit ID="deBuscarFechaInicio" runat="server"
                    ClientInstanceName="deBuscarFechaInicio" Width="150px"
                    DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy"
                    NullText="dd/MM/yyyy o yyyy-MM-dd">
                    <ClientSideEvents KeyDown="onBusquedaKeyDown"
                                      LostFocus="function(s,e){ onFechaLostFocus(s, 'ini'); }" />
                </dx:ASPxDateEdit>
            </div>

            <div id="divFechaFin" class="search-field" style="display: none;">
                <span class="search-field-label">Fecha Fin</span>
                <dx:ASPxDateEdit ID="deBuscarFechaFin" runat="server"
                    ClientInstanceName="deBuscarFechaFin" Width="150px"
                    DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy"
                    NullText="dd/MM/yyyy o yyyy-MM-dd">
                    <ClientSideEvents KeyDown="onBusquedaKeyDown"
                                      LostFocus="function(s,e){ onFechaLostFocus(s, 'fin'); }" />
                </dx:ASPxDateEdit>
            </div>

            <!-- Botones -->
            <div class="search-buttons">
                <dx:ASPxButton runat="server" ID="btnBuscar" Text="Buscar" AutoPostBack="false" Width="90px"
                    BackColor="#353943" ForeColor="White" Font-Bold="true">
                    <ClientSideEvents Click="EjecutarBusqueda" />
                </dx:ASPxButton>
                <dx:ASPxButton runat="server" ID="btnLimpiar" Text="Limpiar" AutoPostBack="false" Width="90px"
                    BackColor="Teal" ForeColor="White" Font-Bold="true">
                    <ClientSideEvents Click="LimpiarBusqueda" />
                </dx:ASPxButton>
            </div>

        </div>
        <!-- fin divSearchContent -->

    </div>

    <!-- ── Grid ──────────────────────────────────────────────────────────────── -->
    <div class="grid-scroll-container">
        <dx:ASPxGridView ID="gridSolicitudesAptosMedicos" runat="server"
            KeyFieldName="id_solicitud"
            ForeColor="Black"
            ClientInstanceName="gridSolicitudesAptosMedicos"
            OnDataBinding="gridSolicitudesAptosMedicos_DataBinding"
            OnCustomButtonCallback="gridSolicitudesAptosMedicos_CustomButtonCallback"
            OnCustomCallback="gridSolicitudesAptosMedicos_CustomCallback"
            EnableRowsCache="false"
            OnDataBound="gridSolicitudesAptosMedicos_DataBound"
            Width="2600px">
            <ClientSideEvents
                EndCallback="OnGridSolicitudesEndCallback"
                CustomButtonClick="function(s, e) { OnCustomButtonClickSolicitud(s, e); }" />
            <Styles>
                <Header BackColor="#353943" ForeColor="White" Font-Bold="true"></Header>
            </Styles>
            <Settings ShowColumnHeaders="true" />
            <SettingsResizing ColumnResizeMode="Control" />
            <Columns>
                <dx:GridViewDataTextColumn FieldName="Id_Solicitud" Caption="ID" Visible="true" ReadOnly="true" Width="80" CellStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Nombre_Solicitante" Caption="Solicitante" HeaderStyle-HorizontalAlign="Center" Width="150" />
                <dx:GridViewDataTextColumn FieldName="Proyecto" Caption="Proyecto" Width="150" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Id_Global" Caption="ID Global" Width="100" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Nombre" Caption="Nombre" Width="100" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Apellidos" Caption="Apellidos" Width="100" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="No_Identidad" Caption="No Identidad" Width="120" CellStyle-HorizontalAlign="Center" />
                <dx:GridViewDataDateColumn FieldName="Fecha_Nacimiento" Caption="Fecha nacimiento" Width="140" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center">
                    <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="Fecha_Solicitud" Caption="Fecha solicitud" Width="140" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center">
                    <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataTextColumn FieldName="nombre_contratista" Caption="Nombre contratista" Width="150" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Puesto" Caption="Puesto" Width="140" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Medico_Asigna_Examen" Caption="Medico" Width="130" />
                <dx:GridViewDataTextColumn FieldName="Medico_Apto" Caption="Medico Apto" Width="110" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Nombre_Tipo_Examen" Caption="Tipo Examen" Width="120" CellStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Apto" Caption="Apto" Width="60" CellStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataDateColumn FieldName="Fecha_Diagnostico" Caption="Fecha diagnostico" Width="140" ReadOnly="true" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center">
                    <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="Fecha_Vigencia" Caption="Fecha vigencia" Width="140" ReadOnly="true" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center">
                    <PropertiesDateEdit DisplayFormatString="yyyy/MM/dd" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataTextColumn FieldName="Motivo_Cancelacion" Caption="Motivo cancelacion" Width="160" HeaderStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Edad" Width="60" Caption="Edad" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Sexo" Width="60" Caption="Sexo" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" />
            </Columns>
            <SettingsPager PageSize="25" />
            <StylesPager CurrentPageNumber-BackColor="#353943" PageSizeItem-HoverStyle-BackColor="Teal" />
        </dx:ASPxGridView>
    </div>
</asp:Content>