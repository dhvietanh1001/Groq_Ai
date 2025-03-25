using Groq_AI_Agent.Models;
using Groq_AI_Agent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Groq_AI_Agent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GroqController : ControllerBase
    {
        private readonly GroqService _chatService;
        private readonly ILogger<GroqController> _logger;

        public GroqController(GroqService chatService, ILogger<GroqController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// Gửi tin nhắn và nhận phản hồi từ AI (tạo hoặc tiếp tục phiên hội thoại)
        /// <param name="request">
        /// {
        ///   "sessionId": "string", // (optional) ID phiên hiện tại
        ///   "message": "string",   // Nội dung tin nhắn
        ///   "model": "string"     // (optional) Tên model (mặc định: llama3-70b-8192)
        /// }
        /// </param>
        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                _logger.LogInformation($"New chat request - Session: {request.SessionId}");

                // Gọi service và nhận kết quả
                var (response, sessionId) = await _chatService.ChatAsync(
                    request.SessionId,
                    request.Message,
                    request.Model
                );

                return Ok(new ChatResponse
                {
                    Response = response,
                    SessionId = sessionId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chat error");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// Xóa lịch sử hội thoại của một phiên
        [HttpDelete("{sessionId}")]
        public IActionResult ClearSession(string sessionId)
        {
            _chatService.ClearHistory(sessionId);
            _logger.LogInformation($"Cleared session: {sessionId}");
            return Ok(new { message = $"Session {sessionId} cleared" });
        }
    }
}
