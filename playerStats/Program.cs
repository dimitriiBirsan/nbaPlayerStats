using Microsoft.EntityFrameworkCore;
using playerStats.Data;
using playerStats.Services;
using playerStats.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "frontEndPolicy";

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
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddSession(options => {
	options.IdleTimeout = TimeSpan.FromMinutes(31);
	options.Cookie.HttpOnly = false;
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
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "./web_interface/build")),
    RequestPath = ""
});
app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/api/GetAllStatsForCurrentSeasonAsync", async ( PlayerService playerService) =>
{
    var players = await playerService.GetStatsForCurrentSeasonAsync();
    return players;

}).WithOpenApi();

app.MapGet("/api/GetAndSaveStatsAsync", async (PlayerService playerService) =>
{
	return await playerService.GetAndSaveStatsAsync();
}).WithOpenApi();

app.MapGet("/api/checkIfUserIsLoggedIn", async (UserService userService, HttpContext httpContext) =>
{
	return await userService.CheckIfUserIsLoggedIn(httpContext);
}).WithOpenApi();

app.MapPost("/api/CreateUser", async (UserService userService, string email) =>
{
	var user = await userService.CreateUserAsync(email);
	return user;
}).WithOpenApi();

app.MapPost("/api/AddFavoritePlayer", async ([FromBody] AddFavoritePlayer favoritePlayer,  UserService userService,  HttpContext httpContext) =>
{
	return await userService.AddFavoritePlayerAsync(favoritePlayer, httpContext);
}).WithOpenApi();


app.MapGet("/api/GetTrendiestPlayer", async ([FromBody] string? userEmail, PlayerService playerService, HttpContext httpContext) =>
{
	var trendiestPlayers = await playerService.GetTrendiestPlayersAsync(userEmail, httpContext);
    return trendiestPlayers;
}).WithOpenApi();

app.MapPost("/api/login", async ( UserLoginRequest loginRequest, UserService userService, HttpContext httpContext) =>
{
	return await userService.LoginAsync(loginRequest.Email, httpContext);
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

