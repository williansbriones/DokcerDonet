using api_proyecto_web.DBConText;
using api_proyecto_web.Modelos;
using api_proyecto_web.Servicios;
using api_proyecto_web.Servicios.Implementacion;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_proyecto_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller_usuario : ControllerBase
    {
        Servicios.IcrudUsuario usuario = new Servicios.Implementacion.UsuarioServicio();


        // GET: api/Controller_usuario/InformacionUsuario}
        //terminado
        [HttpGet("InformacionUsuario")] //controller lo unico que realiza en la obtencion de datos
        public Usuario Get([FromForm]int id)
        {
            return usuario.informacionUsuario(id);
        }

        // POST api/Controller_usuario/Inicio_Sesion
        //terminado
        [HttpPost("Inicio_Sesion")]
        public ObjectResult Post([FromBody]Usuario datos)
        {
            string email = datos.Email;
            string contraseña = datos.Contraseña;
            try
            {
                return Ok(usuario.InicioSesion(email, contraseña));
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/Controller_usuario/CrearUsuario
        //Terminado
        [HttpPost("CrearUsuario")]
        public int CrearUsuario([FromBody]Usuario Usu)
        {
            string nombre = Usu.Nombre;
            string apellidos = Usu.Apellido;
            string telefono = Usu.telefono;
            string email = Usu.Email;
            string direccion = Usu.Direccion;
            string comuna = Usu.Comuna;
            string contraseña = Usu.Contraseña;
            int id = usuario.CrearUsuario(nombre, apellidos, telefono, email, direccion, comuna, contraseña);
            return id;
        }
        //PUT api/Controller_usuario/EditarInformacion
        //terminado
        [HttpPut("EditarInformacion")]
        public ObjectResult EditarUsuario([FromBody]Usuario usu)
        {
            try
            {
                string nombre = usu.Nombre;
                string apellido = usu.Apellido;
                string telefono = usu.telefono;
                string email = usu.Email;
                int id = usu.Id;
                string direccion = usu.Direccion;
                usuario.EditarUsuario(nombre, apellido, telefono, email, id, direccion);
                return Ok("Actualizado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // api/Controller_usuario/CambioDeFoto
        //Terminado
        [HttpPost("CambioDeFoto")]
        public void cambioImagen([FromForm]int id, [FromForm] IFormFile imagen)
        {
            usuario.cambiofoto(imagen, id);
        }
        // api/Controller_usuario/inicio_trabajador
        //
        [HttpPost("inicio_trabajador")]
        public int inicio_trabajador  ([FromBody] Usuario us)
        {
            string email = us.Email;
            string contraseña = us.Contraseña;
            int id = usuario.inicio_trabajador(email,contraseña);
            return id;
            
        }
    }
   
}
