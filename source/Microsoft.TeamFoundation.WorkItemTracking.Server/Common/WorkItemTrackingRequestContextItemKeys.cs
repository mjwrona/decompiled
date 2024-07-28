// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingRequestContextItemKeys
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class WorkItemTrackingRequestContextItemKeys
  {
    public const string SqlTimingsKey = "WorkItemTracking.SqlTimings";
    public const string QueryIdKey = "queryId";
    public const string AsOfHistoryRange = "AsOfHistoryRange";
    public const string OptimizationInstanceInUse = "OptimizationInstanceInUse";
    public const string MockExecutionTimeInMs = "MockExecutionTimeInMs";
    public const string QueryOptimizationsFromRest = "QueryOptimizationsFromRest";
    public const string QueryText = "QueryText";
    public const string ProjectId = "ProjectId";
  }
}
