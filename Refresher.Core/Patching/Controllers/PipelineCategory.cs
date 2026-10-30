namespace Refresher.Core.Patching.Controllers;

/// <summary>
/// This is supposed to make it easier for forms to address (get/execute) pipelines in a controller.
/// </summary>
public enum PipelineCategory
{
    Main,
    RevertPatch,
    DownloadGameData,
    AutoDiscover,
}