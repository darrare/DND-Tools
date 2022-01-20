using DndTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.TreasureHoard;

namespace DndTools.Controllers
{
    public class TreasureHoardGeneratorController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public TreasureHoardGeneratorController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult GenerateHoard(string challengeLevel, string currencyRollingGuideJson)
        {
            if (!int.TryParse(challengeLevel, out int challengeLevelAsInt))
            {
                return new JsonResult(new { error = $"Failure to parse {challengeLevel} as an integer." });
            }
            else
            {
                return new JsonResult(TreasureHoardModel.Create(challengeLevelAsInt, currencyRollingGuideJson));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
