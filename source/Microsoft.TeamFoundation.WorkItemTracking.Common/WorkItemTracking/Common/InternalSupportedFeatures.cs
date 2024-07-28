// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.InternalSupportedFeatures
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class InternalSupportedFeatures
  {
    public const string GuidFields = "GuidFields";
    public const string BooleanFields = "BooleanFields";
    public const string IdentityFields = "IdentityFields";
    public const string QueryFolders = "QueryFolders";
    public const string QueryFolderPermissions = "QueryFolderPermissions";
    public const string QueryFolderSetOwner = "QueryFolderSetOwner";
    public const string QueryFieldsComparison = "QueryFieldsComparison";
    public const string QueryHistoricalRevisions = "QueryHistoricalRevisions";
    public const string QueryInGroupFilter = "QueryInGroup";
    public const string WorkItemTypeCategories = "WorkItemTypeCategories";
    public const string WorkItemLinks = "WorkItemLinks";
    public const string WorkItemLinkLocks = "WorkItemLinkLocks";
    public const string BatchSaveWorkItemsFromDifferentProjects = "BatchSaveWorkItemsFromDifferentProjects";
    public const string SyncNameChanges = "SyncNameChanges";
    public const string ReportingNames = "ReportingNames";
    public const string SetReportingTypeToNone = "SetReportingTypeToNone";
    public const string GlobalWorkflow = "GlobalWorkflow";
    public const string QueryRecursiveReturnMatchingChildren = "QueryRecursiveReturnMatchingChildren";
    public const string ProvisionPermission = "ProvisionPermission";
    public const string ConfigurableBulkUpdateBatchSize = "ConfigurableBulkUpdateBatchSize";
  }
}
