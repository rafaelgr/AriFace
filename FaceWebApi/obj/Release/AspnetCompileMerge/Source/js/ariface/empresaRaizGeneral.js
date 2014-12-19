/*-------------------------------------------------------------------------- 
administradorGeneral.js
Funciones js par la página AdministradorGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var dataEmpresas;
var empresaId;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    comprobarLogin();
    // de smart admin
    pageSetUp();
    //
    $('#btnBuscar').click(buscarEmpresa());
    $('#btnAlta').click(crearEmpresa());
    $('#frmBuscar').submit(function () {
        return false
    });
    //
    initTablaEmpresas();
    // comprobamos parámetros
    empresaId = gup('EmpresaId');
    if (empresaId !== '') {
        $('#btnBuscar').hide();
        $('#ldgBuscar').show();
        // cargar la tabla con un único valor que es el que corresponde.
        var data = {
            nif: empresaId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "EmpresaRaizApi.aspx/GetEmpresaRaizById",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                var data2 = [data.d];
                loadTablaEmpresas(data2);
                $('#btnBuscar').show();
                $('#ldgBuscar').hide();
            },
            error: function (xhr, textStatus, errorThrwon) {
                $('#btnBuscar').show();
                $('#ldgBuscar').hide();
                var m = xhr.responseText;
                if (!m) m = "Error general posiblemente falla la conexión";
                mostrarMensaje(m);
            }
        });
    }
}

function initTablaEmpresas() {
    tablaCarro = $('#dt_empresa').dataTable({
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_empresa'), breakpointDefinition);
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
        data: dataEmpresas,
        columns: [{
            data: "Nif"
        }, {
            data: "Nombre"
        },{
            data: "Nif",
            render: function (data, type, row) {
                var bt1 = "<button class='btn btn-circle btn-danger btn-lg' onclick='deleteEmpresa(\"" + data + "\");' title='Eliminar registro'> <i class='fa fa-trash-o fa-fw'></i> </button>";
                var bt2 = "<button class='btn btn-circle btn-success btn-lg' onclick='editEmpresa(\"" + data + "\");' title='Editar registro'> <i class='fa fa-edit fa-fw'></i> </button>";
                var html = "<div class='pull-right'>" + bt2 + "</div>";
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

function loadTablaEmpresas(data) {
    var dt = $('#dt_empresa').dataTable();
    if (data !== null && data.length === 0) {
        mostrarMensajeSmart('No se han encontrado registros');
        $("#tbEmpresa").hide();
    } else {
        dt.fnClearTable();
        dt.fnAddData(data);
        dt.fnDraw();
        $("#tbEmpresa").show();
    }
}

function buscarEmpresa() {
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
            url: "EmpresaRaizApi.aspx/BuscarEmpresaRaiz",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                loadTablaEmpresas(data.d);
            },
            error: errorAjax
        });
    };
    return mf;
}

function crearEmpresa() {
    var mf = function () {
        var url = "EmpresaRaizDetalle.html?empresaId=0";
        window.open(url, '_self');
    };
    return mf;
}

function deleteEmpresa(nif) {
    // mensaje de confirmación
    var mens = "¿Realmente desea borrar este registro?";
    $.SmartMessageBox({
        title: "<i class='fa fa-info'></i> Mensaje",
        content: mens,
        buttons: '[Aceptar][Cancelar]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Aceptar") {
            var data = {
                nif: nif            };
            $.ajax({
                type: "POST",
                url: "EmpresaRaizApi.aspx/DeleteEmpresaRaiz",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (data, status) {
                    var fn = buscarEmpresas();
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

function editEmpresa(id) {
    // hay que abrir la página de detalle de administrador
    // pasando en la url ese ID
    var url = "EmpresaRaizDetalle.html?EmpresaId=" + id;
    window.open(url, '_self');
}


