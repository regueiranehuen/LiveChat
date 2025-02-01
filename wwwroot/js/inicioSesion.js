var connection = new signalR.HubConnectionBuilder().withUrl("/usuariohub").build();

document.getElementById("btnInicioSesion").disabled = true;

// Inicia la conexión
connection.start()
    .then(function () {
        console.log("Conexión a SignalR establecida con éxito.");
        // Ahora la conexión está lista, podemos registrar los eventos
        document.getElementById("btnInicioSesion").disabled = false;
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



document.getElementById("btnInicioSesion").addEventListener("click", async function () {
    var user = document.getElementsByName("username")[0].value;
    var password = document.getElementsByName("password")[0].value;
    alert(`user: ${user}`);
    try {
        var usuarioRegistrado = await connection.invoke("IniciarSesion", user, password);
        if (usuarioRegistrado) {

            // Reemplazar el placeholder con el valor real del usuario
            var url = urlTemplate.replace('__usuario__', encodeURIComponent(user));

            alert(`Redirigiendo a url: ${url}`);

            // Redirigir
            window.location.href = url;
        } else {
            alert("El usuario no existe // La contraseña es incorrecta");
        }
    } catch (error) {
        console.error('Error al iniciar sesión:', error);
        alert("Hubo un error al iniciar sesión");
    }
});


