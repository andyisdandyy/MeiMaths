using MeiMaths.Models;
using MeiMaths.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeiMaths.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ExerciseSetService _exerciseSetService;

        public List<CategoryInfo> Categories { get; set; } = [];

        public IndexModel(ExerciseSetService exerciseSetService)
        {
            _exerciseSetService = exerciseSetService;
        }

        public void OnGet()
        {
            Categories = _exerciseSetService.GetAll()
                .GroupBy(s => s.Category)
                .Select(g => new CategoryInfo { Name = g.Key, Count = g.Count() })
                .OrderBy(c => c.Name)
                .ToList();
        }
    }

    public class CategoryInfo
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }
}
