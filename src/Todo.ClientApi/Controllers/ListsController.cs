// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;

namespace Todo.ClientApi.Controllers;

[ApiController]
[Route("/todo/lists")]
public class ListsController : ControllerBase
{
    private readonly IConfiguration Configuration;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IHttpClientFactory httpClientFactory;

    public ListsController(IConfiguration configuration, IHttpClientFactory factory, IHttpContextAccessor contextAccessor)
    {
        this.Configuration = configuration;
        this.httpClientFactory = factory;
        this.contextAccessor = contextAccessor;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(List<TodoList>))]
    public async Task<TodoListPage> GetListsAsync([FromQuery(Name = "$skip")] int? skip = null, [FromQuery(Name = "$top")] int? batchSize = null)
    {
        var baseUrl = this.Configuration["TodoApiSettings:BaseUrl"];
        var swaggerClient = new swaggerClient(baseUrl, httpClientFactory.CreateClient());
        return await swaggerClient.ListsGETAsync(skip, batchSize);
    }

    [HttpGet("{list_id}/items")]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(List<TodoList>))]
    public async Task<TodoItemPage> GetItemsAsync([FromRoute(Name = "list_id")] string listId, [FromQuery(Name = "$skip")] int? skip = null, [FromQuery(Name = "$top")] int? batchSize = null)
    {
        var baseUrl = this.Configuration["TodoApiSettings:BaseUrl"];
        var swaggerClient = new swaggerClient(baseUrl, httpClientFactory.CreateClient());
        return await swaggerClient.ItemsGETAsync(listId, skip, batchSize);
    }
}
