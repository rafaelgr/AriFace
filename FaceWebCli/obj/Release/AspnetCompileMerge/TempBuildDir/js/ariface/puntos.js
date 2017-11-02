/*-------------------------------------------------------------------------- 
administradorGeneral.js
Funciones js par la página AdministradorGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var dataPuntos;
var facturaId;
var usuario;
var clientePuntos;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};

// variables y objetos usados
var miniUnidad = function (codigo, nombre) {
    this.Codigo = codigo;
    this.Nombre = nombre;
}

function initForm(proveedor) {
    usuario = comprobarLogin();
    comprobarClientePuntos(usuario.CodClienAriges);
    // de smart admin
    pageSetUp();
    // 
    vm = new facData();
    ko.applyBindings(vm);

    // numeral en español
    numeral.language('es', {
        delimiters: {
            thousands: '.',
            decimal: ','
        }
    });
    numeral.language('es');

    //
    //
    initTablaPuntos();
    // cargamos los datos de cliente con sus puntos
    var data = {
        codclien: usuario.CodClienAriges
    };
    $.ajax({
        type: "POST",
        url: "PuntosApi.aspx/GetClientePuntos",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            // Regresa el mensaje
            if (!data.d) {
                mostrarMensajeSmart('No se ha podido comprobar el cliente en Ariges');
            }
            var a = data.d;
            vm.SumTotal(numeral(a.Puntos).format('#,###,##0.00'));

        },
        error: function (xhr, textStatus, errorThrwon) {
            var m = xhr.responseText;
            if (!m) m = "Error general posiblemente falla la conexión";
            mostrarMensajeSmart(m);
        }
    });
    //
    var data = {
        codclien: usuario.CodClienAriges
    };
    $.ajax({
        type: "POST",
        url: "PuntosApi.aspx/GetPuntosCliente",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data, status) {
            var puntos = data.d;
            // esta es la zona de carga de valores
            loadTablaPuntos(puntos);
        },
        error: function (xhr, textStatus, errorThrwon) {
            var m = xhr.responseText;
            if (!m) m = "Error general posiblemente falla la conexión";
            mostrarMensajeSmart(m);
        }
    });
}

function facData() {
    var self = this;
    self.SumTotal = ko.observable();
}


function initTablaPuntos() {
    tablaPuntos = $('#dt_puntos').dataTable({
        bSort: false,
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_puntos'), breakpointDefinition);
            }
        },
        rowCallback: function (nRow, data) {
            responsiveHelper_dt_basic.createExpandIcon(nRow);
            if (data.Nueva) {
                $(nRow).css({ "background-color": "beige" });
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
        data: dataPuntos,
        columns: [
            {
                data: "FechaAlb",
                render: function (data, type, row) {
                    var html = "<div style='text-align:center'>" + moment(data, 'YYYYMMDD').format('DD/MM/YYYY') + "</div>";
                    return html;
                }
            },
            {
                data: "Concepto"
            },
            {
                data: "NumAlbar"
            },
            {
                data: "Observaciones"
            },
            {
                data: "Puntos",
                render: function (data, type, row) {
                    var html = "<div style='text-align:right'>" + numeral(data).format('#,###,##0.00') + " </div>";
                    return html;
                }
            }
        ]
    });
}

function datosOK() {
    //TODO: Incluir en la validación si el certificado figura en el almacén de certificados.
    $('#frmBuscar').validate({
        rules: {
            txtBuscar: { required: true },
        },
        // Messages for form validation
        messages: {
            txtBuscar: {
                required: 'Introduzca el texto a buscar'
            }
        },
        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });
    return $('#frmBuscar').valid();
}

function loadTablaPuntos(data) {
    var dt = $('#dt_puntos').dataTable();
    if (data !== null && data.length === 0) {
        mostrarMensajeSmart('No se han encontrado registros');
    } else {
        dt.fnClearTable();
        dt.fnAddData(data);
        dt.fnDraw();
        $("#tbPuntos").show();
    }
}

function buscarFacturas() {
    var mf = function () {
        // cargamos las facturas que puede ver ese usuario
        var data = {
            usuarioId: usuario.UsuarioId,
            a: Number(vm.Ano().Codigo),
            q: Number(vm.Trimestre().Codigo),
            m: Number(vm.Mes().Codigo),
            esCliente: true
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "FacturaApi.aspx/GetFacturasUsuarioPeriodo",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                var pf = data.d;
                vm.SumBases(numeral(pf.SumBase).format('#,###,##0.00'));
                vm.SumCuotas(numeral(pf.SumCuota).format('#,###,##0.00'));
                vm.SumTotal(numeral(pf.SumTotal).format('#,###,##0.00'));
                vm.SumRetencion(numeral(pf.SumRetencion).format('#,###,##0.00'));
                vm.SumAportacion(numeral(pf.SumAportacion).format('#,###,##0.00'));
                // esta es la zona de carga de valores
                loadTablaPuntos(pf.Facturas);
            },
            error: errorAjax
        });
    };
    return mf;
}

function getFacturas() {
    var mf = function () {
        // obtener el n.serie del certificado para la firma.
        $.ajax({
            type: "POST",
            url: "FacturaApi.aspx/GetFacturas",
            dataType: "json",
            contentType: "application/json",
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                loadTablaPuntos(data.d);
            },
            error: errorAjax
        });
    };
    return mf;
}

function getFacturasNoEnviadas() {
    var mf = function () {
        $.ajax({
            type: "POST",
            url: "FacturaApi.aspx/GetFacturasNoEnviadas",
            dataType: "json",
            contentType: "application/json",
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                loadTablaPuntos(data.d);
            },
            error: errorAjax
        });
    };
    return mf;
}

function chkEnviadas() {
    var mf = function () {
        if ($('#chkEnviadas').attr('checked')) {
            getFacturas();
        }
    }
    return mf;
}

function crearFactura() {
    var mf = function () {
        var url = "FacturaDetalle.html?FacturaId=0";
        window.open(url, '_self');
    };
    return mf;
}

function deleteFactura(id) {
    // mensaje de confirmación
    var mens = "¿Realmente desea borrar este registro?";
    $.SmartMessageBox({
        title: "<i class='fa fa-info'></i> Mensaje",
        content: mens,
        buttons: '[Aceptar][Cancelar]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Aceptar") {
            var data = {
                id: id
            };
            $.ajax({
                type: "POST",
                url: "FacturaApi.aspx/DeleteFactura",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (data, status) {
                    var fn = getFacturasNoEnviadas();
                    fn();
                },
                error: errorAjax
            });
        }
        if (ButtonPressed === "Cancelar") {
            // no hacemos nada (no quiere borrar)
        }
    });
}

function editFactura(id) {
    // hay que abrir la página de detalle de administrador
    // pasando en la url ese ID
    var url = "FacturaDetalle.html?FacturaId=" + id;
    window.open(url, '_self');
}

function eliminarDeEnvio(id) {
    // mensaje de confirmación
    var mens = "¿Realmente desea eliminar la factura del envio?";
    $.SmartMessageBox({
        title: "<i class='fa fa-info'></i> Mensaje",
        content: mens,
        buttons: '[Aceptar][Cancelar]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Aceptar") {
            var data = {
                facturaId: id
            };
            $.ajax({
                type: "POST",
                url: "EnvioApi.aspx/EliminarFacturaDeEnvio",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (data, status) {
                    $.ajax({
                        type: "POST",
                        url: "FacturaApi.aspx/GetFacturas",
                        dataType: "json",
                        contentType: "application/json",
                        success: function (data, status) {
                            // hay que mostrarlo en la zona de datos
                            loadTablaPuntos(data.d);
                        },
                        error: errorAjax
                    });
                },
                error: errorAjax
            });
        }
        if (ButtonPressed === "Cancelar") {
            // no hacemos nada (no quiere borrar)
        }
    });
}

function agregarAlEnvio(id) {
    // mensaje de confirmación
    var mens = "¿Quiere volver a incorporar esta factura para futuros envíos?";
    $.SmartMessageBox({
        title: "<i class='fa fa-info'></i> Mensaje",
        content: mens,
        buttons: '[Aceptar][Cancelar]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Aceptar") {
            var data = {
                facturaId: id
            };
            $.ajax({
                type: "POST",
                url: "EnvioApi.aspx/RecuperarFacturaDeEnvio",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (data, status) {
                    $.ajax({
                        type: "POST",
                        url: "FacturaApi.aspx/GetFacturas",
                        dataType: "json",
                        contentType: "application/json",
                        success: function (data, status) {
                            // hay que mostrarlo en la zona de datos
                            loadTablaPuntos(data.d);
                        },
                        error: errorAjax
                    });
                },
                error: errorAjax
            });
        }
        if (ButtonPressed === "Cancelar") {
            // no hacemos nada (no quiere borrar)
        }
    });

}

function verPdf(id) {
    var user = JSON.parse(getCookie("usu"));
    var data = {
        facturaId: id,
        usuarioId: user.UsuarioId
    };
    $.ajax({
        type: "POST",
        url: "FacturaApi.aspx/VerPdf",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data, status) {
            var url = data.d;
            window.open(url, '_blank');
        },
        error: errorAjax
    });

}

function verXml(id) {
    var user = JSON.parse(getCookie("usu"));
    var data = {
        facturaId: id,
        usuarioId: user.UsuarioId
    };
    $.ajax({
        type: "POST",
        url: "FacturaApi.aspx/VerXml",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data, status) {
            var url = data.d;
            window.open(url, '_blank');
        },
        error: errorAjax
    });

}