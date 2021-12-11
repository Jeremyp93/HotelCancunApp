namespace HotelCancun.Model;

/// <summary>
/// Room record
/// </summary>
public class RoomModel
{
    public Guid Id => Guid.NewGuid();
    public string Name { get; set; }
    public int NumberBeds { get; set; }
}
