using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenForCongress.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        public IActionResult Index() => View();
    }
}