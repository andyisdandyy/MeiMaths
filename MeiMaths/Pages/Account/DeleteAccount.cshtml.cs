using MeiMaths.Data;
using MeiMaths.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MeiMaths.Pages.Account;

[Authorize]
public class DeleteAccountModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public DeleteAccountModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToPage("/Account/Login");

        // Bekræft med adgangskode
        if (!await _userManager.CheckPasswordAsync(user, Password))
        {
            ErrorMessage = "Forkert adgangskode. Din konto blev ikke slettet.";
            return Page();
        }

        // Slet alle quiz-resultater for brugeren
        var results = await _db.QuizResults
            .Where(r => r.UserId == user.Id)
            .ToListAsync();

        _db.QuizResults.RemoveRange(results);
        await _db.SaveChangesAsync();

        // Slet brugerkontoen
        await _userManager.DeleteAsync(user);

        // Log brugeren ud
        await _signInManager.SignOutAsync();

        return RedirectToPage("/AccountDeleted");
    }
}