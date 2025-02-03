"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/conversacionhub").build(); // Chathub mapeado en Program.cs. Creamos la conexion inicial al chathub



// Evento cuando la conexión se establece correctamente
connection.on("connected", function () {
    console.log("Conexión establecida con éxito.");
});

// Evento cuando la conexión se cierra
connection.onclose(function (error) {
    if (error) {
        console.log("La conexión se cerró con un error:", error);
    } else {
        console.log("La conexión se cerró de manera limpia.");
    }
});

// Evento cuando la conexión se reestablece después de una desconexión
connection.onreconnected(function (connectionId) {
    console.log("La conexión se restableció con el ID de conexión:", connectionId);
});

// Evento cuando la reconexión falla
connection.onreconnecting(function (error) {
    console.log("Reintentando la conexión debido a un error:", error);
});

connection.start().then(function () {
    console.log("Conexión exitosa al Hub.");
    
    let conversaciones = connection.invoke("ObtenerConversacionesPorUsuario",usuario).catch(function (err) {
        return console.error(err.toString());
    });
    conversaciones.forEach(conversacion => {
        let li = document.createElement("li");
        document.getElementById("listaMensajes").appendChild(li);
        li.textContent = connection.invoke("ObtenerUltimoMensajeConversacion",conversacion);
    });
}).catch(function (err) {
    return console.error(err.toString());
});


