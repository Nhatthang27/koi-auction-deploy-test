using AuctionService.Models;
using AuctionService.Dto;
using AuctionService.Dto.AuctionMethod;

namespace AuctionService.Mapper
{
    public static class AuctionMethodMapper
    {
        public static AuctionMethodDto ToAuctionMethodDtoFromAuctionMethod(this AuctionMethod auctionMethod)
        {
            return new AuctionMethodDto
            {
                AuctionMethodId = auctionMethod.AuctionMethodId,
                AuctionMethodName = auctionMethod.AuctionMethodName,
                Description = auctionMethod.Description
            };
        }

        public static AuctionMethod ToActionMethodFromCreateAuctionMethodDto(this CreateAuctionMethodDto auctionMethod)
        {
            return new AuctionMethod
            {
                AuctionMethodName = auctionMethod.AuctionMethodName!,
                Description = auctionMethod.Description
            };
        }

    }
}