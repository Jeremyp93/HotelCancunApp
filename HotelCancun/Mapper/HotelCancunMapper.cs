namespace HotelCancun.Mapper;

/// <summary>
/// Class used to create the profiles to map databse objects with models
/// </summary>
public class HotelCancunMapper : Profile
{
    public HotelCancunMapper()
    {
        CreateMap<Room, RoomModel>();
        CreateMap<ReservationModel, Reservation>();
        CreateMap<Reservation, ReservationModel>();
        CreateMap<InsertReservationModel, ReservationModel>();
        //CreateMap<User, DbUser>().ForMember(u => u.PasswordHash, opt => opt.MapFrom(u2 => provider.Protect(u2.Password)));
    }
}

