using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI;

var endpoint = new Uri("<Url>");
var deploymentName = "gpt-4.1";
var apiKey = "<API Key>";

var azureClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));


var aiAgent = azureClient.GetChatClient(deploymentName).CreateAIAgent();

var response = await aiAgent.RunAsync("What is the capital of France ?");
Console.WriteLine(Environment.NewLine);
System.Console.WriteLine(response);
Console.WriteLine("Input Token: " + response?.Usage?.InputTokenCount + " tokens" + "Output Token: " + response?.Usage?.OutputTokenCount + " tokens");


var updates = new List<AgentRunResponseUpdate>();
await foreach(AgentRunResponseUpdate update in aiAgent.RunStreamingAsync("How to make soup ?"))
{
    updates.Add(update);
    System.Console.Write(update);
}

Console.WriteLine(Environment.NewLine);
response = updates.ToAgentRunResponse();
Console.WriteLine("Input Token count: " + response?.Usage?.InputTokenCount);
Console.WriteLine("Output Token count: " + response?.Usage?.OutputTokenCount);