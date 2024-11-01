using AutoMapper;
using ItemWorks.Api.Application.Core.Services.User;
using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Domain.Repositories;
using ItemWorks.Api.Shared.Uitls;
using MediatR;
using System.Net;

namespace ItemWorks.Api.Application.Core.Application.ItemWorks.Queries
{
    public class ItemWorkFindQry : IRequest<ResponseApi<Dictionary<string, List<ItemWorkDto>>>>
    {
        public string UserName { get; set; }
    }

    public class ItemWorkFindQryHandler : IRequestHandler<ItemWorkFindQry, ResponseApi<Dictionary<string, List<ItemWorkDto>>>>
    {
        private readonly IUserService _userService;
        private readonly IItemWorkRepository _itemWorkRepository;
        private readonly IMapper _mapper;

        public ItemWorkFindQryHandler(IUserService userService, IItemWorkRepository itemWorkRepository, IMapper mapper)
        {
            _userService = userService;
            _itemWorkRepository = itemWorkRepository;
            _mapper = mapper;
        }

        public async Task<ResponseApi<Dictionary<string, List<ItemWorkDto>>>> Handle(ItemWorkFindQry request, CancellationToken cancellationToken)
        {
            try
            {
                var userResp = await _userService.FindUser(username: request.UserName);

                if (userResp.Code != HttpStatusCode.OK)
                    return ResponseApi<Dictionary<string, List<ItemWorkDto>>>.Fail(userResp.Message, userResp.Code);

                var itemsPending = await _itemWorkRepository.GetPendingByUser(userResp.Data.Id);
                var itemsPendingDto = _mapper.Map<IEnumerable<ItemWorkDto>>(itemsPending);
                
                var result = new Dictionary<string, List<ItemWorkDto>>
                {
                    { userResp.Data.UserName, itemsPendingDto.ToList() }
                };

                return ResponseApi<Dictionary<string, List<ItemWorkDto>>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResponseApi<Dictionary<string, List<ItemWorkDto>>>.Fail(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
