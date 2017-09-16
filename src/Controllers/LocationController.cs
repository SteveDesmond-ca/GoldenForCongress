using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GoldenForCongress.Data;
using GoldenForCongress.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GoldenForCongress.Controllers
{
    [Route("location")]
    public class LocationController : Controller
    {
        private readonly DB _db;
        private readonly IHostingEnvironment _env;

        public LocationController(DB db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        public IEnumerable<Location> Index()
        {
            return _db.Locations;
        }

        [HttpPost]
        public async Task<Location> Index([FromBody]JObject locationJSON)
        {
            var location = locationJSON.ToObject<Location>(Startup.SnakeCase);
            await _db.AddAsync(location);
            await _db.SaveChangesAsync();

            var json = JObject.FromObject(location, Startup.SnakeCase);
            json["position"] = JObject.Parse(json["position"].Value<string>());
            System.IO.File.WriteAllText(Path.Combine(_env.WebRootPath, "ian.json"), json.ToString());

            return location;
        }

        [HttpDelete]
        public async Task<IEnumerable<Location>> Delete()
        {
            await _db.ClearLocationHistory();
            return _db.Locations;
        }
    }
}