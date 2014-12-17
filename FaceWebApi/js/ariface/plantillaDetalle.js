/*-------------------------------------------------------------------------- 
administradorDetalle.js
Funciones js par la página AdministradorDetalle.html
---------------------------------------------------------------------------*/
function initForm() {
    // de smart admin
    pageSetUp();
    // 
    vm = new plantillaData();
    ko.applyBindings(vm);
    // asignación de eventos al clic
    $("#btnAceptar").click(aceptar());
    $("#btnSalir").click(salir());
    $("#frmPlantilla").submit(function () {
        return false;
    });
    CKEDITOR.replace('txtContenido', { height: '380px', startupFocus: true });
    var plantillaId = gup('PlantillaId');
    if (plantillaId != 0) {
        var data = {
            plantillaId: plantillaId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "PlantillaApi.aspx/GetPlantilla",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                loadData(data.d);
            },
            error: errorAjax
        });
    } else {
        // se trata de un alta ponemos el id a cero para indicarlo.
        vm.PlantillaId(0);
    }
}

function plantillaData() {
    var self = this;
    self.PlantillaId = ko.observable();
    self.Nombre = ko.observable();
    self.Contenido = ko.observable();
    self.Observaciones = ko.observable();
}

function loadData(data) {
    vm.PlantillaId(data.PlantillaId);
    vm.Nombre(data.Nombre);
    vm.Contenido(data.Contenido);
    vm.Observaciones(data.Observaciones);
    // las observaciones van a ir a una zona específica
    $('#Observaciones').html(vm.Observaciones());
}

function datosOK() {
    $('#frmPlantilla').validate({
                                        rules: {
            txtNombre: { required: true },
            txtContenido: { required: true }
        },
                                        // Messages for form validation
                                        messages: {
            txtNombre: {
                                                required: 'Introduzca el nombre'
                                            },
            txtContenido: {
                                                required: 'Introduzca el contenido'
                                            },
        },
                                        // Do not change code below
                                        errorPlacement: function (error, element) {
                                            error.insertAfter(element.parent());
                                        }
                                    });
    return $('#frmPlantilla').valid();
}

function aceptar() {
    var mf = function () {
        if (!datosOK())
            return;
        var data = {
            plantilla: {
                "PlantillaId": vm.PlantillaId(),
                "Nombre": vm.Nombre(),
                "Contenido": CKEDITOR.instances['txtContenido'].getData()
            }
        };
        $.ajax({
                   type: "POST",
                   url: "PlantillaApi.aspx/SetPlantilla",
                   dataType: "json",
                   contentType: "application/json",
                   data: JSON.stringify(data),
                   success: function (data, status) {
                       // hay que mostrarlo en la zona de datos
                       loadData(data.d);
                       // Nos volvemos al general
                       var url = "PlantillaGeneral.html";
                       window.open(url, '_self');
                   },
                   error: errorAjax
               });
    };
    return mf;
}

function salir() {
    var mf = function () {
        var url = "PlantillaGeneral.html";
        window.open(url, '_self');
    }
    return mf;
}