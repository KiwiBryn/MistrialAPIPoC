using System.Text.Json.Serialization;

namespace MistralAIConsole.Models;

public class ChatCompletionRequest
{
   [JsonPropertyName("model")]
   public string Model { get; set; } = string.Empty;

   [JsonPropertyName("messages")]
   public List<ChatMessage> Messages { get; set; } = new();

   [JsonPropertyName("max_tokens")]
   public int MaxTokens { get; set; } = 1024;

   [JsonPropertyName("temperature")]
   public double Temperature { get; set; } = 0.7;
}

public class ChatMessage
{
   [JsonPropertyName("role")]
   public string Role { get; set; } = "user";

   [JsonPropertyName("content")]
   public string Content { get; set; } = string.Empty;
}

public class ChatCompletionResponse
{
   [JsonPropertyName("id")]
   public string Id { get; set; } = string.Empty;

   [JsonPropertyName("object")]
   public string Object { get; set; } = string.Empty;

   [JsonPropertyName("created")]
   public long Created { get; set; }

   [JsonPropertyName("model")]
   public string Model { get; set; } = string.Empty;

   [JsonPropertyName("choices")]
   public List<ChatChoice> Choices { get; set; } = new();

   [JsonPropertyName("usage")]
   public UsageInfo Usage { get; set; } = new();
}

public class ChatChoice
{
   [JsonPropertyName("index")]
   public int Index { get; set; }

   [JsonPropertyName("message")]
   public ChatMessage Message { get; set; } = new();

   [JsonPropertyName("finish_reason")]
   public string FinishReason { get; set; } = string.Empty;
}

public class UsageInfo
{
   [JsonPropertyName("prompt_tokens")]
   public int PromptTokens { get; set; }

   [JsonPropertyName("completion_tokens")]
   public int CompletionTokens { get; set; }

   [JsonPropertyName("total_tokens")]
   public int TotalTokens { get; set; }
}
