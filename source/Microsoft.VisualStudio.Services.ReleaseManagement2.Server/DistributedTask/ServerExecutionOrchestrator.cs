// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.ServerExecutionOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public class ServerExecutionOrchestrator : DistributedTaskOrchestrator
  {
    public ServerExecutionOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext, projectId)
    {
    }

    public override string TaskHubName => "Release";

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be refactored.")]
    public override IOrchestrationProcess CreateTaskOrchestrationContainer(
      AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      TaskOrchestrationContainer container = new TaskOrchestrationContainer();
      IList<WorkflowTask> workflow = input.DeployPhaseData.Workflow;
      TaskHub taskHub = DistributedTaskOrchestrator.GetTaskHub(this.RequestContext, this.TaskHubName);
      this.TraceInformation("ServerExecutionOrchestrator: Creating Task orchestration container for ReleaseId: {0}, ReleaseDeployPhaseId: {1}, ProjectId: {2}, DeploymentInput: {3}", (object) input.ReleaseId, (object) input.ReleaseDeployPhaseId, (object) this.ProjectId, (object) JsonUtility.ToString((object) input.DeployPhaseData));
      List<TaskInstance> list = workflow.Where<WorkflowTask>((Func<WorkflowTask, bool>) (task => task.Enabled)).Select<WorkflowTask, TaskInstance>((Func<WorkflowTask, TaskInstance>) (x => x.ToTaskInstance((IDictionary<string, ConfigurationVariableValue>) input.Variables, this.RequestContext))).ToList<TaskInstance>();
      if (!list.Any<TaskInstance>())
        return (IOrchestrationProcess) container;
      ServerDeploymentInput deploymentInput = (ServerDeploymentInput) input.DeployPhaseData.GetDeploymentInput((IDictionary<string, ConfigurationVariableValue>) input.Variables);
      string jobRefName = OutputVariablesUtility.GetJobRefName(input);
      TaskOrchestrationJob job;
      if (!taskHub.TryCreateJob(this.RequestContext, "Release", jobRefName, list, out List<TaskInstance> _, out job, jobTimeoutInMinutes: this.GetJobExecutionTimeout(deploymentInput.TimeoutInMinutes), executionMode: "Server"))
        throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ServerJobCreationError));
      container.Children.Add((TaskOrchestrationItem) job);
      ParallelExecutionHandlerFactory.GetHandler(deploymentInput.ParallelExecution, (Action<string>) (message => this.TraceInformation(message))).ApplyParallelExecution((PlanEnvironment) this.GetEnvironment(input), container);
      this.TraceInformation("Number of jobs in container: {0}.", (object) container.Children.Count<TaskOrchestrationItem>((Func<TaskOrchestrationItem, bool>) (x => x is TaskOrchestrationJob)));
      return (IOrchestrationProcess) container;
    }

    public override IOrchestrationEnvironment GetEnvironment(AutomationEngineInput input)
    {
      IOrchestrationEnvironment environment = base.GetEnvironment(input);
      environment.Variables[WellKnownDistributedTaskVariables.EnableAccessToken] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) true.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return environment;
    }

    public override int GetJobExecutionTimeout(int timeoutInMinutes) => timeoutInMinutes;
  }
}
