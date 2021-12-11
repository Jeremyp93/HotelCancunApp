using HotelCancun.Entities;
using HotelCancun.Entities.DbContexts;
using HotelCancun.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace HoelCancun.Tests;

/// <summary>
/// Test class for our API methods
/// </summary>
public class ApiTests
{
    public ApiTests()
    {
    }

    /// <summary>
    /// Test if the room is available on the given dates
    /// </summary>
    /// <param name="start">Start date</param>
    /// <param name="end">End date</param>
    /// <returns></returns>
    [Theory]
    [InlineData("20/12/2021", "22/12/2021")]
    [InlineData("22/12/2021", "23/12/2021")]
    [InlineData("23/12/2021", "26/12/2021")]
    public async Task IsRoomAvailable_ValidDates_ReturnTrue(string start, string end)
    {
        await using var application = new HotelCancunFactory();
        await CreateRoom(application);

        var client = application.CreateClient();

        var available = await client.GetFromJsonAsync<bool>($"/availability/?start={start}&end={end}");

        Assert.True(available);
    }

    public static IEnumerable<object[]> GetAvailableData()
    {
        yield return new object[] { DateTime.Now.AddDays(-1).ToString("dd'/'MM'/'yyyy"), DateTime.Now.ToString("dd'/'MM'/'yyyy") };
        yield return new object[] { DateTime.Now.AddDays(5).ToString("dd'/'MM'/'yyyy"), DateTime.Now.AddDays(6).ToString("dd'/'MM'/'yyyy") };
        yield return new object[] { DateTime.Now.AddDays(9).ToString("dd'/'MM'/'yyyy"), DateTime.Now.AddDays(11).ToString("dd'/'MM'/'yyyy") };

    }
    /// <summary>
    /// Test if the dates are not available that the API will return false
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    [Theory]
    [MemberData(nameof(GetAvailableData))]
    public async Task IsRoomAvailable_DatesNotAvailable_ReturnFalse(string start, string end)
    {
        await using var application = new HotelCancunFactory();
        var room = await CreateRoom(application);
        await InitializeReservations(application, room.Id);

        var client = application.CreateClient();

        var available = await client.GetFromJsonAsync<bool>($"/availability/?start={start}&end={end}");
        Assert.False(available);
    }

    public static IEnumerable<object[]> CreateData()
    {
        yield return new object[] { new InsertReservationModel { Name = "Foo", StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(2) } };
        yield return new object[] { new InsertReservationModel { Name = "Foo", StartDate = DateTime.Now.AddDays(10), EndDate = DateTime.Now.AddDays(12) } };
        yield return new object[] { new InsertReservationModel { Name = "Foo", StartDate = DateTime.Now.AddDays(29), EndDate = DateTime.Now.AddDays(30) } };
    }
    /// <summary>
    /// Test to see if we can create a reservation
    /// </summary>
    /// <param name="insertReservation"></param>
    /// <returns></returns>
    [Theory]
    [MemberData(nameof(CreateData))]
    public async Task CreateReservation_RoomIsAvailable_ReservationCreated(InsertReservationModel insertReservation)
    {
        await using var application = new HotelCancunFactory();
        await CreateRoom(application);
        var client = application.CreateClient();
        try
        {
            var response = await client.PostAsJsonAsync("/reservation", insertReservation);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var res = await response.Content.ReadFromJsonAsync<ReservationModel>();
            var reservation = await client.GetFromJsonAsync<ReservationModel>($"/reservation/{res.ReservationNumber}");
            Assert.Equal("Foo", reservation?.Name);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static IEnumerable<object[]> CreateDatesNotAvailableData()
    {
        yield return new object[] { new InsertReservationModel { Name = "Foo", StartDate = DateTime.Now.AddDays(30), EndDate = DateTime.Now.AddDays(31) } };
        yield return new object[] { new InsertReservationModel { Name = "Foo", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(6) } };
        yield return new object[] { new InsertReservationModel { Name = "Foo", StartDate = DateTime.Now.AddDays(9), EndDate = DateTime.Now.AddDays(11) } };
        yield return new object[] { new InsertReservationModel { Name = "Foo", StartDate = DateTime.Now.AddDays(20), EndDate = DateTime.Now.AddDays(25) } };
    }
    /// <summary>
    /// Test to create a reservation when dates are not valid nor available
    /// </summary>
    /// <param name="insertReservation"></param>
    /// <returns></returns>
    [Theory]
    [MemberData(nameof(CreateDatesNotAvailableData))]
    public async Task CreateReservation_RoomIsNotAvailable_ReturnBadRequest(InsertReservationModel insertReservation)
    {
        await using var application = new HotelCancunFactory();
        var room = await CreateRoom(application);
        await InitializeReservations(application, room.Id);
        var client = application.CreateClient();

        var response = await client.PostAsJsonAsync("/reservation", insertReservation);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public static IEnumerable<object[]> UpdateData()
    {
        yield return new object[] { new InsertReservationModel { Name = "Updated Foo", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(6) } };
        yield return new object[] { new InsertReservationModel { Name = "Updated Foo", StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(2) } };
        yield return new object[] { new InsertReservationModel { Name = "Updated Foo", StartDate = DateTime.Now.AddDays(6), EndDate = DateTime.Now.AddDays(7) } };
        yield return new object[] { new InsertReservationModel { Name = "Updated Foo", StartDate = DateTime.Now.AddDays(29), EndDate = DateTime.Now.AddDays(30) } };
    }
    /// <summary>
    /// Test to update an existing reservation
    /// </summary>
    /// <param name="insertReservation"></param>
    /// <returns></returns>
    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateReservation_ReservationExists_ReservationUpdated(InsertReservationModel insertReservation)
    {
        await using var application = new HotelCancunFactory();
        var room = await CreateRoom(application);
        var reservation = await CreateReservation(application, "Foo", DateTime.Now.AddDays(5), DateTime.Now.AddDays(6), 1, room.Id);
        var client = application.CreateClient();

        var response = await client.PutAsJsonAsync($"/reservation/{reservation.ReservationNumber}", insertReservation);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var reservationUpdated = await client.GetFromJsonAsync<ReservationModel>($"/reservation/{reservation.ReservationNumber}");
        Assert.Equal("Updated Foo", reservationUpdated?.Name);
    }

    public static IEnumerable<object[]> UpdateDateNotAvailableData()
    {
        yield return new object[] { new InsertReservationModel { Name = "Updated Foo", StartDate = DateTime.Now.AddDays(12), EndDate = DateTime.Now.AddDays(14) } };
        yield return new object[] { new InsertReservationModel { Name = "Updated Foo", StartDate = DateTime.Now.AddDays(30), EndDate = DateTime.Now.AddDays(35) } };
    }
    /// <summary>
    /// Test to see if updating a reservation on dates that are not available will return bad request
    /// </summary>
    /// <param name="insertReservation"></param>
    /// <returns></returns>
    [Theory]
    [MemberData(nameof(UpdateDateNotAvailableData))]
    public async Task UpdateReservation_ReservationExistsDatesNotFree_ReturnBadRequest(InsertReservationModel insertReservation)
    {
        await using var application = new HotelCancunFactory();
        var room = await CreateRoom(application);
        await InitializeReservations(application, room.Id);
        var client = application.CreateClient();

        var response = await client.PutAsJsonAsync($"/reservation/1", insertReservation);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    /// <summary>
    /// Test to delete an existing reservation
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteReservation_ReservationExist_ReservationDeleted()
    {
        await using var application = new HotelCancunFactory();
        var room = await CreateRoom(application);
        var reservation = await CreateReservation(application, "Foo", DateTime.Now.AddDays(5), DateTime.Now.AddDays(6), 1, room.Id);
        var client = application.CreateClient();
        var response = await client.DeleteAsync($"/reservation/{reservation.ReservationNumber}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var responseGet = await client.GetAsync($"/reservation/{reservation.ReservationNumber}");
        Assert.Equal(HttpStatusCode.NotFound, responseGet.StatusCode);
    }
    /// <summary>
    /// Test to delete a reservation number that does not exist
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteReservation_ReservationDoesNotExist_ReturnNotFound()
    {
        await using var application = new HotelCancunFactory();
        var room = await CreateRoom(application);
        var client = application.CreateClient();
        var response = await client.DeleteAsync($"/reservation/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #region Helper methods
    private static async Task<Room> CreateRoom(HotelCancunFactory? application)
    {
        var room = new Room { Id = Guid.NewGuid(), Name = "Room 1", NumberBeds = 4 };
        using (var scope = application?.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            using (var dbContext = provider.GetRequiredService<HotelCancunContext>())
            {
                await dbContext.Database.EnsureCreatedAsync();

                await dbContext.Rooms.AddAsync(room);
                await dbContext.SaveChangesAsync();
                return room;
            }
        }
    }

    private static async Task<Reservation> CreateReservation(HotelCancunFactory? application, string name, DateTime startDate, DateTime endDate, int reservationnumber, Guid roomId)
    {
        var reservation = new Reservation { Id = Guid.NewGuid(), Name = name, StartDate = startDate, EndDate = endDate, RoomId = roomId, ReservationNumber = reservationnumber };
        using (var scope = application?.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            using (var dbContext = provider.GetRequiredService<HotelCancunContext>())
            {
                await dbContext.Database.EnsureCreatedAsync();
                await dbContext.Reservations.AddAsync(reservation);
                await dbContext.SaveChangesAsync();
                return reservation;
            }
        }
    }

    private static async Task InitializeReservations(HotelCancunFactory? application, Guid roomId)
    {
        List<InsertReservationModel> resList = new List<InsertReservationModel>
        {
            new InsertReservationModel
            {
                Name = "Foo 1",
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(7),
            },
            new InsertReservationModel
            {
                Name = "Foo 2",
                StartDate = DateTime.Now.AddDays(9),
                EndDate = DateTime.Now.AddDays(11),
            },
            new InsertReservationModel
            {
                Name = "Foo 3",
                StartDate = DateTime.Now.AddDays(12),
                EndDate = DateTime.Now.AddDays(13),
            }
        };

        using (var scope = application?.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            using (var dbContext = provider.GetRequiredService<HotelCancunContext>())
            {
                await dbContext.Database.EnsureCreatedAsync();
                for (int i = 0; i < resList.Count; i++)
                {
                    var reservation = new Reservation
                    {
                        Name = resList[i].Name,
                        ReservationNumber = i + 1,
                        Id = Guid.NewGuid(),
                        StartDate = resList[i].StartDate,
                        EndDate = resList[i].EndDate,
                        RoomId = roomId
                    };
                    await dbContext.Reservations.AddAsync(reservation);
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
    #endregion
}