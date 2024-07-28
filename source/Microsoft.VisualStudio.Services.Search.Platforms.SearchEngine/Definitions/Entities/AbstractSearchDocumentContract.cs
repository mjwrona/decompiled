// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.AbstractSearchDocumentContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities
{
  public abstract class AbstractSearchDocumentContract
  {
    public const string ElasticSearchDocRoutingIdField = "_routing";
    public const string IsDocumentDeletedInReindexing = "isDocumentDeletedInReIndexing";
    public const string UpdatedDocumentParam = "doc";
    public const string IndexedTimeStampField = "indexedTimeStamp";
    public const string CollectionNameField = "collectionName";
    public const string CollectionIdField = "collectionId";
    public const string OrganizationNameField = "organizationName";
    public const string OrganizationIdField = "organizationId";
    public const string CollectionNameOriginalField = "collectionNameOriginal";
    public const string OrganizationNameOriginalField = "organizationNameOriginal";
    public const string BranchNameField = "branchName";
    public const string PathsBranchNameOriginalField = "paths.branchNameOriginal";
    public const string BranchNameOriginalField = "branchNameOriginal";
    public const string ProjectNameField = "projectName";
    public const string ProjectNameOriginalField = "projectNameOriginal";
    public const string ContractTypePlatformFieldName = "contractType";
    public const string ItemPlatformFieldName = "item";
    public const string CollectionFilterId = "collectionId";

    public abstract string Item { get; set; }

    public abstract string DocumentId { get; set; }

    public abstract string CollectionName { get; set; }

    public abstract string CollectionId { get; set; }

    public abstract long? PreviousDocumentVersion { get; set; }

    public abstract long CurrentDocumentVersion { get; set; }

    public abstract int IndexedTimeStamp { get; set; }

    public abstract string Routing { get; set; }

    public abstract string ParentDocumentId { get; set; }

    public abstract DocumentContractType ContractType { get; }

    public abstract string GetFieldNameForStoredField(string storedField);

    public abstract string GetStoredFieldForFieldName(string field);

    public abstract string GetStoredFieldValue(string field, string fieldValue);

    public abstract string GetSearchFieldForType(string type);

    public abstract int GetSize();

    public static string GetBranchNameOriginalFieldName(DocumentContractType documentContractType)
    {
      switch (documentContractType)
      {
        case DocumentContractType.DedupeFileContractV3:
        case DocumentContractType.DedupeFileContractV4:
        case DocumentContractType.DedupeFileContractV5:
          return "paths.branchNameOriginal";
        case DocumentContractType.WikiContract:
          return FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "branchName", (object) "raw"));
        default:
          return "branchNameOriginal";
      }
    }

    public static string GetProjectNameOriginalFieldName(DocumentContractType documentContractType)
    {
      if (documentContractType != DocumentContractType.WikiContract)
        return "projectNameOriginal";
      return FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "projectName", (object) "raw"));
    }

    protected AbstractSearchDocumentContract()
    {
    }

    protected AbstractSearchDocumentContract(ISearchQueryClient elasticClient) => this.SearchQueryClient = elasticClient;

    public abstract EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request);

    public abstract ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request);

    public abstract EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest);

    internal ISearchQueryClient SearchQueryClient { get; }

    public static AbstractSearchDocumentContract CreateContract(DocumentContractType contractType)
    {
      switch (contractType)
      {
        case DocumentContractType.ProjectContract:
          return (AbstractSearchDocumentContract) new ProjectContract();
        case DocumentContractType.WorkItemContract:
          return (AbstractSearchDocumentContract) new WorkItemContract();
        case DocumentContractType.RepositoryContract:
          return (AbstractSearchDocumentContract) new RepositoryContract();
        case DocumentContractType.SourceNoDedupeFileContractV3:
          return (AbstractSearchDocumentContract) new SourceNoDedupeFileContractV3();
        case DocumentContractType.DedupeFileContractV3:
          return (AbstractSearchDocumentContract) new DedupeFileContractV3();
        case DocumentContractType.WikiContract:
          return (AbstractSearchDocumentContract) new WikiContract();
        case DocumentContractType.PackageVersionContract:
          return (AbstractSearchDocumentContract) new PackageVersionContract();
        case DocumentContractType.SourceNoDedupeFileContractV4:
          return (AbstractSearchDocumentContract) new SourceNoDedupeFileContractV4();
        case DocumentContractType.BoardContract:
          return (AbstractSearchDocumentContract) new BoardVersionContract();
        case DocumentContractType.DedupeFileContractV4:
          return (AbstractSearchDocumentContract) new DedupeFileContractV4();
        case DocumentContractType.SettingContract:
          return (AbstractSearchDocumentContract) new SettingContract();
        case DocumentContractType.SourceNoDedupeFileContractV5:
          return (AbstractSearchDocumentContract) new SourceNoDedupeFileContractV5();
        case DocumentContractType.DedupeFileContractV5:
          return (AbstractSearchDocumentContract) new DedupeFileContractV5();
        default:
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported contract type '{0}'", (object) contractType)));
      }
    }

    public abstract bool IsSourceEnabled(IVssRequestContext requestContext);

    public static class CommonStoredFields
    {
      public const string OrganizationName = "organizationName";
      public const string AccountName = "accountName";
      public const string CollectionName = "collectionName";
      public const string CollectionId = "collectionId";
      public const string ProjectName = "projectName";
      public const string ProjectId = "projectId";
    }
  }
}
