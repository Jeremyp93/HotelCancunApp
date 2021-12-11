namespace HotelCancun.Model;
/// <summary>
/// Reservation record
/// </summary>
public class InsertReservationModel
{
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
