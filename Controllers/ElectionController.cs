using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteHub.Models;
using VoteHub.Services;

namespace VoteHub.Controllers
{
    [Authorize]
    public class ElectionController : Controller
    {
        private readonly IElectionService _electionService;
        private readonly ICandidateService _candidateService;

        public ElectionController(IElectionService electionService, ICandidateService candidateService)
        {
            _electionService = electionService;
            _candidateService = candidateService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string description, string category, DateTime startDate, DateTime endDate)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    TempData["Error"] = "User not authenticated. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    TempData["Error"] = "Election title is required";
                    return View();
                }

                if (string.IsNullOrWhiteSpace(category))
                {
                    TempData["Error"] = "Category is required";
                    return View();
                }

                if (startDate >= endDate)
                {
                    TempData["Error"] = "End date must be after start date";
                    return View();
                }

                var election = new Election
                {
                    UserId = userId,
                    Title = title.Trim(),
                    Description = (description ?? string.Empty).Trim(),
                    Category = category.Trim(),
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = "Open",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var createdElection = await _electionService.CreateElectionAsync(election);
                TempData["Success"] = "Election created successfully! Now add candidates.";
                return RedirectToAction("Manage", new { id = createdElection.ElectionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating election: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int id)
        {
            try
            {
                var election = await _electionService.GetElectionByIdAsync(id);
                if (election == null)
                {
                    TempData["Error"] = "Election not found";
                    return RedirectToAction("Index", "Home");
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot manage this election";
                    return RedirectToAction("Index", "Home");
                }

                return View(election);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int electionId, string title, string description, string category, DateTime startDate, DateTime endDate)
        {
            try
            {
                var election = await _electionService.GetElectionByIdAsync(electionId);
                if (election == null)
                {
                    TempData["Error"] = "Election not found";
                    return RedirectToAction("Index", "Home");
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot edit this election";
                    return RedirectToAction("Manage", new { id = electionId });
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    TempData["Error"] = "Title is required";
                    return RedirectToAction("Manage", new { id = electionId });
                }

                election.Title = title.Trim();
                election.Description = (description ?? string.Empty).Trim();
                election.Category = category.Trim();
                election.StartDate = startDate;
                election.EndDate = endDate;
                election.UpdatedAt = DateTime.Now;

                await _electionService.UpdateElectionAsync(election);
                TempData["Success"] = "Election updated successfully!";
                return RedirectToAction("Manage", new { id = electionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating election: " + ex.Message;
                return RedirectToAction("Manage", new { id = electionId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var election = await _electionService.GetElectionByIdAsync(id);
                if (election == null)
                {
                    TempData["Error"] = "Election not found";
                    return RedirectToAction("Index", "Home");
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot delete this election";
                    return RedirectToAction("Index", "Home");
                }

                await _electionService.DeleteElectionAsync(id);
                TempData["Success"] = "Election deleted successfully!";
                return RedirectToAction("MyElections");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting election: " + ex.Message;
                return RedirectToAction("Manage", new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyElections()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var elections = await _electionService.GetElectionsByUserAsync(userId);
                return View(elections);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return View(new List<Election>());
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var election = await _electionService.GetElectionByIdAsync(id);
                if (election == null)
                {
                    TempData["Error"] = "Election not found";
                    return RedirectToAction("Index", "Home");
                }

                return View(election);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}