using VoteHub.Models;

namespace VoteHub.ViewModels
{
    public class AdminDashboardStatisticsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalElections { get; set; }
        public int TotalVotes { get; set; }
        public int TotalCandidates { get; set; }
        public List<User> RecentUsers { get; set; }
    }
}