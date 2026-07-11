using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteHub.Data;
using VoteHub.Services;
using Microsoft.EntityFrameworkCore;

namespace VoteHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly VoteHubContext _context;
        private readonly IStatisticsService _statisticsService;
        private readonly IElectionService _electionService;

        public AdminController(VoteHubContext context, IStatisticsService statisticsService, IElectionService electionService)
        {
            _context = context;
            _statisticsService = statisticsService;
            _electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _statisticsService.GetAdminDashboardStatisticsAsync();
            return View(stats);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Elections()
        {
            var elections = await _electionService.GetAllElectionsAsync();
            return View(elections);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteElection(int id)
        {
            try
            {
                await _electionService.DeleteElectionAsync(id);
                TempData["Success"] = "Election deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Elections");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound();

                user.IsActive = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "User deactivated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Users");
        }
    }
}