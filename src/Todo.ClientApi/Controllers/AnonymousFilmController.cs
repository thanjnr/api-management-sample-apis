// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Todo.ClientApi.Controllers;

[ApiController]
[Route("/anonymous/api/films")]
public class AnonymousFilmController : ControllerBase
{
    private readonly InternalApiSettings apiSettings;
    private readonly IHttpClientFactory httpClientFactory;

    public AnonymousFilmController(IOptions<InternalApiSettings> settings, IHttpClientFactory factory, IHttpContextAccessor contextAccessor)
    {
        this.apiSettings = settings.Value;
        this.httpClientFactory = factory;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(ICollection<Film>))]
    public async Task<ICollection<Film>> GetListsAsync()
    {
        var swaggerClient = new swagger1Client(this.apiSettings.BaseUrl, GetHttpClient());
        return await swaggerClient.FilmAllAsync();
    }

    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(Film))]
    public async Task<Film> GetItemsAsync([FromRoute(Name = "id")] int id)
    {
        var swaggerClient = new swagger1Client(this.apiSettings.BaseUrl, GetHttpClient());
        return await swaggerClient.FilmAsync(id);
    }

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();

        return client;
    }
}
