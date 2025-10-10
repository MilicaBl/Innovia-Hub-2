
using System.Text;
using System.Text.Json;

public class OpenAiClient : IAIClient
{
    private readonly HttpClient _httpClient;
    public OpenAiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
   
    public async Task<string> SendChatAsync(string prompt)
    {
        //Body som skickas till ai
        var requestBody = new { model = "gpt-4.1", messages = new[] { new { role = "user", content = prompt } } };
        //searilisera body
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        //Skicka till ai server
        var response = await _httpClient.PostAsync("chat/completions", content);
        //Svaret 
        var raw = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"OpenAI API failed: {response.StatusCode}");

        return raw;
    }
}