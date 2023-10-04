using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_proyecto_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller_TipoProducto : ControllerBase
    {
        // GET: api/<Tipo_Producto>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Tipo_Producto>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Tipo_Producto>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Tipo_Producto>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Tipo_Producto>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
