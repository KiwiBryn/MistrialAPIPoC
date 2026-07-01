using MistralAIConsole.Models;

namespace MistralAIConsole.Services;

public interface IChatService
{
   Task<ChatCompletionResponse> GetChatCompletionAsync(ChatCompletionRequest request);
}