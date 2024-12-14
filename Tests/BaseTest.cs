using AventStack.ExtentReports;
using QPROS_APITestingAssignment.Utilities;
using RestSharp;
using Xunit.Abstractions;

namespace QPROS_APITestingAssignment.Tests;

public class BaseTest : IAsyncLifetime, IDisposable
{
    protected RestClient _client;
    protected ExtentTest _test;
    private static ExtentReports? _extent;
    protected readonly ITestOutputHelper _output;

    public BaseTest(ITestOutputHelper output)
    {
        _output = output;
        // Update the base URL to match the Swagger documentation
        _client = new RestClient("https://petstore.swagger.io/v2/");

        // Initialize Extent Reports if not already initialized
        if (_extent == null)
        {
            _extent = InitializationForExtentReports.InitializeExtentReports();
        }

        // Create a test in Extent Reports
        _test = _extent.CreateTest(GetType().Name);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    // Method to add default headers for authentication or API requirements
    protected void AddDefaultHeaders(RestRequest request)
    {
        // Add any default headers that might be required
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");

        // If API requires an API key or authentication token
        // request.AddHeader("api_key", "your-api-key-here");
    }

    protected RestRequest RequestWithAuth(string resource)
    {
        var request = new RestRequest(resource);
        AddDefaultHeaders(request);
        return request;
    }

    protected RestRequest RequestWithoutAuth(string resource)
    {
        var request = new RestRequest(resource);
        AddDefaultHeaders(request);
        return request;
    }

    public void Dispose()
    {
        // Flush Extent Reports when all tests are completed
        InitializationForExtentReports.FlushExtentReports();
    }
}