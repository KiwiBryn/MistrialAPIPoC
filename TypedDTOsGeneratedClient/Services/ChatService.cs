using System.Net.Http.Json;
using System.Text.Json;
using MistralAIConsole.Models;
using Microsoft.Extensions.Options;

namespace MistralAIConsole.Services;

public class ChatService : IChatService
{
   private readonly HttpClient _httpClient;
   private readonly MistralAIConfiguration _config;

   public ChatService(HttpClient httpClient, IOptions<MistralAIConfiguration> config)
   {
      _httpClient = httpClient;
      _config = config.Value;
   }

   public async Task<ChatCompletionResponse> GetChatCompletionAsync(ChatCompletionRequest request)
   {
      var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
      response.EnsureSuccessStatusCode();

      var content = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<ChatCompletionResponse>(content) ??
             throw new InvalidOperationException("Failed to deserialize response");
   }
}
