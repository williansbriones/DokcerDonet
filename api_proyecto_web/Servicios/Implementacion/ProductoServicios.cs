using api_proyecto_web.DBConText;
using api_proyecto_web.Modelos;
using api_proyecto_web.Modelos.@enum;
using System;
using System.Data;
namespace api_proyecto_web.Servicios.Implementacion
{

    public class ProductoServicios : IcrudProductos
    {

        static Productos producto1 = new Productos();
        static connnecionBlob conBlob = new connnecionBlob();



        public void GenerarProducto(int id_tipo_Producto, string nombre, string caracteristicas, int precio, IFormFile imagen1, IFormFile imagen2, IFormFile imagen3, IFormFile imagen4, IFormFile imagen5)
        {
            string query_id = "select id_producto() as id";
            DataTable dt_id = ComprasServicios.db.Execute(query_id);
            Productos productito = new Productos();
            string query1 = string.Format("insert into producto values (" + dt_id.Rows[0]["id"] + ",'{0}','{1}',{2},{3}) ", nombre, caracteristicas, id_tipo_Producto, precio);
            DataTable dt1 = ComprasServicios.db.Execute(query1);
            string commit1 = string.Format("COMMIT");
            DataTable cm1 = ComprasServicios.db.Execute(commit1);
            int productoId = Convert.ToInt32(dt_id.Rows[0]["id"]);

            if (imagen1 != null)
            {
                string url = conBlob.IngresoImagenProducto(imagen1);
                InsertarImagen(productoId, url, "1"); // Insertar imagen 1
            }
            if (imagen2 != null)
            {
                string url = conBlob.IngresoImagenProducto(imagen2);
                InsertarImagen(productoId, url, "2"); // Insertar imagen 2
            }
            if (imagen3 != null)
            {
                string url = conBlob.IngresoImagenProducto(imagen3);
                InsertarImagen(productoId, url, "3"); // Insertar imagen 3
            }
            if (imagen4 != null)
            {
                string url = conBlob.IngresoImagenProducto(imagen4);
                InsertarImagen(productoId, url, "4"); // Insertar imagen 4
            }
            if (imagen5 != null)
            {
                string url = conBlob.IngresoImagenProducto(imagen5);
                InsertarImagen(productoId, url, "5"); // Insertar imagen 5
            }
        }

        //revisar metodo "InsertarImagen" para ver si eliminarlo
        private void InsertarImagen(int productoId, string url, string orden)
        {
            string queryIdImagen = "SELECT id_imagen() as id";
            DataTable dtId = ComprasServicios.db.Execute(queryIdImagen);
            int imagenId = Convert.ToInt32(dtId.Rows[0]["id"]);

            string queryIngresoFoto = string.Format(" insert into imagen values ({0}, '{1}', 2,null,{2}, '{3}', True)", imagenId, url, productoId, orden.ToString());
            ComprasServicios.db.Execute(queryIngresoFoto);
            string commit1 = string.Format("COMMIT");
            DataTable cm1 = ComprasServicios.db.Execute(commit1);
        }
        public Productos InformacionProducto(int id)
        {
            string query1 = string.Format(" Select * from producto WHERE id_producto=" + id);
            DataTable dt1 = ComprasServicios.db.Execute(query1);
            Productos produc = new Productos();
            produc.Id = Convert.ToInt32(dt1.Rows[0]["id_producto"]);
            produc.nombre = dt1.Rows[0]["Nombre"].ToString();
            produc.caracteristicas = dt1.Rows[0]["caracteristicas"].ToString();
            produc.precio = Convert.ToInt32(dt1.Rows[0]["precio"]);
            produc.tipo_producto = (Tipo_Producto)Convert.ToInt32(dt1.Rows[0]["id_tipo_producto"]);
            string query_imagenes = "select id_imagen as id_img,id_tipo_imagen as tipo_img,i.nombre as nombre,url_imagen as url from producto p join imagen i on (p.id_producto = i.id_producto) where p.id_producto = " + produc.Id + " and i.estado = True";
            DataTable dt_imagenes = ComprasServicios.db.Execute(query_imagenes);
            if (dt_imagenes.Rows.Count > 0)
            {
                if (dt_imagenes.Rows.Count == 1)
                {
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                }
                else if (dt_imagenes.Rows.Count == 2)
                {
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                }
                else if (dt_imagenes.Rows.Count == 3)
                {
                    //primera imagen
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //segunda imagen
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    //tercera imagen
                    produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                    produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                    produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                    produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                }
                else if (dt_imagenes.Rows.Count == 4)
                {
                    //primera imagen
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //segunda imagen
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    //tercera imagen
                    produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                    produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                    produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                    produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                    //cuarta imagen
                    produc.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                    produc.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                    produc.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                    produc.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                }
                else
                {
                    //primera imagen
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //segunda imagen
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    //tercera imagen
                    produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                    produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                    produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                    produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                    //cuarta imagen
                    produc.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                    produc.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                    produc.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                    produc.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                    //quinta imagen
                    produc.imagen5.id = Convert.ToInt32(dt_imagenes.Rows[4]["id_img"]);
                    produc.imagen5.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[4]["tipo_img"]);
                    produc.imagen5.Nombre = dt_imagenes.Rows[4]["nombre"].ToString();
                    produc.imagen5.URL = dt_imagenes.Rows[4]["url"].ToString();
                }

            }
            return produc;

        }

        public List<Productos> ObtenerTodosLosProductos()
        {
            string query = "SELECT * FROM producto";
            DataTable dt = ComprasServicios.db.Execute(query);

            List<Productos> productos = new List<Productos>();
            foreach (DataRow row in dt.Rows)
            {
                Productos producto = new Productos();
                producto.Id = Convert.ToInt32(row["id_producto"]);
                producto.tipo_producto = (Tipo_Producto)Convert.ToInt32(row["id_tipo_producto"]);
                producto.nombre = row["Nombre"].ToString();
                producto.caracteristicas = row["caracteristicas"].ToString();
                producto.precio = Convert.ToInt32(row["precio"]);
                string query_imagenes = "select id_imagen as id_img,id_tipo_imagen as tipo_img,i.nombre as nombre,url_imagen as url from producto p join imagen i on (p.id_producto = i.id_producto) where p.id_producto = " + producto.Id + " and i.estado = True";
                DataTable dt_imagenes = ComprasServicios.db.Execute(query_imagenes);
                if (dt_imagenes.Rows.Count > 0)
                {
                    if (dt_imagenes.Rows.Count == 1)
                    {
                        producto.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        producto.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        producto.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        producto.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 2)
                    {
                        producto.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        producto.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        producto.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        producto.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //
                        producto.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        producto.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        producto.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        producto.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 3)
                    {
                        //primera imagen
                        producto.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        producto.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        producto.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        producto.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        producto.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        producto.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        producto.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        producto.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        producto.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        producto.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        producto.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        producto.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 4)
                    {
                        //primera imagen
                        producto.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        producto.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        producto.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        producto.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        producto.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        producto.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        producto.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        producto.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        producto.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        producto.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        producto.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        producto.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                        //cuarta imagen
                        producto.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                        producto.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                        producto.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                        producto.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                    }
                    else
                    {
                        //primera imagen
                        producto.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        producto.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        producto.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        producto.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        producto.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        producto.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        producto.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        producto.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        producto.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        producto.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        producto.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        producto.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                        //cuarta imagen
                        producto.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                        producto.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                        producto.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                        producto.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                        //quinta imagen
                        producto.imagen5.id = Convert.ToInt32(dt_imagenes.Rows[4]["id_img"]);
                        producto.imagen5.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[4]["tipo_img"]);
                        producto.imagen5.Nombre = dt_imagenes.Rows[4]["nombre"].ToString();
                        producto.imagen5.URL = dt_imagenes.Rows[4]["url"].ToString();
                    }
                }

                // Agregar el objeto producto a la lista
                productos.Add(producto);
            }
            return productos;
        }
        public List<Productos> productoMasivo()
        {
            IList<Productos> products = new List<Productos>();
            string query1 = string.Format(" Select * from producto");
            DataTable dt1 = ComprasServicios.db.Execute(query1);

            foreach (DataRow row in dt1.Rows)
            {
                products.Add(new Productos
                {
                    Id = Convert.ToInt32(row["id_producto"]),
                    nombre = row["Nombre"].ToString(),
                    caracteristicas = row["caracteristicas"].ToString(),
                    precio = Convert.ToInt32(row["precio"]),
                    estado = row["estado"].ToString() == "T" ? true : false,
                    tipo_producto = (Tipo_Producto)Convert.ToInt32(row["id_tipo_producto"])

                });
            }

            foreach (Productos produc in products)
            {
                string query_imagenes = "select id_imagen as id_img,id_tipo_imagen as tipo_img,i.nombre as nombre,url_imagen as url from producto p join imagen i on (p.id_producto = i.id_producto) where p.id_producto = " + produc.Id + " and i.estado = 'T'";
                DataTable dt_imagenes = ComprasServicios.db.Execute(query_imagenes);
                if (dt_imagenes.Rows.Count > 0)
                {
                    if (dt_imagenes.Rows.Count == 1)
                    {
                        produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 2)
                    {
                        produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //
                        produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 3)
                    {
                        //primera imagen
                        produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 4)
                    {
                        //primera imagen
                        produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                        //cuarta imagen
                        produc.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                        produc.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                        produc.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                        produc.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                    }
                    else
                    {
                        //primera imagen
                        produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                        //cuarta imagen
                        produc.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                        produc.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                        produc.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                        produc.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                        //quinta imagen
                        produc.imagen5.id = Convert.ToInt32(dt_imagenes.Rows[4]["id_img"]);
                        produc.imagen5.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[4]["tipo_img"]);
                        produc.imagen5.Nombre = dt_imagenes.Rows[4]["nombre"].ToString();
                        produc.imagen5.URL = dt_imagenes.Rows[4]["url"].ToString();
                    }
                }
                //}
            }
            return (List<Productos>)products;
        }

        public Productos BuscarProducto(string nombre)
        {
            
            string query1 = string.Format(" Select * from producto WHERE nombre LIKE UPPER ("+"'"+nombre+"%"+"')");
            DataTable dt1 = ComprasServicios.db.Execute(query1);
            Productos produc = new Productos();
            produc.Id = Convert.ToInt32(dt1.Rows[0]["id_producto"]);
            produc.nombre = dt1.Rows[0]["Nombre"].ToString();
            produc.caracteristicas = dt1.Rows[0]["caracteristicas"].ToString();
            produc.precio = Convert.ToInt32(dt1.Rows[0]["precio"]);
            produc.estado = dt1.Rows[0]["estado"].ToString() == "T" ? true : false;
            produc.tipo_producto = (Tipo_Producto)Convert.ToInt32(dt1.Rows[0]["id_tipo_producto"]);
            string query_imagenes = "select id_imagen as id_img,id_tipo_imagen as tipo_img,i.nombre as nombre,url_imagen as url from producto p join imagen i on (p.id_producto = i.id_producto) where p.id_producto = " + produc.Id + " and i.estado = 'T'";
            DataTable dt_imagenes = ComprasServicios.db.Execute(query_imagenes);
            if (dt_imagenes.Rows.Count > 0)
            {
                if (dt_imagenes.Rows.Count == 1)
                {
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                }
                else if (dt_imagenes.Rows.Count == 2)
                {
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                }
                else if (dt_imagenes.Rows.Count == 3)
                {
                    //primera imagen
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //segunda imagen
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    //tercera imagen
                    produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                    produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                    produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                    produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                }
                else if (dt_imagenes.Rows.Count == 4)
                {
                    //primera imagen
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //segunda imagen
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    //tercera imagen
                    produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                    produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                    produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                    produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                    //cuarta imagen
                    produc.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                    produc.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                    produc.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                    produc.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                }
                else
                {
                    //primera imagen
                    produc.imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                    produc.imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                    produc.imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                    produc.imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    //segunda imagen
                    produc.imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                    produc.imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                    produc.imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                    produc.imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    //tercera imagen
                    produc.imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                    produc.imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                    produc.imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                    produc.imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                    //cuarta imagen
                    produc.imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                    produc.imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                    produc.imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                    produc.imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                    //quinta imagen
                    produc.imagen5.id = Convert.ToInt32(dt_imagenes.Rows[4]["id_img"]);
                    produc.imagen5.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[4]["tipo_img"]);
                    produc.imagen5.Nombre = dt_imagenes.Rows[4]["nombre"].ToString();
                    produc.imagen5.URL = dt_imagenes.Rows[4]["url"].ToString();
                }

            }
            string commit1 = string.Format("COMMIT");
            DataTable cm1 = ComprasServicios.db.Execute(commit1);
            return produc;

        }
    }


    

    
}
