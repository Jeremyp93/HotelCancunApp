namespace HotelCancun.Entities;

/// <summary>
/// Class representing the Reservations database table
/// </summary>
public partial class Reservation
{
    public Guid Id { get; set; }
    public int ReservationNumber { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid RoomId { get; set; }
}

