// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Phase
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class Phase : PhaseNode, IJobFactory
  {
    public bool inputsInjectionFeatureFlagEnabled;
    private static readonly Dictionary<string, int> NodeVersionToAgentVersion = new Dictionary<string, int>()
    {
      ["Node"] = 6,
      ["Node10"] = 10,
      ["Node14"] = 14
    };
    [DataMember(Name = "Steps", EmitDefaultValue = false)]
    private IList<Step> m_steps;

    public Phase()
    {
    }

    private Phase(Phase phaseToCopy)
      : base((PhaseNode) phaseToCopy)
    {
      if (phaseToCopy.m_steps == null || phaseToCopy.m_steps.Count <= 0)
        return;
      this.m_steps = (IList<Step>) new List<Step>(phaseToCopy.m_steps.Select<Step, Step>((Func<Step, Step>) (x => x.Clone())));
    }

    [DataMember(EmitDefaultValue = false)]
    public override PhaseType Type => PhaseType.Phase;

    public IList<Step> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = (IList<Step>) new List<Step>();
        return this.m_steps;
      }
    }

    public JobExecutionContext CreateJobContext(
      PhaseExecutionContext context,
      string name,
      int attempt)
    {
      ArgumentUtility.CheckForNull<PhaseTarget>(this.Target, "Target");
      return this.Target.CreateJobContext(context, name, attempt, (IJobFactory) this);
    }

    public ExpandPhaseResult Expand(PhaseExecutionContext context, JobExpansionOptions options = null)
    {
      ArgumentUtility.CheckForNull<PhaseTarget>(this.Target, "Target");
      ExpandPhaseResult expandPhaseResult = this.Target.Expand(context, (IJobFactory) this, options);
      if (expandPhaseResult != null)
      {
        ExpressionResult<bool> expressionResult = this.ContinueOnError?.GetValue((IPipelineContext) context);
        expandPhaseResult.ContinueOnError = expressionResult != null && expressionResult.Value;
      }
      return expandPhaseResult;
    }

    internal static string GetErrorMessage(string code, params object[] values)
    {
      string str = (string) values[0];
      if (string.IsNullOrEmpty(str) || str.Equals(PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
      {
        switch (code)
        {
          case "NameInvalid":
            return PipelineStrings.PhaseNameInvalid(values[1]);
          case "NameNotUnique":
            return PipelineStrings.PhaseNamesMustBeUnique(values[1]);
          case "StartingPointNotFound":
            return PipelineStrings.PipelineNotValidNoStartingPhase();
          case "DependencyNotFound":
            return PipelineStrings.PhaseDependencyNotFound(values[1], values[2]);
          case "GraphContainsCycle":
            return PipelineStrings.PhaseGraphCycleDetected(values[1], values[2]);
        }
      }
      else
      {
        switch (code)
        {
          case "NameInvalid":
            return PipelineStrings.StagePhaseNameInvalid(values[0], values[1]);
          case "NameNotUnique":
            return PipelineStrings.StagePhaseNamesMustBeUnique(values[0], values[1]);
          case "StartingPointNotFound":
            return PipelineStrings.StageNotValidNoStartingPhase(values[0]);
          case "DependencyNotFound":
            return PipelineStrings.StagePhaseDependencyNotFound(values[0], values[1], values[2]);
          case "GraphContainsCycle":
            return PipelineStrings.StagePhaseGraphCycleDetected(values[0], values[1], values[2]);
        }
      }
      throw new NotSupportedException();
    }

    public override void Validate(PipelineBuildContext context, ValidationResult result)
    {
      base.Validate(context, result);
      Phase.StepValidationResult phaseStepValidationResult = new Phase.StepValidationResult();
      if (context.BuildOptions.DemandLatestAgent)
      {
        phaseStepValidationResult.MinAgentVersion = (context.PackageStore?.GetLatestVersion(WellKnownPackageTypes.Agent) ?? throw new NotSupportedException("Unable to determine the latest agent package version")).ToString();
        phaseStepValidationResult.MinAgentVersionSource = AgentFeatureDemands.YamlPipelinesDemandSource();
      }
      else if (!string.IsNullOrEmpty(context.BuildOptions.MinimumAgentVersion))
      {
        phaseStepValidationResult.MinAgentVersion = context.BuildOptions.MinimumAgentVersion;
        phaseStepValidationResult.MinAgentVersionSource = context.BuildOptions.MinimumAgentVersionDemandSource;
      }
      if (context.EnvironmentVersion < 2)
      {
        Step step = this.Steps.SingleOrDefault<Step>((Func<Step, bool>) (x => x.IsCheckoutTask()));
        if (step != null)
        {
          if ((step as TaskStep).Inputs[PipelineConstants.CheckoutTaskInputs.Repository] == PipelineConstants.NoneAlias)
            this.Variables.Add((IVariable) new Variable()
            {
              Name = "agent.source.skip",
              Value = bool.TrueString
            });
          this.Steps.Remove(step);
        }
      }
      Phase.ValidateSteps(context, (PhaseNode) this, this.Target, result, this.Steps, phaseStepValidationResult);
      bool flag = false;
      if (this.Target.Type == PhaseTargetType.Pool || this.Target.Type == PhaseTargetType.Server)
        flag = true;
      else if (this.Target is AgentQueueTarget target && target.IsLiteral())
        flag = true;
      if (!flag)
        return;
      this.Target.Validate((IPipelineContext) context, context.BuildOptions, result, this.Steps, (ISet<Demand>) phaseStepValidationResult.TaskDemands);
    }

    public static void ValidateSteps(
      PipelineBuildContext context,
      PhaseNode phase,
      PhaseTarget phaseTarget,
      ValidationResult result,
      IList<Step> steps,
      Phase.StepValidationResult phaseStepValidationResult)
    {
      List<Step> values1 = new List<Step>();
      foreach (Step step in (IEnumerable<Step>) steps)
      {
        if (step.Enabled)
        {
          if (step.Type == StepType.Task)
          {
            List<PipelineValidationError> values2 = Phase.ValidateTaskStep(context, phase, phaseTarget, result.ReferencedResources, result.UnauthorizedResources, step as TaskStep, phaseStepValidationResult);
            if (values2.Count == 0)
              values1.Add(step);
            else
              result.Errors.AddRange<PipelineValidationError, IList<PipelineValidationError>>((IEnumerable<PipelineValidationError>) values2);
          }
          else if (step.Type == StepType.Group)
          {
            List<PipelineValidationError> values3 = Phase.ValidateGroupStep(context, phase, phaseTarget, result.ReferencedResources, result.UnauthorizedResources, step as GroupStep, phaseStepValidationResult);
            if (values3.Count == 0)
              values1.Add(step);
            else
              result.Errors.AddRange<PipelineValidationError, IList<PipelineValidationError>>((IEnumerable<PipelineValidationError>) values3);
          }
          else
            result.Errors.Add(new PipelineValidationError(PipelineStrings.StepNotSupported()));
        }
      }
      steps.Clear();
      steps.AddRange<Step, IList<Step>>((IEnumerable<Step>) values1);
      if (phaseStepValidationResult.UnnamedSteps.Count > 0)
        Phase.GenerateDefaultTaskNames((ISet<string>) phaseStepValidationResult.KnownNames, (IDictionary<string, List<Step>>) phaseStepValidationResult.UnnamedSteps);
      if (phaseStepValidationResult.MinAgentVersion == null)
        return;
      phaseStepValidationResult.TaskDemands.Add((Demand) new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, phaseStepValidationResult.MinAgentVersion, phaseStepValidationResult.MinAgentVersionSource));
    }

    private static List<PipelineValidationError> ValidateTaskStep(
      PipelineBuildContext context,
      PhaseNode phase,
      PhaseTarget phaseTarget,
      PipelineResources referencedResources,
      PipelineResources unauthorizedResources,
      TaskStep taskStep,
      Phase.StepValidationResult stepValidationResult)
    {
      List<PipelineValidationError> pipelineValidationErrorList = new List<PipelineValidationError>();
      if (taskStep.Reference == null || taskStep.Reference.Version == null || taskStep.Reference.Id == Guid.Empty && string.IsNullOrEmpty(taskStep.Reference.Name))
      {
        pipelineValidationErrorList.Add(new PipelineValidationError(PipelineStrings.StepTaskReferenceInvalid((object) phase.Name, (object) taskStep.Name)));
        return pipelineValidationErrorList;
      }
      TaskDefinition taskDefinition = (TaskDefinition) null;
      try
      {
        if (taskStep.Reference.Id != Guid.Empty)
          taskDefinition = context.TaskStore?.ResolveTask(taskStep.Reference.Id, taskStep.Reference.Version);
        else if (!string.IsNullOrEmpty(taskStep.Reference.Name))
          taskDefinition = context.TaskStore?.ResolveTask(taskStep.Reference.Name, taskStep.Reference.Version);
      }
      catch (AmbiguousTaskSpecificationException ex)
      {
        pipelineValidationErrorList.Add(new PipelineValidationError(PipelineStrings.TaskStepReferenceInvalid((object) phase.Name, (object) taskStep.Name, (object) ex.Message)));
        return pipelineValidationErrorList;
      }
      catch (ArgumentException ex)
      {
        string str = taskStep.Reference.Id != Guid.Empty ? taskStep.Reference.Id.ToString() : taskStep.Reference.Name;
        pipelineValidationErrorList.Add(new PipelineValidationError(PipelineStrings.TaskVersionMissing((object) phase.Name, (object) taskStep.Name, (object) str, (object) taskStep.Reference.Version)));
        return pipelineValidationErrorList;
      }
      if (taskDefinition == null || taskDefinition.Disabled)
      {
        string str = taskStep.Reference.Id != Guid.Empty ? taskStep.Reference.Id.ToString() : taskStep.Reference.Name;
        pipelineValidationErrorList.Add(new PipelineValidationError(PipelineStrings.TaskMissing((object) phase.Name, (object) taskStep.Name, (object) str, (object) taskStep.Reference.Version)));
        return pipelineValidationErrorList;
      }
      if (!phaseTarget.IsValid(taskDefinition))
      {
        if (phaseTarget is ServerTarget)
          pipelineValidationErrorList.Add(new PipelineValidationError(PipelineStrings.TaskInvalidForServerTarget((object) phase.Name, (object) taskDefinition.Name)));
        else
          pipelineValidationErrorList.Add(new PipelineValidationError(PipelineStrings.TaskInvalidForGivenTarget((object) phase.Name, (object) taskStep.Name, (object) taskDefinition.Name, (object) taskDefinition.Version)));
        return pipelineValidationErrorList;
      }
      IList<int> restrictedNodeVersions = context.BuildOptions.RestrictedNodeVersions;
      if ((restrictedNodeVersions != null ? (restrictedNodeVersions.Count > 0 ? 1 : 0) : 0) != 0)
      {
        HashSet<int> nodeVersionsForTask = Phase.GetNodeVersionsForTask(taskDefinition);
        List<int> list = context.BuildOptions.RestrictedNodeVersions.Intersect<int>((IEnumerable<int>) nodeVersionsForTask).ToList<int>();
        if (list.Count > 0)
          pipelineValidationErrorList.Add(new PipelineValidationError(PipelineStrings.TaskUsingRestrictedNodeVersion((object) phase.Name, (object) taskStep.Name, (object) taskDefinition.Name, (object) taskDefinition.Version, (object) string.Join<int>(",", (IEnumerable<int>) list))));
      }
      taskStep.Reference.Id = taskDefinition.Id;
      taskStep.Reference.Name = taskDefinition.Name;
      taskStep.Reference.Version = (string) taskDefinition.Version;
      taskStep.IsServerOwned = new bool?(taskDefinition.ServerOwned);
      PipelineValidationError pipelineValidationError1 = Phase.ValidateStepCondition(context, phase, taskStep.Name, taskStep.Condition);
      if (pipelineValidationError1 != null)
        pipelineValidationErrorList.Add(pipelineValidationError1);
      IList<PipelineValidationError> collection = Phase.ResolveInputs(context, phase, referencedResources, unauthorizedResources, taskStep, taskDefinition);
      if (collection.Count > 0)
        pipelineValidationErrorList.AddRange((IEnumerable<PipelineValidationError>) collection);
      string defaultName = NameValidation.Sanitize(taskStep.Reference.Name, context.BuildOptions.AllowHyphenNames);
      PipelineValidationError pipelineValidationError2 = Phase.ValidateStepName(context, phase, stepValidationResult, (JobStep) taskStep, defaultName);
      if (pipelineValidationError2 != null)
        pipelineValidationErrorList.Add(pipelineValidationError2);
      stepValidationResult.TasksSatisfy.UnionWith((IEnumerable<string>) taskDefinition.Satisfies);
      bool newMinimum;
      stepValidationResult.MinAgentVersion = taskDefinition.GetMinimumAgentVersion(stepValidationResult.MinAgentVersion, out newMinimum);
      if (newMinimum)
        stepValidationResult.MinAgentVersionSource = new DemandSource()
        {
          SourceName = taskDefinition.Name,
          SourceVersion = (string) taskDefinition.Version,
          SourceType = DemandSourceType.Task
        };
      IEnumerable<Demand> demands = taskDefinition.Demands.Where<Demand>((Func<Demand, bool>) (d => !stepValidationResult.TasksSatisfy.Contains(d.Name)));
      if (demands.Any<Demand>())
        stepValidationResult.TaskDemands.UnionWith(demands);
      return pipelineValidationErrorList;
    }

    private static List<PipelineValidationError> ValidateGroupStep(
      PipelineBuildContext context,
      PhaseNode phase,
      PhaseTarget phaseTarget,
      PipelineResources referencedResources,
      PipelineResources unauthorizedResources,
      GroupStep groupStep,
      Phase.StepValidationResult stepValidationResult)
    {
      List<PipelineValidationError> pipelineValidationErrorList = new List<PipelineValidationError>();
      PipelineValidationError pipelineValidationError1 = Phase.ValidateStepCondition(context, phase, groupStep.Name, groupStep.Condition);
      if (pipelineValidationError1 != null)
        pipelineValidationErrorList.Add(pipelineValidationError1);
      Phase.StepValidationResult stepValidationResult1 = new Phase.StepValidationResult();
      List<TaskStep> values = new List<TaskStep>();
      foreach (TaskStep step in (IEnumerable<TaskStep>) groupStep.Steps)
      {
        if (step.Enabled)
        {
          List<PipelineValidationError> collection = Phase.ValidateTaskStep(context, phase, phaseTarget, referencedResources, unauthorizedResources, step, stepValidationResult1);
          if (collection.Count == 0)
            values.Add(step);
          else
            pipelineValidationErrorList.AddRange((IEnumerable<PipelineValidationError>) collection);
        }
      }
      groupStep.Steps.Clear();
      groupStep.Steps.AddRange<TaskStep, IList<TaskStep>>((IEnumerable<TaskStep>) values);
      if (stepValidationResult1.UnnamedSteps.Count > 0)
        Phase.GenerateDefaultTaskNames((ISet<string>) stepValidationResult1.KnownNames, (IDictionary<string, List<Step>>) stepValidationResult1.UnnamedSteps);
      if (DemandMinimumVersion.CompareVersion(stepValidationResult1.MinAgentVersion, stepValidationResult.MinAgentVersion) > 0)
      {
        stepValidationResult.MinAgentVersion = stepValidationResult1.MinAgentVersion;
        stepValidationResult.MinAgentVersionSource = stepValidationResult1.MinAgentVersionSource;
      }
      stepValidationResult.TasksSatisfy.UnionWith((IEnumerable<string>) stepValidationResult1.TasksSatisfy);
      IEnumerable<Demand> demands = stepValidationResult1.TaskDemands.Where<Demand>((Func<Demand, bool>) (d => !stepValidationResult.TasksSatisfy.Contains(d.Name)));
      if (demands.Any<Demand>())
        stepValidationResult.TaskDemands.UnionWith(demands);
      PipelineValidationError pipelineValidationError2 = Phase.ValidateStepName(context, phase, stepValidationResult, (JobStep) groupStep, "group");
      if (pipelineValidationError2 != null)
        pipelineValidationErrorList.Add(pipelineValidationError2);
      return pipelineValidationErrorList;
    }

    private static PipelineValidationError ValidateStepName(
      PipelineBuildContext context,
      PhaseNode phase,
      Phase.StepValidationResult stepValidationResult,
      JobStep step,
      string defaultName)
    {
      if (string.IsNullOrEmpty(step.Name))
      {
        List<Step> stepList;
        if (!stepValidationResult.UnnamedSteps.TryGetValue(defaultName, out stepList))
        {
          stepList = new List<Step>();
          stepValidationResult.UnnamedSteps.Add(defaultName, stepList);
        }
        stepList.Add((Step) step);
        if (string.IsNullOrEmpty(step.DisplayName))
          step.DisplayName = defaultName;
      }
      else
      {
        bool flag = NameValidation.IsValid(step.Name, context.BuildOptions.AllowHyphenNames);
        if (!flag)
        {
          if (context.BuildOptions.ValidateStepNames)
            return new PipelineValidationError(PipelineStrings.StepNameInvalid((object) phase.Name, (object) step.Name));
          string str = NameValidation.Sanitize(step.Name, context.BuildOptions.AllowHyphenNames);
          if (string.IsNullOrEmpty(str))
            str = defaultName;
          step.Name = str;
          flag = true;
        }
        if (flag && !stepValidationResult.KnownNames.Add(step.Name))
        {
          if (context.BuildOptions.ValidateStepNames)
            return new PipelineValidationError(PipelineStrings.StepNamesMustBeUnique((object) phase.Name, (object) step.Name));
          List<Step> stepList;
          if (!stepValidationResult.UnnamedSteps.TryGetValue(step.Name, out stepList))
          {
            stepList = new List<Step>();
            stepValidationResult.UnnamedSteps.Add(step.Name, stepList);
          }
          stepList.Add((Step) step);
        }
        if (string.IsNullOrEmpty(step.DisplayName))
          step.DisplayName = step.Name;
      }
      return (PipelineValidationError) null;
    }

    private static PipelineValidationError ValidateStepCondition(
      PipelineBuildContext context,
      PhaseNode phase,
      string stepName,
      string stepCondition)
    {
      if (!string.IsNullOrEmpty(stepCondition))
      {
        try
        {
          if (context.BuildOptions.ValidatePhaseExpressions)
          {
            PhaseCondition phaseCondition = new PhaseCondition(stepCondition);
          }
          new ExpressionParser().ValidateSyntax(stepCondition, (ITraceWriter) context.Trace);
        }
        catch (ParseException ex)
        {
          return new PipelineValidationError(PipelineStrings.StepConditionIsNotValid((object) phase.Name, (object) stepName, (object) stepCondition, (object) ex.Message));
        }
      }
      return (PipelineValidationError) null;
    }

    private static void GenerateDefaultTaskNames(
      ISet<string> knownNames,
      IDictionary<string, List<Step>> unnamedTasks)
    {
      foreach (KeyValuePair<string, List<Step>> unnamedTask in (IEnumerable<KeyValuePair<string, List<Step>>>) unnamedTasks)
      {
        if (unnamedTask.Value.Count == 1 && knownNames.Add(unnamedTask.Key))
        {
          unnamedTask.Value[0].Name = unnamedTask.Key;
        }
        else
        {
          int num = 1;
          foreach (Step step in unnamedTask.Value)
          {
            string str;
            for (str = string.Format("{0}{1}", (object) unnamedTask.Key, (object) num); !knownNames.Add(str); str = string.Format("{0}{1}", (object) unnamedTask.Key, (object) num))
              ++num;
            ++num;
            step.Name = str;
          }
        }
      }
    }

    private static IList<PipelineValidationError> ResolveInputs(
      PipelineBuildContext context,
      PhaseNode phase,
      PipelineResources referencedResources,
      PipelineResources unauthorizedResources,
      TaskStep step,
      TaskDefinition taskDefinition)
    {
      IList<PipelineValidationError> collection = (IList<PipelineValidationError>) new List<PipelineValidationError>();
      foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) taskDefinition.Inputs)
      {
        string inputAlias = Phase.ResolveAlias(context, step, input);
        string inputValue;
        if (step.Inputs.TryGetValue(input.Name, out inputValue))
        {
          collection.AddRange<PipelineValidationError, IList<PipelineValidationError>>(Phase.ValidateInput(context, phase, step, input, inputAlias, inputValue));
          collection.AddRange<PipelineValidationError, IList<PipelineValidationError>>(ResourceResolver.ResolveResources((IPipelineContext) context, phase.Name, context.BuildOptions, referencedResources, unauthorizedResources, step, input, inputAlias, inputValue));
        }
      }
      return collection;
    }

    private static string ResolveAlias(
      PipelineBuildContext context,
      TaskStep step,
      TaskInputDefinition input)
    {
      string str1 = input.Name;
      if (context.BuildOptions.ResolveTaskInputAliases && !step.Inputs.ContainsKey(input.Name))
      {
        foreach (string alias in (IEnumerable<string>) input.Aliases)
        {
          string str2;
          if (step.Inputs.TryGetValue(alias, out str2))
          {
            str1 = alias;
            step.Inputs.Remove(alias);
            step.Inputs.Add(input.Name, str2);
            break;
          }
        }
      }
      return str1;
    }

    private static IEnumerable<PipelineValidationError> ValidateInput(
      PipelineBuildContext context,
      PhaseNode phase,
      TaskStep step,
      TaskInputDefinition input,
      string inputAlias,
      string value)
    {
      if (!context.BuildOptions.ValidateTaskInputs || input.Validation == null)
        return Enumerable.Empty<PipelineValidationError>();
      string input1 = context.ExpandVariables(value, false);
      if (VariableUtility.IsVariable(input1))
        return Enumerable.Empty<PipelineValidationError>();
      InputValidationContext context1 = new InputValidationContext()
      {
        Evaluate = true,
        EvaluationOptions = context.ExpressionOptions,
        Expression = input.Validation.Expression,
        SecretMasker = context.SecretMasker,
        TraceWriter = (ITraceWriter) context.Trace,
        Value = input1
      };
      InputValidationResult validationResult = context.InputValidator.Validate(context1);
      if (validationResult.IsValid)
        return Enumerable.Empty<PipelineValidationError>();
      string str1 = context.SecretMasker.MaskSecrets(input1);
      string str2 = validationResult.Reason ?? input.Validation.Message;
      return (IEnumerable<PipelineValidationError>) new PipelineValidationError[1]
      {
        new PipelineValidationError(PipelineStrings.StepTaskInputInvalid((object) phase.Name, (object) step.Name, (object) inputAlias, (object) str1, (object) context1.Expression, (object) str2))
      };
    }

    internal static string GenerateDisplayName(Stage stage = null, PhaseNode phase = null, Job job = null)
    {
      string str1 = (string) null;
      if (stage != null)
        str1 = stage.DisplayName ?? stage.Name;
      string str2 = (string) null;
      if (phase != null)
        str2 = phase.DisplayName ?? phase.Name;
      string str3 = (string) null;
      if (job != null)
        str3 = job.DisplayName ?? job.Name;
      return Phase.GenerateDisplayName(str1, str2, str3);
    }

    internal static string GenerateDisplayName(PhaseNode factory, string configuration = null) => Phase.GenerateDisplayName(factory == null ? string.Empty : factory.DisplayName ?? factory.Name, configuration);

    internal static string GenerateDisplayName(params string[] tokens)
    {
      if (tokens == null)
        return string.Empty;
      string defaultNodeName = PipelineConstants.DefaultJobName;
      int l = defaultNodeName.Length;
      string str = string.Join(" ", ((IEnumerable<string>) tokens).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).Select<string, string>((Func<string, string>) (x => (x.StartsWith(defaultNodeName) ? x.Substring(l) : x).Trim())).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))));
      return !string.IsNullOrWhiteSpace(str) ? str : PipelineConstants.DefaultJobDisplayName;
    }

    public Job CreateJob(
      JobExecutionContext context,
      ExpressionValue<string> container,
      IDictionary<string, ExpressionValue<string>> sidecarContainers,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      string displayName = null)
    {
      if (string.IsNullOrWhiteSpace(displayName))
        displayName = Phase.GenerateDisplayName(context.Phase.Definition);
      Job job = new Job()
      {
        Id = context.GetInstanceId(),
        Name = context.Job.Name,
        DisplayName = displayName,
        ContinueOnError = continueOnError,
        TimeoutInMinutes = timeoutInMinutes,
        CancelTimeoutInMinutes = cancelTimeoutInMinutes
      };
      if (context.ExecutionOptions.EnableResourceExpressions)
        job.Target = (PhaseTarget) this.GenerateJobSpecificTarget(context);
      if (job.Target == null)
      {
        ArgumentUtility.CheckForNull<PhaseTarget>(this.Target, "Target");
        job.Target = this.Target.Clone();
      }
      int? hostedJobTimeout = context.ExecutionOptions.MaxHostedJobTimeout;
      if (hostedJobTimeout.HasValue && this.Target is AgentQueueTarget target)
      {
        hostedJobTimeout = context.ExecutionOptions.MaxHostedJobTimeout;
        int num1 = hostedJobTimeout.Value;
        TaskAgentQueue taskAgentQueue = context.ResourceStore.Queues?.Get(target.Queue);
        int num2;
        if (taskAgentQueue == null)
        {
          num2 = 0;
        }
        else
        {
          bool? isHosted = taskAgentQueue.Pool?.IsHosted;
          bool flag = true;
          num2 = isHosted.GetValueOrDefault() == flag & isHosted.HasValue ? 1 : 0;
        }
        if (num2 != 0)
        {
          if (job.TimeoutInMinutes > num1)
          {
            context.Trace?.AddWarning(PipelineStrings.TimeoutExceedsMMSMaximum((object) job.DisplayName, (object) job.TimeoutInMinutes, (object) num1));
            job.TimeoutInMinutes = num1;
          }
          if (job.CancelTimeoutInMinutes > num1)
          {
            context.Trace?.AddWarning(PipelineStrings.CancelTimeoutExceedsMMSMaximum((object) job.DisplayName, (object) job.TimeoutInMinutes, (object) num1));
            job.CancelTimeoutInMinutes = num1;
          }
        }
      }
      if (context.EnvironmentVersion > 1)
      {
        RepositoryResource repositoryResource1 = (RepositoryResource) null;
        RepositoryResource repositoryResource2 = context.ResourceStore?.Repositories.Get(PipelineConstants.SelfAlias);
        if (repositoryResource2 == null)
        {
          RepositoryResource repositoryResource3 = context.ResourceStore?.Repositories.Get(PipelineConstants.DesignerRepo);
          if (repositoryResource3 != null)
            repositoryResource1 = repositoryResource3;
        }
        else
          repositoryResource1 = repositoryResource2;
        if (repositoryResource1 != null)
        {
          context.ReferencedResources.Repositories.Add(repositoryResource1);
          if (repositoryResource1.Endpoint != null)
          {
            context.ReferencedResources.AddEndpointReference(repositoryResource1.Endpoint);
            IResourceStore resourceStore = context.ResourceStore;
            if ((resourceStore != null ? resourceStore.GetEndpoint(repositoryResource1.Endpoint) : (ServiceEndpoint) null) == null)
              throw new ResourceNotFoundException(PipelineStrings.ServiceEndpointNotFound((object) repositoryResource1.Endpoint));
          }
        }
      }
      if (container != (ExpressionValue<string>) null)
      {
        string inputAlias = container.GetValue((IPipelineContext) context).Value;
        string containerAlias = PhaseNode.ResolveContainerResource(context, inputAlias);
        job.Container = containerAlias;
        PhaseNode.UpdateJobContextReferencedContainers(context, containerAlias);
      }
      if (sidecarContainers != null)
      {
        foreach (KeyValuePair<string, ExpressionValue<string>> sidecarContainer in (IEnumerable<KeyValuePair<string, ExpressionValue<string>>>) sidecarContainers)
        {
          string inputAlias = sidecarContainer.Value.GetValue((IPipelineContext) context).Value;
          string containerAlias = PhaseNode.ResolveContainerResource(context, inputAlias);
          job.SidecarContainers.Add(sidecarContainer.Key, containerAlias);
          PhaseNode.UpdateJobContextReferencedContainers(context, containerAlias);
        }
      }
      if (context.Phase.Definition?.ExplicitResources != null)
      {
        ResourceReferences explicitResources = context.Phase.Definition.ExplicitResources;
        foreach (string repository in (IEnumerable<string>) explicitResources.Repositories)
          PhaseNode.UpdateJobContextReferencedRepositories(context, repository);
        foreach (string queue in (IEnumerable<string>) explicitResources.Queues)
          PhaseNode.UpdateJobContextReferencedQueues(context, queue);
      }
      this.UpdateJobContextVariablesFromJob(context, job);
      List<JobStep> jobStepList = new List<JobStep>();
      string instanceName = context.GetInstanceName();
      foreach (Step step in (IEnumerable<Step>) this.Steps)
      {
        if (step.Type == StepType.Task)
          jobStepList.Add((JobStep) Phase.CreateJobTaskStep(context, (PhaseNode) this, instanceName, step as TaskStep));
        else if (step.Type == StepType.Group)
          jobStepList.Add((JobStep) Phase.CreateJobStepGroup(context, (PhaseNode) this, instanceName, step as GroupStep));
      }
      context.Job.Definition = job;
      HashSet<Demand> demandSet1 = new HashSet<Demand>();
      List<TaskStep> taskStepList1 = new List<TaskStep>();
      if (context.StepProviders != null)
      {
        ReadOnlyCollection<JobStep> steps = new ReadOnlyCollection<JobStep>((IList<JobStep>) jobStepList);
        foreach (IStepProvider stepProvider in (IEnumerable<IStepProvider>) context.StepProviders)
          taskStepList1.AddRange((IEnumerable<TaskStep>) stepProvider.GetPreSteps((IPipelineContext) context, (IReadOnlyList<JobStep>) steps));
      }
      // ISSUE: explicit non-virtual call
      if (taskStepList1 != null && __nonvirtual (taskStepList1.Count) > 0)
      {
        for (int index = 0; index < taskStepList1.Count; ++index)
        {
          taskStepList1[index].Name = string.Format("__system_{0}", (object) (index + 1));
          IList<JobStep> resolvedSteps = (IList<JobStep>) new List<JobStep>();
          if (this.ResolveTaskStep(context, (PhaseNode) this, instanceName, (JobStep) taskStepList1[index], out resolvedSteps, demandSet1))
            job.Steps.AddRange<JobStep, IList<JobStep>>((IEnumerable<JobStep>) resolvedSteps);
          else
            job.Steps.Add((JobStep) Phase.CreateJobTaskStep(context, (PhaseNode) this, instanceName, taskStepList1[index], demandSet1));
        }
      }
      HashSet<Demand> demandSet2 = new HashSet<Demand>();
      foreach (JobStep step in jobStepList)
      {
        IList<JobStep> resolvedSteps;
        if (this.ResolveTaskStep(context, (PhaseNode) this, instanceName, step, out resolvedSteps, demandSet2))
          job.Steps.AddRange<JobStep, IList<JobStep>>((IEnumerable<JobStep>) resolvedSteps);
        else
          job.Steps.Add(step);
      }
      List<TaskStep> taskStepList2 = new List<TaskStep>();
      if (context.StepProviders != null)
      {
        ReadOnlyCollection<JobStep> steps = new ReadOnlyCollection<JobStep>(job.Steps);
        foreach (IStepProvider stepProvider in (IEnumerable<IStepProvider>) context.StepProviders)
          taskStepList2.AddRange((IEnumerable<TaskStep>) stepProvider.GetPostSteps((IPipelineContext) context, (IReadOnlyList<JobStep>) steps));
      }
      // ISSUE: explicit non-virtual call
      if (taskStepList2 != null && __nonvirtual (taskStepList2.Count) > 0)
      {
        for (int index = 0; index < taskStepList2.Count; ++index)
        {
          taskStepList2[index].Name = string.Format("__system_post_{0}", (object) (index + 1));
          job.Steps.Add((JobStep) Phase.CreateJobTaskStep(context, (PhaseNode) this, instanceName, taskStepList2[index], demandSet1));
        }
      }
      Dictionary<Guid, List<TaskStep>> stepsToInsert1 = this.GetStepsToInsert(context, jobStepList, Phase.TargetOfTasks.PostCheckoutTasks);
      if (stepsToInsert1 != null && stepsToInsert1.Keys.Count > 0)
      {
        foreach (KeyValuePair<Guid, List<TaskStep>> keyValuePair in stepsToInsert1)
        {
          int? nullable = new int?();
          for (int index = job.Steps.Count - 1; index >= 0; --index)
          {
            if (job.Steps[index] is TaskStep step && step.Reference.Id.Equals(keyValuePair.Key))
            {
              nullable = new int?(index);
              break;
            }
          }
          if (nullable.HasValue)
          {
            for (int index = 0; index < keyValuePair.Value.Count; ++index)
            {
              keyValuePair.Value[index].Name = string.Format("__system_postcheckout_{0}", (object) (index + 1));
              job.Steps.Insert(nullable.Value + index + 1, (JobStep) Phase.CreateJobTaskStep(context, (PhaseNode) this, instanceName, keyValuePair.Value[index], demandSet1));
            }
          }
        }
      }
      Dictionary<Guid, List<string>> targetTaskInputs = new Dictionary<Guid, List<string>>();
      if (this.inputsInjectionFeatureFlagEnabled)
      {
        targetTaskInputs = this.GetTaskInputsToRead(context);
        if (targetTaskInputs.Keys.Count > 0)
          demandSet1.Add((Demand) AgentFeatureDemands.DecoratorPickupTargetTaskInputsDemand());
      }
      Dictionary<Guid, List<TaskStep>> stepsToInsert2 = this.GetStepsToInsert(context, jobStepList, Phase.TargetOfTasks.PostTargetTask);
      try
      {
        this.InsertTasksRelativeToTarget(context, job, stepsToInsert2, false, instanceName, demandSet1, targetTaskInputs);
      }
      catch
      {
        context.Trace?.AddWarning(PipelineStrings.FailedInsertStepToPhase((object) "PostTargetTask"));
      }
      Dictionary<Guid, List<TaskStep>> stepsToInsert3 = this.GetStepsToInsert(context, jobStepList, Phase.TargetOfTasks.PreTargetTask);
      try
      {
        this.InsertTasksRelativeToTarget(context, job, stepsToInsert3, true, instanceName, demandSet1, targetTaskInputs);
      }
      catch
      {
        context.Trace?.AddWarning(PipelineStrings.FailedInsertStepToPhase((object) "PreTargetTask"));
      }
      this.AddDemands(context, job, (ISet<Demand>) demandSet1);
      this.AddDemands(context, job, (ISet<Demand>) demandSet2);
      this.AddDemands(context, job, this.Target?.Demands);
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) context.Variables)
        context.Job.Definition.Variables.Add((IVariable) new Variable()
        {
          Name = variable.Key,
          Value = (variable.Value.IsSecret ? (string) null : variable.Value.Value),
          Secret = variable.Value.IsSecret,
          Readonly = variable.Value.IsReadOnly
        });
      return job;
    }

    private Dictionary<Guid, List<TaskStep>> GetStepsToInsert(
      JobExecutionContext context,
      List<JobStep> steps,
      Phase.TargetOfTasks target)
    {
      Dictionary<Guid, List<TaskStep>> stepsToInsert = new Dictionary<Guid, List<TaskStep>>();
      if (context.StepProviders != null)
      {
        ReadOnlyCollection<JobStep> steps1 = new ReadOnlyCollection<JobStep>((IList<JobStep>) steps);
        foreach (IStepProvider stepProvider in (IEnumerable<IStepProvider>) context.StepProviders)
        {
          Dictionary<Guid, List<TaskStep>> dictionary = (Dictionary<Guid, List<TaskStep>>) null;
          switch (target)
          {
            case Phase.TargetOfTasks.PostCheckoutTasks:
              dictionary = stepProvider.GetPostTaskSteps((IPipelineContext) context, (IReadOnlyList<JobStep>) steps1);
              break;
            case Phase.TargetOfTasks.PostTargetTask:
              dictionary = stepProvider.GetPostTargetTaskSteps((IPipelineContext) context, (IReadOnlyList<JobStep>) steps1);
              break;
            case Phase.TargetOfTasks.PreTargetTask:
              dictionary = stepProvider.GetPreTargetTaskSteps((IPipelineContext) context, (IReadOnlyList<JobStep>) steps1);
              break;
          }
          if (dictionary != null)
          {
            foreach (Guid key in dictionary.Keys)
            {
              if (!stepsToInsert.ContainsKey(key))
                stepsToInsert[key] = new List<TaskStep>();
              stepsToInsert[key].AddRange((IEnumerable<TaskStep>) dictionary[key]);
            }
          }
        }
      }
      return stepsToInsert;
    }

    private void InsertTasksRelativeToTarget(
      JobExecutionContext context,
      Job job,
      Dictionary<Guid, List<TaskStep>> stepsToAdd,
      bool beforeTargetTask,
      string identifier,
      HashSet<Demand> stepProviderDemands,
      Dictionary<Guid, List<string>> targetTaskInputs)
    {
      LinkedList<JobStep> values = new LinkedList<JobStep>((IEnumerable<JobStep>) job.Steps);
      LinkedListNode<JobStep> node = values.First;
      int num = 0;
      for (; node != null; node = node.Next)
      {
        if (node.Value is TaskStep taskStep1 && taskStep1.Reference != null)
        {
          Guid id = taskStep1.Reference.Id;
          List<TaskStep> taskStepList;
          if (id != Guid.Empty && stepsToAdd.TryGetValue(id, out taskStepList))
          {
            TaskStep targetTask = taskStep1;
            foreach (Step step in taskStepList)
            {
              TaskStep taskStep = step.Clone() as TaskStep;
              if (beforeTargetTask)
              {
                taskStep.Name = string.Format("__system_pretargettask_{0}", (object) num);
                List<string> targetInputs;
                if (this.inputsInjectionFeatureFlagEnabled && targetTaskInputs.TryGetValue(id, out targetInputs))
                  this.InsertTargetTaskInputs(context, targetTask, taskStep, targetInputs);
                values.AddBefore(node, (JobStep) Phase.CreateJobTaskStep(context, (PhaseNode) this, identifier, taskStep, stepProviderDemands));
              }
              else
              {
                taskStep.Name = string.Format("__system_posttargettask_{0}", (object) num);
                List<string> targetInputs;
                if (this.inputsInjectionFeatureFlagEnabled && targetTaskInputs.TryGetValue(id, out targetInputs))
                  this.InsertTargetTaskInputs(context, targetTask, taskStep, targetInputs);
                values.AddAfter(node, (JobStep) Phase.CreateJobTaskStep(context, (PhaseNode) this, identifier, taskStep, stepProviderDemands));
                node = node.Next;
              }
              ++num;
            }
          }
        }
      }
      job.Steps.Clear();
      job.Steps.AddRange<JobStep, IList<JobStep>>((IEnumerable<JobStep>) values);
    }

    private void InsertTargetTaskInputs(
      JobExecutionContext context,
      TaskStep targetTask,
      TaskStep injectedTask,
      List<string> targetInputs)
    {
      foreach (string targetInput in targetInputs)
      {
        string empty = string.Empty;
        if (targetTask.Inputs.TryGetValue(targetInput, out empty))
        {
          string key = "target_" + targetInput;
          if (injectedTask.Inputs.ContainsKey(key))
            injectedTask.Inputs[key] = empty;
          else
            injectedTask.Inputs.Add(key, empty);
        }
      }
    }

    private Dictionary<Guid, List<string>> GetTaskInputsToRead(JobExecutionContext context)
    {
      Dictionary<Guid, List<string>> taskInputsToRead = new Dictionary<Guid, List<string>>();
      if (context.StepProviders != null)
      {
        foreach (IStepProvider stepProvider in (IEnumerable<IStepProvider>) context.StepProviders)
        {
          Dictionary<Guid, List<string>> inputsToProvide = stepProvider.GetInputsToProvide((IPipelineContext) context);
          if (inputsToProvide != null)
          {
            foreach (Guid key in inputsToProvide.Keys)
            {
              if (!taskInputsToRead.ContainsKey(key))
                taskInputsToRead[key] = new List<string>();
              taskInputsToRead[key].AddRange((IEnumerable<string>) inputsToProvide[key]);
            }
          }
        }
      }
      return taskInputsToRead;
    }

    private void AddDemands(JobExecutionContext context, Job job, ISet<Demand> demands)
    {
      if (context == null || job == null || demands == null)
        return;
      IList<Demand> demands1 = job.Demands;
      foreach (Demand demand1 in (IEnumerable<Demand>) demands)
      {
        if (demand1 != null)
        {
          string str1 = demand1.Value;
          if (string.IsNullOrEmpty(str1))
          {
            demands1.Add(demand1.Clone());
          }
          else
          {
            string str2 = context.ExpandVariables(str1, true);
            try
            {
              Demand demand2 = demand1.Clone();
              demand2.Update(str2);
              demands1.Add(demand2);
            }
            catch (Exception ex)
            {
              throw new PipelineValidationException(PipelineStrings.DemandExpansionInvalid((object) demand1.ToString(), (object) demand1.Value, (object) str2), ex);
            }
          }
        }
      }
    }

    private bool ResolveTaskStep(
      JobExecutionContext context,
      PhaseNode phase,
      string identifier,
      JobStep step,
      out IList<JobStep> resolvedSteps,
      HashSet<Demand> resolvedDemands = null)
    {
      bool flag = false;
      IList<TaskStep> resolvedSteps1 = (IList<TaskStep>) new List<TaskStep>();
      resolvedSteps = (IList<JobStep>) new List<JobStep>();
      IResourceStore resourceStore = context.ResourceStore;
      if ((resourceStore != null ? (resourceStore.ResolveStep((IPipelineContext) context, step, out resolvedSteps1) ? 1 : 0) : 0) != 0)
      {
        foreach (TaskStep task in (IEnumerable<TaskStep>) resolvedSteps1)
          resolvedSteps.Add((JobStep) Phase.CreateJobTaskStep(context, phase, identifier, task, resolvedDemands));
        flag = true;
      }
      return flag;
    }

    private AgentQueueTarget GenerateJobSpecificTarget(JobExecutionContext context)
    {
      if (!(context?.Phase?.Definition is Phase definition))
        return (AgentQueueTarget) null;
      if (!(definition.Target is AgentQueueTarget target))
        return (AgentQueueTarget) null;
      if (target.IsLiteral())
        return (AgentQueueTarget) null;
      ValidationResult result1 = new ValidationResult();
      AgentQueueTarget jobSpecificTarget = target.Evaluate(context, result1);
      if (jobSpecificTarget != null)
      {
        JobExecutionContext context1 = context;
        ValidationResult validationResult = result1;
        BuildOptions buildOptions = new BuildOptions();
        buildOptions.EnableResourceExpressions = true;
        buildOptions.ValidateResources = true;
        buildOptions.ValidateExpressions = true;
        buildOptions.AllowEmptyQueueTarget = false;
        ValidationResult result2 = validationResult;
        List<Step> steps = new List<Step>();
        HashSet<Demand> taskDemands = new HashSet<Demand>();
        jobSpecificTarget.Validate((IPipelineContext) context1, buildOptions, result2, (IList<Step>) steps, (ISet<Demand>) taskDemands);
      }
      if (result1.Errors.Count <= 0)
        return jobSpecificTarget;
      throw new PipelineValidationException((IEnumerable<PipelineValidationError>) result1.Errors);
    }

    private static TaskStep CreateJobTaskStep(
      JobExecutionContext context,
      PhaseNode phase,
      string jobIdentifier,
      TaskStep task,
      HashSet<Demand> resolvedDemands = null)
    {
      TaskStep step = (TaskStep) task.Clone();
      string instanceName = context.IdGenerator.GetInstanceName(jobIdentifier, task.Name);
      step.Id = context.IdGenerator.GetInstanceId(instanceName);
      step.DisplayName = context.ExpandVariables(step.DisplayName, true);
      TaskDefinition taskDefinition = context.TaskStore.ResolveTask(step.Reference.Id, step.Reference.Version);
      if (taskDefinition == null)
        throw new TaskDefinitionNotFoundException(PipelineStrings.TaskMissing((object) phase.Name, (object) step.Name, (object) step.Reference.Id, (object) step.Reference.Version));
      foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) taskDefinition.Inputs)
      {
        string inputValue;
        if (task.Inputs.TryGetValue(input.Name, out inputValue))
          ResourceResolver.ResolveResources((IPipelineContext) context, phase.Name, BuildOptions.None, context.ReferencedResources, (PipelineResources) null, step, input, input.Name, inputValue, true);
      }
      if (step.Target?.Target != null && step.Target.Target != PipelineConstants.StepContainerConstants.Host)
      {
        resolvedDemands?.Add((Demand) AgentFeatureDemands.StepTargetVersionDemand());
        PhaseNode.UpdateJobContextReferencedContainers(context, step.Target.Target);
      }
      if (resolvedDemands != null)
      {
        resolvedDemands.AddRange<Demand, HashSet<Demand>>((IEnumerable<Demand>) taskDefinition.Demands);
        bool newMinimum;
        string minimumAgentVersion = taskDefinition.GetMinimumAgentVersion((string) null, out newMinimum);
        if (newMinimum)
          resolvedDemands.Add((Demand) new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, minimumAgentVersion, new DemandSource()
          {
            SourceName = taskDefinition.Name,
            SourceVersion = (string) taskDefinition.Version,
            SourceType = DemandSourceType.Task
          }));
      }
      return step;
    }

    private static GroupStep CreateJobStepGroup(
      JobExecutionContext context,
      PhaseNode phase,
      string jobIdentifier,
      GroupStep group)
    {
      GroupStep jobStepGroup = (GroupStep) group.Clone();
      string instanceName = context.IdGenerator.GetInstanceName(jobIdentifier, group.Name);
      jobStepGroup.Id = context.IdGenerator.GetInstanceId(instanceName);
      jobStepGroup.DisplayName = context.ExpandVariables(jobStepGroup.DisplayName, true);
      List<TaskStep> values = new List<TaskStep>();
      foreach (TaskStep step in (IEnumerable<TaskStep>) jobStepGroup.Steps)
        values.Add(Phase.CreateJobTaskStep(context, phase, instanceName, step));
      jobStepGroup.Steps.Clear();
      jobStepGroup.Steps.AddRange<TaskStep, IList<TaskStep>>((IEnumerable<TaskStep>) values);
      return jobStepGroup;
    }

    private static HashSet<int> GetNodeVersionsForTask(TaskDefinition task)
    {
      HashSet<int> nodeVersionsForTask = new HashSet<int>();
      foreach (IDictionary<string, JObject> dictionary in new List<IDictionary<string, JObject>>()
      {
        task.Execution,
        task.PreJobExecution,
        task.PostJobExecution
      })
      {
        IDictionary<string, JObject> exec = dictionary;
        if (exec != null && Phase.NodeVersionToAgentVersion.Any<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (x => exec.ContainsKey(x.Key))))
        {
          foreach (KeyValuePair<string, JObject> keyValuePair in (IEnumerable<KeyValuePair<string, JObject>>) exec)
          {
            if (Phase.NodeVersionToAgentVersion.ContainsKey(keyValuePair.Key))
              nodeVersionsForTask.Add(Phase.NodeVersionToAgentVersion.GetValueOrDefault<string, int>(keyValuePair.Key, 0));
          }
        }
      }
      return nodeVersionsForTask;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IList<Step> steps = this.m_steps;
      if ((steps != null ? (steps.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_steps = (IList<Step>) null;
    }

    private enum TargetOfTasks
    {
      PostCheckoutTasks,
      PostTargetTask,
      PreTargetTask,
    }

    public class StepValidationResult
    {
      private HashSet<Demand> m_taskDemands;
      private HashSet<string> m_knownNames;
      private HashSet<string> m_tasksSatisfy;
      private Dictionary<string, List<Step>> m_unnamedSteps;

      public string MinAgentVersion { get; set; }

      public DemandSource MinAgentVersionSource { get; set; }

      public HashSet<Demand> TaskDemands
      {
        get
        {
          if (this.m_taskDemands == null)
            this.m_taskDemands = new HashSet<Demand>();
          return this.m_taskDemands;
        }
      }

      public HashSet<string> KnownNames
      {
        get
        {
          if (this.m_knownNames == null)
            this.m_knownNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          return this.m_knownNames;
        }
      }

      public HashSet<string> TasksSatisfy
      {
        get
        {
          if (this.m_tasksSatisfy == null)
            this.m_tasksSatisfy = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          return this.m_tasksSatisfy;
        }
      }

      public Dictionary<string, List<Step>> UnnamedSteps
      {
        get
        {
          if (this.m_unnamedSteps == null)
            this.m_unnamedSteps = new Dictionary<string, List<Step>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          return this.m_unnamedSteps;
        }
      }
    }
  }
}
