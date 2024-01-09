using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Net;

namespace DBrickandmorty
{
    public class Formato
    {
        public string nombre { get; set; }
    }
    public static class Function
    {
        [FunctionName("Nombre")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            
            //Recogemos el body
            StreamReader sr = new StreamReader(req.Body);
            string texto = sr.ReadToEnd();
            //Deserealizamos el body
            Formato format = JsonConvert.DeserializeObject<Formato>(texto);
            string url = "https://rickandmortyapi.com/api/character/?name=" + format.nombre;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return new OkObjectResult(recogerResponse(response));
        }
        static string recogerResponse(HttpWebResponse response)
        {
            FormatoJson formatoJson;
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            String texto = streamReader.ReadToEnd();
            Rootobject root = JsonConvert.DeserializeObject<Rootobject>(texto);
            formatoJson = new FormatoJson(root.results[0].id, root.results[0].name, root.results[0].status, root.results[0].species, root.results[0].gender, root.results[0].image);
            string respuesta = System.Text.Json.JsonSerializer.Serialize(formatoJson);
            return respuesta;
        }

    }
}
