using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json.Linq;
using MongoDB.Driver.Core.Connections;



namespace LiveChat
{
    

    public class UsuarioHub : Hub
    {

        private UsuarioRepository _usuarioRepository;

        public UsuarioHub(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }


        // Para guardar intentos de login de cada connectionId
        private static ConcurrentDictionary<string, int> Usuarios = new ConcurrentDictionary<string, int>();


        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            Usuarios[connectionId] = 0;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Usuarios.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }


        public async Task<string> RegistrarUsuario(string username, string password, string token)
        {
            var funcionesRegex = new FuncionesRegex();
            
            var usuarioExistente = await _usuarioRepository.ObtenerUsuarioPorUsername(username);

            bool captchaValido = await VerificarCaptcha(token);

            if (usuarioExistente != null)
            {
                return "El usuario ya existe";
            }
            else if (!funcionesRegex.UserValido(username) || !funcionesRegex.PasswordValida(password))
            {
                return "El nombre de usuario y/o la contraseña no son válidos";
            }
            
            else if (!captchaValido)
            {
                return "Complete el captcha";
            }
            var funcionesPasswords = new FuncionesPasswords();

            string salt = funcionesPasswords.GenerarSalt(16);
            string passwordHasheada = funcionesPasswords.HashearPassword(password, salt);


            var nuevoUsuario = new Usuario(username, passwordHasheada, salt);
            await _usuarioRepository.CrearUsuario(nuevoUsuario);
            return "Usuario registrado con éxito"; 
        }


        private async Task<bool>VerificarCaptcha(string token)
        {
            string? claveSecreta = Environment.GetEnvironmentVariable("SecretKeyRecaptchaLiveChat");


            string url = $"https://www.google.com/recaptcha/api/siteverify?secret={claveSecreta}&response={token}";
            
            using (HttpClient cliente = new HttpClient())
            {
                string rta = await cliente.GetStringAsync(url);
                JObject json = JObject.Parse(rta);

                return (bool)json["success"];
            }
        }


        public async Task<string> IniciarSesion(string username, string password, string token, string connectionId)
        {
            if (!Usuarios.ContainsKey(connectionId))
            {
                return "Connection id inválido";
            }

            if (Usuarios[connectionId] > 3 && await VerificarCaptcha(token)==false)
            {
               
                return "Verificar captcha"; // Verificar captcha
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorUsername(username);

            

            if (usuario == null)
            {
                Usuarios[connectionId]+=1;

                System.Diagnostics.Debug.WriteLine("Intentos del usuario " + connectionId + ":" + Usuarios[connectionId]);

                if (Usuarios[connectionId] == 3) // Luego de 3 intentos del usuario para loguearse, le arrojo el captcha
                {
                    return "Aparecer captcha";
                }

                return "El usuario no existe"; // Usuario no existe
            }
            var funcionesPasswords = new FuncionesPasswords();
            string passwordEscritaHasheada = funcionesPasswords.HashearPassword(password, usuario.Salt);

            if (usuario.PasswordHash != passwordEscritaHasheada)
            {
                Usuarios[connectionId] += 1;

                System.Diagnostics.Debug.WriteLine("Intentos del usuario " + connectionId + ":" + Usuarios[connectionId]);
                    
                if (Usuarios[connectionId] == 3) // Luego de 3 intentos del usuario para loguearse, le arrojo el captcha
                {
                    return "Aparecer captcha";
                }
                return "La contraseña es incorrecta"; // Contraseña incorrecta
            }


            if (Usuarios[connectionId] > 3)
            {
                if (await VerificarCaptcha(token))
                {
                    return "Inicio de sesión exitoso"; // Inicio de sesión exitoso
                }
            }
            return "Inicio de sesión exitoso"; // Inicio de sesión exitoso
        }


        


    }

    public class FuncionesPasswords
    {
        

        public string GenerarSalt(int size)
        {
            if (size <= 0)
                throw new ArgumentException("El tamaño del salt debe ser mayor que 0.");

            byte[] saltBytes = new byte[size];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Convertir a string en formato Base64
            return Convert.ToBase64String(saltBytes);
        }

        public string HashearPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }

    public class FuncionesRegex
    {

        public bool UserValido(string username)
        {
            string regex = @"^[a-zA-Z0-9_-]{4,}$"; // Al menos 4 caracteres, letras del abecedario (sin la ñ), números y guiones permitidos
            if (Regex.IsMatch(username, regex))
            {
                return true;
            }
            return false;
        }

        public bool PasswordValida(string password)
        {
            string regex = @"^.{8,}$"; // Al menos 8 caracteres
            if (Regex.IsMatch(password, regex))
            {
                return true;
            }
            return false;
        }
    }
    


    }
