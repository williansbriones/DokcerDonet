namespace api_proyecto_web.Modelos
{
    public class imagen
    {

        public int id { get; set; }
        public int IdTipoClase { get; set; }

        public string Nombre { get; set; }
        public string URL { get; set; }

        public imagen()
        {
            id = new int();
            IdTipoClase = new int();
            Nombre = string.Empty;
            URL = string.Empty;
        }
    }
}
