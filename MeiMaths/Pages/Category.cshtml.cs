using MeiMaths.Models;
using MeiMaths.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeiMaths.Pages;

public class CategoryModel : PageModel
{
    private readonly ExerciseSetService _exerciseSetService;

    public string CategoryName { get; set; } = "";
    public List<ExerciseSet> ExerciseSets { get; set; } = [];
    public bool IsExam { get; set; }

    public CategoryModel(ExerciseSetService exerciseSetService)
    {
        _exerciseSetService = exerciseSetService;
    }

    public IActionResult OnGet(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RedirectToPage("/Index");

        CategoryName = name;
        IsExam = name.Equals("Eksamensopgaver", StringComparison.OrdinalIgnoreCase);

        ExerciseSets = _exerciseSetService.GetAll()
            .Where(s => s.Category.Equals(name, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.Title)
            .ToList();

        if (ExerciseSets.Count == 0)
            return RedirectToPage("/Index");

        return Page();
    }
}
