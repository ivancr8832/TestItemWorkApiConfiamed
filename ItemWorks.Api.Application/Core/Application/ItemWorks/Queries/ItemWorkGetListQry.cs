using AutoMapper;
using ItemWorks.Api.Application.Core.Services.User;
using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Domain.Enums;
using ItemWorks.Api.Domain.Repositories;
using ItemWorks.Api.Shared.Uitls;
using MediatR;
using System.Net;

namespace ItemWorks.Api.Application.Core.Application.ItemWorks.Queries
{
    public class ItemWorkGetListQry : IRequest<ResponseApi<Dictionary<string, List<ItemWorkDto>>>>
    {
        public string UserName { get; set; }
    }

    public class ItemWorGetListQryHandler : IRequestHandler<ItemWorkGetListQry, ResponseApi<Dictionary<string, List<ItemWorkDto>>>>
    {
        private readonly IItemWorkRepository _itemWorkRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public ItemWorGetListQryHandler(IItemWorkRepository itemWorkRepository, IMapper mapper, IUserService userService)
        {
            _itemWorkRepository = itemWorkRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ResponseApi<Dictionary<string, List<ItemWorkDto>>>> Handle(ItemWorkGetListQry request, CancellationToken cancellationToken)
        {

            try
            {
                var users = await _userService.GetAllUser();

                if (users.Code != HttpStatusCode.OK)
                    return ResponseApi<Dictionary<string, List<ItemWorkDto>>>.Fail(users.Message, users.Code);

                var items = await _itemWorkRepository.GetAllAsync();

                var itemsPending = items.Where(x => x.Status != ItemStatus.Completed);

                var result = users.Data.ToDictionary(
                    user => user.UserName,
                    user => itemsPending
                                .Where(x => x.UserId == user.Id)
                                .Select(x => new ItemWorkDto
                                {
                                    Id = x.Id,
                                    Description = x.Description,
                                    DeliveryDate = x.DeliveryDate,
                                    Status = x.Status,
                                    Relevance = x.Relevance
                                }).ToList()
                );

                result = result.Where(kvp => kvp.Value.Any())
                               .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                return ResponseApi<Dictionary<string, List<ItemWorkDto>>>.Success(result);
            }
            catch (Exception ex)
            {
                return ResponseApi<Dictionary<string, List<ItemWorkDto>>>.Fail(ex.Message, HttpStatusCode.InternalServerError);
            }

        }
    }
}
