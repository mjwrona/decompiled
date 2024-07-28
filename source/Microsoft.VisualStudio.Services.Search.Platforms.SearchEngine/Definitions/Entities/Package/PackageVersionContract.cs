// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package.PackageVersionContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package
{
  public class PackageVersionContract : AbstractSearchDocumentContract
  {
    private int m_size;
    private SortableVersionBuilder m_sortableVersionBuilder = new SortableVersionBuilder(4);

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

    [Keyword(Name = "collectionName")]
    public override string CollectionName { get; set; }

    [Keyword(Name = "collectionNameOriginal")]
    public string CollectionNameOriginal { get; set; }

    [Keyword(Name = "collectionId")]
    public override string CollectionId { get; set; }

    [Keyword(Name = "collectionUrl")]
    public string CollectionUrl { get; set; }

    [Keyword(Name = "organizationId")]
    public string OrganizationId { get; set; }

    [Keyword(Name = "feedName")]
    public string FeedName { get; set; }

    [Keyword(Name = "feedId")]
    public string FeedId { get; set; }

    [Keyword(Name = "name")]
    public string PackageName { get; set; }

    [Keyword(Name = "normalizedName")]
    public string NormalizedName { get; set; }

    [Nest.Text(Name = "protocol")]
    public string Protocol { get; set; }

    [Nest.Text(Name = "author")]
    public string Author { get; set; }

    [Keyword(Name = "packageId")]
    public string PackageId { get; set; }

    [Keyword(Name = "versionId")]
    public string VersionId { get; set; }

    [Nest.Text(Name = "version")]
    public string Version { get; set; }

    [Date(Name = "publishedTime")]
    public DateTime PublishedTime { get; set; }

    [Nest.Text(Name = "normalizedVersion")]
    public string NormalizedVersion { get; set; }

    [Keyword(Name = "sortableVersion")]
    public string SortableVersion { get; set; }

    [Nest.Text(Name = "description")]
    public string Description { get; set; }

    [Keyword(Name = "indexedTimeStamp")]
    public override int IndexedTimeStamp { get; set; }

    [Keyword(Ignore = true)]
    public override string ParentDocumentId
    {
      get => (string) null;
      set => throw new NotImplementedException();
    }

    [Object(Name = "views")]
    public IList<PackageViewInfo> Views { get; set; }

    [Keyword(Name = "tags")]
    public IEnumerable<string> Tags { get; set; }

    [Boolean(Name = "isListed")]
    public bool? IsListed { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.PackageVersionContract;

    public void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      object data,
      ParsedData parsedData)
    {
      this.m_size = parsedData.Content.Length;
      PackageChange packageChange1 = JsonConvert.DeserializeObject<PackageChange>(Encoding.UTF8.GetString(parsedData.Content));
      PackageVersionChangeEntityMetadata changeEntityMetadata = data as PackageVersionChangeEntityMetadata;
      Guid guid = requestContext.GetOrganizationID();
      string organizationId = guid.ToString();
      guid = requestContext.GetCollectionID();
      string collectionId = guid.ToString();
      string collectionName = this.GetCollectionName(requestContext);
      string collectionUrl = this.GetCollectionUrl(requestContext);
      PackageChange packageChange2 = packageChange1;
      PackageVersionChangeEntityMetadata packageVersionChangeEntityMetadata = changeEntityMetadata;
      this.PopulateFileContractDetails(organizationId, collectionId, collectionName, collectionUrl, packageChange2, packageVersionChangeEntityMetadata);
    }

    private void PopulateFileContractDetails(
      string organizationId,
      string collectionId,
      string collectionName,
      string collectionUrl,
      PackageChange packageChange,
      PackageVersionChangeEntityMetadata packageVersionChangeEntityMetadata)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Package package = packageChange.Package;
      PackageVersion packageVersion = packageChange.PackageVersionChange.PackageVersion;
      this.OrganizationId = organizationId;
      this.CollectionId = collectionId;
      this.CollectionName = collectionName;
      this.CollectionNameOriginal = collectionName;
      this.CollectionUrl = collectionUrl;
      this.Routing = this.OrganizationId;
      this.Author = packageVersion.Author;
      this.Description = packageVersion.Description;
      this.NormalizedName = package.NormalizedName;
      this.PackageName = package.Name;
      this.PackageId = package.Id.ToString();
      this.Version = packageVersion.Version;
      this.NormalizedVersion = packageVersion.NormalizedVersion;
      this.VersionId = packageVersion.Id.ToString();
      this.Protocol = package.ProtocolType;
      this.PublishedTime = !packageVersion.PublishDate.HasValue ? DateTime.MinValue : packageVersion.PublishDate.Value;
      this.IndexedTimeStamp = (int) DateTime.Now.Ticks;
      Guid guid = packageVersionChangeEntityMetadata.FeedId;
      this.FeedId = guid.ToString();
      this.FeedName = packageVersionChangeEntityMetadata.FeedName;
      this.DocumentId = FormattableString.Invariant(FormattableStringFactory.Create("{0}@{1}@{2}", (object) this.NormalizedName, (object) this.PackageId, (object) this.VersionId));
      this.Tags = packageVersion.Tags;
      this.IsListed = new bool?(packageVersion.IsListed);
      this.Views = (IList<PackageViewInfo>) new List<PackageViewInfo>();
      if (packageVersion.Views != null)
      {
        foreach (FeedView view in packageVersion.Views)
        {
          PackageViewInfo packageViewInfo1 = new PackageViewInfo();
          PackageViewInfo packageViewInfo2 = packageViewInfo1;
          guid = view.Id;
          string str = guid.ToString();
          packageViewInfo2.ViewId = str;
          packageViewInfo1.ViewName = view.Name.ToLowerInvariant();
          packageViewInfo1.ViewNameOriginal = view.Name;
          packageViewInfo1.ViewUrl = view.Url;
          this.Views.Add(packageViewInfo1);
        }
      }
      try
      {
        this.SortableVersion = this.m_sortableVersionBuilder.GetSortableVersion(packageVersion.NormalizedVersion);
      }
      catch (InvalidVersionException ex)
      {
        this.SortableVersion = packageVersion.NormalizedVersion;
      }
    }

    internal void PopulateFileContractDetails(
      IMetaDataStore metaDataStore,
      PackageChange packageChange,
      PackageVersionChangeEntityMetadata packageVersionChangeEntityMetadata)
    {
      this.PopulateFileContractDetails(metaDataStore["OrganizationId"], metaDataStore["CollectionId"], metaDataStore["CollectionName"], metaDataStore["collectionUrl"], packageChange, packageVersionChangeEntityMetadata);
    }

    private string GetCollectionUrl(IVssRequestContext requestContext) => requestContext.GetService<ICollectionRedirectionService>().GetCollectionUrl(requestContext)?.ToString();

    private string GetCollectionName(IVssRequestContext requestContext) => requestContext.GetService<ICollectionRedirectionService>().GetCollectionName(requestContext)?.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public override string GetFieldNameForStoredField(string storedField) => storedField;

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

    public static class PlatformFieldNames
    {
      public const string DocumentIdField = "documentId";
      public const string NameField = "name";
      public const string NormalizedNameField = "normalizedName";
      public const string DescriptionField = "description";
      public const string CollectionUrlField = "collectionUrl";
      public const string ProtocolField = "protocol";
      public const string ProtocolRawField = "protocol.raw";
      public const string ProtocolFilterField = "protocol.lower";
      public const string AuthorField = "author";
      public const string VersionField = "version";
      public const string NormalizedVersionField = "normalizedVersion";
      public const string SortableVersionField = "sortableVersion";
      public const string PublishDateField = "publishDate";
      public const string FeedIdField = "feedId";
      public const string FeedNameField = "feedName";
      public const string FeedNameRawField = "feedName.raw";
      public const string FeedNameFilterField = "feedName.lower";
      public const string VersionIdField = "versionId";
      public const string PackageIdField = "packageId";
      public const string ViewsField = "views";
      public const string ViewNameField = "viewName";
      public const string ViewNameOriginalField = "viewNameOriginal";
      public const string ViewIdField = "viewId";
      public const string ViewUrlField = "viewUrl";
      public const string TagsField = "tags";
      public const string IsListedField = "isListed";
      public const string PackageName = "name";
      public const string PackageNameCaseChangeAnalyzed = "name.casechangeanalyzed";
      public const string PackageId = "packageId";
      public const string NormalizedName = "normalizedName";
      public const string Protocol = "protocol";
      public const string Author = "author";
      public const string Version = "version";
      public const string NormalizedVersion = "normalizedVersion";
      public const string SortableVersion = "sortableVersion";
      public const string Description = "description";
      public const string PublishedTime = "publishedTime";
      public const string FeedName = "feedName";
      public const string FeedId = "feedId";
      public const string VersionId = "versionId";
      public const string CollectionUrl = "collectionUrl";
    }

    public static class AggsNames
    {
      public const string Package_Count_Aggs = "Package_Count_Aggs";
      public const string Filtered_Results_Aggs = "Filtered_Results_Aggs";
      public const string Package_Aggs = "Package_Aggs";
      public const string Feed_Aggs = "Feed_Aggs";
      public const string Top_Versions_Aggs = "Top_Versions_Aggs";
      public const string Max_Score_Agg = "max_score_agg";
    }

    public static class StoredNames
    {
      public const string ViewNameFilterField = "views.viewName";
      public const string ViewNameAggregationField = "views.viewNameOriginal";
      public const string ViewIdFilterField = "views.viewId";
    }

    public static class ServiceFieldNames
    {
      public const string PackageName = "name";
      public const string PackageId = "packageId";
      public const string NormalizedName = "normalizedName";
      public const string Protocol = "protocol";
      public const string Author = "author";
      public const string Version = "version";
      public const string NormalizedVersion = "normalizedVersion";
      public const string SortableVersion = "sortableVersion";
      public const string Description = "description";
      public const string PublishedTime = "publishedTime";
      public const string FeedName = "feedName";
      public const string FeedId = "feedId";
      public const string VersionId = "versionId";
      public const string CollectionUrl = "collectionUrl";
    }
  }
}
