using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QPROS_APITestingAssignment.DataProviders;
using QPROS_APITestingAssignment.Utilities;
using RestSharp;
using System.Net;
using xRetry;
using Xunit.Abstractions;

namespace QPROS_APITestingAssignment.Tests;

public class UserManagementTests : BaseTest
{
    private readonly SchemaValidator _schemaValidator;
    private new readonly ITestOutputHelper _output;

    public UserManagementTests(ITestOutputHelper output) : base(output)
    {
        _output = output;
        _schemaValidator = new SchemaValidator(output);
    }

    [RetryFact]
    public void UserLifecycle_CreateUpdateGetDelete_ShouldSucceed()
    {
        // Arrange: Create User with dynamically generated data
        var createRequest = RequestWithAuth("user");
        createRequest.Method = Method.Post;

        // Use UserDataProvider to generate test data
        var userDataProvider = UserDataProvider.CreateUserData().First();
        var userPayload = (dynamic)userDataProvider[0];

        var payload = new Dictionary<string, object>
        {
            { "id", userPayload.id },
            { "username", userPayload.username },
            { "firstName", userPayload.firstName },
            { "lastName", userPayload.lastName },
            { "email", userPayload.email },
            { "password", userPayload.password },
            { "phone", userPayload.phone },
            { "userStatus", userPayload.userStatus }
        };

        createRequest.AddJsonBody(payload);

        // Act: Create User
        var createResponse = _client.Execute(createRequest);

        // Assert: User Creation
        _output.WriteLine($"Create User Response Status: {createResponse.StatusCode}");
        _output.WriteLine($"Create User Response Content: {createResponse.Content}");
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act & Assert: Get User Details
        var getUserRequest = RequestWithAuth($"user/{userPayload.username}");
        getUserRequest.Method = Method.Get;

        var getUserResponse = _client.Execute(getUserRequest);

        _output.WriteLine($"Get User Response Status: {getUserResponse.StatusCode}");
        _output.WriteLine($"Get User Response Content: {getUserResponse.Content}");
        getUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        // Schema Validation
        bool isValid = _schemaValidator.ValidateJson(getUserResponse.Content ?? string.Empty, "CreateUser");
        Assert.True(isValid, "Schema validation failed for user creation response");

        // Verify retrieved user details match created user
        var retrievedUser = JsonConvert.DeserializeObject<dynamic>(getUserResponse.Content!);
        Assert.NotNull(retrievedUser);
        ((string)retrievedUser!.username).Should().Be(userPayload.username);
        ((string)retrievedUser.firstName).Should().Be(userPayload.firstName);

        // Act: Update User
        var updateRequest = RequestWithAuth($"user/{userPayload.username}");
        updateRequest.Method = Method.Put;

        var updatedUserPayload = new Dictionary<string, object>
        {
            { "id", userPayload.id },
            { "username", userPayload.username },
            { "firstName", $"Updated{userPayload.firstName}" },
            { "lastName", userPayload.lastName },
            { "email", FakeDataGenerator.GenerateEmail() },
            { "password", FakeDataGenerator.GeneratePassword() },
            { "phone", FakeDataGenerator.Faker.Phone.PhoneNumber() },
            { "userStatus", 2 }
        };

        updateRequest.AddJsonBody(updatedUserPayload);

        var updateResponse = _client.Execute(updateRequest);

        // Assert: User Update
        _output.WriteLine($"Update User Response Status: {updateResponse.StatusCode}");
        _output.WriteLine($"Update User Response Content: {updateResponse.Content}");
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act: User Login
        var loginRequest = RequestWithAuth("user/login");
        loginRequest.Method = Method.Get;
        loginRequest.AddQueryParameter("username", payload["username"].ToString());
        loginRequest.AddQueryParameter("password", payload["password"].ToString());

        var loginResponse = _client.Execute(loginRequest);

        // Assert: User Login
        _output.WriteLine($"Login Response Status: {loginResponse.StatusCode}");
        _output.WriteLine($"Login Response Content: {loginResponse.Content}");
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify login-specific headers
        var loginHeaders = loginResponse.Headers!.ToDictionary(h => h.Name, h => h.Value?.ToString());
        loginHeaders.Should().ContainKey("X-Rate-Limit", "Should have rate limit header");
        loginHeaders.Should().ContainKey("X-Expires-After", "Should have expiration header");

        // Act: User Logout
        var logoutRequest = RequestWithAuth("user/logout");
        logoutRequest.Method = Method.Get;

        var logoutResponse = _client.Execute(logoutRequest);

        // Assert: User Logout
        _output.WriteLine($"Logout Response Status: {logoutResponse.StatusCode}");
        _output.WriteLine($"Logout Response Content: {logoutResponse.Content}");
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act: Delete User
        var deleteRequest = RequestWithAuth($"user/{userPayload.username}");
        deleteRequest.Method = Method.Delete;

        var deleteResponse = _client.Execute(deleteRequest);

        // Assert: User Deletion
        _output.WriteLine($"Delete User Response Status: {deleteResponse.StatusCode}");
        _output.WriteLine($"Delete User Response Content: {deleteResponse.Content}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify user is actually deleted by attempting to get the user
        var verifyDeleteRequest = RequestWithAuth($"user/{userPayload.username}");
        verifyDeleteRequest.Method = Method.Get;

        var verifyDeleteResponse = _client.Execute(verifyDeleteRequest);

        // Expect a 404 Not Found for deleted user
        verifyDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [RetryTheory]
    [InlineData("nonexistentuser")]
    public void GetUser_NonExistentUser_ShouldReturnNotFound(string username)
    {
        // Arrange
        var request = RequestWithAuth($"user/{username}");
        request.Method = Method.Get;

        // Act
        var response = _client.Execute(request);

        // Assert
        _output.WriteLine($"Get Non-Existent User Response Status: {response.StatusCode}");
        _output.WriteLine($"Get Non-Existent User Response Content: {response.Content}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [RetryTheory]
    [InlineData("")]
    public void GetUser_InvalidUsername_ShouldReturnMethodNotAllowed(string username)
    {
        // Arrange
        var request = RequestWithAuth($"user/{username}");
        request.Method = Method.Get;

        // Act
        var response = _client.Execute(request);

        // Assert
        _output.WriteLine($"Get Invalid User Response Status: {response.StatusCode}");
        _output.WriteLine($"Get Invalid User Response Content: {response.Content}");

        // Verify status code is Method Not Allowed
        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);

        // Optionally, validate the response body
        var responseBody = JObject.Parse(response.Content!);
        responseBody.Should().ContainKey("code");
        responseBody["code"]!.Value<int>().Should().Be(405);
        responseBody.Should().ContainKey("type");
        responseBody["type"]!.Value<string>().Should().Be("unknown");
    }
}