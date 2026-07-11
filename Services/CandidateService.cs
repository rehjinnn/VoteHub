using Microsoft.EntityFrameworkCore;
using VoteHub.Data;
using VoteHub.Models;

namespace VoteHub.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly VoteHubContext _context;

        public CandidateService(VoteHubContext context)
        {
            _context = context;
        }

        public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
        {
            candidate.CreatedAt = DateTime.Now;
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<Candidate> GetCandidateByIdAsync(int candidateId)
        {
            return await _context.Candidates
                .Include(c => c.Election)
                .FirstOrDefaultAsync(c => c.CandidateId == candidateId);
        }

        public async Task<List<Candidate>> GetCandidatesByElectionAsync(int electionId)
        {
            return await _context.Candidates
                .Where(c => c.ElectionId == electionId)
                .Include(c => c.Votes)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Candidate> UpdateCandidateAsync(Candidate candidate)
        {
            _context.Candidates.Update(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<bool> DeleteCandidateAsync(int candidateId)
        {
            var candidate = await GetCandidateByIdAsync(candidateId);
            if (candidate == null)
                return false;

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserEditCandidateAsync(int userId, int candidateId)
        {
            var candidate = await GetCandidateByIdAsync(candidateId);
            return candidate != null && candidate.Election.UserId == userId;
        }
    }
}