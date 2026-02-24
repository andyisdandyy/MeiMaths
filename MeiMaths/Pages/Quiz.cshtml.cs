using System.Globalization;
using MeiMaths.Data;
using MeiMaths.Models;
using MeiMaths.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeiMaths.Pages
{
    public class QuizModel : PageModel
    {
        private readonly ExerciseSetService _exerciseSetService;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExerciseSet? ExerciseSet { get; set; }

        public QuizModel(ExerciseSetService exerciseSetService, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _exerciseSetService = exerciseSetService;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult OnGet(string id)
        {
            ExerciseSet = _exerciseSetService.GetById(id);

            if (ExerciseSet is null)
                return RedirectToPage("/Index");

            return Page();
        }

        public IActionResult OnPostCheckAnswer([FromBody] CheckAnswerRequest request)
        {
            var set = _exerciseSetService.GetById(request.SetId);
            if (set is null)
                return NotFound();

            var question = set.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
            if (question is null)
                return NotFound();

            var userAnswer = request.Answer?.Trim() ?? "";
            var correctAnswer = question.CorrectAnswer;

            bool isCorrect;

            var userNormalized = userAnswer.Replace(',', '.');
            var correctNormalized = correctAnswer.Replace(',', '.');

            if (double.TryParse(userNormalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var userNum) &&
                double.TryParse(correctNormalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var correctNum))
            {
                isCorrect = Math.Abs(userNum - correctNum) < 0.01;
            }
            else
            {
                isCorrect = correctAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);
            }

            return new JsonResult(new { isCorrect, correctAnswer });
        }

        public async Task<IActionResult> OnPostSaveResultAsync([FromBody] SaveResultRequest request)
        {
            if (User.Identity?.IsAuthenticated != true)
                return new JsonResult(new { saved = false });

            var userId = _userManager.GetUserId(User);
            if (userId is null)
                return new JsonResult(new { saved = false });

            _db.QuizResults.Add(new QuizResult
            {
                UserId = userId,
                ExerciseSetId = request.SetId,
                ExerciseSetTitle = request.SetTitle,
                Score = request.Score,
                TotalQuestions = request.TotalQuestions,
                CompletedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return new JsonResult(new { saved = true });
        }
    }

    public class CheckAnswerRequest
    {
        public string SetId { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        public string? Answer { get; set; }
    }

    public class SaveResultRequest
    {
        public string SetId { get; set; } = string.Empty;
        public string SetTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
    }
}
