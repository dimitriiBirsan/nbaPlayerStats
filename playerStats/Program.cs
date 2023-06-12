using Microsoft.EntityFrameworkCore;
using playerStats.Data;
using playerStats.Services;
using playerStats.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<Player>();


builder.Services.AddDbContext<playerStats.Data.PlayerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/GetAllPlayersAsync", async ( PlayerService playerService) =>
{
    var players = await playerService.GetAllPlayersAsync();
    return players;

}).WithOpenApi();

app.MapPost("/CreateUser", async (UserService userService, string email) =>
{
	var user = await userService.CreateUserAsync(email);
	return user;
}).WithOpenApi();

app.MapPost("/AddFavoritePlayer", async (UserService userService, string email, Player player) =>
{
	await userService.AddFavoritePlayerAsync(email, player);
	return Results.Ok(player);
}).WithOpenApi();

app.MapGet("/GetTrendiestPlayer", async (PlayerService playerService) =>
{
	var trendiestPlayers = await playerService.GetTrendiestPlayersAsync();
    return trendiestPlayers;
}).WithOpenApi();


using (var scope = app.Services.CreateScope())
{
    var services =  scope.ServiceProvider;
	try
	{
		var conext = services.GetRequiredService<PlayerContext>();

	}
	catch (Exception ex)
	{
		Console.WriteLine(ex);
		throw;
	}
}

app.Run();

