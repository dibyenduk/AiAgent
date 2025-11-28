using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI;

var endpoint = new Uri("<url>");
var deploymentName = "gpt-4.1";
var apiKey = "<API Token>";

var azureClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));


var aiAgent = azureClient.GetChatClient(deploymentName).CreateAIAgent();

//var response = await aiAgent.RunAsync("What is the capital of France ?");
//System.Console.WriteLine(response);

await foreach(AgentRunResponseUpdate update in aiAgent.RunStreamingAsync("How to make soup ?"))
{
    System.Console.Write(update);
}