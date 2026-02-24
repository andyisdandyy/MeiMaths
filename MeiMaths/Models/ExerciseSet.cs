using System.Text.Json.Serialization;

namespace MeiMaths.Models;

public class ExerciseSet
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = "Andet";

    [JsonPropertyName("questions")]
    public List<Question> Questions { get; set; } = [];
}

public class Question
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }

    [JsonPropertyName("correctAnswer")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [JsonPropertyName("hint")]
    public string Hint { get; set; } = string.Empty;

    [JsonPropertyName("graph")]
    public GraphConfig? Graph { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class GraphConfig
{
    [JsonPropertyName("functions")]
    public List<GraphFunction> Functions { get; set; } = [];

    [JsonPropertyName("triangle")]
    public TriangleConfig? Triangle { get; set; }

    [JsonPropertyName("xMin")]
    public double XMin { get; set; } = -10;

    [JsonPropertyName("xMax")]
    public double XMax { get; set; } = 10;

    [JsonPropertyName("yMin")]
    public double YMin { get; set; } = -10;

    [JsonPropertyName("yMax")]
    public double YMax { get; set; } = 10;
}

public class GraphFunction
{
    [JsonPropertyName("expression")]
    public string Expression { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string Color { get; set; } = "#0d6efd";

    [JsonPropertyName("label")]
    public string? Label { get; set; }
}

public class TriangleConfig
{
    [JsonPropertyName("sideA")]
    public double? SideA { get; set; }

    [JsonPropertyName("sideB")]
    public double? SideB { get; set; }

    [JsonPropertyName("sideC")]
    public double? SideC { get; set; }

    [JsonPropertyName("angleA")]
    public double? AngleA { get; set; }

    [JsonPropertyName("angleB")]
    public double? AngleB { get; set; }

    [JsonPropertyName("angleC")]
    public double? AngleC { get; set; }

    [JsonPropertyName("labels")]
    public TriangleLabels? Labels { get; set; }
}

public class TriangleLabels
{
    [JsonPropertyName("sideA")]
    public string? SideA { get; set; }

    [JsonPropertyName("sideB")]
    public string? SideB { get; set; }

    [JsonPropertyName("sideC")]
    public string? SideC { get; set; }

    [JsonPropertyName("angleA")]
    public string? AngleA { get; set; }

    [JsonPropertyName("angleB")]
    public string? AngleB { get; set; }

    [JsonPropertyName("angleC")]
    public string? AngleC { get; set; }
}
