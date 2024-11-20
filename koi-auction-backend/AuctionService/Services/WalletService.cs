using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AuctionService.Dto.AuctionDeposit;
using AuctionService.Dto.Wallet;
using Microsoft.AspNetCore.Authentication.BearerToken;

namespace AuctionService.Services
{
    public class WalletService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public WalletService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Phương thức lấy token sử dụng username và password từ appsettings
        private async Task<string?> LoginAsync()
        {
            var username = _configuration["BiddingService:Username"];
            var password = _configuration["BiddingService:Password"];
            var tokenUrl = _configuration["BiddingService:TokenUrl"];

            var credentials = new { Username = username, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(tokenUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseContent); // accesstoken
                System.Console.WriteLine("login success");
                return tokenResponse?.AccessToken; // Giả sử TokenResponse có thuộc tính Token
            }
            return null;
        }

        public async Task<WalletDto?> GetBalanceByIdAsync(int id)
        {
            WalletDto? wallet = null;

            var token = _configuration["AuctionService:ServiceToken"];

            // Thêm token vào header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi PaymentService để lấy thông tin ví
            var response = await _httpClient.GetAsync($"http://localhost:3004/api/internal/get-wallet-balance/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                wallet = JsonSerializer.Deserialize<WalletDto>(content);

            }
            return wallet;
        }

        public async Task<string> PaymentAsync(PaymentDto paymentDto)
        {
            var token = _configuration["AuctionService:ServiceToken"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //use httpClient to payment
            System.Console.WriteLine($"{paymentDto.UserId} payment {paymentDto.Amount} for {paymentDto.Description}");
            var response = await _httpClient.PostAsJsonAsync("http://localhost:3004/api/internal/payment", paymentDto);

            //print response info
            System.Console.WriteLine($"Payment response: {response.ReasonPhrase}");
            if (response.IsSuccessStatusCode)
            {
                return "Payment success";
            }
            else
            {
                // var result = await response.Content.ReadAsStringAsync();
                // Console.WriteLine(result);
                throw new Exception("Payment failed! Not enough balance");
            }
        }

        public async Task<string> RefundAsync(List<RefundDto> refundListDto)
        {
            var token = _configuration["AuctionService:ServiceToken"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //use httpClient to refund
            var response = await _httpClient.PostAsJsonAsync("http://localhost:3004/api/internal/refund-many", refundListDto);

            //print response info
            System.Console.WriteLine($"Refund response: {response.ReasonPhrase}");
            if (response.IsSuccessStatusCode)
            {
                return "Refund success";
            }
            else
            {
                // var result = await response.Content.ReadAsStringAsync();
                // Console.WriteLine(result);
                throw new Exception("Refund failed!");
            }
        }
    }
}
