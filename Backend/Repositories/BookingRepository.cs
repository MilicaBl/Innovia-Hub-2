using System;
using Backend.DTOs.Booking;
using Backend.DTOs.Recommendation;
using Backend.Interfaces.IRepositories;
using Backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;
    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Booking> AddBookingAsync(DTOCreateBooking booking)
    {
        var resourcesList = await GetAvailableResourcesAsync(booking.ResourceTypeId, booking.Date, booking.TimeSlot);
        var resource = GetResourceByIdAsync(resourcesList);
        if (!resourcesList.Any())
        {
            throw new InvalidOperationException("No available resources for the selected type, date, and timeslot.");
        }


        var newBooking = new Booking
        {
            Date = booking.Date,
            TimeSlot = booking.TimeSlot,
            ResourceTypeId = booking.ResourceTypeId,
            UserId = booking.UserId,
            ResourceId = resource.Id,
        };

        _context.Bookings.Add(newBooking);
        await _context.SaveChangesAsync();
        return newBooking;
    }
    public async Task<List<Resource>> GetAvailableResourcesAsync(
                int resourceTypeId, DateTime date, string timeSlot)
    {
        var availableResources = await _context.Resources
    .Where(r => r.IsBookable
             && r.ResourceTypeId == resourceTypeId
             && !r.Bookings.Any(b =>
                    b.Date == date &&
                    b.TimeSlot == timeSlot))
    .ToListAsync();

        return availableResources;
    }

    public async Task<IEnumerable<Booking>> GetBookingByDate(DateTime date)
    {
        var bookings = await _context.Bookings
        .Include(b => b.Resource)
        .Include(b => b.ResourceType)
        .Include(b => b.User)
        .Where(b => b.Date == date)
        .ToListAsync();
        if (bookings is null)
        {
            return [];
        }

        return bookings;

    }

    public async Task<List<UserBookingDTO>> GetBookingsByUser(string userId)
    {
         var userBookings = await _context.Bookings
        .Include(b => b.Resource)
        .Where(b => b.UserId == userId)
        .Select(b => new UserBookingDTO
        {
            bookingId = b.Id,
            date = b.Date.ToString("yyyy-MM-dd"),
            timeSlot = b.TimeSlot,
            resourceName = b.Resource.ResourceName

        }).ToListAsync();

        return userBookings;
    }
    public Resource? GetResourceByIdAsync(List<Resource> resources)
    {
        return resources.FirstOrDefault();

    }

    public async Task<bool> DeleteBooking(int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(DateTime fromDate)
    {

        // Definera alla möjliga tidsslott
        var allSlots = new List<string> { "08-10", "10-12", "12-14", "14-16", "16-18", "18-20" };

         //Aktuell tid 
        var now = DateTime.Now;

    var resources = await _context.Resources
        .Include(r => r.Bookings)
        .Include(r => r.ResourceType)
        .ToListAsync(); // Hämta allt först, EF slutar försöka översätta AvailableSlots till SQL

    var availableSlots = resources.Select(r => new AvailableSlotDto
    {
        ResourceType = r.ResourceType?.ResourceTypeName ?? "Unknown",
        ResourceName = r.ResourceName,
        Date=fromDate.ToString(),
        ResourceTypeId=r.ResourceTypeId,
        AvailableSlots = allSlots
            .Where(slot => !r.Bookings.Any(b => b.Date == fromDate && b.TimeSlot == slot))
            .Where(slot =>
                {
                    //Om datum inte är dagens datum returnera true 
                    if (fromDate.Date != now.Date)
                        return true;
                        
                    //Om det är dagens datum kontorllera returnera endast om starttiden är större än sluttiden
                    var startHour = int.Parse(slot.Split('-')[0]);
                    return startHour > now.Hour;
                })
            .ToList()
    }).ToList();

    return availableSlots;
    }
}
