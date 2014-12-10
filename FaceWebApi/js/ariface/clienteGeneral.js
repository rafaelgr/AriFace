﻿/*-------------------------------------------------------------------------- 
administradorGeneral.js
Funciones js par la página AdministradorGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var dataClientes;
var clienteId;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    // de smart admin
    pageSetUp();
    //
    $('#btnBuscar').click(buscarClientes());
    $('#btnAlta').click(crearCliente());
    $('#frmBuscar').submit(function () {
        return false
    });
    //
    initTablaClientes();
    // comprobamos parámetros
    clienteId = gup('ClienteId');
    if (administradorId !== '') {
        // cargar la tabla con un único valor que es el que corresponde.
        var data = {
            id: clienteId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "ClienteApi.aspx/GetClienteById",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                var data2 = [data.d];
                loadTablaClientes(data2);
            },
            error: errorAjax
        });
    }
}

function initTablaClientes() {
    tablaCarro = $('#dt_cliente').dataTable({
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_cliente'), breakpointDefinition);
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
        data: dataClientes,
        columns: [{
            data: "Nombre"
        }, {
            data: "Cif"
        }, {
            data: "Email"
        }, {
            data: "CodOrganoGestor"
        }, {
            data: "CodUnidadTramiento"
        }, {
            data: "CodOficinaContable"
        }, {
            data: "AdministradorId",
            render: function (data, type, row) {
                var bt1 = "<button class='btn btn-circle btn-danger btn-lg' onclick='deleteAdministrador(" + data + ");' title='Eliminar registro'> <i class='fa fa-trash-o fa-fw'></i> </button>";
                var bt2 = "<button class='btn btn-circle btn-success btn-lg' onclick='editAdministrador(" + data + ");' title='Editar registro'> <i class='fa fa-edit fa-fw'></i> </button>";
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

function loadTablaClientes(data) {
    var dt = $('#dt_cliente').dataTable();
    if (data !== null && data.length === 0) {
        mostrarMensajeSmart('No se han encontrado registros');
        $("#tbAdministrador").hide();
    } else {
        dt.fnClearTable();
        dt.fnAddData(data);
        dt.fnDraw();
        $("#tbCliente").show();
    }
}

function buscarClientes() {
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
            url: "ClienteApi.aspx/BuscarClientes",
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

function crearCliente() {
    var mf = function () {
        var url = "ClienteDetalle.html?ClienteId=0";
        window.open(url, '_self');
    };
    return mf;
}

function deleteCliente(id) {
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
                url: "ClienteApi.aspx/DeleteCliente",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (data, status) {
                    var fn = buscarClientes();
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

function editCliente(id) {
    // hay que abrir la página de detalle de administrador
    // pasando en la url ese ID
    var url = "ClienteDetalle.html?ClienteId=" + id;
    window.open(url, '_self');
}


