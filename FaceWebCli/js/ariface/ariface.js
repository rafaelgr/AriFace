// ------------------------------------------------------------------------------------------------
// ariface.js:
// contiene las funciones comunes a todas páginas de la aplicación
// ------------------------------------------------------------------------------------------------

//-- Se ejecuta al inciio de todas las páginas

function comprobarLogin() {
    // buscar el cookie
    try{
        var user = JSON.parse(getCookie("usu"));
    }
    catch(e) {
        // volver al login
        window.open('login.html', '_self');
    }
    // cargar el nombre en la zona correspondiente
    $('#userName').text(user.Nombre);
    $('.empresa_raiz').text(user.NifNombre);
    $('.empresa_cliente').text(user.ClienteNombre);
    $('.empresa_departamento').text(user.DepartamentoNombre);
    // probando
    getParametros();
    return user;
}


function mostrarMensaje(mens) {
    $("#mensaje").text(mens);
}

function mostrarMensajeSmart(mens) {
    $.SmartMessageBox({
        title: "<i class='fa fa-info'></i> Mensaje",
        content: mens,
        buttons: '[Aceptar]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Aceptar") {
            // no hacemos nada solo queríamos mostrar em mensaje
        }
    });
}

function mostrarMensajeSmartSiNo(mens) {
    $.SmartMessageBox({
        title: "<i class='fa fa-info'></i> Mensaje",
        content: mens,
        buttons: '[Aceptar][Cancelar]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Aceptar") {
            return 'S';
        }
        if (ButtonPressed === "Cancelar") {
            return 'N';
        }
    });
}

var errorAjax = function (xhr, textStatus, errorThrwon) {
    var m = xhr.responseText;
    if (!m) m = "Error general posiblemente falla la conexión";
    mostrarMensaje(m);
}

// gup stands from Get Url Parameters
function gup(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results === null)
        return "";
    else
        return results[1];
}

/*
*   Set and Get Cookies
*   this funtions come from http://www.w3schools.com/js/js_cookies.asp
*   they are used in forms in order to and retrieve
*   field's values in a cookie
*/
function are_cookies_enabled() {
    var cookieEnabled = (navigator.cookieEnabled) ? true : false;
    if (typeof navigator.cookieEnabled == "undefined" && !cookieEnabled) {
        document.cookie = "testcookie";
        cookieEnabled = (document.cookie.indexOf("testcookie") != -1) ? true : false;
    }
    return (cookieEnabled);
}

function setCookie(c_name, value, exdays) {
    if (!are_cookies_enabled()) {
        alert("NO COOKIES");
    }
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value;
}

function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
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
                mostrarMensajeSmart('No se ha podido obtener la versión');
            }
            var a = data.d;
            $("#version").text(a);

        },
        error: function (xhr, textStatus, errorThrwon) {
            var m = xhr.responseText;
            if (!m) m = "Error general posiblemente falla la conexión";
            mostrarMensajeSmart(m);
        }
    });
}

function getParametros() {
    $.ajax({
        type: "POST",
        url: "ParametrosApi.aspx/GetParametros",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            // Regresa el mensaje
            if (!data.d) {
                mostrarMensajeSmart('No se ha podido acceder a los parámetros');
            }
            var a = data.d;
            $(".titulo_app").text(a.TituloApp);

        },
        error: function (xhr, textStatus, errorThrwon) {
            var m = xhr.responseText;
            if (!m) m = "Error general posiblemente falla la conexión";
            mostrarMensajeSmart(m);
        }
    });
}

function comprobarClientePuntos() {
    $.ajax({
        type: "POST",
        url: "ParametrosApi.aspx/GetParametros",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            // Regresa el mensaje
            if (!data.d) {
                mostrarMensajeSmart('No se ha podido acceder a los parámetros');
            }
            var a = data.d;
            $(".titulo_app").text(a.TituloApp);

        },
        error: function (xhr, textStatus, errorThrwon) {
            var m = xhr.responseText;
            if (!m) m = "Error general posiblemente falla la conexión";
            mostrarMensajeSmart(m);
        }
    });
}

