// de blank_ (pruebas)
var chart = null;

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
    //
    vm = new facData();
    ko.applyBindings(vm);
    // cargar graficos (con anyo actual)
    var anyo = new Date().getFullYear();
    loadComboAnos(anyo);
    grafico(anyo);
    $('#cmbAno').change(function () {
        loadComboAnos(Number(vm.Ano().Codigo));
        grafico(Number(vm.Ano().Codigo));
    });
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
                var ms = m;
                var d = {
                    Mes: anyo + "-" + ms,
                    Valor: n
                };
                datos.push(d);
            }
            if (chart == null) {
                chart = new Morris.Line({
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
            } else {
                chart.setData(datos);
            }
        },
        error: errorAjax
    });
}

var miniUnidad = function (codigo, nombre) {
    this.Codigo = codigo;
    this.Nombre = nombre;
}

function facData() {
    var self = this;
    self.Ano = ko.observable();
    // apoyo para desplegables
    self.PosiblesAnos = ko.observableArray([]);
}

function loadComboAnos(ano) {
    if (ano == null) ano = 0;
    $.ajax({
        type: "POST",
        url: "EstadisticaApi.aspx/GetAnosFacturados",
        dataType: "json",
        contentType: "application/json",
        success: function (data, status) {
            var v = [];
            // hay que mostrarlo en la zona de datos
            for (var i = 0; i < data.d.length; i++) {
                var mu = new miniUnidad(data.d[i].Codigo, data.d[i].Nombre);
                v.push(mu);
                if (ano != null) {
                    if (data.d[i].Codigo == ano) {
                        vm.Ano(mu);
                    }
                }
            }
            vm.PosiblesAnos(v);
        },
        error: errorAjax
    });
}

function changeComboAno() {
    var mf = function () { };
    return mf;
}