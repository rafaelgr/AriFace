/*-------------------------------------------------------------------------- 
administradorGeneral.js
Funciones js par la página AdministradorGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var unidadData;
var unidadId;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    // de smart admin
    pageSetUp();
    //
    $('#btnBuscar').click(sendConsultarUnidades());
    $('#frmBuscar').submit(function () {
        return false
    });
    //
    initTablaUnidades();
    // consultamos las unidades de la base de datos
    $.ajax({
        type: "POST",
        url: "UnidadApi.aspx/GetUnidades",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            // hay que mostrarlo en la zona de datos
            loadTablaUnidades(data.d);
        },
        error: errorAjax
    });
}
function initTablaUnidades() {
    tablaCarro = $('#dt_unidades').dataTable({
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_unidades'), breakpointDefinition);
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
        data: unidadData,
        columns: [
            {
                data: "OrganoGestorCodigo"
            }, {
                data: "OrganoGestorNombre"
            }, {
                data: "UnidadTramitadoraCodigo"
            }, {
                data: "UnidadTramitadoraNombre"
            }, {
                data: "OficinaContableCodigo"
            }, {
                data: "OficinaContableNombre"
            }
        ]
    });
}

function datosOKUnidades() {
    // verificar y obtener el certificado?
    return true;
}

function loadTablaUnidades(data) {
    var dt = $('#dt_unidades').dataTable();
    dt.fnClearTable();
    dt.fnAddData(data);
    dt.fnDraw();
    $("#tbUnidades").show();
}

function sendConsultarUnidades() {
    var mf = function () {
        if (!datosOKUnidades()) {
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
            url: "FaceApi.aspx/GetUnidades",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                $('#btnBuscar').show();
                $('#ldgBuscar').hide();
                // debemos eliminat la propiedad _type
                var unidades = data.d;
                if (unidades.length === 0) {
                    mostrarMensajeSmart('La consulta no ha devuelto datos. ¿Es correcto el certificado de este usuario?');
                    return;
                }
                for (var i = 0; i < unidades.length; i++) {
                    var unidad = unidades[i];
                    delete unidad.__type;
                }
                data = { unidades: unidades };
                loadTablaUnidades(unidades);
                // actualización de la base de datos
                $.ajax({
                    type: "POST",
                    url: "UnidadApi.aspx/SetUnidades",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify(data),
                    success: function (data, status) {
                        // actualización de la base de datos
                        $('#btnConsultarUnidades').show();
                        $('#ldgConsultarUnidades').hide();
                        mostrarMensajeSmart('Las unidades se han grabado correctamente en la base de datos');
                    },
                    error: function (xhr, textStatus, errorThrwon) {
                        $('#btnConsultarUnidades').show();
                        $('#ldgConsultarUnidades').hide();
                        var m = xhr.responseText;
                        if (!m) m = "Error general posiblemente falla la conexión";
                        mostrarMensaje(m);
                    }
                });
            },
            error: function (xhr, textStatus, errorThrwon) {
                $('#btnConsultarUnidades').show();
                $('#ldgConsultarUnidades').hide();
                var m = xhr.responseText;
                if (!m) m = "Error general posiblemente falla la conexión";
                mostrarMensaje(m);
            }
        });
    };
    return mf;
}




