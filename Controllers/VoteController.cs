using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteHub.Models;
using VoteHub.Services;

namespace VoteHub.Controllers
{
    [Authorize]
    public class VoteController : Controller
    {
        private readonly IVoteService _voteService;
        private readonly IElectionService _electionService;

        public VoteController(IVoteService voteService, IElectionService electionService)
        {
            _voteService = voteService;
            _electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> Ballot(int electionId)
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

                var existingVote = await _voteService.GetVoteAsync(electionId, userId);
                if (existingVote != null)
                {
                    TempData["Error"] = "You have already voted in this election";
                    return RedirectToAction("Details", "Election", new { id = electionId });
                }

                return View(election);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading ballot: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CastVote(int electionId, int candidateId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var existingVote = await _voteService.GetVoteAsync(electionId, userId);
                if (existingVote != null)
                {
                    TempData["Error"] = "You have already voted in this election";
                    return RedirectToAction("Details", "Election", new { id = electionId });
                }

                var vote = new Vote
                {
                    ElectionId = electionId,
                    CandidateId = candidateId,
                    UserId = userId,
                    DateVoted = DateTime.Now
                };

                await _voteService.CastVoteAsync(vote);

                TempData["Success"] = "Your vote has been cast successfully!";
                return RedirectToAction("Confirmation", new { electionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error casting vote: " + ex.Message;
                return RedirectToAction("Ballot", new { electionId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int electionId)
        {
            var election = await _electionService.GetElectionByIdAsync(electionId);
            if (election == null)
            {
                TempData["Error"] = "Election not found";
                return RedirectToAction("Index", "Home");
            }

            return View(election);
        }
    }
}