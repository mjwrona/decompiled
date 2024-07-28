// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.ContributionPipelineDecorator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  public abstract class ContributionPipelineDecorator : IPipelineDecorator, IStepProvider
  {
    protected readonly IVssRequestContext m_requestContext;
    private readonly string m_pipelineDecoratorContributionType = "ms.azure-pipelines.pipeline-decorator";
    private readonly string m_layer = nameof (ContributionPipelineDecorator);

    public ContributionPipelineDecorator(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public IList<TaskStep> GetPreSteps(IPipelineContext context, IReadOnlyList<JobStep> steps)
    {
      List<ContributionPipelineDecorator.PipelineDecorator> list = this.GetPipelineDecoratorResources(context).Where<ContributionPipelineDecorator.PipelineDecorator>((Func<ContributionPipelineDecorator.PipelineDecorator, bool>) (decorator => decorator.PipelineDecoratorType == ContributionPipelineDecorator.PipelineDecoratorType.PreJobTask)).ToList<ContributionPipelineDecorator.PipelineDecorator>();
      return this.GetStepsFromDecorators(context, (IList<ContributionPipelineDecorator.PipelineDecorator>) list, steps);
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      List<ContributionPipelineDecorator.PipelineDecorator> list = this.GetPipelineDecoratorResources(context).Where<ContributionPipelineDecorator.PipelineDecorator>((Func<ContributionPipelineDecorator.PipelineDecorator, bool>) (decorator => decorator.PipelineDecoratorType == ContributionPipelineDecorator.PipelineDecoratorType.PostCheckoutTask)).ToList<ContributionPipelineDecorator.PipelineDecorator>();
      return this.GetTaskStepsFromDecorators(context, (IList<ContributionPipelineDecorator.PipelineDecorator>) list, steps);
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      List<ContributionPipelineDecorator.PipelineDecorator> list = this.GetPipelineDecoratorResources(context).Where<ContributionPipelineDecorator.PipelineDecorator>((Func<ContributionPipelineDecorator.PipelineDecorator, bool>) (decorator => decorator.PipelineDecoratorType == ContributionPipelineDecorator.PipelineDecoratorType.PostTargetTask)).ToList<ContributionPipelineDecorator.PipelineDecorator>();
      return this.GetTaskStepsFromDecorators(context, (IList<ContributionPipelineDecorator.PipelineDecorator>) list, steps);
    }

    public Dictionary<Guid, List<TaskStep>> GetPreTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      List<ContributionPipelineDecorator.PipelineDecorator> list = this.GetPipelineDecoratorResources(context).Where<ContributionPipelineDecorator.PipelineDecorator>((Func<ContributionPipelineDecorator.PipelineDecorator, bool>) (decorator => decorator.PipelineDecoratorType == ContributionPipelineDecorator.PipelineDecoratorType.PreTargetTask)).ToList<ContributionPipelineDecorator.PipelineDecorator>();
      return this.GetTaskStepsFromDecorators(context, (IList<ContributionPipelineDecorator.PipelineDecorator>) list, steps);
    }

    public IList<TaskStep> GetPostSteps(IPipelineContext context, IReadOnlyList<JobStep> steps)
    {
      List<ContributionPipelineDecorator.PipelineDecorator> list = this.GetPipelineDecoratorResources(context).Where<ContributionPipelineDecorator.PipelineDecorator>((Func<ContributionPipelineDecorator.PipelineDecorator, bool>) (decorator => decorator.PipelineDecoratorType == ContributionPipelineDecorator.PipelineDecoratorType.PostJobTask)).ToList<ContributionPipelineDecorator.PipelineDecorator>();
      return this.GetStepsFromDecorators(context, (IList<ContributionPipelineDecorator.PipelineDecorator>) list, steps);
    }

    public Dictionary<Guid, List<string>> GetInputsToProvide(IPipelineContext context)
    {
      List<ContributionPipelineDecorator.PipelineDecorator> list = this.GetPipelineDecoratorResources(context).Where<ContributionPipelineDecorator.PipelineDecorator>((Func<ContributionPipelineDecorator.PipelineDecorator, bool>) (decorator => decorator.PipelineDecoratorType == ContributionPipelineDecorator.PipelineDecoratorType.PreTargetTask)).ToList<ContributionPipelineDecorator.PipelineDecorator>();
      list.AddRange((IEnumerable<ContributionPipelineDecorator.PipelineDecorator>) this.GetPipelineDecoratorResources(context).Where<ContributionPipelineDecorator.PipelineDecorator>((Func<ContributionPipelineDecorator.PipelineDecorator, bool>) (decorator => decorator.PipelineDecoratorType == ContributionPipelineDecorator.PipelineDecoratorType.PostTargetTask)).ToList<ContributionPipelineDecorator.PipelineDecorator>());
      return this.GetInputsToReadFromTargetTasks((IList<ContributionPipelineDecorator.PipelineDecorator>) list);
    }

    public bool ResolveStep(
      IPipelineContext context,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      resolvedSteps = (IList<TaskStep>) new List<TaskStep>();
      return false;
    }

    public abstract string PreJobTaskTarget { get; }

    public abstract string PostJobTaskTarget { get; }

    public abstract string PostCheckoutTaskTarget { get; }

    public abstract string PreTaskTarget { get; }

    public abstract string PostTaskTarget { get; }

    private IList<TaskStep> GetStepsFromDecorators(
      IPipelineContext context,
      IList<ContributionPipelineDecorator.PipelineDecorator> decorators,
      IReadOnlyList<JobStep> steps)
    {
      List<TaskStep> stepsFromDecorators = new List<TaskStep>();
      foreach (ContributionPipelineDecorator.PipelineDecorator decorator in (IEnumerable<ContributionPipelineDecorator.PipelineDecorator>) decorators)
        stepsFromDecorators.AddRange((IEnumerable<TaskStep>) this.GetStepsFromYamlFile(context, decorator, steps));
      return (IList<TaskStep>) stepsFromDecorators;
    }

    private Dictionary<Guid, List<TaskStep>> GetTaskStepsFromDecorators(
      IPipelineContext context,
      IList<ContributionPipelineDecorator.PipelineDecorator> decorators,
      IReadOnlyList<JobStep> steps)
    {
      Dictionary<Guid, List<TaskStep>> stepsFromDecorators = new Dictionary<Guid, List<TaskStep>>();
      foreach (ContributionPipelineDecorator.PipelineDecorator decorator in (IEnumerable<ContributionPipelineDecorator.PipelineDecorator>) decorators)
      {
        IList<TaskStep> stepsFromYamlFile = this.GetStepsFromYamlFile(context, decorator, steps);
        Dictionary<Guid, List<TaskStep>> dictionary1 = stepsFromDecorators;
        Guid? taskId = decorator.TaskId;
        Guid key1 = taskId.Value;
        if (!dictionary1.ContainsKey(key1))
        {
          Dictionary<Guid, List<TaskStep>> dictionary2 = stepsFromDecorators;
          taskId = decorator.TaskId;
          Guid key2 = taskId.Value;
          List<TaskStep> taskStepList = new List<TaskStep>();
          dictionary2[key2] = taskStepList;
        }
        Dictionary<Guid, List<TaskStep>> dictionary3 = stepsFromDecorators;
        taskId = decorator.TaskId;
        Guid key3 = taskId.Value;
        dictionary3[key3].AddRange((IEnumerable<TaskStep>) stepsFromYamlFile);
      }
      return stepsFromDecorators;
    }

    private Dictionary<Guid, List<string>> GetInputsToReadFromTargetTasks(
      IList<ContributionPipelineDecorator.PipelineDecorator> decorators)
    {
      Dictionary<Guid, List<string>> readFromTargetTasks = new Dictionary<Guid, List<string>>();
      foreach (ContributionPipelineDecorator.PipelineDecorator decorator in (IEnumerable<ContributionPipelineDecorator.PipelineDecorator>) decorators)
      {
        Dictionary<Guid, List<string>> dictionary1 = readFromTargetTasks;
        Guid? taskId = decorator.TaskId;
        Guid key1 = taskId.Value;
        if (!dictionary1.ContainsKey(key1))
        {
          Dictionary<Guid, List<string>> dictionary2 = readFromTargetTasks;
          taskId = decorator.TaskId;
          Guid key2 = taskId.Value;
          List<string> stringList = new List<string>();
          dictionary2[key2] = stringList;
        }
        if (decorator.TargetTaskInputs != null)
        {
          Dictionary<Guid, List<string>> dictionary3 = readFromTargetTasks;
          taskId = decorator.TaskId;
          Guid key3 = taskId.Value;
          dictionary3[key3].AddRange((IEnumerable<string>) decorator.TargetTaskInputs);
        }
      }
      return readFromTargetTasks;
    }

    private IList<TaskStep> GetStepsFromYamlFile(
      IPipelineContext context,
      ContributionPipelineDecorator.PipelineDecorator decorator,
      IReadOnlyList<JobStep> steps)
    {
      PipelineStepsTemplate steps1 = this.GetSteps(context, decorator, steps);
      if (steps1 == null)
      {
        this.m_requestContext.TraceError(10016108, this.m_layer, "Steps template was null.");
        return (IList<TaskStep>) Array.Empty<TaskStep>();
      }
      if (steps1.Errors.Any<PipelineValidationError>())
      {
        for (int index = 0; index < steps1.Errors.Count; ++index)
          this.m_requestContext.TraceError(10016108, this.m_layer, "Step template error {0} of {1}: {2}.", (object) (index + 1), (object) steps1.Errors.Count, (object) steps1.Errors[index].Message);
        return (IList<TaskStep>) Array.Empty<TaskStep>();
      }
      List<TaskStep> stepsFromYamlFile = new List<TaskStep>();
      foreach (Step step in (IEnumerable<Step>) steps1.Steps)
      {
        if (step.Type == StepType.Task)
          stepsFromYamlFile.Add(step as TaskStep);
        else
          this.m_requestContext.TraceError(10016108, this.m_layer, "Task of type {0} not supported.", (object) step.Type);
      }
      return (IList<TaskStep>) stepsFromYamlFile;
    }

    private PipelineStepsTemplate GetSteps(
      IPipelineContext context,
      ContributionPipelineDecorator.PipelineDecorator decorator,
      IReadOnlyList<JobStep> steps)
    {
      YamlPipelineLoaderService.PipelineTraceWriter trace = new YamlPipelineLoaderService.PipelineTraceWriter(this.m_requestContext);
      ContributionFileProviderFactory fileProviderFactory = new ContributionFileProviderFactory(this.m_requestContext, decorator.ContributionId);
      ParseOptions parseOptions = ParseOptionsFactory.CreateParseOptions(this.m_requestContext, RetrieveOptions.Default);
      PipelineParser pipelineParser = new PipelineParser((ITraceWriter) trace, parseOptions, (IFileProviderFactory) fileProviderFactory);
      try
      {
        PipelineStepsTemplate steps1 = pipelineParser.LoadStepsContribution(decorator.YamlFilePath, context.Variables, context.ResourceStore, steps, this.m_requestContext.CancellationToken);
        IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
        bool flag = this.m_requestContext.IsFeatureEnabled("DistributedTask.DisableYamlDemandsLatestAgent");
        BuildOptions options = new BuildOptions()
        {
          DemandLatestAgent = !flag,
          MinimumAgentVersion = flag ? service.GetValue<string>(this.m_requestContext, (RegistryQuery) "/Service/DistributedTask/Pipelines/MinAgentVersion", true, "2.163.1") : (string) null,
          MinimumAgentVersionDemandSource = flag ? AgentFeatureDemands.YamlPipelinesDemandSource() : (DemandSource) null,
          ResolveTaskInputAliases = true,
          ValidateResources = true,
          ValidateStepNames = true,
          AllowEmptyQueueTarget = true
        };
        steps1.Errors.AddRange<PipelineValidationError, IList<PipelineValidationError>>((IEnumerable<PipelineValidationError>) context.Validate(steps1.Steps, (PhaseTarget) null, options));
        if (context.Trace != null)
        {
          context.Trace.Info("########## " + decorator.ContributionId + " ##########\n");
          context.Trace.Info(trace.Log());
        }
        return steps1;
      }
      catch (Exception ex)
      {
        PipelineStepsTemplate steps2 = new PipelineStepsTemplate();
        steps2.Errors.Add(new PipelineValidationError(ex.ToString()));
        this.m_requestContext.TraceException(10016108, this.m_layer, ex);
        return steps2;
      }
    }

    private IList<ContributionPipelineDecorator.PipelineDecorator> GetPipelineDecoratorResources(
      IPipelineContext context)
    {
      List<ContributionPipelineDecorator.PipelineDecorator> decoratorResources = new List<ContributionPipelineDecorator.PipelineDecorator>();
      if (context is JobExecutionContext executionContext && executionContext.Job.Definition != null && executionContext.Job.Definition.Target != null && executionContext.Job.Definition.Target is ServerTarget)
        return (IList<ContributionPipelineDecorator.PipelineDecorator>) decoratorResources;
      if (string.IsNullOrWhiteSpace(this.PreJobTaskTarget) && string.IsNullOrWhiteSpace(this.PostJobTaskTarget) && string.IsNullOrWhiteSpace(this.PostCheckoutTaskTarget))
        return (IList<ContributionPipelineDecorator.PipelineDecorator>) decoratorResources;
      try
      {
        IEnumerable<Contribution> contributions = this.m_requestContext.GetService<IContributionService>().QueryContributionsForType(this.m_requestContext, this.m_pipelineDecoratorContributionType);
        if (contributions == null)
          return (IList<ContributionPipelineDecorator.PipelineDecorator>) decoratorResources;
        foreach (Contribution contribution in contributions)
        {
          foreach (string target in contribution.Targets)
          {
            if (string.Equals(target, this.PreJobTaskTarget, StringComparison.OrdinalIgnoreCase))
              decoratorResources.Add(new ContributionPipelineDecorator.PipelineDecorator()
              {
                ContributionId = contribution.Id,
                PipelineDecoratorType = ContributionPipelineDecorator.PipelineDecoratorType.PreJobTask,
                YamlFilePath = this.GetYamlFilePath(contribution)
              });
            else if (string.Equals(target, this.PostJobTaskTarget, StringComparison.OrdinalIgnoreCase))
              decoratorResources.Add(new ContributionPipelineDecorator.PipelineDecorator()
              {
                ContributionId = contribution.Id,
                PipelineDecoratorType = ContributionPipelineDecorator.PipelineDecoratorType.PostJobTask,
                YamlFilePath = this.GetYamlFilePath(contribution)
              });
            else if (string.Equals(target, this.PostCheckoutTaskTarget, StringComparison.OrdinalIgnoreCase))
              decoratorResources.Add(new ContributionPipelineDecorator.PipelineDecorator()
              {
                ContributionId = contribution.Id,
                PipelineDecoratorType = ContributionPipelineDecorator.PipelineDecoratorType.PostCheckoutTask,
                YamlFilePath = this.GetYamlFilePath(contribution),
                TaskId = new Guid?(PipelineConstants.CheckoutTask.Id)
              });
            else if (string.Equals(target, this.PreTaskTarget, StringComparison.OrdinalIgnoreCase))
              decoratorResources.Add(new ContributionPipelineDecorator.PipelineDecorator()
              {
                ContributionId = contribution.Id,
                PipelineDecoratorType = ContributionPipelineDecorator.PipelineDecoratorType.PreTargetTask,
                YamlFilePath = this.GetYamlFilePath(contribution),
                TaskId = new Guid?(this.GetTargetTaskGuid(contribution)),
                TargetTaskInputs = this.GetListOfProvidedInputs(contribution)
              });
            else if (string.Equals(target, this.PostTaskTarget, StringComparison.OrdinalIgnoreCase))
              decoratorResources.Add(new ContributionPipelineDecorator.PipelineDecorator()
              {
                ContributionId = contribution.Id,
                PipelineDecoratorType = ContributionPipelineDecorator.PipelineDecoratorType.PostTargetTask,
                YamlFilePath = this.GetYamlFilePath(contribution),
                TaskId = new Guid?(this.GetTargetTaskGuid(contribution)),
                TargetTaskInputs = this.GetListOfProvidedInputs(contribution)
              });
          }
        }
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(10016108, this.m_layer, ex);
        return (IList<ContributionPipelineDecorator.PipelineDecorator>) Array.Empty<ContributionPipelineDecorator.PipelineDecorator>();
      }
      return (IList<ContributionPipelineDecorator.PipelineDecorator>) decoratorResources;
    }

    private string GetYamlFilePath(Contribution contribution)
    {
      try
      {
        return contribution.Properties.ToObject<ContributionPipelineDecorator.PipelineDecoratorContributionProperties>().Template;
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(10016108, this.m_layer, ex);
        return string.Empty;
      }
    }

    private Guid GetTargetTaskGuid(Contribution contribution)
    {
      try
      {
        return new Guid(contribution.Properties.ToObject<ContributionPipelineDecorator.PipelineDecoratorContributionProperties>().TargetTask);
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(10016108, this.m_layer, ex);
        return Guid.Empty;
      }
    }

    private string[] GetListOfProvidedInputs(Contribution contribution)
    {
      try
      {
        return contribution.Properties.ToObject<ContributionPipelineDecorator.PipelineDecoratorContributionProperties>().TargetTaskInputs;
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(10016108, this.m_layer, ex);
        return Array.Empty<string>();
      }
    }

    private static class PipelineDecoratorType
    {
      public static readonly string PreJobTask = "pre-job-tasks";
      public static readonly string PostCheckoutTask = "post-checkout-tasks";
      public static readonly string PostJobTask = "post-job-tasks";
      public static readonly string PostTargetTask = "post-task-tasks";
      public static readonly string PreTargetTask = "pre-task-tasks";
    }

    private class PipelineDecorator
    {
      public string ContributionId { get; set; }

      public string PipelineDecoratorType { get; set; }

      public string YamlFilePath { get; set; }

      public Guid? TaskId { get; set; }

      public string[] TargetTaskInputs { get; set; }
    }

    private class PipelineDecoratorContributionProperties
    {
      public string Template { get; set; }

      public string TargetTask { get; set; }

      public string[] TargetTaskInputs { get; set; }
    }
  }
}
