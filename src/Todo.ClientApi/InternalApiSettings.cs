namespace Todo.ClientApi
{
    public class InternalApiSettings
    {
        public required string BaseUrl { get; set; }
        public required string ClientId { get; set; }
        public required string Scope { get; set; }
        public required string Secret { get; set; }
        public required string TenantId { get; set; }
        public required string TokenUrl { get; set; }
    }
}
