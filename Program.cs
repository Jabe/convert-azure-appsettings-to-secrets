using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var appSettings = new List<Setting>();
var connectionStrings = new List<Setting>();

var options = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };

Console.WriteLine("Convert Settings to secrets.json. End input with newlines.");
Console.WriteLine();
Console.WriteLine("# Paste \"Application settings\" from Azure Portal (use advanced edit)");

if (JsonSerializer.Deserialize<AppSetting[]>(ReadAll()) is var a && a is not null)
    appSettings.AddRange(a);


Console.WriteLine("# Paste \"Connection strings\" from Azure Portal (use advanced edit)");

if (JsonSerializer.Deserialize<ConnectionString[]>(ReadAll()) is var b && b is not null)
    appSettings.AddRange(b);


var values = appSettings.Concat(connectionStrings).Cast<Setting>().ToDictionary(x => ConvertName(x), x => x.Value);

var data = JsonSerializer.Serialize(values, options);
Console.WriteLine(data);



static string ReadAll()
{
    var res = new StringBuilder();

    string? line;
    while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
    {
        res.AppendLine(line);
    }

    return res.ToString();
}

static string ConvertName(Setting value)
{
    return value switch
    {
        AppSetting => value.Name.Replace("__", ":"),
        ConnectionString => "ConnectionStrings:" + value.Name.Replace("__", ":"),
        _ => value.Name
    };
}

abstract record Setting([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("value")] string Value);
record AppSetting(string Name, string Value) : Setting(Name, Value);
record ConnectionString(string Name, string Value) : Setting(Name, Value);
