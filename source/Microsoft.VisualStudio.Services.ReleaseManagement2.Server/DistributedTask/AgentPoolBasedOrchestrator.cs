// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.AgentPoolBasedOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public class AgentPoolBasedOrchestrator : DeploymentOrchestrator
  {
    public AgentPoolBasedOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext, projectId)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design.")]
    protected internal AgentPoolBasedOrchestrator(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<Guid, int, TaskAgentPoolReference> getPoolFromQueueId,
      Func<IVssRequestContext, IEnumerable<ArtifactTypeBase>> getArtifactExtensions)
      : base(requestContext, projectId, getPoolFromQueueId, getArtifactExtensions)
    {
    }

    public override string TaskHubName => "Release";

    public override IOrchestrationProcess CreateTaskOrchestrationContainer(
      AutomationEngineInput input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      AgentDeploymentInput deploymentInput = (AgentDeploymentInput) input.DeployPhaseData.GetDeploymentInput((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables);
      ExecutionHandler handler = ParallelExecutionHandlerFactory.GetHandler(deploymentInput.ParallelExecution, (Action<string>) (message => this.TraceInformation(message)));
      TaskOrchestrationContainer orchestrationContainer = (TaskOrchestrationContainer) base.CreateTaskOrchestrationContainer(input);
      PlanEnvironment environment = (PlanEnvironment) this.GetEnvironment(input);
      TaskOrchestrationContainer container = orchestrationContainer;
      handler.ApplyParallelExecution(environment, container);
      this.TraceInformation("Number of jobs in container: {0}.", (object) orchestrationContainer.Children.Count<TaskOrchestrationItem>((Func<TaskOrchestrationItem, bool>) (x => x is TaskOrchestrationJob)));
      if (deploymentInput.ImageId != 0)
        orchestrationContainer.Data[TaskAgentRequestConstants.HostedAgentImageIdKey] = deploymentInput.ImageId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return (IOrchestrationProcess) orchestrationContainer;
    }

    public override Dictionary<Guid, string> GetJobIdNameMap(
      IEnumerable<TimelineRecord> timelineRecords)
    {
      Dictionary<Guid, string> jobIdNameMap = new Dictionary<Guid, string>();
      foreach (TimelineRecord timelineRecord in timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.RecordType == "Job")))
        jobIdNameMap[timelineRecord.Id] = timelineRecord.Name;
      return jobIdNameMap;
    }

    protected override bool ShouldIncludeTaskDemands(AutomationEngineInput input) => !this.GetTaskAgentPoolReference(input).IsHosted;
  }
}
