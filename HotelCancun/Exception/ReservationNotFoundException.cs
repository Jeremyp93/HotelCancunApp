namespace HotelCancun.Exception;

/// <summary>
/// Custom exception if a reservation is not found
/// </summary>
public class ReservationNotFoundException : System.Exception
{
    private static readonly string _defaultMessage = "Reservation was not found.";
    public ReservationNotFoundException() : base(_defaultMessage)
    {
    }

    public ReservationNotFoundException(string message)
        : base(message)
    {
    }

    public ReservationNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}

