using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;
using AuctionService.Dto.KoiFish;

namespace AuctionService.IRepository
{
    public interface IKoiFishRepository
    {
        Task<KoiFish> CreateKoiAsync(KoiFish koiFish);
        Task<KoiFish> UpdateKoiAsync(int id, UpdateKoiFishDto updateKoiDto);
    }
}