/*-------------------------------------------------------------------------- 
administradorGeneral.js
Funciones js par la página AdministradorGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var estadoData;
var estadoId;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    comprobarLogin();
    // de smart admin
    pageSetUp();
    getVersionFooter();
    //
    $('#btnBuscar').click(sendConsultarEstados());
    $('#frmBuscar').submit(function () {
        return false
    });
    //
    initTablaEstados();
    // consultamos las unidades de la base de datos
    $.ajax({
        type: "POST",
        url: "EstadoApi.aspx/GetEstados",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            // hay que mostrarlo en la zona de datos
            loadTablaEstados(data.d);
        },
        error: errorAjax
    });
}

function initTablaEstados() {
    tablaCarro = $('#dt_estados').dataTable({
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_estados'), breakpointDefinition);
            }
        },
        rowCallback: function (nRow) {
            responsiveHelper_dt_basic.createExpandIcon(nRow);
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
            }
        },
        data: estadoData,
        columns: [
            {
                data: "Codigo"
            }, {
                data: "Nombre"
            }, {
                data: "Descripcion"
            }
        ]
    });
}

function datosOKEstados() {
    // verificar y obtener el certificado?
    return true;
}

function loadTablaEstados(data) {
    var dt = $('#dt_estados').dataTable();
    dt.fnClearTable();
    dt.fnAddData(data);
    dt.fnDraw();
    $("#tbEstados").show();
}

function sendConsultarEstados() {
    var mf = function () {
        if (!datosOKEstados()) {
            return;
        }
        // 
        var adm = JSON.parse(getCookie("admin"));
        // obtener el n.serie del certificado para la firma.
        var certSn = adm.Certsn;
        // enviar la consulta por la red (AJAX)
        var data = {
            "certSn": certSn
        };
        $('#btnBuscar').hide();
        $('#ldgBuscar').show();
        $.ajax({
            type: "POST",
            url: "FaceApi.aspx/GetEstados",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                $('#btnBuscar').show();
                $('#ldgBuscar').hide();
                // debemos eliminat la propiedad _type
                var estados = data.d;
                if (estados.length === 0) {
                    mostrarMensajeSmart('La consulta no ha devuelto datos. ¿Es correcto el certificado de este usuario?');
                    return;
                }
                for (var i = 0; i < estados.length; i++) {
                    var estado = estados[i];
                    delete estado.__type;
                }
                data = { estados: estados };
                loadTablaEstados(estados);
                // actualización de la base de datos
                $.ajax({
                    type: "POST",
                    url: "EstadoApi.aspx/SetEstados",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify(data),
                    success: function (data, status) {
                        // actualización de la base de datos
                        $('#btnBuscar').show();
                        $('#ldgBuscar').hide();
                        mostrarMensajeSmart('Los estados se han grabado correctamente en la base de datos');
                    },
                    error: function (xhr, textStatus, errorThrwon) {
                        $('#btnBuscar').show();
                        $('#ldgBuscar').hide();
                        var m = xhr.responseText;
                        if (!m) m = "Error general posiblemente falla la conexión";
                        mostrarMensaje(m);
                    }
                });
            },
            error: function (xhr, textStatus, errorThrwon) {
                $('#btnBuscar').show();
                $('#ldgBuscar').hide();
                var m = xhr.responseText;
                if (!m) m = "Error general posiblemente falla la conexión";
                mostrarMensaje(m);
            }
        });
    };
    return mf;
}




