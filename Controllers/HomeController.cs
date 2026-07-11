using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteHub.Services;
using VoteHub.Data;
using Microsoft.EntityFrameworkCore;

namespace VoteHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly IElectionService _electionService;
        private readonly IStatisticsService _statisticsService;
        private readonly VoteHubContext _context;

        public HomeController(IElectionService electionService, IStatisticsService statisticsService, VoteHubContext context)
        {
            _electionService = electionService;
            _statisticsService = statisticsService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var elections = await _electionService.GetAllElectionsAsync();
                // Filter only open elections
                elections = elections
                    .Where(e => e.Status == "Open" && e.EndDate > DateTime.Now)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList();

                return View(elections);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading elections: " + ex.Message;
                return View(new List<VoteHub.Models.Election>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Account");

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var myElections = await _electionService.GetElectionsByUserAsync(userId);
                var stats = await _statisticsService.GetDashboardStatisticsAsync();

                ViewBag.MyElectionsCount = myElections.Count;
                ViewBag.MyVotesCount = await _context.Votes.CountAsync(v => v.UserId == userId);

                return View(stats);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                ViewBag.MyElectionsCount = 0;
                ViewBag.MyVotesCount = 0;
                return View(new VoteHub.ViewModels.DashboardStatisticsViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            try
            {
                var elections = await _electionService.SearchElectionsAsync(keyword);
                elections = elections
                    .Where(e => e.Status == "Open" && e.EndDate > DateTime.Now)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList();

                ViewBag.Keyword = keyword;
                return View("Index", elections);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}