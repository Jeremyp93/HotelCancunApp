namespace HotelCancun.Entities.DbContexts;

/// <summary>
/// Databse context of the HotelCancun database used in this project
/// </summary>
public partial class HotelCancunContext : DbContext
{
    public HotelCancunContext(DbContextOptions<HotelCancunContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Reservation> Reservations { get; set; } = null!;
    public virtual DbSet<Room> Rooms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasIndex(e => new { e.RoomId, e.StartDate, e.EndDate }, "Room_Unique_Reservation")
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.EndDate).HasColumnType("date");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.ReservationNumber).ValueGeneratedOnAdd();

            entity.Property(e => e.StartDate).HasColumnType("date");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.NumberBeds }, "Name_Beds_Unique")
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
