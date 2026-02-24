namespace MeiMaths.Models;

public class QuizResult
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public string ExerciseSetId { get; set; } = string.Empty;
    public string ExerciseSetTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime CompletedAt { get; set; }
}
