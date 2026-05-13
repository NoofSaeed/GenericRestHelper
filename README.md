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

Integrating **GenericRestHelper** into an MVC project is straightforward thanks to its native support for Dependency Injection (DI).

### 1. Register the Service

In your MVC project's `Program.cs` file, register the `HttpClient` and the `RestClientService`:

```csharp
// Add HttpClient support
builder.Services.AddHttpClient();

// Register the GenericRestHelper Service
builder.Services.AddScoped<GenericRestHelper.Services.RestClientService>();

```

### 2. Inject into Controller

Inject the service into your Controller’s constructor to start consuming APIs:

```csharp
public class ProductsController : Controller
{
    private readonly RestClientService _restClient;

    public ProductsController(RestClientService restClient)
    {
        _restClient = restClient;
    }

    public async Task<IActionResult> Index()
    {
        var url = "https://api.example.com/products";
        
        // Consume the API in one line
        var products = await _restClient.GetAsync<List<ProductDto>>(url);

        return View(products);
    }
}

```

### 3. Display Data in View

Receive the DTOs in your Razor View (`Index.cshtml`):

```html
@model List<ProductDto>

<table class="table">
    @foreach (var item in Model) {
        <tr>
            <td>@item.Name</td>
            <td>@item.Price</td>
        </tr>
    }
</table>

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
To consume secured APIs that require a Bearer Token, you can easily configure the HttpClient during registration in your Program.cs.

Option 1: Static Token (Quick Setup):
If you have a fixed token, you can set it globally for the service:

``` C#
builder.Services.AddHttpClient<RestClientService>(client =>
{
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "your_access_token_here");
});
```
Option 2: Dynamic Token (Advanced / Recommended): 
For dynamic tokens (e.g., tokens from user sessions or IdentityServer), the best practice is to use a DelegatingHandler. This keeps your code clean and handles token logic automatically for every request:

```C#
// 1. Create a custom handler
public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Fetch your token dynamically (from a service, cache, or storage)
        var token = "dynamic_token_logic_here"; 
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return await base.SendAsync(request, cancellationToken);
    }
}

// 2. Register it in Program.cs
builder.Services.AddTransient<AuthHeaderHandler>();
builder.Services.AddHttpClient<RestClientService>()
                .AddHttpMessageHandler<AuthHeaderHandler>();
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

