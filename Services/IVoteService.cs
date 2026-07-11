using VoteHub.Models;

namespace VoteHub.Services
{
    public interface IVoteService
    {
        Task<Vote> CastVoteAsync(int electionId, int candidateId, int userId);
        Task<bool> HasUserVotedAsync(int electionId, int userId);
        Task<Vote> GetVoteAsync(int voteId);
        Task<List<Vote>> GetVotesByElectionAsync(int electionId);
        Task<int> GetVoteCountForCandidateAsync(int candidateId);
        Task<Dictionary<int, int>> GetVoteCountsByElectionAsync(int electionId);
        Task<bool> CanVoteAsync(int electionId, int userId);
    }
}