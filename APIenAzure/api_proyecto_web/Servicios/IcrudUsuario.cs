using api_proyecto_web.Modelos;

namespace api_proyecto_web.Servicios
{
    public interface IcrudUsuario
    {
        public Usuario informacionUsuario(int id);
        public int InicioSesion(string correo, string contraseña);
        public int CrearUsuario(string nombre, string apellido, string telefono, string email, string direccion, string comuna, string contraseña);
        public void EditarUsuario(string nombre, string apellido, string telefono, string email, int id, string direccion);
        public void cambiofoto(IFormFile imagen, int id);
        public int inicio_trabajador(string correo, string contraseña);
    }

}
