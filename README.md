## GenericRestHelper 🚀

**GenericRestHelper** is a lightweight, high-performance .NET class library designed for **Consuming RESTful APIs**. Built on top of `HttpClient`, it abstracts the complexities of HTTP networking, serialization, and error handling, allowing developers to interact with any API using a clean, generic interface.

---

## ✨ Key Features

* **Generic CRUD Operations**: Perform `GET`, `POST`, `PUT`, and `DELETE` with any DTO (Data Transfer Object).
* **Automated Serialization**: Seamlessly handles JSON-to-Object mapping using `System.Text.Json`.
* **Built-in Resilience**: Returns default values on API failure to prevent application crashes.
* **Integrated Logging**: Native support for `ILogger` to track API errors and lifecycle events.
* **Standardized Error Handling**: Centralized logic for handling status codes like 404, 500, and 400.

---

## 🛠 Project Structure

* **`GenericRestHelper`**: The core library containing the `RestClientService`.
* **`GenericRestHelper.Demo`**: A console-based dashboard for live integration testing.
* **`GenericRestHelper.Tests`**: A comprehensive suite of Unit Tests using **xUnit** and **Moq**.

---

## 🚀 ASP.NET Core MVC Integration

The library is designed to integrate seamlessly with the ASP.NET Core dependency injection system. By using the AddGenericRestClient extension, you can configure your API base address and security handlers in one place.

### 1. Registration (Program.cs)
Register the service and configure the underlying HttpClient. You can also chain multiple handlers for tasks like logging, authentication, or retries.

```csharp
using GenericRestHelper.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register the Generic Rest Client
builder.Services.AddGenericRestClient(options =>
{
    options.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<AuthHeaderHandler>(); // Attach custom logic easily

builder.Services.AddControllersWithViews();

```
### 2. Usage in Controllers
Once registered, simply inject IRestClientService into your controllers. The service will automatically handle serialization and deserialization using the configured client.

``` c#
public class ExpertsController : Controller
{
    private readonly IRestClientService _apiClient;

    public ExpertsController(IRestClientService apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        // One-line API call with automatic deserialization
        var response = await _apiClient.GetAsync<ApiResponse<List<ExpertReadDto>>>("api/experts");
        
        if (response != null && response.IsSuccess)
        {
            return View(response.Data);
        }

        return View(new List<ExpertReadDto>());
    }
}

```

---

## 📊 Testing & Quality Assurance

### Unit Testing

The library is 100% testable. We use **Moq** to simulate various API scenarios:

* **Success Paths**: Validates correct mapping of JSON to C# objects.
* **Fail Paths**: Ensures 404/500 errors are logged without interrupting the UI flow.

### Integration Dashboard

The Demo project provides a color-coded CLI dashboard to verify real-world API connectivity:

* **Green [PASSED]**: Successful data retrieval and integrity.
* **Red [FAILED]**: API returned a non-success status code.
* **Dark Red [EXCEPTION]**: Connection or network-level failure.

---

### 📊 Test-Driven Reliability
To ensure maximum stability, the library includes a built-in Integration Dashboard that validates all REST methods against live endpoints:

<img width="1073" height="598" alt="Integration Test Dashboard" src="https://github.com/user-attachments/assets/92b3fad0-9990-40d2-a157-ec03b6e454fe" />

*The console output above demonstrates successful handling of GET, POST, PUT, and DELETE operations, including automated error logging.*

---
### 🔐 Handling Authentication (Bearer Tokens)
To consume secured APIs that require a Bearer Token, the library now supports the standard .NET IHttpClientBuilder pattern, allowing for clean and automated token injection.

Option 1: Static Token (Quick Setup)
If you have a fixed token, you can set it directly during service registration:

``` C#

builder.Services.AddGenericRestClient(client =>
{
    client.BaseAddress = new Uri("https://api.yourdomain.com/");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "your_static_token_here");
});
```
Option 2: Dynamic Token (Standard & Recommended)
For dynamic tokens (e.g., stored in Cookies or Session), use a DelegatingHandler. This keeps your business logic clean by automatically attaching the token to every outgoing request.

1. Create a Custom Handler:

``` C#
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Example: Get token from Cookies
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```
2. Register the Handler with the Service:
Thanks to the latest refactor, you can now chain the handler directly:

``` C#
// Register the handler first
builder.Services.AddTransient<AuthHeaderHandler>();

// Register the client and attach the handler
builder.Services.AddGenericRestClient(client => 
{
    client.BaseAddress = new Uri("https://api.yourdomain.com/");
})
.AddHttpMessageHandler<AuthHeaderHandler>(); // Chaining support!

```
---
## ⚙️ Configuration

The service is pre-configured with `PropertyNameCaseInsensitive = true`, ensuring compatibility between `camelCase` (JSON) and `PascalCase` (C#) properties automatically.

---

## 📝 License

This project is open-source and available under the [MIT License](https://www.google.com/search?q=LICENSE).

---

### Author

*Professional API Consumer Wrapper for .NET Developers.*

---

