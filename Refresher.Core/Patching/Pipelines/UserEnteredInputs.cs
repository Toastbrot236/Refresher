namespace Refresher.Core.Patching.Pipelines;

/// <summary>
/// Additional data passed from GUI controller, mostly user-entered inputs
/// </summary>
public class UserEnteredInputs
{
    public string? TitleID { get; set; }

    /// <summary>
    /// Server URL entered by the user. Used for the AutoDiscover request.
    /// </summary>
    public string? EnteredServerUrl { get; set; }
    /// <summary>
    /// Join key entered by the user, before it's hashed.
    /// </summary>
    public string? EnteredJoinKey { get; set; }
    
    // TODO Patchwork 2.0 stuff, like UseJoinKey

    // TODO also allow specifying path to imported_patches.yml when we change RPCS3 patching
    public string? Rpcs3DevHdd0Path { get; set; }

    /// <summary>
    /// The IP address of the console, so we could connect to its FTP server
    /// </summary>
    public string? ConsoleIP { get; set; }
    
    // TODO PS4 inputs, like FTP port and bluesphere token

    // Legacy stuff
    public string? InputElfPath { get; set; }
    public string? OutputElfPath { get; set; }

    public static UserEnteredInputs FromIni(StreamReader reader)
    {
        Dictionary<string, string> ini = new();
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (line == null)
                break;
            
            if(string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || !line.Contains('='))
                continue;

            int equals = line.IndexOf('=');

            string key = line[..equals];
            string value = line[(equals + 1)..];

            ini[key] = value;
        }

        return new()
        {
            // TODO maybe we want to rename some of these keys eventually to be accurate and to not collide with some future stuff?
            // ip -> ps3-ip, url -> server-main-url, hdd0-path -> rpcs3-hdd0-path etc.
            ConsoleIP = ini.GetValueOrDefault("ip"),
            TitleID = ini.GetValueOrDefault("title-id"),
            EnteredServerUrl = ini.GetValueOrDefault("url"),
            Rpcs3DevHdd0Path = ini.GetValueOrDefault("hdd0-path"),
            EnteredJoinKey = ini.GetValueOrDefault("lobby-password"),
            InputElfPath = ini.GetValueOrDefault("elf-input"),
            OutputElfPath = ini.GetValueOrDefault("elf-output"),
        };
    }

    public string ToIni()
    {
        return $"ip={this.ConsoleIP} \n" +
            $"title-id={this.TitleID} \n" +
            $"url={this.EnteredServerUrl} \n" +
            $"hdd0-path={this.Rpcs3DevHdd0Path} \n" +
            $"lobby-password={this.EnteredJoinKey} \n" +
            $"elf-input={this.InputElfPath} \n" +
            $"elf-output={this.OutputElfPath}";
    }
}