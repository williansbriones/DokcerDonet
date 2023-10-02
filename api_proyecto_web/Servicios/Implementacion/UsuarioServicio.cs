using api_proyecto_web.DBConText;
using api_proyecto_web.Modelos;
using api_proyecto_web.Modelos.@enum;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Data;

namespace api_proyecto_web.Servicios.Implementacion
{
    public class UsuarioServicio : IcrudUsuario
    {

       
        static connnecionBlob conBlob = new connnecionBlob();

        public static Usuario UsuarioIniciado = UsuarioIniciado !=null ? UsuarioIniciado : new Usuario() ;
        
        public Usuario informacionUsuario(int id)//metodo que entrega la informacion del usuario que se encuentre iniciado 
        {
            Usuario Datos = new Usuario();//variable que entregara los datos al usuario iniciado
            //quey que valida si existe algun usuario con la contraseña y correo ingresados
            string Query = string.Format("SELECT id_usuario AS id_usuario, nombre AS nombre , appaterno||' '||apmaterno AS apellidos, id_tipo_usuario as tipo_usuario, telefono as telefono, email as email, direccion as direccion, comuna as comuna, contraseña as contraseña FROM usuario where id_usuario =" + id);
            DataTable dt1 = ComprasServicios.db.Execute(Query);
            string query_imagen = "select id_imagen as id_imagen, url_imagen as url, id_imagen as id_imagen, nombre as nombre from imagen where estado = True and id_usuario = " + Convert.ToInt32(dt1.Rows[0]["id_usuario"]);
            DataTable dt_imagen = ComprasServicios.db.Execute(query_imagen);
            if (dt1.Rows.Count > 0)//validador de que exista informacion
            {
                string apellido_completo = dt1.Rows[0]["apellidos"].ToString();
                //ingreso de informacion en el objeto datos
                Datos.Id = Convert.ToInt32(dt1.Rows[0]["id_usuario"]);
                Datos.Nombre = dt1.Rows[0]["nombre"].ToString();
                Datos.Apellido = dt1.Rows[0]["apellidos"].ToString() == " " ? "" : apellido_completo;
                Datos.tipo_Usuario = (Tipo_usuario)Convert.ToInt32(dt1.Rows[0]["tipo_usuario"]);
                Datos.telefono = dt1.Rows[0]["telefono"].ToString();
                Datos.Email = dt1.Rows[0]["email"].ToString();
                Datos.Direccion = dt1.Rows[0]["direccion"].ToString();
                Datos.Comuna = dt1.Rows[0]["comuna"].ToString();

            

            //ingreso de informacion en el usuario iniciado
            Datos.Foto_perfil.id = Convert.ToInt32(dt_imagen.Rows[0]["id_imagen"]);
            Datos.Foto_perfil.URL = dt_imagen.Rows[0]["url"].ToString();
            Datos.Foto_perfil.IdTipoClase = 1;
            Datos.Foto_perfil.Nombre = dt_imagen.Rows[0]["nombre"].ToString();
            return Datos;
            }
            else
            {
                throw new Exception();
            }
        }
        public int InicioSesion(string correo, string contraseña) //metodod que inicia sesion del usuario
        {
            Usuario Datos = new Usuario();//variable que entregara los datos al usuario iniciado
            //quey que valida si existe algun usuario con la contraseña y correo ingresados
            string Query = string.Format("SELECT id_usuario AS id_usuario, nombre AS nombre , appaterno||' '||apmaterno AS apellidos, id_tipo_usuario as tipo_usuario, telefono as telefono, email as email, direccion as direccion, comuna as comuna, contraseña as contraseña FROM usuario where email ='"+correo +"'and contraseña ='"+contraseña+ "' and id_tipo_usuario = 1");
            DataTable dt1 = ComprasServicios.db.Execute(Query);
            if (dt1.Rows.Count > 0)//validador de que exista informacion
            {
                //ingreso de informacion en el objeto datos
                Datos.Id            = Convert.ToInt32(dt1.Rows[0]["id_usuario"]);

                return Datos.Id;

            }
            else
            {
                throw new Exception();
            }
        }
        public int CrearUsuario(string nombre, string apellidos, string telefono, string email, string direccion, string comuna, string contraseña) // metodo que permite crear un usuario
        {
            //variables que perimitiran saber si tiene uno o dos apellidos
            string primer_apellido = "";
            string segundo_apellido = "";
            int indice_espacio = (apellidos.IndexOf(" "));
            int largoApellido = (apellidos.Length);

            if (indice_espacio > 0)//validador para saber si tiene uno o dos apellidos
            {
                primer_apellido = apellidos.Substring(0, (indice_espacio));
                segundo_apellido = apellidos.Substring((indice_espacio + 1), (largoApellido - (indice_espacio + 1)));
            }
            else//en el caso de tener un apellido setea el apellido paterno 
            {
                primer_apellido = apellidos;
            }
            //ingreso de datos del usuario registrado a la base de datos
            string Obtencion_id = "select id_usuario() as id";
            DataTable id_tb = ComprasServicios.db.Execute(Obtencion_id);
            string QueryCreacionUsuario = "INSERT INTO usuario VALUES ("+ id_tb.Rows[0]["id"] +",'" + nombre + "' ,'" + primer_apellido + "','" + segundo_apellido + "',1,'" + telefono + "','" + email + "','" + direccion + "','" + comuna + "','" + contraseña + "')";
            ComprasServicios.db.Execute(QueryCreacionUsuario);
            ComprasServicios.db.Execute("commit");
            string Query_imagen_us = "insert into imagen values ( id_imagen(),'https://img.freepik.com/vector-premium/linda-imagen-vectorial-dibujos-animados-estrellas-brillantes-amarillas_423491-67.jpg?w=2000',1," + id_tb.Rows[0]["id"]+",null,to_char(" + id_tb.Rows[0]["id"] + "),'True')";
            ComprasServicios.db.Execute(Query_imagen_us);
            ComprasServicios.db.Execute("commit");
            int id = Convert.ToInt32(id_tb.Rows[0]["id"]);
            return id;
        }
        public void EditarUsuario(string nombre, string apellido, string telefono, string email, int id, string direccion)
        {
            if (id == 0) //codigo que genera un usuario el cual se incia en caso de no tener un usuario iniciado
            {
                string query = "SELECT SQ_id_usuario.nextval as numero from dual ";
                DataTable dt_id_nuevo_usuario = new DataTable();
                dt_id_nuevo_usuario = ComprasServicios.db.Execute(query);
                string query_ingreso_usuario = "INSERT INTO usuario VALUES(" + dt_id_nuevo_usuario.Rows[0]["numero"] + ", '', '', '', 0, '" + dt_id_nuevo_usuario.Rows[0]["numero"] + "', '" + dt_id_nuevo_usuario.Rows[0]["numero"] + "', '', '', '')";
                ComprasServicios.db.Execute(query_ingreso_usuario);
                ComprasServicios.db.Execute("commit");
                UsuarioIniciado.Id = Convert.ToInt32(dt_id_nuevo_usuario.Rows[0]["numero"]);
            }
            //edita la informacion del usuaria en caso de que se le indique en algun parametro
            string nombre_str = nombre ;
            string apellido_str = apellido;
            string telefono_str = telefono;
            string email_str = email;
            string direccion_str = direccion;
            string primer_apellido = "";
            string segundo_apellido = "";
            //apellidos 
            int indice_espacio = (apellido_str.IndexOf(" "));
            int largoApellido = (apellido_str.Length);
            //separa apellidos segun indique en algun caso
            if (indice_espacio > 0)
            {
                primer_apellido = apellido_str.Substring(0, (indice_espacio));
                segundo_apellido = apellido_str.Substring((indice_espacio + 1), (largoApellido - (indice_espacio + 1)));
            }
            else
            {
                primer_apellido = apellido_str;
            }
            //valida si  hay algun usuario que tenga el mismo correo y telefono
            string queryValidarUsuario = "select * from usuario where email = '"+email_str+"' or telefono = '"+telefono_str+"'";
            DataTable dtValidadorUsuario = new DataTable();
            //esta query valida si el usuario mantiene el mismo correo o no
            string query_validar2 = "select * from usuario where (email = '" + email_str + "' or telefono = '" + telefono_str + "') and id_usuario = "+id;
            //ejecucion de querys
            DataTable dtValidador2 = ComprasServicios.db.Execute(query_validar2);
            dtValidadorUsuario = ComprasServicios.db.Execute(queryValidarUsuario);
            //actualiza los datos del usuario
            if (dtValidadorUsuario.Rows.Count == 0 || dtValidador2.Rows.Count == 1) 
            { 
                string QueryInsert = "UPDATE usuario SET nombre= '" + nombre_str+"', appaterno = '"+primer_apellido+"', apmaterno = '"+segundo_apellido+"', telefono = '"+telefono_str+"', email= '"+email_str+ "', direccion = '"+direccion_str+"' WHERE id_usuario = " + id; 
                ComprasServicios.db.Execute(QueryInsert);
                ComprasServicios.db.Execute("commit");
            }
            else
            {
                Console.WriteLine("No se ingresaron los datos de usuario ya que hay datos repetidos");
            }
        }
        public void cambiofoto(IFormFile imagen, int id)
        {
            string url;

            string Query = string.Format("SELECT id_usuario AS id_usuario, nombre AS nombre , appaterno||' '||apmaterno AS apellidos, id_tipo_usuario as tipo_usuario, telefono as telefono, email as email, direccion as direccion, comuna as comuna, contraseña as contraseña FROM usuario where id_usuario =" + id);
            DataTable dt1 = ComprasServicios.db.Execute(Query);

            if ((Tipo_usuario)Convert.ToInt32(dt1.Rows[0]["tipo_usuario"]) == (Tipo_usuario)1)
            {
                url = conBlob.IngresoImagenUsuario(imagen);
                if (url != null)
                {
                    string query_id_imagen = "select id_imagen() as id ";
                    DataTable dt_id = ComprasServicios.db.Execute(query_id_imagen);
                    string query_desactivar_imagen = "update imagen set estado = False where id_usuario = " + id;
                    string query_cambio_foto = "insert into imagen values (" + dt_id.Rows[0]["id"] + ",'" + url + "',1," + id + ",null,'cambio de foto',True)";
                    ComprasServicios.db.Execute(query_desactivar_imagen);
                    ComprasServicios.db.Execute(query_cambio_foto);
                    ComprasServicios.db.Execute("commit");
                    UsuarioIniciado.Foto_perfil.URL = url;
                    UsuarioIniciado.Foto_perfil.Nombre = url;
                    UsuarioIniciado.Foto_perfil.id = Convert.ToInt32(dt_id.Rows[0]["id"]);
                    UsuarioIniciado.Foto_perfil.IdTipoClase = 1;

                }
            }
        }
        public int inicio_trabajador(string correo, string contraseña)
        {
            string query = "select id_usuario from usuario  where email = '" + correo + "' and contraseña = '" + contraseña +"' and id_tipo_usuario = 2 ";
            DataTable dt_id = ComprasServicios.db.Execute(query);
            int id = Convert.ToInt32(dt_id.Rows[0]["id_usuario"]);
            return id ;
        }
    }
}
