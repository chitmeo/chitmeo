using System.Text.Json.Serialization;

namespace ChitMeo.Shared.Configuration.Options;

public class DataConnectionOptions
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DataProvider Provider { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}