// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingServicesConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class WorkItemTrackingServicesConstants
  {
    public const string WorkItemTracking = "WorkItemTracking";
    public const int WorkItemDefaultPageSize = 200;
    public const int MaxQueryDepth = 64;
    public const int QueryItemSearchLimit = 200;
    public const int QueryItemSearchDefault = 50;
    public const string EmptyTexts = "/Service/WorkItemTracking/Settings/EmptyText/*";
    public const int WorkItemDestroyBatchSize = 2048;
    public const int WorkItemDependencyViolationJobDelayDefault = 60;
    public const string WorkItemDependencyViolationJobDelayKey = "/Service/WorkItemTracking/Settings/DependencyViolationJobLimit";
    public const int WorkItemDependencyViolationMaxBackupDaysDefault = -30;
    public const string WorkItemDependencyViolationLastBackupDate = "/Service/WorkItemTracking/Settings/DependencyViolationBackupDate";
    public const int DaysToKeepDeletedQueriesDefault = 30;
    public const string DaysToKeepDeletedQueries = "/Service/WorkItemTracking/Settings/DaysToKeepDeletedQueries";
    public const int WorkItemsDeleteBatchSize = 200;
  }
}
