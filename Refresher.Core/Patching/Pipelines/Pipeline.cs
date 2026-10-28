using System.Collections.Frozen;
using System.Diagnostics;
using Refresher.Core.Accessors;
using Refresher.Core.Patching.Steps;
using Refresher.Core.Patching.Steps.Common;
using Refresher.Core.Platform;
using Refresher.Core.Storage;
using Refresher.Core.Verification.AutoDiscover;
using GlobalState = Refresher.Core.State;

namespace Refresher.Core.Patching.Pipelines;

public abstract class Pipeline : IAccessesPlatform
{
    public abstract string Name { get; }

    public IPlatformInterface Platform => this.Context.Platform;
    internal List<GameInformation>? GameList => this.Context.GameList;
    public AutoDiscoverResponse? AutoDiscover => this.Context.AutoDiscover;
    public IEnumerable<string> GameNameWhitelist => this.Context.GameNameWhitelist;
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

    public virtual string? GuideLink => null;
    public virtual string? ShorthandUrlId => null;

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

    protected virtual Type? SetupAccessorStepType => null;
    public virtual bool ReplacesEboot => false;

    protected abstract List<Type> StepTypes { get; }
    private List<Step> _steps = [];
    
    private int _stepCount;
    private byte _currentStepIndex;
    private Step? _currentStep;

    public void Initialize(IPlatformInterface platform)
    {
        this.Platform = platform;

        List<StepInput> requiredInputs = [];
        
        this._steps = new List<Step>(this.StepTypes.Count + 1);
        
        if(this.SetupAccessorStepType != null)
            this.AddStep(requiredInputs, this.SetupAccessorStepType);

        foreach (Type type in this.StepTypes)
            this.AddStep(requiredInputs, type);
        
        this.RequiredInputs = requiredInputs.DistinctBy(i => i.Id).ToFrozenSet();
    }

    public void Reset()
    {
        this.Inputs.Clear();

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

    private void AddStep(List<StepInput> requiredInputs, Type type)
    {
        Debug.Assert(type.IsAssignableTo(typeof(Step)));

        Step step = (Step)Activator.CreateInstance(type, this)!;

        this._steps.Add(step);
        requiredInputs.AddRange(step.Inputs);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (this.State != PipelineState.NotStarted)
        {
            this.State = PipelineState.Error;
            throw new InvalidOperationException("Pipeline must be restarted before it can be executed again.");
        }

        // Validation should be done by either the form or the pipeline steps (or both if you really want to).

        GlobalState.Logger.LogInfo(LogType.Pipeline, $"Pipeline {this.GetType().Name} started.");
        this.State = PipelineState.Running;

        bool success = await this.RunListOfSteps(this._steps, cancellationToken);

        if (success)
        {
            GlobalState.Logger.LogInfo(LogType.Pipeline, $"Pipeline {this.GetType().Name} finished!");
            this.State = PipelineState.Finished;
        
            PreviousInputStorage.Write(this.Context.UserEntered);

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
                    Uri oldUri = new(url);
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
                    GlobalState.Logger.LogWarning(LogType.Platform, "Failed to open server URL: " + e);
                    this.Platform.WarnPrompt("Sorry, we couldn't open the URL. You can usually reach it by using the same link as the one you gave to Refresher.");
                }
            }
        }
        else
        {
            GlobalState.Logger.LogInfo(LogType.Pipeline, $"Pipeline {this} failed!");
            this.State = PipelineState.Error;
        }
    }

    private async Task<bool> RunListOfSteps(List<Step> steps, CancellationToken cancellationToken = default)
    {
        this._stepCount = steps.Count;
        byte i = 1;
        foreach (Step step in steps)
        {
            GlobalState.Logger.LogInfo(LogType.Pipeline, $"Executing {step.GetType().Name}... ({i}/{steps.Count})");
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
}