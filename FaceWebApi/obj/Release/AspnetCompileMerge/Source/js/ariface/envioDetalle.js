/*-------------------------------------------------------------------------- 
envioDetalle.js
Funciones js par la página EnvioDetalle.html
---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var dataFacturas;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    comprobarLogin();
    // de smart admin
    pageSetUp();
    // 
    vm = new envData();
    ko.applyBindings(vm);
    // asignación de eventos al clic
    $("#btnSalir").click(salir());
    $("#btnSalir1").click(salir());
    $("#frmEnvio").submit(function () {
        return false;
    });

    initTablaFacturas();

    // numeral en español
    numeral.language('es', {
        delimiters: {
            thousands: '.',
            decimal: ','
        }
    });
    numeral.language('es');

    var clienteId = gup('ClienteId');
    var departamentoId = gup('DepartamentoId');
    var param = {
        clienteId: clienteId,
        departamentoId: departamentoId
    };
    $.ajax({
        type: "POST",
        url: "EnvioApi.aspx/GetEnvio",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(param),
        success: function (data, status) {
            // hay que mostrarlo en la zona de datos
            loadData(data.d);
        },
        error: errorAjax
    });

    // obtener las facturas asociadas.
    $.ajax({
        type: "POST",
        url: "EnvioApi.aspx/GetFacturasEnvio",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(param),
        success: function (data, status) {
            // hay que mostrarlo en la zona de datos
            loadTablaFacturas(data.d);
        },
        error: errorAjax
    });
}

function envData() {
    var self = this;
    self.Nif = ko.observable();
    self.Cliente = ko.observable();
    self.Departamento = ko.observable();
    self.FechaInicial = ko.observable();
    self.FechaFinal = ko.observable();
    self.Bases = ko.observable();
    self.Cuotas = ko.observable();
    self.Retencion = ko.observable();
    self.Total = ko.observable();
    self.Face = ko.observable();
}

function loadData(data) {
    vm.Nif(data.Nif);
    vm.Cliente(data.ClienteNombre);
    vm.Departamento(data.DepartamentoNombre);
    vm.FechaInicial(moment(data.StrFechaInicial, 'YYYYMMDD').format('DD/MM/YYYY'));
    vm.FechaFinal(moment(data.StrFechaFinal, 'YYYYMMDD').format('DD/MM/YYYY'));
    vm.Bases(numeral(data.Bases).format('#,###,##0.00'));
    vm.Cuotas(numeral(data.Cuotas).format('#,###,##0.00'));
    vm.Retencion(numeral(data.Retencion).format('#,###,##0.00'));
    vm.Total(numeral(data.Total).format('#,###,##0.00'));
    if (data.EsFace) {
        vm.Face('SI');
    } else {
        vm.Face('NO');
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
        columns: [{ data: "Sistema" },
            {
                data: "StrFecha",
                render: function (data, type, row) {
                    var html = "<div style='text-align:center'>" + moment(data, 'YYYYMMDD').format('DD/MM/YYYY') + "</div>";
                    return html;
                }
            },
            { data: "Serie" },
            { data: "NumFactura" },
            {
                data: "BaseIva",
                render: function (data, type, row) {
                    var html = "<div style='text-align:right'>" + numeral(data).format('#,###,##0.00') + " €</div>";
                    return html;
                }
            },
            {
                data: "CuotaIva",
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
                data: "EsDeCliente",
                render: function (data, type, row) {
                    var tipo = "PROVEEDOR";
                    if (data) tipo = "CLIENTE";
                    var html = "<div style='text-align:center'>" + tipo + "</div>";
                    return html;
                }
            },
            {
                data: "FacturaId",
                render: function (data, type, row) {
                    var bt1 = "<button class='btn btn-circle btn-success btn-lg' onclick='verPdf(" + data + ");' title='Ver / descargar PDF'> <i class='fa fa-file-pdf-o fa-fw'></i> </button>";
                    var bt2 = "<button class='btn btn-circle btn-warning btn-lg' onclick='eliminarDeEnvio(" + data + ");' title='Eliminar del envío'> <i class='fa fa-remove fa-fw'></i> </button>";
                    var html = "<div class='pull-right'>" + bt1 + " " +  bt2 + "</div>";
                    return html;
                }
            }]
    });
}

function loadTablaFacturas(data) {
    var dt = $('#dt_factura').dataTable();
    if (data !== null && data.length === 0) {
        mostrarMensajeSmart('No se han encontrado registros');
        var url = "EnvioGeneral.html";
        window.open(url, '_self');
    } else {
        dt.fnClearTable();
        dt.fnAddData(data);
        dt.fnDraw();
        $("#tbFactura").show();
    }
}

function verPdf(id) {
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
                    // hay que mostrarlo en la zona de datos
                    $.ajax({
                        type: "POST",
                        url: "EnvioApi.aspx/GetFacturasEnvio",
                        dataType: "json",
                        contentType: "application/json",
                        data: JSON.stringify(getParam()),
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

function salir() {
    var mf = function () {
        var url = "EnvioGeneral.html";
        window.open(url, '_self');
    }
    return mf;
}

function getParam() {
    var clienteId = gup('ClienteId');
    var departamentoId = gup('DepartamentoId');
    var param = {
        clienteId: clienteId,
        departamentoId: departamentoId
    };
    return param;
}