using AutoMapper;
using FluentValidation;
using ItemWorks.Api.Application.Core.Services.User;
using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Domain.Entities;
using ItemWorks.Api.Domain.Enums;
using ItemWorks.Api.Domain.Repositories;
using ItemWorks.Api.Shared.Uitls;
using MediatR;
using System.Net;

namespace ItemWorks.Api.Application.Core.Application.ItemWorks.Command
{
    public class ItemWorkCreateCmd : IRequest<ResponseApi<ItemWorkDto>>
    {
        public string Description { get; set; }
        public DateTime DeliveryDate { get; set; }
        public RelevanceType Relevance { get; set; }
    }

    public class ItemWorkCreateCmdValidator : AbstractValidator<ItemWorkCreateCmd>
    {
        public ItemWorkCreateCmdValidator()
        {
            RuleFor(x => x.Description).NotEmpty().NotNull();
            RuleFor(x => x.DeliveryDate).NotNull();
            RuleFor(x => x.Relevance).NotNull();
        }
    }

    public class ItemWorkCreateCmdHandler : IRequestHandler<ItemWorkCreateCmd, ResponseApi<ItemWorkDto>>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IItemWorkRepository _itemWorkRepository;

        public ItemWorkCreateCmdHandler(IUserService userService, IMapper mapper, IItemWorkRepository itemWorkRepository)
        {
            _userService = userService;
            _mapper = mapper;
            _itemWorkRepository = itemWorkRepository;
        }

        public async Task<ResponseApi<ItemWorkDto>> Handle(ItemWorkCreateCmd request, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener todos los usuarios disponibles
                var usersResp = await _userService.GetAllUser();

                if (usersResp.Code != HttpStatusCode.OK)
                    return ResponseApi<ItemWorkDto>.Fail(usersResp.Message, usersResp.Code);

                if (!usersResp.Data.Any())
                    return ResponseApi<ItemWorkDto>.Fail("Don't exists users available in system", HttpStatusCode.NotFound);

                // Obtener todos los ítems pendientes
                var items = await _itemWorkRepository.GetAllAsync();

                // Si la tabla de ítems está vacía, asignar al usuario con menos ítems pendientes
                if (!items.Any())
                {
                    var item = _mapper.Map<ItemWork>(request);
                    var userAssing = usersResp.Data.FirstOrDefault();
                    item.UserId = userAssing.Id;

                    await _itemWorkRepository.AddAsync(item);

                    return ResponseApi<ItemWorkDto>.Success(_mapper.Map<ItemWorkDto>(item));
                }

                // Agrupar por usuario los ítems pendientes y contar los de alta relevancia
                var userPendings = items
                    .Where(i => i.Status != ItemStatus.Completed)
                    .GroupBy(i => i.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        QuantityPending = g.Count(),
                        QuantityHighRevelance = g.Count(i => i.Relevance == RelevanceType.High)
                    })
                    .Where(u => u.QuantityHighRevelance <= 3) // Filtra usuarios no saturados
                    .ToDictionary(u => u.UserId, u => u);

                // Añadir usuarios que no tengan ítems asignados con conteo en 0
                var usersCompleted = usersResp.Data.Select(user => new
                {
                    User = user,
                    QuantityPending = userPendings.ContainsKey(user.Id) ? userPendings[user.Id].QuantityPending : 0,
                    QuantityHighRevelance = userPendings.ContainsKey(user.Id) ? userPendings[user.Id].QuantityHighRevelance : 0
                }).OrderBy(u => u.QuantityPending).ToList();

                int? userId = 0;

                // Si la fecha de entrega está próxima a vencer, asignar al usuario con menos pendientes
                if ((request.DeliveryDate - DateTime.Today).TotalDays < 3)
                {
                     userId = usersCompleted.FirstOrDefault()?.User.Id;
                }
                else if (request.Relevance == RelevanceType.High)
                {
                    userId = usersCompleted.FirstOrDefault()?.User.Id;
                }
                else if (request.Relevance == RelevanceType.Low)
                {
                    userId = usersCompleted
                        .OrderBy(u => u.QuantityPending)
                        .FirstOrDefault()?.User.Id;
                }

                // Verificar si hay un usuario disponible para la asignación
                if (userId.HasValue || userId == 0)
                {
                    var itemWork = _mapper.Map<ItemWork>(request);
                    itemWork.UserId = userId.Value;
                    itemWork.Status = ItemStatus.Pending;
                    await _itemWorkRepository.AddAsync(itemWork);
                    return ResponseApi<ItemWorkDto>.Success(_mapper.Map<ItemWorkDto>(itemWork));
                }

                return ResponseApi<ItemWorkDto>.Fail("There are no users available to assign the item.", HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                return ResponseApi<ItemWorkDto>.Fail(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
