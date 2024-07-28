// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineBuilder
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineBuilder
  {
    private readonly ITaskTemplateStore m_templateStore;
    private readonly PipelineContextBuilder m_contextBuilder;

    public PipelineBuilder()
      : this((EvaluationOptions) null, (ICounterStore) null, (IPackageStore) null, (IResourceStore) null, (IList<IStepProvider>) null, (ITaskStore) null, (ITaskTemplateStore) null, (IPipelineIdGenerator) null, (IList<IPhaseProvider>) null, (IDictionary<string, bool>) null)
    {
    }

    public PipelineBuilder(
      EvaluationOptions expressionOptions = null,
      ICounterStore counterStore = null,
      IPackageStore packageStore = null,
      IResourceStore resourceStore = null,
      IList<IStepProvider> stepProviders = null,
      ITaskStore taskStore = null,
      ITaskTemplateStore templateStore = null,
      IPipelineIdGenerator idGenerator = null,
      IList<IPhaseProvider> phaseProviders = null,
      IDictionary<string, bool> featureFlags = null)
      : this(new PipelineContextBuilder(featureFlags, counterStore, packageStore, resourceStore, stepProviders, taskStore, idGenerator, expressionOptions, phaseProviders), templateStore)
    {
    }

    internal PipelineBuilder(IPipelineContext context)
      : this(new PipelineContextBuilder(context))
    {
    }

    private PipelineBuilder(PipelineContextBuilder contextBuilder, ITaskTemplateStore templateStore = null)
    {
      ArgumentUtility.CheckForNull<PipelineContextBuilder>(contextBuilder, nameof (contextBuilder));
      this.m_contextBuilder = contextBuilder;
      this.m_templateStore = templateStore;
    }

    public AgentQueueReference DefaultQueue { get; set; }

    public JObject DefaultAgentSpecification { get; set; }

    public CheckoutOptions DefaultCheckoutOptions { get; set; }

    public WorkspaceOptions DefaultWorkspaceOptions { get; set; }

    public int EnvironmentVersion
    {
      get => this.m_contextBuilder.EnvironmentVersion;
      set => this.m_contextBuilder.EnvironmentVersion = value;
    }

    public ICounterStore CounterStore => this.m_contextBuilder.CounterStore;

    public IPipelineIdGenerator IdGenerator => this.m_contextBuilder.IdGenerator;

    public IPackageStore PackageStore => this.m_contextBuilder.PackageStore;

    public IResourceStore ResourceStore => this.m_contextBuilder.ResourceStore;

    public IList<IVariable> UserVariables => this.m_contextBuilder.UserVariables;

    public IDictionary<string, VariableValue> SystemVariables => this.m_contextBuilder.SystemVariables;

    public IDictionary<string, bool> FeatureFlags => this.m_contextBuilder.FeatureFlags;

    public PipelineBuildResult Build(IList<PhaseNode> phases, BuildOptions options = null)
    {
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) phases, nameof (phases));
      options = options ?? BuildOptions.None;
      Stage stage = new Stage()
      {
        Name = PipelineConstants.DefaultJobName
      };
      stage.Phases.AddRange<PhaseNode, IList<PhaseNode>>((IEnumerable<PhaseNode>) phases);
      return this.Build((IList<Stage>) new Stage[1]{ stage }, options);
    }

    public PipelineBuildResult Build(IList<Stage> stages, BuildOptions options = null)
    {
      PipelineBuildContext buildContext = this.CreateBuildContext(options);
      if (this.DefaultCheckoutOptions != null)
      {
        foreach (RepositoryResource repositoryResource in buildContext.ResourceStore.Repositories.GetAll())
        {
          if (!repositoryResource.Properties.TryGetValue<CheckoutOptions>(RepositoryPropertyNames.CheckoutOptions, out CheckoutOptions _))
            repositoryResource.Properties.Set<CheckoutOptions>(RepositoryPropertyNames.CheckoutOptions, this.DefaultCheckoutOptions.Clone());
        }
      }
      PipelineProcess process = this.CreateProcess(buildContext, stages);
      ValidationResult result = buildContext.Validate(process);
      PipelineEnvironment pipelineEnvironment = result.Environment = new PipelineEnvironment();
      pipelineEnvironment.Version = buildContext.EnvironmentVersion;
      pipelineEnvironment.Counters.AddRange<KeyValuePair<string, int>, IDictionary<string, int>>((IEnumerable<KeyValuePair<string, int>>) buildContext.CounterStore.Counters);
      pipelineEnvironment.Resources.MergeWith(buildContext.ResourceStore.GetAuthorizedResources());
      pipelineEnvironment.UserVariables.AddRange<IVariable, IList<IVariable>>((IEnumerable<IVariable>) this.m_contextBuilder.UserVariables);
      pipelineEnvironment.SystemVariables.AddRange<KeyValuePair<string, VariableValue>, IDictionary<string, VariableValue>>((IEnumerable<KeyValuePair<string, VariableValue>>) this.m_contextBuilder.SystemVariables);
      return new PipelineBuildResult(result.Environment, process, result);
    }

    public PipelineBuildContext CreateBuildContext(BuildOptions options, bool includeSecrets = false) => this.m_contextBuilder.CreateBuildContext(options, this.PackageStore, includeSecrets);

    public PhaseExecutionContext CreatePhaseExecutionContext(
      StageInstance stage,
      PhaseInstance phase,
      IDictionary<string, PhaseInstance> dependencies = null,
      IDictionary<string, IDictionary<string, PhaseInstance>> stageDependencies = null,
      PipelineState state = PipelineState.InProgress,
      bool includeSecrets = false,
      IPipelineTraceWriter trace = null,
      ExecutionOptions executionOptions = null,
      IDictionary<string, bool> featureFlags = null)
    {
      return this.m_contextBuilder.CreatePhaseExecutionContext(stage, phase, dependencies, stageDependencies, state, includeSecrets, trace, executionOptions, featureFlags);
    }

    public StageExecutionContext CreateStageExecutionContext(
      StageInstance stage,
      IDictionary<string, StageInstance> dependencies = null,
      PipelineState state = PipelineState.InProgress,
      bool includeSecrets = false,
      IPipelineTraceWriter trace = null,
      ExecutionOptions executionOptions = null)
    {
      return this.m_contextBuilder.CreateStageExecutionContext(stage, dependencies, state, includeSecrets, trace, executionOptions);
    }

    public IList<PipelineValidationError> Validate(PipelineProcess process, BuildOptions options = null)
    {
      ArgumentUtility.CheckForNull<PipelineProcess>(process, nameof (process));
      return this.CreateBuildContext(options).Validate(process).Errors;
    }

    public IList<PipelineValidationError> Validate(
      IList<Step> steps,
      PhaseTarget target = null,
      BuildOptions options = null)
    {
      ArgumentUtility.CheckForNull<IList<Step>>(steps, nameof (steps));
      Phase phase = new Phase();
      phase.Steps.AddRange<Step, IList<Step>>((IEnumerable<Step>) steps);
      phase.Target = target;
      Stage stage = new Stage(PipelineConstants.DefaultJobName, (IList<PhaseNode>) new Phase[1]
      {
        phase
      });
      PipelineBuildContext buildContext = this.CreateBuildContext(options);
      PipelineProcess process = this.CreateProcess(buildContext, (IList<Stage>) new Stage[1]
      {
        stage
      });
      return buildContext.Validate(process).Errors;
    }

    public PipelineResources GetReferenceResources(IList<Step> steps, PhaseTarget target = null)
    {
      ArgumentUtility.CheckForNull<IList<Step>>(steps, nameof (steps));
      Phase phase = new Phase();
      phase.Steps.AddRange<Step, IList<Step>>((IEnumerable<Step>) steps);
      phase.Target = target;
      Stage stage = new Stage(PipelineConstants.DefaultJobName, (IList<PhaseNode>) new Phase[1]
      {
        phase
      });
      PipelineBuildContext buildContext = this.CreateBuildContext(new BuildOptions());
      PipelineProcess process = this.CreateProcess(buildContext, (IList<Stage>) new Stage[1]
      {
        stage
      });
      return buildContext.Validate(process).ReferencedResources;
    }

    private PipelineProcess CreateProcess(PipelineBuildContext context, IList<Stage> stages)
    {
      ArgumentUtility.CheckForNull<PipelineBuildContext>(context, nameof (context));
      foreach (Stage stage in (IEnumerable<Stage>) stages)
      {
        foreach (PhaseNode phase in (IEnumerable<PhaseNode>) stage.Phases)
        {
          if (phase.Target == null)
            phase.Target = (PhaseTarget) new AgentQueueTarget();
          if (phase.Target.Type == PhaseTargetType.Queue && this.DefaultQueue != null)
          {
            AgentQueueTarget target = phase.Target as AgentQueueTarget;
            bool flag = false;
            AgentQueueReference queue = target.Queue;
            if (queue == null)
              flag = true;
            else if (queue.Id == 0)
            {
              ExpressionValue<string> name = queue.Name;
              if (name == (ExpressionValue<string>) null || name.IsLiteral && string.IsNullOrEmpty(name.Literal))
                flag = true;
            }
            if (flag)
            {
              target.Queue = this.DefaultQueue.Clone();
              if (target.AgentSpecification == null)
                target.AgentSpecification = (JObject) this.DefaultAgentSpecification?.DeepClone();
            }
          }
          if (phase.Target.Type == PhaseTargetType.Queue && this.DefaultWorkspaceOptions != null)
          {
            AgentQueueTarget target = phase.Target as AgentQueueTarget;
            if (target.Workspace == null)
              target.Workspace = this.DefaultWorkspaceOptions.Clone();
          }
          IList<Step> stepList = (IList<Step>) null;
          if (phase.Type == PhaseType.Phase)
            stepList = (phase as Phase).Steps;
          if (stepList != null)
          {
            List<Step> values = new List<Step>();
            foreach (Step step1 in stepList.Where<Step>((Func<Step, bool>) (x => x.Enabled)))
            {
              if (step1.Type == StepType.Task)
              {
                TaskStep taskStep = step1 as TaskStep;
                values.Add((Step) taskStep);
              }
              else if (step1.Type == StepType.Group)
              {
                GroupStep groupStep = step1 as GroupStep;
                values.Add((Step) groupStep);
              }
              else if (step1.Type == StepType.TaskTemplate)
              {
                TaskTemplateStep step2 = step1 as TaskTemplateStep;
                IEnumerable<TaskStep> collection = this.m_templateStore != null ? this.m_templateStore.ResolveTasks(step2) : throw new ArgumentException(PipelineStrings.TemplateStoreNotProvided((object) step2.Name, (object) "ITaskTemplateStore"));
                values.AddRange((IEnumerable<Step>) collection);
              }
            }
            stepList.Clear();
            stepList.AddRange<Step, IList<Step>>((IEnumerable<Step>) values);
          }
        }
      }
      return new PipelineProcess(stages);
    }
  }
}
