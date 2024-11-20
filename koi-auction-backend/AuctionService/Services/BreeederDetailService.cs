using System.Text.Json;
using AuctionService.Dto.BreederDetail;

namespace AuctionService.Services
{
    public class BreederDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<int, BreederDetailDto> _breederCache;

        private string? _userBaseUrl;

        private readonly IConfiguration _configuration;

        public BreederDetailService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _breederCache = new();
            _userBaseUrl = _configuration["UserService:BaseUrl"];
        }

        // Lấy thông tin breeder theo ID và lưu vào cache nếu chưa có
        public async Task<BreederDetailDto?> GetBreederByIdAsync(int breederId)
        {
            if (_breederCache.TryGetValue(breederId, out var breeder))
            {
                return breeder;
            }

            // Nếu chưa có trong cache, gọi UserService để lấy thông tin
            // var response = await _httpClient.GetAsync($"http://localhost:3000/user-service/manage/breeder/profile/{breederId}");
            var response = await _httpClient.GetAsync($"{_userBaseUrl}/manage/breeder/profile/{breederId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                breeder = JsonSerializer.Deserialize<BreederDetailDto>(content);
                _breederCache[breederId] = breeder!; // Lưu breeder vào cache tạm thời
            }

            return breeder;
        }

        public async Task<List<BreederDetailDto>> GetAllBreederAsync()
        {
            var breeders = new List<BreederDetailDto>();
            var response = await _httpClient.GetAsync($"{_userBaseUrl}/manage/breeder/profile");
            // var response = await _httpClient.GetAsync($"http://localhost:3000/user-service/manage/breeder/profile");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                breeders = JsonSerializer.Deserialize<List<BreederDetailDto>>(content); // Deserialize as a list of BreederDetailDto

            }
            if (breeders == null)
                throw new Exception("No Breeders are existed");
            return breeders;
        }
    }
}


