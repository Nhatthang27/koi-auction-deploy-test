using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AuctionService.Dto.User;

namespace AuctionService.Services
{
    public class UserSevice
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<int, UserDto> _userCache;


        public UserSevice(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _userCache = new();
        }

        // Lấy thông tin user theo ID và lưu vào cache nếu chưa có
        public async Task<UserDto?> GetuserByIdAsync(int userId)
        {
            if (_userCache.TryGetValue(userId, out var user))
            {
                return user;
            }

            // Nếu chưa có trong cache, gọi UserService để lấy thông tin
            var response = await _httpClient.GetAsync($"http://localhost:3000/user-service/manage/profile/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                user = JsonSerializer.Deserialize<UserDto>(content);
                _userCache[userId] = user!; // Lưu user vào cache tạm thời
            }

            return user;
        }

        public async Task<List<UserDto>> GetAllUserAsync()
        {
            var users = new List<UserDto>();
            var response = await _httpClient.GetAsync($"http://localhost:3000/user-service/manage/profile");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                users = JsonSerializer.Deserialize<List<UserDto>>(content); // Deserialize as a list of userDetailDto

            }
            if (users == null)
                throw new Exception("No users are existed");
            return users;
        }
    }
}