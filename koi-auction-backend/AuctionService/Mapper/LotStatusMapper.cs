using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;
using AuctionService.Dto.LotStatus;

namespace AuctionService.Mapper
{
    public static class LotStatusMapper
    {
        public static LotStatusDto ToLotStatusDtoFromLotStatus(this LotStatus lotStatus)
        {
            if (lotStatus == null)
            {
                return null!;
            }
            return new LotStatusDto
            {
                LotStatusId = lotStatus.LotStatusId,
                LotStatusName = lotStatus.LotStatusName
            };
        }

        public static LotStatus ToLotStatusFromCreateLotStatusDto(this CreateLotStatusDto lotStatusDto)
        {
            return new LotStatus
            {
                LotStatusName = lotStatusDto.LotStatusName!
            };
        }
    }
}