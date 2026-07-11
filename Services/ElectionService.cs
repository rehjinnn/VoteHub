using Microsoft.EntityFrameworkCore;
using VoteHub.Data;
using VoteHub.Models;

namespace VoteHub.Services
{
    public class ElectionService : IElectionService
    {
        private readonly VoteHubContext _context;

        public ElectionService(VoteHubContext context)
        {
            _context = context;
        }

        public async Task<Election> CreateElectionAsync(Election election)
        {
            // Validate that the user exists before creating the election
            if (election.UserId.HasValue)  // If UserId is provided
            {
                var userExists = await _context.Users.AnyAsync(u => u.UserId == election.UserId);
                if (!userExists)
                {
                    throw new InvalidOperationException($"User with ID {election.UserId} does not exist.");
                }
            }

            election.CreatedAt = DateTime.Now;
            election.UpdatedAt = DateTime.Now;
            election.Status = "Open";

            _context.Elections.Add(election);
            await _context.SaveChangesAsync();
            return election;
        }

        public async Task<Election> GetElectionByIdAsync(int electionId)
        {
            return await _context.Elections
                .AsSplitQuery()
                .Include(e => e.Creator)
                .Include(e => e.Candidates)
                    .ThenInclude(c => c.Votes)
                .Include(e => e.Votes)
                .FirstOrDefaultAsync(e => e.ElectionId == electionId);
        }

        public async Task<List<Election>> GetAllElectionsAsync()
        {
            return await _context.Elections
                .AsSplitQuery()
                .Include(e => e.Creator)
                .Include(e => e.Candidates)
                    .ThenInclude(c => c.Votes)
                .Include(e => e.Votes)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Election>> GetElectionsByUserAsync(int userId)
        {
            return await _context.Elections
                .AsSplitQuery()
                .Where(e => e.UserId == userId)
                .Include(e => e.Candidates)
                    .ThenInclude(c => c.Votes)
                .Include(e => e.Votes)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Election>> SearchElectionsAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return await GetAllElectionsAsync();

            keyword = keyword.ToLower();
            return await _context.Elections
                .AsSplitQuery()
                .Where(e => e.Title.ToLower().Contains(keyword) ||
                           e.Description.ToLower().Contains(keyword) ||
                           e.Category.ToLower().Contains(keyword))
                .Include(e => e.Creator)
                .Include(e => e.Candidates)
                    .ThenInclude(c => c.Votes)
                .Include(e => e.Votes)
                .ToListAsync();
        }

        public async Task<Election> UpdateElectionAsync(Election election)
        {
            election.UpdatedAt = DateTime.Now;
            _context.Elections.Update(election);
            await _context.SaveChangesAsync();
            return election;
        }

        public async Task<bool> DeleteElectionAsync(int electionId)
        {
            var election = await GetElectionByIdAsync(electionId);
            if (election == null)
                return false;

            _context.Elections.Remove(election);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserEditElectionAsync(int userId, int electionId)
        {
            var election = await GetElectionByIdAsync(electionId);
            return election != null && election.UserId == userId;
        }

        public async Task UpdateElectionStatusAsync()
        {
            var now = DateTime.Now;
            var elections = await _context.Elections.ToListAsync();

            foreach (var election in elections)
            {
                if (election.EndDate < now && election.Status == "Open")
                {
                    election.Status = "Closed";
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}