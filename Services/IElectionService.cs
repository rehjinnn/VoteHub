using VoteHub.Models;

namespace VoteHub.Services
{
    public interface IElectionService
    {
        Task<Election> CreateElectionAsync(Election election);
        Task<Election> GetElectionByIdAsync(int electionId);
        Task<List<Election>> GetAllElectionsAsync();
        Task<List<Election>> GetElectionsByUserAsync(int userId);
        Task<List<Election>> SearchElectionsAsync(string keyword);
        Task<Election> UpdateElectionAsync(Election election);
        Task<bool> DeleteElectionAsync(int electionId);
        Task<bool> CanUserEditElectionAsync(int userId, int electionId);
        Task UpdateElectionStatusAsync();
    }
}