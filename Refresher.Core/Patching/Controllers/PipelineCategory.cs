using System.Diagnostics;
using Refresher.Core.Patching.Pipelines;
using Refresher.Core.Platform;
using Refresher.Core.Storage;

namespace Refresher.Core.Patching.Controllers;

/// <summary>
/// This is supposed to make it easier for forms to address (get/execute) pipelines in a controller.
/// </summary>
public enum PipelineCategory
{
    Main,
    RevertPatch,
    DownloadGameData,
    VerifyServerInput,
}