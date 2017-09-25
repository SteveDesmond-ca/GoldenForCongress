using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GoldenForCongress.Data;
using GoldenForCongress.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GoldenForCongress.Controllers
{
    [Authorize(Roles = "Admin")]
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

        public IEnumerable<Section> Cache()
        {
            var routes = _db.RouteSections;
            var json = JArray.FromObject(routes, Startup.SnakeCase);
            foreach (var item in json)
                item["path"] = JArray.Parse(item["path"].Value<string>());
            System.IO.File.WriteAllText(Path.Combine(_env.WebRootPath, "route.json"), json.ToString());
            return routes;
        }

        public IEnumerable<Section> Index() => _db.RouteSections;

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

        [HttpDelete]
        public async Task<IEnumerable<Section>> Delete(Guid id)
        {
            var route = await _db.RouteSections.FindAsync(id);
            _db.Remove(route);
            await _db.SaveChangesAsync();
            return _db.RouteSections;
        }

        public async Task<IActionResult> GPX(Guid id)
        {
            var section = await _db.RouteSections.FindAsync(id);
            var gpx = Services.GPX.Transform(section);
            var contents = Encoding.UTF8.GetBytes(gpx.ToString());
            return new FileContentResult(contents, "application/gpx+xml")
            {
                FileDownloadName = section.Date.ToString("yyyy-MM-dd") + ".gpx"
            };
        }
    }
}
