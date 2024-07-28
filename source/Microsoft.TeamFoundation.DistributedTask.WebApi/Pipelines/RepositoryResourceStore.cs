// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResourceStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public class RepositoryResourceStore : 
    InMemoryResourceStore<RepositoryResource>,
    IRepositoryStore,
    IStepProvider
  {
    private bool m_useSystemStepsDecorator;
    private bool m_includeCheckoutOptions;

    public RepositoryResourceStore(IEnumerable<RepositoryResource> repositories)
      : this(repositories, false, false)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public RepositoryResourceStore(
      IEnumerable<RepositoryResource> repositories,
      bool useSystemStepsDecorator,
      bool includeCheckoutOptions)
      : base(repositories)
    {
      this.m_useSystemStepsDecorator = useSystemStepsDecorator;
      this.m_includeCheckoutOptions = includeCheckoutOptions;
    }

    public IList<TaskStep> GetPreSteps(IPipelineContext context, IReadOnlyList<JobStep> steps)
    {
      if (context.EnvironmentVersion < 2)
        return (IList<TaskStep>) null;
      if (context is JobExecutionContext executionContext && (executionContext.Phase.Definition is Phase definition ? (definition.Target.Type != 0 ? 1 : 0) : 1) != 0)
        return (IList<TaskStep>) null;
      if (!this.m_includeCheckoutOptions)
      {
        foreach (TaskStep checkoutTask in steps.Where<JobStep>((Func<JobStep, bool>) (x => x.IsCheckoutTask())).OfType<TaskStep>())
        {
          RepositoryResource repositoryResource = this.Get(checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Repository]);
          CheckoutOptions checkoutOptions;
          if (repositoryResource != null && repositoryResource.Properties.TryGetValue<CheckoutOptions>(RepositoryPropertyNames.CheckoutOptions, out checkoutOptions))
            this.MergeCheckoutOptions(checkoutOptions, checkoutTask);
        }
      }
      if (this.m_useSystemStepsDecorator)
        return (IList<TaskStep>) null;
      RepositoryResource repositoryResource1 = this.Get(PipelineConstants.SelfAlias);
      if (repositoryResource1 == null)
        return (IList<TaskStep>) null;
      if (steps.Any<JobStep>((Func<JobStep, bool>) (x => x.IsCheckoutTask())))
        return (IList<TaskStep>) null;
      TaskStep taskStep = new TaskStep();
      taskStep.Enabled = true;
      taskStep.DisplayName = PipelineConstants.CheckoutTask.FriendlyName;
      taskStep.Reference = new TaskStepDefinitionReference()
      {
        Id = PipelineConstants.CheckoutTask.Id,
        Version = (string) PipelineConstants.CheckoutTask.Version,
        Name = PipelineConstants.CheckoutTask.Name
      };
      TaskStep checkoutTask1 = taskStep;
      checkoutTask1.Inputs[PipelineConstants.CheckoutTaskInputs.Repository] = repositoryResource1.Alias;
      CheckoutOptions checkoutOptions1;
      if (repositoryResource1.Properties.TryGetValue<CheckoutOptions>(RepositoryPropertyNames.CheckoutOptions, out checkoutOptions1))
        this.MergeCheckoutOptions(checkoutOptions1, checkoutTask1);
      return (IList<TaskStep>) new TaskStep[1]
      {
        checkoutTask1
      };
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPreTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<string>> GetInputsToProvide(IPipelineContext context) => new Dictionary<Guid, List<string>>();

    public IList<TaskStep> GetPostSteps(IPipelineContext context, IReadOnlyList<JobStep> steps) => (IList<TaskStep>) new List<TaskStep>();

    private void MergeCheckoutOptions(CheckoutOptions checkoutOptions, TaskStep checkoutTask)
    {
      if (!checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.Clean) && !string.IsNullOrEmpty(checkoutOptions.Clean))
        checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Clean] = checkoutOptions.Clean;
      if (!checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.FetchDepth) && !string.IsNullOrEmpty(checkoutOptions.FetchDepth))
        checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.FetchDepth] = checkoutOptions.FetchDepth;
      if (!checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.FetchTags) && !string.IsNullOrEmpty(checkoutOptions.FetchTags))
        checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.FetchTags] = checkoutOptions.FetchTags;
      if (!checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.Lfs) && !string.IsNullOrEmpty(checkoutOptions.Lfs))
        checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Lfs] = checkoutOptions.Lfs;
      if (!checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.PersistCredentials) && !string.IsNullOrEmpty(checkoutOptions.PersistCredentials))
        checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.PersistCredentials] = checkoutOptions.PersistCredentials;
      if (checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.Submodules) || string.IsNullOrEmpty(checkoutOptions.Submodules))
        return;
      checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Submodules] = checkoutOptions.Submodules;
    }

    public bool ResolveStep(
      IPipelineContext context,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      resolvedSteps = (IList<TaskStep>) new List<TaskStep>();
      return false;
    }

    public RepositoryResource Get(RepositoryReference reference)
    {
      if (reference == null)
        return (RepositoryResource) null;
      string id = reference.Id;
      string literal = reference.Name?.Literal;
      return string.IsNullOrEmpty(id) && string.IsNullOrEmpty(literal) ? (RepositoryResource) null : this.Get(id);
    }
  }
}
