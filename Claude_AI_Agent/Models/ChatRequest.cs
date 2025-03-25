using System.ComponentModel.DataAnnotations;

namespace Groq_AI_Agent.Models
{
    public class ChatRequest
    {
        public string SessionId { get; set; }

        [Required(ErrorMessage = "Message content is required")]
        public string Message { get; set; }

        public string Model { get; set; }
    }

}
