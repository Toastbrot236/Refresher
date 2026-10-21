namespace Refresher.Core.Patching.Pipelines;

/// <summary>
/// Additional data passed from GUI controller, mostly user-entered inputs
/// </summary>
public class UserEnteredContext
{
    public string TitleID { get; set; } = "";

    /// <summary>
    /// Server URL entered by the user. Used for the AutoDiscover request.
    /// </summary>
    public string EnteredServerUrl { get; set; } = "";
    /// <summary>
    /// Join key entered by the user, before it's hashed.
    /// </summary>
    public string EnteredJoinKey { get; set; } = "";
    
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
}