namespace HotelCancun.Entities;

/// <summary>
/// Class representing the Rooms database table
/// </summary>
public partial class Room
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int NumberBeds { get; set; }
}

