//
function initForm() {
    // de smart admin
    pageSetUp();
    // asignación de eventos al clic
    $("#btnConsultarEstados").click(sendConsultarEstados());
    $("#frmConsultarEstados").submit(function () {
        return false;
    });
}
function datosOK() {
    //TODO: Incluir en la validación si el certificado figura en el almacén de certificados.
    $('#frmConsultarEstados').validate({
        rules: {
            txtCertSn1: { required: true },
        },
        // Messages for form validation
        messages: {
            txtCertSn1: {
                required: 'Introduzca el número de serie del certificado con el que desea firmar'
            }
        },
        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        }
    });
    return $('#frmConsultarEstados').valid();
}

function sendConsultarEstados() {
    var mf = function () {
        if (!datosOK()) {
            return;
        }
        // obtener el n.serie del certificado para la firma.
        var certSn = $('#txtCertSn1').val();
        // enviar la consulta por la red (AJAX)
        var data = {
            "certSn": certSn
        };
        $.ajax({
            type: "POST",
            url: "FaceApi.aspx/GetEstados",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (data, status) {
                // hay que mostrarlo en la zona de datos
            },
            error: errorAjax
        });
    };
    return mf;
}

var errorAjax = function (xhr, textStatus, errorThrwon) {
    var m = xhr.responseText;
    if (!m) m = "Error general posiblemente falla la conexión";
    mostrarMensajeSmart(m);
}