using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Dto.Mail;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;



namespace AuctionService.Services
{
    public class MailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public MailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Phương thức lấy token sử dụng username và password từ appsettings
        // private async Task<string?> LoginAsync()
        // {
        //     var username = _configuration["BiddingService:Username"];
        //     var password = _configuration["BiddingService:Password"];
        //     var tokenUrl = _configuration["BiddingService:TokenUrl"];

        //     var credentials = new { Username = username, Password = password };
        //     var content = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");

        //     var response = await _httpClient.PostAsync(tokenUrl, content);
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var responseContent = await response.Content.ReadAsStringAsync();
        //         var tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseContent); // accesstoken
        //         System.Console.WriteLine("login success");
        //         return tokenResponse?.AccessToken; // Giả sử TokenResponse có thuộc tính Token
        //     }
        //     return null;
        // }

        public async Task<MailDto> SendMailAsync(MailDto mailDto)
        {
            MailDto? mail = null;

            var token = _configuration["AuctionService:ServiceToken"];

            // Thêm token vào header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi MailService để gửi email
            var response = await _httpClient.PostAsJsonAsync($"http://localhost:3005/api/send-email", mailDto);
            return mail!;
        }
    }
}