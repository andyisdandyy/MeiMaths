using MeiMaths.Data;
using MeiMaths.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MeiMaths.Pages.Account;

[Authorize]
public class MyDataModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MyDataModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostDownloadAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToPage("/Account/Login");

        var results = await _db.QuizResults
            .Where(r => r.UserId == user.Id)
            .OrderByDescending(r => r.CompletedAt)
            .Select(r => new
            {
                r.ExerciseSetTitle,
                r.Score,
                r.TotalQuestions,
                r.CompletedAt
            })
            .ToListAsync();

        var exportData = new
        {
            Konto = new
            {
                Brugernavn = user.UserName,
                Visningsnavn = user.DisplayName,
                OprettetSamtykke = user.PrivacyPolicyAcceptedAt
            },
            QuizResultater = results,
            EksporteretTidspunkt = DateTime.UtcNow
        };

        var json = JsonSerializer.SerializeToUtf8Bytes(exportData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return File(json, "application/json", $"meimaths-mine-data-{DateTime.UtcNow:yyyy-MM-dd}.json");
    }
}