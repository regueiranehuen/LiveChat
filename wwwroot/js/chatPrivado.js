﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build(); // Chathub mapeado en Program.cs. Creamos la conexion inicial al chathub



// Evento cuando la conexión se establece correctamente

connection.on("connected", function () {
    console.log("Conexión establecida con éxito.");
});


console.log("Suscribiendo evento RecibirMensaje...");

connection.on("RecibirMensaje", function (mensaje) { // Al recibir el evento SendMessage, el cliente va a recibir el mensaje y agregarlo a la lista de mensajes

   
    let listaMensajes = document.getElementById("listaMensajes");

    

    var li = document.createElement("li");
    li.id = mensaje.idConversacion;
    console.log(mensaje.idConversacion);
    li.textContent = `(${mensaje.fecha}) ${mensaje.emisor}: ${mensaje.texto}`;
    listaMensajes.appendChild(li);


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
            if (!mensajes) {
                console.log("Buen intento Alan Turing");
                return; 
            }
            let listaMensajes = document.getElementById("listaMensajes");
            listaMensajes.innerHTML = ""; // Limpiar la lista antes de agregar nuevos elementos

            // Iterar sobre la lista de conversaciones obtenida
            mensajes.forEach(mensaje => {
                let li = document.createElement("li");
                li.id = idConversacion;
                console.log(idConversacion);
                // Obtener el último mensaje de cada conversación

                li.textContent = `(${mensaje.fecha}) ${mensaje.emisor}: ${mensaje.texto}`;
                // Agregar un event listener al li
               
                listaMensajes.appendChild(li);
            })

        })
    

}).catch(function (err) {
    console.error("Error al conectar con el Hub:", err.toString());
});




document.getElementById("btnEnviarMensaje").addEventListener("click", enviarMensaje);

document.getElementById("inputMensaje").addEventListener('keydown', async function (evento) {
    if (evento.key === 'Enter') {
        await enviarMensaje();
    }
});


async function enviarMensaje() {

    var textoMensaje = document.getElementById("inputMensaje").value;
    console.log(idConversacion);
    console.log(textoMensaje);
    try {
        let listaMensajes = document.getElementById("listaMensajes");
        let mensaje = await connection.invoke("EnviarMensaje", idConversacion, textoMensaje, usuarioRegistrado, usuario2);

        let li = document.createElement("li");
        li.id = idConversacion;

        li.textContent = `(${mensaje.fecha}) ${mensaje.emisor}: ${mensaje.texto}`;
        // Agregar un event listener al li

        listaMensajes.appendChild(li);

        document.getElementById("inputMensaje").value = "";

    } catch (error) {
        console.error('Error al enviar mensaje:', error);
        alert("Hubo un error al enviar mensaje");
    }
}


