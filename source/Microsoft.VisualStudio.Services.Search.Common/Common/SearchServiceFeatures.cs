// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchServiceFeatures
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class SearchServiceFeatures
  {
    public const string CodeIndexing = "Search.Server.Code.Indexing";
    public const string CodeCrudOperations = "Search.Server.Code.CrudOperations";
    public const string WorkItemIndexing = "Search.Server.WorkItem.Indexing";
    public const string WorkItemCrudOperations = "Search.Server.WorkItem.CrudOperations";
    public const string FaultManagement = "Search.Server.FaultManagement";
    public const string SecurityChecksCache = "Search.Server.SecurityChecksCache";
    public const string WikiSecurityChecksCache = "Search.Server.WikiSecurityChecksCache";
    public const string WorkItemSecurityChecksCache = "Search.Server.WorkItemSecurityChecksCache";
    public const string PackageSecurityChecksCache = "Search.Server.PackageSecurityChecksCache";
    public const string GitRepositoryHealer = "Search.Server.GitRepositoryHealer";
    public const string TfvcRepositoryHealer = "Search.Server.TfvcRepositoryHealer";
    public const string JobResourceUtilizationControl = "Search.Server.JobResourceUtilizationControl";
    public const string IndexingUnitLocking = "Search.Server.IndexingUnitLocking";
    public const string WorkItemSkipDiscussionsCrawling = "Search.Server.WorkItem.SkipDiscussionsCrawling";
    public const string EnableRelevanceRulesEngine = "Search.Server.Relevance.RulesEngine";
    public const string LimitSearchResults = "Search.Server.LimitSearchResults";
    public const string ScopedQuery = "Search.Server.ScopedQuery";
    public const string ProjectScopedQuery = "Search.Server.ProjectScopedQuery";
    public const string WikiIndexing = "Search.Server.Wiki.Indexing";
    public const string WikiContinuousIndexing = "Search.Server.Wiki.ContinuousIndexing";
    public const string WikiProductDocumentationSearch = "Search.Server.Wiki.ProductDocumentationSearch";
    public const string PackageSearch = "Search.Server.Package.PackageSearch";
    public const string PackageIndexing = "Search.Server.Package.Indexing";
    public const string PackageContinuousIndexing = "Search.Server.Package.ContinuousIndexing";
    public const string IndexOriginalCodeContent = "Search.Server.IndexOriginalCodeContent";
    public const string IgnoreProjectAndRepoNotifications = "Search.Server.IgnoreProjectAndRepoNotifications";
    public const string CodeSearchDataProviderEnabled = "Search.Server.CodeSearchDataProviderEnabled";
    public const string WorkItemSearchDataProviderEnabled = "Search.Server.WorkItemSearchDataProviderEnabled";
    public const string WorkItemSearchQueryIdentityFields = "Search.Server.WorkItem.QueryIdentityFields";
    public const string CodeSearchPagination = "Search.Server.ScrollSearchQuery";
    public const string CodeZeroStalenessReindexingFeatureFlag = "Search.Server.Code.ShadowIndexing";
    public const string RedoSecurityCheckForBadTokensDisabled = "Search.Server.WorkItem.RedoSecurityCheckForBadTokensDisabled";
    public const string WorkItemZeroStalenessReindexingFeatureFlag = "Search.Server.WorkItem.ShadowIndexing";
    public const string UseDiscussionEditDeleteEndpoint = "Search.Server.WorkItem.UseDiscussionEditDeleteEndpoint";
    public const string BoardSearchIndexingFeatureFlag = "Search.Server.Board.Indexing";
    public const string BoardContinuousIndexing = "Search.Server.Board.ContinuousIndexing";
    public const string WorkItemRecentActivityCacheEnabled = "Search.Server.WorkItem.RecentActivityCacheEnabled";
    public const string WorkItemRecentActivityTrackingEnabled = "Search.Server.WorkItem.RecentActivityTracking";
    public const string ProximitySearchForCodeEnabled = "Search.Server.Code.ProximitySearch";
    public const string NoPayloadCodeSearchHighlighterV2Enabled = "Search.Server.Code.NoPayloadCodeSearchHighlighterV2";
    public const string WildcardConstantScoreRewriteEnabled = "Search.Server.Code.WildcardConstantScoreRewrite";
    public const string SearchSettingEnabled = "Search.Server.Setting.EnableSearchSettings";
    public const string DLITStrictValidationEnabled = "Search.Server.DLIT.StrictValidationEnabled";
    public const string DLITDocumentCreationEnabled = "Search.Server.DLIT.DocumentCreationEnabled";
    public const string CodeSearchBoostRelevanceEnabled = "Search.Server.Code.BoostRelevance";
    public const string CodeSearchWildCardPartialResults = "Search.Server.Code.WildCardPartialResults";
    public const string CodesearchV4HighlighterEnabled = "Search.Server.Code.CodesearchV4Highlighter";
    public const string CodeSearchIncludeSnippetInHits = "Search.Server.Code.IncludeSnippetInHits";
    public const string OrphanRepoCleanUp = "Search.Server.Code.OrphanRepoCleanUp";
    public const string FetchStalenessRetrial = "Search.Server.Code.FetchStalenessRetrials";
    public const string DisableESCleanUp = "Search.Server.Code.DisableESCleanUp";
    public const string ProjectScopedResultsCount = "Search.Server.Code.ProjectScopedResultsCount";
    public const string EnableFrameworkService = "Search.Server.Code.EnableFrameworkService";
    public const string EnableWorkItemReadReplica = "Search.Server.WorkItem.EnableReadReplica";
    public const string EnableWikiReadReplica = "Search.Server.Wiki.EnableReadReplica";
    public const string EnableGitReadReplica = "Search.Server.Git.EnableReadReplica";
    public const string EnableProjectReadReplica = "Search.Server.Project.EnableReadReplica";
    public const string EnableGetLatestRef = "Search.Server.Code.EnableGetLatestRef";
  }
}
