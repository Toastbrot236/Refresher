using System.Collections.Frozen;
using System.Diagnostics;
using Refresher.Core.Accessors;
using Refresher.Core.Patching.Steps;
using Refresher.Core.Patching.Steps.Common;
using Refresher.Core.Platform;
using Refresher.Core.Storage;
using Refresher.Core.Verification.AutoDiscover;

namespace Refresher.Core.Patching.Pipelines;

public abstract class Pipeline : IAccessesPlatform
{
    /// <summary>
    /// These are no longer used for window title bars (we use PipelineController.Name for that now),
    /// so these names are now only used to quickly summarize what the pipeline actually does (e.g. "Patch game on PS3", or "Revert Eboot patch").
    /// </summary>
    public abstract string Name { get; }

    public IPlatformInterface Platform { 
        get => this.Context.Platform;
        set => this.Context.Platform = value;
    }
    internal List<GameInformation>? GameList { 
        get => this.Context.GameList;
        set => this.Context.GameList = value;
    }
    public AutoDiscoverResponse? AutoDiscover { 
        get => this.Context.AutoDiscover;
        set => this.Context.AutoDiscover = value;
    }
    public string[] GameNameWhitelist { 
        get => this.Context.GameNameWhitelist;
        set => this.Context.GameNameWhitelist = value;
    }

    /// <summary>
    /// Initialized by PipelineController. Some info gets set by the controller (e.g. game name lists, user-entered inputs),
    /// some gets set by its (other) pipelines (e.g. game list, AutoDiscover)
    /// </summary>
    public PipelineContext Context { get; set; } = null!;
    
    // These are not in the context because we don't need to share them between pipelines.
    // Every pipelines initiates them themselves, e.g. incase something happened to the game or target platform
    // inbetween pipeline runs by the same controller.
    public IPatcher? Patcher { get; set; }
    public PatchAccessor? Accessor { get; set; }
    public GameInformation? GameInformation { get; set; }
    public EncryptionDetails? EncryptionDetails { get; set; }

    public PipelineState State { get; private set; } = PipelineState.NotStarted;
    
    public float Progress
    {
        get
        {
            if (this.State == PipelineState.Finished)
                return 1;
            
            float completed = (this._currentStepIndex - 1) / (float)this._stepCount;
            float currentStep = this._currentStep?.Progress ?? 0f;
            float stepWeight = 1f / this._stepCount;
            
            return completed + currentStep * stepWeight;
        }
    }

    public float CurrentProgress => this.State == PipelineState.Finished ? 1 : this._currentStep?.Progress ?? 0;

    public virtual bool NeedsAccessor => false;

    protected abstract List<Type> StepTypes { get; }
    private List<Step> _steps = [];
    
    private int _stepCount;
    private byte _currentStepIndex;
    private Step? _currentStep;

    public virtual string GetStateText()
    {
        return this.State switch
        {
            // Most pipelines are patch pipelines, so default to terms related to patching
            PipelineState.NotStarted => "Patch!",
            PipelineState.Running => "Patching... (click to cancel)",
            PipelineState.Finished => "Finished! (click to patch again)",
            PipelineState.Cancelled => "Patch!",
            PipelineState.Error => "Retry (Patch failed)",
            _ => "Patch!",
        };
    }

    // From now on, PipelineController will use NeedsAccessor to determine whether to add an accessor setup type.
    public void Initialize(PipelineContext context)
    {
        this.Context = context;
        this._steps = new List<Step>(this.StepTypes.Count + 1);

        if (this.TargetPlatform != TargetPlatform.None && pipeline.NeedsAccessor)
        {
            Type? setupAccessorType = this.TargetPlatform switch
            {
                TargetPlatform.PS3 => typeof(SetupPS3AccessorStep),
                TargetPlatform.RPCS3 => typeof(SetupEmulatorAccessorStep),
                _ => null
            };
            
            if (setupAccessorType == null)
            {
                RefresherState.Logger.LogWarning(LogType.Pipeline, $"Target platform {this.TargetPlatform} has no corresponding accessor, so pipeline {pipeline} will not work! Please report this issue!");
            }
            else
            {
                RefresherState.Logger.LogDebug(LogType.Pipeline, $"Adding {setupAccessorType} as accessor setup step for target platform {context.TargetPlatform}, pipeline {this}.");
                this.AddStep(setupAccessorStepType);
            }
        }

        foreach (Type type in this.StepTypes)
        {
            RefresherState.Logger.LogDebug(LogType.Pipeline, $"Adding regular step {type} to pipeline {this}.");
            this.AddStep(type);
        }
    }

    public void Reset()
    {
        this.State = PipelineState.NotStarted;

        this._stepCount = 0;
        this._currentStepIndex = 0;
        this._currentStep = null;

        this.Patcher = null;
        if(this.Accessor is IDisposable disposable)
            disposable.Dispose();
        this.Accessor = null;
        this.GameInformation = null;
        this.EncryptionDetails = null;
    }

    private void AddStep(Type type)
    {
        Debug.Assert(type.IsAssignableTo(typeof(Step)));

        Step step = (Step)Activator.CreateInstance(type, this)!;

        this._steps.Add(step);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (this.State != PipelineState.NotStarted)
        {
            this.State = PipelineState.Error;
            throw new InvalidOperationException("Pipeline must be restarted before it can be executed again.");
        }

        // Validation should be done by either the form or the pipeline steps (or both if you really want to).

        RefresherState.Logger.LogInfo(LogType.Pipeline, $"Pipeline {this.GetType().Name} started.");
        this.State = PipelineState.Running;

        bool success = await this.RunListOfSteps(this._steps, cancellationToken);

        if (success)
        {
            RefresherState.Logger.LogInfo(LogType.Pipeline, $"Pipeline {this.GetType().Name} finished!");
            this.State = PipelineState.Finished;
        
            PreviousInputStorage.Write(this.Context.UserInputs);

            // TODO don't show this if the server doesn't specify a registration page URL via AutoDiscover v4
            // TODO allow user to set this prompt to not show at all
            QuestionResult result = this.Platform.Ask(
                "Patch successful!\r\n\r\n" +
                        "Some servers like Bonsai require registering an account to play. " +
                        "Would you like to open the server's website?");

            if (result == QuestionResult.Yes)
            {
                try
                {
                    Uri oldUri = new(this.Context.GetServerUrl());
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
                    RefresherState.Logger.LogWarning(LogType.Platform, "Failed to open server URL: " + e);
                    this.Platform.WarnPrompt("Sorry, we couldn't open the URL. You can usually reach it by using the same link as the one you gave to Refresher.");
                }
            }
        }
        else
        {
            RefresherState.Logger.LogInfo(LogType.Pipeline, $"Pipeline {this} failed!");
            this.State = PipelineState.Error;
        }
    }

    private async Task<bool> RunListOfSteps(List<Step> steps, CancellationToken cancellationToken = default)
    {
        this._stepCount = steps.Count;
        byte i = 1;
        foreach (Step step in steps)
        {
            RefresherState.Logger.LogInfo(LogType.Pipeline, $"Executing {step.GetType().Name}... ({i}/{steps.Count})");
            this._currentStepIndex = i;
            this._currentStep = step;

            try
            {
                await step.ExecuteAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                this.State = PipelineState.Cancelled;
                return false;
            }
            catch (Exception)
            {
                this.State = PipelineState.Error;
                throw;
            }

            if (this.State == PipelineState.Error)
            {
                return false;
            }

            i++;
        }

        return true;
    }

    public void Fail(Step? step, string reason)
    {
        this.Platform.ErrorPrompt($"{step?.GetType().Name ?? "Pipeline"} failed: {reason}\n\nPatching cannot continue.");
        this.State = PipelineState.Error;
    }

    public override string ToString()
    {
        return $"'{this.Name}' ({this.GetType().Name})";
    }
}