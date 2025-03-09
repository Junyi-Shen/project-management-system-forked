var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/", () =>
{
    return "API is working";
})
.WithName("GetHome");

// new endpoint

// /{price}/{tax} --> variable
// /100.34/0.15 
// receive by get price, tax
// {
//     price: 0.0,
//     tax: '0%',
//     final: 0.0 + tax 
// }

app.MapGet("/{price}/{tax}/", (string price, string tax) =>
{
    if (!decimal.TryParse(price, out decimal parsedPrice) || !decimal.TryParse(tax, out decimal parsedTax))
    {
        return Results.BadRequest(new { error = "Invalid price or tax format. Please try again." });
    }

    decimal taxAmount = parsedPrice * parsedTax;
    decimal finalPrice = parsedPrice + taxAmount;

    var response = new
    {
        price = parsedPrice.ToString("0.00"),
        tax = (parsedTax * 100).ToString("0") + "%",
        final = finalPrice.ToString("0.00")
    };

    return Results.Json(response);
})
.WithName("CalculatePrice");


app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
