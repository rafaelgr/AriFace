/*-------------------------------------------------------------------------- 
administradorDetalle.js
Funciones js par la página AdministradorDetalle.html
---------------------------------------------------------------------------*/

// variables y objetos usados
var miniUnidad = function (codigo, nombre) {
    this.Codigo = codigo;
    this.Nombre = nombre;
}

function initForm() {
    comprobarLogin();
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
    $('#cmbOrganos').change(cambioOrgano);
    $('#cmbUnidades').change(cambioUnidad);
    $('#cmbOficinas').change(cambioOficina);

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
                       loadData(data.d);
                   },
                   error: errorAjax
               });
    }
    else {
        // se trata de un alta ponemos el id a cero para indicarlo.
        vm.ClienteId(0);
        loadComboOrganoGestor();
    }
}

function cliData() {
    var self = this;
    self.ClienteId = ko.observable();
    self.Nombre = ko.observable();
    self.Cif = ko.observable();
    self.Email = ko.observable();
    self.UnidadTramitadora = ko.observable();
    self.OrganoGestor = ko.observable();
    self.OficinaContable = ko.observable();
    // apoyo para desplegables
    self.PosiblesOrganos = ko.observableArray([]);
    self.PosiblesUnidades = ko.observableArray([]);
    self.PosiblesOficinas = ko.observableArray([]);
    // nuevos para códigos
    self.CodOrganoGestor = ko.observable();
    self.CodUnidadTramitadora = ko.observable();
    self.CodOficinaContable = ko.observable();
}

function loadData(data) {
    vm.ClienteId(data.ClienteId);
    vm.Nombre(data.Nombre);
    vm.Cif(data.Cif);
    vm.Email(data.Email);
    vm.CodOrganoGestor(data.CodOrganoGestor);
    vm.CodUnidadTramitadora(data.CodUnidadTramitadora);
    vm.CodOficinaContable(data.CodOficinaContable);
    vm.OrganoGestor(data.CodOrganoGestor);
    vm.UnidadTramitadora(data.CodUnidadTramitadora);
    vm.OficinaContable(data.CodOficinaContable);
    loadComboOrganoGestor(data.CodOrganoGestor);
    loadComboUnidadTramitadora(data.CodOrganoGestor, data.CodUnidadTramitadora);
    loadComboOficinaContable(data.CodOrganoGestor, data.CodUnidadTramitadora, data.CodOficinaContable)
}

function loadComboOrganoGestor(organoGestorCodigo) {
    $.ajax({
        type: "POST",
        url: "UnidadApi.aspx/GetOg",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            vm.PosiblesOrganos(v);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (organoGestorCodigo != null) {
                    if (data.d[i].Codigo === organoGestorCodigo) {
                        vm.OrganoGestor(mu);
                        vm.CodOrganoGestor(data.d[i].Codigo);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (organoGestorCodigo == null) {
                vm.OrganoGestor(new miniUnidad('', ''));
                vm.CodOrganoGestor('');
            }
            vm.PosiblesOrganos(v);
        },
        error: errorAjax
    });

}

function loadComboUnidadTramitadora(organoGestorCodigo, unidadTramitadoraCodigo) {
    data = { "organoGestorCodigo": organoGestorCodigo, "unidadTramitadoraCodigo": unidadTramitadoraCodigo };
    $.ajax({
        type: "POST",
        url: "UnidadApi.aspx/GetUtOfOg",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            vm.PosiblesUnidades(v);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (unidadTramitadoraCodigo != null) {
                    if (data.d[i].Codigo === unidadTramitadoraCodigo) {
                        vm.UnidadTramitadora(mu);
                        vm.CodUnidadTramitadora(data.d[i].Codigo);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (unidadTramitadoraCodigo == null) {
                vm.UnidadTramitadora(new miniUnidad('', ''));
                vm.CodUnidadTramitadora('');
            }
            vm.PosiblesUnidades(v);
        },
        error: errorAjax
    });
}

function loadComboOficinaContable(organoGestorCodigo, unidadTramitadoraCodigo, oficinaContableCodigo) {
    data = { "organoGestorCodigo": organoGestorCodigo, "unidadTramitadoraCodigo": unidadTramitadoraCodigo };
    $.ajax({
        type: "POST",
        url: "UnidadApi.aspx/GetOcOfUt",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            vm.PosiblesOficinas(v);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (oficinaContableCodigo != null) {
                    if (data.d[i].Codigo === oficinaContableCodigo) {
                        vm.OficinaContable(mu);
                        vm.CodOficinaContable(data.d[i].Codigo);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (oficinaContableCodigo == null) {
                vm.OficinaContable(new miniUnidad('', ''));
                vm.CodOficinaContable('');
            }
            vm.PosiblesOficinas(v);
        },
        error: errorAjax
    });

}

// se dispara cuando cambia el órgano
function cambioOrgano() {
    var codOg = vm.OrganoGestor().Codigo;
    if (codOg == null) codOg = vm.CodOrganoGestor();
    loadComboUnidadTramitadora(codOg);
    vm.OficinaContable(new miniUnidad('', ''));
    vm.CodOrganoGestor(codOg);
    vm.CodUnidadTramitadora('');
    vm.CodOficinaContable('');
}

function cambioUnidad() {
    var codUt = vm.UnidadTramitadora().Codigo;
    if (codUt == null) codUt = vm.CodUnidadTramitadora().Codigo;
    loadComboOficinaContable(vm.OrganoGestor().Codigo, codUt);
    vm.CodUnidadTramitadora(vm.UnidadTramitadora().Codigo);
    vm.CodOficinaContable('');
}

function cambioOficina() {
    vm.CodOficinaContable(vm.OficinaContable().Codigo);
}

function datosOK() {
    // antes de la validación decerificamos que si ha rellenado FACE
    // lo ha hecho completo
    if (vm.OrganoGestor().Codigo !== "") {
        if (vm.UnidadTramitadora().Codigo === "" || vm.OficinaContable().Codigo === "") {
            var mens = "Si rellena los campos FACE, debe darle valor a todos";
            mostrarMensajeSmart(mens);
        }
    } 
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
        var cog = vm.CodOrganoGestor();
        var cut = vm.CodUnidadTramitadora();
        var coc = vm.CodOficinaContable();
        //if (vm.OrganoGestor() != null && vm.OrganoGestor().Codigo !== "") cog = vm.OrganoGestor().Codigo;
        //if (vm.UnidadTramitadora() != null && vm.UnidadTramitadora().Codigo !== "") cut = vm.UnidadTramitadora().Codigo;
        //if (vm.OficinaContable() != null && vm.OficinaContable().Codigo !== "") coc = vm.OficinaContable().Codigo;
        var data = {
            cliente: {
                "Cif": vm.Cif(),
                "ClienteId": vm.ClienteId(),
                "CodOficinaContable": coc,
                "CodOrganoGestor": cog,
                "CodUnidadTramitadora": cut,
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