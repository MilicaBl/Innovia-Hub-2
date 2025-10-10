public interface IAIClient
{
    Task<string> SendChatAsync(string prompt);
}