using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GoldenForCongress.Data;
using GoldenForCongress.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GoldenForCongress.Controllers
{
    [Route("route")]
    public class RouteController : Controller
    {
        private readonly DB _db;
        private readonly IHostingEnvironment _env;
        private readonly HttpClient _httpClient;

        public RouteController(DB db, IHostingEnvironment env, HttpClient httpClient)
        {
            _db = db;
            _env = env;
            _httpClient = httpClient;
        }

        [HttpGet("cache")]
        public IEnumerable<Section> Cache()
        {
            var routes = _db.RouteSections;
            var json = JArray.FromObject(routes, Startup.SnakeCase);
            foreach (var item in json)
                item["path"] = JArray.Parse(item["path"].Value<string>());
            System.IO.File.WriteAllText(Path.Combine(_env.WebRootPath, "route.json"), json.ToString());
            return routes;
        }

        [HttpGet]
        public IEnumerable<Section> Index()
        {
            return _db.RouteSections;
        }

        [HttpPost]
        public async Task<IEnumerable<Section>> Index([FromBody]JObject routeJSON)
        {
            var route = routeJSON.ToObject<Section>(Startup.SnakeCase);
            if (route.ID == Guid.Empty)
                await Add(route);
            else
                await Update(route);
            return _db.RouteSections;
        }

        private async Task Update(Section section)
        {
            _db.Update(section);
            await _db.SaveChangesAsync();
        }

        private async Task Add(Section section)
        {
            section.ID = Guid.NewGuid();
            await _db.AddAsync(section);
            await _db.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IEnumerable<Section>> Delete(Guid id)
        {
            var route = await _db.RouteSections.FindAsync(id);
            _db.Remove(route);
            await _db.SaveChangesAsync();
            return _db.RouteSections;
        }

        [HttpGet("from-gmaps/{url}")]
        public async Task<IEnumerable<LatLng>> FromGoogle(string url)
        {
            var decodedURL = Uri.UnescapeDataString(url);
            var delimiter = decodedURL.IndexOf("dir/", StringComparison.Ordinal);
            var directionsPart = decodedURL.Substring(delimiter + 4);
            var data = await _httpClient.GetAsync("https://mapstogpx.com/load.php?gdata=" + directionsPart);
            data.Content.Headers.ContentType.CharSet = "UTF-8";
            var dataObject = JObject.Parse(await data.Content.ReadAsStringAsync());
            return dataObject["points"].Select(j => new LatLng { Lat = j["lat"].Value<double>(), Lng = j["lng"].Value<double>() });
        }

        [HttpGet("from-gmaps-detail/{url}")]
        public async Task<IEnumerable<LatLng>> FromGoogleDetailed(string url)
        {
            var body = await System.IO.File.ReadAllTextAsync("body.txt");
            var message = new HttpRequestMessage(HttpMethod.Post, "http://www.gpsvisualizer.com/convert");
            message.Content = new StringContent(body.Replace("{{url}}", Uri.UnescapeDataString(url)));
            message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data; boundary=---boundary");
            var response = await _httpClient.SendAsync(message);
            var data = await response.Content.ReadAsStringAsync();
            var pointData = data.Split("<pre>")[1].Split("</pre>")[0];
            var lines = pointData.Split(Environment.NewLine);

            var coords = new List<LatLng>();
            foreach (var line in lines.Where(l => l.StartsWith("T")))
            {
                var fields = line.Split(ConsoleKey.Tab.ToString());
                var lat = Convert.ToDouble(fields[0]);
                var lng = Convert.ToDouble(fields[1]);
                coords.Add(new LatLng { Lat = lat, Lng = lng });
            }
            return coords;
        }
    }
}
