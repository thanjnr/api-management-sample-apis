namespace Todo.ClientApi.Services
{
    public interface IAuthService
    {
        Task<string> GetAccessTokenAsync();
    }
}