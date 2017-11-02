// Funciones de apoyo a la página login.html
function initForm() {
    // de smart admin
    runAllForms();
    // 
    vm = new loginData();
    ko.applyBindings(vm);
    //
    getVersion();
    getParametros();
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
            url: "UsuarioApi.aspx/GetUsuarioLogin",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // Regresa el mensaje
                if (!data.d) {
                    mostrarMensaje('Login y/o password incorrectos');
                } else {
                    var a = data.d;
                    // guadar el usuario en los cookies
                    setCookie("usu", JSON.stringify(data.d), 1)
                    window.open('Index.html', '_self');
                }
            },
            error: function (xhr, textStatus, errorThrwon) {
                var m = xhr.responseText;
                if (!m) m = "Error general posiblemente falla la conexión";
                mostrarMensaje(m);
            }
        });
    };
    return mf;
}