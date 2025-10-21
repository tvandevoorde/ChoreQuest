using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChoreQuest.Api.DTOs;
using Xunit;

namespace ChoreQuest.Api.Tests;

public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public AccountControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkAndToken()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.Token);
        Assert.Equal("testuser", authResponse.Username);
        Assert.Equal("test@example.com", authResponse.Email);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var request1 = new RegisterRequest
        {
            Username = "duplicate",
            Email = "user1@example.com",
            Password = "password123"
        };

        var request2 = new RegisterRequest
        {
            Username = "duplicate",
            Email = "user2@example.com",
            Password = "password123"
        };

        // Act
        await _client.PostAsJsonAsync("/api/account/register", request1);
        var response = await _client.PostAsJsonAsync("/api/account/register", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var request1 = new RegisterRequest
        {
            Username = "user1",
            Email = "duplicate@example.com",
            Password = "password123"
        };

        var request2 = new RegisterRequest
        {
            Username = "user2",
            Email = "duplicate@example.com",
            Password = "password123"
        };

        // Act
        await _client.PostAsJsonAsync("/api/account/register", request1);
        var response = await _client.PostAsJsonAsync("/api/account/register", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "loginuser",
            Email = "login@example.com",
            Password = "password123"
        };

        await _client.PostAsJsonAsync("/api/account/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Username = "loginuser",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.Token);
        Assert.Equal("loginuser", authResponse.Username);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "testpassword",
            Email = "testpass@example.com",
            Password = "correctpassword"
        };

        await _client.PostAsJsonAsync("/api/account/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Username = "testpassword",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonexistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "nonexistent",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/account/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ReturnsOkAndProfile()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "profileuser",
            Email = "profile@example.com",
            Password = "password123"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/account/register", registerRequest);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authResponse!.Token);

        // Act
        var response = await _client.GetAsync("/api/account/profile");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>(_jsonOptions);
        Assert.NotNull(profile);
        Assert.Equal("profileuser", profile.Username);
        Assert.Equal("profile@example.com", profile.Email);
    }

    [Fact]
    public async Task GetProfile_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/account/profile");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ReturnsOkAndUpdatedProfile()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "updateuser",
            Email = "update@example.com",
            Password = "password123"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/account/register", registerRequest);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authResponse!.Token);

        var updateRequest = new UpdateProfileRequest
        {
            Email = "newemail@example.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/account/profile", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>(_jsonOptions);
        Assert.NotNull(profile);
        Assert.Equal("newemail@example.com", profile.Email);
    }

    [Fact]
    public async Task UpdateProfile_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var user1Request = new RegisterRequest
        {
            Username = "user1update",
            Email = "user1@example.com",
            Password = "password123"
        };

        var user2Request = new RegisterRequest
        {
            Username = "user2update",
            Email = "user2@example.com",
            Password = "password123"
        };

        await _client.PostAsJsonAsync("/api/account/register", user1Request);
        var registerResponse = await _client.PostAsJsonAsync("/api/account/register", user2Request);
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authResponse!.Token);

        var updateRequest = new UpdateProfileRequest
        {
            Email = "user1@example.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/account/profile", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
