/*-------------------------------------------------------------------------- 
administradorDetalle.js
Funciones js par la página AdministradorDetalle.html
---------------------------------------------------------------------------*/
function initForm() {
    // de smart admin
    pageSetUp();
    // 
    vm = new admData();
    ko.applyBindings(vm);
    // asignación de eventos al clic
    $("#btnAceptar").click(aceptar());
    $("#btnSalir").click(salir());
    $("#frmAdministrador").submit(function () {
        return false;
    });

    var adminId = gup('AdministradorId');
    if (adminId !== '') {
        var data = {
            id: adminId
        }
        // hay que buscar ese elemento en concreto
        $.ajax({
            type: "POST",
            url: "AdministradorApi.aspx/GetAdministradorById",
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
}

function admData() {
    var self = this;
    self.AdministradorId = ko.observable();
    self.Nombre = ko.observable();
    self.Login = ko.observable();
    self.Password = ko.observable();
    self.Email = ko.observable();
    self.Certsn = ko.observable();
}

function loadData(data) {
    vm.AdministradorId(data.AdministradorId);
    vm.Nombre(data.Nombre);
    vm.Login(data.Login);
    vm.Password(data.Password);
    vm.Email(data.Email);
    vm.Certsn(data.Certsn);
}

function datosOK() {
    // antes de la validación de form hay que verificar las password
    if ($('#txtPassword1').val() !== "") {
        // si ha puesto algo, debe coincidir con el otro campo
        if ($('#txtPassword1').val() !== $('#txtPassword2').val()) {
            mostrarMensajeSmart('Las contraseñas no coinciden');
            return false;
        }
    }
    $('#frmAdministrador').validate({
        rules: {
            txtNombre: { required: true },
            txtLogin: { required: true },
            txtEmail: { required: true, email:true },
            txtCertSn: { required: true }
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
            },
            txtCertSn: {
                required: 'Introduzca la clave de su certificado'
            }
        },
        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });
    return $('#frmAdministrador').valid();
}


function aceptar() {
    var mf = function () {
        if (!datosOK()) return;
        var data = {
            "AdministradorId": vm.AdministradorId(),
            "Nombre": vm.Nombre(),
            "Login": vm.Login(),
            "Password": vm.Password(),
            "Email": vm.Email(),
            "Certsn": vm.Certsn()
        };
        $.ajax({
            type: "POST",
            url: "AdministradorApi.aspx/GetAdministradorById",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
                loadData(data.d);
            },
            error: errorAjax
        });
    };
    return mf;
}

function salir() {
    var mf = function () {
        var url = "AdministradorGeneral.html";
        window.open(url, '_self');
    }
    return mf;
}