using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteHub.Models;
using VoteHub.Services;

namespace VoteHub.Controllers
{
    [Authorize]
    public class CandidateController : Controller
    {
        private readonly ICandidateService _candidateService;
        private readonly IElectionService _electionService;
        private readonly IWebHostEnvironment _environment;

        public CandidateController(ICandidateService candidateService, IElectionService electionService, IWebHostEnvironment environment)
        {
            _candidateService = candidateService;
            _electionService = electionService;
            _environment = environment;
        }

        // GET: Add candidate form
        [HttpGet("Candidate/Add/{electionId}")]
        public async Task<IActionResult> Add(int electionId)
        {
            try
            {
                // Get the election
                var election = await _electionService.GetElectionByIdAsync(electionId);

                if (election == null)
                {
                    TempData["Error"] = "Election not found";
                    return RedirectToAction("Index", "Home");
                }

                // Get current user ID
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Check if user owns this election
                if (election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot add candidates to this election";
                    return RedirectToAction("MyElections");
                }

                // Pass election to view
                ViewBag.ElectionId = election.ElectionId;
                ViewBag.ElectionTitle = election.Title;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Save new candidate
        [HttpPost("Candidate/Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int electionId, string name, string position, string party, string biography, string platform)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(position) || string.IsNullOrWhiteSpace(party))
                {
                    TempData["Error"] = "Name, Position, and Party are required";
                    return RedirectToAction("Add", new { electionId });
                }

                // Get the election
                var election = await _electionService.GetElectionByIdAsync(electionId);
                if (election == null)
                {
                    TempData["Error"] = "Election not found";
                    return RedirectToAction("Index", "Home");
                }

                // Check permission
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot add candidates to this election";
                    return RedirectToAction("MyElections");
                }

                // Create candidate
                var candidate = new Candidate
                {
                    ElectionId = electionId,
                    Name = name.Trim(),
                    Position = position.Trim(),
                    Party = party.Trim(),
                    Biography = (biography ?? "").Trim(),
                    Platform = (platform ?? "").Trim(),
                    Photo = ""
                };

                await _candidateService.CreateCandidateAsync(candidate);
                TempData["Success"] = "Candidate added successfully!";
                return RedirectToAction("Manage", "Election", new { id = electionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding candidate: " + ex.Message;
                return RedirectToAction("Add", new { electionId });
            }
        }

        // GET: Edit candidate
        [HttpGet("Candidate/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var candidate = await _candidateService.GetCandidateByIdAsync(id);
                if (candidate == null)
                {
                    TempData["Error"] = "Candidate not found";
                    return RedirectToAction("Index", "Home");
                }

                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (candidate.Election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot edit this candidate";
                    return RedirectToAction("MyElections");
                }

                ViewBag.ElectionId = candidate.ElectionId;
                return View(candidate);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Update candidate
        [HttpPost("Candidate/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int candidateId, string name, string position, string party, string biography, string platform)
        {
            try
            {
                var candidate = await _candidateService.GetCandidateByIdAsync(candidateId);
                if (candidate == null)
                {
                    TempData["Error"] = "Candidate not found";
                    return RedirectToAction("Index", "Home");
                }

                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (candidate.Election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot edit this candidate";
                    return RedirectToAction("MyElections");
                }

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(position) || string.IsNullOrWhiteSpace(party))
                {
                    TempData["Error"] = "Name, Position, and Party are required";
                    ViewBag.ElectionId = candidate.ElectionId;
                    return View(candidate);
                }

                candidate.Name = name.Trim();
                candidate.Position = position.Trim();
                candidate.Party = party.Trim();
                candidate.Biography = (biography ?? "").Trim();
                candidate.Platform = (platform ?? "").Trim();

                await _candidateService.UpdateCandidateAsync(candidate);
                TempData["Success"] = "Candidate updated successfully!";
                return RedirectToAction("Manage", "Election", new { id = candidate.ElectionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating candidate: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Delete candidate
        [HttpPost("Candidate/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var candidate = await _candidateService.GetCandidateByIdAsync(id);
                if (candidate == null)
                {
                    TempData["Error"] = "Candidate not found";
                    return RedirectToAction("Index", "Home");
                }

                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (candidate.Election.UserId != userId && !User.IsInRole("Admin"))
                {
                    TempData["Error"] = "You cannot delete this candidate";
                    return RedirectToAction("MyElections");
                }

                int electionId = candidate.ElectionId;
                await _candidateService.DeleteCandidateAsync(id);
                TempData["Success"] = "Candidate deleted successfully!";
                return RedirectToAction("Manage", "Election", new { id = electionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting candidate: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        private static bool IsValidCandidate(Candidate candidate, IFormFile? photoFile, out string validationError, bool allowPhotoToBeMissing = false)
        {
            if (candidate == null)
            {
                validationError = "Invalid candidate data.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(candidate.Name) ||
                string.IsNullOrWhiteSpace(candidate.Position) ||
                string.IsNullOrWhiteSpace(candidate.Party))
            {
                validationError = "Please complete all candidate profile fields.";
                return false;
            }

            if (!allowPhotoToBeMissing && (photoFile == null || photoFile.Length == 0))
            {
                validationError = "Candidate photo is required.";
                return false;
            }

            validationError = string.Empty;
            return true;
        }

        private async Task<string> SaveCandidatePhotoAsync(IFormFile photoFile)
        {
            if (photoFile == null || photoFile.Length == 0)
            {
                throw new Exception("Candidate photo is required.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(photoFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Photo must be an image file.");

            var uploadDirectory = Path.Combine(_environment.WebRootPath, "uploads", "candidates");
            Directory.CreateDirectory(uploadDirectory);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await photoFile.CopyToAsync(stream);

            return $"/uploads/candidates/{fileName}";
        }
    }
}