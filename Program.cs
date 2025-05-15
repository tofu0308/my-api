
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

var builder = WebApplication.CreateBuilder(args);

// SQLiteパスを絶対パスにして設定
var relativePath = builder.Configuration.GetConnectionString("DefaultConnection");
var absolutePath = Path.Combine(AppContext.BaseDirectory, relativePath.Replace("Data Source=", ""));
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={absolutePath}";

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS の設定追加
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular開発サーバーのURL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 環境名（例：Development, Production など）に応じた appsettings ファイルを読み込む
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// DbContext に接続文字列を適用
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var env = builder.Environment.EnvironmentName;
Console.WriteLine($"【現在の環境】{env}");

var conn = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"【使用されている接続文字列】{conn}");


// Swaggerのサービス登録
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// CORSを有効にする
app.UseCors("AllowAngularApp");

// Swaggerのミドルウェア追加
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Memos.Any())
    {
        context.Memos.AddRange(
            new Memo { Title = "初期メモ1", Status = MemoStatus.ToDo },
            new Memo { Title = "初期メモ2", Status = MemoStatus.Completed }
        );
        context.SaveChanges();
    }
}
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
