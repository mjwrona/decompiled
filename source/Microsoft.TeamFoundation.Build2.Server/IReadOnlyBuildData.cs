// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IReadOnlyBuildData
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public interface IReadOnlyBuildData
  {
    string BuildNumber { get; }

    int? BuildNumberRevision { get; }

    bool ChangesCalculated { get; }

    MinimalBuildDefinition Definition { get; }

    bool Deleted { get; }

    Guid? DeletedBy { get; }

    DateTime? DeletedDate { get; }

    string DeletedReason { get; }

    List<Demand> Demands { get; }

    DateTime? FinishTime { get; }

    int Id { get; }

    Guid LastChangedBy { get; }

    DateTime LastChangedDate { get; }

    LogReference Logs { get; }

    TaskOrchestrationPlanReference OrchestrationPlan { get; }

    string Parameters { get; }

    List<TaskOrchestrationPlanReference> Plans { get; }

    QueuePriority Priority { get; }

    Guid ProjectId { get; }

    PropertiesCollection Properties { get; }

    int? QueueId { get; }

    QueueOptions QueueOptions { get; }

    DateTime? QueueTime { get; }

    BuildReason Reason { get; }

    MinimalBuildRepository Repository { get; }

    Guid RequestedBy { get; }

    Guid RequestedFor { get; }

    BuildResult? Result { get; }

    string SourceBranch { get; }

    string SourceVersion { get; }

    SourceVersionInfo SourceVersionInfo { get; }

    string SourceVersionInfoString { get; }

    DateTime? StartTime { get; }

    BuildStatus? Status { get; }

    List<string> Tags { get; }

    Dictionary<string, object> TemplateParameters { get; }

    TriggeredByBuild TriggeredByBuild { get; }

    IDictionary<string, string> TriggerInfo { get; }

    string TriggerInfoString { get; }

    bool AppendCommitMessageToRunName { get; }

    Uri Uri { get; }

    List<BuildRequestValidationResult> ValidationResults { get; }

    string ValidationResultsString { get; }

    List<RetentionLease> RetentionLeases { get; }
  }
}
