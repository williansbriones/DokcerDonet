using api_proyecto_web.DBConText;
using api_proyecto_web.Modelos;
using api_proyecto_web.Modelos.@enum;
using Google.Protobuf.WellKnownTypes;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization.Formatters;
using System.Security.Policy;
using static System.Net.Mime.MediaTypeNames;

namespace api_proyecto_web.Servicios.Implementacion
{
    public class ComprasServicios : IcrudCompras<compras>
    {
        public compras CarroDeCompra =  new compras(); //carro de compra que contendra los productos que se quieran comprar
        private IcrudUsuario us = new UsuarioServicio();
        public static DBConText.Connection db = new DBConText.Connection();
        
        public IList<compras> BusquedaComprasCliente(int id_cliente)//metodo para obtener las compras de un cliente en especifico *listo* 
        {
            //obtencion de informacion para rellenar  con las lista y las compras para entregar la informacion de las compras
            IList<compras> listaCompras = new List<compras>();
            string Query = String.Format("select dp.cantidad as cantidad_producto, c.id_compra as id_compra, p.id_producto as id_producto, c.id_usuario as id_usuario, p.id_tipo_producto as id_tipo_producto, p.nombre as nombre_producto, p.caracteristicas as caracteristicas, p.precio as precio from compra c join detalle_compra dp on (dp.id_compra = c.id_compra) join producto p on (p.id_producto = dp.id_producto) LEFT JOIN cupon cu on (c.id_cupon = cu.id_cupon) where  c.id_usuario = " + id_cliente + " order by c.id_compra");
            string Query2 = string.Format("select c.id_estado_compra as id_estado_compra, c.id_compra as id_compra, IFNULL(DATE_FORMAT(cu.fecha_compra,'%d-%m-%Y'),DATE_FORMAT(now(),'%d-%m-%Y')) as fecha_termino,IFNULL(DATE_FORMAT(cu.fecha_entrega,'%d-%m-%Y'),DATE_FORMAT(NOW(),'%d-%m-%Y')) as fecha_inicio, IFNULL(cu.cant_uso, 0) as cantidad_uso, IFNULL(cu.codigo, 'Sin codigo') as condigo_desc, IFNULL(cu.cant_descuento,0) as descuento_cupon, IFNULL(cu.nombre,'Sin cupon') as nombre_cupon, IFNULL(cu.id_cupon,0) as id_cupon from compra c LEFT JOIN cupon cu on (c.id_cupon = cu.id_cupon) where c.id_usuario = " + id_cliente);
            
            DataTable dt1 = db.Execute(Query);//ejecucion de la consulta
            DataTable dt2 = db.Execute(Query2);//ejecucion de la consulta
            CultureInfo provider = new CultureInfo("es-CL");

            if (dt2.Rows.Count > 0) //evalua si las consultas tienen iformacion que entregar
            { 
                foreach (DataRow dr in dt2.Rows) //for para iterar las compras existentes
                {
                    listaCompras.Add(new compras //ingreso de todas las compras asociadas a un cliente a la lista
                    {
                        id_usuario = id_cliente,
                        Estado_compra = (EstadoCompra)Convert.ToInt32(dr["id_estado_compra"]),
                        id_compra = Convert.ToInt32(dr["id_compra"]),
                        Fecha_entrega = DateTime.ParseExact(dr["fecha_termino"].ToString(),"dd/MM/yyyy",provider),
                        Fecha_compra = DateTime.ParseExact(dr["fecha_inicio"].ToString(),"dd/MM/yyyy",provider),
                    });
                    
                }
                foreach (compras com in listaCompras) //ingreso de productos a las compras individuales 
                {
                    com.lista_productos = (from DataRow dr in dt1.Rows
                                           where Convert.ToInt32(dr["id_compra"]) == com.id_compra
                                           select new Productos()
                                           {
                                               Id = Convert.ToInt32(dr["id_producto"]),
                                               cantidad = Convert.ToInt32(dr["cantidad_producto"]),
                                               tipo_producto = (Tipo_Producto)Convert.ToInt32(dr["id_tipo_producto"]),
                                               nombre = dr["nombre_producto"].ToString(),
                                               caracteristicas = dr["caracteristicas"].ToString(),
                                               precio = Convert.ToInt32(dr["precio"]),
                                               imagen1 = new imagen { URL = UrlImagen(Convert.ToInt32(dr["id_producto"])) }

                                           }
                                           ).ToList();
                }
            }
            //resumen de los loops y como estas rellenaban las listas //resumen listaCompras(tiene muchas) ----> compra(tiene) -----> listaproductos(tiene muchos productos) --->productos//
            return listaCompras;
        }
        public int Agregarproducto(int id_producto, int cantidad, int id) // metodo que agregar productos al carro de compra *Listo* 
        {
            int id_usuario = id;
            if (id == 0) //codigo que genera un usuario el cual se incia en caso de no tener un usuario iniciado
            {
                string query = "SELECT id_usuario() as numero";
                DataTable dt_id_nuevo_usuario = new DataTable();
                dt_id_nuevo_usuario = db.Execute(query);
                string query_ingreso_usuario = "INSERT INTO usuario VALUES(" + dt_id_nuevo_usuario.Rows[0]["numero"] + ",'', '', '', 0, '" + dt_id_nuevo_usuario.Rows[0]["numero"] + "', '" + dt_id_nuevo_usuario.Rows[0]["numero"] + "', '', '', '')";
                db.Execute(query_ingreso_usuario);
                db.Execute("commit");
                UsuarioServicio.UsuarioIniciado.Id = Convert.ToInt32(dt_id_nuevo_usuario.Rows[0]["numero"]);
                id_usuario = Convert.ToInt32(dt_id_nuevo_usuario.Rows[0]["numero"]);
                string Query_imagen_us = "insert into imagen values (id_imagen() ,'https://upgradeimagens.blob.core.windows.net/imagenusuario/26-06-2023 19:47:33.png',1," + id_usuario + ",null,'imagen Usuario invitado',True)";
                db.Execute(Query_imagen_us);
                db.Execute("commit");
            }
            //Querys que buscan el producto indicado 
            string Query_Ingresar_producto = "select id_producto as id_producto, id_tipo_producto as id_tipo_producto, nombre as nombre, caracteristicas as caracteristicas, precio as precio from producto where id_producto = "+ id_producto;
            DataTable dt_Producto_ingreso = db.Execute(Query_Ingresar_producto);
            int indice; //variable para realizar busquedas en las listas 
            //Query que busca informacion si el usuario ya tiene un carro de compra en la base de datos activo actualmente
            String Query_Obtener_compra = String.Format("select c.id_estado_compra as id_estado_compra, c.id_compra as id_compra, ifnull(cu.cant_uso, 0) as cantidad_uso, ifnull(cu.codigo, 'Sin codigo') as condigo_desc, ifnull(cu.cant_descuento,0) as descuento_cupon, ifnull(cu.nombre,'Sin cupon') as nombre_cupon, ifnull(cu.id_cupon,0) as id_cupon from compra c LEFT JOIN cupon cu on (c.id_cupon = cu.id_cupon) where c.id_usuario =" + id_usuario + " and c.ID_ESTADO_COMPRA = 1 ");
            DataTable dt_obtener_compra = db.Execute(Query_Obtener_compra);
            if (dt_obtener_compra.Rows.Count > 0)//valida si el usuario tiene carro de compras
            {
                //relleno de informacion la compra del usuario
                CarroDeCompra.id_compra = Convert.ToInt32(dt_obtener_compra.Rows[0]["id_compra"]);
                CarroDeCompra.id_usuario = id_usuario;
                CarroDeCompra.Estado_compra = (EstadoCompra)Convert.ToInt32(dt_obtener_compra.Rows[0]["id_estado_compra"]);
            }
            else // genera carro de compras en caso que el usuario no tenga
            {
                string query_Create_compra = "INSERT INTO compra VALUES (id_compra() ," + id_usuario + ",0,0,1,NULL)";
                DataTable dt_creacion_compra = db.Execute(query_Create_compra);
                dt_creacion_compra = db.Execute(Query_Obtener_compra);
                db.Execute("commit");

                CarroDeCompra.id_compra = Convert.ToInt32(dt_creacion_compra.Rows[0]["id_compra"]);
                CarroDeCompra.id_usuario = id_usuario;
                CarroDeCompra.Estado_compra = (EstadoCompra)Convert.ToInt32(dt_creacion_compra.Rows[0]["id_estado_compra"]);
            }
            //obtiene los productos que contenga las compras
            string Query_obtencion_productos = String.Format("select dp.cantidad as cantidad_producto, c.id_compra as id_compra, p.id_producto as id_producto, c.id_usuario as id_usuario, p.id_tipo_producto as id_tipo_producto, p.nombre as nombre_producto, p.caracteristicas as caracteristicas, p.precio as precio from compra c join detalle_compra dp on (dp.id_compra = c.id_compra) join producto p on (p.id_producto = dp.id_producto) LEFT JOIN cupon cu on (c.id_cupon = cu.id_cupon) where  c.id_usuario = " + id_usuario + " order by c.id_compra");
            DataTable dt_obtencion_producto = db.Execute(Query_obtencion_productos);
            if (dt_obtencion_producto.Rows.Count > 0) // valida si tiene produtos asociados a las compras del usuario 
            {
                //ingreso y filtro de las compra del cliente la cual se encuentren dentro del carro de compras
                CarroDeCompra.lista_productos = (from DataRow dr in dt_obtencion_producto.Rows
                                                 where Convert.ToInt32(dr["id_compra"]) == CarroDeCompra.id_compra
                                                 select new Productos()
                                                {
                                                    Id = Convert.ToInt32(dr["id_producto"]),
                                                    cantidad = Convert.ToInt32(dr["cantidad_producto"]),
                                                    tipo_producto = (Tipo_Producto)Convert.ToInt32(dr["id_tipo_producto"]),
                                                    nombre = dr["nombre_producto"].ToString(),
                                                    caracteristicas = dr["caracteristicas"].ToString(),
                                                    precio = Convert.ToInt32(dr["precio"])
                                                }
                                                ).ToList();
            }

            try
            {
                if (dt_Producto_ingreso.Rows.Count > 0) //verifica que exista informacion en la consulta 
                {
                    //busqueda del indica para aumentar la cantidad del producto por si existe dentro del carro de compra
                    indice = CarroDeCompra.lista_productos.Select((item, index) => new
                    {
                        itemname = item,
                        indexx = index,
                    }).Where(x => x.itemname.Id == id_producto)
                    .First()
                    .indexx;
                    int precio;
                    precio = CarroDeCompra.lista_productos[indice].precio; //obtencion de precio para ajustar el valor segun la cantidad en la base de datos
                    Console.WriteLine("indice encontrado" + indice);
                    CarroDeCompra.lista_productos[indice].cantidad = CarroDeCompra.lista_productos[indice].cantidad + cantidad;
                    Console.WriteLine(CarroDeCompra.lista_productos[indice].cantidad);
                    //acutualizacion de la cantidad aumentada y de los valores
                    string QueryActualizacionPorductos = "UPDATE detalle_compra set sub_total = " + precio * CarroDeCompra.lista_productos[indice].cantidad + ", cantidad = " + CarroDeCompra.lista_productos[indice].cantidad + " where id_compra = "+ CarroDeCompra.id_compra + " and id_producto = "+ id_producto;
                    DataTable dt = db.Execute(QueryActualizacionPorductos);
                    db.Execute("commit");
                    string QueryActualizacionCompra = "UPDATE compra set descuento = "+CarroDeCompra.Descuento +" , total = "+CarroDeCompra.Total+" where id_compra = "+CarroDeCompra.id_compra;
                    db.Execute(QueryActualizacionCompra);
                    db.Execute("commit");
                }
                return id_usuario;
            }
            catch // en caso de que no exista producto asociado a la compra 
            {
                CarroDeCompra.lista_productos.Add (new Productos { 
                    Id = Convert.ToInt32(dt_Producto_ingreso.Rows[0]["id_producto"]),
                    tipo_producto = (Tipo_Producto)Convert.ToInt32(dt_Producto_ingreso.Rows[0]["id_tipo_producto"]),
                    nombre = dt_Producto_ingreso.Rows[0]["nombre"].ToString(),
                    caracteristicas = dt_Producto_ingreso.Rows[0]["caracteristicas"].ToString(),
                    precio = Convert.ToInt32(dt_Producto_ingreso.Rows[0]["precio"]),
                    cantidad = cantidad
                });
                Console.WriteLine("producto agregado: "+dt_Producto_ingreso.Rows[0]["nombre"].ToString());
                //aumenta la cantidad de producto y genera un fila en la base de datos
                string QueryIngresoDetalle = "INSERT INTO detalle_compra VALUES ( " + CarroDeCompra.id_compra + ", "+ Convert.ToInt32(dt_Producto_ingreso.Rows[0]["id_producto"]) + " , "+ cantidad+ " , "+(cantidad * Convert.ToInt32(dt_Producto_ingreso.Rows[0]["precio"])) + " )";
                DataTable dt = db.Execute(QueryIngresoDetalle);
                db.Execute("commit");
                string QueryActualizacionCompra = "UPDATE compra set descuento = " + CarroDeCompra.Descuento + " , total = " + CarroDeCompra.Total + " where id_compra = " + CarroDeCompra.id_compra;
                db.Execute(QueryActualizacionCompra);
                return id_usuario;
            }    
        }
        public void EliminarProducto(int id_producto, int cantidad, int id) //metodo que elimina cantidad de productos que se encuentren en el carro de compra *listo* 
        {
            //querys que obtiene informacion del producto
            string Query_Ingresar_producto = "select id_producto as id_producto, id_tipo_producto as id_tipo_producto, nombre as nombre, caracteristicas as caracteristicas, precio as precio from producto where id_producto = " + id_producto;
            DataTable dt_Producto_ingreso = db.Execute(Query_Ingresar_producto);
            int indice; //indice para buscar productos
            //query que busque si el usuario tiene o no una compra en el carro de compras
            String Query_Obtener_compra = String.Format("select c.id_estado_compra as id_estado_compra, c.id_compra as id_compra, ifnull(cu.cant_uso, 0) as cantidad_uso, ifnull(cu.codigo, 'Sin codigo') as condigo_desc, ifnull(cu.cant_descuento,0) as descuento_cupon, ifnull(cu.nombre,'Sin cupon') as nombre_cupon, ifnull(cu.id_cupon,0) as id_cupon from compra c LEFT JOIN cupon cu on (c.id_cupon = cu.id_cupon) where c.id_usuario =" + id + " and c.ID_ESTADO_COMPRA = 1 ");
            DataTable dt_obtener_compra = db.Execute(Query_Obtener_compra);
            if (dt_obtener_compra.Rows.Count > 0)// valida si existe carro de compras
            {
                //ingreso de informacion al carro de compras 
                CarroDeCompra.id_compra = Convert.ToInt32(dt_obtener_compra.Rows[0]["id_compra"]);
                CarroDeCompra.id_usuario = id;
                CarroDeCompra.Estado_compra = (EstadoCompra)Convert.ToInt32(dt_obtener_compra.Rows[0]["id_estado_compra"]);
            }
            //obtiene informacion de los productos por de las compras asociadas al usuario iniciado
            string Query_obtencion_productos = String.Format("select dp.cantidad as cantidad_producto, c.id_compra as id_compra, p.id_producto as id_producto, c.id_usuario as id_usuario, p.id_tipo_producto as id_tipo_producto, p.nombre as nombre_producto, p.caracteristicas as caracteristicas, p.precio as precio from compra c join detalle_compra dp on (dp.id_compra = c.id_compra) join producto p on (p.id_producto = dp.id_producto) LEFT JOIN cupon cu on (c.id_cupon = cu.id_cupon) where  c.id_usuario = " + id + " order by c.id_compra");
            DataTable dt_obtencion_producto = db.Execute(Query_obtencion_productos);
            if (dt_obtencion_producto.Rows.Count > 0) //valida si hay informacion de la query 
            {
                //ingreso de productos al carro de comrpas
                CarroDeCompra.lista_productos = (from DataRow dr in dt_obtencion_producto.Rows
                                                 where Convert.ToInt32(dr["id_compra"]) == CarroDeCompra.id_compra
                                                 select new Productos()
                                                 {
                                                     Id = Convert.ToInt32(dr["id_producto"]),
                                                     cantidad = Convert.ToInt32(dr["cantidad_producto"]),
                                                     tipo_producto = (Tipo_Producto)Convert.ToInt32(dr["id_tipo_producto"]),
                                                     nombre = dr["nombre_producto"].ToString(),
                                                     caracteristicas = dr["caracteristicas"].ToString(),
                                                     precio = Convert.ToInt32(dr["precio"])
                                                 }
                                                ).ToList();
            }
            try
            {
                if (dt_Producto_ingreso.Rows.Count > 0) //valida si existen productos en las compras
                {
                    //busqueda del indice del producto que se quiere eliminar o disminuir su cantidad
                    indice = CarroDeCompra.lista_productos.Select((item, index) => new
                    {
                        itemname = item,
                        indexx = index,
                    }).Where(x => x.itemname.Id == id_producto)
                    .First()
                    .indexx;
                    Console.WriteLine("indice encontrado" + indice);

                    int precio;
                    precio = CarroDeCompra.lista_productos[indice].precio; //obtencion de precio del producto para la actualizacion den la base de datos
                    int cantidad_total = CarroDeCompra.lista_productos[indice].cantidad - cantidad; //obencion de nueva cantidad que se ingresara para actualizar la base de datos

                    if (cantidad_total > 0 ) //valida si la cantidad es mayor a 0 y si no elimina el producto del carro de compras
                    {
                        CarroDeCompra.lista_productos[indice].cantidad = cantidad_total;
                        Console.WriteLine(CarroDeCompra.lista_productos[indice].cantidad);
                        //actualizacion de precios y cantidades en el carro de compras
                        string QueryActualizacionPorductos = "UPDATE detalle_compra set sub_total = " + precio * CarroDeCompra.lista_productos[indice].cantidad + ", cantidad = " + CarroDeCompra.lista_productos[indice].cantidad + " where id_compra = " + CarroDeCompra.id_compra + " and id_producto = " + id_producto;
                        DataTable dt = db.Execute(QueryActualizacionPorductos);
                        db.Execute("commit");
                        string QueryActualizacionCompra = "UPDATE compra set descuento = " + CarroDeCompra.Descuento + " , total = " + CarroDeCompra.Total + " where id_compra = " + CarroDeCompra.id_compra;
                        db.Execute(QueryActualizacionCompra);
                        db.Execute("commit");

                    }
                    else//en el caso de que la cantidad sea menor que cero esta eliminara los productos del carro de compra
                    {
                        string query = "delete from detalle_compra where id_compra = " + CarroDeCompra.id_compra + " and id_producto = " + id_producto;
                        db.Execute(query);
                        db.Execute("commit");
                        CarroDeCompra.lista_productos.RemoveAt(indice);
                        string QueryActualizacionCompra = "UPDATE compra set descuento = " + CarroDeCompra.Descuento + " , total = " + CarroDeCompra.Total + " where id_compra = " + CarroDeCompra.id_compra;
                        db.Execute(QueryActualizacionCompra);
                        db.Execute("commit");
                    }
                }
            }catch (Exception ex) //en caso de no existir el producto no eliminara nada
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("no existe el producto en la compra");


            }
        }
        public void ConfirmarCompra(int id_compra, int id_usuaro)//metodo confirma el carro de compra como una compra realizada *listo* 
        {
            //ingreso de fechas para actualizar la tabla delivey en la base de datos
            DateTime fechaActual = DateTime.Now;
            DateTime fechaEntrega = fechaActual.AddDays(3);
            //formateo de fecha para entregarla a la base de datos 
            fechaActual.ToString("dd/MM/yyyy");
            fechaEntrega.ToString("dd/MM/yyyy");
            Usuario usuario = us.informacionUsuario(id_usuaro);

            //validador de que el usuario tenga sus datos bien ingresado para que el delivery se pueda realizar
            if (id_compra != 0 & usuario.Comuna != string.Empty & usuario.Email != string.Empty & usuario.telefono != string.Empty)
            {
                //Querys de ingreso nueva compra en el delivery y actualizacion de el estado de la compra 
                string QueryIngresoDelivery = "Insert into delivery values ( id_delivery(), " + id_compra + ", 'Diego Diaz','"+ usuario.Direccion+"' , '" +fechaActual.ToString("yyyy/MM/dd") + "' , '" +fechaEntrega.ToString("yyyy/MM/dd")+"')";
                string QueryActulizacionCompra = "UPDATE compra set id_estado_compra = 2 where id_compra = "+ id_compra;
                db.Execute(QueryIngresoDelivery);
                db.Execute("commit");
                db.Execute(QueryActulizacionCompra);
                db.Execute("commit");
                //una vez que se ejecute este paso el carro de compras quedara vacio 
                CarroDeCompra = new compras();
            }
            else
            {
                if (CarroDeCompra.id_compra != 0)
                {
                    Console.WriteLine("No hay compra registrada");
                    throw new Exception();
                }
                else if (UsuarioServicio.UsuarioIniciado.Direccion != "")
                {
                    Console.WriteLine("No tiene direccion registrada");
                    throw new Exception();
                } else if (UsuarioServicio.UsuarioIniciado.Id != 0)
                {
                    Console.WriteLine("No hay usuario registrado a la compra");
                    throw new Exception();
                } else if (CarroDeCompra.lista_productos.Count > 0)
                {
                    Console.WriteLine("No hay productos asociadoa al carro de compra");
                    throw new Exception();
                } else if (UsuarioServicio.UsuarioIniciado.Comuna != "" || UsuarioServicio.UsuarioIniciado.Email != "" || UsuarioServicio.UsuarioIniciado.telefono != "")
                {
                    Console.WriteLine("La informacion de usuario es incorrecta");
                    throw new Exception();
                }
                throw new Exception();
            }   
        }
        public compras BusquedaCarroCompras(int id)//metodo que consulta el carro de compra del usuario que se encutre actuvo *Listo*  
        {
            //variables que rellenan informacion del carro de compras
            IList<Productos> ListaProductos = new List<Productos>();
            compras listaCompras = new compras();
            listaCompras.lista_productos = ListaProductos;
            //Query que consulta por las compras que tiene el cliente iniciado
            String Query = String.Format("select dp.cantidad as cantidad_producto, ifnull(DATE_FORMAT(cu.fecha_compra,'%d %m %Y'),DATE_FORMAT(now(),'%d %m %Y')) as fecha_termino, ifnull(DATE_FORMAT(cu.fecha_entrega,'%d %m %Y'),DATE_FORMAT(now(),'%d %m %Y')) as fecha_inicio, ifnull(cu.cant_uso, 0) as cantidad_uso, ifnull(cu.codigo, 'Sin codigo') as condigo_desc, ifnull(cu.cant_descuento,0) as descuento_cupon, ifnull(cu.nombre,'Sin cupon') as nombre_cupon, ifnull(cu.id_cupon,0) as id_cupon, c.id_compra as id_compra, c.id_estado_compra as id_estado_compra, p.id_producto as id_producto, c.id_usuario as id_usuario, p.id_tipo_producto as id_tipo_producto, p.nombre as nombre_producto, p.caracteristicas as caracteristicas, p.precio as precio from compra c join detalle_compra dp on (dp.id_compra = c.id_compra) join producto p on (p.id_producto = dp.id_producto) LEFT JOIN cupon cu on (c.id_cupon = cu.id_cupon) where  c.id_estado_compra = " + 1 + "  and c.id_usuario = " + id);
            DataTable dt = db.Execute(Query);
            if (dt.Rows.Count > 0) //validador para que saber si el usuario tiene un carro de compras 
            {
                listaCompras.id_compra = Convert.ToInt32(dt.Rows[0]["id_compra"]);
                listaCompras.id_usuario = Convert.ToInt32(dt.Rows[0]["id_usuario"]);
                foreach (DataRow dr in dt.Rows)
                {
                    ListaProductos.Add(new Productos
                    {
                        Id = Convert.ToInt32(dr["id_producto"]),
                        tipo_producto = (api_proyecto_web.Modelos.@enum.Tipo_Producto)Convert.ToInt32(dr["id_tipo_producto"]),
                        nombre = (string)dr["nombre_producto"],
                        caracteristicas = (string)dr["caracteristicas"],
                        precio = Convert.ToInt32(dr["precio"]),
                        imagen1 = new imagen(),
                        imagen2 = new imagen(),
                        imagen3 = new imagen(),
                        imagen4 = new imagen(),
                        imagen5 = new imagen(),
                        cantidad = Convert.ToInt32(dr["cantidad_producto"])
                    });
                }

                listaCompras.Estado_compra = (EstadoCompra)Convert.ToInt32(dt.Rows[0]["id_estado_compra"]);
            }
            //actulizacion del carro de compras
            int indice_productos = listaCompras.lista_productos.Count;

            for (int pro = 0; pro < indice_productos; pro++)
            {
                int id_producto = listaCompras.lista_productos[pro].Id;
                string query_imagenes = "select id_imagen as id_img,id_tipo_imagen as tipo_img,i.nombre as nombre,url_imagen as url from producto p join imagen i on (p.id_producto = i.id_producto) where p.id_producto = " + id_producto + " and i.estado = True";
                DataTable dt_imagenes = db.Execute(query_imagenes);
                if (dt_imagenes.Rows.Count > 0)
                {
                    if (dt_imagenes.Rows.Count == 1)
                    {
                        listaCompras.lista_productos[pro].imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        listaCompras.lista_productos[pro].imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 2)
                    {
                        listaCompras.lista_productos[pro].imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        listaCompras.lista_productos[pro].imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //
                        listaCompras.lista_productos[pro].imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        listaCompras.lista_productos[pro].imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 3)
                    {
                        //primera imagen
                        listaCompras.lista_productos[pro].imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        listaCompras.lista_productos[pro].imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        listaCompras.lista_productos[pro].imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        listaCompras.lista_productos[pro].imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        listaCompras.lista_productos[pro].imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        listaCompras.lista_productos[pro].imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                    }
                    else if (dt_imagenes.Rows.Count == 4)
                    {
                        //primera imagen
                        listaCompras.lista_productos[pro].imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        listaCompras.lista_productos[pro].imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        listaCompras.lista_productos[pro].imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        listaCompras.lista_productos[pro].imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        listaCompras.lista_productos[pro].imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        listaCompras.lista_productos[pro].imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                        //cuarta imagen
                        listaCompras.lista_productos[pro].imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                        listaCompras.lista_productos[pro].imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                    }
                    else
                    {
                        //primera imagen
                        listaCompras.lista_productos[pro].imagen1.id = Convert.ToInt32(dt_imagenes.Rows[0]["id_img"]);
                        listaCompras.lista_productos[pro].imagen1.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[0]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen1.Nombre = dt_imagenes.Rows[0]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen1.URL = dt_imagenes.Rows[0]["url"].ToString();
                        //segunda imagen
                        listaCompras.lista_productos[pro].imagen2.id = Convert.ToInt32(dt_imagenes.Rows[1]["id_img"]);
                        listaCompras.lista_productos[pro].imagen2.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[1]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen2.Nombre = dt_imagenes.Rows[1]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen2.URL = dt_imagenes.Rows[1]["url"].ToString();
                        //tercera imagen
                        listaCompras.lista_productos[pro].imagen3.id = Convert.ToInt32(dt_imagenes.Rows[2]["id_img"]);
                        listaCompras.lista_productos[pro].imagen3.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[2]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen3.Nombre = dt_imagenes.Rows[2]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen3.URL = dt_imagenes.Rows[2]["url"].ToString();
                        //cuarta imagen
                        listaCompras.lista_productos[pro].imagen4.id = Convert.ToInt32(dt_imagenes.Rows[3]["id_img"]);
                        listaCompras.lista_productos[pro].imagen4.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[3]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen4.Nombre = dt_imagenes.Rows[3]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen4.URL = dt_imagenes.Rows[3]["url"].ToString();
                        //quinta imagen
                        listaCompras.lista_productos[pro].imagen5.id = Convert.ToInt32(dt_imagenes.Rows[4]["id_img"]);
                        listaCompras.lista_productos[pro].imagen5.IdTipoClase = Convert.ToInt32(dt_imagenes.Rows[4]["tipo_img"]);
                        listaCompras.lista_productos[pro].imagen5.Nombre = dt_imagenes.Rows[4]["nombre"].ToString();
                        listaCompras.lista_productos[pro].imagen5.URL = dt_imagenes.Rows[4]["url"].ToString();
                    }
                }

            }


            return listaCompras;
        }

        public string UrlImagen(int id_producto)
        {
            string Url = string.Empty;
            string QueryImagenProucto = "select url_imagen as url from imagen where id_producto = " + id_producto + " limit 1";
            DataTable ImagenProducto = db.Execute(QueryImagenProucto);
            Url = ImagenProducto.Rows[0]["url"].ToString();

            return Url;
        }


    }
}
