/*-------------------------------------------------------------------------- 
administradorDetalle.js
Funciones js par la página AdministradorDetalle.html
---------------------------------------------------------------------------*/

// variables y objetos usados
var miniUnidad = function (codigo, nombre) {
    this.codigo = codigo;
    this.nombre = nombre;
}

function initForm() {
    // de smart admin
    pageSetUp();
    // 
    vm = new cliData();
    ko.applyBindings(vm);
    // asignación de eventos al clic
    $("#btnAceptar").click(aceptar());
    $("#btnSalir").click(salir());
    $("#frmDatos").submit(function () {
        return false;
    });

    var clienteId = gup('ClienteId');
    if (clienteId != 0) {
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
                       loadData(data.d);
                   },
                   error: errorAjax
               });
    }
    else {
        // se trata de un alta ponemos el id a cero para indicarlo.
        vm.ClienteId(0);
    }
}

function cliData() {
    var self = this;
    self.ClienteId = ko.observable();
    self.Nombre = ko.observable();
    self.Cif = ko.observable();
    self.Email = ko.observable();
    self.CodUnidadTramitadora = ko.observable();
    self.CodOrganoGestor = ko.observable();
    self.CodOficinaContable = ko.observable();
    // apoyo para desplegables
    self.PosiblesOrganos = ko.observableArray([]);
    self.PosiblesUnidades = ko.observableArray([]);
    self.PosiblesOficinas = ko.observableArray([]);
}

function loadData(data) {
    vm.ClienteId(data.ClienteId);
    vm.Nombre(data.Nombre);
    vm.Cif(data.Cif);
    vm.Email(data.Email);
    vm.CodOrganoGestor(data.CodOrganoGestor);
    vm.CodUnidadTramitadora(data.CodUnidadTramitadora);
    vm.CodOficinaContable(data.CodOficinaContable);
}

function datosOK() {
    // antes de la validación de form hay que verificar las password
    $('#frmDatos').validate({
                                  rules: {
            txtNombre: { required: true },
            txtCif: { required: true },
            txtEmail: { required: true, email:true },
        },
                                  // Messages for form validation
                                  messages: {
            txtNombre: {
                                          required: 'Introduzca el nombre'
                                      },
            txtCif: {
                                          required: 'Introduzca el cif'
                                      },
            txtEmail: {
                                          required: 'Introduzca el correo',
                                          email: 'Debe usar un correo válido'
                                      }

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
            cliente: {
                "Cif": vm.Cif(),
                "ClienteId": vm.ClienteId(),
                "CodOficinaContable": vm.CodOficinaContable(),
                "CodOrganoGestor": vm.CodOrganoGestor(),
                "CodUnidadTramitadora": vm.CodUnidadTramitadora(),
                "Email": vm.Email(),
                "Nombre": vm.Nombre()
            }
        };
        $.ajax({
                   type: "POST",
                   url: "ClienteApi.aspx/SetCliente",
                   dataType: "json",
                   contentType: "application/json",
                   data: JSON.stringify(data),
                   success: function (data, status) {
                       // hay que mostrarlo en la zona de datos
                       loadData(data.d);
                       // Nos volvemos al general
                       var url = "ClienteGeneral.html?ClienteId=" + vm.ClienteId();
                       window.open(url, '_self');
                   },
                   error: errorAjax
               });
    };
    return mf;
}

function salir() {
    var mf = function () {
        var url = "ClienteGeneral.html";
        window.open(url, '_self');
    }
    return mf;
}