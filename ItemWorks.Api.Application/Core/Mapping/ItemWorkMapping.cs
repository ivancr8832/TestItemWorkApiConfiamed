using AutoMapper;
using ItemWorks.Api.Application.Core.Application.ItemWorks.Command;
using ItemWorks.Api.Contracts.DTOs;
using ItemWorks.Api.Domain.Entities;

namespace ItemWorks.Api.Application.Core.Mapping
{
    public class ItemWorkMapping : Profile
    {
        public ItemWorkMapping()
        {
            CreateMap<ItemWork, ItemWorkDto>();
            CreateMap<ItemWorkCreateCmd, ItemWork>();
        }
    }
}
