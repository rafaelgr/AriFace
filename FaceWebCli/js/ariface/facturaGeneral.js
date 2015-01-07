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
var usuario;

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
    // de smart admin
    pageSetUp();
    //
    $('#btnBuscar').click(buscarFacturas());
    $('#frmBuscar').submit(function () {
        return false
    });
    $('#cmbTrimestre').change(function () {
        loadComboMeses(Number(vm.Trimestre().Codigo));
    });
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
    initTablaFacturas();
    // cargamos las facturas que puede ver ese usuario
    var data = {
        usuarioId: usuario.UsuarioId,
        a: 0,
        q: 0,
        m: 0,
        esCliente: !proveedor
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
            loadTablaFacturas(pf.Facturas);
        },
        error: errorAjax
    });
    vm.Ano(new miniUnidad('0', 'Todos los años'));
    vm.Trimestre(new miniUnidad('0', 'Todos los trimestres'));
    vm.Mes(new miniUnidad('0', 'Todos los meses'));
    loadComboAnos();
    loadComboTrimestres();
    loadComboMeses();
}

function facData() {
    var self = this;
    self.SumBases = ko.observable();
    self.SumCuotas = ko.observable();
    self.SumTotal = ko.observable();
    self.SumRetencion = ko.observable();
    self.SumAportacion = ko.observable();
    self.Ano = ko.observable();
    self.Trimestre = ko.observable();
    self.Mes = ko.observable();
    // apoyo para desplegables
    self.PosiblesAnos = ko.observableArray([]);
    self.PosiblesTrimestres = ko.observableArray([]);
    self.PosiblesMeses = ko.observableArray([]);
}

function loadComboAnos(ano) {
    if (ano == null) ano = 0;
    data = { "usuarioId": usuario.UsuarioId};
    $.ajax({
        type: "POST",
        url: "FacturaApi.aspx/GetAnosFacturados",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            var mu2 = new miniUnidad('0', 'Todos los años');
            v.push(mu2);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (ano != null) {
                    if (data.d[i].Codigo === ano) {
                        vm.Ano(mu);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (ano == 0) {
                vm.Ano(mu2);
            }
            vm.PosiblesAnos(v);
        },
        error: errorAjax
    });
}

function loadComboTrimestres(t) {
    if (t == null) t = 0;
    $.ajax({
        type: "POST",
        url: "FacturaApi.aspx/GetTrimestres",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            vm.PosiblesTrimestres(v);
            var mu2 = new miniUnidad('0', 'Todos los trimestres');
            v.push(mu2);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (t != null) {
                    if (data.d[i].Codigo === t) {
                        vm.Trimestre(mu);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (t == 0) {
                vm.Trimestre(mu2);
            }
            vm.PosiblesTrimestres(v);
        },
        error: errorAjax
    });
}

function loadComboMeses(t) {
    if (t == null) t = 0;
    data = { "t": t };
    $.ajax({
        type: "POST",
        url: "FacturaApi.aspx/GetMeses",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            vm.PosiblesMeses(v);
            var mu2 = new miniUnidad('0', 'Todos los meses');
            v.push(mu2);            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                //if (t != null) {
                //    if (data.d[i].Codigo === t) {
                //        vm.Mes(mu);
                //    }
                //}
            }
            // en las altas hay que dejar una selección en vacío.
            vm.Mes(mu2);
            vm.PosiblesMeses(v);
        },
        error: errorAjax
    });
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
                var bt0 = "<button class='btn btn-circle btn-primary' onclick='verXml(" + data + ");' title='Ver / descargar XML'> <i class='fa fa-file-code-o fa-fw'></i> </button>";
                var bt1 = "<button class='btn btn-circle btn-success' onclick='verPdf(" + data + ");' title='Ver / descargar PDF'> <i class='fa fa-file-pdf-o fa-fw'></i> </button>";
                var bt2 = "<button class='btn btn-circle btn-warning' onclick='eliminarDeEnvio(" + data + ");' title='Eliminar del envío'> <i class='fa fa-remove fa-fw'></i> </button>";
                if (row.Estado == 0) {
                    bt2 = "<button class='btn btn-circle btn-warning' onclick='agregarAlEnvio(" + data + ");' title='Agregar al envío'> <i class='fa fa-undo fa-fw'></i> </button>";
                } 
                var html = "<div class='pull-right'>" + bt0 + " " + bt1 + "</div>";
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
                loadTablaFacturas(pf.Facturas);
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