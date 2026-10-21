using Refresher.Core.Patching.Pipelines;
using Refresher.Core.Platform;

namespace Refresher.Core.Patching.Steps.Common;

public class OfferRegistrationPageStep : Step
{
    public OfferRegistrationPageStep(Pipeline pipeline) : base(pipeline)
    {}

    public override string Name { get; } = "Downloading server data via AutoDiscover";
    public override float Progress { get; protected set; }
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // TODO just assuming that discovered URL + /register = registration web page URL is not good, 
        // the server should be able to specify the actual URL via AutoDiscover instead (if it's null, we would skip this offer).
        string? serverUrl = this.Pipeline.GetServerUrl();
        if (serverUrl == null) return Task.CompletedTask; // Unimportant, and shouldn't even happen at this point, so skip.

        QuestionResult result = this.Platform.Ask(
                "Patch successful!\r\n\r\n" +
                        "Some servers like Bonsai require registering an account to play. " +
                        "Would you like to open the server's website?");

        if (result != QuestionResult.Yes) return Task.CompletedTask;
        this.Progress = 0.5f;
        
        try
        {
            Uri oldUri = new(serverUrl);
            UriBuilder builder = new()
            {
                Host = oldUri.Host,
                Scheme = "https",
                Path = "/register",
                Port = 443,
            };

            this.Platform.OpenUrl(builder.Uri);
        }
        catch (Exception e)
        {
            State.Logger.LogWarning(LogType.Platform, "Failed to open registration page URL: " + e);
            this.Platform.WarnPrompt("Sorry, we couldn't open the URL. You can usually reach it by using the same link as the one you gave to Refresher.");
        }

        this.Progress = 1.0f;
        return Task.CompletedTask;
    }
}