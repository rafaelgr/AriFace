/*-------------------------------------------------------------------------- 
plantillaGeneral.js
Funciones js par la página PlantillaGeneral.html

---------------------------------------------------------------------------*/
var responsiveHelper_dt_basic = undefined;
var responsiveHelper_datatable_fixed_column = undefined;
var responsiveHelper_datatable_col_reorder = undefined;
var responsiveHelper_datatable_tabletools = undefined;

var dataPlantillas;
var plantillaId;

var breakpointDefinition = {
    tablet: 1024,
    phone: 480
};


function initForm() {
    // de smart admin
    pageSetUp();
    //
    //
    initTablaPlantillas();
    // siempre se cargan todas las plantillas existentes (no serán muchas)
    $.ajax({
        type: "POST",
        url: "PlantillaApi.aspx/GetPlantillas",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            // hay que mostrarlo en la zona de datos
            loadTablaPlantillas(data.d);
        },
        error: errorAjax
    });
}

function initTablaPlantillas() {
    tablaCarro = $('#dt_plantilla').dataTable({
        autoWidth: true,
        preDrawCallback: function () {
            // Initialize the responsive datatables helper once.
            if (!responsiveHelper_dt_basic) {
                responsiveHelper_dt_basic = new ResponsiveDatatablesHelper($('#dt_plantilla'), breakpointDefinition);
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
        data: dataPlantillas,
        columns: [{
            data: "Nombre"
        },  {
            data: "PlantillaId",
            render: function (data, type, row) {
                var bt2 = "<button class='btn btn-circle btn-success btn-lg' onclick='editPlantilla(" + data + ");' title='Editar registro'> <i class='fa fa-edit fa-fw'></i> </button>";
                var html = "<div class='pull-right'>" + bt2 + "</div>";
                return html;
            }
        }]
    });
}

function loadTablaPlantillas(data) {
    var dt = $('#dt_plantilla').dataTable();
    if (data !== null && data.length === 0) {
        mostrarMensajeSmart('No se han encontrado registros');
        $("#tbPlantilla").hide();
    } else {
        dt.fnClearTable();
        dt.fnAddData(data);
        dt.fnDraw();
        $("#tbPlantilla").show();
    }
}

function editPlantilla(id) {
    // hay que abrir la página de detalle de administrador
    // pasando en la url ese ID
    var url = "PlantillaDetalle.html?PlantillaId=" + id;
    window.open(url, '_self');
}


