using System.Text.Json;
using MeiMaths.Models;

namespace MeiMaths.Services;

public class ExerciseSetService
{
    private readonly string _dataPath;

    public ExerciseSetService(IWebHostEnvironment env)
    {
        var builtInPath = Path.Combine(env.ContentRootPath, "Data", "ExerciseSets");

        if (env.IsProduction())
        {
            // I produktion (Docker): brug persistent volume-sti
            _dataPath = "/app/data/ExerciseSets";
            Directory.CreateDirectory(_dataPath);

            // Kopiér indbyggede JSON-filer til volume (kun hvis de ikke allerede findes)
            if (Directory.Exists(builtInPath))
            {
                foreach (var file in Directory.GetFiles(builtInPath, "*.json"))
                {
                    var dest = Path.Combine(_dataPath, Path.GetFileName(file));
                    if (!File.Exists(dest))
                        File.Copy(file, dest);
                }
            }
        }
        else
        {
            _dataPath = builtInPath;
        }
    }

    public string DataPath => _dataPath;

    public List<ExerciseSet> GetAll()
    {
        var sets = new List<ExerciseSet>();

        if (!Directory.Exists(_dataPath))
            return sets;

        foreach (var file in Directory.GetFiles(_dataPath, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var set = JsonSerializer.Deserialize<ExerciseSet>(json);
                if (set is not null)
                    sets.Add(set);
            }
            catch
            {
                // Skip filer med ugyldigt JSON
            }
        }

        return sets.OrderBy(s => s.Title).ToList();
    }

    public ExerciseSet? GetById(string id)
    {
        return GetAll().FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    public (bool Success, string Message) SaveExerciseSet(string filename, string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(filename) || string.IsNullOrWhiteSpace(jsonContent))
            return (false, "Filnavn og JSON-indhold skal udfyldes.");

        // Sanitize filename
        filename = filename.Trim();
        if (!filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            filename += ".json";

        foreach (var c in Path.GetInvalidFileNameChars())
            filename = filename.Replace(c, '_');

        // Validate JSON
        ExerciseSet? exerciseSet;
        try
        {
            exerciseSet = JsonSerializer.Deserialize<ExerciseSet>(jsonContent);
        }
        catch (JsonException ex)
        {
            return (false, $"Ugyldigt JSON: {ex.Message}");
        }

        if (exerciseSet is null || string.IsNullOrWhiteSpace(exerciseSet.Id))
            return (false, "JSON skal indeholde mindst et \"id\" felt.");

        if (string.IsNullOrWhiteSpace(exerciseSet.Title))
            return (false, "JSON skal indeholde et \"title\" felt.");

        // Check for duplicate id
        var existing = GetAll().FirstOrDefault(s => s.Id.Equals(exerciseSet.Id, StringComparison.OrdinalIgnoreCase));
        var targetPath = Path.Combine(_dataPath, filename);
        if (existing is not null && !File.Exists(targetPath))
            return (false, $"Et opgavesæt med id \"{exerciseSet.Id}\" findes allerede.");

        Directory.CreateDirectory(_dataPath);
        File.WriteAllText(targetPath, jsonContent);

        return (true, $"Opgavesæt \"{exerciseSet.Title}\" gemt som {filename}.");
    }

    public (bool Success, string Message) DeleteExerciseSet(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return (false, "Filnavn mangler.");

        var filePath = Path.Combine(_dataPath, filename);
        if (!File.Exists(filePath))
            return (false, "Filen blev ikke fundet.");

        File.Delete(filePath);
        return (true, $"{filename} er slettet.");
    }
}
