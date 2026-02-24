using MeiMaths.Data;
using MeiMaths.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MeiMaths.Pages.Account;

[Authorize]
public class MyResultsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public List<QuizResult> Results { get; set; } = [];
    public int TotalQuizzes { get; set; }
    public int TotalScore { get; set; }
    public int TotalQuestions { get; set; }
    public int AveragePercent => TotalQuestions > 0 ? (int)Math.Round(100.0 * TotalScore / TotalQuestions) : 0;
    public List<CategoryProgress> CategoryStats { get; set; } = [];

    public MyResultsModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
            return;

        Results = await _db.QuizResults
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CompletedAt)
            .ToListAsync();

        TotalQuizzes = Results.Count;
        TotalScore = Results.Sum(r => r.Score);
        TotalQuestions = Results.Sum(r => r.TotalQuestions);

        CategoryStats = Results
            .GroupBy(r => r.ExerciseSetTitle)
            .Select(g => new CategoryProgress
            {
                Title = g.Key,
                Attempts = g.Count(),
                BestScore = g.Max(r => r.TotalQuestions > 0 ? (int)Math.Round(100.0 * r.Score / r.TotalQuestions) : 0),
                LatestScore = g.OrderByDescending(r => r.CompletedAt).First().Score,
                LatestTotal = g.OrderByDescending(r => r.CompletedAt).First().TotalQuestions
            })
            .OrderBy(c => c.Title)
            .ToList();
    }
}

public class CategoryProgress
{
    public string Title { get; set; } = "";
    public int Attempts { get; set; }
    public int BestScore { get; set; }
    public int LatestScore { get; set; }
    public int LatestTotal { get; set; }
    public int LatestPercent => LatestTotal > 0 ? (int)Math.Round(100.0 * LatestScore / LatestTotal) : 0;
}
