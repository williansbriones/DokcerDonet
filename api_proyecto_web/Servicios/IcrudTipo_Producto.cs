namespace api_proyecto_web.Servicios
{
    public interface IcrudTipo_Producto<P>
    {
        public void AgregaTipoProducto(int id_tipo_producto,string nombre);

        public IList<P> BusquedaTipoProducto(int id_tipo_producto);


        
        //public void FiltrarTipoProducto();
    }
}
