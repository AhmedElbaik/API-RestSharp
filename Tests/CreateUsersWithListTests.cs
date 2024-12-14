using FluentAssertions;
using Newtonsoft.Json.Linq;
using QPROS_APITestingAssignment.DataProviders;
using QPROS_APITestingAssignment.Utilities;
using RestSharp;
using System.Net;
using xRetry;
using Xunit.Abstractions;

namespace QPROS_APITestingAssignment.Tests;

public class CreateUsersWithListTests : BaseTest
{
    private readonly SchemaValidator _schemaValidator;
    private new readonly ITestOutputHelper _output;

    public CreateUsersWithListTests(ITestOutputHelper output) : base(output)
    {
        _output = output;
        _schemaValidator = new SchemaValidator(output);
    }

    [RetryTheory]
    [MemberData(nameof(UserDataProvider.CreateMultipleUserData), MemberType = typeof(UserDataProvider))]
    public void CreateUsersWithList_ShouldReturnSuccessfulResponse(dynamic[] userPayloads)
    {
        // Arrange
        var request = RequestWithAuth("user/createWithList");
        request.Method = Method.Post;

        // Explicitly convert to list of dictionaries to ensure proper serialization
        var payload = userPayloads.Select(user => new Dictionary<string, object>
        {
            { "id", user.id },
            { "username", user.username },
            { "firstName", user.firstName },
            { "lastName", user.lastName },
            { "email", user.email },
            { "password", user.password },
            { "phone", user.phone },
            { "userStatus", user.userStatus }
        }).ToList();

        // Add the JSON body
        request.AddJsonBody(payload);

        // Act
        var response = _client.Execute(request);

        // Log the response for debugging
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {response.Content}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Parse the response
        var responseData = JObject.Parse(response.Content!);

        // Validate response structure
        responseData.Should().ContainKey("code");
        responseData["code"]!.Value<int>().Should().Be(200);

        responseData.Should().ContainKey("type");
        responseData["type"]!.Value<string>().Should().Be("unknown");

        responseData.Should().ContainKey("message");
        responseData["message"]!.Value<string>().Should().Be("ok");

        // Schema Validation
        //bool isValid = _schemaValidator.ValidateJson(response.Content ?? string.Empty, "CreateUser");
        //Assert.True(isValid, "Schema validation failed for user creation response");
    }
}