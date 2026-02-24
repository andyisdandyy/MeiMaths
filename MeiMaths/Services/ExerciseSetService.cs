using System.Text.Json;
using MeiMaths.Models;

namespace MeiMaths.Services;

public class ExerciseSetService
{
    private readonly string _dataPath;

    public ExerciseSetService(IWebHostEnvironment env)
    {
        _dataPath = Path.Combine(env.ContentRootPath, "Data", "ExerciseSets");
    }

    public List<ExerciseSet> GetAll()
    {
        var sets = new List<ExerciseSet>();

        if (!Directory.Exists(_dataPath))
            return sets;

        foreach (var file in Directory.GetFiles(_dataPath, "*.json"))
        {
            var json = File.ReadAllText(file);
            var set = JsonSerializer.Deserialize<ExerciseSet>(json);
            if (set is not null)
                sets.Add(set);
        }

        return sets.OrderBy(s => s.Title).ToList();
    }

    public ExerciseSet? GetById(string id)
    {
        return GetAll().FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }
}
