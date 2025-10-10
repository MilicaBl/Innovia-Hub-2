using System.Text;
using System.Text.Json;
using Backend.DTOs.Recommendation;
using Backend.Interfaces.IRepositories;
using Backend.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

public class RecommendationService
{
    private readonly IAIClient _aiClient;
    private readonly IBookingRepository _bookingRepoitory;
    public RecommendationService(IAIClient aIClient, IBookingRepository bookingRepository)
    {
        _aiClient = aIClient;
        _bookingRepoitory = bookingRepository;
    }
    public async Task<RecommendationResponseDto> GetRecommendationAsync(string userId)
    {
        var bookingHistory = await _bookingRepoitory.GetBookingsByUser(userId);

        //Gör en text av bokningshistorik
        var historySummary = bookingHistory.Count > 0
        ? string.Join("\n ", bookingHistory.Select(b => $"Resurs: {b.resourceName}, Datum: {b.date} Tidsslott {b.timeSlot}"))
        : "Inga tidigare bokningar";


        //Tillgängliga slotts och resurser
        var availableResources = new List<AvailableSlotDto>();
        for (int i = 0; i < 7; i++)
        {
            var date = DateTime.Today.AddDays(i);
            var slots = await _bookingRepoitory.GetAvailableSlotsAsync(date);
            availableResources.AddRange(slots);
        }
        string availableSummary = string.Join("\n", availableResources.Select(r => $"ResourceType: {r.ResourceType}, ResourceTypeId:{r.ResourceTypeId} ResourceName: {r.ResourceName}, Date: {r.Date},  Available timeslots: {string.Join(", ", r.AvailableSlots)} "));

        var prompt = PromptBuilder.Build(historySummary, availableSummary);

        //Svaret 
        var raw = await _aiClient.SendChatAsync(prompt);

        // Tolka svaret från OpenAI
        using var doc = JsonDocument.Parse(raw);
        var message = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    
        //  Rensa bort markdown-backticks om de finns
        var cleanJson = message
            ?.Replace("```json", "")
            .Replace("```", "")
            .Trim();

        //Deserialisera
        RecommendationResponseDto? parsed;
        try
        {
             parsed = JsonSerializer.Deserialize<RecommendationResponseDto>(cleanJson);
        }
        catch (Exception ex)
        {
            throw new Exception("Kunde inte tolka AI svaret. Kontrollera formatet");
        }

        if (parsed == null)
        {
            throw new Exception("AI returnerade inget gitligt förslag");
        }
        return parsed;

    }
}