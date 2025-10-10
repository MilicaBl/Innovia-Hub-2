using System.Security.Cryptography;
using Backend.DTOs.Booking;
using Xunit;

namespace InnoviaHub.Tests;
public class BookingTests
{
    [Fact]//Att bokningen lyckas när användare resurs och datum är giltiga
    public void CreateBookig_WithValidData_ShouldSuceed()
    {
        //Arrange
        var bookingData = new DTOCreateBooking
        {
            Date = DateTime.Today.AddDays(1),
            TimeSlot = "10-12",
            ResourceTypeId = 1,
            UserId = "testid"
        };
        var startHour = int.Parse(bookingData.TimeSlot.Split('-')[0]);
        var isDateValid = bookingData.Date >= DateTime.Today;
        var isTimeSlotValid = startHour > DateTime.Today.Hour;
        //Act
        var isUserIdValid = !string.IsNullOrWhiteSpace(bookingData.UserId);
        var isResourceIdValid = bookingData.ResourceTypeId > 0 && bookingData.ResourceTypeId < 6;
        var isValid = isUserIdValid && isResourceIdValid && isTimeSlotValid && isDateValid;

        //Assert
        Assert.True(isValid, "BookningsData ska vara giltig");
    }
    [Fact]//Att ingen ska kunna boka något bakåt i tiden 
    public void CreateBooking_InThePast_ShouldFail()
    {
        //Arrange
        var bookingData = new DTOCreateBooking
        {
            Date = DateTime.Today.AddDays(-2),
            TimeSlot = "10-12",
            ResourceTypeId = 1,
            UserId = "testid"
        };
        //Act
        var isValid = bookingData.Date > DateTime.Today;
        //Assert
        Assert.False(isValid, "Bookning bakåt i tiden ska inte vara möjlig");
    }

}