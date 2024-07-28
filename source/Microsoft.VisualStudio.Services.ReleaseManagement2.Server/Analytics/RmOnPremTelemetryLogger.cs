// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics.RmOnPremTelemetryLogger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics
{
  public class RmOnPremTelemetryLogger : IRmTelemetryLogger
  {
    private const string TaskInputTypePrefix = "connectedService:";
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    [StaticSafe]
    private static readonly RmOnPremTelemetryLogger instance = new RmOnPremTelemetryLogger();

    [StaticSafe]
    public static RmOnPremTelemetryLogger Instance => RmOnPremTelemetryLogger.instance;

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Telemetry needs to get data from many sources")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Telemetry api shuld not throw")]
    public void PublishReleaseCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Guid projectId)
    {
      if (requestContext == null)
        return;
      if (release == null)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> source1 = new List<RegistryEntry>();
        Dictionary<string, int> source2 = new Dictionary<string, int>();
        CachedRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = vssRequestContext;
        RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData;
        ref RegistryQuery local1 = ref registryQuery;
        if (!registryService1.GetValue<bool>(requestContext1, in local1, false))
          return;
        CachedRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = vssRequestContext;
        registryQuery = (RegistryQuery) ReleaseServerTelemetryConstants.ReleasesCountPerDayRegistryKey;
        ref RegistryQuery local2 = ref registryQuery;
        int num1 = registryService2.GetValue<int>(requestContext2, in local2, 0) + 1;
        source1.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleasesCountPerDayRegistryKey, num1.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) release.Environments)
        {
          JArray jarray = JArray.Parse(environment.GetCompatWorkflow());
          CachedRegistryService registryService3 = service;
          IVssRequestContext requestContext3 = vssRequestContext;
          registryQuery = (RegistryQuery) ReleaseServerTelemetryConstants.ReleaseTotalTasksTelemetryRegistryKey;
          ref RegistryQuery local3 = ref registryQuery;
          int num2 = registryService3.GetValue<int>(requestContext3, in local3, 0) + jarray.Count;
          source1.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleaseTotalTasksTelemetryRegistryKey, num2.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          for (JToken jtoken = jarray.First; jtoken != null; jtoken = jtoken.Next)
          {
            foreach (JProperty jproperty in (IEnumerable<JToken>) jtoken)
            {
              if (jproperty.Name.Equals("TaskId"))
              {
                JToken taskId = jproperty.Value;
                int num3 = source2.FirstOrDefault<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (x => x.Key.Equals(ReleaseServerTelemetryConstants.ReleaseTasksTelemetryRegistryRoot + ((JValue) taskId).Value?.ToString()))).Value;
                if (num3 == 0)
                {
                  CachedRegistryService registryService4 = service;
                  IVssRequestContext requestContext4 = vssRequestContext;
                  registryQuery = (RegistryQuery) (ReleaseServerTelemetryConstants.ReleaseTasksTelemetryRegistryRoot + ((JValue) taskId).Value?.ToString());
                  ref RegistryQuery local4 = ref registryQuery;
                  int num4 = registryService4.GetValue<int>(requestContext4, in local4, 0) + 1;
                  source1.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleaseTasksTelemetryRegistryRoot + ((JValue) taskId).Value?.ToString(), num4.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
                  source2.Add(ReleaseServerTelemetryConstants.ReleaseTasksTelemetryRegistryRoot + ((JValue) taskId).Value?.ToString(), num4);
                }
                else
                {
                  int num5 = num3 + 1;
                  RegistryEntry registryEntry = source1.FirstOrDefault<RegistryEntry>((Func<RegistryEntry, bool>) (e => e.Path.Equals(ReleaseServerTelemetryConstants.ReleaseTasksTelemetryRegistryRoot + ((JValue) taskId).Value?.ToString())));
                  if (registryEntry != null)
                    registryEntry.Value = num5.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                  source2[ReleaseServerTelemetryConstants.ReleaseTasksTelemetryRegistryRoot + ((JValue) taskId).Value?.ToString()] = num5;
                }
              }
            }
          }
        }
        int num6 = service.GetValue<int>(vssRequestContext, (RegistryQuery) ReleaseServerTelemetryConstants.ReleaseArtifactsCountPerDayRegistryKey, 0) + release.LinkedArtifacts.Count;
        source1.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleaseArtifactsCountPerDayRegistryKey, num6.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) source1);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972107, TraceLevel.Info, "ReleaseManagementService", "Analytics", "Exception in PublishReleaseCreated. details {0}", (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "complex object")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Telemetry api shuld not throw")]
    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "It requires to be in lower case")]
    public void PublishEventsOnEnvironmentCompletion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      EnvironmentStatus status,
      Guid projectId,
      string message,
      Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int, Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>> getChangesForEnvironment)
    {
      if (requestContext == null)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        CachedRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = vssRequestContext;
        RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData;
        ref RegistryQuery local1 = ref registryQuery;
        if (!registryService1.GetValue<bool>(requestContext1, in local1, false))
          return;
        CachedRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = vssRequestContext;
        registryQuery = (RegistryQuery) ReleaseServerTelemetryConstants.EnvironmentsCountPerDayRegistryKey;
        ref RegistryQuery local2 = ref registryQuery;
        int num1 = registryService2.GetValue<int>(requestContext2, in local2, 0) + 1;
        registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.EnvironmentsCountPerDayRegistryKey, num1.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        CachedRegistryService registryService3 = service;
        IVssRequestContext requestContext3 = vssRequestContext;
        registryQuery = (RegistryQuery) (ReleaseServerTelemetryConstants.ReleaseEnvironmentStatusRegistryRoot + status.ToString());
        ref RegistryQuery local3 = ref registryQuery;
        int num2 = registryService3.GetValue<int>(requestContext3, in local3, 0) + 1;
        registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleaseEnvironmentStatusRegistryRoot + status.ToString(), num2.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        if (releaseEnvironment != null)
        {
          if (releaseEnvironment.DeploymentSnapshot is DesignerDeploymentSnapshot deploymentSnapshot)
          {
            foreach (string str in deploymentSnapshot.DeployPhaseSnapshots.SelectMany<DeployPhaseSnapshot, WorkflowTask>((Func<DeployPhaseSnapshot, IEnumerable<WorkflowTask>>) (phase => (IEnumerable<WorkflowTask>) phase.Workflow)).Where<WorkflowTask>((Func<WorkflowTask, bool>) (x => x.Enabled)).GetUniqueEndpointTypesInUse(requestContext, releaseEnvironment.ProcessParameters))
            {
              string registryPath = ReleaseServerTelemetryConstants.EnvironmentsWithEndpointCountRegistryRoot + str;
              CachedRegistryService registryService4 = service;
              IVssRequestContext requestContext4 = vssRequestContext;
              registryQuery = (RegistryQuery) registryPath;
              ref RegistryQuery local4 = ref registryQuery;
              int num3 = registryService4.GetValue<int>(requestContext4, in local4, 0) + 1;
              registryEntryList.Add(new RegistryEntry(registryPath, num3.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
            }
          }
          CachedRegistryService registryService5 = service;
          IVssRequestContext requestContext5 = vssRequestContext;
          registryQuery = (RegistryQuery) ReleaseServerTelemetryConstants.AzureSubscriptionsUsedPerDayRegistryKey;
          ref RegistryQuery local5 = ref registryQuery;
          string empty = string.Empty;
          string str1 = registryService5.GetValue<string>(requestContext5, in local5, empty);
          string subscriptionsInUse = releaseEnvironment.GetEnabledReleaseEnvironmentTasks().GetUniqueAzureSubscriptionsInUse(requestContext, releaseEnvironment.ProcessParameters, projectId);
          registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.AzureSubscriptionsUsedPerDayRegistryKey, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", (object) str1, (object) subscriptionsInUse).Trim(',')));
          if (currentRelease != null)
          {
            IList<DeployPhaseSnapshot> deployPhaseSnapshots = releaseEnvironment.GetDesignerDeployPhaseSnapshots(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment);
            Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary1 = VariableGroupsMerger.MergeVariableGroups(currentRelease.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
            {
              Value = p.Value.Value,
              IsSecret = p.Value.IsSecret
            }));
            IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary2 = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>[2]
            {
              currentRelease.Variables,
              (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) dictionary1
            });
            IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>[3]
            {
              (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) releaseEnvironment.ProcessParameters.GetProcessParametersAsDataModelVariables(),
              releaseEnvironment.Variables,
              dictionary2
            });
            HashSet<string> source = new HashSet<string>();
            foreach (DeployPhaseSnapshot deployPhaseSnapshot in (IEnumerable<DeployPhaseSnapshot>) deployPhaseSnapshots)
            {
              if (deployPhaseSnapshot.GetDeploymentInput(variables) is MachineGroupDeploymentInput deploymentInput)
              {
                int queueId = deploymentInput.QueueId;
                foreach (DeploymentMachine withTagsAndProperty in RmOnPremTelemetryLogger.GetMachinesWithTagsAndProperties(requestContext, projectId, queueId, deploymentInput.Tags))
                {
                  string str2;
                  if (withTagsAndProperty.Properties.TryGetValue<string>("AzureSubscriptionId", out str2))
                    source.Add(str2.ToLowerInvariant());
                }
              }
            }
            string str3 = service.GetValue<string>(vssRequestContext, (RegistryQuery) ReleaseServerTelemetryConstants.AzureSubscriptionsInDeploymentTargetsUsedPerDayRegistryKey, string.Empty);
            string separatedIdsString = RmOnPremTelemetryLogger.GetCommaSeparatedIdsString((IList<string>) source.ToList<string>());
            registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.AzureSubscriptionsInDeploymentTargetsUsedPerDayRegistryKey, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", (object) str3, (object) separatedIdsString).Trim(',')));
          }
        }
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972107, TraceLevel.Info, "ReleaseManagementService", "Analytics", "Exception in PublishEnvironmentCompleted. details {0}", (object) ex);
      }
    }

    private static IEnumerable<DeploymentMachine> GetMachinesWithTagsAndProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<string> tags)
    {
      return (IEnumerable<DeploymentMachine>) requestContext.GetService<IDistributedTaskPoolService>().GetDeploymentMachines(requestContext, projectId, machineGroupId, tags, enabled: new bool?(true), propertyFilters: (IList<string>) new List<string>()
      {
        "AzureSubscriptionId"
      });
    }

    private static string GetCommaSeparatedIdsString(IList<string> azureSubscriptionIds)
    {
      string idsString = string.Empty;
      azureSubscriptionIds.ToList<string>().ForEach((Action<string>) (x => idsString = idsString + x.ToString() + ","));
      idsString = idsString.TrimEnd(',');
      return idsString;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Telemetry api shuld not throw")]
    public void PublishDefinitionCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (requestContext == null)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        CachedRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = vssRequestContext;
        RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData;
        ref RegistryQuery local1 = ref registryQuery;
        if (!registryService1.GetValue<bool>(requestContext1, in local1, false))
          return;
        CachedRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = vssRequestContext;
        registryQuery = (RegistryQuery) ReleaseServerTelemetryConstants.ReleaseDefinitionsCreatedCountPerDayRegistryKey;
        ref RegistryQuery local2 = ref registryQuery;
        int num = registryService2.GetValue<int>(requestContext2, in local2, 0) + 1;
        registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleaseDefinitionsCreatedCountPerDayRegistryKey, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972107, TraceLevel.Info, "ReleaseManagementService", "Analytics", "Exception in PublishDefinitionCreated. details {0}", (object) ex);
      }
    }

    public void PublishDefinitionDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId)
    {
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Telemetry api shuld not throw")]
    public void PublishReleaseCompleted(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (requestContext == null)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        CachedRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = vssRequestContext;
        RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData;
        ref RegistryQuery local1 = ref registryQuery;
        if (!registryService1.GetValue<bool>(requestContext1, in local1, false))
          return;
        CachedRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = vssRequestContext;
        registryQuery = (RegistryQuery) ReleaseServerTelemetryConstants.ReleasesCompletedPerDayRegistryKey;
        ref RegistryQuery local2 = ref registryQuery;
        int num = registryService2.GetValue<int>(requestContext2, in local2, 0) + 1;
        registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleasesCompletedPerDayRegistryKey, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972107, TraceLevel.Info, "ReleaseManagementService", "Analytics", "Exception in PublishReleaseCompleted. details {0}", (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Telemetry api shuld not throw")]
    public void PublishReleaseGetByUser(
      IVssRequestContext requestContext,
      int releaseId,
      Guid userId,
      Guid userCuid)
    {
    }

    public void PublishUpdateRetainBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      string jobResult)
    {
    }

    public void PublishRunPlanCompleted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int releaseId,
      Guid projectId,
      TimelineRecord jobTimelineRecord,
      bool timelineRecordsPassed)
    {
    }

    public void PublishWorkflowFailedEvent(
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      string message)
    {
    }

    public void PublishPlanGroupsStartedEvent(
      IVssRequestContext requestContext,
      IEnumerable<TaskOrchestrationPlanGroupReference> planGroupReferences)
    {
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Telemetry api shuld not throw")]
    public void PublishDefinitionUpdated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (requestContext == null)
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        CachedRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = vssRequestContext;
        RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData;
        ref RegistryQuery local1 = ref registryQuery;
        if (!registryService1.GetValue<bool>(requestContext1, in local1, false))
          return;
        CachedRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = vssRequestContext;
        registryQuery = (RegistryQuery) ReleaseServerTelemetryConstants.ReleaseDefinitionsUpdateCountPerDayRegistryKey;
        ref RegistryQuery local2 = ref registryQuery;
        int num = registryService2.GetValue<int>(requestContext2, in local2, 0) + 1;
        registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleaseDefinitionsUpdateCountPerDayRegistryKey, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972107, TraceLevel.Info, "ReleaseManagementService", "Analytics", "Exception in PublishDefinitionUpdated. details {0}", (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Telemetry api shuld not throw")]
    public void PublishApprovalUpdated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> approvals)
    {
      if (requestContext == null || approvals == null)
        return;
      if (!approvals.Any<ReleaseEnvironmentStep>())
        return;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false))
          return;
        int num = service.GetValue<int>(vssRequestContext, (RegistryQuery) ReleaseServerTelemetryConstants.ReleaseApprovalsCountPerDayRegistryKey, 0) + approvals.Count<ReleaseEnvironmentStep>();
        registryEntryList.Add(new RegistryEntry(ReleaseServerTelemetryConstants.ReleaseApprovalsCountPerDayRegistryKey, num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972107, TraceLevel.Info, "ReleaseManagementService", "Analytics", "Exception in PublishApprovalUpdated. details {0}", (object) ex);
      }
    }

    public void PublishRevalidateApprovalIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      int environmentId,
      DeploymentAuthorizationInfo approverInfo)
    {
    }
  }
}
