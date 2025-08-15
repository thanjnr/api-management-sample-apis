// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Todo.ClientApi.Services;

namespace Todo.ClientApi.Controllers;

[ApiController]
[Route("/anonymous/todo/lists")]
public class AnonymousListsController : ControllerBase
{
    private readonly InternalApiSettings apiSettings;
    private readonly IAuthService authService;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IHttpClientFactory httpClientFactory;

    public AnonymousListsController(IOptions<InternalApiSettings> settings, IAuthService authService, IHttpClientFactory factory, IHttpContextAccessor contextAccessor)
    {
        this.apiSettings = settings.Value;
        this.authService = authService;
        this.httpClientFactory = factory;
        this.contextAccessor = contextAccessor;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(List<TodoList>))]
    public async Task<TodoListPage> GetListsAsync([FromQuery(Name = "$skip")] int? skip = null, [FromQuery(Name = "$top")] int? batchSize = null)
    {
        var swaggerClient = new swaggerClient(this.apiSettings.BaseUrl, GetHttpClient());
        return await swaggerClient.ListsGETAsync(skip, batchSize);
    }

    [HttpGet("{list_id}/items")]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(List<TodoList>))]
    public async Task<TodoItemPage> GetItemsAsync([FromRoute(Name = "list_id")] string listId, [FromQuery(Name = "$skip")] int? skip = null, [FromQuery(Name = "$top")] int? batchSize = null)
    {
        var swaggerClient = new swaggerClient(this.apiSettings.BaseUrl, GetHttpClient());
        return await swaggerClient.ItemsGETAsync(listId, skip, batchSize);
    }

    private HttpClient GetHttpClient()
    {
        var client = httpClientFactory.CreateClient();

        return client;
    }
}
