using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoldenForCongress.Data;
using GoldenForCongress.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoldenForCongress.Controllers
{
    [Route("route")]
    public class RouteController : Controller
    {
        private readonly DB _db;

        public RouteController(DB db)
        {
            _db = db;
        }

        public IEnumerable<Route> Index()
        {
            return _db.Routes;
        }

        [HttpPost]
        public async Task<IEnumerable<Route>> Index([FromBody]Route route)
        {
            if (route.ID == Guid.Empty)
                await Add(route);
            else
                await Update(route);
            return Index();
        }

        private async Task Update(Route route)
        {
            _db.Update(route);
            await _db.SaveChangesAsync();
        }

        private async Task Add(Route route)
        {
            route.ID = Guid.NewGuid();
            await _db.AddAsync(route);
            await _db.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IEnumerable<Route>> Delete(int id)
        {
            var route = await _db.Routes.FindAsync(id);
            _db.Remove(route);
            await _db.SaveChangesAsync();
            return Index();
        }
    }
}
