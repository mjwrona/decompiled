// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.ArtifactTaskMapper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design.")]
  public static class ArtifactTaskMapper
  {
    private const string AgentVersion = "Agent.Version";
    private const string MinAgentVersionForDownloadBuildArtifactTaskV1 = "2.188.2";

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design.")]
    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "By design.")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design.")]
    public static IEnumerable<WorkflowTask> GetArtifactTasks(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<IVssRequestContext, IEnumerable<ArtifactTypeBase>> getArtifactExtensions,
      IList<ArtifactSource> linkedArtifacts,
      ArtifactsDownloadInput artifactsDownloadInput,
      AutomationEngineInput automationEngineInput)
    {
      if (getArtifactExtensions == null)
        throw new ArgumentNullException(nameof (getArtifactExtensions));
      if (linkedArtifacts == null)
        throw new ArgumentNullException(nameof (linkedArtifacts));
      if (artifactsDownloadInput == null)
        throw new ArgumentNullException(nameof (artifactsDownloadInput));
      if (automationEngineInput == null)
        throw new ArgumentNullException(nameof (automationEngineInput));
      List<CustomArtifact> list1 = getArtifactExtensions(requestContext).ToList<ArtifactTypeBase>().Select<ArtifactTypeBase, CustomArtifact>((Func<ArtifactTypeBase, CustomArtifact>) (extension => extension as CustomArtifact)).ToList<CustomArtifact>();
      List<WorkflowTask> artifactTasks = new List<WorkflowTask>();
      Dictionary<WorkflowTask, CustomArtifact> taskToArtifactExtensionMap = new Dictionary<WorkflowTask, CustomArtifact>();
      Dictionary<string, WorkflowTask> artifactAliasToTaskMap = new Dictionary<string, WorkflowTask>();
      List<ArtifactDownloadInputBase> list2 = artifactsDownloadInput.DownloadInputs.Where<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (downloadInput => downloadInput.IsNonTaskifiedArtifact())).ToList<ArtifactDownloadInputBase>();
      string str = list2.IsNullOrEmpty<ArtifactDownloadInputBase>() ? "All" : list2.First<ArtifactDownloadInputBase>().ArtifactDownloadMode;
      if (ReleasePropertyExtensions.GetReleasePropertyValues(requestContext, automationEngineInput.ReleaseId, projectId, (IEnumerable<string>) new List<string>()
      {
        "BuildArtifacts"
      }).FirstOrDefault<Microsoft.TeamFoundation.Framework.Server.PropertyValue>() == null)
        ArtifactTaskMapper.AddBuildArtifactsToReleaseProperties(requestContext, projectId, automationEngineInput.ReleaseId);
      foreach (ArtifactSource linkedArtifact1 in (IEnumerable<ArtifactSource>) linkedArtifacts)
      {
        ArtifactSource linkedArtifact = linkedArtifact1;
        ArtifactDownloadInputBase artifactDownloadInput = artifactsDownloadInput.DownloadInputs.SingleOrDefault<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (a => a.Alias.Equals(linkedArtifact.Alias, StringComparison.OrdinalIgnoreCase)));
        bool flag = linkedArtifact.ArtifactTypeId == "Build" && linkedArtifact.IsXamlBuildArtifact(requestContext);
        if (flag && artifactDownloadInput != null && !artifactDownloadInput.ArtifactDownloadMode.Equals(str, StringComparison.OrdinalIgnoreCase))
          throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.SelectiveArtifactsNotSupportedForXamlBuild, (object) linkedArtifact.Alias));
        if (artifactDownloadInput == null || !artifactDownloadInput.SkipArtifactDownload())
        {
          Microsoft.TeamFoundation.Framework.Server.PropertyValue propertyValue = ReleasePropertyExtensions.GetReleasePropertyValues(requestContext, automationEngineInput.ReleaseId, projectId, (IEnumerable<string>) new List<string>()
          {
            "DownloadBuildArtifactsUsingTask"
          }).FirstOrDefault<Microsoft.TeamFoundation.Framework.Server.PropertyValue>();
          bool result = false;
          if (propertyValue != null)
            bool.TryParse((string) propertyValue.Value, out result);
          switch (linkedArtifact.ArtifactTypeId)
          {
            case "Build":
              BuildArtifactDownloadInput buildArtifactDownloadInput = (BuildArtifactDownloadInput) artifactDownloadInput;
              if (!flag)
              {
                ArtifactTaskMapper.PopulateCustomBuildArtifactTasks(requestContext, automationEngineInput.ReleaseId, projectId, linkedArtifact, buildArtifactDownloadInput, (IList<CustomArtifact>) list1, (IList<WorkflowTask>) artifactTasks, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) automationEngineInput.Variables, result, automationEngineInput);
                continue;
              }
              continue;
            case "Git":
            case "GitHub":
            case "TFVC":
              continue;
            case "Jenkins":
              if (result)
              {
                artifactTasks.Add(ArtifactTaskMapper.GetJenkinsArtifactTask(requestContext, linkedArtifact, (JenkinsArtifactDownloadInput) artifactDownloadInput, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) automationEngineInput.Variables));
                continue;
              }
              continue;
            default:
              CustomArtifact customArtifact;
              if ((customArtifact = list1.FirstOrDefault<CustomArtifact>((Func<CustomArtifact, bool>) (s => string.Equals(s.ArtifactTypeId, linkedArtifact.ArtifactTypeId, StringComparison.OrdinalIgnoreCase)))) != null && !customArtifact.ArtifactType.IsNullOrEmpty<char>() && customArtifact.ArtifactDownloadTaskId != Guid.Empty && (!customArtifact.ArtifactType.Equals("Build", StringComparison.OrdinalIgnoreCase) || requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.CustomBuildArtifactsTasks")))
              {
                WorkflowTask customArtifactTask = ArtifactTaskMapper.GetCustomArtifactTask(requestContext, customArtifact, linkedArtifact, (IDictionary<string, string>) null, artifactDownloadInput, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) automationEngineInput.Variables);
                artifactTasks.Add(customArtifactTask);
                taskToArtifactExtensionMap.Add(customArtifactTask, customArtifact);
                artifactAliasToTaskMap.Add(linkedArtifact.Alias, customArtifactTask);
                continue;
              }
              continue;
          }
        }
      }
      if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.ValidateTaskInputTypes"))
        ArtifactTaskMapper.ValidateTaskInputsForEndpointTypeInputs(requestContext, projectId, (IDictionary<string, WorkflowTask>) artifactAliasToTaskMap, (IDictionary<WorkflowTask, CustomArtifact>) taskToArtifactExtensionMap, linkedArtifacts);
      if (str == "Skip" && !artifactTasks.IsNullOrEmpty<WorkflowTask>())
        artifactTasks.Insert(0, ArtifactTaskMapper.GetDeleteArtifactsDirectoryTask(automationEngineInput));
      return (IEnumerable<WorkflowTask>) artifactTasks;
    }

    private static void AddBuildArtifactsToReleaseProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> action = (Func<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) (component => component.GetRelease(projectId, releaseId));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>(action);
      try
      {
        Dictionary<int, IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>> toSerialize = new Dictionary<int, IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>();
        foreach (ArtifactSource artifactSource in release.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (a => a.IsBuildArtifact)))
        {
          InputValue inputValue;
          int result;
          if (artifactSource.SourceData.TryGetValue("version", out inputValue) && int.TryParse(inputValue.Value, out result))
            toSerialize.Add(artifactSource.Id, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Build.BuildArtifact.GetBuildArtifacts(requestContext, artifactSource.TeamProjectId, result));
        }
        if (toSerialize.Count <= 0)
          return;
        release.Properties.Add(new Microsoft.TeamFoundation.Framework.Server.PropertyValue("BuildArtifacts", (object) JsonUtility.Serialize((object) toSerialize)));
        ArtifactTaskMapper.GetDataAccessLayer(requestContext, projectId).SaveProperties(release.CreateArtifactSpec(projectId), (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) release.Properties);
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        requestContext.TraceCatch(1979015, "ReleaseManagementService", "Service", (Exception) ex);
      }
    }

    private static IDataAccessLayer GetDataAccessLayer(IVssRequestContext context, Guid projectId) => (IDataAccessLayer) new DataAccessLayer(context, projectId);

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "By design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design")]
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "by design.")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.TeamFoundation.Framework.Server.VssRequestContextExtensions.Trace(Microsoft.TeamFoundation.Framework.Server.IVssRequestContext,System.Int32,System.Diagnostics.TraceLevel,System.String,System.String,System.String)", Justification = "telemetry string")]
    public static void PopulateCustomBuildArtifactTasks(
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId,
      ArtifactSource linkedArtifact,
      BuildArtifactDownloadInput buildArtifactDownloadInput,
      IList<CustomArtifact> customArtifactExtensions,
      IList<WorkflowTask> artifactTasksList,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables = null,
      bool downloadBuildArtifactUsingTask = false,
      AutomationEngineInput automationEngineInput = null)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (linkedArtifact == null)
        throw new ArgumentNullException(nameof (linkedArtifact));
      if (customArtifactExtensions == null)
        throw new ArgumentNullException(nameof (customArtifactExtensions));
      if (artifactTasksList == null)
        throw new ArgumentNullException(nameof (artifactTasksList));
      InputValue inputValue;
      int result;
      if (!linkedArtifact.SourceData.TryGetValue("version", out inputValue) || !int.TryParse(inputValue.Value, out result))
        throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.BuildVersionNotFound, (object) linkedArtifact.Alias));
      IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> buildArtifacts = ArtifactTaskMapper.GetBuildArtifacts(requestContext, releaseId, projectId, linkedArtifact, result);
      if (buildArtifacts == null)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No artifact found corresponding to build {0}", (object) result);
        requestContext.Trace(1972333, TraceLevel.Info, "ReleaseManagementService", "ArtifactExtensions", message);
      }
      else
      {
        foreach (Microsoft.TeamFoundation.Build.WebApi.BuildArtifact buildArtifact in (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) buildArtifacts)
        {
          Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact = buildArtifact;
          string resourceData = artifact.Resource?.Data ?? string.Empty;
          resourceData += "/**";
          if (buildArtifactDownloadInput == null || !buildArtifactDownloadInput.DownloadSelectiveArtifacts() || buildArtifactDownloadInput.ArtifactItems.Any<string>((Func<string, bool>) (x => x.Equals(artifact.Name, StringComparison.OrdinalIgnoreCase) || x.Equals(artifact.Name + "/**", StringComparison.OrdinalIgnoreCase) || x.Equals(artifact.Name + "\\**", StringComparison.OrdinalIgnoreCase) || resourceData.EndsWith("/**" + x, StringComparison.OrdinalIgnoreCase) || resourceData.EndsWith("\\**" + x, StringComparison.OrdinalIgnoreCase))))
          {
            if (artifact.Resource == null)
            {
              string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Resource not found corresponding to artifact {0}", (object) artifact.Name);
              requestContext.Trace(1972333, TraceLevel.Info, "ReleaseManagementService", "ArtifactExtensions", message);
            }
            else
            {
              if (string.Equals(artifact.Resource.Type, "Container", StringComparison.OrdinalIgnoreCase) & downloadBuildArtifactUsingTask)
              {
                WorkflowTask buildArtifactTask = ArtifactTaskMapper.GetBuildArtifactTask(requestContext, linkedArtifact, buildArtifactDownloadInput, variables, artifact.Name);
                if (!requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectLatestDownloadBuildArtifactsTask") || requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.FallbackToBuildArtifactsTaskV0") && ArtifactTaskMapper.ShouldFallbackBuildArtifactsTaskV0(requestContext, projectId, variables, automationEngineInput))
                  buildArtifactTask.Version = "0";
                artifactTasksList.Add(buildArtifactTask);
              }
              if (string.Equals(artifact.Resource.Type, "FilePath", StringComparison.OrdinalIgnoreCase) & downloadBuildArtifactUsingTask)
                artifactTasksList.Add(ArtifactTaskMapper.GetFileShareArtifactTask(requestContext, linkedArtifact, buildArtifactDownloadInput, variables, artifact.Resource.Data, artifact.Name));
              if (requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.UseDropArtifactTask") && string.Equals(artifact.Resource.Type, "PipelineArtifact", StringComparison.OrdinalIgnoreCase))
              {
                WorkflowTask pipelineArtifactTask = ArtifactTaskMapper.GetPipelineArtifactTask(requestContext, linkedArtifact, artifact.Name, variables);
                if (!requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectDownloadPipelineArtifactV2"))
                  pipelineArtifactTask.Version = "1";
                artifactTasksList.Add(pipelineArtifactTask);
              }
              if (customArtifactExtensions.FirstOrDefault<CustomArtifact>((Func<CustomArtifact, bool>) (x => string.Equals(x.ArtifactTypeId, artifact.Resource.Type, StringComparison.OrdinalIgnoreCase))) != null)
              {
                if (!requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.ShowServiceConnectionsUsedInLinkedArtifacts"))
                  throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.DownloadOfCustomArtifactsNotSupported, (object) artifact.Name));
                artifactTasksList.Add(ArtifactTaskMapper.GetTaskForBuildArtifactsUsingCustomStorage(requestContext, customArtifactExtensions, linkedArtifact, (IDictionary<string, string>) artifact.Resource.Properties, artifact.Resource.Type, artifact.Name, (ArtifactDownloadInputBase) buildArtifactDownloadInput, variables));
              }
              else
              {
                string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No extension found corresponding to artifact type {0}", (object) artifact.Resource?.Type);
                requestContext.Trace(1972333, TraceLevel.Error, "ReleaseManagementService", "ArtifactExtensions", message);
              }
            }
          }
        }
      }
    }

    private static bool ShouldFallbackBuildArtifactsTaskV0(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      AutomationEngineInput automationEngineInput)
    {
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      DeploymentInput deploymentInput = (DeploymentInput) automationEngineInput.DeployPhaseData.GetDeploymentInput(variables);
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demands = JsonUtility.FromString<List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>(JsonUtility.ToString<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Demand>(deploymentInput.Demands)) ?? new List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
      Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand = demands.Find((Predicate<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) (x => ArtifactTaskMapper.IsAgentVersionDemand(x)));
      if (demand != null)
        demands.Remove(demand);
      Microsoft.TeamFoundation.DistributedTask.WebApi.DemandMinimumVersion demandMinimumVersion = new Microsoft.TeamFoundation.DistributedTask.WebApi.DemandMinimumVersion("Agent.Version", "2.188.2");
      demands.Add((Microsoft.TeamFoundation.DistributedTask.WebApi.Demand) demandMinimumVersion);
      TaskAgentQueue agentQueue = service.GetAgentQueue(requestContext, projectId, deploymentInput.QueueId);
      int num1 = service.GetAgents(requestContext, agentQueue.Pool.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demands).Count<TaskAgent>((Func<TaskAgent, bool>) (agent => agent.Status == TaskAgentStatus.Online && agent.Enabled.GetValueOrDefault()));
      if (num1 == 0)
      {
        TaskAgentPool agentPool = service.GetAgentPool(requestContext, agentQueue.Pool.Id);
        bool flag = agentPool == null || !agentPool.IsHosted;
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fallback to BuildArtifactsTaskV0 is {0}. 0 compatible agents found for higher task version.", flag ? (object) "approved" : (object) "rejected");
        requestContext.Trace(1972334, TraceLevel.Info, "ReleaseManagementService", "ArtifactExtensions", message);
        return flag;
      }
      if (num1 > 1)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fallback to BuildArtifactsTaskV0 is rejected. {0} compatible agents found for higher task version.", (object) num1);
        requestContext.Trace(1972334, TraceLevel.Info, "ReleaseManagementService", "ArtifactExtensions", message);
        return false;
      }
      demands.Remove((Microsoft.TeamFoundation.DistributedTask.WebApi.Demand) demandMinimumVersion);
      if (demand != null)
        demands.Add(demand);
      int num2 = service.GetAgents(requestContext, agentQueue.Pool.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demands).Count<TaskAgent>((Func<TaskAgent, bool>) (agent => agent.Status == TaskAgentStatus.Online && agent.Enabled.GetValueOrDefault()));
      if (num2 > 1)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fallback to BuildArtifactsTaskV0 is approved. Compatible agents count: V0 - {0}, higher version - 1.", (object) num2);
        requestContext.Trace(1972334, TraceLevel.Info, "ReleaseManagementService", "ArtifactExtensions", message);
        return true;
      }
      string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fallback to BuildArtifactsTaskV0 is rejected. Compatible agents count for V0 and higher is 1.");
      requestContext.Trace(1972334, TraceLevel.Info, "ReleaseManagementService", "ArtifactExtensions", message1);
      return false;
    }

    private static bool IsAgentVersionDemand(Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand) => demand is Microsoft.TeamFoundation.DistributedTask.WebApi.DemandMinimumVersion && demand.Name.Equals("Agent.Version", StringComparison.OrdinalIgnoreCase);

    private static IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> GetBuildArtifacts(
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId,
      ArtifactSource linkedArtifact,
      int buildId)
    {
      IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> buildArtifacts = (IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) null;
      Microsoft.TeamFoundation.Framework.Server.PropertyValue propertyValue = ReleasePropertyExtensions.GetReleasePropertyValues(requestContext, releaseId, projectId, (IEnumerable<string>) new List<string>()
      {
        "BuildArtifacts"
      }).FirstOrDefault<Microsoft.TeamFoundation.Framework.Server.PropertyValue>();
      if (propertyValue != null && propertyValue.Value != null)
        JsonUtility.Deserialize<Dictionary<int, IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>>((byte[]) propertyValue.Value).TryGetValue(linkedArtifact.Id, out buildArtifacts);
      else
        buildArtifacts = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Build.BuildArtifact.GetBuildArtifacts(requestContext, linkedArtifact.TeamProjectId, buildId);
      return buildArtifacts;
    }

    public static bool SkipArtifactDownload(
      this ArtifactDownloadInputBase artifactDownloadInput)
    {
      return artifactDownloadInput != null ? artifactDownloadInput.ArtifactDownloadMode.Equals("Skip", StringComparison.OrdinalIgnoreCase) : throw new ArgumentNullException(nameof (artifactDownloadInput));
    }

    public static bool GetSkipArtifactsDownloadValueForAgent(DeploymentInput deploymentInput)
    {
      if (deploymentInput == null)
        throw new ArgumentNullException(nameof (deploymentInput));
      if (deploymentInput.ArtifactsDownloadInput != null && !deploymentInput.ArtifactsDownloadInput.DownloadInputs.IsNullOrEmpty<ArtifactDownloadInputBase>())
      {
        if (deploymentInput.ArtifactsDownloadInput.DownloadInputs.All<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (input => input.SkipArtifactDownload())))
          return true;
        foreach (ArtifactDownloadInputBase downloadInput in (IEnumerable<ArtifactDownloadInputBase>) deploymentInput.ArtifactsDownloadInput.DownloadInputs)
        {
          if (downloadInput.IsNonTaskifiedArtifact())
            return downloadInput.SkipArtifactDownload();
        }
      }
      return deploymentInput.SkipArtifactsDownload;
    }

    private static bool DownloadSelectiveArtifacts(
      this ArtifactDownloadInputBase artifactDownloadInput)
    {
      return artifactDownloadInput.ArtifactDownloadMode.Equals("Selective", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetItemPattern(IList<string> artifactItems) => string.Join("\n", artifactItems.Select<string, string>((Func<string, string>) (x => x.Replace('\\', '/'))));

    private static WorkflowTask GetDeleteArtifactsDirectoryTask(
      AutomationEngineInput automationEngineInput)
    {
      Dictionary<string, string> values = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      values["SourceFolder"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$(system.defaultWorkingDirectory)");
      values["Contents"] = "**";
      bool flag = false;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue configurationVariableValue;
      bool result;
      if (automationEngineInput.Variables.TryGetValue("ContinueOnErrorForAutoInjectedCleanupTask", out configurationVariableValue) && bool.TryParse(configurationVariableValue?.Value, out result))
        flag = result;
      WorkflowTask artifactsDirectoryTask = new WorkflowTask();
      artifactsDirectoryTask.AlwaysRun = false;
      artifactsDirectoryTask.ContinueOnError = flag;
      artifactsDirectoryTask.Enabled = true;
      artifactsDirectoryTask.Name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cleanup artifacts directory");
      artifactsDirectoryTask.TaskId = new Guid("B7E8B412-0437-4065-9371-EDC5881DE25B");
      artifactsDirectoryTask.Version = "*";
      artifactsDirectoryTask.Inputs.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) values);
      return artifactsDirectoryTask;
    }

    private static WorkflowTask GetBuildArtifactTask(
      IVssRequestContext requestContext,
      ArtifactSource artifactSource,
      BuildArtifactDownloadInput artifactDownloadInput,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      string artifactName)
    {
      int num = !requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.UseDownloadPipelineArtifactTaskForServerBuildArtifacts") ? 0 : (requestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0);
      Guid taskId = num != 0 ? new Guid("61F2A582-95AE-4948-B34D-A1B3C4F6A737") : new Guid("a433f589-fce1-4460-9ee6-44a624aeb1fb");
      Dictionary<string, string> taskInputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      taskInputs["project"] = artifactSource.TeamProjectId.ToString();
      if (num == 0)
      {
        taskInputs["buildType"] = "specific";
        taskInputs["definition"] = artifactSource.SourceData["definition"].Value;
        taskInputs["buildId"] = artifactSource.SourceData["version"].Value;
        taskInputs[nameof (artifactName)] = artifactName;
        taskInputs["downloadType"] = "single";
        taskInputs["itemPattern"] = "**";
        taskInputs["buildVersionToDownload"] = "specific";
        if (artifactDownloadInput != null && artifactDownloadInput.DownloadSelectiveArtifacts() && !artifactDownloadInput.ArtifactItems.IsNullOrEmpty<string>())
          taskInputs["itemPattern"] = ArtifactTaskMapper.GetItemPattern(artifactDownloadInput.ArtifactItems);
      }
      else
      {
        taskInputs["source"] = "specific";
        taskInputs["pipeline"] = artifactSource.SourceData["definition"].Value;
        taskInputs["runId"] = artifactSource.SourceData["version"].Value;
        taskInputs["artifact"] = artifactName;
        taskInputs["patterns"] = "**";
        taskInputs["runVersion"] = "specific";
        taskInputs["path"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, artifactName);
        taskInputs["downloadPath"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, artifactName);
      }
      if (variables != null && variables.Keys.Contains("release.artifact.download.parallellimit"))
        taskInputs["parallelizationLimit"] = variables["release.artifact.download.parallellimit"].Value;
      return ArtifactTaskMapper.GetArtifactTask(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1}", (object) artifactSource.Alias, (object) artifactName), artifactSource.Alias, taskId, taskInputs, variables);
    }

    private static WorkflowTask GetFileShareArtifactTask(
      IVssRequestContext requestContext,
      ArtifactSource artifactSource,
      BuildArtifactDownloadInput artifactDownloadInput,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      string location,
      string artifactName)
    {
      int num = !requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.UseDownloadPipelineArtifactTaskForServerBuildArtifacts") ? 0 : (requestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0);
      Guid taskId = num != 0 ? new Guid("61F2A582-95AE-4948-B34D-A1B3C4F6A737") : new Guid("E3CF3806-AD30-4EC4-8F1E-8ECD98771AA0");
      Dictionary<string, string> taskInputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (num == 0)
      {
        taskInputs["filesharePath"] = location;
        taskInputs[nameof (artifactName)] = artifactName;
        taskInputs["itemPattern"] = "**";
        if (variables != null && variables.Keys.Contains("release.artifact.download.parallellimit"))
          taskInputs["parallelizationLimit"] = variables["release.artifact.download.parallellimit"].Value;
        if (artifactDownloadInput != null && artifactDownloadInput.DownloadSelectiveArtifacts() && !artifactDownloadInput.ArtifactItems.IsNullOrEmpty<string>())
          taskInputs["itemPattern"] = ArtifactTaskMapper.GetItemPattern(artifactDownloadInput.ArtifactItems);
      }
      else
        ArtifactTaskMapper.UpdateTaskInputForFileShareArtifactType(taskInputs, artifactSource, artifactName, variables);
      return ArtifactTaskMapper.GetArtifactTask(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1}", (object) artifactSource.Alias, (object) artifactName), artifactSource.Alias, taskId, taskInputs, variables);
    }

    private static void UpdateTaskInputForFileShareArtifactType(
      Dictionary<string, string> taskInputs,
      ArtifactSource artifactSource,
      string artifactName,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      taskInputs["project"] = artifactSource.TeamProjectId.ToString();
      taskInputs["source"] = "specific";
      taskInputs["pipeline"] = artifactSource.SourceData["definition"].Value;
      taskInputs["runId"] = artifactSource.SourceData["version"].Value;
      taskInputs["artifact"] = artifactName;
      taskInputs["patterns"] = "**";
      taskInputs["runVersion"] = "specific";
      taskInputs["path"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, (string) null);
      taskInputs["downloadPath"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, (string) null);
    }

    private static WorkflowTask GetPipelineArtifactTask(
      IVssRequestContext requestContext,
      ArtifactSource artifactSource,
      string artifactName,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      Guid taskId = new Guid("61F2A582-95AE-4948-B34D-A1B3C4F6A737");
      Dictionary<string, string> taskInputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      taskInputs["project"] = artifactSource.TeamProjectId.ToString();
      if (!requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectDownloadPipelineArtifactV2"))
      {
        taskInputs["buildType"] = "specific";
        taskInputs["definition"] = artifactSource.SourceData["definition"].Value;
        taskInputs["buildId"] = artifactSource.SourceData["version"].Value;
        taskInputs[nameof (artifactName)] = artifactName;
        taskInputs["itemPattern"] = "**";
        taskInputs["buildVersionToDownload"] = "specific";
        taskInputs["downloadPath"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, artifactName);
      }
      else
      {
        taskInputs["source"] = "specific";
        taskInputs["pipeline"] = artifactSource.SourceData["definition"].Value;
        taskInputs["runId"] = artifactSource.SourceData["version"].Value;
        taskInputs["artifact"] = artifactName;
        taskInputs["patterns"] = "**";
        taskInputs["runVersion"] = "specific";
        taskInputs["path"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, artifactName);
        taskInputs["downloadPath"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, artifactName);
      }
      return ArtifactTaskMapper.GetArtifactTask(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1}", (object) artifactSource.Alias, (object) artifactName), artifactSource.Alias, taskId, taskInputs, variables);
    }

    private static WorkflowTask GetJenkinsArtifactTask(
      IVssRequestContext requestContext,
      ArtifactSource artifactSource,
      JenkinsArtifactDownloadInput artifactDownloadInput,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      Guid taskId = new Guid("86c37a92-59a7-444b-93c7-220fcf91e29c");
      Dictionary<string, string> taskInputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      taskInputs["serverEndpoint"] = artifactSource.SourceData["connection"].Value;
      taskInputs["jobName"] = artifactSource.SourceData["definition"].Value;
      taskInputs["jenkinsBuild"] = "BuildNumber";
      taskInputs["jenkinsBuildNumber"] = artifactSource.SourceData["version"].Value;
      taskInputs["itemPattern"] = "**";
      taskInputs["saveTo"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$(system.defaultWorkingDirectory)/{0}", (object) artifactSource.Alias);
      if (artifactSource.SourceData.ContainsKey("propagatedArtifacts"))
      {
        taskInputs["propagatedArtifacts"] = artifactSource.SourceData["propagatedArtifacts"].Value;
        taskInputs["ConnectedServiceNameARM"] = artifactSource.SourceData["azureRmEndpoint"].Value;
        taskInputs["storageAccountName"] = artifactSource.SourceData["storageAccount"].Value;
        taskInputs["containerName"] = artifactSource.SourceData["container"].Value;
        taskInputs["commonVirtualPath"] = artifactSource.SourceData["commonVirtualPath"].Value;
        if (artifactSource.SourceData.ContainsKey("version"))
        {
          if (string.IsNullOrWhiteSpace(taskInputs["commonVirtualPath"]))
            taskInputs["commonVirtualPath"] = artifactSource.SourceData["version"].Value;
          else
            taskInputs["commonVirtualPath"] = taskInputs["commonVirtualPath"].TrimEnd('/') + "/" + artifactSource.SourceData["version"].Value;
        }
      }
      if (artifactDownloadInput != null && artifactDownloadInput.DownloadSelectiveArtifacts() && !artifactDownloadInput.ArtifactItems.IsNullOrEmpty<string>())
        taskInputs["itemPattern"] = ArtifactTaskMapper.GetItemPattern(artifactDownloadInput.ArtifactItems);
      ArtifactTaskMapper.UpdateArtifactDetailsInput(requestContext, taskInputs, artifactSource);
      return ArtifactTaskMapper.GetArtifactTask(artifactSource.Alias, artifactSource.Alias, taskId, taskInputs, variables);
    }

    private static void UpdateArtifactDetailsInput(
      IVssRequestContext requestContext,
      Dictionary<string, string> taskInputs,
      ArtifactSource artifactSource)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks") || artifactSource.SourceData.ContainsKey("ArtifactDetailsReference"))
        return;
      taskInputs["downloadCommitsAndWorkItems"] = true.ToString();
      taskInputs["artifactDetailsFileNameSuffix"] = ArtifactTaskMapper.GetCommitsWorkItemsFileNameSuffix(artifactSource);
      taskInputs["startJenkinsBuildNumber"] = ArtifactTaskMapper.GetPreviousArtifactVersion(artifactSource);
    }

    private static string GetCommitsWorkItemsFileNameSuffix(ArtifactSource artifactSource) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_v{1}.json", (object) artifactSource.Alias, (object) 1);

    private static string GetPreviousArtifactVersion(ArtifactSource artifactSource)
    {
      string empty = string.Empty;
      if (artifactSource.SourceData.ContainsKey("PreviousArtifactVersion"))
        empty = artifactSource.SourceData["PreviousArtifactVersion"].Value;
      return empty;
    }

    private static WorkflowTask GetCustomArtifactTask(
      IVssRequestContext requestContext,
      CustomArtifact customArtifact,
      ArtifactSource artifactSource,
      IDictionary<string, string> artifactSourceDataOverrides,
      ArtifactDownloadInputBase artifactDownloadInput,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      string artifactName = null)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ArtifactTaskMapper.PopulateMappedTaskInputs(artifactDownloadInput, artifactSource, artifactSourceDataOverrides, customArtifact, dictionary, variables, artifactName);
      WorkflowTask customArtifactTask = new WorkflowTask()
      {
        AlwaysRun = false,
        ContinueOnError = false,
        Enabled = true,
        Name = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DownloadCustomArtifactTaskNameFormat, (object) artifactSource.Alias, (object) customArtifact.DisplayName),
        TaskId = customArtifact.ArtifactDownloadTaskId,
        Version = "*"
      };
      customArtifactTask.Inputs.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) dictionary);
      if (ReleaseWorkItemsCommitsHelper.DoesArtifactTypeSupportsCommitsOrWorkItemsTraceability(requestContext, customArtifact.ArtifactTypeId))
      {
        customArtifactTask.Inputs.Add("previousVersion", ArtifactTaskMapper.GetPreviousArtifactVersion(artifactSource));
        customArtifactTask.Inputs.Add("artifactDetailsFileNameSuffix", ArtifactTaskMapper.GetCommitsWorkItemsFileNameSuffix(artifactSource));
      }
      return customArtifactTask;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design.")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design.")]
    public static void ValidateTaskInputsForEndpointTypeInputs(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, WorkflowTask> artifactAliasToTaskMap,
      IDictionary<WorkflowTask, CustomArtifact> taskToArtifactExtensionMap,
      IList<ArtifactSource> linkedArtifacts)
    {
      if (artifactAliasToTaskMap == null || artifactAliasToTaskMap.Count == 0 || taskToArtifactExtensionMap == null || taskToArtifactExtensionMap.Count == 0)
        return;
      List<Guid> list = artifactAliasToTaskMap.Select<KeyValuePair<string, WorkflowTask>, Guid>((Func<KeyValuePair<string, WorkflowTask>, Guid>) (x => x.Value.TaskId)).Distinct<Guid>().ToList<Guid>();
      IList<TaskDefinition> definitionsByIds = TaskDefinitionsHelper.GetTaskDefinitionsByIds(requestContext, projectId, true, (IList<Guid>) list);
      foreach (KeyValuePair<string, WorkflowTask> artifactAliasToTask in (IEnumerable<KeyValuePair<string, WorkflowTask>>) artifactAliasToTaskMap)
      {
        KeyValuePair<string, WorkflowTask> artifactAliasTaskPair = artifactAliasToTask;
        Guid taskId = artifactAliasTaskPair.Value.TaskId;
        IEnumerable<TaskDefinition> source1 = definitionsByIds.Where<TaskDefinition>((Func<TaskDefinition, bool>) (task => task.Id.Equals(taskId)));
        TaskDefinition taskDefinition = source1 != null ? source1.OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (task => task.Version)).First<TaskDefinition>() : (TaskDefinition) null;
        IEnumerable<KeyValuePair<WorkflowTask, CustomArtifact>> source2 = taskToArtifactExtensionMap.Where<KeyValuePair<WorkflowTask, CustomArtifact>>((Func<KeyValuePair<WorkflowTask, CustomArtifact>, bool>) (x => x.Key.TaskId.Equals(taskId)));
        CustomArtifact customArtifactExtension = source2 != null ? source2.FirstOrDefault<KeyValuePair<WorkflowTask, CustomArtifact>>().Value : (CustomArtifact) null;
        WorkflowTask artifactTask = artifactAliasTaskPair.Value;
        List<KeyValuePair<string, string>> endpointTypeTaskInputs = new List<KeyValuePair<string, string>>();
        if (taskDefinition != null)
          taskDefinition.Inputs.ForEach<TaskInputDefinition>((Action<TaskInputDefinition>) (x =>
          {
            if (!x.InputType.StartsWith("connectedService:", StringComparison.OrdinalIgnoreCase))
              return;
            endpointTypeTaskInputs.Add(artifactTask.Inputs.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (y => y.Key.Equals(x.Name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<KeyValuePair<string, string>>());
          }));
        IEnumerable<ArtifactSource> source3 = linkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias.Equals(artifactAliasTaskPair.Key, StringComparison.OrdinalIgnoreCase)));
        ArtifactSource artifactSource = source3 != null ? source3.First<ArtifactSource>() : (ArtifactSource) null;
        endpointTypeTaskInputs.ForEach((Action<KeyValuePair<string, string>>) (taskInput =>
        {
          string artifactInputName = artifactSource.SourceData.Where<KeyValuePair<string, InputValue>>((Func<KeyValuePair<string, InputValue>, bool>) (y => taskInput.Value != null && y.Value.Value.Equals(taskInput.Value, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<KeyValuePair<string, InputValue>>().Key;
          if (artifactInputName.IsNullOrEmpty<char>())
            throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.TaskInputPopulatedFromNonEndpointTypeartifactInput, (object) taskInput.Key));
          InputDescriptor artifactInput = customArtifactExtension.InputDescriptors.Single<InputDescriptor>((Func<InputDescriptor, bool>) (z => z.Id.Equals(artifactInputName, StringComparison.OrdinalIgnoreCase)));
          if (artifactInput == null)
            return;
          ArtifactTaskMapper.IsTypeMismatch(taskInput.Key, artifactInput);
        }));
      }
    }

    private static void IsTypeMismatch(string taskInput, InputDescriptor artifactInput)
    {
      if (artifactInput == null)
        return;
      bool flag1 = true;
      if (artifactInput != null)
      {
        bool? nullable = artifactInput.Type?.StartsWith("connectedService:", StringComparison.OrdinalIgnoreCase);
        bool flag2 = true;
        if (nullable.GetValueOrDefault() == flag2 & nullable.HasValue)
          flag1 = false;
      }
      if (flag1 && artifactInput != null && artifactInput.Id.Equals("connection", StringComparison.OrdinalIgnoreCase))
        flag1 = false;
      if (flag1)
        throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.TaskInputTypeMismatch, (object) taskInput));
    }

    private static WorkflowTask GetTaskForBuildArtifactsUsingCustomStorage(
      IVssRequestContext requestContext,
      IList<CustomArtifact> customArtifactExtensions,
      ArtifactSource artifactSource,
      IDictionary<string, string> artifactSourceDataOverrides,
      string artifactType,
      string linkedArtifactName,
      ArtifactDownloadInputBase artifactDownloadInput,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      artifactSource.SourceData = artifactSource.SourceData ?? new Dictionary<string, InputValue>();
      ArtifactTaskMapper.ValidateServiceEndpointsUsedInLinkedCustomArtifacts((IDictionary<string, string>) artifactSource.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, string>((Func<KeyValuePair<string, InputValue>, string>) (x => x.Key), (Func<KeyValuePair<string, InputValue>, string>) (x => x.Value.Value)), artifactSourceDataOverrides, linkedArtifactName);
      CustomArtifact customArtifact = customArtifactExtensions.FirstOrDefault<CustomArtifact>((Func<CustomArtifact, bool>) (s => string.Equals(s.ArtifactTypeId, artifactType, StringComparison.OrdinalIgnoreCase)));
      return ArtifactTaskMapper.GetCustomArtifactTask(requestContext, customArtifact, artifactSource, artifactSourceDataOverrides, artifactDownloadInput, variables, linkedArtifactName);
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "The newly created instance IS being used explicitly.")]
    private static void ValidateServiceEndpointsUsedInLinkedCustomArtifacts(
      IDictionary<string, string> artifactSourceData,
      IDictionary<string, string> artifactSourceDataOverrides,
      string artifactName)
    {
      Guid result1 = Guid.Empty;
      if (artifactSourceDataOverrides.ContainsKey("connection"))
        Guid.TryParse(artifactSourceDataOverrides["connection"], out result1);
      Guid result2 = Guid.Empty;
      if (artifactSourceData.ContainsKey("connection"))
        Guid.TryParse(artifactSourceData["connection"], out result2);
      if (!object.Equals((object) result1, (object) result2))
        throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ServiceConnectionIsNotValid, (object) result2, (object) artifactName));
    }

    private static void PopulateMappedTaskInputs(
      ArtifactDownloadInputBase artifactDownloadInput,
      ArtifactSource artifactSource,
      IDictionary<string, string> artifactSourceDataOverrides,
      CustomArtifact customArtifact,
      Dictionary<string, string> taskInputs,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      string artifactName)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) artifactSource.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, string>((Func<KeyValuePair<string, InputValue>, string>) (x => x.Key), (Func<KeyValuePair<string, InputValue>, string>) (x => x.Value.Value));
      if (artifactSourceDataOverrides != null)
        dictionary.Merge(artifactSourceDataOverrides);
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dictionary)
        taskInputs.Add(keyValuePair.Key, keyValuePair.Value);
      taskInputs["downloadPath"] = ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSource.Alias, artifactName);
      taskInputs["itemPattern"] = "**";
      if (artifactDownloadInput != null && artifactDownloadInput.DownloadSelectiveArtifacts() && !artifactDownloadInput.ArtifactItems.IsNullOrEmpty<string>())
        taskInputs["itemPattern"] = ArtifactTaskMapper.GetItemPattern(artifactDownloadInput.ArtifactItems);
      EndpointStringResolver endpointStringResolver = new EndpointStringResolver(JToken.FromObject((object) taskInputs));
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (customArtifact.TaskInputMapping ?? (IDictionary<string, string>) new Dictionary<string, string>()))
        taskInputs[keyValuePair.Key] = endpointStringResolver.ResolveVariablesInMustacheFormat(keyValuePair.Value);
    }

    private static string GetDownloadPathForArtifacts(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      string artifactSourceAlias,
      string artifactName)
    {
      bool result;
      return ((variables == null || !variables.ContainsKey("System.DownloadArtifactsToRootFolder") ? 0 : (bool.TryParse(variables["System.DownloadArtifactsToRootFolder"].Value, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0})", (object) "system.defaultWorkingDirectory") : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0})/{1}/{2}", (object) "system.defaultWorkingDirectory", (object) artifactSourceAlias, (object) artifactName);
    }

    private static WorkflowTask GetArtifactTask(
      string artifactTaskName,
      string artifactSourceAlias,
      Guid taskId,
      Dictionary<string, string> taskInputs,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      WorkflowTask artifactTask = new WorkflowTask()
      {
        AlwaysRun = false,
        ContinueOnError = false,
        Enabled = true,
        Name = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DownloadArtifactTaskNameFormat, (object) artifactTaskName),
        TaskId = taskId,
        Version = "*"
      };
      if (taskInputs != null)
        artifactTask.Inputs.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) taskInputs);
      if (!artifactTask.Inputs.ContainsKey("downloadPath"))
        artifactTask.Inputs.Add("downloadPath", ArtifactTaskMapper.GetDownloadPathForArtifacts(variables, artifactSourceAlias, (string) null));
      return artifactTask;
    }
  }
}
