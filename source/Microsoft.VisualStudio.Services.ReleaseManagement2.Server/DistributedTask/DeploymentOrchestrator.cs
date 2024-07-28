// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.DeploymentOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public abstract class DeploymentOrchestrator : DistributedTaskOrchestrator
  {
    private readonly Func<Guid, int, TaskAgentPoolReference> getPoolFromQueueId;
    private readonly Func<IVssRequestContext, IEnumerable<ArtifactTypeBase>> getArtifactExtensions;

    protected DeploymentOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : this(requestContext, projectId, (Func<Guid, int, TaskAgentPoolReference>) ((p, s) => DeploymentOrchestrator.GetPoolFromQueueId(requestContext, p, s)), (Func<IVssRequestContext, IEnumerable<ArtifactTypeBase>>) (tasks => ExtensionArtifactsRetriever.GetExtensionArtifacts(requestContext)))
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design.")]
    protected internal DeploymentOrchestrator(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<Guid, int, TaskAgentPoolReference> getPoolFromQueueId,
      Func<IVssRequestContext, IEnumerable<ArtifactTypeBase>> getArtifactExtensions)
      : base(requestContext, projectId)
    {
      this.getPoolFromQueueId = getPoolFromQueueId;
      this.getArtifactExtensions = getArtifactExtensions;
    }

    public override IOrchestrationEnvironment GetEnvironment(AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      DeploymentInput agentDeploymentInput = DistributedTaskOrchestrator.GetAgentDeploymentInput(input.DeployPhaseData, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables);
      IOrchestrationEnvironment environment = base.GetEnvironment(input);
      environment.Variables["release.skipartifactsDownload"] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) ArtifactTaskMapper.GetSkipArtifactsDownloadValueForAgent(agentDeploymentInput).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      environment.Variables[WellKnownDistributedTaskVariables.EnableAccessToken] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) agentDeploymentInput.EnableAccessToken.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      environment.Variables[WellKnownDistributedTaskVariables.JobParallelismTag] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) ArtifactExtensions.GetParallelismTag(this.RequestContext, this.ProjectId, input.Artifacts);
      return environment;
    }

    public override IOrchestrationProcess CreateTaskOrchestrationContainer(
      AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      TaskOrchestrationContainer orchestrationContainer = new TaskOrchestrationContainer();
      IList<WorkflowTask> workflow = input.DeployPhaseData.Workflow;
      TaskHub taskHub = DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName);
      this.TraceInformation("AgentPoolBasedOrchestrator: Creating Task orchestration container for ReleaseId: {0}, ReleaseDeployPhaseId: {1}, ProjectId: {2}, DeploymentInput: {3}", (object) input.ReleaseId, (object) input.ReleaseDeployPhaseId, (object) this.ProjectId, (object) JsonUtility.ToString((object) input.DeployPhaseData));
      string minimumAgentVersion = this.ComputeMinimumAgentVersion(input.Artifacts);
      List<TaskInstance> source = new List<TaskInstance>();
      DeploymentInput agentDeploymentInput = DistributedTaskOrchestrator.GetAgentDeploymentInput(input.DeployPhaseData, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables);
      if (!agentDeploymentInput.SkipArtifactsDownload)
      {
        IEnumerable<WorkflowTask> artifactTasks = ArtifactTaskMapper.GetArtifactTasks(this.RequestContext, this.ProjectId, this.getArtifactExtensions, input.Artifacts, agentDeploymentInput.ArtifactsDownloadInput, input);
        source.AddRange(artifactTasks.Select<WorkflowTask, TaskInstance>((Func<WorkflowTask, TaskInstance>) (x => x.ToTaskInstance())));
      }
      if (!input.ExternalVariableTasks.IsNullOrEmpty<TaskInstance>())
        source.AddRange((IEnumerable<TaskInstance>) input.ExternalVariableTasks);
      source.AddRange(workflow.Select<WorkflowTask, TaskInstance>((Func<WorkflowTask, TaskInstance>) (x => x.ToTaskInstance((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables, this.RequestContext))));
      string toDeserialize = JsonUtility.ToString<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Demand>(agentDeploymentInput.Demands);
      string jobRefName = OutputVariablesUtility.GetJobRefName(input);
      List<TaskInstance> missingTasks;
      TaskOrchestrationJob job;
      if (!taskHub.TryCreateJob(this.RequestContext, "Release", jobRefName, source.Where<TaskInstance>((Func<TaskInstance, bool>) (task => task.Enabled)).ToList<TaskInstance>(), out missingTasks, out job, JsonUtility.FromString<List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>(toDeserialize), this.GetJobExecutionTimeout(agentDeploymentInput.TimeoutInMinutes), minimumAgentVersion, includeTaskDemands: this.ShouldIncludeTaskDemands(input)))
        throw new TaskDefinitionNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.DeploymentNotStartedTaskNotFoundMessageFormat, (object) missingTasks.First<TaskInstance>().Id));
      orchestrationContainer.Children.Add((TaskOrchestrationItem) job);
      return (IOrchestrationProcess) orchestrationContainer;
    }

    public override TaskAgentPoolReference GetTaskAgentPoolReference(AutomationEngineInput input)
    {
      DeployPhaseSnapshot phaseData = input != null ? input.DeployPhaseData : throw new ArgumentNullException(nameof (input));
      if (phaseData == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException();
      return this.getPoolFromQueueId(this.ProjectId, DistributedTaskOrchestrator.GetAgentDeploymentInput(phaseData, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables).QueueId);
    }

    protected abstract bool ShouldIncludeTaskDemands(AutomationEngineInput input);

    private static TaskAgentPoolReference GetPoolFromQueueId(
      IVssRequestContext context,
      Guid projectId,
      int queueId)
    {
      IVssRequestContext vssRequestContext = context.Elevate();
      TaskAgentQueue agentQueue = vssRequestContext.GetService<IDistributedTaskPoolService>().GetAgentQueue(vssRequestContext, projectId, queueId);
      if (agentQueue == null)
        throw new QueueNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.QueueNotFound, (object) queueId));
      return agentQueue.Pool != null ? agentQueue.Pool : throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.PoolNotFound, (object) agentQueue.Name));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design.")]
    private string ComputeMinimumAgentVersion(IList<ArtifactSource> artifacts)
    {
      List<CustomArtifact> list = this.getArtifactExtensions(this.RequestContext).ToList<ArtifactTypeBase>().Select<ArtifactTypeBase, CustomArtifact>((Func<ArtifactTypeBase, CustomArtifact>) (extension => extension as CustomArtifact)).ToList<CustomArtifact>();
      string minimumAgentVersion = "1.97.0";
      if (artifacts.Any<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.ArtifactTypeId == "TFVC")))
        minimumAgentVersion = "1.98.0";
      if (artifacts.Any<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.ArtifactTypeId == "ExternalTfsXamlBuild")))
        minimumAgentVersion = "1.98.1";
      foreach (ArtifactSource artifact in (IEnumerable<ArtifactSource>) artifacts)
      {
        ArtifactSource linkedArtifact = artifact;
        CustomArtifact customArtifact = list.FirstOrDefault<CustomArtifact>((Func<CustomArtifact, bool>) (s => s.ArtifactTypeId == linkedArtifact.ArtifactTypeId));
        if (customArtifact != null && customArtifact.ArtifactType != null && !customArtifact.ArtifactType.Equals("Build", StringComparison.OrdinalIgnoreCase))
          minimumAgentVersion = "1.99.0";
      }
      if (artifacts.Any<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => artifact.ArtifactTypeId == "GitHub")))
        minimumAgentVersion = "1.101.0";
      if (artifacts.Any<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => (artifact.IsGitArtifact || artifact.IsGitHubArtifact) && artifact.SourceData.ContainsKey("checkoutSubmodules") && string.Equals(artifact.SourceData["checkoutSubmodules"].Value, bool.TrueString, StringComparison.OrdinalIgnoreCase) && artifact.SourceData.ContainsKey("checkoutNestedSubmodules") && string.Equals(artifact.SourceData["checkoutNestedSubmodules"].Value, bool.TrueString, StringComparison.OrdinalIgnoreCase))))
        minimumAgentVersion = "2.135.0";
      return minimumAgentVersion;
    }
  }
}
