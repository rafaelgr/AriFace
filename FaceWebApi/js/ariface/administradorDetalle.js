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
    var adminId = gup('AdministradorId');
    $("#frmAdministrador").submit(function () {
        return false;
    });
    if (adminId !== '') {
        // hay que buscar ese elemento en concreto

    }
}

function admData() {
    var self = this;
    self.AdministradorId = ko.observable();
    self.Nombre = ko.observable();
    self.Login = ko.observable();
    self.Password = ko.observable();
    self.Email = ko.observable();
    self.CertSn = ko.observable();
}

function loadData(data) {
    vm.AdministradorId(data.AdministradorId);
    vm.Nombre(data.Nombre);
    vm.Login(data.Login);
    vm.Password(data.Password);
    vm.Email(data.Email);
    vm.CertSn(data.CertSn);
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
            txtEmail: { required: true },
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
                required: 'Introduzca el correo'
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
    };
    return mf;
}

function salir() {
}