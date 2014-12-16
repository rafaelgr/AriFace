/*-------------------------------------------------------------------------- 
administradorGeneral.js
Funciones js par la página AdministradorGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var dataFacturas;
var facturaId;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    // de smart admin
    pageSetUp();

    // numeral en español
    numeral.language('es', {
        delimiters: {
            thousands: '.',
            decimal: ','
        }
    });
    numeral.language('es');

    //
    $('#btnBuscar').click(buscarFactura());
    $('#btnAlta').click(crearFactura());
    $('#frmBuscar').submit(function () {
        return false
    });
    $('#chkEnviadas').change(function () {
        var fn;
        if (this.checked) {
            fn = getFacturas();
        } else {
            fn = getFacturasNoEnviadas();
        }
        fn();
    });
    //
    initTablaFacturas();
    // comprobamos parámetros
    facturaId = gup('facturaId');
    if (facturaId !== '') {
        // cargar la tabla con un único valor que es el que corresponde.
        var data = {
            id: facturaId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "FacturaApi.aspx/GetFacturaById",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                var data2 = [data.d];
                loadTablaFacturas(data2);
            },
            error: errorAjax
        });
    } else {
        // mostramos sólo las facturas no enviadas.
        var fn = getFacturasNoEnviadas();
        fn();
    }
}

function initTablaFacturas() {
    tablaCarro = $('#dt_factura').dataTable({
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_factura'), breakpointDefinition);
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
        data: dataFacturas,
        columns: [{ data: "ClienteNombre" },
            { data: "Sistema" },
            { data: "Departamento" },
            { data: "Serie" },
            { data: "NumFactura" },
            {
                data: "StrFecha",
                render: function (data, type, row) {
                    var html = "<div style='text-align:center'>" + moment(data, 'YYYYMMDD').format('DD/MM/YYYY') + "</div>";
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
            { data: "Estado" },
            { data: "RegistroFace" },
            { data: "MotivoFace" },
            {
            data: "FacturaId",
            render: function (data, type, row) {
                var bt1 = "<button class='btn btn-circle btn-success' onclick='verPdf(" + data + ");' title='Ver / descargar PDF'> <i class='fa fa-file-pdf-o fa-fw'></i> </button>";
                var bt2 = "<button class='btn btn-circle btn-warning' onclick='eliminarDeEnvio(" + data + ");' title='Eliminar del envío'> <i class='fa fa-remove fa-fw'></i> </button>";
                if (row.Estado == 0) {
                    bt2 = "<button class='btn btn-circle btn-warning' onclick='agregarAlEnvio(" + data + ");' title='Agregar al envío'> <i class='fa fa-undo fa-fw'></i> </button>";
                } 
                var html = "<div class='pull-right'>" + bt1 + " " + bt2 + "</div>";
                return html;
            }
        }]
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

function loadTablaFacturas(data) {
    var dt = $('#dt_factura').dataTable();
    if (data !== null && data.length === 0) {
        mostrarMensajeSmart('No se han encontrado registros');
    } else {
        dt.fnClearTable();
        dt.fnAddData(data);
        dt.fnDraw();
        $("#tbFactura").show();
    }
}

function buscarFactura() {
    var mf = function () {
        if (!datosOK()) {
            return;
        }
        // obtener el n.serie del certificado para la firma.
        var aBuscar = $('#txtBuscar').val();
        // enviar la consulta por la red (AJAX)
        var data = {
            "aBuscar": aBuscar
        };
        $.ajax({
            type: "POST",
            url: "ClienteApi.aspx/BuscarCliente",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                loadTablaClientes(data.d);
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
                loadTablaFacturas(data.d);
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
                loadTablaFacturas(data.d);
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
                            loadTablaFacturas(data.d);
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
}
