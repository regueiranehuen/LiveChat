using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveChat
{
    public class UsuarioHub : Hub
    {

        private UsuarioRepository _usuarioRepository;

        public UsuarioHub(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> RegistrarUsuario(string username, string password)
        {
            var funcionesPasswords = new FuncionesPasswords();
            var usuarioExistente = await _usuarioRepository.ObtenerUsuarioPorUsername(username);
            if (usuarioExistente != null && !funcionesPasswords.PasswordValida(password))
            {
                return false; // El usuario ya existe // la contraseña no es valida
            }
            string salt = funcionesPasswords.GenerarSalt(16);
            string passwordHasheada = funcionesPasswords.HashearPassword(password, salt);


            var nuevoUsuario = new Usuario(username, passwordHasheada, salt);
            await _usuarioRepository.CrearUsuario(nuevoUsuario);
            return true; // Usuario registrado correctamente
        }


        public async Task<bool> IniciarSesion(string username, string passwordHash)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorUsername(username);
            if (usuario == null || usuario.PasswordHash != passwordHash)
            {
                return false; // Usuario no existe o contraseña incorrecta
            }

            return true; // Inicio de sesión exitoso
        }
    }

    public class FuncionesPasswords
    {
        public bool PasswordValida(string password)
        {
            string regex = @"^[a-zA-Z]{8,}$"; // Al menos 8 caracteres
            if (Regex.IsMatch(password, regex))
            {
                return true;
            }
            return false;
        }

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

}
