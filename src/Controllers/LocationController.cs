using System;
using System.Collections.Generic;
using System.IO;
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
    public class LocationController : Controller
    {
        private readonly DB _db;
        private readonly IHostingEnvironment _env;

        public LocationController(DB db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IEnumerable<Location> Index() => _db.Locations;

        [HttpPost]
        public async Task<Location> Index([FromBody]JObject locationJSON)
        {
            var location = locationJSON.ToObject<Location>(Startup.SnakeCase);
            location.ID = Guid.NewGuid();
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