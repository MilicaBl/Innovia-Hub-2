namespace Backend.DTOs.Recommendation
{
    public class RecommendationRequestDto
    {
        public required string UserId { get; set; }
    }
    public class RecommendationResponseDto
    {
        public string? ResourceType { get; set; }
        public string? ResourceName { get; set; }
        public int ResourceTypeId{ get; set; }
        public string? Date { get; set; }
        public string? TimeSlot { get; set; }

    }
    public class AvailableSlotDto
    {
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public List<string> AvailableSlots { get; set; } = new();
        public int ResourceTypeId{ get; set; }
    }
}