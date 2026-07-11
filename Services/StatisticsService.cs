using Microsoft.EntityFrameworkCore;
using VoteHub.Data;
using VoteHub.ViewModels;

namespace VoteHub.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly VoteHubContext _context;

        public StatisticsService(VoteHubContext context)
        {
            _context = context;
        }

        public async Task<ElectionStatisticsViewModel> GetElectionStatisticsAsync(int electionId)
        {
            var election = await _context.Elections
                .Include(e => e.Candidates)
                .Include(e => e.Votes)
                .FirstOrDefaultAsync(e => e.ElectionId == electionId);

            if (election == null)
                return null;

            var totalVotes = election.Votes.Count;
            var candidateStats = new List<CandidateStatisticViewModel>();

            foreach (var candidate in election.Candidates)
            {
                var voteCount = election.Votes.Count(v => v.CandidateId == candidate.CandidateId);
                var percentage = totalVotes > 0 ? (voteCount * 100.0) / totalVotes : 0;

                candidateStats.Add(new CandidateStatisticViewModel
                {
                    CandidateId = candidate.CandidateId,
                    Name = candidate.Name,
                    Party = candidate.Party,
                    Position = candidate.Position,
                    VoteCount = voteCount,
                    Percentage = percentage
                });
            }

            candidateStats = candidateStats.OrderByDescending(c => c.VoteCount).ToList();

            return new ElectionStatisticsViewModel
            {
                ElectionId = election.ElectionId,
                ElectionTitle = election.Title,
                TotalVotes = totalVotes,
                TotalCandidates = election.Candidates.Count,
                Winner = candidateStats.FirstOrDefault(),
                CandidateStatistics = candidateStats
            };
        }

        public async Task<DashboardStatisticsViewModel> GetDashboardStatisticsAsync()
        {
            var totalElections = await _context.Elections.CountAsync();
            var totalVotes = await _context.Votes.CountAsync();
            var openElections = await _context.Elections.CountAsync(e => e.Status == "Open");

            return new DashboardStatisticsViewModel
            {
                TotalElections = totalElections,
                TotalVotes = totalVotes,
                OpenElections = openElections
            };
        }

        public async Task<AdminDashboardStatisticsViewModel> GetAdminDashboardStatisticsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalElections = await _context.Elections.CountAsync();
            var totalVotes = await _context.Votes.CountAsync();
            var totalCandidates = await _context.Candidates.CountAsync();
            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            return new AdminDashboardStatisticsViewModel
            {
                TotalUsers = totalUsers,
                TotalElections = totalElections,
                TotalVotes = totalVotes,
                TotalCandidates = totalCandidates,
                RecentUsers = recentUsers
            };
        }
    }
}