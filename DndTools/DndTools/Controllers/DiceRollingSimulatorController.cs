using DndTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DndTools.Models.DiceRoller;
using DndTools.Models.DiceRoller.Helpers;

namespace DndTools.Controllers
{
    public class DiceRollingSimulatorController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public DiceRollingSimulatorController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult RollDice(string advancedDiceNotation)
        {
            try
            {
                return new JsonResult(DiceRoller.Roll(advancedDiceNotation));
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
