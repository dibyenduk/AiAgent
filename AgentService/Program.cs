using AgentService;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.Reflection;
using System.Text;
using System.Threading;

var endpoint = new Uri("");
var deploymentName = "gpt-4.1";
var apiKey = "";

var azureClient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));

//Simple agent run
var aiAgent = azureClient.GetChatClient(deploymentName).CreateAIAgent();

//var response = await aiAgent.RunAsync("What is the capital of France ?");
//Console.WriteLine(Environment.NewLine);
//System.Console.WriteLine(response);
//Console.WriteLine("Input Token: " + response?.Usage?.InputTokenCount + " tokens" + "Output Token: " + response?.Usage?.OutputTokenCount + " tokens");

//// Streaming agent run
//var updates = new List<AgentRunResponseUpdate>();
//await foreach (AgentRunResponseUpdate update in aiAgent.RunStreamingAsync("How to make soup ?"))
//{
//    updates.Add(update);
//    System.Console.Write(update);
//}

//Console.WriteLine(Environment.NewLine);
//response = updates.ToAgentRunResponse();
//Console.WriteLine("Input Token count: " + response?.Usage?.InputTokenCount);
//Console.WriteLine("Output Token count: " + response?.Usage?.OutputTokenCount);

// Agent with tools
//var agent = azureClient.GetChatClient(deploymentName).CreateAIAgent(
//    instructions: "You are a time expert",
//    tools: new List<AITool>()
//    {
//        AIFunctionFactory.Create(Tools.GetCurrentDateAndTime, "current_date_and_time"),
//        AIFunctionFactory.Create(Tools.GetCurrentTimeZone, "current_timezone")
//    });

//AgentThread thread = agent.GetNewThread();

//while(true)
//{
//    Console.Write("> ");
//    string? userInput = Console.ReadLine();
//    ChatMessage message = new ChatMessage(ChatRole.User, userInput);
//    var timeResponse = await agent.RunAsync(message, thread);
//    Console.WriteLine(timeResponse);
//    Console.WriteLine(Environment.NewLine);
//}

// Agent with File Tools

// Get all the public methods in FileSystemTools through reflection
var methods = typeof(FileSystemTools).GetMethods(BindingFlags.Public | BindingFlags.Instance);

var dangerousMethod = DangerousMethods.DoSomethingDangerous;

List<AITool> fileTools = methods
    .Select(m => AIFunctionFactory.Create(m, new FileSystemTools())).Cast<AITool>().ToList();

#pragma warning disable MEAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
fileTools.Add(new ApprovalRequiredAIFunction(AIFunctionFactory.Create(DangerousMethods.DoSomethingDangerous)));
#pragma warning restore MEAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var fileToolsAgent = azureClient.GetChatClient(deploymentName).CreateAIAgent(
    instructions: "You are a file system expert. When working with the file system tools, provide the full path of the file or directory.",
   tools: fileTools).AsBuilder().Use(FunctionCallMiddleware).Build();

AgentThread fileToolsThread = fileToolsAgent.GetNewThread();

while(true)
{
   Console.Write("> ");
   string? userInput = Console.ReadLine();
   ChatMessage message = new ChatMessage(ChatRole.User, userInput);
   var fileToolsResponse = await fileToolsAgent.RunAsync(message, fileToolsThread);
   
   var userInputsRequests = fileToolsResponse.UserInputRequests.ToList();   
   while(userInputsRequests.Count > 0)
   {
#pragma warning disable MEAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        List<ChatMessage> userInputResponses = userInputsRequests.
            OfType<FunctionApprovalRequestContent>()
            .Select(functionApprovalRequestContent => {
                Console.WriteLine("Agent is requesting approval for the following action:");
                Console.WriteLine(functionApprovalRequestContent.ToString());
                Console.Write("Do you approve? (yes/no): ");
                return new ChatMessage(ChatRole.User, [functionApprovalRequestContent.CreateResponse(Console.ReadLine()?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)]);
            }).ToList();

#pragma warning restore MEAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        
        fileToolsResponse = await fileToolsAgent.RunAsync(userInputResponses, fileToolsThread);
        userInputsRequests = fileToolsResponse.UserInputRequests.ToList();
    }
   Console.WriteLine(Environment.NewLine);
   Console.WriteLine(fileToolsResponse);   
}

async ValueTask<object?> FunctionCallMiddleware(AIAgent callingAgent, FunctionInvocationContext context, Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken cancellationToken)
{
    StringBuilder functionCallDetails = new();
    functionCallDetails.Append($"Tool Call Details: {context.Function.Name}");
    if(context.Arguments != null && context.Arguments != null)
    {
        functionCallDetails.Append(" with parameters: ");
        if (context.Arguments.Count > 0)
        {
            foreach (var arg in context.Arguments)
            {
                functionCallDetails.Append($"{arg.Key} = {arg.Value}, ");
            }            
        }        
    }

    Console.WriteLine(functionCallDetails.ToString());
    return await next(context, cancellationToken);
}
