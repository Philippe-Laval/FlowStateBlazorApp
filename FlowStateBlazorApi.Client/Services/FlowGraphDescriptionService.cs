using FlowStateBlazor.Data.Models;
using System.Net.Http.Json;

namespace FlowStateBlazorApi.Client.Services
{
    public class FlowGraphDescriptionService
    {
        private HttpClient _httpClient;

        public FlowGraphDescriptionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Basic request (get_all, get_one, update, remove, insert)

        public Task<List<FlowGraphDescription>?> GetElementsAsync(CancellationToken cancellationToken = default)
        {
            return _httpClient.GetFromJsonAsync<List<FlowGraphDescription>>("api/FlowGraphDescriptions", cancellationToken);
        }
        public Task<FlowGraphDescription?> GetElementAsync(int id, CancellationToken cancellationToken = default)
        {
            return _httpClient.GetFromJsonAsync<FlowGraphDescription>($"api/FlowGraphDescriptions/{id}", cancellationToken);
        }

        public Task InsertElementAsync(FlowGraphDescription element, CancellationToken cancellationToken = default)
        {
            return _httpClient.PostAsJsonAsync<FlowGraphDescription>("api/FlowGraphDescriptions", element, cancellationToken);
        }

        public Task UpdateElementAsync(FlowGraphDescription element, CancellationToken cancellationToken = default)
        {
            return _httpClient.PutAsJsonAsync<FlowGraphDescription>($"api/FlowGraphDescriptions/{element.Id}", element, cancellationToken);
        }

        public Task DeleteElementAsync(FlowGraphDescription element, CancellationToken cancellationToken = default)
        {
            return _httpClient.DeleteAsync($"api/FlowGraphDescriptions/{element.Id}", cancellationToken);
        }

        public Task DeleteElementAsync(int id, CancellationToken cancellationToken = default)
        {
            return _httpClient.DeleteAsync($"api/FlowGraphDescriptions/{id}", cancellationToken);
        }

        #endregion
    }
}
