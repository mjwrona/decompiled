// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class WikiContract : AbstractSearchDocumentContract
  {
    private int m_size;
    [StaticSafe]
    private static readonly Dictionary<string, string> s_storedFieldToFieldMapping = new Dictionary<string, string>()
    {
      {
        "projectName.search",
        "projectName"
      },
      {
        "projectName.searchwithcamelcasedelimiter",
        "projectName"
      },
      {
        "collectionName.search",
        "collectionName"
      },
      {
        "collectionName.searchwithcamelcasedelimiter",
        "collectionName"
      },
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "fileNames", (object) "lower")),
        "fileNames"
      }
    };
    public const string ProjectNameSearchField = "projectName.search";
    public const string CollectionNameSearchField = "collectionName.search";
    public const string ProjectNameCamelCaseSearchField = "projectName.searchwithcamelcasedelimiter";
    public const string CollectionNameCamelCaseSearchField = "collectionName.searchwithcamelcasedelimiter";
    public const string FileNameLowerCaseSearchField = "fileNames.lower";
    protected const string branchRefPrefix = "refs/heads/";
    public const string DocumentIdField = "documentId";
    public const string ProjectVisibilityField = "projectVisibility";
    public const string ProjectIdField = "projectId";
    public const string RepoNameField = "repoName";
    public const string RepositoryIdField = "repositoryId";
    public const string IsDefaultBranchField = "isDefaultBranch";
    public const string WikiNameField = "wikiName";
    public const string WikiIdField = "wikiId";
    public const string MappedPathField = "mappedPath";
    public const string RawFieldNameSuffix = "raw";
    public const string LowerFieldNameSuffix = "lower";
    public const string PatternFieldNameSuffix = "pattern";
    public const string SearchFieldNameSuffix = "search";
    public const string UnstemmedFieldNameSuffix = "unstemmed";
    public const string SearchFieldWithCamelCaseNameSuffix = "searchwithcamelcasedelimiter";

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

    [Text(Name = "collectionName")]
    public override string CollectionName { get; set; }

    [Keyword(Name = "collectionId")]
    public override string CollectionId { get; set; }

    [Keyword(Name = "collectionUrl")]
    public string CollectionUrl { get; set; }

    [Keyword(Name = "organizationId")]
    public string OrganizationId { get; set; }

    [Text(Name = "projectName")]
    public string ProjectName { get; set; }

    [Keyword(Name = "projectVisibility")]
    public string ProjectVisibility { get; set; }

    [Keyword(Name = "projectId")]
    public string ProjectId { get; set; }

    [Text(Name = "repoName")]
    public string RepoName { get; set; }

    [Keyword(Name = "repositoryId")]
    public string RepositoryId { get; set; }

    [Text(Name = "wikiName")]
    public string WikiName { get; set; }

    [Keyword(Name = "wikiId")]
    public string WikiId { get; set; }

    [Text(Name = "branchName")]
    public string BranchName { get; set; }

    [Boolean(Name = "isDefaultBranch")]
    public bool IsDefaultBranch { get; set; }

    [Date(Name = "indexedTimeStamp")]
    public override int IndexedTimeStamp { get; set; }

    [Text(Name = "filePath")]
    public string FilePath { get; set; }

    [Keyword(Name = "mappedPath")]
    public string MappedPath { get; set; }

    [Text(Name = "fileNames")]
    public IEnumerable<string> FileNames { get; set; }

    [Keyword(Name = "fileExtension")]
    public string FileExtension { get; set; }

    [Number(Name = "fileExtensionId")]
    public float? FileExtensionId { get; set; }

    [Text(Name = "content")]
    public string Content { get; set; }

    [Keyword(Name = "contentMetadata")]
    public string FileContentMetadata { get; set; }

    [Text(Name = "contentLinks")]
    public string[] ContentLinks { get; set; }

    [Keyword(Name = "contentId")]
    public string ContentId { get; set; }

    [Text(Name = "tags")]
    public string[] Tags { get; set; }

    [Date(Name = "lastUpdated")]
    public DateTime LastUpdatedTime { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.WikiContract;

    public void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      object data,
      IMetaDataStore metaDataStore,
      ParsedData parsedData)
    {
      WikiMetaDataStoreItem metaDataStoreItem = data as WikiMetaDataStoreItem;
      WikiDocument wikiDocument = new WikiDocument()
      {
        ContentId = metaDataStoreItem.ContentId.HexHash,
        FilePath = metaDataStoreItem.Path,
        FileName = metaDataStoreItem.FileName,
        FileExtension = metaDataStoreItem.Extension,
        WikiId = metaDataStoreItem.WikiId,
        WikiName = metaDataStoreItem.WikiName,
        MappedPath = metaDataStoreItem.MappedPath
      };
      try
      {
        if (parsedData.Content != null)
        {
          this.m_size = parsedData.Content.Length;
          MarkDownParsedContent downParsedContent = parsedData.Content.DeserializeToObject<MarkDownParsedContent>();
          wikiDocument.Content = downParsedContent.Content;
          wikiDocument.ContentLinks = downParsedContent.Links.ToArray();
        }
      }
      catch
      {
        TextEncoding textEncoding = new TextEncoding();
        wikiDocument.Content = textEncoding.GetString(parsedData.Content);
      }
      wikiDocument.ContentMetadata = string.IsNullOrWhiteSpace(wikiDocument.Content) ? "Empty" : "NonEmpty";
      string branchName = "";
      if (metaDataStoreItem.BranchesInfo != null && metaDataStoreItem.BranchesInfo.Count == 1)
        branchName = CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", metaDataStoreItem.BranchesInfo[0].BranchName);
      wikiDocument.LastUpdated = Convert.ToDateTime(metaDataStore["LatestChangeUtcTimeKey"], (IFormatProvider) CultureInfo.InvariantCulture);
      this.PopulateFileContractDetails(metaDataStore, wikiDocument, branchName, this.GetCollectionUrl(requestContext));
    }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "This is a string")]
    public virtual void PopulateFileContractDetails(
      IMetaDataStore metaDataStore,
      WikiDocument wikiDocument,
      string branchName,
      string collectionUrl)
    {
      this.CollectionName = metaDataStore["CollectionName"];
      this.CollectionId = metaDataStore["CollectionId"].ToLowerInvariant();
      this.OrganizationId = metaDataStore["OrganizationId"].ToLowerInvariant();
      this.ProjectName = metaDataStore["ProjectName"];
      this.ProjectId = metaDataStore["ProjectId"].ToLowerInvariant();
      this.ProjectVisibility = metaDataStore["ProjectVisibility"];
      this.RepoName = metaDataStore["RepoName"];
      this.RepositoryId = metaDataStore["RepoId"].ToLowerInvariant();
      this.LastUpdatedTime = wikiDocument.LastUpdated;
      this.Content = wikiDocument.Content;
      this.FileContentMetadata = wikiDocument.ContentMetadata;
      this.ContentLinks = wikiDocument.ContentLinks;
      string contentId = wikiDocument.ContentId;
      this.ContentId = contentId != null ? contentId.NormalizeString() : (string) null;
      string fileExtension = wikiDocument.FileExtension;
      this.FileExtension = fileExtension != null ? fileExtension.NormalizeFileExtension() : (string) null;
      this.FileExtensionId = new float?((float) RelevanceUtility.GetFileExtensionId(this.FileExtension));
      this.FilePath = wikiDocument.FilePath;
      this.FileNames = (IEnumerable<string>) new List<string>()
      {
        this.GetDecodedWikiFileName(wikiDocument.FileName, wikiDocument.FilePath, wikiDocument.MappedPath)
      };
      this.Tags = wikiDocument.TagsField;
      this.BranchName = branchName;
      this.WikiId = wikiDocument.WikiId;
      this.WikiName = wikiDocument.WikiName;
      this.MappedPath = wikiDocument.MappedPath;
      this.IsDefaultBranch = true;
      string str;
      if (string.IsNullOrEmpty(branchName))
        str = this.WikiId + "@" + this.FilePath;
      else
        str = this.WikiId + "@" + branchName + "@" + this.FilePath;
      this.DocumentId = str;
      this.Item = this.FilePath;
      this.CollectionUrl = collectionUrl;
    }

    private string GetDecodedWikiFileName(
      string fileName,
      string encodedFilePath,
      string mappedPath)
    {
      try
      {
        return WikiFilePathHelper.GetFileNameWithoutExtension(WikiFilePathHelper.GetPageReadablePath(encodedFilePath, mappedPath));
      }
      catch (Exception ex)
      {
        return fileName;
      }
    }

    private string GetCollectionUrl(IVssRequestContext requestContext) => requestContext.GetService<ICollectionRedirectionService>().GetCollectionUrl(requestContext)?.ToString();

    public override string GetFieldNameForStoredField(string storedField)
    {
      string str;
      return WikiContract.s_storedFieldToFieldMapping.TryGetValue(storedField, out str) ? str : storedField;
    }

    public override string GetStoredFieldForFieldName(string field) => field;

    public override string GetStoredFieldValue(string field, string fieldValue) => fieldValue;

    public override string GetSearchFieldForType(string type) => type;

    public override int GetSize() => this.m_size;

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

    public override bool IsSourceEnabled(IVssRequestContext requestContext) => true;

    [Keyword(Ignore = true)]
    public override string ParentDocumentId
    {
      get => (string) null;
      set => throw new NotImplementedException();
    }

    public static class ServiceFieldNames
    {
      public const string LastUpdated = "lastUpdated";
      public const string FileNamesField = "fileNames";
      public const string FilePathField = "filePath";
      public const string FileExtensionField = "fileExtension";
      public const string FileExtensionIdField = "fileExtensionId";
      public const string ContentField = "content";
      public const string ContentMetadataField = "contentMetadata";
      public const string ContentLinksField = "contentLinks";
      public const string ContentIdField = "contentId";
      public const string TagsField = "tags";
      public const string CollectionUrl = "collectionUrl";
      public const string CollectionNameField = "collectionName";
    }

    public static class PlatformFieldNames
    {
      public const string LastUpdated = "lastUpdated";
      public const string FileNamesField = "fileNames";
      public const string FilePathField = "filePath";
      public const string FileExtensionField = "fileExtension";
      public const string FileExtensionIdField = "fileExtensionId";
      public const string ContentField = "content";
      public const string ContentMetadataField = "contentMetadata";
      public const string ContentLinksField = "contentLinks";
      public const string ContentIdField = "contentId";
      public const string TagsField = "tags";
      public const string CollectionUrl = "collectionUrl";
      public const string CollectionNameField = "collectionName";
    }
  }
}
