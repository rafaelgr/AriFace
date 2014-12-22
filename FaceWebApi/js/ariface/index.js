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
    // cargar graficos
    grafico();
}


function grafico() {
    datos = [];
    $.ajax({
        type: "POST",
        url: "EstadisticaApi.aspx/GetNumFacMes",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            for (var i = 0; i < data.d.length; i++) {
                var d = {
                    t: data.d[i].Anyo + data.d[i].Mes,
                    v: data.d[i].Numero
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
                xkey: 't',
                // A list of names of data record attributes that contain y-values.
                ykeys: ['v'],
                // Labels for the ykeys -- will be displayed when you hover over the
                // chart.
                labels: ['v']
            });
        },
        error: errorAjax
    });
}