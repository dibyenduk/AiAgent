using AgentService;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

var endpoint = new Uri("Url");
var deploymentName = "gpt-4.1";
var apiKey = "<API Key>";

var azureClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));

//Simple agent run
var aiAgent = azureClient.GetChatClient(deploymentName).CreateAIAgent();
var response = await aiAgent.RunAsync("What is the capital of France ?");
Console.WriteLine(Environment.NewLine);
System.Console.WriteLine(response);
Console.WriteLine("Input Token: " + response?.Usage?.InputTokenCount + " tokens" + "Output Token: " + response?.Usage?.OutputTokenCount + " tokens");

// Streaming agent run
var updates = new List<AgentRunResponseUpdate>();
await foreach (AgentRunResponseUpdate update in aiAgent.RunStreamingAsync("How to make soup ?"))
{
    updates.Add(update);
    System.Console.Write(update);
}

Console.WriteLine(Environment.NewLine);
response = updates.ToAgentRunResponse();
Console.WriteLine("Input Token count: " + response?.Usage?.InputTokenCount);
Console.WriteLine("Output Token count: " + response?.Usage?.OutputTokenCount);

// Agent with tools
var agent = azureClient.GetChatClient(deploymentName).CreateAIAgent(
    instructions: "You are a time expert",
    tools: new List<AITool>()
    {
        AIFunctionFactory.Create(Tools.GetCurrentDateAndTime, "current_date_and_time"),
        AIFunctionFactory.Create(Tools.GetCurrentTimeZone, "current_timezone")
    });

AgentThread thread = agent.GetNewThread();

while(true)
{
    Console.Write("> ");
    string? userInput = Console.ReadLine();
    ChatMessage message = new ChatMessage(ChatRole.User, userInput);
    var timeResponse = await agent.RunAsync(message, thread);
    Console.WriteLine(timeResponse);
    Console.WriteLine(Environment.NewLine);
}