using VoteHub.ViewModels;

namespace VoteHub.Services
{
    public interface IStatisticsService
    {
        Task<ElectionStatisticsViewModel> GetElectionStatisticsAsync(int electionId);
        Task<DashboardStatisticsViewModel> GetDashboardStatisticsAsync();
        Task<AdminDashboardStatisticsViewModel> GetAdminDashboardStatisticsAsync();
    }
}