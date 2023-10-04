using api_proyecto_web.Modelos;
using api_proyecto_web.Servicios;
using api_proyecto_web.Servicios.Implementacion;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_proyecto_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerCompra : ControllerBase
    {
        IcrudCompras<compras> servicio_compras = new ComprasServicios();

        // GET: api/<ControllerCompra>
        [HttpGet("CarrodeCompra")]
        public ObjectResult CarrodeCompra([FromForm] int id_compra)
        {
            int id = id_compra;
            try
            {
                return Ok(servicio_compras.BusquedaCarroCompras(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ingreso_Producto")]
        public int Post([FromBody] JsonElement adicion)
        {


            Console.WriteLine(adicion.GetProperty("id"));
            string id_str = adicion.GetProperty("id").ToString();
            string id_producto_str = adicion.GetProperty("id_producto").ToString();
            string cantidad_str = adicion.GetProperty("cantidad").ToString();
            int id = Convert.ToInt32(id_str);
            int id_producto = Convert.ToInt32(id_producto_str);
            int cantidad = Convert.ToInt32(cantidad_str);
            return servicio_compras.Agregarproducto(id_producto, cantidad, id);


        }
        // PUT api/Controller_compras/EliminarProducto
        [HttpPut("EliminarProducto")]
        public void EliminarProducto([FromBody] JsonElement disminucion)
        {
            string id_producto_str = disminucion.GetProperty("id_producto").ToString();
            string cantidad_str = disminucion.GetProperty("cantidad").ToString();
            string id_str = disminucion.GetProperty("id").ToString();
            int id_producto = Convert.ToInt32(id_producto_str);
            int cantidad = Convert.ToInt32(cantidad_str);
            int id = Convert.ToInt32(id_str);
            servicio_compras.EliminarProducto(id_producto, cantidad, id);
        }
        [HttpPost("ConfimacionDeCompra")]
        public ObjectResult ConfirmaciondeCompra([FromBody] JsonElement confirmacion)
        {
            try
            {
                string id_usuario_str = confirmacion.GetProperty("id_usuario").ToString();
                string id_compra_str = confirmacion.GetProperty("id_compra").ToString();
                int id_compra = Convert.ToInt32(id_compra_str);
                int id_usuario = Convert.ToInt32(id_usuario_str);
                servicio_compras.ConfirmarCompra(id_compra, id_usuario);
                return Ok(id_compra);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Ordenes_de_clientes/{id_cliente}", Name = "GetCompras")]//Consulta de ordenes de compra por cliente
        public IList<compras> GetCompras(int id_cliente)
        {
            return servicio_compras.BusquedaComprasCliente(id_cliente);
        }

    }
}
