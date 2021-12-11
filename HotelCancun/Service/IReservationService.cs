using HotelCancun.Entities.DbContexts;

namespace HotelCancun.Service;

/// <summary>
/// All functions concerning reservations will be handled here
/// </summary>
public interface IReservationService
{
    bool IsRoomAvailble(DateTime startDate, DateTime endDate, int reservationNumber = 0);
    ReservationModel GetReservation(int reservationNumber);
    Task<ReservationModel> CreateReservation(InsertReservationModel reservation);
    Task CancelReservation(int reservationNumber);
    Task UpdateReservation(int reservationNumber, InsertReservationModel reservation);
}

public class ReservationService : IReservationService
{
    private readonly HotelCancunContext _dbContext;
    private readonly IMapper _mapper;

    public ReservationService(HotelCancunContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Function checking if the room is available
    /// </summary>
    /// <param name="startDate">Start date of the reservation</param>
    /// <param name="endDate">End date of the reservation</param>
    /// <returns>A boolean indicating if the room is available</returns>
    public bool IsRoomAvailble(DateTime startDate, DateTime endDate, int reservationNumber = 0)
    {
        if (startDate == DateTime.MinValue || endDate == DateTime.MinValue) throw new System.Exception("Given dates are not valid dates.");
        if (endDate < startDate) throw new System.Exception("End date is sooner than start date.");
        if (startDate < DateTime.Now) return false;
        var room = _dbContext.Rooms.FirstOrDefault();
        if (room is null) throw new System.Exception("The room was not found.");
        var reservation = reservationNumber == 0 ?
            _dbContext.Reservations.FirstOrDefault(x => x.RoomId == room.Id && (x.StartDate <= endDate.Date && x.EndDate >= startDate.Date)) :
            _dbContext.Reservations.FirstOrDefault(x => x.RoomId == room.Id && (x.StartDate <= endDate.Date && x.EndDate >= startDate.Date) && x.ReservationNumber != reservationNumber);
        return reservation is null;
    }

    /// <summary>
    /// Function that stores a reservation in de database
    /// </summary>
    /// <param name="reservation">The reservation object</param>
    /// <returns>The reservation number</returns>
    public async Task<ReservationModel> CreateReservation(InsertReservationModel insertReservation)
    {
        var res = _dbContext.Reservations.ToList();
        if (!IsRoomAvailble(insertReservation.StartDate, insertReservation.EndDate))
            throw new System.Exception("Room is not available.");
        ValidateDates(insertReservation.StartDate, insertReservation.EndDate);
        var reservation = _mapper.Map<ReservationModel>(insertReservation);
        var resDb = _mapper.Map<Reservation>(reservation);
        var room = _dbContext.Rooms.FirstOrDefault();
        if (room is null) throw new System.Exception("The room was not found.");
        resDb.RoomId = room.Id;
        _dbContext.Add(resDb);
        await _dbContext.SaveChangesAsync();
        reservation = _mapper.Map<ReservationModel>(resDb);
        return reservation;
    }

    /// <summary>
    /// Function that deletes the reservation from the database
    /// </summary>
    /// <param name="reservationNumber">The reservation number</param>
    public async Task CancelReservation(int reservationNumber)
    {
        var reservation = _dbContext.Reservations.FirstOrDefault(x => x.ReservationNumber == reservationNumber);
        if (reservation is null) throw new ReservationNotFoundException();
        _dbContext.Remove(reservation);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Function that updates the reservation in the database
    /// </summary>
    /// <param name="reservationNumber">The reservation number</param>
    /// <param name="reservation">The reservation object</param>
    public async Task UpdateReservation(int reservationNumber, InsertReservationModel reservation)
    {
        ValidateDates(reservation.StartDate, reservation.EndDate);
        var resDb = _dbContext.Reservations.FirstOrDefault(x => x.ReservationNumber == reservationNumber);
        if (resDb is null) throw new ReservationNotFoundException();
        if (resDb.StartDate.Date != reservation.StartDate.Date || resDb.EndDate.Date != reservation.EndDate.Date)
        {
            if (!IsRoomAvailble(reservation.StartDate, reservation.EndDate, resDb.ReservationNumber))
                throw new System.Exception("Room is not available.");
        }
        resDb.Name = reservation.Name;
        resDb.StartDate = reservation.StartDate;
        resDb.EndDate = reservation.EndDate;
        _dbContext.Update(resDb);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Function that gets the reservation based on id
    /// </summary>
    /// <param name="id">The reservation id</param>
    /// <returns>The reservation</returns>
    public ReservationModel GetReservation(int reservationNumber)
    {
        var res = _dbContext.Reservations.FirstOrDefault(x => x.ReservationNumber == reservationNumber);
        if (res is null) throw new ReservationNotFoundException();
        return _mapper.Map<ReservationModel>(res);
    }

    /// <summary>
    /// Function that throws errors if dates are not valid for a reservation
    /// </summary>
    /// <param name="startDate">Start date of the reservation</param>
    /// <param name="endDate">End date of the reservation</param>
    private void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate) throw new System.Exception("End date is sooner than start date.");
        if (startDate < DateTime.Now) throw new System.Exception("You can't make reservations for the past.");
        if ((endDate.Date - startDate.Date).Days > 3) throw new System.Exception("You can't make a reservation for longer than 3 days.");
        if ((startDate.Date - DateTime.Now.Date).Days > 30 || (endDate.Date - DateTime.Now.Date).Days > 30) throw new System.Exception("You can only make reservations 30 days in advance.");
    }
}