public interface IEmbedService
{
    Task<EmbedData> GetEmbedDataAsync(string url, int? width = null, int? height = null);
    Task<string> RenderEmbedHtmlAsync(EmbedData data);
    Task RegisterDefaultHandlersAsync();
    Task<bool> AddCustomHandlerAsync(string id, string pattern, Func<string, Dictionary<string, object>, string> handler, int priority = 10);
}