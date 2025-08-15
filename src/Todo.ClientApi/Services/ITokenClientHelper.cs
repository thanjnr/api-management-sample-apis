namespace Todo.ClientApi.Services
{
    public interface ITokenClientHelper
    {
        Task<string> GetAccessTokenAsync();

        Task<string> GetManagedIdentityAccessTokenAsync();
    }
}