using HotelCancun.Entities.DbContexts;
using HotelCancun.Mapper;
using HotelCancun.Middleware;
using HotelCancun.Service;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddDbContext<HotelCancunContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SQLDBConnection")));
builder.Services.AddScoped<IReservationService, ReservationService>();
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new HotelCancunMapper());
});
IMapper autoMapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(autoMapper);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_allowAllCors",
                      builder =>
                      {
                          builder.AllowAnyOrigin().AllowAnyMethod();
                      });
});

var app = builder.Build();
app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("_allowAllCors");

app.MapGet("/availability", (string start, string end, IReservationService reservationService) =>
{
    DateTime.TryParseExact(start, "dd'/'MM'/'yyyy",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out var startDate);
    DateTime.TryParseExact(end, "dd'/'MM'/'yyyy",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out var endDate);

    return reservationService.IsRoomAvailble(startDate, endDate);
});

app.MapGet("/reservation/{reservationNumber}", (int reservationNumber, IReservationService reservationService) =>
{
    return reservationService.GetReservation(reservationNumber);
});

app.MapPost("/reservation", async (InsertReservationModel res, IReservationService reservationService) => 
{
    var reservation = await reservationService.CreateReservation(res);
    return Results.Created($"/reservation/{reservation.ReservationNumber}", reservation);
});
app.MapDelete("/reservation/{reservationNumber}", async (int reservationNumber, IReservationService reservationService) =>
{
    await reservationService.CancelReservation(reservationNumber);
    return Results.NoContent();
});
app.MapPut("/reservation/{reservationNumber}", async (int reservationNumber, InsertReservationModel res, IReservationService reservationService) =>
{
    await reservationService.UpdateReservation(reservationNumber, res);
    return Results.NoContent();
});

app.Run();