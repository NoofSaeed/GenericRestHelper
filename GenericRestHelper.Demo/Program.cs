using GenericRestHelper.Services;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder => {
    builder.AddConsole().SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<RestClientService>();
using var httpClient = new HttpClient();
var restClient = new RestClientService(httpClient, logger);

string baseUrl = "https://jsonplaceholder.typicode.com/posts";

Console.Clear();
Console.WriteLine("===============================================");
Console.WriteLine("   REST API INTEGRATION TESTS DASHBOARD");
Console.WriteLine("===============================================\n");

await RunTest("GET All Posts", async () => {
    var posts = await restClient.GetAsync<List<PostDto>>(baseUrl);
    return posts != null && posts.Count > 0;
});

await RunTest("GET Single Post (ID: 1)", async () => {
    var post = await restClient.GetAsync<PostDto>($"{baseUrl}/1");
    return post != null && post.Id == 1;
});

await RunTest("POST New Post", async () => {
    var newPost = new PostDto { UserId = 1, Title = "Test Title", Body = "Test Body" };
    var created = await restClient.PostAsync<PostDto, PostDto>(baseUrl, newPost);
    return created != null && created.Id > 0;
});

await RunTest("Error Handling (404 Not Found)", async () => {
    var result = await restClient.GetAsync<PostDto>($"{baseUrl}/invalid/url/path");
    return result == null;
});
await RunTest("PUT Update Post (ID: 1)", async () => {
    var updatedData = new PostDto
    {
        Id = 1,
        UserId = 1,
        Title = "Updated Title",
        Body = "Updated content"
    };
    var result = await restClient.PutAsync<PostDto, PostDto>($"{baseUrl}/1", updatedData);
    return result != null && result.Title == "Updated Title";
});

await RunTest("DELETE Post (ID: 1)", async () => {
    var isDeleted = await restClient.DeleteAsync($"{baseUrl}/1");
    return true;
});
Console.WriteLine("\n===============================================");
Console.WriteLine("        ALL TESTS COMPLETED");
Console.WriteLine("===============================================");
Console.ReadKey();


async Task RunTest(string testName, Func<Task<bool>> testLogic)
{
    Console.Write($"Running: {testName,-40}");
    try
    {
        bool success = await testLogic();
        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[PASSED]");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[FAILED]");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"[EXCEPTION: {ex.Message}]");
    }
    Console.ResetColor();
}

public class PostDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}