using System.Text.Json.Serialization;

namespace Groq_AI_Agent.Models
{
    public class GroqResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        public class Choice
        {
            [JsonPropertyName("message")]
            public Message Message { get; set; }
        }
    }
}
