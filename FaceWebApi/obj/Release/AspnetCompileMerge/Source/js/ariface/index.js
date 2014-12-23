// de blank_ (pruebas)
function initForm() {
    comprobarLogin();
    // de smart admin
    pageSetUp();
    // buscar estadísticas
    $.ajax({
        type: "POST",
        url: "EstadisticaApi.aspx/GetEstadistica",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            var est = data.d;
            $('#facP').text(est.NumPendientes);
            $('#facA').text(est.NumAparcadas);
            $('#facE').text(est.NumEnviadas);
        },
        error: errorAjax
    });
    // cargar graficos (con anyo actual)
    var anyo = new Date().getFullYear();
    grafico(anyo);
}


function grafico(anyo) {
    datos = [];
    var data = { "anyo": anyo };
    $.ajax({
        type: "POST",
        url: "EstadisticaApi.aspx/GetNumFacMes",
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data, status) {
            for (var i = 0; i < data.d.length; i++) {
                var m = data.d[i].Mes;
                var n = data.d[i].Numero;
                var ms = ('0' + m).substring(m.length - 2, 2);
                var d = {
                    Mes: anyo + "-" + ms,
                    Valor: n
                };
                datos.push(d);
            }
            new Morris.Line({
                // ID of the element in which to draw the chart.
                element: 'graficoFac',
                // Chart data records -- each entry in this array corresponds to a point on
                // the chart.
                data: datos,
                // The name of the data record attribute that contains x-values.
                xkey: 'Mes',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['Valor'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['Facturas mes'],
                xLabels: 'month'
            });
        },
        error: errorAjax
    });
}