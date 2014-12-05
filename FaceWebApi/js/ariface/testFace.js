//
// para tablas
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var certData;
var estData;
var unidadData;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};

function initForm() {
    // de smart admin
    pageSetUp();
    //
    $('#btnCertificados').click(sendConsultarCertificados());
    $('#frmCertificados').submit(function () {
        return false
    });
    // asignación de eventos al clic
    $("#btnConsultarEstados").click(sendConsultarEstados());
    $("#frmConsultarEstados").submit(function () {
        return false;
    });
    $("#btnConsultarUnidades").click(sendConsultarUnidades());
    $("#frmConsultarUnidades").submit(function () {
        return false;
    });
    //
    initTablaCertificados();
    initTablaEstados();
    initTablaUnidades();
}

function datosOK() {
    //TODO: Incluir en la validación si el certificado figura en el almacén de certificados.
    $('#frmConsultarEstados').validate({
                                           rules: {
            txtCertSn1: { required: true },
        },
                                           // Messages for form validation
                                           messages: {
            txtCertSn1: {
                                                   required: 'Introduzca el número de serie del certificado con el que desea firmar'
                                               }
        },
                                           // Do not change code below
                                           errorPlacement: function (error, element) {
                                               error.insertAfter(element.parent());
                                           }
                                       });
    return $('#frmConsultarEstados').valid();
}

function datosOKUnidades() {
    //TODO: Incluir en la validación si el certificado figura en el almacén de certificados.
    $('#frmConsultarUnidades').validate({
        rules: {
            txtCertSn1: { required: true },
        },
        // Messages for form validation
        messages: {
            txtCertSn2: {
                required: 'Introduzca el número de serie del certificado con el que desea firmar'
            }
        },
        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });
    return $('#frmConsultarUnidades').valid();
}

function sendConsultarEstados() {
    var mf = function () {
        if (!datosOK()) {
            return;
        }
        // obtener el n.serie del certificado para la firma.
        var certSn = $('#txtCertSn1').val();
        // enviar la consulta por la red (AJAX)
        var data = {
            "certSn": certSn
        };
        $('#btnConsultarEstados').hide();
        $('#ldgConsultarEstados').show();
        $.ajax({
                   type: "POST",
                   url: "FaceApi.aspx/GetEstados",
                   dataType: "json",
                   contentType: "application/json",
                   data: JSON.stringify(data),
                   success: function (data, status) {
                       // debemos eliminat la propiedad _type
                       var estados = data.d;
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
                                      $('#btnConsultarEstados').show();
                                      $('#ldgConsultarEstados').hide();
                                      mostrarMensajeSmart('Los estados se han grabado correctamente en la base de datos');
                                  },
                                  error: errorAjax
                              });
                   },
                   error: errorAjax
               });
    };
    return mf;
}

function sendConsultarUnidades() {
    var mf = function () {
        if (!datosOKUnidades()) {
            return;
        }
        // obtener el n.serie del certificado para la firma.
        var certSn = $('#txtCertSn2').val();
        // enviar la consulta por la red (AJAX)
        var data = {
            "certSn": certSn
        };
        $('#btnConsultarUnidades').hide();
        $('#ldgConsultarUnidades').show();
        $.ajax({
            type: "POST",
            url: "FaceApi.aspx/GetUnidades",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // debemos eliminat la propiedad _type
                var unidades = data.d;
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
                    error: errorAjax
                });
            },
            error: errorAjax
        });
    };
    return mf;
}

function sendConsultarCertificados() {
    var mf = function () {
        $('#btnCertificados').hide();
        $('#CertAnimate').show();
        $.ajax({
                   type: "POST",
                   url: "CertificadoApi.aspx/GetCertificados",
                   dataType: "json",
                   contentType: "application/json",
                   success: function (data, status) {
                       // hay que mostrarlo en la zona de datos
                       loadTablaCertificados(data.d);
                       $('#btnCertificados').show();
                       $('#CertAnimate').hide();
                   },
                   error: errorAjax
               });
    };
    return mf;
}

var errorAjax = function (xhr, textStatus, errorThrwon) {
    var m = xhr.responseText;
    if (!m)
        m = "Error general posiblemente falla la conexión";
    mostrarMensajeSmart(m);
}

function loadTablaCertificados(data) {
    var dt = $('#dt_certificados').dataTable();
    dt.fnClearTable();
    dt.fnAddData(data);
    dt.fnDraw();
    $("#tbCertificado").show();
}

function loadTablaEstados(data) {
    var dt = $('#dt_estados').dataTable();
    dt.fnClearTable();
    dt.fnAddData(data);
    dt.fnDraw();
    $("#tbEstados").show();
}

function loadTablaUnidades(data) {
    var dt = $('#dt_unidades').dataTable();
    dt.fnClearTable();
    dt.fnAddData(data);
    dt.fnDraw();
    $("#tbUnidades").show();
}

function initTablaCertificados() {
    tablaCarro = $('#dt_certificados').dataTable({
    autoWidth: true,
    preDrawCallback: function () {
        // Initialize the responsive datatables helper once.
        if (!responsiveHelper_dt_basic) {
            responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_certificados'), breakpointDefinition);
        }
        },
        rowCallback : function(nRow) {
            responsiveHelper_dt_basic.createExpandIcon(nRow);
        },
        drawCallback : function(oSettings) {
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
        data: certData,
        columns: [
            {
                data: "SerialNumber"
            }, {
                data: "FriendlyName"
            }, {
                data: "ExpirationDateString"
            }
        ]
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
        data: estData,
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