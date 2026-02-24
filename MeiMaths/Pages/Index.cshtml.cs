using MeiMaths.Models;
using MeiMaths.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MeiMaths.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ExerciseSetService _exerciseSetService;

        public List<ExerciseSet> ExerciseSets { get; set; } = [];

        public IndexModel(ExerciseSetService exerciseSetService)
        {
            _exerciseSetService = exerciseSetService;
        }

        public void OnGet()
        {
            ExerciseSets = _exerciseSetService.GetAll();
        }
    }
}
