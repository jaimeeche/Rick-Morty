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
using System.Net.Http;
using System.Net;
using DBrickandmorty;
using Microsoft.WindowsAzure.Storage;

namespace DBrickandmorty
{

    public class RootobjectCaracter
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string species { get; set; }
        public string type { get; set; }
        public string gender { get; set; }
        public Origin origin { get; set; }
        public Location location { get; set; }
        public string image { get; set; }
        public string[] episode { get; set; }
        public string url { get; set; }
        public DateTime created { get; set; }
    }

    public class Rootobject
    {
        public Info info { get; set; }
        public Result[] results { get; set; }
    }

    public class Info
    {
        public int count { get; set; }
        public int pages { get; set; }
        public string next { get; set; }
        public object prev { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string species { get; set; }
        public string type { get; set; }
        public string gender { get; set; }
        public Origin origin { get; set; }
        public Location location { get; set; }
        public string image { get; set; }
        public string[] episode { get; set; }
        public string url { get; set; }
        public DateTime created { get; set; }
    }

    public class Origin
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    class FormatoJson
    {
        public int id { get; set; }
        public static int contadorObjetos = 0;
        public string nombre { get; set; }
        public string estado { get; set; }
        public string especie { get; set; }
        public string genero { get; set; }
        public string imagen { get; set; }
        public string localizacion { get; set; }
        public string episodio { get; set; }
        public FormatoJson(int id, string nombre, string estado, string especie, string genero, string imagen, string localizacion,string episodio)
        {
            this.id = id;
            this.nombre = nombre;
            this.estado = estado;
            this.especie = especie;
            this.genero = genero;
            this.imagen = imagen;
            this.localizacion = localizacion;
            this.episodio = episodio;
            contadorObjetos++;
            
        }public FormatoJson(int id, string nombre, string estado, string especie, string genero, string imagen)
        {
            this.id = id;
            this.nombre = nombre;
            this.estado = estado;
            this.especie = especie;
            this.genero = genero;
            this.imagen = imagen;
            
            contadorObjetos++;
            
        }
        public FormatoJson(string nombre)
        {
            this.nombre = nombre;
            contadorObjetos++;
        }
    }
    class Nombre
    {
        public string nombre { get; set; }
        public string imagen { get; set; }
        public int id { get; set; }
        public static int contadorObjetos = 0;
        public Nombre(string nombre, string imagen, int id)
        {
            this.nombre = nombre;
            contadorObjetos++;
            this.imagen = imagen;
            this.id = id;
        }
    }
    class Request
    {
        public int id { get; set; }
    }
}
public static class Function1
{
    [FunctionName("GetPersonaje")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        //string respuesta = System.Text.Json.JsonSerializer.Serialize(request());
        //string respuesta = System.Text.Json.JsonSerializer.Serialize(nombre);
        return new OkObjectResult(recogerDatos(obtenerDatos(req)));
    }


    static void recogerResponse(HttpWebResponse response, FormatoJson[] json)
    {
        Stream stream = response.GetResponseStream();
        StreamReader streamReader = new StreamReader(stream);
        String texto = streamReader.ReadToEnd();
        Rootobject root = JsonConvert.DeserializeObject<Rootobject>(texto);

        for (int i = 0; i < 20; i++)
        {
            //linea de codigo que vale, la segunda es de prueba 
            json[FormatoJson.contadorObjetos] = new FormatoJson(root.results[i].id, root.results[i].name, root.results[i].status, root.results[i].species, root.results[i].gender, root.results[i].image);
            //nombre[Nombre.contadorObjetos] = new Nombre(root.results[i].name, root.results[i].image, root.results[i].id);              
        }
    }

    static FormatoJson[] request()
    {
        int id = 1;
        string url = "https://rickandmortyapi.com/api/character/?page=" + id;
        bool salir = false;
        HttpWebResponse response;
        FormatoJson[] json = new FormatoJson[820];

        //Nombre[] nombre = new Nombre[820];
        //bucle para recorrer todo los json
        while (salir == false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            response = (HttpWebResponse)request.GetResponse();
            recogerResponse(response, json);
            id++;
            url = "https://rickandmortyapi.com/api/character/?page=" + id;
            if (id == 42)
            {
                salir = true;
            }
        }
        return json;
    }
    static HttpWebResponse obtenerDatos(HttpRequest req)
    {
        StreamReader sr = new StreamReader(req.Body);
        string texto = sr.ReadToEnd();
        //Deserealizamos el body
        Request request = JsonConvert.DeserializeObject<Request>(texto);
        string url = "https://rickandmortyapi.com/api/character/" + request.id;
        HttpWebRequest solicitud = (HttpWebRequest)WebRequest.Create(url);
        solicitud.Method = "GET";
        HttpWebResponse response = (HttpWebResponse)solicitud.GetResponse();
        return response;
    }
    static string recogerDatos(HttpWebResponse response)
    {
        FormatoJson formatoJson ;
        Stream stream = response.GetResponseStream();
        StreamReader streamReader = new StreamReader(stream);
        string texto = streamReader.ReadToEnd();
        RootobjectCaracter root = JsonConvert.DeserializeObject<RootobjectCaracter>(texto);
        formatoJson = new FormatoJson(root.id, root.name, root.status, root.species, root.gender, root.image, root.origin.name, root.episode[0]);
        string respuesta = System.Text.Json.JsonSerializer.Serialize(formatoJson);
        return respuesta;
        //JsonConvert.SerializeObject(formatoJson);;
    }
}


