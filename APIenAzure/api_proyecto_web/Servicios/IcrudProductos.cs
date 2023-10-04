using api_proyecto_web.Modelos;
using api_proyecto_web.Modelos.@enum;

namespace api_proyecto_web.Servicios
{
    public interface IcrudProductos
    {
        //POST
        public void GenerarProducto(int tipo_Producto, string nombre, string caracteristicas, int precio,IFormFile imagen1, IFormFile imagen2, IFormFile imagen3, IFormFile imagen4, IFormFile imagen5);

        //GET 
        public Productos InformacionProducto(int id);

        public List<Productos> productoMasivo();
         
        
        //GET
        public List<Productos> ObtenerTodosLosProductos();

        //GET
        public Productos BuscarProducto(string nombre);




    }
}
