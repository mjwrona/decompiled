// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentCompatExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public static class EnvironmentCompatExtensions
  {
    public static JObject CreateCompatDeploymentInputJObject(
      string demands,
      int queueId,
      bool skipArtifactsDownload,
      int timeoutInMinutes,
      bool enableAccessToken)
    {
      return EnvironmentCompatExtensions.CreateCompatDeploymentInput(demands, queueId, skipArtifactsDownload, timeoutInMinutes, enableAccessToken).ToJObject();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    private static AgentDeploymentInput CreateCompatDeploymentInput(
      string demands,
      int queueId,
      bool skipArtifactsDownload,
      int timeoutInMinutes,
      bool enableAccessToken)
    {
      AgentDeploymentInput compatDeploymentInput = new AgentDeploymentInput();
      compatDeploymentInput.Demands = demands != null ? (IList<Demand>) JsonConvert.DeserializeObject<List<Demand>>(demands) : (IList<Demand>) null;
      compatDeploymentInput.QueueId = queueId;
      compatDeploymentInput.SkipArtifactsDownload = skipArtifactsDownload;
      compatDeploymentInput.TimeoutInMinutes = timeoutInMinutes;
      compatDeploymentInput.EnableAccessToken = enableAccessToken;
      return compatDeploymentInput;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase CreateCompatDeployPhase(
      this ReleaseDefinitionEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      List<WorkflowTask> workflowTaskList = new List<WorkflowTask>();
      if (environment.DeployStep != null && environment.DeployStep.Tasks != null)
        workflowTaskList.AddRange((IEnumerable<WorkflowTask>) environment.DeployStep.Tasks);
      AgentBasedDeployPhase compatDeployPhase = new AgentBasedDeployPhase();
      compatDeployPhase.Name = environment.Name;
      compatDeployPhase.Rank = 1;
      compatDeployPhase.WorkflowTasks = (IList<WorkflowTask>) workflowTaskList;
      AgentDeploymentInput agentDeploymentInput = new AgentDeploymentInput();
      agentDeploymentInput.Demands = environment.Demands != null ? (IList<Demand>) new List<Demand>((IEnumerable<Demand>) environment.Demands) : (IList<Demand>) null;
      agentDeploymentInput.QueueId = environment.QueueId;
      agentDeploymentInput.SkipArtifactsDownload = environment.EnvironmentOptions.SkipArtifactsDownload;
      agentDeploymentInput.TimeoutInMinutes = environment.EnvironmentOptions.TimeoutInMinutes;
      agentDeploymentInput.EnableAccessToken = environment.EnvironmentOptions.EnableAccessToken;
      compatDeployPhase.DeploymentInput = agentDeploymentInput;
      return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) compatDeployPhase;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase CreateCompatDeployPhasesSnapshot(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      ReleaseEnvironment serverEnvironment)
    {
      string str = environment != null ? environment.Name : throw new ArgumentNullException(nameof (environment));
      if (serverEnvironment != null && serverEnvironment.GetDesignerDeployPhaseSnapshots() != null && serverEnvironment.GetDesignerDeployPhaseSnapshots().Any<DeployPhaseSnapshot>())
        str = serverEnvironment.GetDesignerDeployPhaseSnapshots().First<DeployPhaseSnapshot>().Name;
      AgentBasedDeployPhase basedDeployPhase = new AgentBasedDeployPhase();
      basedDeployPhase.Name = str;
      basedDeployPhase.Rank = 1;
      basedDeployPhase.WorkflowTasks = (IList<WorkflowTask>) new List<WorkflowTask>((IEnumerable<WorkflowTask>) environment.WorkflowTasks);
      AgentBasedDeployPhase deployPhasesSnapshot = basedDeployPhase;
      foreach (WorkflowTask workflowTask in (IEnumerable<WorkflowTask>) deployPhasesSnapshot.WorkflowTasks)
        workflowTask.Condition = (string) null;
      AgentDeploymentInput agentDeploymentInput1 = new AgentDeploymentInput();
      agentDeploymentInput1.Demands = (IList<Demand>) new List<Demand>((IEnumerable<Demand>) environment.Demands);
      agentDeploymentInput1.QueueId = environment.QueueId;
      agentDeploymentInput1.SkipArtifactsDownload = environment.EnvironmentOptions.SkipArtifactsDownload;
      agentDeploymentInput1.TimeoutInMinutes = environment.EnvironmentOptions.TimeoutInMinutes;
      agentDeploymentInput1.EnableAccessToken = environment.EnvironmentOptions.EnableAccessToken;
      AgentDeploymentInput agentDeploymentInput2 = agentDeploymentInput1;
      deployPhasesSnapshot.DeploymentInput = agentDeploymentInput2;
      return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) deployPhasesSnapshot;
    }

    public static string GetCompatWorkflow(this DefinitionEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.DeployPhases != null)
      {
        DeployPhase deployPhase = environment.DeployPhases.FirstOrDefault<DeployPhase>((Func<DeployPhase, bool>) (d => d.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment));
        if (deployPhase != null)
          return deployPhase.Workflow;
      }
      return (string) null;
    }

    public static string GetCompatWorkflow(this ReleaseEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.GetDesignerDeployPhaseSnapshots() != null)
      {
        DeployPhaseSnapshot deployPhaseSnapshot = environment.GetDesignerDeployPhaseSnapshots().FirstOrDefault<DeployPhaseSnapshot>();
        if (deployPhaseSnapshot != null)
          return JsonConvert.SerializeObject((object) deployPhaseSnapshot.Workflow);
      }
      return (string) null;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static IList<WorkflowTask> GetCompatWorkflow(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.DeployPhasesSnapshot != null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase = environment.DeployPhasesSnapshot.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>();
        if (deployPhase != null)
          return deployPhase.WorkflowTasks;
      }
      return (IList<WorkflowTask>) new List<WorkflowTask>();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static IList<WorkflowTask> GetCompatWorkflow(
      this ReleaseDefinitionEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.DeployPhases != null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase = environment.DeployPhases.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (d => d.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment));
        if (deployPhase != null)
          return deployPhase.WorkflowTasks;
      }
      return (IList<WorkflowTask>) new List<WorkflowTask>();
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions GetCompatEnvironmentOptions(
      this DefinitionEnvironment environment)
    {
      EnvironmentOptions environmentOptions1 = environment != null ? environment.EnvironmentOptions : throw new ArgumentNullException(nameof (environment));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions environmentOptions2 = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions()
      {
        EmailNotificationType = environmentOptions1.EmailNotificationType,
        EmailRecipients = environmentOptions1.EmailRecipients,
        PublishDeploymentStatus = environmentOptions1.PublishDeploymentStatus,
        AutoLinkWorkItems = environmentOptions1.AutoLinkWorkItems,
        PullRequestDeploymentEnabled = environmentOptions1.PullRequestDeploymentEnabled
      };
      AgentDeploymentInput deploymentInput = environment.GetDeploymentInput();
      if (deploymentInput != null)
      {
        environmentOptions2.SkipArtifactsDownload = deploymentInput.SkipArtifactsDownload;
        environmentOptions2.TimeoutInMinutes = deploymentInput.TimeoutInMinutes;
        environmentOptions2.EnableAccessToken = deploymentInput.EnableAccessToken;
      }
      return environmentOptions2;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions GetCompatEnvironmentOptions(
      this ReleaseEnvironment environment)
    {
      EnvironmentOptions environmentOptions1 = environment != null ? environment.EnvironmentOptions : throw new ArgumentNullException(nameof (environment));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions environmentOptions2 = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions();
      environmentOptions2.EmailNotificationType = environmentOptions1.EmailNotificationType;
      environmentOptions2.EmailRecipients = environmentOptions1.EmailRecipients;
      environmentOptions2.PublishDeploymentStatus = environmentOptions1.PublishDeploymentStatus;
      environmentOptions2.AutoLinkWorkItems = environmentOptions1.AutoLinkWorkItems;
      environmentOptions2.PullRequestDeploymentEnabled = environmentOptions1.PullRequestDeploymentEnabled;
      AgentDeploymentInput deploymentInput = environment.GetDeploymentInput();
      if (deploymentInput != null)
      {
        environmentOptions2.SkipArtifactsDownload = deploymentInput.SkipArtifactsDownload;
        environmentOptions2.TimeoutInMinutes = deploymentInput.TimeoutInMinutes;
        environmentOptions2.EnableAccessToken = deploymentInput.EnableAccessToken;
      }
      return environmentOptions2;
    }

    public static string GetCompatDemands(this DefinitionEnvironment environment)
    {
      AgentDeploymentInput deploymentInput = environment.GetDeploymentInput();
      return deploymentInput != null && deploymentInput.Demands != null ? JsonConvert.SerializeObject((object) deploymentInput.Demands) : (string) null;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static IList<Demand> GetCompatDemands(this ReleaseDefinitionEnvironment environment)
    {
      AgentDeploymentInput compatDeploymentInput = environment.GetCompatDeploymentInput();
      return compatDeploymentInput != null && compatDeploymentInput.Demands != null ? compatDeploymentInput.Demands : (IList<Demand>) new List<Demand>();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static IList<Demand> GetCompatDemands(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      AgentDeploymentInput compatDeploymentInput = environment.GetCompatDeploymentInput();
      return compatDeploymentInput != null && compatDeploymentInput.Demands != null ? compatDeploymentInput.Demands : (IList<Demand>) new List<Demand>();
    }

    public static int GetCompatQueueId(this DefinitionEnvironment environment)
    {
      AgentDeploymentInput deploymentInput = environment.GetDeploymentInput();
      return deploymentInput == null ? 0 : deploymentInput.QueueId;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static int GetCompatQueueId(this ReleaseDefinitionEnvironment environment)
    {
      AgentDeploymentInput compatDeploymentInput = environment.GetCompatDeploymentInput();
      return compatDeploymentInput == null ? 0 : compatDeploymentInput.QueueId;
    }

    public static string GetCompatDemands(this ReleaseEnvironment environment)
    {
      AgentDeploymentInput deploymentInput = environment.GetDeploymentInput();
      return deploymentInput != null && deploymentInput.Demands != null ? JsonConvert.SerializeObject((object) deploymentInput.Demands) : string.Empty;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static int GetCompatQueueId(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      AgentDeploymentInput compatDeploymentInput = environment.GetCompatDeploymentInput();
      return compatDeploymentInput == null ? 0 : compatDeploymentInput.QueueId;
    }

    public static int GetCompatQueueId(this ReleaseEnvironment environment)
    {
      AgentDeploymentInput deploymentInput = environment.GetDeploymentInput();
      return deploymentInput == null ? 0 : deploymentInput.QueueId;
    }

    public static DeployPhaseSnapshot GetCompatReleaseDeployPhase(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions environmentOptions,
      string workflow,
      string demands,
      int queueId)
    {
      bool skipArtifactsDownload = environmentOptions != null ? environmentOptions.SkipArtifactsDownload : throw new ArgumentNullException(nameof (environmentOptions));
      int timeoutInMinutes = environmentOptions.TimeoutInMinutes;
      bool enableAccessToken = environmentOptions.EnableAccessToken;
      AgentDeploymentInput compatDeploymentInput = EnvironmentCompatExtensions.CreateCompatDeploymentInput(demands, queueId, skipArtifactsDownload, timeoutInMinutes, enableAccessToken);
      return new DeployPhaseSnapshot()
      {
        PhaseType = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment,
        Name = string.Empty,
        Rank = 1,
        Workflow = (IList<WorkflowTask>) workflow.ToWorkflowTaskList(),
        DeploymentInput = compatDeploymentInput.ToJObject()
      };
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase GetCompatReleaseDeployPhase(
      DeploymentAttempt deployStep)
    {
      return deployStep != null ? deployStep.ReleaseDeployPhases.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase>() : throw new ArgumentNullException(nameof (deployStep));
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static AgentDeploymentInput GetCompatDeploymentInput(
      this ReleaseDefinitionEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (environment.DeployPhases != null)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase = environment.DeployPhases.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (d => d.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment));
        if (deployPhase != null)
          return deployPhase.GetDeploymentInput() as AgentDeploymentInput;
      }
      return (AgentDeploymentInput) null;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "right term")]
    public static AgentDeploymentInput GetCompatDeploymentInput(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase = environment.DeployPhasesSnapshot.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (d => d.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment));
      return deployPhase != null ? deployPhase.GetDeploymentInput() as AgentDeploymentInput : (AgentDeploymentInput) null;
    }

    private static AgentDeploymentInput GetDeploymentInput(this DefinitionEnvironment environment)
    {
      DeployPhase deployPhase = environment.DeployPhases.FirstOrDefault<DeployPhase>((Func<DeployPhase, bool>) (d => d.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment));
      return deployPhase != null ? deployPhase.GetDeploymentInput() as AgentDeploymentInput : (AgentDeploymentInput) null;
    }

    private static AgentDeploymentInput GetDeploymentInput(this ReleaseEnvironment environment)
    {
      DeployPhaseSnapshot deployPhaseSnapshot = environment.GetDesignerDeployPhaseSnapshots().FirstOrDefault<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (d => d.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment));
      return deployPhaseSnapshot != null && deployPhaseSnapshot.DeploymentInput != null ? deployPhaseSnapshot.GetDeploymentInput() as AgentDeploymentInput : (AgentDeploymentInput) null;
    }
  }
}
