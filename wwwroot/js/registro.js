"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/usuariohub").build();

// Evento cuando la conexión se establece correctamente
connection.on("connected", function () {
    console.log("Conexión establecida con éxito.");
});

connection.start().catch(function (err) {
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

document.getElementById("btnRegistro").addEventListener("click", function () {
    var user = document.getElementsByName("username")[0].value;
    var password = document.getElementsByName("password")[0].value;

    connection.invoke("RegistrarUsuario", user, password)
        .then(function () {
            alert("Usuario registrado con éxito");
        })
        .catch(function (err) {
            console.error(err.toString());
            alert("El usuario ya existe // Contraseña no válida");
        });
});
