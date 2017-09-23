using System.Collections.Generic;
using System.Threading.Tasks;
using GoldenForCongress.Data;
using GoldenForCongress.Models;
using GoldenForCongress.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenForCongress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LocationController : Controller
    {
        private readonly DB _db;
        private readonly SPOTAPI _spotAPI;

        public LocationController(DB db, SPOTAPI spotAPI)
        {
            _db = db;
            _spotAPI = spotAPI;
        }

        public IEnumerable<Location> Index() => _db.Locations;

        public bool Status() => _spotAPI.Running;

        [HttpPost]
        public bool StartTracking()
        {
#pragma warning disable 4014
            if(!_spotAPI.Running)
                _spotAPI.Start();
#pragma warning restore 4014
            return _spotAPI.Running;
        }

        [HttpPost]
        public bool StopTracking()
        {
            _spotAPI.Stop();
            return _spotAPI.Running;
        }

        [HttpDelete]
        public async Task<IEnumerable<Location>> Delete()
        {
            await _db.ClearLocationHistory();
            return _db.Locations;
        }
    }
}