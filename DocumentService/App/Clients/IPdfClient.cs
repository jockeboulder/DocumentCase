namespace Clients;
public interface IPdfClient
{
    Task<HttpResponseMessage> CreateAsync(Guid documentNumber, string customerNumber, string documentText);
    Task<HttpResponseMessage> GetAsync(Guid documentNumber, string customerNumber);
}