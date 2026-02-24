using MeiMaths.Data;
using MeiMaths.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MeiMaths.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public List<UserStats> Users { get; set; } = [];
    public List<TopicStats> TopicProgress { get; set; } = [];

    public DashboardModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var results = await _db.QuizResults.ToListAsync();

        Users = users.Select(u => new UserStats
        {
            UserId = u.Id,
            UserName = u.UserName ?? "",
            DisplayName = u.DisplayName,
            TotalQuizzes = results.Count(r => r.UserId == u.Id),
            TotalScore = results.Where(r => r.UserId == u.Id).Sum(r => r.Score),
            TotalQuestions = results.Where(r => r.UserId == u.Id).Sum(r => r.TotalQuestions),
            Results = results.Where(r => r.UserId == u.Id).OrderByDescending(r => r.CompletedAt).ToList()
        }).OrderBy(u => u.DisplayName).ToList();

        TopicProgress = results
            .GroupBy(r => r.ExerciseSetTitle)
            .Select(g => new TopicStats
            {
                Title = g.Key,
                TotalAttempts = g.Count(),
                UniqueUsers = g.Select(r => r.UserId).Distinct().Count(),
                TotalCorrect = g.Sum(r => r.Score),
                TotalQuestions = g.Sum(r => r.TotalQuestions)
            })
            .OrderBy(t => t.Title)
            .ToList();
    }
}

public class UserStats
{
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public int TotalQuizzes { get; set; }
    public int TotalScore { get; set; }
    public int TotalQuestions { get; set; }
    public int AveragePercent => TotalQuestions > 0 ? (int)Math.Round(100.0 * TotalScore / TotalQuestions) : 0;
    public List<QuizResult> Results { get; set; } = [];
}

public class TopicStats
{
    public string Title { get; set; } = "";
    public int TotalAttempts { get; set; }
    public int UniqueUsers { get; set; }
    public int TotalCorrect { get; set; }
    public int TotalQuestions { get; set; }
    public int AveragePercent => TotalQuestions > 0 ? (int)Math.Round(100.0 * TotalCorrect / TotalQuestions) : 0;
}
