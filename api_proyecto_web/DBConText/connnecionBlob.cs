using api_proyecto_web.Modelos;
using Azure.Storage.Blobs;
using System.Runtime.ConstrainedExecution;
using Microsoft.Extensions.Configuration;
using System.IO;



namespace api_proyecto_web.DBConText
{
    public class connnecionBlob
    {
        private string coneccion_str;
        private string contenedor = "imagenusuario";
        private string contenedor_producto = "productos";
        public connnecionBlob() {
            var constructor = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
            coneccion_str = constructor.GetSection("ConnectionStrings:blobs").Value;
        }

        public string IngresoImagenProducto(IFormFile imagen)
        {
            string url;
            string extension = Path.GetExtension(path: imagen.FileName);
            Console.WriteLine(extension);
            if (extension == ".png" || extension == ".jpg")
            {
                string nombre_imagen = (DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
                BlobServiceClient server = new BlobServiceClient(coneccion_str);
                BlobContainerClient container = server.GetBlobContainerClient(contenedor_producto);
                container.UploadBlob(nombre_imagen, imagen.OpenReadStream());
                Console.WriteLine(container.Uri + "/" + nombre_imagen);
                url = (container.Uri + "/" + nombre_imagen).ToString();
                return url;
            }
            else
            {
                throw new Exception();
            }
        }

        public string IngresoImagenUsuario(IFormFile imagen)
        {
            string url;
            string extencion = System.IO.Path.GetExtension(imagen.FileName);
            Console.WriteLine(extencion);
            if (extencion == ".png" || extencion == ".jpg")
            {
                string nombre_imagen = (DateTime.Now + extencion).ToString();
                BlobServiceClient server = new BlobServiceClient(coneccion_str);
                BlobContainerClient container = server.GetBlobContainerClient(contenedor);
                container.UploadBlob(nombre_imagen, imagen.OpenReadStream());
                Console.WriteLine(container.Uri + "/" + nombre_imagen);
                url = (container.Uri + "/" + nombre_imagen).ToString();
                return url;
            }
            else
            {
                throw new Exception();
            }
        }



    }
}
