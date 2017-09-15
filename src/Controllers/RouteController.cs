using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GoldenForCongress.Data;
using GoldenForCongress.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GoldenForCongress.Controllers
{
    [Route("route")]
    public class RouteController : Controller
    {
        private readonly DB _db;
        private readonly IHostingEnvironment _env;

        public RouteController(DB db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
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
    }
}
