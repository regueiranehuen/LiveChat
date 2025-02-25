"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/conversacionhub").build(); // Chathub mapeado en Program.cs. Creamos la conexion inicial al chathub



// Evento cuando la conexión se establece correctamente
connection.on("connected", function () {
    console.log("Conexión establecida con éxito.");
});

connection.on("RecibirMensaje", function (mensaje) { // Al recibir el evento SendMessage, el cliente va a recibir el mensaje y agregarlo a la lista de mensajes
    let listaMensajes = document.getElementById("listaMensajes");
    


    let conversacionExistente = document.getElementById(mensaje.idConversacion);
    if (conversacionExistente) {
        console.log("Actualizando conversacion existente con ID:", mensaje.idConversacion);
        conversacionExistente.textContent = `(${mensaje.fecha}) ${mensaje.emisor}: ${mensaje.texto}`;
    } else {
        console.log("Creando nueva conversacion con ID:", mensaje.idConversacion);

        var li = document.createElement("li");
        li.id = mensaje.idConversacion; // Asignar ID
        li.textContent = `(${mensaje.fecha}) ${mensaje.emisor}: ${mensaje.texto}`;

        console.log("Elemento creado:", li); // Verificar si tiene ID
        // Agregar un event listener al li
        li.addEventListener("click", function () {

            // Reemplazar el placeholder con el valor real del usuario
            var url = urlTemplate.replace('__conversacion__', encodeURIComponent(conversacion.id)).replace('__usuario__', encodeURIComponent(usuario));
            // Redirigir
            window.location.href = url;

        });
        listaMensajes.appendChild(li);
    }


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
                console.log(conversacion);

                li.id = conversacion.id;

                // Obtener el último mensaje de cada conversación
                if (conversacion.ultimoMensaje) {
                    li.textContent = `(${conversacion.ultimoMensaje.fecha}) ${conversacion.ultimoMensaje.emisor}: ${conversacion.ultimoMensaje.texto}`;
                } else {
                    li.textContent = conversacion.ultimoMensaje.emisor;
                }
                
                // Agregar un event listener al li
                li.addEventListener("click", function () {
                    
                   
                    // Reemplazar el placeholder con el valor real del usuario
                    var url = urlTemplate.replace('__conversacion__', encodeURIComponent(conversacion.id)).replace('__usuario__', encodeURIComponent(usuario));
                    // Redirigir
                    window.location.href = url;

                });
                listaMensajes.appendChild(li);
            })

        })

      

}).catch(function (err) {
    console.error("Error al conectar con el Hub:", err.toString());
});

document.getElementById("btnIniciarConversacion").addEventListener("click", async function () {
    var usuarioBuscado = document.getElementById("txtUsuarioBuscado").value;

    try {
        // Esperamos la respuesta de la invocación para saber si existe la conversación
        var conversacion = await connection.invoke("ExisteConversacion", usuario, usuarioBuscado);

        if (conversacion != null && conversacion != undefined) {
            alert("Ya existe una conversación con ese usuario. Serás redirigido allí");
            var url = urlTemplate.replace('__conversacion__', encodeURIComponent(conversacion.id)).replace('__usuario__', encodeURIComponent(usuario));
            // Redirigir
            window.location.href = url;
        } else {
            // Si no existe, creamos la conversación
            var conversacionCreada = await connection.invoke("CrearConversacion", usuario, usuarioBuscado);

            if (conversacionCreada != null && conversacionCreada != undefined) {
                let idConversacion = conversacionCreada.id || conversacionCreada._id;

                if (idConversacion) {
                    var url = urlTemplate.replace('__conversacion__', encodeURIComponent(idConversacion)).replace('__usuario__', encodeURIComponent(usuario));
                    // Redirigir
                    window.location.href = url;
                    alert("Conversación creada con éxito. Serás redirigido allí");
                } else {
                    alert("Error: La conversación no tiene un ID válido.");
                }
            } else {
                alert("Error al crear la conversación: No se encontró al usuario");
            }
        }
    } catch (err) {
        console.error("Error al iniciar la conversación:", err.message);
    }
});




