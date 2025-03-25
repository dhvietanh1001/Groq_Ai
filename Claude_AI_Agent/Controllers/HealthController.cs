using AI_Agent.Models;
using Groq_AI_Agent.Models;
using Groq_AI_Agent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Groq_AI_Agent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly GroqService _groqService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(GroqService groqService, ILogger<HealthController> logger)
        {
            _groqService = groqService;
            _logger = logger;
        }

        /// Kiểm tra trạng thái tổng thể hệ thống
        /// Kết quả
        ///   "status": "Healthy/Unhealthy",
        ///   "components": {
        ///     "api": "Healthy",
        ///     "groqConnection": "Healthy/Unhealthy",
        ///     "memoryUsage": "123MB"



        [HttpGet]
        public async Task<IActionResult> CheckHealth()
        {
            var healthReport = new HealthReport
            {
                Status = "Healthy",
                Components = new Dictionary<string, object>()
            };

            try
            {
                // 1. Kiểm tra bộ nhớ
                var memoryUsage = GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024;
                healthReport.Components.Add("memoryUsage", $"{memoryUsage}MB");

                // 2. Kiểm tra kết nối Groq API
                var groqHealth = await CheckGroqConnectionAsync();
                healthReport.Components.Add("groqConnection", groqHealth ? "Healthy" : "Unhealthy");

                if (!groqHealth)
                {
                    healthReport.Status = "Unhealthy";
                    _logger.LogWarning("Groq API connection failed");
                }

                return Ok(healthReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                healthReport.Status = "Unhealthy";
                return StatusCode(500, healthReport);
            }
        }

        private async Task<bool> CheckGroqConnectionAsync()
        {
            try
            {
                const string HealthCheckMessage = "health_check";
                var healthCheckRequest = new GroqRequest
                {
                    Messages = new List<Message>
                    {
                        new()
                        {
                            Role = "user",
                            Content = HealthCheckMessage
                        }
                    },
                    MaxTokens = 10, // Đủ dài để nhận phản hồi nhưng không tốn token
                    Temperature = 0.1 // Giảm tính ngẫu nhiên cho response ổn định
                };

                try
                {
                    // Sử dụng sessionId đặc biệt cho health check
                    var (response, _) = await _groqService.ChatAsync(
                        sessionId: "health_check_session",
                        userMessage: HealthCheckMessage,
                        model: "gemma2-9b-it"); 

                    // Validate phản hồi
                    var isHealthy = !string.IsNullOrEmpty(response) &&
                                   !response.Contains("error", StringComparison.OrdinalIgnoreCase);

                    if (!isHealthy)
                    {
                        throw new Exception($"Invalid API response: {response}");
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log chi tiết
                    _logger.LogError(ex, "Groq API health check failed");
                    throw;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    
}
