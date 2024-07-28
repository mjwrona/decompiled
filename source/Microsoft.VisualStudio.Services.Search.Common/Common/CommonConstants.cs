// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CommonConstants
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class CommonConstants
  {
    public static readonly char DirectorySeparatorCharacter = Path.AltDirectorySeparatorChar;
    public static readonly string DirectorySeparatorString = CommonConstants.DirectorySeparatorCharacter.ToString();
    public const string BranchNamePrefix = "refs/heads/";
    public const string MetaDataStoreFileExtensionRegex = "*.tstore";
    public const string ParseStoreRootDir = "ParseStore";
    public static readonly int NumberOfArgumentsForParserExecutable = 7;
    public static readonly int ReturnCodeForShellExecutableFailed = -1;
    public static readonly int ReturnCodeForShellExecutableOnParserLoadFailure = -2;
    public static readonly int ReturnCodeForShellExecutableSuccessful = 0;
    public static readonly int MaxNumberOfBucketsInTermsAggregations = 5000;
    public const string OrganizationNameKey = "OrganizationName";
    public const string CollectionNameKey = "CollectionName";
    public const string ProjectNameKey = "ProjectName";
    public const string ProjectVisibilityKey = "ProjectVisibility";
    public const string VcTypeKey = "VcType";
    public const string RepoNameKey = "RepoName";
    public const string BranchNameKey = "BranchName";
    public const string DefaultBranchNameKey = "DefaultBranchName";
    public const string IsDefaultBranchKey = "IsDefaultBranch";
    public const string ParsedRelevanceContentKey = "ParsedRelevanceContentKey";
    public const string LatestChangeIdKey = "LatestChangeIdKey";
    public const string LatestChangeUtcTimeKey = "LatestChangeUtcTimeKey";
    public const string NormalizedOrganizationNameKey = "NormalizedOrganizationName";
    public const string NormalizedCollectionNameKey = "NormalizedCollectionName";
    public const string NormalizedProjectNameKey = "NormalizedProjectName";
    public const string NormalizedRepoNameKey = "NormalizedRepoName";
    public const string NormalizedBranchNameKey = "NormalizedBranchName";
    public const string NormalizedDefaultBranchNameKey = "NormalizedDefaultBranchName";
    public const string NormalizedLatestCommitIdKey = "NormalizedLatestCommitIdKey";
    public const string CollectionIdKey = "CollectionId";
    public const string OrganizationIdKey = "OrganizationId";
    public const string ProjectIdKey = "ProjectId";
    public const string RepoIdKey = "RepoId";
    public const string BranchIdKey = "BranchId";
    public const string Items = "Items";
    public const string IsDocumentDeletedInReindexing = "isDocumentDeletedInReIndexing";
    public const string WorkItemFieldsKey = "WorkItemFields";
    public const string True = "true";
    public const string SearchBaseConfigPath = "/Service/ALMSearch/Settings/";
    public const string Mapping = "Mapping";
    public const string SourceNoDedupeFileContract = "SourceNoDedupeFileContract";
    public const string DedupeFileContract = "DedupeFileContract";
    public const string DefaultCodeDocumentContractType = "DefaultCodeDocumentContractType";
    public const string Unknown = "Unknown";
    public const string RootFolderRoutingValue = "rootfolder";
    public const string IndexVersionKey = "version";
    public const string SearchSecurityNamespaceId = "ca535e7e-67ce-457f-93fe-6e53aa4e4160";
    public const string SearchSecurityTokenForAuthenticatedUsers = "/SearchValidUserData";
    public const string SearchSecurityToken = "";
    public static readonly int SearchReadPermissionBitForMembers = 1;
    public static readonly int SearchReadPermissionBitForPublicUsers = 2;
    public const string SearchSecurityTokenKey = "searchServiceSecurityTokenKey";
    public const string SearchSecurityPermissionKey = "searchServiceSecurityPermissionKey";
    public const string SearchSecurityNamespaceGuidkey = "searchServiceSecurityNamespaceGuidKey";
    public const string IsUserAnonymousKey = "isUserAnonymousKey";
    public const string IncludeSnippetInCodeSearchKey = "includeSnippetInCodeSearchKey";
    public static readonly Guid SearchSecurityNamespaceGuid = new Guid("ca535e7e-67ce-457f-93fe-6e53aa4e4160");
    public const string FeedSecurityNamespaceId = "9FED0191-DCA2-4112-86B7-A6A48D1B204C";
    public static readonly Guid FeedSecurityNamespaceGuid = new Guid("9FED0191-DCA2-4112-86B7-A6A48D1B204C");
    public const int MaxThresholdCountForTeamsApiCall = 100;
    public const int TopCountForTeamsApiCall = 1000;
    public const string DefaultTermExpressionType = "*";
    public const string RegexpTermExpressionType = "regex";
    public const string ColonSeparator = ":";
    public const int NumberOfShards = 3;
    public static readonly int CodeTokenKindCount = Enum.GetNames(typeof (CodeTokenKind)).Length;
    public const string MetadataContentKey = "Metadata";
    public const string ReadMeRawContentKey = "ReadMe";
    public const string DescriptionContentKey = "Description";
    public const string ChildInfoContentKey = "ChildInfo";
    public const string Data = "data";
    public const string EntityType = "EntityType";
    public const string QueryContractType = "queryContractType";
    public const string IndexContractType = "indexContractType";
    public const string IndexESConnectionString = "indexESConnectionString";
    public const string QueryESConnectionString = "queryESConnectionString";
    public const string IndexIndices = "indexIndices";
    public const string QueryIndices = "queryIndices";
    public const string RegistryJobServiceSettingsQuery = "/Configuration/JobService/*";
    public const string RegistryResourceCalculatorSettingsQuery = "/Service/ALMSearch/Settings/*";
    public const string RawFieldNameSuffix = "raw";
    public const string WorkItemFieldIndexCriteria = "-WEF_????????????????????????????????_*,-System.IterationLevel*,-System.AreaLevel*,-System.Watermark";
    public const string Git = "git";
    public const string Tfvc = "tfvc";
    public const string Custom = "custom";
    public const string ProjectRenameRegistryPrefix = "ProjectRename";
    public const string HighPriorityAccountRegistryPath = "/AccountIndexingPriority/PriorityValue";
    public const string CodesearchSharedIndexNamePrefix = "codesearchshared";
    public const string ESRedStateExceptionMessage = "The state of the Elastic Search Cluster is red.";
    public const string ESYellowStateExceptionMessage = "The state of the Elastic Search Cluster is yellow.";
    public const string ESNestNonGenericExceptionMessage = "A non-generic exception was through from the Elastic Search NEST client.";
    public const string ESRejectedExecutionExceptionMessage = "es_rejected_execution_exception";
    public const string FileSizeTooLargeMsg = "SD-Indexing : File Size is too large.";
    public const string UnsupportedFileMsg = "SD-Indexing : Unsupported extension.";
    public const string BinaryFileMsg = "SD-Indexing : Detected as Binary.";
    public const string NotAbleToIndexMsg = "SD-Indexing : Something went wrong while indexing";
    public const string InvalidFileDetailsMsg = "SD-Crawler : Invalid File details";
    public const string ExpectedTfvcRootElement = "$/";
    public const string HostCleanup = "HostCleanup";
    public const string CollectionSoftDeletePrefix = "SOFTDELETE-";
    public const string CollectionSoftDeleteDGuidFormat = "D{0}";
    public const string GitRepositoryRoot = "/";
    public const string TfvcRepositoryUrlSuffix = "/_versionControl";
    public const string TfvcRepoDocumentIdSuffix = "_TFVC";
    public const string LargeRepositoryMultipeBranchIndexing = "LargeRepositoryMultipeBranchIndexing";
    public const string AllEntity = "All";
    public const string CodeEntity = "Code";
    public const string ProjectRepoEntity = "ProjectRepo";
    public const string TenantCodeEntity = "TenantCode";
    public const string TenantWikiEntity = "TenantWiki";
    public const string WorkItemEntity = "WorkItem";
    public const string FileEntity = "File";
    public const string WikiEntity = "Wiki";
    public const string PackageEntity = "Package";
    public const string BoardEntity = "Board";
    public const string SettingEntity = "Setting";
    public const int AllEntityID = 0;
    public const int CodeEntityID = 1;
    public const int ProjectRepoEntityID = 2;
    public const int TenantCodeEntityID = 3;
    public const int WorkItemEntityID = 4;
    public const int FileEntityID = 5;
    public const int WikiEntityID = 6;
    public const int PackageEntityID = 7;
    public const int BoardEntityID = 8;
    public const int TenantWikiEntityID = 9;
    public const int SettingEntityID = 10;
    public static readonly IReadOnlyCollection<Type> KnownEntityTypesList = (IReadOnlyCollection<Type>) new Type[11]
    {
      typeof (AllEntityType),
      typeof (CodeEntityType),
      typeof (FileEntityType),
      typeof (PackageEntityType),
      typeof (ProjectRepoEntityType),
      typeof (TenantCodeEntityType),
      typeof (TenantWikiEntityType),
      typeof (WikiEntityType),
      typeof (WorkItemEntityType),
      typeof (BoardEntityType),
      typeof (SettingEntityType)
    };
    public static readonly IReadOnlyCollection<IEntityType> KnownEntitiesList = (IReadOnlyCollection<IEntityType>) new IEntityType[11]
    {
      (IEntityType) AllEntityType.GetInstance(),
      (IEntityType) CodeEntityType.GetInstance(),
      (IEntityType) ProjectRepoEntityType.GetInstance(),
      (IEntityType) TenantCodeEntityType.GetInstance(),
      (IEntityType) TenantWikiEntityType.GetInstance(),
      (IEntityType) WorkItemEntityType.GetInstance(),
      (IEntityType) FileEntityType.GetInstance(),
      (IEntityType) WikiEntityType.GetInstance(),
      (IEntityType) PackageEntityType.GetInstance(),
      (IEntityType) BoardEntityType.GetInstance(),
      (IEntityType) SettingEntityType.GetInstance()
    };
    public const string IndexingUnitType_Collection = "Collection";
    public const string IndexingUnitType_Project = "Project";
    public const string IndexingUnitType_Git_Repository = "Git_Repository";
    public const string IndexingUnitType_TFVC_Repository = "TFVC_Repository";
    public const string IndexingUnitType_CustomRepository = "CustomRepository";
    public const string IndexingUnitType_ScopedIndexingUnit = "ScopedIndexingUnit";
    public const string IndexingUnitType_TemporaryIndexingUnit = "TemporaryIndexingUnit";
    public const string IndexingUnitType_Organization = "Organization";
    public const string IndexingUnitType_Feed = "Feed";
    public const string IndexingUnitType_Git_RepositoryShadow = "Git_Repository*";
    public const string IndexingUnitType_TFVC_RepositoryShadow = "TFVC_Repository*";
    public const string IndexingUnitType_CustomRepositoryShadow = "CustomRepository*";
    public const string IndexingUnitType_ScopedIndexingUnitShadow = "ScopedIndexingUnit*";
    public const string IndexingUnitType_TemporaryIndexingUnitShadow = "TemporaryIndexingUnit*";
    public static readonly IReadOnlyCollection<string> AllIndexingUnitTypes = (IReadOnlyCollection<string>) new string[9]
    {
      "Collection",
      "Project",
      "Git_Repository",
      "TFVC_Repository",
      "CustomRepository",
      "ScopedIndexingUnit",
      "TemporaryIndexingUnit",
      "Organization",
      "Feed"
    };
    public static readonly IReadOnlyCollection<Type> KnownIndexingPropertiesList = (IReadOnlyCollection<Type>) new Type[12]
    {
      typeof (CollectionIndexingProperties),
      typeof (ProjectCodeIndexingProperties),
      typeof (TfvcCodeRepoIndexingProperties),
      typeof (GitCodeRepoIndexingProperties),
      typeof (CustomRepoCodeIndexingProperties),
      typeof (ProjectWorkItemIndexingProperties),
      typeof (ScopedGitRepositoryIndexingProperties),
      typeof (CollectionProjectRepoIndexingProperties),
      typeof (OrganizationIndexingProperties),
      typeof (FeedIndexingProperties),
      typeof (CollectionPackageIndexingProperties),
      typeof (CollectionBoardIndexingProperties)
    };
    public static readonly IReadOnlyCollection<Type> KnownTFSEntityAttributesList = (IReadOnlyCollection<Type>) new Type[11]
    {
      typeof (CollectionAttributes),
      typeof (CodeRepoTFSAttributes),
      typeof (TfvcCodeRepoTFSAttributes),
      typeof (CustomRepoCodeTFSAttributes),
      typeof (GitCodeRepoTFSAttributes),
      typeof (ProjectCodeTFSAttributes),
      typeof (ProjectWorkItemTFSAttributes),
      typeof (ScopedGitRepositoryAttributes),
      typeof (OrganizationAttributes),
      typeof (FeedTFSAttributes),
      typeof (BoardTFSAttributes)
    };
    public static readonly IReadOnlyCollection<Type> KnownChangeEventDataList = (IReadOnlyCollection<Type>) new Type[44]
    {
      typeof (CodeBalanceShardsEventData),
      typeof (CodeBulkIndexEventData),
      typeof (CodeDeleteOrphanFilesEventData),
      typeof (CodeIndexPublishData),
      typeof (CodeMetadataCrawlEventData),
      typeof (CodeProjectRenameEventData),
      typeof (CollectionProjectRepoProjectCrudEventData),
      typeof (CollectionProjectRepoRepoCrudEventData),
      typeof (CompleteGitBranchDeleteEventData),
      typeof (CustomRepositoryIndexRequestEventData),
      typeof (DeleteIndexEventData),
      typeof (EntityRenameEventData),
      typeof (FileSplitGroupData),
      typeof (GitBranchDeleteEventData),
      typeof (GitBranchAddedEventData),
      typeof (GitCodeContinuousIndexEventData),
      typeof (GitRepositoryAcesSyncEventData),
      typeof (GitRepositoryBIEventData),
      typeof (GitRepositoryCustomBulkIndexingEventData),
      typeof (LargeRepositoryMetadataCrawlerEventData),
      typeof (PackageBulkIndexEventData),
      typeof (PackageContinuousIndexEventData),
      typeof (PackageMetadataDeleteIndexEventData),
      typeof (ProjectEventData),
      typeof (ProjectRepoMetadataCrawlEventData),
      typeof (ProjectRepoMetadataDeleteIndexEventData),
      typeof (RepositoryPatchEventData),
      typeof (TFVCCodeContinuousIndexEventData),
      typeof (UpdateMetadataEventData),
      typeof (WikiBulkIndexEventData),
      typeof (WikiIndexPublishData),
      typeof (WikiMetadataCrawlEventData),
      typeof (WikiMetadataDeleteIndexEventData),
      typeof (WorkItemBulkIndexEventData),
      typeof (WorkItemClassificationNodesEventData),
      typeof (WorkItemContinuousIndexEventData),
      typeof (WorkItemDestroyedEventData),
      typeof (WorkItemIndexPublishData),
      typeof (WorkItemMetadataCrawlEventData),
      typeof (WorkItemPatchEventData),
      typeof (WorkItemPatchEventData),
      typeof (WorkItemUpdateFieldValuesEventData),
      typeof (WorkItemUpdateIndexEventData),
      typeof (BoardBulkIndexEventData)
    };
    public const string PlatformUserKey = "User";
    public const string PlatformPasswordKey = "Password";
    public const string ElasticsearchPasswordDrawer = "ElasticsearchPasswordDrawer";
    public const string ElasticsearchPassword = "ElasticsearchPassword";
    public const string ElasticSearchHostedAuthPassword = "ElasticSearchHostedAuthPassword";
    public const string SearchTempStorePathEnvVar = "AZUREDEVOPS_SEARCH_TEMPSTOREPATH";
    public const string GenericFeederName = "GenericFeeder";
    public const string CodeParserName = "CodeParser";
    public const string WikiParserName = "WikiParser";
    public const string CodeCrawlerName = "CodeCrawler";
    public const string TfvcPatchHttpCrawlerName = "TfvcPatchHttpCrawler";
    public const string TfvcRepositoryChangesetCrawler = "TfvcRepositoryChangesetCrawler";
    public const char NewlineChar = '\n';
    public const char CarriageReturnChar = '\r';
    public const string AzureStorageTableConnectionString = "AzureStorageTableConnectionString";

    public static class AuditTrailMessages
    {
      public const string FileNameNotIndexedDueToUnsupportedFileExtension = "FileNameNotIndexedDueToUnsupportedFileExtension";
      public const string ContentNotIndexedDueToUnsupportedFileExtension = "ContentNotIndexedDueToUnsupportedFileExtension";
      public const string ContentNotIndexedDueToLargeSize = "ContentNotIndexedDueToLargeSize";
      public const string FileNameNotIndexedDueToLargeSize = "FileNameNotIndexedDueToLargeSize";
      public const string FileInUnsupportedFolder = "FileInUnsupportedFolder";
      public const string FileNotPartOfAnyWiki = "FileNotPartOfAnyWiki";
      public const string FailedToFindFileInCrawlStore = "FailedToFindFileInCrawlStore";
      public const string FailedToAddFileToContentStore = "FailedToAddFileToContentStore";
    }
  }
}
