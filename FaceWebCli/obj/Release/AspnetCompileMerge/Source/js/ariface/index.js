// de blank_ (pruebas)
var usuario;

function initForm() {
    usuario = comprobarLogin();
    comprobarClientePuntos(usuario.CodClienAriges);
    // de smart admin
    pageSetUp();
    //
    getParametros();
    
}

