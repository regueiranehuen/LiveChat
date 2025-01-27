"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/usuariohub").build();

document.getElementById("btnRegistro").disabled = true;

// Inicia la conexión
connection.start()
    .then(function () {
        console.log("Conexión a SignalR establecida con éxito.");
        // Ahora la conexión está lista, podemos registrar los eventos
        document.getElementById("btnRegistro").disabled = false;
        // Evento cuando la conexión se establece correctamente
        connection.on("connected", function () {
            alert("Conexión establecida con éxito.");
            //document.getElementById("btnRegistro").disabled = false; // Habilitar el boton de registro tras establecer conexión con SignalR
        });

        // Evento cuando la conexión se cierra
        connection.onclose(function (error) {
            if (error) {
                console.log("La conexión se cerró con un error:", error);
                

            } else {
                console.log("La conexión se cerró de manera limpia.");
            }
            document.getElementById("btnRegistro").disabled = true;
        });

        // Evento cuando la conexión se restablece después de una desconexión
        connection.onreconnected(function (connectionId) {
            console.log("La conexión se restableció con el ID de conexión:", connectionId);
        });

        // Evento cuando la reconexión falla
        connection.onreconnecting(function (error) {
            console.log("Reintentando la conexión debido a un error:", error);
        });
    })
    .catch(function (err) {
        console.error("Error al conectar a SignalR: ", err);
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

//document.getElementById("btnRegistro").disabled = true;


document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("checkboxMostrarContraseña").addEventListener("click", function (event) {
        var passwordFields = document.getElementsByName("password");

        passwordFields.forEach(function (passwordField) {
            if (passwordField.type === "password") {
                passwordField.type = "text"; // Muestra la contraseña
            } else {
                passwordField.type = "password"; // Oculta la contraseña
            }
        });
    });
});

document.getElementById("btnRegistro").addEventListener("click", async function () {
    var user = document.getElementsByName("username")[0].value;
    var password = document.getElementsByName("password")[0].value;
    try {
        var usuarioRegistrado = await connection.invoke("RegistrarUsuario", user, password);

        if (usuarioRegistrado) {
            alert("Usuario registrado con éxito");
        } else {
            alert("El usuario ya existe o la contraseña no es válida");
        }
    } catch (error) {
        console.error('Error al registrar usuario:', error);
        alert("Hubo un error al registrar el usuario");
    }
});