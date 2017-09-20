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
    [Route("events")]
    public class EventsController : Controller
    {
        private readonly DB _db;
        private readonly IHostingEnvironment _env;

        public EventsController(DB db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet("cache")]
        public IEnumerable<Event> Cache()
        {
            var media = _db.Events;
            var json = JArray.FromObject(media, Startup.SnakeCase);
            foreach (var item in json)
                item["location"] = JObject.Parse(item["location"].Value<string>());
            System.IO.File.WriteAllText(Path.Combine(_env.WebRootPath, "events.json"), json.ToString());
            return media;
        }

        [HttpGet]
        public IEnumerable<Event> Index()
        {
            return _db.Events;
        }

        [HttpPost]
        public async Task<IEnumerable<Event>> Index([FromBody]JObject eventsJSON)
        {
            var eventInfo = eventsJSON.ToObject<Event>(Startup.SnakeCase);
            if (eventInfo.ID == Guid.Empty)
                await Add(eventInfo);
            else
                await Update(eventInfo);
            return _db.Events;
        }

        private async Task Update(Event eventInfo)
        {
            _db.Update(eventInfo);
            await _db.SaveChangesAsync();
        }

        private async Task Add(Event eventInfo)
        {
            eventInfo.ID = Guid.NewGuid();
            await _db.AddAsync(eventInfo);
            await _db.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IEnumerable<Event>> Delete(Guid id)
        {
            var eventInfo = await _db.Events.FindAsync(id);
            _db.Remove(eventInfo);
            await _db.SaveChangesAsync();
            return _db.Events;
        }
    }
}