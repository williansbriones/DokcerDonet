using api_proyecto_web.Modelos;
using System.Security.Cryptography.X509Certificates;

namespace api_proyecto_web.Servicios
{
    public interface IcrudCompras<Compras>
    {
        //Crud
        public IList<Compras> BusquedaComprasCliente(int id_cliente);
        public Compras BusquedaCompraIndividual(int id_compra);
        public IList<Compras> BusquedaComprasClienteIniciado(int id);
        public int Agregarproducto(int id_producto, int cantidad, int id);
        public void ConfirmarCompra(int id_compra, int id_usuario);
        public void EliminarProducto(int id_producto, int cantidad, int id);
        public void IngresoCupon(string codigo_cupon, int id_compra);
        public Compras BusquedaCarroCompras(int id);
    }
}