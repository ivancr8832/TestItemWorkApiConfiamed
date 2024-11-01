using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Shared.Uitls;
using System.Net.Http.Json;

namespace ItemWorks.Api.Application.Core.Services.User
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseApi<IEnumerable<UserDto>>> GetAllUser()
        {
            try
            {
                var response = await _httpClient.GetAsync(string.Empty);
                response.EnsureSuccessStatusCode();

                var responseApi = await response.Content.ReadFromJsonAsync<ResponseApi<IEnumerable<UserDto>>>();

                return responseApi;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseApi<UserDto>> FindUser(int? id = null, string username = null)
        {
            try
            {
                var url = $"find?";
                
                if (id.HasValue)
                    url += $"id={id.Value}&";

                if (!string.IsNullOrEmpty(username))
                    url += $"UserName={username}&";

                var response = await _httpClient.GetAsync(url.Substring(0, url.Length - 1));
                response.EnsureSuccessStatusCode();

                var responseApi = await response.Content.ReadFromJsonAsync<ResponseApi<UserDto>>();

                return responseApi;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
