﻿"use strict";

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

    // Invocar el método y manejarlo correctamente con `.then`
    connection.invoke("ObtenerConversacionesPorUsuario", usuario)
        .then(conversaciones => {
            let listaMensajes = document.getElementById("listaMensajes");
            listaMensajes.innerHTML = ""; // Limpiar la lista antes de agregar nuevos elementos

            // Iterar sobre la lista de conversaciones obtenida
            conversaciones.forEach(conversacion => {
                let li = document.createElement("li");

                // Obtener el último mensaje de cada conversación

                li.textContent = conversacion.ultimoMensaje.emisor + ": " + conversacion.ultimoMensaje.texto + " " + conversacion.ultimoMensaje.fecha;
                // Agregar un event listener al li
                li.addEventListener("click", function () {
                    
                    alert(`Clickeaste en la conversación con id ${conversacion.id}`);
                    alert(`usuario: ${usuario}`)
                    // Reemplazar el placeholder con el valor real del usuario
                    var url = urlTemplate.replace('__conversacion__', encodeURIComponent(conversacion.id)).replace('__usuario__', encodeURIComponent(usuario));
                    // Redirigir
                    window.location.href = url;

                });
                listaMensajes.appendChild(li);
            })
                .catch(function (err) {
                    console.error("Error al obtener el último mensaje:", err.toString());
                });
        })

        .catch(function (err) {
            console.error("Error al obtener conversaciones:", err.toString());
        });

}).catch(function (err) {
    console.error("Error al conectar con el Hub:", err.toString());
});



