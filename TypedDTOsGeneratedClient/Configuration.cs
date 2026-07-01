namespace MistralAIConsole.Models;

public class MistralAIConfiguration
{
   public string ApiUrl { get; set; } = string.Empty;
   public string Model { get; set; } = string.Empty;
   public int MaxTokens { get; set; } = 1024;
   public double Temperature { get; set; } = 0.7;
}

public class AppConfiguration
{
   public MistralAIConfiguration MistralAI { get; set; } = new();
}