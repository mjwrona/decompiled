// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Operation
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class Operation
  {
    public const string CrawlMetadata = "CrawlMetadata";
    public const string UpdateMetadata = "UpdateMetadata";
    public const string Add = "Add";
    public const string Delete = "Delete";
    public const string BeginBulkIndex = "BeginBulkIndex";
    public const string CompleteBulkIndex = "CompleteBulkIndex";
    public const string UpdateIndex = "UpdateIndex";
    public const string BeginMigrateIndex = "BeginMigrateIndex";
    public const string CompleteMigrateIndex = "CompleteMigrateIndex";
    public const string BeginProjectRename = "BeginProjectRename";
    public const string CustomGitRepositoryBulkIndex = "CustomGitRepositoryBulkIndex";
    public const string ReIndex = "ReIndex";
    public const string UpdateField = "UpdateField";
    public const string BranchDelete = "BranchDelete";
    public const string DeleteOrphanFiles = "DeleteOrphanFiles";
    public const string Patch = "Patch";
    public const string BeginEntityRename = "BeginEntityRename";
    public const string CompleteEntityRename = "CompleteEntityRename";
    public const string GitRepositorySecurityAcesSync = "GitRepositorySecurityAcesSync";
    public const string UpdateIndexingUnitProperties = "UpdateIndexingUnitProperties";
    public const string SyncAllClassificationNode = "SyncAllClassificationNode";
    public const string AddClassificationNode = "AddClassificationNode";
    public const string UpdateClassificationNode = "UpdateClassificationNode";
    public const string AreaNodeSecurityAcesSync = "AreaNodeSecurityAcesSync";
    public const string PackageSecurityAcesSync = "FeedSecurityAcesSync";
    public const string UpdateSearchUrlInIndexingUnitProperties = "UpdateSearchUrlInIndexingUnitProperties";
    public const string ResetBranchesInGitRepoAttributes = "ResetBranchesInGitRepoAttributes";
    public const string ProjectDeleteFromCollection = "ProjectDeleteFromCollection";
    public const string RepositoryDeleteFromCollection = "RepositoryDeleteFromCollection";
    public const string ProjectUpdateInCollection = "ProjectUpdateInCollection";
    public const string RepositoryRenameInCollection = "RepositoryRenameInCollection";
    public const string UpdateWorkItemFieldValues = "UpdateWorkItemFieldValues";
    public const string CompleteBranchDelete = "CompleteBranchDelete";
    public const string CleanUpFeeds = "CleanUpFeeds";
    public const string AssignRouting = "AssignRouting";
    public const string ExtensionBeginUninstall = "ExtensionBeginUninstall";
    public const string ExtensionFinalizeUninstall = "ExtensionFinalizeUninstall";
    public const string Destroy = "Destroy";
    public const string DeleteDuplicateDocuments = "DeleteDuplicateDocuments";
    public const string FeedUpdates = "FeedUpdates";
  }
}
