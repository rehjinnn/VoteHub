using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteHub.Services;

namespace VoteHub.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IElectionService _electionService;

        public StatisticsController(IStatisticsService statisticsService, IElectionService electionService)
        {
            _statisticsService = statisticsService;
            _electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            var election = await _electionService.GetElectionByIdAsync(id);
            if (election == null)
                return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Only creator or admin can view results
            if (election.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You don't have permission to view these results.";
                return RedirectToAction("Details", "Election", new { id });
            }

            var stats = await _statisticsService.GetElectionStatisticsAsync(id);
            return View(stats);
        }
    }
}