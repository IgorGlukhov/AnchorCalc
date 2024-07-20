using AnchorCalc.Domain.Rest;
using Newtonsoft.Json;

namespace AnchorCalc.Infrastructure.Rest;

internal class ApiRequestExecutor(IHttpClientFactory httpClientFactory) : IApiRequestExecutor
{
    private readonly Uri _baseAddress = new("http://localhost:32768");

    public async Task<TResponse?> GetAsync<TResponse>(string request)
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = _baseAddress;
        var httpResponseMessage = await httpClient.GetAsync(request);
        var content = await httpResponseMessage.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<TResponse>(content);
        return response;
    }
}