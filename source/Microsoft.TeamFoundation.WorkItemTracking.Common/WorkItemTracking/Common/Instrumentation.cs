// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Instrumentation
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct Instrumentation
  {
    public const string LoggingSource = "TFS WorkItem Tracking";
    public const string CounterCategory = "TFS WorkItem Tracking";
    public const string ActiveQueryRequests = "Active Query Requests";
    public const string ActivePagingRequests = "Active Paging Requests";
    public const string ActiveGetWorkitemRequests = "Active GetWorkitem Requests";
    public const string ActiveUpdateRequests = "Active Update Requests";
    public const string ActiveGetMetadataRequests = "Active GetMetadata Requests";
    public const string ActiveGetStoredQueryRequests = "Active GetStoredQuery Requests";
    public const string ActiveGetStoredQueriesRequests = "Active GetStoredQueries Requests";
    public const string ActiveGetQueryAccessControlListRequests = "Active GetQueryAccessControlList Requests";
  }
}
