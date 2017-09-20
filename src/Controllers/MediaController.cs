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
    public class MediaController : Controller
    {
        private readonly DB _db;
        private readonly IHostingEnvironment _env;

        public MediaController(DB db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IEnumerable<Media> Cache()
        {
            var media = _db.Media;
            var json = JArray.FromObject(media, Startup.SnakeCase);
            foreach (var item in json)
                item["location"] = JObject.Parse(item["location"].Value<string>());
            System.IO.File.WriteAllText(Path.Combine(_env.WebRootPath, "media.json"), json.ToString());
            return media;
        }

        public IEnumerable<Media> Index() => _db.Media;

        [HttpPost]
        public async Task<IEnumerable<Media>> Index([FromBody]JObject mediaJSON)
        {
            var media = mediaJSON.ToObject<Media>(Startup.SnakeCase);
            if (media.ID == Guid.Empty)
                await Add(media);
            else
                await Update(media);
            return _db.Media;
        }

        private async Task Update(Media mediaInfo)
        {
            _db.Update(mediaInfo);
            await _db.SaveChangesAsync();
        }

        private async Task Add(Media mediaInfo)
        {
            mediaInfo.ID = Guid.NewGuid();
            await _db.AddAsync(mediaInfo);
            await _db.SaveChangesAsync();
        }

        [HttpDelete]
        public async Task<IEnumerable<Media>> Delete(Guid id)
        {
            var media = await _db.Media.FindAsync(id);
            _db.Remove(media);
            await _db.SaveChangesAsync();
            return _db.Media;
        }
    }
}