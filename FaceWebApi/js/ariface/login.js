// Funciones de apoyo a la página login.html
function initForm() {
    // de smart admin
    runAllForms();
    // 
    vm = new loginData();
    ko.applyBindings(vm);
    //
    getVersion();
    // asignación de eventos al clic
    $("#btnLogin").click(loginForm());
    $("#login-form").submit(function () {
        return false;
    });
}

function loginData() {
    var self = this;
    self.login = ko.observable();
    self.password = ko.observable();
}

function datosOK() {
    $('#login-form').validate({
        rules: {
            login: { required: true },
            password: { required: true }
        },
        // Messages for form validation
        messages: {
            login: {
                required: 'Introduzca login'

            },
            password: {
                required: 'Introduzca password'
            }
        },
        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });
    return $('#login-form').valid();
}

function loginForm() {
    var mf = function () {
        if (!datosOK()) {
            return;
        }
        var data = {
            "login": vm.login(),
            "password": vm.password()
        };
        $.ajax({
            type: "POST",
            url: "AdministradorApi.aspx/GetAdministradorLogin",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // Regresa el mensaje
                if (!data.d) {
                    mostrarMensaje('Login y/o password incorrectos');
                }
                var a = data.d;
                window.open('blank_.html','_self');
            },
            error: errorAjax
        });
    };
    return mf;
}

function getVersion() {
    $.ajax({
        type: "POST",
        url: "VersionApi.aspx/GetVersion",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            // Regresa el mensaje
            if (!data.d) {
                mostrarMensaje('Login y/o password incorrectos');
            }
            var a = data.d;
            $("#version").text(a);
            
        },
        error: errorAjax
    });
}

var errorAjax = function(xhr, textStatus, errorThrwon) {
    var m = xhr.responseText;
    if (!m) m = "Error general posiblemente falla la conexión";
    mostrarMensaje(m);
}

function mostrarMensaje(mens) {
    $("#mensaje").text(mens);
}