namespace UsingBlazor.Client.Pages;

public readonly record struct WeatherForecast
{
    public DateOnly Date { get; init; }

    public int TemperatureC { get; init; }

    public string? Summary { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public WeatherForecast()
    {
        Date = DateOnly.FromDateTime(DateTime.Now);
        Summary = string.Empty;
    }

    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }
}