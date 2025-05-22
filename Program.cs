using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS の設定追加
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("https://zealous-sand-0eede5a1e.6.azurestaticapps.net")
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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbPath = connectionString.Replace("Data Source=", "");

// 環境に応じたパスの設定
if (builder.Environment.IsDevelopment())
{
    // 開発環境では相対パスを絶対パスに変換
    dbPath = Path.Combine(AppContext.BaseDirectory, dbPath);
}
else
{
    // 本番環境では指定されたパスをそのまま使用
    // Databaseディレクトリが存在することを確認
    var databaseDir = Path.GetDirectoryName(dbPath);
    if (!Directory.Exists(databaseDir))
    {
        Directory.CreateDirectory(databaseDir);
    }
}

// 接続文字列を更新
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={dbPath}";

// SQLiteを使用するように設定
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

builder.Logging.AddConsole(options =>
{
    options.IncludeScopes = true;
});

builder.Services.AddHealthChecks()
    .AddCheck<SqliteHealthCheck>("Database");

var app = builder.Build();

// Production でもエラー詳細を見るために一時的に追加
app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORSを有効にする
app.UseCors("AllowAngularApp");

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

app.MapHealthChecks("/health");

// データベースの初期化
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // データベースが存在しない場合は作成
    context.Database.EnsureCreated();

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
