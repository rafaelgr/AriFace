/*-------------------------------------------------------------------------- 
usuarioDetalle.js
Funciones js par la página UsuarioDetalle.html
---------------------------------------------------------------------------*/
var usuario;
// variables y objetos usados
var miniUnidad = function (codigo, nombre) {
    this.Codigo = codigo;
    this.Nombre = nombre;
}


function initForm() {
    usuario = comprobarLogin();
    comprobarClientePuntos(usuario.CodClienAriges);
    // de smart admin
    pageSetUp();
    // 
    vm = new usuData();
    ko.applyBindings(vm);
    // asignación de eventos al clic
    $("#btnAceptar").click(aceptar());
    $("#btnSalir").click(salir());
    $("#frmUsuario").submit(function () {
        return false;
    });
    //$('#cmbNifs').change(cambioEmpresaRaiz);
    //$('#cmbClientes').change(cambioCliente);

    var usuarioId = usuario.UsuarioId;
    if (usuarioId != 0) {
        var data = {
            id: usuarioId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "UsuarioApi.aspx/GetUsuarioById",
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
        vm.UsuarioId(0);
        loadComboNifs();
    }
}

function usuData() {
    var self = this;
    self.UsuarioId = ko.observable();
    self.Nombre = ko.observable();
    self.Password = ko.observable();
    self.Login = ko.observable();
    self.Email = ko.observable();
    self.EmpresaRaiz = ko.observable();
    self.Cliente = ko.observable();
    self.Departamento = ko.observable();
    // apoyo para desplegables
    self.PosiblesNifs = ko.observableArray([]);
    self.PosiblesClientes = ko.observableArray([]);
    self.PosiblesDepartamentos = ko.observableArray([]);
}

function loadData(data) {
    vm.UsuarioId(data.UsuarioId);
    vm.Nombre(data.Nombre);
    vm.Password(data.Password);
    vm.Login(data.Login);
    vm.Email(data.Email);
    // carga de combos
    loadComboNifs(data.Nif);
    loadComboClientes(data.Nif, data.ClienteId);
    loadComboDepartamentos(data.ClienteId, data.DepartamentoId);
}

function datosOK() {
    // antes de la validación de form hay que verificar las password
    if ($('#txtPassword1').val() !== "") {
        // si ha puesto algo, debe coincidir con el otro campo
        if ($('#txtPassword1').val() !== $('#txtPassword2').val()) {
            mostrarMensajeSmart('Las contraseñas no coinciden');
            return false;
        }
        vm.Password($("#txtPassword1").val());
    } else {
        vm.Password("");
    }
    // controlamos que si es un alta debe dar una contraseña.
    if (vm.UsuarioId() === 0 && $('#txtPassword1').val() === ""){
        mostrarMensajeSmart('Debe introducir una contraseña en el alta');
        return false;
    }
    $('#frmUsuario').validate({
                                        rules: {
            txtNombre: { required: true },
            txtLogin: { required: true },
            txtEmail: { required: true, email:true },
        },
                                        // Messages for form validation
                                        messages: {
            txtNombre: {
                                                required: 'Introduzca el nombre'
                                            },
            txtLogin: {
                                                required: 'Introduzca el login'
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
    return $('#frmUsuario').valid();
}

function aceptar() {
    var mf = function () {
        if (!datosOK())
            return;
        var data = {
            usuario: {
                "UsuarioId": vm.UsuarioId(),
                "Nombre": vm.Nombre(),
                "Password": vm.Password(),
                "NifNombre": vm.EmpresaRaiz().Nombre,
                "Nif": vm.EmpresaRaiz().Codigo,
                "Login": vm.Login(),
                "Email": vm.Email(),
                "DepartamentoNombre": vm.Departamento().Nombre,
                "DepartamentoId": vm.Departamento().Codigo,
                "ClienteNombre": vm.Cliente().Nombre,
                "ClienteId": vm.Cliente().Codigo
            }
        };
        $.ajax({
                   type: "POST",
                   url: "UsuarioApi.aspx/SetUsuario",
                   dataType: "json",
                   contentType: "application/json",
                   data: JSON.stringify(data),
                   success: function (data, status) {
                       // hay que mostrarlo en la zona de datos
                       loadData(data.d);
                       // Nos volvemos al general
                       var url = "Index.html";
                       window.open(url, '_self');
                   },
                   error: errorAjax
               });
    };
    return mf;
}

function salir() {
    var mf = function () {
        var url = "Index.html";
        window.open(url, '_self');
    }
    return mf;
}

function loadComboNifs(nif) {
    $.ajax({
        type: "POST",
        url: "UsuarioApi.aspx/GetEr",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            vm.PosiblesNifs(v);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (nif != null) {
                    if (data.d[i].Codigo === nif) {
                        vm.EmpresaRaiz(mu);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (nif == null) {
                vm.EmpresaRaiz(new miniUnidad('', ''));
            }
            vm.PosiblesNifs(v);
        },
        error: errorAjax
    });

}

function loadComboClientes(nif, clienteId) {
    data = { "nif": nif, "clienteId": clienteId };
    $.ajax({
        type: "POST",
        url: "UsuarioApi.aspx/GetCliOfEr",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            vm.PosiblesClientes(data.d);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (clienteId != 0) {
                    if (data.d[i].Codigo === clienteId) {
                        vm.Cliente(mu);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (clienteId == 0) {
                vm.Cliente(new miniUnidad(0, ''));
            }
            vm.PosiblesClientes(v);
        },
        error: errorAjax
    });
}

function loadComboDepartamentos(clienteId, departamentoId) {
    data = { "clienteId": clienteId };
    $.ajax({
        type: "POST",
        url: "UsuarioApi.aspx/GetDepOfCli",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (data, status) {
            // hay que mostrarlo en la zona de datos
            var v = [];
            vm.PosiblesDepartamentos(data.d);
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (departamentoId != 0) {
                    if (data.d[i].Codigo === departamentoId) {
                        vm.Departamento(mu);
                    }
                }
            }
            // en las altas hay que dejar una selección en vacío.
            if (departamentoId == 0) {
                vm.Departamento(new miniUnidad(0, ''));
            }
            vm.PosiblesDepartamentos(v);
        },
        error: errorAjax
    });

}

// se dispara cuando cambia el órgano
function cambioEmpresaRaiz() {
    loadComboClientes(vm.EmpresaRaiz().Codigo);
    vm.Departamento(new miniUnidad(0, ''));
}

function cambioCliente() {
    loadComboDepartamentos(vm.Cliente().Codigo);
}