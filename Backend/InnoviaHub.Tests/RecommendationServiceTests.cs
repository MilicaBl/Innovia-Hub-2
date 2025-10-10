using System.Net;
using System.Text.Json;
using Backend.DTOs.Booking;
using Backend.DTOs.Recommendation;
using Backend.Interfaces.IRepositories;
using Moq;
using Xunit;

namespace InnoviaHub.Tests;

public class RecommendationServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepo;
    private readonly Mock<IHttpClientFactory> _mockFactory;
    public RecommendationServiceTests()
    {
        _mockRepo = new Mock<IBookingRepository>();
    }
    [Fact]
    public async Task GetRecommendationAsync_ReturnsRecommendation()
    {
        //Arange
        //Mocka repository: tom historik + en ledig slot
        var fakeBookings = new List<UserBookingDTO>();

        //.Setup(...) betyder: "när den här metoden anropas under testet — gör så här istället.
        //It.IsAny<string>() betyder: Jag bryr mig inte vilket värde som skickas in (vilket userId)
        _mockRepo.Setup(r => r.GetBookingsByUser(It.IsAny<string>())).ReturnsAsync(fakeBookings);

        var fakeAvailable = new List<AvailableSlotDto>
        {
                    new AvailableSlotDto {
                    ResourceTypeId = 1,
                    ResourceName = "Mötesrum 1",
                    ResourceType = "Mötesrum",
                    Date = DateTime.Today.ToString("yyyy-MM-dd"),
                    AvailableSlots = new List<string>{ "10-12", "12-14" }
                }

        };
        _mockRepo.Setup(r => r.GetAvailableSlotsAsync(It.IsAny<DateTime>())).ReturnsAsync(fakeAvailable);

        // Bygg det "inre" JSON-objektet som AI ska returnera
        var expectedRecommendation = new RecommendationResponseDto
        {
            ResourceType = "Mötesrum",
            ResourceName = "Mötesrum 1",
            ResourceTypeId = 1,
            Date = "2025-10-10",
            TimeSlot = "10-12"
        };

        // OpenAI svarar ofta som: { "choices": [ { "message": { "content": "<json-string>" } } ] }
        var innerJson = JsonSerializer.Serialize(expectedRecommendation);
        var outer = new { choices = new[] { new { message = new { content = innerJson } } } };
        var outerJson = JsonSerializer.Serialize(outer);

        //Mock HttpClient via FakeHttpMessageHandler
        var handler = new FakeHttpMessageHandler(outerJson, HttpStatusCode.OK);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };

        var mockAiClient = new Mock<IAIClient>();

        // Mocka SendChatAsync att returnera "outerJson" som OpenAI skulle göra
        mockAiClient.Setup(c => c.SendChatAsync(It.IsAny<string>())).ReturnsAsync(outerJson);


        var service = new RecommendationService(mockAiClient.Object, _mockRepo.Object);
        //Act
        var result = await service.GetRecommendationAsync("user-123");
        //Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRecommendation.ResourceType, result.ResourceType);
        Assert.Equal(expectedRecommendation.ResourceName, result.ResourceName);
        Assert.Equal(expectedRecommendation.ResourceTypeId, result.ResourceTypeId);
        Assert.Equal(expectedRecommendation.Date, result.Date);
        Assert.Equal(expectedRecommendation.TimeSlot, result.TimeSlot);
    }
}