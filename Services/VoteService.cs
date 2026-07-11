using Microsoft.EntityFrameworkCore;
using VoteHub.Data;
using VoteHub.Models;

namespace VoteHub.Services
{
    public class VoteService : IVoteService
    {
        private readonly VoteHubContext _context;

        public VoteService(VoteHubContext context)
        {
            _context = context;
        }

        public async Task<Vote> CastVoteAsync(int electionId, int candidateId, int userId)
        {
            if (await HasUserVotedAsync(electionId, userId))
                throw new Exception("You have already voted in this election");

            var election = await _context.Elections.FindAsync(electionId);
            if (election?.Status != "Open" || election.EndDate < DateTime.Now)
                throw new Exception("This election is closed");

            var vote = new Vote
            {
                ElectionId = electionId,
                CandidateId = candidateId,
                UserId = userId,
                DateVoted = DateTime.Now
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
            return vote;
        }

        public async Task<bool> HasUserVotedAsync(int electionId, int userId)
        {
            return await _context.Votes
                .AnyAsync(v => v.ElectionId == electionId && v.UserId == userId);
        }

        public async Task<Vote> GetVoteAsync(int voteId)
        {
            return await _context.Votes.FindAsync(voteId);
        }

        public async Task<List<Vote>> GetVotesByElectionAsync(int electionId)
        {
            return await _context.Votes
                .Where(v => v.ElectionId == electionId)
                .Include(v => v.Candidate)
                .Include(v => v.User)
                .ToListAsync();
        }

        public async Task<int> GetVoteCountForCandidateAsync(int candidateId)
        {
            return await _context.Votes
                .CountAsync(v => v.CandidateId == candidateId);
        }

        public async Task<Dictionary<int, int>> GetVoteCountsByElectionAsync(int electionId)
        {
            var votes = await _context.Votes
                .Where(v => v.ElectionId == electionId)
                .GroupBy(v => v.CandidateId)
                .Select(g => new { CandidateId = g.Key, Count = g.Count() })
                .ToListAsync();

            return votes.ToDictionary(x => x.CandidateId, x => x.Count);
        }

        public async Task<bool> CanVoteAsync(int electionId, int userId)
        {
            var election = await _context.Elections.FindAsync(electionId);
            if (election == null || election.Status != "Open" || election.EndDate < DateTime.Now)
                return false;

            return !await HasUserVotedAsync(electionId, userId);
        }
    }
}