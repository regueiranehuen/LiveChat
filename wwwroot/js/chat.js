"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build(); // Chathub mapeado en Program.cs. Creamos la conexion inicial al chathub



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


// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, sentAt) { // Al recibir el evento SendMessage, el cliente va a recibir el mensaje y agregarlo a la lista de mensajes
    var li = document.createElement("li"); // List
    document.getElementById("messagesList").appendChild(li); // Agregamos el mensaje a la lista de mensajes
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you
    // should be aware of possible script injection concerns.
    li.textContent = `${user}: at ${sentAt}: says ${message}`;
});

connection.start().then(function () {
    console.log("Conexión exitosa al Hub.");
    document.getElementById("sendButton").disabled = false; // Al habilitarse la conexión, habilitamos el botón de enviar
}).catch(function (err) {
    return console.error(err.toString());
});





document.getElementById("sendButton").addEventListener("click", function (event) { // OnClick event
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessage", user, message).catch(function (err) { // Invoke llama al método pasado como parámetro en el ChatHub
        return alert(err.toString());
    });
    event.preventDefault(); // Evita que la página se recargue
});
