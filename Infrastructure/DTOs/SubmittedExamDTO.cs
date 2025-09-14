namespace Infrastructure.DTOs
{
    public class SubmittedExamDTO
    {
        public int EID { get; set; }
        public int UserId { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal? Duration { get; set; }
        public string? Name { get; set; }
        public int? DisplayedQuestions { get; set; }
        public virtual ICollection<ReceivedResponseDTO> Responses { get; set; } = new List<ReceivedResponseDTO>();
    }
}