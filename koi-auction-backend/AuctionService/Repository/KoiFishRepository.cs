using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.IRepository;
using AuctionService.Models;
using AuctionService.Dto.KoiFish;
using Microsoft.EntityFrameworkCore;
using AuctionService.Data;

namespace AuctionService.Repository
{
    public class KoiFishRepository : IKoiFishRepository
    {
        private readonly AuctionManagementDbContext _context;
        public KoiFishRepository(AuctionManagementDbContext context)
        {
            _context = context;
        }
        public async Task<KoiFish> CreateKoiAsync(KoiFish koiFish)
        {
            await _context.KoiFishes.AddAsync(koiFish);

            return koiFish;
        }

        public async Task<KoiFish> UpdateKoiAsync(int id, UpdateKoiFishDto updateKoiDto)
        {
            var koiFish = await _context.KoiFishes.FirstOrDefaultAsync(f => f.KoiFishId == id);
            if (koiFish == null)
            {
                throw new KeyNotFoundException($"No koifish found with ID: {id}");
            }
            koiFish.Variety = updateKoiDto.Variety;
            koiFish.Sex = updateKoiDto.Sex;
            koiFish.SizeCm = updateKoiDto.SizeCm;
            koiFish.YearOfBirth = updateKoiDto.YearOfBirth;
            koiFish.WeightKg = updateKoiDto.WeightKg;
            return koiFish;
        }
    }
}