﻿using System.Net;
using FastEndpoints;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using Justwish.Users.WebApi;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.FunctionalTests;

public sealed class RefreshTests
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public RefreshTests()
    {
        _factory = new TestWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Doesnt_Refresh_Not_Event_Token()
    {
        // Arrange
        const string fakeToken = "looksLikeItIsNotEvenAToken.OkayIWillAddSomeDots.AndDotsMore";
        
        // Act
        var response =
            await _client.POSTAsync<RefreshEndpoint, RefreshEndpoint.RefreshRequest, RefreshEndpoint.RefreshResponse>(
                new RefreshEndpoint.RefreshRequest(fakeToken));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    [Fact]
    public async Task Doesnt_Refresh_Token_ValidPayload_InvalidSignature()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
        User user1 = await scope.ServiceProvider.GetRequiredService<IUserReadRepository>()
            .GetUserByUsernameAsync(TestData.User1.Username);
        
        var issued = await jwtService.IssueAsync(user1);

        var fakeRefreshToken = new JwtToken(issued.RefreshToken.Token + "A"); // Just destroying verify-signature with odd letter.
        
        // Act
        var response =
            await _client.POSTAsync<RefreshEndpoint, RefreshEndpoint.RefreshRequest, RefreshEndpoint.RefreshResponse>(
                new RefreshEndpoint.RefreshRequest(fakeRefreshToken.Token));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    [Fact]
    public async Task Doesnt_Refresh_InvalidatedToken()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
        User user1 = await scope.ServiceProvider.GetRequiredService<IUserReadRepository>()
            .GetUserByUsernameAsync(TestData.User1.Username);
        
        var issued = await jwtService.IssueAsync(user1);
        await jwtService.InvalidateRefreshTokenAsync(issued.RefreshToken);

        var fakeRefreshToken = new JwtToken(issued.RefreshToken.Token + "A"); // Just destroying verify-signature with odd letter.
        
        // Act
        var response =
            await _client.POSTAsync<RefreshEndpoint, RefreshEndpoint.RefreshRequest, RefreshEndpoint.RefreshResponse>(
                new RefreshEndpoint.RefreshRequest(fakeRefreshToken.Token));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    [Fact]
    public async Task Refreshes_ValidToken()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
        User user1 = await scope.ServiceProvider.GetRequiredService<IUserReadRepository>()
            .GetUserByUsernameAsync(TestData.User1.Username);
        
        var issued = await jwtService.IssueAsync(user1);

        // Act
        var response =
            await _client.POSTAsync<RefreshEndpoint, RefreshEndpoint.RefreshRequest, RefreshEndpoint.RefreshResponse>(
                new RefreshEndpoint.RefreshRequest(issued.RefreshToken.Token));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.NotEqual(issued.RefreshToken.Token, response.Result.RefreshToken);
        Assert.NotEqual(issued.AccessToken.Token, response.Result.AccessToken);
    }
}