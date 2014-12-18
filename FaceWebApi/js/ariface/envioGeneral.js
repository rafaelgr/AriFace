/*-------------------------------------------------------------------------- 
administradorGeneral.js
Funciones js par la página AdministradorGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var dataEnvios;
var clienteId;
var departamentoId;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    // de smart admin
    pageSetUp();
    //
    $('#btnEnvio').click(sendEnvios());
    $('#frmEnvio').submit(function () {
        return false
    });
    //
    initTablaEnvios();
    // comprobamos parámetros
    clienteId = gup('ClienteId');
    departamentoId = gup('DepartamentoId');
    if (clienteId !== "" && departamentoId !== "") {
        // cargar la tabla con un único valor que es el que corresponde.
        var data = {
            "clienteId": clienteId,
            "departamentoId": departamentoId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "EnvioApi.aspx/GetEnvio",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                var data2 = [data.d];
                loadTablaEnvios(data2);
            },
            error: errorAjax
        });
    } else {
        // mostramos sólo las facturas no enviadas.
        var fn = getEnvios();
        fn();
    }
    // numeral en español
    // load a language
    numeral.language('es', {
        delimiters: {
            thousands: '.',
            decimal: ','
        }
    });
    numeral.language('es');
}

function initTablaEnvios() {
    tablaEnvio = $('#dt_envio').dataTable({
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_envio'), breakpointDefinition);
            }
        },
        rowCallback: function (nRow, data) {
            responsiveHelper_dt_basic.createExpandIcon(nRow);
            if (data.EsFace) {
                $(nRow).css({ "background-color": "lightgreen" });
            }
        },
        drawCallback: function (oSettings) {
            responsiveHelper_dt_basic.respond();
        },
        language: {
            processing: "Procesando...",
            info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
            infoFiltered: "(filtrado de un total de _MAX_ registros)",
            infoPostFix: "",
            loadingRecords: "Cargando...",
            zeroRecords: "No se encontraron resultados",
            emptyTable: "Ningún dato disponible en esta tabla",
            paginate: {
                first: "Primero",
                previous: "Anterior",
                next: "Siguiente",
                last: "Último"
            },
            aria: {
                sortAscending: ": Activar para ordenar la columna de manera ascendente",
                sortDescending: ": Activar para ordenar la columna de manera descendente"
            },
            decimal: ',',
            thousands: '.'
        },
        data: dataEnvios,
        columns: [{ data: "Nif" },
            { data: "ClienteNombre" },
            { data: "DepartamentoNombre" },
            {
                data: "StrFechaInicial",
                render: function (data, type, row) {
                    var html = "<div style='text-align:center'>" + moment(data, 'YYYYMMDD').format('DD/MM/YYYY') + "</div>";
                    return html;
                }
            },
            {
                data: "StrFechaFinal",
                render: function (data, type, row) {
                    var html = "<div style='text-align:center'>" + moment(data, 'YYYYMMDD').format('DD/MM/YYYY') + "</div>";
                    return html;
                }
            },
            {
                data: "Bases",
                render: function (data, type, row) {
                    var html = "<div style='text-align:right'>" + numeral(data).format('#,###,##0.00') + " €</div>";
                    return html;
                }
            },
            {
                data: "Cuotas",
                render: function (data, type, row) {
                    var html = "<div style='text-align:right'>" + numeral(data).format('#,###,##0.00') + " €</div>";
                    return html;
                }
            },
            {
                data: "Retencion",
                render: function (data, type, row) {
                    var html = "<div style='text-align:right'>" + numeral(data).format('#,###,##0.00') + " €</div>";
                    return html;
                }
            },
            {
                data: "Total",
                render: function (data, type, row) {
                    var html = "<div style='text-align:right'>" + numeral(data).format('#,###,##0.00') + " €</div>";
                    return html;
                }
            },
            {
                data: "EsFace",
                render: function (data, type, row) {
                    var html = "";
                    if (row.EsFace) {
                        html = "SI"
                    }
                    else {
                        html = "NO";
                    }
                    return html;
                }
            },
            {
            data: "ClienteId",
            render: function (data, type, row) {
                var param = row.ClienteId + ", " + row.DepartamentoId;
                var bt1 = "<button class='btn btn-circle btn-primary' onclick='sendEnvio(" + param + ");' title='Enviar directamente'> <i class='fa fa-send fa-fw'></i> </button>";
                var bt2 = "<button class='btn btn-circle btn-success' onclick='editEnvio(" + param + ");' title='Editar registro'> <i class='fa fa-edit fa-fw'></i> </button>";
                var html = "<div class='pull-right'>" + bt1 + " " + bt2 + "</div>";
                return html;
            }
        }]
    });
}

function datosOK() {
    return true;
}

function loadTablaEnvios(data) {
    var dt = $('#dt_envio').dataTable();
    if (data !== null && data.length === 0) {
        mostrarMensajeSmart('No se han encontrado registros');
    } else {
        dt.fnClearTable();
        dt.fnAddData(data);
        dt.fnDraw();
        $("#tbEnvio").show();
    }
}

function getEnvios() {
    var mf = function () {
        // obtener el n.serie del certificado para la firma.
        $.ajax({
            type: "POST",
            url: "EnvioApi.aspx/GetEnvios",
            dataType: "json",
            contentType: "application/json",
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                loadTablaEnvios(data.d);
            },
            error: errorAjax
        });
    };
    return mf;
}

function editEnvio(clienteId, departamentoId) {
    // hay que abrir la página de detalle de administrador
    // pasando en la url ese ID
    var url = "EnvioDetalle.html?ClienteId=" + clienteId + "&DepartamentoId=" + departamentoId;
    window.open(url, '_self');
}

function sendEnvio(clienteId, departamentoId) {
    // 
    var adm = JSON.parse(getCookie("admin"));
    // obtener el n.serie del certificado para la firma.
    var certSn = adm.Certsn;
    var data = {
        clienteId: clienteId,
        departamentoId: departamentoId,
        certSn: certSn
    };
    $.ajax({
        type: "POST",
        url: "EnvioApi.aspx/SendEnvio",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            // hay que mostrarlo en la zona de datos
            mostrarMensajeSmart(data.d);
            // recargar
            var fn = getEnvios();
            fn();
        },
        error: errorAjax
    });
}

function sendEnvios() {
    var mf = function () {
        $('#btnEnvio').hide();
        $('#ldgEnvio').show()
        // 
        var adm = JSON.parse(getCookie("admin"));
        // obtener el n.serie del certificado para la firma.
        var certSn = adm.Certsn;
        var data = {
            certSn: certSn
        };
        $.ajax({
            type: "POST",
            url: "EnvioApi.aspx/SendEnvios",
            dataType: "json",
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                $('#btnEnvio').show();
                $('#ldgEnvio').hide();
                mostrarMensajeSmart(data.d);
                // recargar
                var fn = getEnvios();
                fn();
            },
            error: function (xhr, textStatus, errorThrwon) {
                $('#btnEnvio').show();
                $('#ldgEnvio').hide();
                var m = xhr.responseText;
                if (!m) m = "Error general posiblemente falla la conexión";
                mostrarMensaje(m);
            }

        });
    };
    return mf;
}


