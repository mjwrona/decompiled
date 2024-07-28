// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.GreenlightingOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public class GreenlightingOrchestrator : DistributedTaskOrchestrator
  {
    private const string ReleaseDeploymentEndTimeVariable = "release.deployment.endTime";
    private const string ReleaseDeploymentGateStabilizationTimeInMinutes = "release.deployment.gate.stabilizationTime";

    public GreenlightingOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext, projectId)
    {
    }

    public override string TaskHubName => "Gates";

    public override Guid StartDeployment(AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      try
      {
        TaskAgentPoolReference agentPoolReference = this.GetTaskAgentPoolReference(input);
        Uri greenlightingArtifactUri = GreenlightingOrchestrator.GetGreenlightingArtifactUri(this.ProjectId, input);
        IOrchestrationEnvironment environment = this.GetEnvironment(input);
        IOrchestrationProcess orchestrationContainer = this.CreateTaskOrchestrationContainer(input);
        Guid deploymentRequestedForId = input.ReleaseDeploymentRequestedForId;
        string planGroup = input.ReleaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        TaskOrchestrationOwner definitionReference = this.GetDefinitionReference(input);
        TaskOrchestrationOwner ownerReference = this.GetOwnerReference(input);
        Guid guid = GreenlightingOrchestrator.RunPlan(this.RequestContext, this.TaskHubName, this.ProjectId, Guid.NewGuid(), planGroup, agentPoolReference, greenlightingArtifactUri, environment, (TaskOrchestrationContainer) orchestrationContainer, deploymentRequestedForId, definitionReference, ownerReference);
        this.TraceInformation("DistributedTaskOrchestrator: orchestration started for ReleaseId: {0}, ReleaseStepId: {1}, ReleaseDeployPhaseId: {2}, ProjectId: {3}, RunPlan: {4}", (object) input.ReleaseId, (object) input.ReleaseStepId, (object) input.ReleaseDeployPhaseId, (object) this.ProjectId, (object) guid);
        return guid;
      }
      catch (Exception ex)
      {
        this.TraceError("DistributedTaskOrchestrator: Start orchestration failed for ReleaseId: {0}, ReleaseStepId: {1}, ReleaseDeployPhaseId: {2}, with exception: {3}", (object) input.ReleaseId, (object) input.ReleaseStepId, (object) input.ReleaseDeployPhaseId, (object) ex.ToString());
        throw;
      }
    }

    public override bool CancelDeployment(Guid planId, TimeSpan jobCancelTimeout)
    {
      if (planId == Guid.Empty)
        return false;
      DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName).CancelPlan(this.RequestContext, this.ProjectId, planId, jobCancelTimeout, string.Empty);
      return true;
    }

    public override IOrchestrationProcess CreateTaskOrchestrationContainer(
      AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      this.TraceInformation("GreenlightingOrchestrator: Creating Task orchestration container for ReleaseId: {0}, ReleaseDeployPhaseId: {1}, ProjectId: {2}, DeploymentInput: {3}", (object) input.ReleaseId, (object) input.ReleaseDeployPhaseId, (object) this.ProjectId, (object) JsonUtility.ToString((object) input.DeployPhaseData));
      TaskOrchestrationContainer orchestrationContainer = new TaskOrchestrationContainer();
      orchestrationContainer.Data["GreenlightingStabilizationTimeInMinutes"] = input.Data["GreenlightingStabilizationTimeInMinutes"];
      orchestrationContainer.Data["GreenlightingSamplingIntervalInMinutes"] = input.Data["GreenlightingSamplingIntervalInMinutes"];
      orchestrationContainer.Data["GreenlightingMinimumSuccessfulMinutes"] = input.Data["GreenlightingMinimumSuccessfulMinutes"];
      List<TaskInstance> list = input.DeployPhaseData.Workflow.Where<WorkflowTask>((Func<WorkflowTask, bool>) (task => task.Enabled)).Select<WorkflowTask, TaskInstance>((Func<WorkflowTask, TaskInstance>) (x => x.ToTaskInstance((IDictionary<string, ConfigurationVariableValue>) input.Variables, this.RequestContext))).ToList<TaskInstance>();
      if (!list.Any<TaskInstance>())
        return (IOrchestrationProcess) orchestrationContainer;
      string jobRefName = OutputVariablesUtility.GetJobRefName(input);
      TaskOrchestrationJob job;
      if (!DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName).TryCreateJob(this.RequestContext, "Release", jobRefName, list, out List<TaskInstance> _, out job, jobTimeoutInMinutes: GreenlightingOrchestrator.GetJobTimeout(input.Data), executionMode: "Server"))
        throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ServerJobCreationError));
      orchestrationContainer.Children.Add((TaskOrchestrationItem) job);
      return (IOrchestrationProcess) orchestrationContainer;
    }

    public override IOrchestrationEnvironment GetEnvironment(AutomationEngineInput input)
    {
      IOrchestrationEnvironment environment = base.GetEnvironment(input);
      environment.Variables[WellKnownDistributedTaskVariables.EnableAccessToken] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) true.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (input.StepType == 8)
      {
        string str;
        input.Data.TryGetValue("DeploymentEndTime", out str);
        environment.Variables["release.deployment.endTime"] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) str;
      }
      string str1;
      input.Data.TryGetValue("GreenlightingStabilizationTimeInMinutes", out str1);
      environment.Variables["release.deployment.gate.stabilizationTime"] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) str1;
      return environment;
    }

    public override string GetLogRelativeFilePath(TimelineRecord timelineRecord, string folderName)
    {
      string str1 = timelineRecord != null ? FileSpec.RemoveInvalidFileNameChars(timelineRecord.Name) : throw new ArgumentNullException(nameof (timelineRecord));
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      string str2 = str1;
      // ISSUE: variable of a boxed type
      __Boxed<int?> order = (ValueType) timelineRecord.Order;
      TaskResult? result = timelineRecord.Result;
      string str3;
      if (result.HasValue)
      {
        result = timelineRecord.Result;
        str3 = result.ToString();
      }
      else
        str3 = "InProgress";
      return string.Format((IFormatProvider) currentCulture, "{0}/Sample{1}_{2}.log", (object) str2, (object) order, (object) str3);
    }

    public virtual void UpdateGate(Guid planId, IList<string> gateNames)
    {
      if (planId == Guid.Empty)
        return;
      TaskHub taskHub = DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName);
      DeploymentGatesChangeEvent eventData = new DeploymentGatesChangeEvent(gateNames);
      try
      {
        taskHub.RaiseGateChangeEvent(this.RequestContext, planId, eventData);
      }
      catch (TaskOrchestrationPlanTerminatedException ex)
      {
      }
    }

    private static Guid RunPlan(
      IVssRequestContext context,
      string taskhubName,
      Guid projectId,
      Guid planId,
      string planGroup,
      TaskAgentPoolReference taskAgentPoolReference,
      Uri artifactUri,
      IOrchestrationEnvironment planEnvironment,
      TaskOrchestrationContainer taskOrchestrationContainer,
      Guid requestedForId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference)
    {
      return DistributedTaskOrchestrator.GetTaskHub(context, taskhubName).RunPlan(context, taskAgentPoolReference, projectId, planId, planGroup, artifactUri, planEnvironment, (IOrchestrationProcess) taskOrchestrationContainer, (BuildOptions) null, requestedForId, definitionReference, ownerReference).PlanId;
    }

    private static int GetJobTimeout(IDictionary<string, string> dataDictionary)
    {
      int result1 = 2880;
      if (dataDictionary != null && dataDictionary.ContainsKey("GreenlightingJobTimeoutInMinutes"))
      {
        if (!int.TryParse(dataDictionary["GreenlightingJobTimeoutInMinutes"], out result1))
          result1 = 2880;
        int result2;
        if (int.TryParse(dataDictionary["GreenlightingStabilizationTimeInMinutes"], out result2))
          result1 += result2;
      }
      return result1;
    }

    private static Uri GetGreenlightingArtifactUri(Guid projectId, AutomationEngineInput input) => ReleaseArtifactCreator.CreateGreenlightingUri(new ReleaseArtifact()
    {
      ProjectId = projectId,
      ReleaseId = input.ReleaseId,
      EnvironmentId = input.EnvironmentId,
      ReleaseStepId = input.ReleaseStepId,
      ReleaseDeployPhaseId = input.DeployPhaseData.IsGatesPhase() ? input.ReleaseDeployPhaseId : 0
    });
  }
}
