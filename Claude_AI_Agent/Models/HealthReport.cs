namespace Groq_AI_Agent.Models
{
    public class HealthReport
    {
        public string Status { get; set; }
        public Dictionary<string, object> Components { get; set; }
    }
}
