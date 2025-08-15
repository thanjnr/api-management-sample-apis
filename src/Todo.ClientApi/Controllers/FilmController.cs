// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Todo.ClientApi.Services;

namespace Todo.ClientApi.Controllers;

[ApiController]
[Route("/api/films")]
public class FilmController : ControllerBase
{
    private readonly InternalApiSettings apiSettings;
    private readonly ITokenClientHelper authService;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IHttpClientFactory httpClientFactory;

    public FilmController(IOptions<InternalApiSettings> settings, ITokenClientHelper authService, IHttpClientFactory factory, IHttpContextAccessor contextAccessor)
    {
        this.apiSettings = settings.Value;
        this.authService = authService;
        this.httpClientFactory = factory;
        this.contextAccessor = contextAccessor;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(ICollection<Film>))]
    public async Task<ICollection<Film>> GetFilmsAsync()
    {
        var swaggerClient = new swagger1Client(this.apiSettings.BaseUrl, await GetHttpClient());
        return await swaggerClient.FilmAllAsync();
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(Film))]
    public async Task<Film> GetFilmAsync([FromRoute(Name = "id")] int id)
    {
        var swaggerClient = new swagger1Client(this.apiSettings.BaseUrl, await GetHttpClient());
        return await swaggerClient.FilmAsync(id);
    }

    private async Task<HttpClient> GetHttpClient()
    {
        var token = await this.authService.GetAccessTokenAsync();
        throw new Exception(token);
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", token);

        return client;
    }
}
