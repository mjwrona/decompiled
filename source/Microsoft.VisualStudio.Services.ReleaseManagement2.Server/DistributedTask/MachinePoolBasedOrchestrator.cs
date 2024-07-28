// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.MachinePoolBasedOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public class MachinePoolBasedOrchestrator : DeploymentOrchestrator
  {
    public MachinePoolBasedOrchestrator(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext, projectId, (Func<Guid, int, TaskAgentPoolReference>) ((p, s) => MachinePoolBasedOrchestrator.GetPoolFromMachineGroupId(requestContext, p, s)), (Func<IVssRequestContext, IEnumerable<ArtifactTypeBase>>) (tasks => ExtensionArtifactsRetriever.GetExtensionArtifacts(requestContext)))
    {
    }

    public override string TaskHubName => "Deployment";

    public override IOrchestrationEnvironment GetEnvironment(AutomationEngineInput input)
    {
      IOrchestrationEnvironment environment = base.GetEnvironment(input);
      environment.Variables["agent.deploymentGroupId"] = (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableValue) ((DeploymentInput) input.DeployPhaseData.GetDeploymentInput((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables)).QueueId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return environment;
    }

    public override IOrchestrationProcess CreateTaskOrchestrationContainer(
      AutomationEngineInput input)
    {
      TaskOrchestrationContainer orchestrationContainer = input != null ? (TaskOrchestrationContainer) base.CreateTaskOrchestrationContainer(input) : throw new ArgumentNullException(nameof (input));
      MachineGroupDeploymentInput deploymentInput = (MachineGroupDeploymentInput) input.DeployPhaseData.GetDeploymentInput((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables);
      string str1 = deploymentInput.DeploymentHealthOption ?? "Custom";
      IDictionary<string, string> data1 = orchestrationContainer.Data;
      int num = deploymentInput.HealthPercent;
      string str2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      data1.Add("HealthPercent", str2);
      orchestrationContainer.Data.Add("DeploymentHealthOption", str1);
      IDictionary<string, string> data2 = orchestrationContainer.Data;
      num = deploymentInput.QueueId;
      string str3 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      data2.Add("MachineGroupId", str3);
      orchestrationContainer.Data.Add("ProjectId", this.ProjectId.ToString());
      orchestrationContainer.Data.Add("Tags", JsonConvert.SerializeObject((object) MachinePoolBasedOrchestrator.GetResolvedTags(deploymentInput.Tags, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) input.Variables)));
      orchestrationContainer.Data.Add("DeploymentTargetIds", MachinePoolBasedOrchestrator.GetTargetIds(input.Data));
      IDictionary<string, string> data3 = orchestrationContainer.Data;
      num = MachinePoolBasedOrchestrator.GetMaxAttemptCount(this.RequestContext);
      string str4 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      data3.Add("MaxAttemptCount", str4);
      return (IOrchestrationProcess) orchestrationContainer;
    }

    protected override bool ShouldIncludeTaskDemands(AutomationEngineInput input) => true;

    private static TaskAgentPoolReference GetPoolFromMachineGroupId(
      IVssRequestContext context,
      Guid projectId,
      int machineGroupId)
    {
      IVssRequestContext vssRequestContext = context.Elevate();
      DeploymentGroup deploymentGroup = vssRequestContext.GetService<IDistributedTaskPoolService>().GetDeploymentGroup(vssRequestContext, projectId, machineGroupId);
      if (deploymentGroup == null)
        throw new DeploymentMachineGroupNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.MachineGroupNotFound, (object) machineGroupId));
      return deploymentGroup.Pool != null ? deploymentGroup.Pool : throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.PoolNotFound, (object) deploymentGroup.Name));
    }

    public override Dictionary<Guid, string> GetJobIdNameMap(
      IEnumerable<TimelineRecord> timelineRecords)
    {
      Dictionary<Guid, string> jobIdNameMap = new Dictionary<Guid, string>();
      foreach (TimelineRecord timelineRecord in timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x => x.RecordType == "Job")))
        jobIdNameMap[timelineRecord.Id] = timelineRecord.WorkerName;
      return jobIdNameMap;
    }

    private static string GetTargetIds(IDictionary<string, string> dataDictionary)
    {
      string str;
      return dataDictionary.TryGetValue("DeploymentTargetIds", out str) ? str : JsonConvert.SerializeObject((object) new List<int>());
    }

    private static IList<string> GetResolvedTags(
      IList<string> tags,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      Dictionary<string, string> dictionary = variables != null ? variables.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (variable => variable.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (variable => variable.Value?.Value)) : (Dictionary<string, string>) null;
      HashSet<string> source = new HashSet<string>();
      foreach (string tag in (IEnumerable<string>) tags)
      {
        string str = VariableResolutionHelper.ResolveVariableValue(tag, (IDictionary<string, string>) dictionary, (IDictionary<string, string>) null);
        if (!string.IsNullOrEmpty(str))
          source.UnionWith((IEnumerable<string>) str.Split(','));
      }
      return (IList<string>) source.ToList<string>();
    }

    private static int GetMaxAttemptCount(IVssRequestContext requestContext)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/RunDeploymentPlan/MaxAttemptCount", true, 5);
      return num <= 0 ? 5 : num;
    }
  }
}
