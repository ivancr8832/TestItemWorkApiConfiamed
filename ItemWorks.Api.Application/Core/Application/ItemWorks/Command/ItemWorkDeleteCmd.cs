using FluentValidation;
using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Domain.Repositories;
using ItemWorks.Api.Shared.Uitls;
using MediatR;
using System.Net;

namespace ItemWorks.Api.Application.Core.Application.ItemWorks.Command
{
    public class ItemWorkDeleteCmd : IRequest<ResponseApi<string>>
    {
        public int Id { get; set; }
    }

    public class ItemWorkDeleteCmdValidator : AbstractValidator<ItemWorkDeleteCmd>
    {
        public ItemWorkDeleteCmdValidator()
        {
            RuleFor(x => x.Id).NotNull().GreaterThan(0);
        }
    }

    public class ItemWorkDeleteCmdHandler : IRequestHandler<ItemWorkDeleteCmd, ResponseApi<string>>
    {
        private readonly IItemWorkRepository _itemWorkRepository;

        public ItemWorkDeleteCmdHandler(IItemWorkRepository itemWorkRepository)
        {
            _itemWorkRepository = itemWorkRepository;
        }

        public async Task<ResponseApi<string>> Handle(ItemWorkDeleteCmd request, CancellationToken cancellationToken)
        {
            try
            {
                var itemWork = await _itemWorkRepository.GetByIdAsync(request.Id);

                if (itemWork is null)
                    return ResponseApi<string>.Fail("Item Work not found", HttpStatusCode.NotFound);

                await _itemWorkRepository.DeleteAsync(itemWork);

                return ResponseApi<string>.Success("Item Work deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseApi<string>.Fail(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
