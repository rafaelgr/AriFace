// ------------------------------------------------------------------------------------------------
// ariface.js:
// contiene las funciones comunes a todas páginas de la aplicación
// ------------------------------------------------------------------------------------------------
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