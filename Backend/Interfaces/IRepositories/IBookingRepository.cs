using System;
using Backend.DTOs.Booking;
using Backend.DTOs.Recommendation;
using Backend.Models;

namespace Backend.Interfaces.IRepositories;

public interface IBookingRepository
{
    Task<Booking> AddBookingAsync(DTOCreateBooking booking);
    Task<IEnumerable<Booking>> GetBookingByDate(DateTime date);
    Task<List<UserBookingDTO>> GetBookingsByUser(string userId);
    Task<List<Resource>> GetAvailableResourcesAsync(int resourceId, DateTime date, string timeSlot);
    Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(DateTime fromDate);
    Task<bool> DeleteBooking(int bookingId);
}
