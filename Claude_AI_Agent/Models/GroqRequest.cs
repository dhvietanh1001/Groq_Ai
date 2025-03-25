using Groq_AI_Agent.Models;

namespace AI_Agent.Models
{
    using System.Text.Json.Serialization;

    public class GroqRequest
    {
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new();

        [JsonPropertyName("model")]
        public string Model { get; set; } = "llama3-70b-8192";

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 1024;
    }
}
