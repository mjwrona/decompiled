// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ProjectRepoContractBase
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Nest;
using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public abstract class ProjectRepoContractBase : AbstractSearchDocumentContract
  {
    public const string JoinFieldName = "joinField";

    [Keyword(Name = "documentId")]
    public override string DocumentId { get; set; }

    [Keyword(Name = "item")]
    public override string Item { get; set; }

    [Keyword(Ignore = true)]
    public override string Routing { get; set; }

    [Keyword(Ignore = true)]
    public override long? PreviousDocumentVersion
    {
      get => new long?();
      set => throw new NotImplementedException();
    }

    [Keyword(Ignore = true)]
    public override long CurrentDocumentVersion { get; set; }

    [Date(Name = "indexedTimeStamp")]
    public override int IndexedTimeStamp { get; set; }

    [Keyword(Name = "organizationId")]
    public string OrganizationId { get; set; }

    [Keyword(Name = "organizationName")]
    public string OrganizationName { get; set; }

    [Keyword(Name = "organizationNameOriginal")]
    public string OrganizationNameOriginal { get; set; }

    [Keyword(Name = "collectionId")]
    public override string CollectionId { get; set; }

    [Keyword(Name = "collectionName")]
    public override string CollectionName { get; set; }

    [Keyword(Name = "collectionNameOriginal")]
    public string CollectionNameOriginal { get; set; }

    [Text(Name = "collectionNameAnalyzed")]
    public string CollectionNameAnalysed { get; set; }

    [Text(Name = "name")]
    public string Name { get; set; }

    [Keyword(Name = "visibility")]
    public string Visibility { get; set; }

    [Date(Name = "lastUpdated")]
    public DateTime? LastUpdatedDate { get; set; }

    [Keyword(Name = "parentDocumentId")]
    public override string ParentDocumentId { get; set; }

    [Object(Name = "joinField")]
    public JoinField JoinField { get; set; }

    public virtual void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      object data,
      IMetaDataStore metaDataStore,
      ParsedData parsedData,
      ProvisionerSettings settings,
      byte[] originalContent = null)
    {
      throw new NotImplementedException("No Implementation found for PopulateFileContractDetails");
    }

    public override string GetFieldNameForStoredField(string storedField) => throw new NotImplementedException("No Implementation found for GetFieldNameForStoredField");

    public override string GetStoredFieldForFieldName(string field) => throw new NotImplementedException("No Implementation found for GetStoredFieldForFieldName");

    public override string GetStoredFieldValue(string field, string fieldValue) => throw new NotImplementedException("No Implementation found for GetStoredFieldValue");

    public override string GetSearchFieldForType(string type) => throw new NotImplementedException("No Implementation found for GetSearchFieldForType");

    public override int GetSize() => throw new NotImplementedException();

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      throw new NotImplementedException();
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      throw new NotImplementedException();
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      throw new NotImplementedException();
    }

    internal virtual void SetMetadataProperties(IMetaDataStore metaDataStore)
    {
      this.CollectionName = metaDataStore["NormalizedCollectionName"];
      this.CollectionNameOriginal = metaDataStore["CollectionName"];
      this.CollectionNameAnalysed = metaDataStore["CollectionName"];
      this.CollectionId = metaDataStore["CollectionId"].ToLowerInvariant();
      this.OrganizationName = metaDataStore["NormalizedOrganizationName"];
      this.OrganizationNameOriginal = metaDataStore["OrganizationName"];
      this.OrganizationId = metaDataStore["OrganizationId"].ToLowerInvariant();
    }

    public static class PlatformFieldNames
    {
      public const string DocumentIdField = "documentId";
      public const string NameField = "name";
      public const string NameRawField = "name.raw";
      public const string NameCamelCaseSearchField = "name.casechangeanalyzed";
      public const string LastUpdatedField = "lastUpdated";
      public const string ActivityStatsField = "activityStats";
      public const string ActivityCount1dayField = "activityCount1day";
      public const string ActivityCount7daysField = "activityCount7day";
      public const string ActivityCount30daysField = "activityCount30days";
      public const string TrendFactor1DayField = "TrendFactor1Day";
      public const string TrendFactor7DaysField = "TrendFactor7Days";
      public const string TrendFactor30DaysField = "TrendFactor30Days";
      public const string DescriptionField = "description";
      public const string DescriptionMetadataField = "descriptionMetadata";
      public const string LikesCountField = "likesCount";
      public const string CollectionNameAnalysedField = "collectionNameAnalyzed";
      public const string CollectionNameCamelCaseAnalysedField = "collectionNameAnalyzed.casechangeanalyzed";
      public const string TagsField = "tags";
      public const string TagsRawField = "tags.raw";
      public const string TagsFilterField = "tags.lower";
      public const string TagsCamelCaseSearchField = "tags.casechangeanalyzed";
      public const string VisibilityField = "visibility";
      public const string ReadmeField = "readme";
      public const string ReadmeMetadataField = "readmeMetadata";
      public const string ReadmeLinksField = "readmeLinks";
      public const string ForksField = "forks";
      public const string LanguagesField = "languages";
      public const string LanguagesRawField = "languages.raw";
      public const string LanguagesSearchField = "languages.analysed";
      public const string DownloadsField = "downloads";
      public const string ParentDocumentIdField = "parentDocumentId";
      public const string ParentProjectNameField = "projectName";
      public const string ParentProjectNameCamelCaseSearchField = "projectName.casechangeanalyzed";
      public const string VersionControlField = "versionControl";
      public const string ActivityDate = "activityDate";
      public const string ActivityValue = "activityValue";
      public const string Url = "url";
      public const string ImageUrl = "imageUrl";
      public const string ReadmeFilePathField = "readmeFilePath";
    }

    public static class StoredFields
    {
      public const string Name = "name";
      public const string DocumentId = "documentId";
      public const string VersionControl = "versionControl";
      public const string LastUpdatedDate = "lastUpdated";
      public const string LikesCount = "likesCount";
      public const string RepoForks = "forks";
      public const string Tags = "tags";
      public const string Description = "description";
      public const string Visibililty = "visibility";
      public const string Languages = "languages";
      public const string Downloads = "downloads";
      public const string ParentProjectNameField = "projectName";
      public const string ActivityCount1day = "activityCount1day";
      public const string ActivityCount7days = "activityCount7day";
      public const string ActivityCount30days = "activityCount30days";
      public const string ActivityStats = "activityStats";
      public const string Readme = "readme";
      public const string ActivityDate = "activityStats.activityDate";
      public const string ActivityValue = "activityStats.activityValue";
      public const string Url = "url";
      public const string ImageUrl = "imageUrl";
      public const string ReadmeFilePathField = "readmeFilePath";
    }
  }
}
