using SCEToolSharp;

namespace Refresher.Core.Pipelines.Steps.Common;

public class ReadEbootContentIdStep : Step
{
    public ReadEbootContentIdStep(Pipeline pipeline) : base(pipeline)
    {}

    public override float Progress { get; protected set; }
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        string ebootPath = this.Game.DownloadedEbootPath!;
        
        LibSceToolSharp.Init();
        this.Progress = 0.5f;
        
        string? contentId = LibSceToolSharp.GetContentId(ebootPath)?.TrimEnd('\0');
        this.Game.ContentId = contentId;

        this.Progress = 1f;

        if(contentId != null)
            State.Logger.LogDebug(LogType.InfoRetrieval, "Got content ID from the game's EBOOT: {0}", contentId);
        else
            this.Platform.WarnPrompt("Unable to find content ID in the game's EBOOT. Encryption may or may not work.");
        return Task.CompletedTask;
    }
}