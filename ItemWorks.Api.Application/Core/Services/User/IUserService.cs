using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Shared.Uitls;

namespace ItemWorks.Api.Application.Core.Services.User
{
    public interface IUserService
    {
        Task<ResponseApi<UserDto>> FindUser(int? id = null, string username = null);
        Task<ResponseApi<IEnumerable<UserDto>>> GetAllUser();
    }
}
