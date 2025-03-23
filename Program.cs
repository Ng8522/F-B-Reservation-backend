using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

string? connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing in appsettings.json!");
}

try
{
    using (var connection = new MySqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("Database connected!");

        string sqlFilePath = Path.Combine(AppContext.BaseDirectory, "maria", "migrations", "000_implement_tables.sql");
        string sqlScript = File.ReadAllText(sqlFilePath);

        using (var command = new MySqlCommand(sqlScript, connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("SQL script executed successfully!");
        }

        connection.Close();
    }
}
catch (Exception ex)
{
    Console.WriteLine("Error executing SQL script: " + ex.Message);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.UseCors("AllowAllOrigins");

app.UseAuthorization();
app.Run();

