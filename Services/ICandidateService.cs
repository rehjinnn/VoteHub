using VoteHub.Models;

namespace VoteHub.Services
{
    public interface ICandidateService
    {
        Task<Candidate> CreateCandidateAsync(Candidate candidate);
        Task<Candidate> GetCandidateByIdAsync(int candidateId);
        Task<List<Candidate>> GetCandidatesByElectionAsync(int electionId);
        Task<Candidate> UpdateCandidateAsync(Candidate candidate);
        Task<bool> DeleteCandidateAsync(int candidateId);
        Task<bool> CanUserEditCandidateAsync(int userId, int candidateId);
    }
}