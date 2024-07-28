// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.RosarioHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  public static class RosarioHelper
  {
    internal static AgentStatus Convert(AgentStatus2010 status)
    {
      AgentStatus agentStatus;
      switch (status)
      {
        case AgentStatus2010.Unavailable:
          agentStatus = AgentStatus.Unavailable;
          break;
        case AgentStatus2010.Available:
          agentStatus = AgentStatus.Available;
          break;
        case AgentStatus2010.Offline:
          agentStatus = AgentStatus.Offline;
          break;
        default:
          agentStatus = AgentStatus.Unavailable;
          break;
      }
      return agentStatus;
    }

    internal static AgentStatus2010 Convert(AgentStatus status)
    {
      AgentStatus2010 agentStatus2010;
      switch (status)
      {
        case AgentStatus.Unavailable:
          agentStatus2010 = AgentStatus2010.Unavailable;
          break;
        case AgentStatus.Available:
          agentStatus2010 = AgentStatus2010.Available;
          break;
        case AgentStatus.Offline:
          agentStatus2010 = AgentStatus2010.Offline;
          break;
        default:
          agentStatus2010 = AgentStatus2010.Unavailable;
          break;
      }
      return agentStatus2010;
    }

    internal static BuildAgentUpdate Convert(BuildAgentUpdate2010 update)
    {
      BuildAgentUpdate buildAgentUpdate = BuildAgentUpdate.None;
      if ((update & BuildAgentUpdate2010.BuildDirectory) == BuildAgentUpdate2010.BuildDirectory)
        buildAgentUpdate |= BuildAgentUpdate.BuildDirectory;
      if ((update & BuildAgentUpdate2010.ControllerUri) == BuildAgentUpdate2010.ControllerUri)
        buildAgentUpdate |= BuildAgentUpdate.ControllerUri;
      if ((update & BuildAgentUpdate2010.Description) == BuildAgentUpdate2010.Description)
        buildAgentUpdate |= BuildAgentUpdate.Description;
      if ((update & BuildAgentUpdate2010.Enabled) == BuildAgentUpdate2010.Enabled)
        buildAgentUpdate |= BuildAgentUpdate.Enabled;
      if ((update & BuildAgentUpdate2010.Name) == BuildAgentUpdate2010.Name)
        buildAgentUpdate |= BuildAgentUpdate.Name;
      if ((update & BuildAgentUpdate2010.Status) == BuildAgentUpdate2010.Status)
        buildAgentUpdate |= BuildAgentUpdate.Status;
      if ((update & BuildAgentUpdate2010.StatusMessage) == BuildAgentUpdate2010.StatusMessage)
        buildAgentUpdate |= BuildAgentUpdate.StatusMessage;
      if ((update & BuildAgentUpdate2010.Tags) == BuildAgentUpdate2010.Tags)
        buildAgentUpdate |= BuildAgentUpdate.Tags;
      return buildAgentUpdate;
    }

    internal static BuildControllerUpdate Convert(BuildControllerUpdate2010 update)
    {
      BuildControllerUpdate controllerUpdate = BuildControllerUpdate.None;
      if ((update & BuildControllerUpdate2010.CustomAssemblyPath) == BuildControllerUpdate2010.CustomAssemblyPath)
        controllerUpdate |= BuildControllerUpdate.CustomAssemblyPath;
      if ((update & BuildControllerUpdate2010.Description) == BuildControllerUpdate2010.Description)
        controllerUpdate |= BuildControllerUpdate.Description;
      if ((update & BuildControllerUpdate2010.Enabled) == BuildControllerUpdate2010.Enabled)
        controllerUpdate |= BuildControllerUpdate.Enabled;
      if ((update & BuildControllerUpdate2010.MaxConcurrentBuilds) == BuildControllerUpdate2010.MaxConcurrentBuilds)
        controllerUpdate |= BuildControllerUpdate.MaxConcurrentBuilds;
      if ((update & BuildControllerUpdate2010.Name) == BuildControllerUpdate2010.Name)
        controllerUpdate |= BuildControllerUpdate.Name;
      if ((update & BuildControllerUpdate2010.Status) == BuildControllerUpdate2010.Status)
        controllerUpdate |= BuildControllerUpdate.Status;
      if ((update & BuildControllerUpdate2010.StatusMessage) == BuildControllerUpdate2010.StatusMessage)
        controllerUpdate |= BuildControllerUpdate.StatusMessage;
      return controllerUpdate;
    }

    internal static Microsoft.TeamFoundation.Build.Server.BuildStatus Convert(BuildStatus2010 status)
    {
      Microsoft.TeamFoundation.Build.Server.BuildStatus buildStatus = Microsoft.TeamFoundation.Build.Server.BuildStatus.None;
      if ((status & BuildStatus2010.All) == BuildStatus2010.All)
        buildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.All;
      if ((status & BuildStatus2010.Failed) == BuildStatus2010.Failed)
        buildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed;
      if ((status & BuildStatus2010.InProgress) == BuildStatus2010.InProgress)
        buildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress;
      if ((status & BuildStatus2010.NotStarted) == BuildStatus2010.NotStarted)
        buildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted;
      if ((status & BuildStatus2010.PartiallySucceeded) == BuildStatus2010.PartiallySucceeded)
        buildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded;
      if ((status & BuildStatus2010.Stopped) == BuildStatus2010.Stopped)
        buildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped;
      if ((status & BuildStatus2010.Succeeded) == BuildStatus2010.Succeeded)
        buildStatus |= Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded;
      return buildStatus;
    }

    internal static ControllerStatus Convert(ControllerStatus2010 status)
    {
      ControllerStatus controllerStatus;
      switch (status)
      {
        case ControllerStatus2010.Unavailable:
          controllerStatus = ControllerStatus.Unavailable;
          break;
        case ControllerStatus2010.Available:
          controllerStatus = ControllerStatus.Available;
          break;
        case ControllerStatus2010.Offline:
          controllerStatus = ControllerStatus.Offline;
          break;
        default:
          controllerStatus = ControllerStatus.Unavailable;
          break;
      }
      return controllerStatus;
    }

    internal static ControllerStatus2010 Convert(ControllerStatus status)
    {
      ControllerStatus2010 controllerStatus2010;
      switch (status)
      {
        case ControllerStatus.Unavailable:
          controllerStatus2010 = ControllerStatus2010.Unavailable;
          break;
        case ControllerStatus.Available:
          controllerStatus2010 = ControllerStatus2010.Available;
          break;
        case ControllerStatus.Offline:
          controllerStatus2010 = ControllerStatus2010.Offline;
          break;
        default:
          controllerStatus2010 = ControllerStatus2010.Unavailable;
          break;
      }
      return controllerStatus2010;
    }

    internal static ContinuousIntegrationType Convert(DefinitionTriggerType trigger)
    {
      ContinuousIntegrationType continuousIntegrationType = (ContinuousIntegrationType) 0;
      if ((trigger & DefinitionTriggerType.All) == DefinitionTriggerType.All)
        continuousIntegrationType |= ContinuousIntegrationType.All;
      if ((trigger & DefinitionTriggerType.BatchedContinuousIntegration) == DefinitionTriggerType.BatchedContinuousIntegration)
        continuousIntegrationType |= ContinuousIntegrationType.Batch;
      if ((trigger & DefinitionTriggerType.BatchedGatedCheckIn) == DefinitionTriggerType.BatchedGatedCheckIn)
        continuousIntegrationType |= ContinuousIntegrationType.Gated;
      if ((trigger & DefinitionTriggerType.ContinuousIntegration) == DefinitionTriggerType.ContinuousIntegration)
        continuousIntegrationType |= ContinuousIntegrationType.Individual;
      if ((trigger & DefinitionTriggerType.GatedCheckIn) == DefinitionTriggerType.GatedCheckIn)
        continuousIntegrationType |= ContinuousIntegrationType.Gated;
      if ((trigger & DefinitionTriggerType.None) == DefinitionTriggerType.None)
        continuousIntegrationType |= ContinuousIntegrationType.None;
      if ((trigger & DefinitionTriggerType.Schedule) == DefinitionTriggerType.Schedule)
        continuousIntegrationType |= ContinuousIntegrationType.Schedule;
      if ((trigger & DefinitionTriggerType.ScheduleForced) == DefinitionTriggerType.ScheduleForced)
        continuousIntegrationType |= ContinuousIntegrationType.ScheduleForced;
      return continuousIntegrationType;
    }

    internal static DefinitionTriggerType Convert(ContinuousIntegrationType trigger)
    {
      DefinitionTriggerType definitionTriggerType = (DefinitionTriggerType) 0;
      if ((trigger & ContinuousIntegrationType.All) == ContinuousIntegrationType.All)
        definitionTriggerType |= DefinitionTriggerType.All;
      if ((trigger & ContinuousIntegrationType.Batch) == ContinuousIntegrationType.Batch)
        definitionTriggerType |= DefinitionTriggerType.BatchedContinuousIntegration;
      if ((trigger & ContinuousIntegrationType.Gated) == ContinuousIntegrationType.Gated)
        definitionTriggerType |= DefinitionTriggerType.GatedCheckIn;
      if ((trigger & ContinuousIntegrationType.Individual) == ContinuousIntegrationType.Individual)
        definitionTriggerType |= DefinitionTriggerType.ContinuousIntegration;
      if ((trigger & ContinuousIntegrationType.None) == ContinuousIntegrationType.None)
        definitionTriggerType |= DefinitionTriggerType.None;
      if ((trigger & ContinuousIntegrationType.Schedule) == ContinuousIntegrationType.Schedule)
        definitionTriggerType |= DefinitionTriggerType.Schedule;
      if ((trigger & ContinuousIntegrationType.ScheduleForced) == ContinuousIntegrationType.ScheduleForced)
        definitionTriggerType |= DefinitionTriggerType.ScheduleForced;
      return definitionTriggerType;
    }

    internal static BuildPhaseStatus2010 Convert(BuildPhaseStatus status)
    {
      BuildPhaseStatus2010 buildPhaseStatus2010 = BuildPhaseStatus2010.Unknown;
      switch (status)
      {
        case BuildPhaseStatus.Failed:
          buildPhaseStatus2010 = BuildPhaseStatus2010.Failed;
          break;
        case BuildPhaseStatus.Succeeded:
          buildPhaseStatus2010 = BuildPhaseStatus2010.Succeeded;
          break;
      }
      return buildPhaseStatus2010;
    }

    internal static BuildReason Convert(BuildReason2010 reason)
    {
      BuildReason buildReason = BuildReason.None;
      if ((reason & BuildReason2010.All) == BuildReason2010.All)
        buildReason |= BuildReason.All;
      if ((reason & BuildReason2010.BatchedCI) == BuildReason2010.BatchedCI)
        buildReason |= BuildReason.BatchedCI;
      if ((reason & BuildReason2010.CheckInShelveset) == BuildReason2010.CheckInShelveset)
        buildReason |= BuildReason.CheckInShelveset;
      if ((reason & BuildReason2010.IndividualCI) == BuildReason2010.IndividualCI)
        buildReason |= BuildReason.IndividualCI;
      if ((reason & BuildReason2010.Manual) == BuildReason2010.Manual)
        buildReason |= BuildReason.Manual;
      if ((reason & BuildReason2010.Schedule) == BuildReason2010.Schedule)
        buildReason |= BuildReason.Schedule;
      if ((reason & BuildReason2010.ScheduleForced) == BuildReason2010.ScheduleForced)
        buildReason |= BuildReason.ScheduleForced;
      if ((reason & BuildReason2010.Triggered) == BuildReason2010.Triggered)
        buildReason |= BuildReason.Triggered;
      if ((reason & BuildReason2010.UserCreated) == BuildReason2010.UserCreated)
        buildReason |= BuildReason.UserCreated;
      if ((reason & BuildReason2010.ValidateShelveset) == BuildReason2010.ValidateShelveset)
        buildReason |= BuildReason.ValidateShelveset;
      return buildReason;
    }

    internal static BuildReason2010 Convert(BuildReason reason)
    {
      BuildReason2010 buildReason2010 = BuildReason2010.None;
      if ((reason & BuildReason.All) == BuildReason.All)
        buildReason2010 |= BuildReason2010.All;
      if ((reason & BuildReason.BatchedCI) == BuildReason.BatchedCI)
        buildReason2010 |= BuildReason2010.BatchedCI;
      if ((reason & BuildReason.CheckInShelveset) == BuildReason.CheckInShelveset)
        buildReason2010 |= BuildReason2010.CheckInShelveset;
      if ((reason & BuildReason.IndividualCI) == BuildReason.IndividualCI)
        buildReason2010 |= BuildReason2010.IndividualCI;
      if ((reason & BuildReason.Manual) == BuildReason.Manual)
        buildReason2010 |= BuildReason2010.Manual;
      if ((reason & BuildReason.Schedule) == BuildReason.Schedule)
        buildReason2010 |= BuildReason2010.Schedule;
      if ((reason & BuildReason.ScheduleForced) == BuildReason.ScheduleForced)
        buildReason2010 |= BuildReason2010.ScheduleForced;
      if ((reason & BuildReason.Triggered) == BuildReason.Triggered)
        buildReason2010 |= BuildReason2010.Triggered;
      if ((reason & BuildReason.UserCreated) == BuildReason.UserCreated)
        buildReason2010 |= BuildReason2010.UserCreated;
      if ((reason & BuildReason.ValidateShelveset) == BuildReason.ValidateShelveset)
        buildReason2010 |= BuildReason2010.ValidateShelveset;
      return buildReason2010;
    }

    internal static BuildStatus2010 Convert(Microsoft.TeamFoundation.Build.Server.BuildStatus status)
    {
      BuildStatus2010 buildStatus2010 = BuildStatus2010.None;
      if ((status & Microsoft.TeamFoundation.Build.Server.BuildStatus.All) == Microsoft.TeamFoundation.Build.Server.BuildStatus.All)
        buildStatus2010 |= BuildStatus2010.All;
      if ((status & Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed) == Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed)
        buildStatus2010 |= BuildStatus2010.Failed;
      if ((status & Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress) == Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress)
        buildStatus2010 |= BuildStatus2010.InProgress;
      if ((status & Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted) == Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted)
        buildStatus2010 |= BuildStatus2010.NotStarted;
      if ((status & Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded) == Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded)
        buildStatus2010 |= BuildStatus2010.PartiallySucceeded;
      if ((status & Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped) == Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped)
        buildStatus2010 |= BuildStatus2010.Stopped;
      if ((status & Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded) == Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded)
        buildStatus2010 |= BuildStatus2010.Succeeded;
      return buildStatus2010;
    }

    internal static GetOption2010 Convert(GetOption options)
    {
      GetOption2010 getOption2010 = GetOption2010.Custom;
      switch (options)
      {
        case GetOption.LatestOnQueue:
          getOption2010 = GetOption2010.LatestOnQueue;
          break;
        case GetOption.LatestOnBuild:
          getOption2010 = GetOption2010.LatestOnBuild;
          break;
      }
      return getOption2010;
    }

    internal static GetOption Convert(GetOption2010 options)
    {
      GetOption getOption = GetOption.Custom;
      switch (options)
      {
        case GetOption2010.LatestOnQueue:
          getOption = GetOption.LatestOnQueue;
          break;
        case GetOption2010.LatestOnBuild:
          getOption = GetOption.LatestOnBuild;
          break;
      }
      return getOption;
    }

    internal static InformationEditOptions Convert(InformationEditOptions2010 options)
    {
      InformationEditOptions informationEditOptions = InformationEditOptions.MergeFields;
      if (options == InformationEditOptions2010.ReplaceFields)
        informationEditOptions = InformationEditOptions.ReplaceFields;
      return informationEditOptions;
    }

    internal static QueryOptions Convert(QueryOptions2010 options)
    {
      QueryOptions queryOptions = QueryOptions.None;
      if ((options & QueryOptions2010.Agents) == QueryOptions2010.Agents)
        queryOptions |= QueryOptions.Agents;
      if ((options & QueryOptions2010.All) == QueryOptions2010.All)
        queryOptions |= QueryOptions.All;
      if ((options & QueryOptions2010.Controllers) == QueryOptions2010.Controllers)
        queryOptions |= QueryOptions.Controllers;
      if ((options & QueryOptions2010.Definitions) == QueryOptions2010.Definitions)
        queryOptions |= QueryOptions.Definitions;
      if ((options & QueryOptions2010.Process) == QueryOptions2010.Process)
        queryOptions |= QueryOptions.Process;
      if ((options & QueryOptions2010.Workspaces) == QueryOptions2010.Workspaces)
        queryOptions |= QueryOptions.Workspaces;
      return queryOptions;
    }

    internal static QueuePriority2010 Convert(QueuePriority priority)
    {
      QueuePriority2010 queuePriority2010 = QueuePriority2010.Normal;
      switch (priority)
      {
        case QueuePriority.High:
          queuePriority2010 = QueuePriority2010.High;
          break;
        case QueuePriority.AboveNormal:
          queuePriority2010 = QueuePriority2010.AboveNormal;
          break;
        case QueuePriority.BelowNormal:
          queuePriority2010 = QueuePriority2010.BelowNormal;
          break;
        case QueuePriority.Low:
          queuePriority2010 = QueuePriority2010.Low;
          break;
      }
      return queuePriority2010;
    }

    internal static QueuePriority Convert(QueuePriority2010 priority)
    {
      QueuePriority queuePriority = QueuePriority.Normal;
      switch (priority)
      {
        case QueuePriority2010.High:
          queuePriority = QueuePriority.High;
          break;
        case QueuePriority2010.AboveNormal:
          queuePriority = QueuePriority.AboveNormal;
          break;
        case QueuePriority2010.BelowNormal:
          queuePriority = QueuePriority.BelowNormal;
          break;
        case QueuePriority2010.Low:
          queuePriority = QueuePriority.Low;
          break;
      }
      return queuePriority;
    }

    internal static QueueStatus2010 Convert(QueueStatus status)
    {
      QueueStatus2010 queueStatus2010 = QueueStatus2010.None;
      if ((status & QueueStatus.All) == QueueStatus.All)
        queueStatus2010 |= QueueStatus2010.All;
      if ((status & QueueStatus.Canceled) == QueueStatus.Canceled)
        queueStatus2010 |= QueueStatus2010.Canceled;
      if ((status & QueueStatus.Completed) == QueueStatus.Completed)
        queueStatus2010 |= QueueStatus2010.Completed;
      if ((status & QueueStatus.InProgress) == QueueStatus.InProgress)
        queueStatus2010 |= QueueStatus2010.InProgress;
      if ((status & QueueStatus.Postponed) == QueueStatus.Postponed)
        queueStatus2010 |= QueueStatus2010.Postponed;
      if ((status & QueueStatus.Queued) == QueueStatus.Queued)
        queueStatus2010 |= QueueStatus2010.Queued;
      if ((status & QueueStatus.Retry) == QueueStatus.Retry)
        queueStatus2010 |= QueueStatus2010.InProgress;
      return queueStatus2010;
    }

    internal static QueueStatus Convert(QueueStatus2010 status)
    {
      QueueStatus queueStatus = QueueStatus.None;
      if ((status & QueueStatus2010.All) == QueueStatus2010.All)
        queueStatus |= QueueStatus.All;
      if ((status & QueueStatus2010.Canceled) == QueueStatus2010.Canceled)
        queueStatus |= QueueStatus.Canceled;
      if ((status & QueueStatus2010.Completed) == QueueStatus2010.Completed)
        queueStatus |= QueueStatus.Completed;
      if ((status & QueueStatus2010.InProgress) == QueueStatus2010.InProgress)
        queueStatus |= QueueStatus.InProgress;
      if ((status & QueueStatus2010.Postponed) == QueueStatus2010.Postponed)
        queueStatus |= QueueStatus.Postponed;
      if ((status & QueueStatus2010.Queued) == QueueStatus2010.Queued)
        queueStatus |= QueueStatus.Queued;
      return queueStatus;
    }

    internal static QueuedBuildUpdate Convert(QueuedBuildUpdate2010 update)
    {
      QueuedBuildUpdate queuedBuildUpdate = QueuedBuildUpdate.None;
      if ((update & QueuedBuildUpdate2010.Postponed) == QueuedBuildUpdate2010.Postponed)
        queuedBuildUpdate |= QueuedBuildUpdate.Postponed;
      if ((update & QueuedBuildUpdate2010.Priority) == QueuedBuildUpdate2010.Priority)
        queuedBuildUpdate |= QueuedBuildUpdate.Priority;
      return queuedBuildUpdate;
    }

    internal static BuildServiceHostUpdate Convert(BuildServiceHostUpdate2010 update)
    {
      BuildServiceHostUpdate serviceHostUpdate = BuildServiceHostUpdate.None;
      if ((update & BuildServiceHostUpdate2010.BaseUrl) == BuildServiceHostUpdate2010.BaseUrl)
        serviceHostUpdate |= BuildServiceHostUpdate.BaseUrl;
      if ((update & BuildServiceHostUpdate2010.Name) == BuildServiceHostUpdate2010.Name)
        serviceHostUpdate |= BuildServiceHostUpdate.Name;
      if ((update & BuildServiceHostUpdate2010.RequireClientCertificates) == BuildServiceHostUpdate2010.RequireClientCertificates)
        serviceHostUpdate |= BuildServiceHostUpdate.RequireClientCertificates;
      return serviceHostUpdate;
    }

    internal static DeleteOptions Convert(DeleteOptions2010 options)
    {
      DeleteOptions deleteOptions = DeleteOptions.None;
      if ((options & DeleteOptions2010.All) == DeleteOptions2010.All)
        deleteOptions |= DeleteOptions.All;
      if ((options & DeleteOptions2010.Details) == DeleteOptions2010.Details)
        deleteOptions |= DeleteOptions.Details;
      if ((options & DeleteOptions2010.DropLocation) == DeleteOptions2010.DropLocation)
        deleteOptions |= DeleteOptions.DropLocation;
      if ((options & DeleteOptions2010.Label) == DeleteOptions2010.Label)
        deleteOptions |= DeleteOptions.Label;
      if ((options & DeleteOptions2010.Symbols) == DeleteOptions2010.Symbols)
        deleteOptions |= DeleteOptions.Symbols;
      if ((options & DeleteOptions2010.TestResults) == DeleteOptions2010.TestResults)
        deleteOptions |= DeleteOptions.TestResults;
      return deleteOptions;
    }

    internal static DeleteOptions2010 Convert(DeleteOptions options)
    {
      DeleteOptions2010 deleteOptions2010 = DeleteOptions2010.None;
      if ((options & DeleteOptions.All) == DeleteOptions.All)
        deleteOptions2010 |= DeleteOptions2010.All;
      if ((options & DeleteOptions.Details) == DeleteOptions.Details)
        deleteOptions2010 |= DeleteOptions2010.Details;
      if ((options & DeleteOptions.DropLocation) == DeleteOptions.DropLocation)
        deleteOptions2010 |= DeleteOptions2010.DropLocation;
      if ((options & DeleteOptions.Label) == DeleteOptions.Label)
        deleteOptions2010 |= DeleteOptions2010.Label;
      if ((options & DeleteOptions.Symbols) == DeleteOptions.Symbols)
        deleteOptions2010 |= DeleteOptions2010.Symbols;
      if ((options & DeleteOptions.TestResults) == DeleteOptions.TestResults)
        deleteOptions2010 |= DeleteOptions2010.TestResults;
      return deleteOptions2010;
    }

    internal static ScheduleDays2010 Convert(ScheduleDays days)
    {
      ScheduleDays2010 scheduleDays2010 = ScheduleDays2010.None;
      if ((days & ScheduleDays.All) == ScheduleDays.All)
        scheduleDays2010 |= ScheduleDays2010.All;
      if ((days & ScheduleDays.Friday) == ScheduleDays.Friday)
        scheduleDays2010 |= ScheduleDays2010.Friday;
      if ((days & ScheduleDays.Monday) == ScheduleDays.Monday)
        scheduleDays2010 |= ScheduleDays2010.Monday;
      if ((days & ScheduleDays.Saturday) == ScheduleDays.Saturday)
        scheduleDays2010 |= ScheduleDays2010.Saturday;
      if ((days & ScheduleDays.Sunday) == ScheduleDays.Sunday)
        scheduleDays2010 |= ScheduleDays2010.Sunday;
      if ((days & ScheduleDays.Thursday) == ScheduleDays.Thursday)
        scheduleDays2010 |= ScheduleDays2010.Thursday;
      if ((days & ScheduleDays.Tuesday) == ScheduleDays.Tuesday)
        scheduleDays2010 |= ScheduleDays2010.Tuesday;
      if ((days & ScheduleDays.Wednesday) == ScheduleDays.Wednesday)
        scheduleDays2010 |= ScheduleDays2010.Wednesday;
      return scheduleDays2010;
    }

    internal static ScheduleDays Convert(ScheduleDays2010 days)
    {
      ScheduleDays scheduleDays = ScheduleDays.None;
      if ((days & ScheduleDays2010.All) == ScheduleDays2010.All)
        scheduleDays |= ScheduleDays.All;
      if ((days & ScheduleDays2010.Friday) == ScheduleDays2010.Friday)
        scheduleDays |= ScheduleDays.Friday;
      if ((days & ScheduleDays2010.Monday) == ScheduleDays2010.Monday)
        scheduleDays |= ScheduleDays.Monday;
      if ((days & ScheduleDays2010.Saturday) == ScheduleDays2010.Saturday)
        scheduleDays |= ScheduleDays.Saturday;
      if ((days & ScheduleDays2010.Sunday) == ScheduleDays2010.Sunday)
        scheduleDays |= ScheduleDays.Sunday;
      if ((days & ScheduleDays2010.Thursday) == ScheduleDays2010.Thursday)
        scheduleDays |= ScheduleDays.Thursday;
      if ((days & ScheduleDays2010.Tuesday) == ScheduleDays2010.Tuesday)
        scheduleDays |= ScheduleDays.Tuesday;
      if ((days & ScheduleDays2010.Wednesday) == ScheduleDays2010.Wednesday)
        scheduleDays |= ScheduleDays.Wednesday;
      return scheduleDays;
    }

    internal static ProcessTemplateType Convert(ProcessTemplateType2010 type)
    {
      ProcessTemplateType processTemplateType = ProcessTemplateType.Custom;
      switch (type)
      {
        case ProcessTemplateType2010.Default:
          processTemplateType = ProcessTemplateType.Default;
          break;
        case ProcessTemplateType2010.Upgrade:
          processTemplateType = ProcessTemplateType.Upgrade;
          break;
      }
      return processTemplateType;
    }

    internal static WorkspaceMappingType2010 Convert(WorkspaceMappingType type)
    {
      WorkspaceMappingType2010 workspaceMappingType2010 = WorkspaceMappingType2010.Cloak;
      if (type == WorkspaceMappingType.Map)
        workspaceMappingType2010 = WorkspaceMappingType2010.Map;
      return workspaceMappingType2010;
    }

    internal static WorkspaceMappingType Convert(WorkspaceMappingType2010 type)
    {
      WorkspaceMappingType workspaceMappingType = WorkspaceMappingType.Cloak;
      if (type == WorkspaceMappingType2010.Map)
        workspaceMappingType = WorkspaceMappingType.Map;
      return workspaceMappingType;
    }

    internal static IList<ProcessTemplateType> Convert(IList<ProcessTemplateType2010> types)
    {
      List<ProcessTemplateType> processTemplateTypeList = new List<ProcessTemplateType>(types.Count);
      foreach (ProcessTemplateType2010 type in (IEnumerable<ProcessTemplateType2010>) types)
        processTemplateTypeList.Add(RosarioHelper.Convert(type));
      return (IList<ProcessTemplateType>) processTemplateTypeList;
    }

    internal static QueueOptions Convert(QueueOptions2010 options) => options == QueueOptions2010.Preview ? QueueOptions.Preview : QueueOptions.None;

    internal static TagComparison Convert(TagComparison2010 comparison)
    {
      TagComparison tagComparison = TagComparison.MatchAtLeast;
      if (comparison == TagComparison2010.MatchExactly)
        tagComparison = TagComparison.MatchExactly;
      return tagComparison;
    }

    internal static BuildPhaseStatus Convert(BuildPhaseStatus2010 status)
    {
      BuildPhaseStatus buildPhaseStatus = BuildPhaseStatus.Unknown;
      switch (status)
      {
        case BuildPhaseStatus2010.Failed:
          buildPhaseStatus = BuildPhaseStatus.Failed;
          break;
        case BuildPhaseStatus2010.Succeeded:
          buildPhaseStatus = BuildPhaseStatus.Succeeded;
          break;
      }
      return buildPhaseStatus;
    }

    internal static BuildUpdate Convert(BuildUpdate2010 update)
    {
      BuildUpdate buildUpdate = BuildUpdate.None;
      if ((update & BuildUpdate2010.BuildNumber) == BuildUpdate2010.BuildNumber)
        buildUpdate |= BuildUpdate.BuildNumber;
      if ((update & BuildUpdate2010.CompilationStatus) == BuildUpdate2010.CompilationStatus)
        buildUpdate |= BuildUpdate.CompilationStatus;
      if ((update & BuildUpdate2010.DropLocation) == BuildUpdate2010.DropLocation)
        buildUpdate |= BuildUpdate.DropLocation;
      if ((update & BuildUpdate2010.FinishTime) == BuildUpdate2010.FinishTime)
        buildUpdate |= BuildUpdate.FinishTime;
      if ((update & BuildUpdate2010.KeepForever) == BuildUpdate2010.KeepForever)
        buildUpdate |= BuildUpdate.KeepForever;
      if ((update & BuildUpdate2010.LabelName) == BuildUpdate2010.LabelName)
        buildUpdate |= BuildUpdate.LabelName;
      if ((update & BuildUpdate2010.LogLocation) == BuildUpdate2010.LogLocation)
        buildUpdate |= BuildUpdate.LogLocation;
      if ((update & BuildUpdate2010.Quality) == BuildUpdate2010.Quality)
        buildUpdate |= BuildUpdate.Quality;
      if ((update & BuildUpdate2010.SourceGetVersion) == BuildUpdate2010.SourceGetVersion)
        buildUpdate |= BuildUpdate.SourceGetVersion;
      if ((update & BuildUpdate2010.Status) == BuildUpdate2010.Status)
        buildUpdate |= BuildUpdate.Status;
      if ((update & BuildUpdate2010.TestStatus) == BuildUpdate2010.TestStatus)
        buildUpdate |= BuildUpdate.TestStatus;
      return buildUpdate;
    }

    internal static AgentReservation2010 Convert(
      AgentReservation reservation,
      string requestBuildUri)
    {
      if (reservation == null)
        return (AgentReservation2010) null;
      AgentReservation2010 agentReservation2010 = new AgentReservation2010();
      agentReservation2010.BuildUri = requestBuildUri;
      agentReservation2010.ControllerUri = reservation.ControllerUri;
      agentReservation2010.Id = reservation.Id;
      agentReservation2010.ReservationTime = reservation.ReservationTime;
      agentReservation2010.ReservedAgentUri = reservation.ReservedAgentUri;
      agentReservation2010.PossibleAgents.AddRange((IEnumerable<string>) reservation.PossibleAgents);
      return agentReservation2010;
    }

    public static BuildAgent2010 Convert(BuildAgent buildAgent)
    {
      if (buildAgent == null)
        return (BuildAgent2010) null;
      BuildAgent2010 buildAgent2010 = new BuildAgent2010();
      buildAgent2010.BuildDirectory = buildAgent.BuildDirectory;
      buildAgent2010.ControllerUri = buildAgent.ControllerUri;
      buildAgent2010.DateCreated = buildAgent.DateCreated;
      buildAgent2010.DateUpdated = buildAgent.DateUpdated;
      buildAgent2010.Description = buildAgent.Description;
      buildAgent2010.Enabled = buildAgent.Enabled;
      buildAgent2010.Name = buildAgent.Name;
      buildAgent2010.ReservedForBuild = buildAgent.ReservedForBuild;
      buildAgent2010.ServiceHostUri = buildAgent.ServiceHostUri;
      buildAgent2010.Status = RosarioHelper.Convert(buildAgent.Status);
      buildAgent2010.StatusMessage = buildAgent.StatusMessage;
      buildAgent2010.Uri = buildAgent.Uri;
      buildAgent2010.Url = buildAgent.Url;
      buildAgent2010.Tags.AddRange((IEnumerable<string>) buildAgent.Tags);
      return buildAgent2010;
    }

    public static BuildAgent Convert(BuildAgent2010 buildAgent)
    {
      if (buildAgent == null)
        return (BuildAgent) null;
      BuildAgent buildAgent1 = new BuildAgent();
      buildAgent1.BuildDirectory = buildAgent.BuildDirectory;
      buildAgent1.ControllerUri = buildAgent.ControllerUri;
      buildAgent1.DateCreated = buildAgent.DateCreated;
      buildAgent1.DateUpdated = buildAgent.DateUpdated;
      buildAgent1.Description = buildAgent.Description;
      buildAgent1.Enabled = buildAgent.Enabled;
      buildAgent1.Name = buildAgent.Name;
      buildAgent1.ReservedForBuild = buildAgent.ReservedForBuild;
      buildAgent1.ServiceHostUri = buildAgent.ServiceHostUri;
      buildAgent1.Status = RosarioHelper.Convert(buildAgent.Status);
      buildAgent1.StatusMessage = buildAgent.StatusMessage;
      buildAgent1.Uri = buildAgent.Uri;
      buildAgent1.Url = buildAgent.Url;
      buildAgent1.Tags.AddRange((IEnumerable<string>) buildAgent.Tags);
      return buildAgent1;
    }

    public static BuildAgentQueryResult2010 Convert(BuildAgentQueryResult queryResult)
    {
      BuildAgentQueryResult2010 agentQueryResult2010 = new BuildAgentQueryResult2010();
      agentQueryResult2010.Agents.AddRange(queryResult.Agents.Select<BuildAgent, BuildAgent2010>((Func<BuildAgent, BuildAgent2010>) (x => RosarioHelper.Convert(x))));
      agentQueryResult2010.Controllers.AddRange(queryResult.Controllers.Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))));
      agentQueryResult2010.ServiceHosts.AddRange(queryResult.ServiceHosts.Select<BuildServiceHost, BuildServiceHost2010>((Func<BuildServiceHost, BuildServiceHost2010>) (x => RosarioHelper.Convert(x))));
      return agentQueryResult2010;
    }

    public static BuildAgentSpec Convert(BuildAgentSpec2010 spec2010)
    {
      if (spec2010 == null)
        return (BuildAgentSpec) null;
      BuildAgentSpec buildAgentSpec = new BuildAgentSpec();
      buildAgentSpec.ControllerName = spec2010.ControllerName;
      buildAgentSpec.Name = spec2010.Name;
      buildAgentSpec.ServiceHostName = spec2010.ServiceHostName;
      buildAgentSpec.Tags.AddRange((IEnumerable<string>) spec2010.Tags);
      return buildAgentSpec;
    }

    public static BuildAgentUpdateOptions Convert(BuildAgentUpdateOptions2010 options2010)
    {
      BuildAgentUpdateOptions agentUpdateOptions = new BuildAgentUpdateOptions();
      agentUpdateOptions.BuildDirectory = options2010.BuildDirectory;
      agentUpdateOptions.ControllerUri = options2010.ControllerUri;
      agentUpdateOptions.Description = options2010.Description;
      agentUpdateOptions.Enabled = options2010.Enabled;
      agentUpdateOptions.Fields = RosarioHelper.Convert(options2010.Fields);
      agentUpdateOptions.Name = options2010.Name;
      agentUpdateOptions.Status = RosarioHelper.Convert(options2010.Status);
      agentUpdateOptions.StatusMessage = options2010.StatusMessage;
      agentUpdateOptions.Uri = options2010.Uri;
      agentUpdateOptions.Tags.AddRange((IEnumerable<string>) options2010.Tags);
      return agentUpdateOptions;
    }

    public static BuildController2010 Convert(BuildController buildController)
    {
      if (buildController == null)
        return (BuildController2010) null;
      BuildController2010 buildController2010 = new BuildController2010();
      buildController2010.CustomAssemblyPath = buildController.CustomAssemblyPath;
      buildController2010.DateCreated = buildController.DateCreated;
      buildController2010.DateUpdated = buildController.DateUpdated;
      buildController2010.Description = buildController.Description;
      buildController2010.Enabled = buildController.Enabled;
      buildController2010.MaxConcurrentBuilds = buildController.MaxConcurrentBuilds;
      buildController2010.Name = buildController.Name;
      buildController2010.QueueCount = buildController.QueueCount;
      buildController2010.ServiceHostUri = buildController.ServiceHostUri;
      buildController2010.Status = RosarioHelper.Convert(buildController.Status);
      buildController2010.StatusMessage = buildController.StatusMessage;
      buildController2010.Uri = buildController.Uri;
      buildController2010.Url = buildController.Url;
      buildController2010.Tags.AddRange((IEnumerable<string>) buildController.Tags);
      return buildController2010;
    }

    public static BuildController Convert(BuildController2010 buildController)
    {
      if (buildController == null)
        return (BuildController) null;
      BuildController buildController1 = new BuildController();
      buildController1.CustomAssemblyPath = buildController.CustomAssemblyPath;
      buildController1.DateCreated = buildController.DateCreated;
      buildController1.DateUpdated = buildController.DateUpdated;
      buildController1.Description = buildController.Description;
      buildController1.Enabled = buildController.Enabled;
      buildController1.MaxConcurrentBuilds = buildController.MaxConcurrentBuilds;
      buildController1.Name = buildController.Name;
      buildController1.QueueCount = buildController.QueueCount;
      buildController1.ServiceHostUri = buildController.ServiceHostUri;
      buildController1.Status = RosarioHelper.Convert(buildController.Status);
      buildController1.StatusMessage = buildController.StatusMessage;
      buildController1.Uri = buildController.Uri;
      buildController1.Url = buildController.Url;
      buildController1.Tags.AddRange((IEnumerable<string>) buildController.Tags);
      return buildController1;
    }

    public static BuildControllerQueryResult2010 Convert(BuildControllerQueryResult queryResult)
    {
      BuildControllerQueryResult2010 controllerQueryResult2010 = new BuildControllerQueryResult2010();
      controllerQueryResult2010.Agents.AddRange(queryResult.Agents.Select<BuildAgent, BuildAgent2010>((Func<BuildAgent, BuildAgent2010>) (x => RosarioHelper.Convert(x))));
      controllerQueryResult2010.Controllers.AddRange(queryResult.Controllers.Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))));
      controllerQueryResult2010.ServiceHosts.AddRange(queryResult.ServiceHosts.Select<BuildServiceHost, BuildServiceHost2010>((Func<BuildServiceHost, BuildServiceHost2010>) (x => RosarioHelper.Convert(x))));
      return controllerQueryResult2010;
    }

    public static BuildControllerSpec Convert(BuildControllerSpec2010 spec2010) => new BuildControllerSpec()
    {
      IncludeAgents = spec2010.IncludeAgents,
      Name = spec2010.Name,
      ServiceHostName = spec2010.ServiceHostName
    };

    public static BuildControllerUpdateOptions Convert(BuildControllerUpdateOptions2010 options2010) => new BuildControllerUpdateOptions()
    {
      CustomAssemblyPath = options2010.CustomAssemblyPath,
      Description = options2010.Description,
      Enabled = options2010.Enabled,
      Fields = RosarioHelper.Convert(options2010.Fields),
      MaxConcurrentBuilds = options2010.MaxConcurrentBuilds,
      Name = options2010.Name,
      Status = RosarioHelper.Convert(options2010.Status),
      StatusMessage = options2010.StatusMessage,
      Uri = options2010.Uri
    };

    public static BuildDefinitionQueryResult2010 Convert(BuildDefinitionQueryResult queryResult)
    {
      BuildDefinitionQueryResult2010 definitionQueryResult2010 = new BuildDefinitionQueryResult2010();
      definitionQueryResult2010.Agents.AddRange(queryResult.Agents.Select<BuildAgent, BuildAgent2010>((Func<BuildAgent, BuildAgent2010>) (x => RosarioHelper.Convert(x))));
      definitionQueryResult2010.Controllers.AddRange(queryResult.Controllers.Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))));
      definitionQueryResult2010.Definitions.AddRange(queryResult.Definitions.Select<BuildDefinition, BuildDefinition2010>((Func<BuildDefinition, BuildDefinition2010>) (x => RosarioHelper.Convert(x))));
      definitionQueryResult2010.ServiceHosts.AddRange(queryResult.ServiceHosts.Select<BuildServiceHost, BuildServiceHost2010>((Func<BuildServiceHost, BuildServiceHost2010>) (x => RosarioHelper.Convert(x))));
      return definitionQueryResult2010;
    }

    public static BuildDefinition2010 Convert(BuildDefinition definition)
    {
      if (definition == null)
        return (BuildDefinition2010) null;
      BuildDefinition2010 buildDefinition2010 = new BuildDefinition2010();
      buildDefinition2010.BuildControllerUri = definition.BuildControllerUri;
      buildDefinition2010.ContinuousIntegrationQuietPeriod = definition.ContinuousIntegrationQuietPeriod;
      buildDefinition2010.ContinuousIntegrationType = RosarioHelper.Convert(definition.TriggerType);
      buildDefinition2010.DefaultDropLocation = definition.DefaultDropLocation;
      buildDefinition2010.Description = definition.Description;
      buildDefinition2010.Enabled = definition.QueueStatus == DefinitionQueueStatus.Enabled || definition.QueueStatus == DefinitionQueueStatus.Paused;
      buildDefinition2010.FullPath = definition.FullPath;
      buildDefinition2010.LastBuildUri = definition.LastBuildUri;
      buildDefinition2010.LastGoodBuildLabel = definition.LastGoodBuildLabel;
      buildDefinition2010.LastGoodBuildUri = definition.LastGoodBuildUri;
      buildDefinition2010.Process = RosarioHelper.Convert(definition.Process);
      buildDefinition2010.ProcessParameters = definition.ProcessParameters;
      buildDefinition2010.ProcessTemplateId = definition.ProcessTemplateId;
      buildDefinition2010.RetentionPolicies.AddRange(definition.RetentionPolicies.Select<RetentionPolicy, RetentionPolicy2010>((Func<RetentionPolicy, RetentionPolicy2010>) (x => RosarioHelper.Convert(x))));
      buildDefinition2010.ScheduleJobId = definition.ScheduleJobId;
      buildDefinition2010.Schedules.AddRange(definition.Schedules.Select<Schedule, Schedule2010>((Func<Schedule, Schedule2010>) (x => RosarioHelper.Convert(x))));
      buildDefinition2010.TeamProject = definition.TeamProject;
      buildDefinition2010.Uri = definition.Uri;
      buildDefinition2010.WorkspaceTemplate = RosarioHelper.Convert(definition.WorkspaceTemplate);
      return buildDefinition2010;
    }

    public static BuildDefinition Convert(BuildDefinition2010 definition)
    {
      if (definition == null)
        return (BuildDefinition) null;
      BuildDefinition buildDefinition = new BuildDefinition();
      buildDefinition.BuildControllerUri = definition.BuildControllerUri;
      buildDefinition.ContinuousIntegrationQuietPeriod = definition.ContinuousIntegrationQuietPeriod;
      buildDefinition.TriggerType = RosarioHelper.Convert(definition.ContinuousIntegrationType);
      buildDefinition.DefaultDropLocation = definition.DefaultDropLocation;
      buildDefinition.Description = definition.Description;
      buildDefinition.QueueStatus = definition.Enabled ? DefinitionQueueStatus.Enabled : DefinitionQueueStatus.Disabled;
      buildDefinition.FullPath = definition.FullPath;
      buildDefinition.LastBuildUri = definition.LastBuildUri;
      buildDefinition.LastGoodBuildLabel = definition.LastGoodBuildLabel;
      buildDefinition.LastGoodBuildUri = definition.LastGoodBuildUri;
      buildDefinition.Process = RosarioHelper.Convert(definition.Process);
      buildDefinition.ProcessParameters = definition.ProcessParameters;
      buildDefinition.ProcessTemplateId = definition.ProcessTemplateId;
      buildDefinition.RetentionPolicies.AddRange(definition.RetentionPolicies.Select<RetentionPolicy2010, RetentionPolicy>((Func<RetentionPolicy2010, RetentionPolicy>) (x => RosarioHelper.Convert(x))));
      buildDefinition.ScheduleJobId = definition.ScheduleJobId;
      buildDefinition.Schedules.AddRange(definition.Schedules.Select<Schedule2010, Schedule>((Func<Schedule2010, Schedule>) (x => RosarioHelper.Convert(x))));
      buildDefinition.TeamProject = definition.TeamProject;
      buildDefinition.Uri = definition.Uri;
      buildDefinition.WorkspaceTemplate = RosarioHelper.Convert(definition.WorkspaceTemplate);
      return buildDefinition;
    }

    public static BuildDefinitionSpec Convert(BuildDefinitionSpec2010 spec2010) => new BuildDefinitionSpec()
    {
      TriggerType = RosarioHelper.Convert(spec2010.ContinuousIntegrationType),
      FullPath = spec2010.FullPath,
      Options = RosarioHelper.Convert(spec2010.Options)
    };

    public static BuildDeletionResult2010 Convert(BuildDeletionResult result) => new BuildDeletionResult2010()
    {
      DropLocationFailure = RosarioHelper.Convert(result.DropLocationFailure),
      LabelFailure = RosarioHelper.Convert(result.LabelFailure),
      SymbolsFailure = RosarioHelper.Convert(result.SymbolsFailure),
      TestResultFailure = RosarioHelper.Convert(result.TestResultFailure)
    };

    public static BuildDetail2010 Convert(BuildDetail build)
    {
      if (build == null)
        return (BuildDetail2010) null;
      return new BuildDetail2010()
      {
        BuildAgentUri = build.BuildControllerUri,
        BuildControllerUri = build.BuildControllerUri,
        BuildDefinitionUri = build.BuildDefinitionUri,
        BuildNumber = build.BuildNumber,
        CompilationStatus = RosarioHelper.Convert(build.CompilationStatus),
        Definition = RosarioHelper.Convert(build.Definition),
        DropLocation = build.DropLocation,
        DropLocationRoot = build.DropLocationRoot,
        FinishTime = build.FinishTime,
        IsDeleted = build.IsDeleted,
        KeepForever = build.KeepForever,
        LabelName = build.LabelName,
        LastChangedBy = build.LastChangedBy,
        LastChangedOn = build.LastChangedOn,
        LogLocation = build.LogLocation,
        ProcessParameters = build.ProcessParameters,
        Quality = build.Quality,
        Reason = RosarioHelper.Convert(build.Reason),
        SourceGetVersion = build.SourceGetVersion,
        StartTime = build.StartTime,
        Status = RosarioHelper.Convert(build.Status),
        TeamProject = build.TeamProject,
        TestStatus = RosarioHelper.Convert(build.TestStatus),
        Uri = build.Uri
      };
    }

    public static BuildDetail2010 Convert(
      BuildDetail build,
      QueuedBuild2010 queuedBuild,
      bool addLatest)
    {
      BuildDetail2010 buildDetail2010 = RosarioHelper.Convert(build);
      if (queuedBuild != null)
      {
        buildDetail2010.RequestedBy = queuedBuild.RequestedBy;
        buildDetail2010.RequestedFor = queuedBuild.RequestedFor;
        buildDetail2010.ShelvesetName = queuedBuild.ShelvesetName;
        buildDetail2010.Uri = RosarioHelper.ConvertBuildUri(DBHelper.ExtractDbId(buildDetail2010.Uri), queuedBuild.Id, addLatest);
      }
      return buildDetail2010;
    }

    public static BuildTeamProjectPermission Convert(BuildTeamProjectPermission2010 permission)
    {
      if (permission == null)
        return (BuildTeamProjectPermission) null;
      BuildTeamProjectPermission projectPermission = new BuildTeamProjectPermission();
      projectPermission.Descriptor = permission.Descriptor;
      projectPermission.IdentityName = permission.IdentityName;
      projectPermission.Allows.AddRange((IEnumerable<string>) permission.Allows);
      projectPermission.Denies.AddRange((IEnumerable<string>) permission.Denies);
      return projectPermission;
    }

    public static Failure2010 Convert(Failure failure)
    {
      if (failure == null)
        return (Failure2010) null;
      return new Failure2010()
      {
        Code = failure.Code,
        Message = failure.Message
      };
    }

    public static InformationChangeRequest Convert(InformationChangeRequest2010 change2010)
    {
      InformationAddRequest2010 informationAddRequest2010 = change2010 as InformationAddRequest2010;
      InformationEditRequest2010 informationEditRequest2010 = change2010 as InformationEditRequest2010;
      InformationDeleteRequest2010 deleteRequest2010 = change2010 as InformationDeleteRequest2010;
      if (informationAddRequest2010 != null)
      {
        InformationAddRequest informationAddRequest = new InformationAddRequest();
        informationAddRequest.BuildUri = informationAddRequest2010.BuildUri;
        informationAddRequest.NodeId = informationAddRequest2010.NodeId;
        informationAddRequest.NodeType = informationAddRequest2010.NodeType;
        informationAddRequest.ParentId = informationAddRequest2010.ParentId;
        informationAddRequest.Fields.AddRange(informationAddRequest2010.Fields.Select<InformationField2010, InformationField>((Func<InformationField2010, InformationField>) (x => new InformationField(x.Name, x.Value))));
        return (InformationChangeRequest) informationAddRequest;
      }
      if (informationEditRequest2010 != null)
      {
        InformationEditRequest informationEditRequest = new InformationEditRequest();
        informationEditRequest.BuildUri = informationEditRequest2010.BuildUri;
        informationEditRequest.NodeId = informationEditRequest2010.NodeId;
        informationEditRequest.Options = RosarioHelper.Convert(informationEditRequest2010.Options);
        informationEditRequest.Fields.AddRange(informationEditRequest2010.Fields.Select<InformationField2010, InformationField>((Func<InformationField2010, InformationField>) (x => new InformationField(x.Name, x.Value))));
        return (InformationChangeRequest) informationEditRequest;
      }
      if (deleteRequest2010 == null)
        return (InformationChangeRequest) null;
      InformationDeleteRequest informationDeleteRequest = new InformationDeleteRequest();
      informationDeleteRequest.BuildUri = deleteRequest2010.BuildUri;
      informationDeleteRequest.NodeId = deleteRequest2010.NodeId;
      return (InformationChangeRequest) informationDeleteRequest;
    }

    public static BuildInformationNode2010 Convert(BuildInformationNode node)
    {
      if (node == null)
        return (BuildInformationNode2010) null;
      BuildInformationNode2010 informationNode2010 = new BuildInformationNode2010()
      {
        BuildUri = node.BuildUri,
        GroupId = node.GroupId,
        LastModifiedBy = node.LastModifiedBy,
        LastModifiedDate = node.LastModifiedDate,
        NodeId = node.NodeId,
        ParentId = node.ParentId,
        Type = node.Type
      };
      foreach (InformationField field in node.Fields)
        informationNode2010.Fields.Add(new InformationField2010(field.Name, field.Value));
      return informationNode2010;
    }

    public static BuildRequest Convert(BuildRequest2010 request2010) => new BuildRequest()
    {
      BatchId = BuildWellKnownBatchIds.DynamicBatch,
      BuildControllerUri = request2010.BuildControllerUri,
      BuildDefinitionUri = request2010.BuildDefinitionUri,
      CheckInTicket = request2010.CheckInTicket,
      CustomGetVersion = request2010.CustomGetVersion,
      DropLocation = request2010.DropLocation,
      GetOption = RosarioHelper.Convert(request2010.GetOption),
      MaxQueuePosition = request2010.MaxQueuePosition,
      Postponed = request2010.Postponed,
      Priority = RosarioHelper.Convert(request2010.Priority),
      ProcessParameters = request2010.ProcessParameters,
      Reason = RosarioHelper.Convert(request2010.Reason),
      RequestedFor = request2010.RequestedFor,
      ShelvesetName = request2010.ShelvesetName
    };

    public static BuildServiceHost2010 Convert(BuildServiceHost buildServiceHost)
    {
      if (buildServiceHost == null)
        return (BuildServiceHost2010) null;
      return new BuildServiceHost2010()
      {
        BaseUrl = buildServiceHost.BaseUrl,
        Name = buildServiceHost.Name,
        RequireClientCertificates = buildServiceHost.RequireClientCertificates,
        Uri = buildServiceHost.Uri,
        IsVirtual = buildServiceHost.IsVirtual
      };
    }

    public static BuildServiceHost Convert(BuildServiceHost2010 buildServiceHost)
    {
      if (buildServiceHost == null)
        return (BuildServiceHost) null;
      return new BuildServiceHost()
      {
        BaseUrl = buildServiceHost.BaseUrl,
        Name = buildServiceHost.Name,
        RequireClientCertificates = buildServiceHost.RequireClientCertificates,
        Uri = buildServiceHost.Uri,
        IsVirtual = buildServiceHost.IsVirtual
      };
    }

    public static BuildServiceHostQueryResult2010 Convert(BuildServiceHostQueryResult queryResult)
    {
      BuildServiceHostQueryResult2010 hostQueryResult2010 = new BuildServiceHostQueryResult2010();
      hostQueryResult2010.Agents.AddRange(queryResult.Agents.Select<BuildAgent, BuildAgent2010>((Func<BuildAgent, BuildAgent2010>) (x => RosarioHelper.Convert(x))));
      hostQueryResult2010.Controllers.AddRange(queryResult.Controllers.Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))));
      hostQueryResult2010.ServiceHosts.AddRange(queryResult.ServiceHosts.Select<BuildServiceHost, BuildServiceHost2010>((Func<BuildServiceHost, BuildServiceHost2010>) (x => RosarioHelper.Convert(x))));
      return hostQueryResult2010;
    }

    public static BuildServiceHostUpdateOptions Convert(BuildServiceHostUpdateOptions2010 update) => new BuildServiceHostUpdateOptions()
    {
      BaseUrl = update.BaseUrl,
      Fields = RosarioHelper.Convert(update.Fields),
      Name = update.Name,
      RequireClientCertificates = update.RequireClientCertificates,
      Uri = update.Uri
    };

    public static BuildQueueQueryResult2010 Convert(BuildQueueQueryResult queryResult)
    {
      BuildQueueQueryResult2010 queueQueryResult2010 = new BuildQueueQueryResult2010();
      Dictionary<string, List<QueuedBuild2010>> dictionary = new Dictionary<string, List<QueuedBuild2010>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (QueuedBuild queuedBuild in queryResult.QueuedBuilds)
      {
        if (queuedBuild == null)
        {
          queueQueryResult2010.Builds.Add((QueuedBuild2010) null);
        }
        else
        {
          QueuedBuild2010 queuedBuild2010 = RosarioHelper.Convert(queuedBuild);
          queueQueryResult2010.Builds.Add(queuedBuild2010);
          foreach (string buildUri in queuedBuild.BuildUris)
          {
            List<QueuedBuild2010> queuedBuild2010List;
            if (!dictionary.TryGetValue(buildUri, out queuedBuild2010List))
            {
              queuedBuild2010List = new List<QueuedBuild2010>();
              dictionary.Add(buildUri, queuedBuild2010List);
            }
            queuedBuild2010List.Add(queuedBuild2010);
          }
        }
      }
      foreach (BuildDetail build in queryResult.Builds)
      {
        List<QueuedBuild2010> queuedBuild2010List;
        if (dictionary.TryGetValue(build.Uri, out queuedBuild2010List))
        {
          foreach (QueuedBuild2010 queuedBuild in queuedBuild2010List)
            queuedBuild.Build = RosarioHelper.Convert(build, queuedBuild, true);
        }
      }
      queueQueryResult2010.Controllers.AddRange(queryResult.Controllers.Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))));
      queueQueryResult2010.Definitions.AddRange(queryResult.Definitions.Select<BuildDefinition, BuildDefinition2010>((Func<BuildDefinition, BuildDefinition2010>) (x => RosarioHelper.Convert(x))));
      queueQueryResult2010.ServiceHosts.AddRange(queryResult.ServiceHosts.Select<BuildServiceHost, BuildServiceHost2010>((Func<BuildServiceHost, BuildServiceHost2010>) (x => RosarioHelper.Convert(x))));
      return queueQueryResult2010;
    }

    public static BuildQueueSpec Convert(BuildQueueSpec2010 spec2010)
    {
      BuildQueueSpec buildQueueSpec = new BuildQueueSpec()
      {
        CompletedAge = Math.Abs(spec2010.CompletedAge),
        ControllerSpec = RosarioHelper.Convert(spec2010.ControllerSpec),
        DefinitionFilterType = spec2010.DefinitionFilterType,
        QueryOptions = RosarioHelper.Convert(spec2010.Options),
        RequestedFor = spec2010.RequestedFor,
        Status = RosarioHelper.Convert(spec2010.StatusFlags)
      };
      buildQueueSpec.DefinitionFilter = buildQueueSpec.DefinitionFilterType != DefinitionFilterType.DefinitionSpec ? spec2010.DefinitionFilter : (object) RosarioHelper.Convert((BuildDefinitionSpec2010) spec2010.DefinitionFilter);
      return buildQueueSpec;
    }

    public static QueuedBuild2010 Convert(QueuedBuild build)
    {
      if (build == null)
        return (QueuedBuild2010) null;
      return new QueuedBuild2010()
      {
        BuildControllerUri = build.BuildControllerUri,
        BuildDefinitionUri = build.BuildDefinitionUri,
        CustomGetVersion = build.CustomGetVersion,
        DropLocation = build.DropLocation,
        GetOption = RosarioHelper.Convert(build.GetOption),
        Id = build.Id,
        Priority = RosarioHelper.Convert(build.Priority),
        ProcessParameters = build.ProcessParameters,
        QueuePosition = build.QueuePosition,
        QueueTime = build.QueueTime,
        Reason = RosarioHelper.Convert(build.Reason),
        RequestedBy = build.RequestedBy,
        RequestedFor = build.RequestedFor,
        ShelvesetName = build.ShelvesetName,
        Status = RosarioHelper.Convert(build.Status),
        TeamProject = build.TeamProject
      };
    }

    public static QueuedBuildUpdateOptions Convert(QueuedBuildUpdateOptions2010 update2010) => new QueuedBuildUpdateOptions()
    {
      Fields = RosarioHelper.Convert(update2010.Fields),
      Postponed = update2010.Postponed,
      Priority = RosarioHelper.Convert(update2010.Priority),
      QueueId = update2010.QueueId
    };

    public static RetentionPolicy2010 Convert(RetentionPolicy policy)
    {
      if (policy == null)
        return (RetentionPolicy2010) null;
      return new RetentionPolicy2010()
      {
        BuildReason = RosarioHelper.Convert(policy.BuildReason),
        BuildStatus = RosarioHelper.Convert(policy.BuildStatus),
        DefinitionUri = policy.DefinitionUri,
        DeleteOptions = RosarioHelper.Convert(policy.DeleteOptions),
        NumberToKeep = policy.NumberToKeep
      };
    }

    public static RetentionPolicy Convert(RetentionPolicy2010 policy)
    {
      if (policy == null)
        return (RetentionPolicy) null;
      return new RetentionPolicy()
      {
        BuildReason = RosarioHelper.Convert(policy.BuildReason),
        BuildStatus = RosarioHelper.Convert(policy.BuildStatus),
        DefinitionUri = policy.DefinitionUri,
        DeleteOptions = RosarioHelper.Convert(policy.DeleteOptions),
        NumberToKeep = policy.NumberToKeep
      };
    }

    public static Schedule2010 Convert(Schedule schedule)
    {
      if (schedule == null)
        return (Schedule2010) null;
      return new Schedule2010()
      {
        DefinitionUri = schedule.DefinitionUri,
        TimeZoneId = schedule.TimeZoneId,
        UtcDaysToBuild = RosarioHelper.Convert(schedule.UtcDaysToBuild),
        UtcStartTime = schedule.UtcStartTime
      };
    }

    public static Schedule Convert(Schedule2010 schedule)
    {
      if (schedule == null)
        return (Schedule) null;
      return new Schedule()
      {
        DefinitionUri = schedule.DefinitionUri,
        TimeZoneId = schedule.TimeZoneId,
        UtcDaysToBuild = RosarioHelper.Convert(schedule.UtcDaysToBuild),
        UtcStartTime = schedule.UtcStartTime
      };
    }

    public static ProcessTemplate2010 Convert(ProcessTemplate template)
    {
      if (template == null)
        return (ProcessTemplate2010) null;
      return new ProcessTemplate2010()
      {
        Description = template.Description,
        FileExists = template.FileExists,
        Id = template.Id,
        Parameters = template.Parameters,
        ServerPath = template.ServerPath,
        SupportedReasons = RosarioHelper.Convert(template.SupportedReasons),
        TeamProject = template.TeamProject,
        TeamProjectObj = template.TeamProjectObj,
        TemplateType = RosarioHelper.Convert(template.TemplateType)
      };
    }

    public static ProcessTemplate Convert(ProcessTemplate2010 template)
    {
      if (template == null)
        return (ProcessTemplate) null;
      return new ProcessTemplate()
      {
        Description = template.Description,
        FileExists = template.FileExists,
        Id = template.Id,
        Parameters = template.Parameters,
        ServerPath = template.ServerPath,
        SupportedReasons = RosarioHelper.Convert(template.SupportedReasons),
        TeamProject = template.TeamProject,
        TeamProjectObj = template.TeamProjectObj,
        TemplateType = RosarioHelper.Convert(template.TemplateType)
      };
    }

    internal static ProcessTemplateType2010 Convert(ProcessTemplateType type)
    {
      ProcessTemplateType2010 templateType2010 = ProcessTemplateType2010.Custom;
      switch (type)
      {
        case ProcessTemplateType.Default:
          templateType2010 = ProcessTemplateType2010.Default;
          break;
        case ProcessTemplateType.Upgrade:
          templateType2010 = ProcessTemplateType2010.Upgrade;
          break;
      }
      return templateType2010;
    }

    public static WorkspaceMapping2010 Convert(WorkspaceMapping mapping) => new WorkspaceMapping2010()
    {
      Depth = mapping.Depth,
      LocalItem = mapping.LocalItem,
      MappingType = RosarioHelper.Convert(mapping.MappingType),
      ServerItem = mapping.ServerItem,
      WorkspaceId = mapping.WorkspaceId
    };

    public static WorkspaceMapping Convert(WorkspaceMapping2010 mapping) => new WorkspaceMapping()
    {
      Depth = mapping.Depth,
      LocalItem = mapping.LocalItem,
      MappingType = RosarioHelper.Convert(mapping.MappingType),
      ServerItem = mapping.ServerItem,
      WorkspaceId = mapping.WorkspaceId
    };

    public static WorkspaceTemplate2010 Convert(WorkspaceTemplate template)
    {
      if (template == null)
        return (WorkspaceTemplate2010) null;
      WorkspaceTemplate2010 workspaceTemplate2010 = new WorkspaceTemplate2010();
      workspaceTemplate2010.DefinitionUri = template.DefinitionUri;
      workspaceTemplate2010.LastModifiedBy = template.LastModifiedBy;
      workspaceTemplate2010.LastModifiedDate = template.LastModifiedDate;
      workspaceTemplate2010.WorkspaceId = template.WorkspaceId;
      workspaceTemplate2010.Mappings.AddRange(template.Mappings.Select<WorkspaceMapping, WorkspaceMapping2010>((Func<WorkspaceMapping, WorkspaceMapping2010>) (x => RosarioHelper.Convert(x))));
      return workspaceTemplate2010;
    }

    public static WorkspaceTemplate Convert(WorkspaceTemplate2010 template)
    {
      if (template == null)
        return (WorkspaceTemplate) null;
      WorkspaceTemplate workspaceTemplate = new WorkspaceTemplate();
      workspaceTemplate.DefinitionUri = template.DefinitionUri;
      workspaceTemplate.LastModifiedBy = template.LastModifiedBy;
      workspaceTemplate.LastModifiedDate = template.LastModifiedDate;
      workspaceTemplate.WorkspaceId = template.WorkspaceId;
      workspaceTemplate.Mappings.AddRange(template.Mappings.Select<WorkspaceMapping2010, WorkspaceMapping>((Func<WorkspaceMapping2010, WorkspaceMapping>) (x => RosarioHelper.Convert(x))));
      return workspaceTemplate;
    }

    internal static string ConvertBuildUri(string buildUri, int queueId, bool addLatest) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?queueId={1}{2}", (object) DBHelper.CreateArtifactUri("Build", buildUri), (object) queueId, addLatest ? (object) "&latest" : (object) string.Empty);

    public static void ConvertBuildUris(
      IVssRequestContext requestContext,
      IList<string> uris,
      bool useCompatibilityFormat,
      bool throwIfInvalidUri)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (ConvertBuildUris));
      using (CompatibilityComponent component = requestContext.CreateComponent<CompatibilityComponent>("Build"))
      {
        IDictionary<string, string> dictionary = component.ResolveBuildUris((ICollection<string>) uris, useCompatibilityFormat);
        for (int index = 0; index < uris.Count; ++index)
        {
          string str;
          if (dictionary.TryGetValue(uris[index], out str))
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Converted build uri '{0}' to '{1}'", (object) uris[index], (object) str);
            uris[index] = str;
          }
          else if (throwIfInvalidUri)
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Invalid build uri '{0}'", (object) uris[index]);
            throw new InvalidBuildUriException(uris[index]);
          }
        }
      }
      requestContext.TraceEnter(0, "Build", "Service", nameof (ConvertBuildUris));
    }

    public static List<BuildAgent2010> AddBuildAgents(
      IVssRequestContext requestContext,
      IList<BuildAgent2010> agents)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildAgent2010>(requestContext, nameof (agents), agents, false, ValidationContext.Add);
      IList<BuildAgent> list = (IList<BuildAgent>) agents.Select<BuildAgent2010, BuildAgent>((Func<BuildAgent2010, BuildAgent>) (x => RosarioHelper.Convert(x))).ToList<BuildAgent>();
      return requestContext.GetService<TeamFoundationBuildResourceService>().AddBuildAgents(requestContext, list).Select<BuildAgent, BuildAgent2010>((Func<BuildAgent, BuildAgent2010>) (x => RosarioHelper.Convert(x))).ToList<BuildAgent2010>();
    }

    public static List<BuildController2010> AddBuildControllers(
      IVssRequestContext requestContext,
      IList<BuildController2010> controllers)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildController2010>(requestContext, nameof (controllers), controllers, false, ValidationContext.Add);
      IList<BuildController> list = (IList<BuildController>) controllers.Select<BuildController2010, BuildController>((Func<BuildController2010, BuildController>) (x => RosarioHelper.Convert(x))).ToList<BuildController>();
      return requestContext.GetService<TeamFoundationBuildResourceService>().AddBuildControllers(requestContext, list).Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))).ToList<BuildController2010>();
    }

    public static List<BuildDefinition2010> AddBuildDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinition2010> definitions)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildDefinition2010>(requestContext, nameof (definitions), definitions, false, ValidationContext.Add);
      IList<BuildDefinition> list = (IList<BuildDefinition>) definitions.Select<BuildDefinition2010, BuildDefinition>((Func<BuildDefinition2010, BuildDefinition>) (x => RosarioHelper.Convert(x))).ToList<BuildDefinition>();
      return requestContext.GetService<TeamFoundationBuildService>().AddBuildDefinitions(requestContext, list).Select<BuildDefinition, BuildDefinition2010>((Func<BuildDefinition, BuildDefinition2010>) (x => RosarioHelper.Convert(x))).ToList<BuildDefinition2010>();
    }

    public static BuildServiceHost2010 AddBuildServiceHost(
      IVssRequestContext requestContext,
      BuildServiceHost2010 serviceHost)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, nameof (serviceHost), (IValidatable) serviceHost, false, ValidationContext.Add);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new NotSupportedException(ResourceStrings.AddingServiceHostRequiresCurrentClient());
      return RosarioHelper.Convert(requestContext.GetService<TeamFoundationBuildResourceService>().AddBuildServiceHost(requestContext, RosarioHelper.Convert(serviceHost)));
    }

    public static List<ProcessTemplate2010> AddProcessTemplates(
      IVssRequestContext requestContext,
      IList<ProcessTemplate2010> processTemplates)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<ProcessTemplate2010>(requestContext, nameof (processTemplates), processTemplates, false, ValidationContext.Add);
      IList<ProcessTemplate> list = (IList<ProcessTemplate>) processTemplates.Select<ProcessTemplate2010, ProcessTemplate>((Func<ProcessTemplate2010, ProcessTemplate>) (x => RosarioHelper.Convert(x))).ToList<ProcessTemplate>();
      return requestContext.GetService<TeamFoundationBuildService>().AddProcessTemplates(requestContext, list).Select<ProcessTemplate, ProcessTemplate2010>((Func<ProcessTemplate, ProcessTemplate2010>) (x => RosarioHelper.Convert(x))).ToList<ProcessTemplate2010>();
    }

    public static void CreateTeamProject(
      IVssRequestContext requestContext,
      string projectUri,
      IList<BuildTeamProjectPermission2010> permissions)
    {
      IList<BuildTeamProjectPermission> list = (IList<BuildTeamProjectPermission>) permissions.Select<BuildTeamProjectPermission2010, BuildTeamProjectPermission>((Func<BuildTeamProjectPermission2010, BuildTeamProjectPermission>) (x => RosarioHelper.Convert(x))).ToList<BuildTeamProjectPermission>();
      requestContext.GetService<TeamFoundationBuildService>().CreateTeamProject(requestContext, projectUri, list);
    }

    public static List<BuildDeletionResult2010> DeleteBuilds(
      IVssRequestContext requestContext,
      IList<string> uris,
      DeleteOptions2010 deleteOptions,
      bool throwIfInvalidUri)
    {
      ArgumentValidation.CheckUriArray(nameof (uris), uris, false, (string) null);
      RosarioHelper.ConvertBuildUris(requestContext, uris, false, throwIfInvalidUri);
      return requestContext.GetService<TeamFoundationBuildService>().DeleteBuilds(requestContext, uris, RosarioHelper.Convert(deleteOptions), throwIfInvalidUri, new Guid(), false).Select<BuildDeletionResult, BuildDeletionResult2010>((Func<BuildDeletionResult, BuildDeletionResult2010>) (x => RosarioHelper.Convert(x))).ToList<BuildDeletionResult2010>();
    }

    public static List<BuildDefinition2010> GetAffectedBuildDefinitions(
      IVssRequestContext requestContext,
      IList<string> serverItems,
      ContinuousIntegrationType continuousIntegrationType)
    {
      return requestContext.GetService<TeamFoundationBuildService>().GetAffectedBuildDefinitions(requestContext, serverItems, RosarioHelper.Convert(continuousIntegrationType)).Select<BuildDefinition, BuildDefinition2010>((Func<BuildDefinition, BuildDefinition2010>) (x => RosarioHelper.Convert(x))).ToList<BuildDefinition2010>();
    }

    public static BuildDetail2010 NotifyBuildCompleted(
      IVssRequestContext requestContext,
      string buildUri)
    {
      List<string> uris = new List<string>(1);
      uris.Add(buildUri);
      RosarioHelper.ConvertBuildUris(requestContext, (IList<string>) uris, false, false);
      return RosarioHelper.Convert(requestContext.GetService<TeamFoundationBuildService>().NotifyBuildCompleted(requestContext, uris[0]));
    }

    public static List<BuildAgentQueryResult2010> QueryBuildAgents(
      IVssRequestContext requestContext,
      IList<BuildAgentSpec2010> agentSpecs)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      IList<BuildAgentSpec> list = (IList<BuildAgentSpec>) agentSpecs.Select<BuildAgentSpec2010, BuildAgentSpec>((Func<BuildAgentSpec2010, BuildAgentSpec>) (x => RosarioHelper.Convert(x))).ToList<BuildAgentSpec>();
      List<BuildAgentQueryResult2010> agentQueryResult2010List = new List<BuildAgentQueryResult2010>();
      IVssRequestContext requestContext1 = requestContext;
      IList<BuildAgentSpec> agentSpecs1 = list;
      foreach (BuildAgentQueryResult queryBuildAgent in service.QueryBuildAgents(requestContext1, agentSpecs1))
        agentQueryResult2010List.Add(RosarioHelper.Convert(queryBuildAgent));
      return agentQueryResult2010List;
    }

    public static BuildAgentQueryResult2010 QueryBuildAgents(
      IVssRequestContext requestContext,
      BuildAgentSpec2010 agentSpec)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildAgentSpec[] agentSpecs = new BuildAgentSpec[1]
      {
        RosarioHelper.Convert(agentSpec)
      };
      List<BuildAgentQueryResult> source = service.QueryBuildAgents(requestContext, (IList<BuildAgentSpec>) agentSpecs);
      return source.Any<BuildAgentQueryResult>() ? RosarioHelper.Convert(source.First<BuildAgentQueryResult>()) : (BuildAgentQueryResult2010) null;
    }

    public static BuildAgentQueryResult2010 QueryBuildAgentsByUri(
      IVssRequestContext requestContext,
      IList<string> agentUris)
    {
      return RosarioHelper.Convert(requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildAgentsByUri(requestContext, agentUris, (IList<string>) null));
    }

    public static List<BuildControllerQueryResult2010> QueryBuildControllers(
      IVssRequestContext requestContext,
      IList<BuildControllerSpec2010> controllerSpecs)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      IList<BuildControllerSpec> list = (IList<BuildControllerSpec>) controllerSpecs.Select<BuildControllerSpec2010, BuildControllerSpec>((Func<BuildControllerSpec2010, BuildControllerSpec>) (x => RosarioHelper.Convert(x))).ToList<BuildControllerSpec>();
      List<BuildControllerQueryResult2010> controllerQueryResult2010List = new List<BuildControllerQueryResult2010>();
      IVssRequestContext requestContext1 = requestContext;
      IList<BuildControllerSpec> controllerSpecs1 = list;
      foreach (BuildControllerQueryResult queryBuildController in service.QueryBuildControllers(requestContext1, controllerSpecs1))
        controllerQueryResult2010List.Add(RosarioHelper.Convert(queryBuildController));
      return controllerQueryResult2010List;
    }

    public static BuildControllerQueryResult2010 QueryBuildControllers(
      IVssRequestContext requestContext,
      BuildControllerSpec2010 controllerSpec)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildControllerSpec[] controllerSpecs = new BuildControllerSpec[1]
      {
        RosarioHelper.Convert(controllerSpec)
      };
      List<BuildControllerQueryResult> controllerQueryResultList = service.QueryBuildControllers(requestContext, (IList<BuildControllerSpec>) controllerSpecs);
      return controllerQueryResultList.Count > 0 ? RosarioHelper.Convert(controllerQueryResultList[0]) : (BuildControllerQueryResult2010) null;
    }

    public static BuildControllerQueryResult2010 QueryBuildControllersByUri(
      IVssRequestContext requestContext,
      IList<string> controllerUris,
      bool includeAgents)
    {
      return RosarioHelper.Convert(requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildControllersByUri(requestContext, controllerUris, (IList<string>) null, includeAgents));
    }

    public static List<BuildDefinitionQueryResult2010> QueryBuildDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinitionSpec2010> specs)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildDefinitionSpec2010>(requestContext, nameof (specs), specs, false, ValidationContext.Query);
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      List<BuildDefinitionSpec> specs1 = new List<BuildDefinitionSpec>();
      foreach (BuildDefinitionSpec2010 spec in (IEnumerable<BuildDefinitionSpec2010>) specs)
        specs1.Add(new BuildDefinitionSpec()
        {
          TriggerType = RosarioHelper.Convert(spec.ContinuousIntegrationType),
          FullPath = spec.FullPath,
          Options = RosarioHelper.Convert(spec.Options)
        });
      List<BuildDefinitionQueryResult> definitionQueryResultList = service.QueryBuildDefinitions(requestContext, (IList<BuildDefinitionSpec>) specs1);
      List<BuildDefinitionQueryResult2010> definitionQueryResult2010List = new List<BuildDefinitionQueryResult2010>();
      foreach (BuildDefinitionQueryResult queryResult in definitionQueryResultList)
        definitionQueryResult2010List.Add(RosarioHelper.Convert(queryResult));
      return definitionQueryResult2010List;
    }

    public static BuildDefinitionQueryResult2010 QueryBuildDefinitions(
      IVssRequestContext requestContext,
      BuildDefinitionSpec2010 spec)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, nameof (spec), (IValidatable) spec, false, ValidationContext.Query);
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      BuildDefinitionSpec[] specs = new BuildDefinitionSpec[1]
      {
        new BuildDefinitionSpec()
        {
          TriggerType = RosarioHelper.Convert(spec.ContinuousIntegrationType),
          FullPath = spec.FullPath,
          Options = RosarioHelper.Convert(spec.Options)
        }
      };
      List<BuildDefinitionQueryResult> definitionQueryResultList = service.QueryBuildDefinitions(requestContext, (IList<BuildDefinitionSpec>) specs);
      return definitionQueryResultList.Count > 0 ? RosarioHelper.Convert(definitionQueryResultList[0]) : (BuildDefinitionQueryResult2010) null;
    }

    public static BuildDefinitionQueryResult2010 QueryBuildDefinitionsByUri(
      IVssRequestContext requestContext,
      IList<string> uris,
      QueryOptions2010 options)
    {
      return RosarioHelper.Convert(requestContext.GetService<TeamFoundationBuildService>().QueryBuildDefinitionsByUri(requestContext, uris, (IList<string>) null, RosarioHelper.Convert(options), new Guid()));
    }

    public static BuildServiceHostQueryResult2010 QueryBuildServiceHosts(
      IVssRequestContext requestContext,
      string computer)
    {
      return RosarioHelper.Convert(requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildServiceHosts(requestContext, computer));
    }

    public static BuildServiceHostQueryResult2010 QueryBuildServiceHostsByUri(
      IVssRequestContext requestContext,
      IList<string> serviceHostUris)
    {
      return RosarioHelper.Convert(requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildServiceHostsByUri(requestContext, serviceHostUris));
    }

    public static List<BuildQueueQueryResult2010> QueryQueuedBuilds(
      IVssRequestContext requestContext,
      List<BuildQueueSpec2010> specs)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildQueueSpec2010>(requestContext, nameof (specs), (IList<BuildQueueSpec2010>) specs, false, ValidationContext.Query);
      IList<BuildQueueSpec> list = (IList<BuildQueueSpec>) specs.Select<BuildQueueSpec2010, BuildQueueSpec>((Func<BuildQueueSpec2010, BuildQueueSpec>) (x => RosarioHelper.Convert(x))).ToList<BuildQueueSpec>();
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      List<BuildQueueQueryResult2010> queueQueryResult2010List = new List<BuildQueueQueryResult2010>();
      IVssRequestContext requestContext1 = requestContext;
      IList<BuildQueueSpec> specs1 = list;
      Guid projectId = new Guid();
      using (TeamFoundationDataReader foundationDataReader = service.QueryQueuedBuilds(requestContext1, specs1, projectId))
      {
        foreach (BuildQueueQueryResult current in foundationDataReader.CurrentEnumerable<BuildQueueQueryResult>())
          queueQueryResult2010List.Add(RosarioHelper.Convert(current));
      }
      return queueQueryResult2010List;
    }

    public static BuildQueueQueryResult2010 QueryQueuedBuildsById(
      IVssRequestContext requestContext,
      IList<int> ids,
      QueryOptions2010 options)
    {
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationBuildService>().QueryQueuedBuildsById(requestContext, ids, (IList<string>) Array.Empty<string>(), RosarioHelper.Convert(options)))
        return RosarioHelper.Convert(foundationDataReader.Current<BuildQueueQueryResult>());
    }

    public static TeamFoundationDataReader QueryBuilds(
      IVssRequestContext requestContext,
      IList<BuildDetailSpec2010> specs)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryBuilds));
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildDetailSpec2010>(requestContext, nameof (specs), specs, false, ValidationContext.Query);
      CommandQueryBuilds2010 disposableObject = (CommandQueryBuilds2010) null;
      try
      {
        disposableObject = new CommandQueryBuilds2010(requestContext);
        disposableObject.Execute(specs);
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryBuilds));
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Results
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Service", ex);
        disposableObject?.Dispose();
        throw;
      }
    }

    public static TeamFoundationDataReader QueryBuildsByUri(
      IVssRequestContext requestContext,
      IList<string> uris,
      IList<string> informationTypes,
      QueryOptions2010 options,
      QueryDeletedOption2010 deletedOption)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryBuildsByUri));
      ArgumentValidation.CheckUriArray(nameof (uris), uris, false, (string) null);
      if (informationTypes == null)
        informationTypes = (IList<string>) Array.Empty<string>();
      RosarioHelper.ConvertBuildUris(requestContext, uris, true, false);
      CommandQueryBuildsByUri2010 disposableObject = (CommandQueryBuildsByUri2010) null;
      try
      {
        disposableObject = new CommandQueryBuildsByUri2010(requestContext);
        disposableObject.Execute(uris, informationTypes, options, deletedOption);
        requestContext.TraceLeave(0, "Build", "Service", nameof (QueryBuildsByUri));
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Service", ex);
        disposableObject?.Dispose();
        throw;
      }
    }

    public static List<ProcessTemplate2010> QueryProcessTemplates(
      IVssRequestContext requestContext,
      string teamProject,
      IList<ProcessTemplateType2010> queryTypes)
    {
      ArgumentValidation.Check(nameof (queryTypes), (object) queryTypes, false);
      return requestContext.GetService<TeamFoundationBuildService>().QueryProcessTemplates(requestContext, teamProject, RosarioHelper.Convert(queryTypes)).Select<ProcessTemplate, ProcessTemplate2010>((Func<ProcessTemplate, ProcessTemplate2010>) (x => RosarioHelper.Convert(x))).ToList<ProcessTemplate2010>();
    }

    public static List<QueuedBuild2010> QueueBuilds(
      IVssRequestContext requestContext,
      IList<BuildRequest2010> requests,
      QueueOptions2010 options)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildRequest2010>(requestContext, nameof (requests), requests, false, ValidationContext.Query);
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      IList<BuildRequest> list = (IList<BuildRequest>) requests.Select<BuildRequest2010, BuildRequest>((Func<BuildRequest2010, BuildRequest>) (x => RosarioHelper.Convert(x))).ToList<BuildRequest>();
      IVssRequestContext requestContext1 = requestContext;
      IList<BuildRequest> requests1 = list;
      int options1 = (int) RosarioHelper.Convert(options);
      Guid projectId = new Guid();
      return RosarioHelper.Convert(service.QueueBuilds(requestContext1, requests1, (QueueOptions) options1, projectId)).Builds;
    }

    public static void StopBuilds(IVssRequestContext requestContext, IList<string> uris)
    {
      ArgumentValidation.CheckUriArray(nameof (uris), uris, false, (string) null);
      RosarioHelper.ConvertBuildUris(requestContext, uris, false, true);
      requestContext.GetService<TeamFoundationBuildService>().StopBuilds(requestContext, uris, new Guid());
    }

    public static BuildAgentQueryResult2010 TestBuildAgentConnection(
      IVssRequestContext requestContext,
      string agentUri)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildAgentQueryResult2010 agentQueryResult2010 = new BuildAgentQueryResult2010();
      try
      {
        BuildAgentQueryResult agentQueryResult = service.QueryBuildAgentsByUri(requestContext, (IList<string>) ((IEnumerable<string>) new string[1]
        {
          agentUri
        }).ToList<string>(), (IList<string>) null);
        BuildAgent[] array = agentQueryResult.Agents.ToArray();
        agentQueryResult2010.Agents.AddRange(((IEnumerable<BuildAgent>) array).Select<BuildAgent, BuildAgent2010>((Func<BuildAgent, BuildAgent2010>) (x => RosarioHelper.Convert(x))));
        agentQueryResult2010.Controllers.AddRange(agentQueryResult.Controllers.Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))));
        agentQueryResult2010.ServiceHosts.AddRange(agentQueryResult.ServiceHosts.Select<BuildServiceHost, BuildServiceHost2010>((Func<BuildServiceHost, BuildServiceHost2010>) (x => RosarioHelper.Convert(x))));
        BuildAgent agent = ((IEnumerable<BuildAgent>) array).FirstOrDefault<BuildAgent>();
        BuildServiceHost host = agentQueryResult.ServiceHosts.FirstOrDefault<BuildServiceHost>((Func<BuildServiceHost, bool>) (x => TFStringComparer.ArtiFactUrl.Equals(x.Uri, agent.ServiceHostUri)));
        if (agent != null && host != null && host.Version < 400)
        {
          agent.Url = host.GetUrlForService(agent.Uri);
          agent.TestAgentAvailability(requestContext, host);
        }
        return agentQueryResult2010;
      }
      catch (Exception ex)
      {
        agentQueryResult2010.Failures.Add(new Failure2010(ex));
        return agentQueryResult2010;
      }
    }

    public static BuildControllerQueryResult2010 TestBuildControllerConnection(
      IVssRequestContext requestContext,
      string controllerUri)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildControllerQueryResult2010 controllerQueryResult2010 = new BuildControllerQueryResult2010();
      try
      {
        BuildControllerQueryResult controllerQueryResult = service.QueryBuildControllersByUri(requestContext, (IList<string>) ((IEnumerable<string>) new string[1]
        {
          controllerUri
        }).ToList<string>(), (IList<string>) null, false);
        controllerQueryResult2010.Agents.AddRange(controllerQueryResult.Agents.Select<BuildAgent, BuildAgent2010>((Func<BuildAgent, BuildAgent2010>) (x => RosarioHelper.Convert(x))));
        controllerQueryResult2010.Controllers.AddRange(controllerQueryResult.Controllers.Select<BuildController, BuildController2010>((Func<BuildController, BuildController2010>) (x => RosarioHelper.Convert(x))));
        controllerQueryResult2010.ServiceHosts.AddRange(controllerQueryResult.ServiceHosts.Select<BuildServiceHost, BuildServiceHost2010>((Func<BuildServiceHost, BuildServiceHost2010>) (x => RosarioHelper.Convert(x))));
        BuildController controller = controllerQueryResult.Controllers.FirstOrDefault<BuildController>();
        BuildServiceHost host = controllerQueryResult.ServiceHosts.FirstOrDefault<BuildServiceHost>((Func<BuildServiceHost, bool>) (x => TFStringComparer.ArtiFactUrl.Equals(x.Uri, controller.ServiceHostUri)));
        if (controller != null && host != null && host.Version < 400)
        {
          controller.Url = host.GetUrlForService(controller.Uri);
          controller.TestControllerAvailability(requestContext, host);
        }
        return controllerQueryResult2010;
      }
      catch (Exception ex)
      {
        controllerQueryResult2010.Failures.Add(new Failure2010(ex));
        return controllerQueryResult2010;
      }
    }

    public static BuildServiceHostQueryResult2010 TestBuildServiceHostConnections(
      IVssRequestContext requestContext,
      string hostUri)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      try
      {
        BuildServiceHostQueryResult queryResult = service.QueryBuildServiceHostsByUri(requestContext, (IList<string>) ((IEnumerable<string>) new string[1]
        {
          hostUri
        }).ToList<string>());
        BuildServiceHost host = queryResult.ServiceHosts.FirstOrDefault<BuildServiceHost>();
        if (host != null && host.Version < 400)
        {
          IVssRequestContext requestContext1 = requestContext.Elevate();
          foreach (BuildController controller in queryResult.Controllers)
          {
            if (TFStringComparer.ArtiFactUrl.Equals(controller.ServiceHostUri, host.Uri))
            {
              controller.Url = host.GetUrlForService(controller.Uri);
              controller.TestControllerAvailability(requestContext1, host);
            }
          }
          foreach (BuildAgent agent in queryResult.Agents)
          {
            if (TFStringComparer.ArtiFactUrl.Equals(agent.ServiceHostUri, host.Uri))
            {
              agent.Url = host.GetUrlForService(agent.Uri);
              agent.TestAgentAvailability(requestContext1, host);
            }
          }
        }
        return RosarioHelper.Convert(queryResult);
      }
      catch (Exception ex)
      {
        BuildServiceHostQueryResult2010 hostQueryResult2010 = new BuildServiceHostQueryResult2010();
        hostQueryResult2010.Failures.Add(new Failure2010(ex));
        return hostQueryResult2010;
      }
    }

    public static void UpdateBuildAgents(
      IVssRequestContext requestContext,
      IList<BuildAgentUpdateOptions2010> updates)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildAgentUpdateOptions2010>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      IList<BuildAgentUpdateOptions> list = (IList<BuildAgentUpdateOptions>) updates.Select<BuildAgentUpdateOptions2010, BuildAgentUpdateOptions>((Func<BuildAgentUpdateOptions2010, BuildAgentUpdateOptions>) (x => RosarioHelper.Convert(x))).ToList<BuildAgentUpdateOptions>();
      requestContext.GetService<TeamFoundationBuildResourceService>().UpdateBuildAgents(requestContext, list, fromDev10Client: true);
    }

    internal static void UpdateBuildControllers(
      IVssRequestContext requestContext,
      IList<BuildControllerUpdateOptions2010> updates)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildControllerUpdateOptions2010>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      IList<BuildControllerUpdateOptions> list = (IList<BuildControllerUpdateOptions>) updates.Select<BuildControllerUpdateOptions2010, BuildControllerUpdateOptions>((Func<BuildControllerUpdateOptions2010, BuildControllerUpdateOptions>) (x => RosarioHelper.Convert(x))).ToList<BuildControllerUpdateOptions>();
      requestContext.GetService<TeamFoundationBuildResourceService>().UpdateBuildControllers(requestContext, list, fromDev10Client: true);
    }

    public static List<BuildDefinition2010> UpdateBuildDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinition2010> updates)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildDefinition2010>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      IList<BuildDefinition> list = (IList<BuildDefinition>) updates.Select<BuildDefinition2010, BuildDefinition>((Func<BuildDefinition2010, BuildDefinition>) (x => RosarioHelper.Convert(x))).ToList<BuildDefinition>();
      return requestContext.GetService<TeamFoundationBuildService>().UpdateBuildDefinitions(requestContext, list, false).Select<BuildDefinition, BuildDefinition2010>((Func<BuildDefinition, BuildDefinition2010>) (x => RosarioHelper.Convert(x))).ToList<BuildDefinition2010>();
    }

    internal static List<BuildInformationNode2010> UpdateBuildInformation(
      IVssRequestContext requestContext,
      IList<InformationChangeRequest2010> changes)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateBuildInformation));
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      List<InformationChangeRequest> changes1 = new List<InformationChangeRequest>();
      using (CompatibilityComponent component = requestContext.CreateComponent<CompatibilityComponent>("Build"))
      {
        IDictionary<string, string> dictionary = component.ResolveBuildUris((ICollection<string>) changes.Select<InformationChangeRequest2010, string>((Func<InformationChangeRequest2010, string>) (x => x.BuildUri)).ToArray<string>(), false);
        foreach (InformationChangeRequest2010 change in (IEnumerable<InformationChangeRequest2010>) changes)
        {
          string str;
          if (!dictionary.TryGetValue(change.BuildUri, out str))
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Invalid build uri '{0}'", (object) change.BuildUri);
            throw new InvalidBuildUriException(change.BuildUri);
          }
          InformationChangeRequest informationChangeRequest = RosarioHelper.Convert(change);
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Converted build uri '{0}' to '{1}'", (object) change.BuildUri, (object) str);
          informationChangeRequest.BuildUri = str;
          changes1.Add(informationChangeRequest);
        }
      }
      List<BuildInformationNode> source = service.UpdateBuildInformation(requestContext, (IList<InformationChangeRequest>) changes1);
      requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateBuildInformation));
      return source.Select<BuildInformationNode, BuildInformationNode2010>((Func<BuildInformationNode, BuildInformationNode2010>) (x => RosarioHelper.Convert(x))).ToList<BuildInformationNode2010>();
    }

    internal static void UpdateBuildServiceHost(
      IVssRequestContext requestContext,
      BuildServiceHostUpdateOptions2010 update)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, nameof (update), (IValidatable) update, false, ValidationContext.Update);
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildServiceHostUpdateOptions hostUpdateOptions = RosarioHelper.Convert(update);
      IVssRequestContext requestContext1 = requestContext;
      BuildServiceHostUpdateOptions update1 = hostUpdateOptions;
      service.UpdateBuildServiceHost(requestContext1, update1);
    }

    public static List<BuildDetail2010> UpdateBuilds(
      IVssRequestContext requestContext,
      IList<BuildUpdateOptions2010> updateOptions)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (UpdateBuilds));
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<BuildUpdateOptions2010>(requestContext, nameof (updateOptions), updateOptions, false, ValidationContext.Update);
      List<BuildUpdateOptions> updateOptions1 = new List<BuildUpdateOptions>();
      IDictionary<string, string> dictionary1 = (IDictionary<string, string>) new Dictionary<string, string>();
      using (CompatibilityComponent component = requestContext.CreateComponent<CompatibilityComponent>("Build"))
      {
        IDictionary<string, string> dictionary2 = component.ResolveBuildUris((ICollection<string>) updateOptions.Select<BuildUpdateOptions2010, string>((Func<BuildUpdateOptions2010, string>) (x => x.Uri)).ToArray<string>(), false);
        foreach (BuildUpdateOptions2010 updateOption in (IEnumerable<BuildUpdateOptions2010>) updateOptions)
        {
          string key;
          if (!dictionary2.TryGetValue(updateOption.Uri, out key))
          {
            requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Invalid build uri '{0}'", (object) updateOption.Uri);
            throw new InvalidBuildUriException(updateOption.Uri);
          }
          dictionary1.Add(key, updateOption.Uri);
          updateOptions1.Add(new BuildUpdateOptions()
          {
            BuildNumber = updateOption.BuildNumber,
            CompilationStatus = RosarioHelper.Convert(updateOption.CompilationStatus),
            DropLocation = updateOption.DropLocation,
            Fields = RosarioHelper.Convert(updateOption.Fields),
            KeepForever = updateOption.KeepForever,
            LabelName = updateOption.LabelName,
            LogLocation = updateOption.LogLocation,
            Quality = updateOption.Quality,
            SourceGetVersion = updateOption.SourceGetVersion,
            Status = RosarioHelper.Convert(updateOption.Status),
            TestStatus = RosarioHelper.Convert(updateOption.TestStatus),
            Uri = key
          });
          requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Converted build uri '{0}' to '{1}'", (object) updateOption.Uri, (object) key);
        }
      }
      List<BuildDetail> buildDetailList = requestContext.GetService<TeamFoundationBuildService>().UpdateBuilds(requestContext, (IList<BuildUpdateOptions>) updateOptions1, new Guid());
      List<BuildDetail2010> buildDetail2010List = new List<BuildDetail2010>();
      foreach (BuildDetail build in buildDetailList)
      {
        string str;
        if (!dictionary1.TryGetValue(build.Uri, out str))
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Invalid build uri '{0}'", (object) build.Uri);
          throw new InvalidBuildUriException(build.Uri);
        }
        build.Uri = str;
        buildDetail2010List.Add(RosarioHelper.Convert(build));
      }
      requestContext.TraceLeave(0, "Build", "Service", nameof (UpdateBuilds));
      return buildDetail2010List;
    }

    public static List<ProcessTemplate2010> UpdateProcessTemplates(
      IVssRequestContext requestContext,
      IList<ProcessTemplate2010> processTemplates)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<ProcessTemplate2010>(requestContext, nameof (processTemplates), processTemplates, false, ValidationContext.Update);
      IList<ProcessTemplate> list = (IList<ProcessTemplate>) processTemplates.Select<ProcessTemplate2010, ProcessTemplate>((Func<ProcessTemplate2010, ProcessTemplate>) (x => RosarioHelper.Convert(x))).ToList<ProcessTemplate>();
      return requestContext.GetService<TeamFoundationBuildService>().UpdateProcessTemplates(requestContext, list).Select<ProcessTemplate, ProcessTemplate2010>((Func<ProcessTemplate, ProcessTemplate2010>) (x => RosarioHelper.Convert(x))).ToList<ProcessTemplate2010>();
    }

    public static List<QueuedBuild2010> UpdateQueuedBuilds(
      IVssRequestContext requestContext,
      IList<QueuedBuildUpdateOptions2010> updates)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable<QueuedBuildUpdateOptions2010>(requestContext, nameof (updates), updates, false, ValidationContext.Update);
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      IList<QueuedBuildUpdateOptions> list = (IList<QueuedBuildUpdateOptions>) updates.Select<QueuedBuildUpdateOptions2010, QueuedBuildUpdateOptions>((Func<QueuedBuildUpdateOptions2010, QueuedBuildUpdateOptions>) (x => RosarioHelper.Convert(x))).ToList<QueuedBuildUpdateOptions>();
      IVssRequestContext requestContext1 = requestContext;
      IList<QueuedBuildUpdateOptions> updates1 = list;
      Guid projectId = new Guid();
      return RosarioHelper.Convert(service.UpdateQueuedBuilds(requestContext1, updates1, projectId)).Builds;
    }

    public static void FixUriForBuildMachine(
      IVssRequestContext requestContext,
      BuildDetail2010 currentBuild)
    {
      if (requestContext.UserAgent == null || requestContext.UserAgent.IndexOf("TFSBuildServiceHost.exe", StringComparison.OrdinalIgnoreCase) < 0 && requestContext.UserAgent.IndexOf("MSBuild.exe", StringComparison.OrdinalIgnoreCase) < 0)
        return;
      int length = currentBuild.Uri.IndexOf('?');
      if (length <= 0)
        return;
      currentBuild.Uri = currentBuild.Uri.Substring(0, length);
    }
  }
}
