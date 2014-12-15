/*-------------------------------------------------------------------------- 
administradorDetalle.js
Funciones js par la página AdministradorDetalle.html
---------------------------------------------------------------------------*/

// variables y objetos usados

function initForm() {
    // de smart admin
    pageSetUp();
    // 
    vm = new empData();
    ko.applyBindings(vm);
    // asignación de eventos al clic
    $("#btnAceptar").click(aceptar());
    $("#btnSalir").click(salir());
    $("#frmDatos").submit(function () {
        return false;
    });
    var empresaId = gup('EmpresaId');
    if (empresaId != "") {
        var data = {
            nif: empresaId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
                   type: "POST",
                   url: "EmpresaraizApi.aspx/GetEmpresaRaizById",
                   dataType: "json",
                   contentType: "application/json",
                   data: JSON.stringify(data),
                   success: function (data, status) {
                       loadData(data.d);
                   },
                   error: errorAjax
               });
    }
    else {
        // se trata de un alta ponemos el id a cero para indicarlo.
        vm.Nif('');
    }
}

function empData() {
    var self = this;
    self.Nif = ko.observable();
    self.Nombre = ko.observable();
}

function loadData(data) {
    vm.Nif(data.Nif);
    vm.Nombre(data.Nombre);
}

function datosOK() {
    // antes de la validación decerificamos que si ha rellenado FACE
    // lo ha hecho completo
    $('#frmDatos').validate({
                                  rules: {
                                      txtNif: { required: true },
                                      txtNombre: { required: true }
            
        },
                                  // Messages for form validation
                                  messages: {
            txtNombre: {
                                          required: 'Introduzca el nombre'
                                      },
            txtNif: {
                                          required: 'Introduzca el NIF'
                                      },
        },
                                  // Do not change code below
                                  errorPlacement: function (error, element) {
                                      error.insertAfter(element.parent());
                                  }
                              });
    return $('#frmDatos').valid();
}

function aceptar() {
    var mf = function () {
        if (!datosOK())
            return;
        var data = {
            empresa: {
                "Nif": vm.Nif(),
                "Nombre": vm.Nombre()
            }
        };
        $.ajax({
                   type: "POST",
                   url: "EmpresaRaizApi.aspx/SetEmpresaRaiz",
                   dataType: "json",
                   contentType: "application/json",
                   data: JSON.stringify(data),
                   success: function (data, status) {
                       // hay que mostrarlo en la zona de datos
                       loadData(data.d);
                       // Nos volvemos al general
                       var url = "EmpresaRaizGeneral.html?EmpresaId=" + vm.Nif();
                       window.open(url, '_self');
                   },
                   error: errorAjax
               });
    };
    return mf;
}

function salir() {
    var mf = function () {
        var url = "EmpresaRaizGeneral.html";
        window.open(url, '_self');
    }
    return mf;
}