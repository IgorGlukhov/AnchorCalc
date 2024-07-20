namespace AnchorCalc.Domain.Rest;

public interface IApiRequestExecutor
{
    Task<TResponse?> GetAsync<TResponse>(string request);
}