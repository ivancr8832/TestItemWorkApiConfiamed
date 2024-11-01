using AutoMapper;
using FluentValidation;
using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Domain.Enums;
using ItemWorks.Api.Domain.Repositories;
using ItemWorks.Api.Shared.Uitls;
using MediatR;
using System.Net;

namespace ItemWorks.Api.Application.Core.Application.ItemWorks.Command
{
    public class ItemWorkUpdateCmd : IRequest<ResponseApi<ItemWorkDto>>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public RelevanceType? Relevance { get; set; }
        public ItemStatus? Status { get; set; }
    }

    public class ItemWorkUpdateCmdValidator : AbstractValidator<ItemWorkUpdateCmd>
    {
        public ItemWorkUpdateCmdValidator()
        {
            RuleFor(x => x.Id).NotNull().GreaterThan(0);
        }
    }

    public class ItemWorkUpdateCmdHandler : IRequestHandler<ItemWorkUpdateCmd, ResponseApi<ItemWorkDto>>
    {
        private readonly IItemWorkRepository _itemWorkRepository;
        private readonly IMapper _mapper;

        public ItemWorkUpdateCmdHandler(IItemWorkRepository itemWorkRepository, IMapper mapper)
        {
            _itemWorkRepository = itemWorkRepository;
            _mapper = mapper;
        }

        public async Task<ResponseApi<ItemWorkDto>> Handle(ItemWorkUpdateCmd request, CancellationToken cancellationToken)
        {
            try
            {
                var itemWork = await _itemWorkRepository.GetByIdAsync(request.Id);

                if (itemWork is null)
                    return ResponseApi<ItemWorkDto>.Fail("Item Work not found", HttpStatusCode.NotFound);

                itemWork.Description = string.IsNullOrEmpty(request.Description) ? itemWork.Description : request.Description;
                itemWork.DateUpdate = DateTime.UtcNow;
                itemWork.Status = request.Status ?? itemWork.Status;
                itemWork.Relevance = request.Relevance ?? itemWork.Relevance;

                await _itemWorkRepository.UpdateAsync(itemWork);

                return ResponseApi<ItemWorkDto>.Success(_mapper.Map<ItemWorkDto>(itemWork));
            }
            catch (Exception ex)
            {
                return ResponseApi<ItemWorkDto>.Fail(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
