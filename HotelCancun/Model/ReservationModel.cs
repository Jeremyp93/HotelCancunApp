namespace HotelCancun.Model;
/// <summary>
/// Reservation record
/// </summary>
public class ReservationModel
{
    public Guid Id => Guid.NewGuid();
    public int ReservationNumber { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

}
