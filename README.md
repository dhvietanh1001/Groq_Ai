# Groq AI Agent API

## Introduction
This project provides an API server integrated with Groq API to provide AI chat capabilities using popular artificial intelligence models such as ChatGPT, Gemini Meta, etc. The server handles chat requests, manages chat sessions, and generates intelligent responses.
Groq is an AI service that provides an API(**Free**) that allows calling models such as: Llama 3.1, GPT-4o, Claude 3.5 Sonnet, Gemini 1.5 Pro
## Project Structure
```
groq-ai-agent/
├── Controllers/
│   ├── GroqController.cs        # API endpoints
│   ├── HealthController.cs      # Check server health
├── Models/
│   ├── GroqRequest.cs          # Groq API request model
│   ├── GroqResponse.cs         # Groq API response model
│   ├── Message.cs              # Message model
│   ├── ChatRequest.cs          # Client request model
│   ├── ChatResponse.cs         # Client response model
│   ├── HealthResponse.cs       # Server health model
├── Services/
│   ├── GroqService.cs          # Groq API service
├── appsettings.json             # Configuration file
├── Program.cs                   # Main entry poinhere](https://console.groq.com/docs/openai)
```

## Installation

### 1. Clone the repository:
```bash
git clone https://github.com/dhvietanh1001/Groq_Ai.git
cd groq-ai-agent
```

### 2. Configure `appsettings.json`:
Quan trọng nhất là cập nhập apikey
```json
{
  "GroqSettings": {
    "ApiKey": "your_groq_api_key",
    "BaseUrl": "https://api.groq.com/openai/v1/",
    "DefaultModel": "gemma2-9b-it"
  }
}
```
You can sign up and get APIkey at: [here](https://console.groq.com/docs/openai)

### 3. Run the application:
```bash
dotnet run
```

## API Endpoints

### **POST** `/api/chat`
**Send chat message and get AI response**

#### Request:
```json
{
  "sessionId": "optional_session_id",
  "message": "your_message",
  "model": "optional_model_name"
}
```

#### Response:
```json
{
  "response": "AI_response",
  "sessionId": "session_id"
}
```

### **GET** `/api/health`
**Check system health**

#### Response:
```json
{
  "status": "Healthy/Unhealthy",
  "components": {
    "api": "status",
    "groqConnection": "status",
    "memoryUsage": "value"
  }
}
```

### **DELETE** `/api/chat/{sessionId}`
**Clear conversation session**

## Usage Examples

### **cURL**
```bash
# Start new chat
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message":"Hello, who are you?"}'

# Continue chat
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{"sessionId":"abc123","message":"Tell me more"}'
```

### **C#**
```csharp
var client = new HttpClient();
var response = await client.PostAsJsonAsync("http://localhost:5000/api/chat", 
    new { message = "Explain quantum computing" });
var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
Console.WriteLine(result.Response);
```

## Features
✅ Multi-session conversation management  
✅ Supports Llama 3 and Mixtral models  
✅ In-memory conversation history  
✅ Comprehensive health checks  
✅ Detailed error handling  

## GROQ Supported Models

| MODEL ID                   | DEVELOPER   | CONTEXT WINDOW (TOKENS) | MAX COMPLETION TOKENS | MAX FILE SIZE |
|----------------------------|-------------|-------------------------|-----------------------|---------------|
| distil-whisper-large-v3-en | HuggingFace | -                       | -                     | 25 MB         | 
| gemma2-9b-it               | Google      | 8,192                   | -                     | -             | 
| llama-3.3-70b-versatile    | Meta        | 128K                    | 32,768                | -             | 
| llama-3.1-8b-instant       | Meta        | 128K                    | 8,192                 | -             | 
| llama-guard-3-8b           | Meta        | 8,192                   | -                     | -             | 
| llama3-70b-8192            | Meta        | 8,192                   | -                     | -             | 
| llama3-8b-8192             | Meta        | 8,192                   | -                     | -             | 
| mixtral-8x7b-32768         | Mistral     | 32,768                  | -                     | -             | 
| whisper-large-v3           | OpenAI      | -                       | -                     | 25 MB         | 
| whisper-large-v3-turbo     | OpenAI      | -                       | -                     | 25 MB         |

## Notes
⚠️ **Keep your API key secure**  
🔹 **Default model:** `gemma2-9b-it`  
💾 **Conversations are stored in memory (not persistent)**
