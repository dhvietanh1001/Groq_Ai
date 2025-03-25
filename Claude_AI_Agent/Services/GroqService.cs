using AI_Agent.Models;
using Groq_AI_Agent.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Groq_AI_Agent.Services
{
    /// <summary>
    /// Service xử lý hội thoại với Groq API, duy trì lịch sử trò chuyện
    /// </summary>
    public class GroqService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly Dictionary<string, List<Message>> _conversationHistory = new();

        public GroqService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GroqSettings:ApiKey"];
            _httpClient.BaseAddress = new Uri(config["GroqSettings:BaseUrl"]);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        /// Gửi tin nhắn và nhận phản hồi, duy trì lịch sử hội thoại theo sessionId
        /// <param name="sessionId">ID phiên hội thoại</param>
        /// <param name="userMessage">Nội dung tin nhắn người dùng</param>
        /// <param name="model">Tên model (optional)</param>
        /// <returns>Phản hồi từ AI và sessionId hiện tại</returns>
        public async Task<(string response, string sessionId)> ChatAsync(string sessionId, string userMessage, string model = null)
        {
            // Tạo hoặc lấy lịch sử hội thoại hiện có
            if (string.IsNullOrEmpty(sessionId))
                sessionId = Guid.NewGuid().ToString();

            if (!_conversationHistory.ContainsKey(sessionId))
                _conversationHistory[sessionId] = new List<Message>();

            var history = _conversationHistory[sessionId];

            // Thêm tin nhắn người dùng vào lịch sử
            history.Add(new Message { Role = "user", Content = userMessage });

            // Chuẩn bị request
            var requestBody = new GroqRequest
            {
                Messages = history,
                Model = model ?? "llama3-70b-8192"
            };

            // Gọi API
            var response = await _httpClient.PostAsync("chat/completions",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API error: {error}");
            }

            // Xử lý phản hồi
            var responseContent = await response.Content.ReadAsStringAsync();
            var aiResponse = JsonSerializer.Deserialize<GroqResponse>(responseContent)?
                .Choices?.FirstOrDefault()?.Message?.Content
                ?? "Xin lỗi, tôi không thể tạo phản hồi";

            // Thêm phản hồi AI vào lịch sử
            history.Add(new Message { Role = "assistant", Content = aiResponse });

            return (aiResponse, sessionId);
        }

        /// Xóa lịch sử hội thoại của một phiên
        public void ClearHistory(string sessionId)
        {
            if (_conversationHistory.ContainsKey(sessionId))
                _conversationHistory.Remove(sessionId);
        }
    }
}
