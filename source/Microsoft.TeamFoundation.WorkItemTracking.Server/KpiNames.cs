// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.KpiNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct KpiNames
  {
    internal const string GetWorkItem = "GetWorkItem";
    internal const string GetWorkItemCount = "GetWorkItemCount";
    internal const string PageWorkItem = "PageWorkItem";
    internal const string FieldsPaged = "FieldsPaged";
    internal const string UpdateWorkItem = "UpdateWorkItem";
    internal const string UpdateWorkItemNoLongText = "UpdateWorkItemNoLongText";
    internal const string UpdateWorkItemNoLongTable = "UpdateWorkItemNoLongTable";
    internal const string UpdateWorkItemNoIdentity = "UpdateWorkItemNoIdentity";
    internal const string UpdateWorkItemNoCacheRefresh = "UpdateWorkItemNoCacheRefresh";
    internal const string UpdateWorkItemCount = "UpdateWorkItemCount";
    internal const string ExecuteQuery = "ExecuteQuery";
    internal const string QueryResultCount = "QueryResultCount";
    internal const string ExecuteQueryWithIdentityInGroupKpi = "ExecuteQueryWithIdentityInGroup";
    internal const string GetMetadataTimeKpi = "GetMetadataTime";
    internal const string GetMetadataRowCountKpi = "GetMetadataRowCount";
    internal const string ReconcileExtensionKpi = "ReconcileExtension";
  }
}
