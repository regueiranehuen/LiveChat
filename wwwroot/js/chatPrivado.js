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

    document.getElementById("h1Usuario2").textContent = "Bienvenido al chat con " + usuario2;

    connection.invoke("ObtenerMensajesDeConversacion", idConversacion)
        .then(mensajes => {
            let listaMensajes = document.getElementById("listaMensajes");
            listaMensajes.innerHTML = ""; // Limpiar la lista antes de agregar nuevos elementos

            // Iterar sobre la lista de conversaciones obtenida
            mensajes.forEach(mensaje => {
                let li = document.createElement("li");

                // Obtener el último mensaje de cada conversación

                li.textContent = mensaje.emisor + ": " + mensaje.texto + " " + mensaje.fecha;
                // Agregar un event listener al li
               
                listaMensajes.appendChild(li);
            })
                .catch(function (err) {
                    console.error("Error al obtener el último mensaje:", err.toString());
                });
        })
    

}).catch(function (err) {
    console.error("Error al conectar con el Hub:", err.toString());
});


