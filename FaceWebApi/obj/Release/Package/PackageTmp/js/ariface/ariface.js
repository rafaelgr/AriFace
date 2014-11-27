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