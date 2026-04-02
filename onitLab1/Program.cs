using Microsoft.EntityFrameworkCore;
using OnitLab1.Data;

var builder = WebApplication.CreateBuilder(args);

// Принудительно указываем порт 80
builder.WebHost.UseUrls("http://*:80");

// Добавляем контроллеры и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Получаем строку подключения
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// Если переменная окружения не найдена, пробуем из конфигурации
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("Using connection string from appsettings.json");
}
else
{
    Console.WriteLine($"Using connection string from environment variable: {connectionString}");
}

// Если всё еще пусто, используем LocalDB по умолчанию
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Server=(localdb)\\mssqllocaldb;Database=OnitLab1Db;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    Console.WriteLine("Using default SQL Server LocalDB connection");
}

// Настраиваем DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString.Contains("Host=") || connectionString.Contains("host="))
    {
        options.UseNpgsql(connectionString);
        Console.WriteLine("✓ Using PostgreSQL");
    }
    else
    {
        options.UseSqlServer(connectionString);
        Console.WriteLine("✓ Using SQL Server");
    }
});

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

var app = builder.Build();

// Применяем миграции при запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Attempting to connect to database...");
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("Database connection successful!");
            // Используем EnsureCreated для автоматического создания БД
            dbContext.Database.EnsureCreated();
            Console.WriteLine("✓ Database ensured created");
        }
        else
        {
            Console.WriteLine("✗ Cannot connect to database");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Database error: {ex.Message}");
    }
}

// Настройка HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

Console.WriteLine("Application starting...");
app.Run();