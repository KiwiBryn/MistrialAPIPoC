using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Http;
using MistralAIConsole.Models;
using MistralAIConsole.Services;
using Microsoft.Extensions.Options;
using Polly;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

var services = new ServiceCollection();

services.Configure<AppConfiguration>(configuration);
services.Configure<MistralAIConfiguration>(configuration.GetSection("MistralAI"));

/* Original code with resilience handler configuration
services.AddHttpClient<IChatService, ChatService>(client =>
    {
        var mistralConfig = configuration.GetSection("MistralAI").Get<MistralAIConfiguration>();
        client.BaseAddress = new Uri(mistralConfig!.ApiUrl);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["MistralAI:ApiKey"]}");
    })
    .AddStandardResilienceHandler()
    .Configure<StandardResilienceOptions>(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromSeconds(1);
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
    });
*/

// Smallest fix for the resilience handler
services.AddHttpClient<IChatService, ChatService>(client =>
{
   var mistralConfig = configuration.GetSection("MistralAI").Get<MistralAIConfiguration>();
   client.BaseAddress = new Uri(mistralConfig!.ApiUrl);
   client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["MistralAI:ApiKey"]}");
}).AddStandardResilienceHandler();

/* Copilot's fix for the resilience handler
services.AddHttpClient<IChatService, ChatService>(client =>
{
   var mistralConfig = configuration.GetSection("MistralAI").Get<MistralAIConfiguration>();
   client.BaseAddress = new Uri(mistralConfig!.ApiUrl);
   client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["MistralAI:ApiKey"]}");
})
.AddStandardResilienceHandler()
.Configure(options =>
{
   // Retry
   options.Retry.MaxRetryAttempts = 3;
   options.Retry.Delay            = TimeSpan.FromSeconds(1);
   options.Retry.BackoffType      = DelayBackoffType.Exponential;

   // Circuit breaker — SamplingDuration must be >= 2 * AttemptTimeout.Timeout
   options.CircuitBreaker.SamplingDuration  = TimeSpan.FromSeconds(30);   // was 10 → invalid
   options.CircuitBreaker.FailureRatio      = 0.5;
   options.CircuitBreaker.MinimumThroughput = 5;
   options.CircuitBreaker.BreakDuration     = TimeSpan.FromSeconds(30);

   // Give the retry budget room:
   //   MaxRetryAttempts * (Retry.Delay + AttemptTimeout.Timeout) <= TotalRequestTimeout.Timeout
   //   3 * (1s + 10s) = 33s  →  must raise TotalRequestTimeout above 33s
   options.AttemptTimeout.Timeout      = TimeSpan.FromSeconds(10);
   options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
});
*/

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("MistralAI Console Client");
Console.WriteLine("Enter your prompt (or press Enter to exit):");

while (true)
{
   Console.Write("> ");
   var prompt = Console.ReadLine();

   if (string.IsNullOrWhiteSpace(prompt))
   {
      Console.WriteLine("Exiting...");
      break;
   }

   try
   {
      var chatService = serviceProvider.GetRequiredService<IChatService>();
      var config = serviceProvider.GetRequiredService<IOptions<MistralAIConfiguration>>().Value;

      var request = new ChatCompletionRequest
      {
         Model = config.Model,
         MaxTokens = config.MaxTokens,
         Temperature = config.Temperature,
         Messages = new List<ChatMessage>
            {
                new() { Role = "user", Content = prompt }
            }
      };

      Console.WriteLine("Sending request to MistralAI...");
      var response = await chatService.GetChatCompletionAsync(request);

      Console.WriteLine("\nResponse:");
      Console.WriteLine(response.Choices.FirstOrDefault()?.Message?.Content ?? "No response content");
      Console.WriteLine($"\nTokens used: {response.Usage.TotalTokens}");
   }
   catch (Exception ex)
   {
      Console.WriteLine($"Error: {ex.Message}");
   }

   Console.WriteLine();
}

await Task.CompletedTask;
