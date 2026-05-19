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
            margin-bottom: 8px;
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

        /* ── Panel JSON ─────────────────────────────────────────── */
        .json-panel {
            display: block;
            margin-bottom: 10px;
        }

        .json-panel-header {
            display: flex;
            align-items: center;
            background-color: #353943;
            color: white;
            padding: 6px 14px;
            border-radius: 4px 4px 0 0;
            font-size: 12px;
            font-weight: bold;
        }

        .json-panel-body {
            background-color: #1e1e1e;
            color: #d4d4d4;
            font-family: 'Consolas', 'Courier New', monospace;
            font-size: 12px;
            line-height: 1.5;
            padding: 12px;
            white-space: pre;
            border: 1px solid #353943;
            border-top: none;
            border-radius: 0 0 4px 4px;
            overflow: auto;
            height: 500px;
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
            if (contenedor) contenedor.style.display = valor ? 'contents' : 'none';
            ocultarTodosLosCampos();
            switch (valor) {
                case 'ID_SOLICITUD': mostrarCampo('divIdSolicitud'); break;
                case 'ID_GLOBAL': mostrarCampo('divIdGlobal'); break;
                case 'FECHA_SOLICITUD': mostrarCampo('divFechaSolicitud'); break;
                case 'RANGO_FECHAS': mostrarCampo('divFechaInicio'); mostrarCampo('divFechaFin'); break;
            }
            console.log('onTipoBusquedaChanged');
        }

        // ── Panel JSON ────────────────────────────────────────────────────────────
        var JSON_PLACEHOLDER = ' ';

        function mostrarJsonPanel(json) {
            var pre = document.getElementById('preJsonBody');
            if (!pre) return;
            try {
                console.log('Mostrando respuesta del API en el panel');
                pre.textContent = JSON.stringify(JSON.parse(json), null, 2);
            }
            catch (x) { pre.textContent = json; }
        }

        // ── Almacén de valores crudos
        var _rawFecha = { sol: '', ini: '', fin: '' };

        //// ── Formateo y parseo de fechas ───────────────────────────────────────────
        function formatearFechaLocal(date) {
            var y = date.getFullYear(), m = date.getMonth() + 1, d = date.getDate();
            return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d);
        }

        function parsearFecha(control, rawKey) {
            var dateObj = control.GetValue();
            if (dateObj instanceof Date && !isNaN(dateObj)) return formatearFechaLocal(dateObj);

            var raw = _rawFecha[rawKey] || '';
            if (!raw) return '';

            var d, m, y;
            var isoMatch = raw.match(/^(\d{4})[-\/](\d{1,2})[-\/](\d{1,2})$/);
            if (isoMatch) { y = parseInt(isoMatch[1], 10); m = parseInt(isoMatch[2], 10); d = parseInt(isoMatch[3], 10); }
            var dmyMatch = raw.match(/^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/);
            if (!isoMatch && dmyMatch) { d = parseInt(dmyMatch[1], 10); m = parseInt(dmyMatch[2], 10); y = parseInt(dmyMatch[3], 10); }
            if (!d || !m || !y) return raw;

            var fecha = new Date(y, m - 1, d);
            if (fecha.getFullYear() !== y || fecha.getMonth() !== m - 1 || fecha.getDate() !== d) return raw;
            return formatearFechaLocal(fecha);
        }

        // ── Búsqueda ──────────────────────────────────────────────────────────────
        function EjecutarBusqueda() {
            var tipo = cbTipoBusqueda.GetValue();
            var valorTexto = '', fechaSol = '', fechaInicio = '', fechaFin = '';

            switch (tipo) {
                case 'ID_SOLICITUD': valorTexto = txtBuscarIdSolicitud.GetValue() || ''; break;
                case 'ID_GLOBAL': valorTexto = txtBuscarIdGlobal.GetValue() || ''; break;
                case 'FECHA_SOLICITUD': fechaSol = parsearFecha(deBuscarFechaSolicitud, 'sol'); break;
                case 'RANGO_FECHAS': 
                    fechaInicio = parsearFecha(deBuscarFechaInicio, 'ini');
                    fechaFin = parsearFecha(deBuscarFechaFin, 'fin');
                    break;
            }
            console.log('SEARCH| ' + tipo + '| ' + valorTexto + '| ' + fechaSol + '| ' + fechaInicio + '| ' + fechaFin)
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
            _rawFecha.sol = ''; _rawFecha.ini = ''; _rawFecha.fin = '';
            var pre = document.getElementById('preJsonBody');
            if (pre) pre.textContent = JSON_PLACEHOLDER;
        }

        // ── Callback del grid ─────────────────────────────────────────────────────
        function OnGridSolicitudesEndCallback(s, e) {
            var estado = s.cpEstadoBusqueda;
            var json = s.cpJsonRespuesta || '';

            if (estado === 'error' || estado === 'ok' || estado === 'sin_resultados') {
                mostrarJsonPanel(json);
            }

            delete s.cpEstadoBusqueda; delete s.cpTotalRegistros;
            delete s.cpJsonRespuesta; delete s.cpMensajeError;
        }

        // Asegurar que los campos de búsqueda estén ocultos al cargar la página y configurar el seguimiento de fechas
        window.addEventListener('load', function () {
            var contenedor = document.getElementById('divSearchContent');
            if (contenedor) contenedor.style.display = 'none';
            ocultarTodosLosCampos();
        });

    </script>

    <div style="padding-top: 8px">
        <dx:ASPxLabel runat="server" ID="ASPxLabel7" Text="Solicitudes" Font-Bold="true" Font-Size="X-Large" />
    </div>
    <br />

    <!-- ── Panel de búsqueda ─────────────────────────────────────────────────── -->
    <div class="search-panel">
        <div class="search-field">
            <span class="search-field-label">Tipo Búsqueda</span>
            <dx:ASPxComboBox ID="cbTipoBusqueda" runat="server" ClientInstanceName="cbTipoBusqueda" Width="160px">
                <Items>
                    <dx:ListEditItem Text="Seleccione..." Value="" />
                    <dx:ListEditItem Text="ID Solicitud" Value="ID_SOLICITUD" />
                    <dx:ListEditItem Text="ID Global" Value="ID_GLOBAL" />
                    <dx:ListEditItem Text="Fecha Solicitud" Value="FECHA_SOLICITUD" />
                    <dx:ListEditItem Text="Rango de fechas" Value="RANGO_FECHAS" />
                </Items>
                <ClientSideEvents SelectedIndexChanged="onTipoBusquedaChanged" />
            </dx:ASPxComboBox>
        </div>

        <div id="divSearchContent" style="display: none;">
            <div id="divIdSolicitud" class="search-field" style="display: none;">
                <span class="search-field-label">ID Solicitud</span>
                <dx:ASPxTextBox ID="txtBuscarIdSolicitud" runat="server"
                    ClientInstanceName="txtBuscarIdSolicitud" Width="160px" NullText="ID Solicitud...">
                </dx:ASPxTextBox>
            </div>

            <div id="divIdGlobal" class="search-field" style="display: none;">
                <span class="search-field-label">ID Global</span>
                <dx:ASPxTextBox ID="txtBuscarIdGlobal" runat="server"
                    ClientInstanceName="txtBuscarIdGlobal" Width="200px" NullText="ID Global...">
                </dx:ASPxTextBox>
            </div>

            <div id="divFechaSolicitud" class="search-field" style="display: none;">
                <span class="search-field-label">Fecha Solicitud</span>
                <dx:ASPxDateEdit ID="deBuscarFechaSolicitud" runat="server"
                    ClientInstanceName="deBuscarFechaSolicitud" Width="150px"
                    DisplayFormatString="yyyy-MM-dd" EditFormatString="yyyy-MM-dd"
                    NullText="yyyy-MM-dd">
                </dx:ASPxDateEdit>
            </div>

            <div id="divFechaInicio" class="search-field" style="display: none;">
                <span class="search-field-label">Fecha Inicio</span>
                <dx:ASPxDateEdit ID="deBuscarFechaInicio" runat="server"
                    ClientInstanceName="deBuscarFechaInicio" Width="150px"
                    DisplayFormatString="yyyy-MM-dd" EditFormatString="yyyy-MM-dd"
                    NullText="yyyy-MM-dd">
                </dx:ASPxDateEdit>
            </div>

            <div id="divFechaFin" class="search-field" style="display: none;">
                <span class="search-field-label">Fecha Fin</span>
                <dx:ASPxDateEdit ID="deBuscarFechaFin" runat="server"
                    ClientInstanceName="deBuscarFechaFin" Width="150px"
                    DisplayFormatString="yyyy-MM-dd" EditFormatString="yyyy-MM-dd"
                    NullText="yyyy-MM-dd">
                </dx:ASPxDateEdit>
            </div>

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
    </div>

    <!-- ── Panel JSON del API ─────────────────────────────────────────────────── -->
    <div id="divJsonPanel" class="json-panel">
        <pre id="preJsonBody" class="json-panel-body"></pre>
    </div>

    <!-- ── Grid ──────────────────────────────────────────────────────────────── -->
    <div class="grid-scroll-container">
        <dx:ASPxGridView ID="gridSolicitudesAptosMedicos" runat="server"
            KeyFieldName="id_solicitud" ForeColor="Black"
            ClientInstanceName="gridSolicitudesAptosMedicos"
            OnDataBinding="gridSolicitudesAptosMedicos_DataBinding"
            OnCustomCallback="gridSolicitudesAptosMedicos_CustomCallback"
            EnableRowsCache="false" OnDataBound="gridSolicitudesAptosMedicos_DataBound"
            Width="2600px">
            <ClientSideEvents EndCallback="OnGridSolicitudesEndCallback" />
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
                <dx:GridViewDataTextColumn FieldName="Edad" Width="60" Caption="Edad" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" />
                <dx:GridViewDataTextColumn FieldName="Sexo" Width="60" Caption="Sexo" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" />
            </Columns>
            <SettingsPager PageSize="25" />
            <StylesPager CurrentPageNumber-BackColor="#353943" PageSizeItem-HoverStyle-BackColor="Teal" />
        </dx:ASPxGridView>
    </div>
</asp:Content>
