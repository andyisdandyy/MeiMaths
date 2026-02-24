using MeiMaths.Data;
using MeiMaths.Models;
using MeiMaths.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MeiMaths.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ExerciseSetService _exerciseSetService;

    public List<UserStats> Users { get; set; } = [];
    public List<TopicStats> TopicProgress { get; set; } = [];
    public List<ExerciseSetInfo> ExerciseSets { get; set; } = [];

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public bool StatusSuccess { get; set; }

    [BindProperty]
    public string NewFileName { get; set; } = "";

    [BindProperty]
    public string NewJsonContent { get; set; } = "";

    public DashboardModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ExerciseSetService exerciseSetService)
    {
        _db = db;
        _userManager = userManager;
        _exerciseSetService = exerciseSetService;
    }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostRefreshAsync()
    {
        StatusMessage = "Opgavesæt genindlæst fra disk.";
        StatusSuccess = true;
        await LoadDataAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddExerciseSetAsync()
    {
        var (success, message) = _exerciseSetService.SaveExerciseSet(NewFileName, NewJsonContent);
        StatusMessage = message;
        StatusSuccess = success;
        await LoadDataAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteExerciseSetAsync(string filename)
    {
        var (success, message) = _exerciseSetService.DeleteExerciseSet(filename);
        StatusMessage = message;
        StatusSuccess = success;
        await LoadDataAsync();
        return RedirectToPage();
    }

    private async Task LoadDataAsync()
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

        // Load exercise sets with filenames
        var dataPath = _exerciseSetService.DataPath;
        ExerciseSets = [];

        if (Directory.Exists(dataPath))
        {
            foreach (var file in Directory.GetFiles(dataPath, "*.json").OrderBy(f => f))
            {
                var fileName = Path.GetFileName(file);

                ExerciseSet? set = null;
                try
                {
                    var json = System.IO.File.ReadAllText(file);
                    set = System.Text.Json.JsonSerializer.Deserialize<ExerciseSet>(json);
                }
                catch { }

                ExerciseSets.Add(new ExerciseSetInfo
                {
                    FileName = fileName,
                    Title = set?.Title ?? "(ugyldigt JSON)",
                    Id = set?.Id ?? "",
                    Category = set?.Category ?? "",
                    QuestionCount = set?.Questions.Count ?? 0,
                    IsValid = set is not null && !string.IsNullOrEmpty(set.Id)
                });
            }
        }
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

public class ExerciseSetInfo
{
    public string FileName { get; set; } = "";
    public string Title { get; set; } = "";
    public string Id { get; set; } = "";
    public string Category { get; set; } = "";
    public int QuestionCount { get; set; }
    public bool IsValid { get; set; }
}
