namespace VoteHub.ViewModels
{
    public class CandidateStatisticViewModel
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string Party { get; set; }
        public string Position { get; set; }
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }

    public class ElectionStatisticsViewModel
    {
        public int ElectionId { get; set; }
        public string ElectionTitle { get; set; }
        public int TotalVotes { get; set; }
        public int TotalCandidates { get; set; }
        public CandidateStatisticViewModel Winner { get; set; }
        public List<CandidateStatisticViewModel> CandidateStatistics { get; set; }
    }
}