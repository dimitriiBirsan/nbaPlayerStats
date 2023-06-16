using Microsoft.EntityFrameworkCore;
using playerStats.Data;
using playerStats.Services;
using playerStats.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<Player>();
builder.Services.AddCors(options =>
{
	options.AddPolicy(
		name: MyAllowSpecificOrigins,
		policy =>
		{
			policy.WithOrigins("http://localhost:3000")
					.AllowAnyMethod()
					.AllowAnyHeader();
        });
});
builder.Services.AddSession(options => {
	options.IdleTimeout = TimeSpan.FromMinutes(31);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});


builder.Services.AddDbContext<playerStats.Data.PlayerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.UseSession();

//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/api/GetAllPlayersAsync", async ( PlayerService playerService) =>
{
    var players = await playerService.GetAllPlayersAsync();
    return players;

}).WithOpenApi();

app.MapGet("/api/checkIfUserIsLoggedIn", async (UserService userService) =>
{
	return await userService.CheckIfUserIsLoggedIn();
}).WithOpenApi();

app.MapPost("/api/CreateUser", async (UserService userService, string email) =>
{
	var user = await userService.CreateUserAsync(email);
	return user;
}).WithOpenApi();

app.MapPost("/api/AddFavoritePlayer", async (UserService userService, Player player) =>
{
	return await userService.AddFavoritePlayerAsync( player);
}).WithOpenApi();


app.MapGet("/api/GetTrendiestPlayer", async (PlayerService playerService) =>
{
	var trendiestPlayers = await playerService.GetTrendiestPlayersAsync();
    return trendiestPlayers;
}).WithOpenApi();

app.MapPost("/api/login", async ( UserLoginRequest loginRequest, UserService userService) =>
{
	return await userService.LoginAsync(loginRequest.Email);
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

