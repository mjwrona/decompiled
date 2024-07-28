// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board.BoardVersionContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Nest;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board
{
  public class BoardVersionContract : AbstractSearchDocumentContract
  {
    private int m_size;

    public override string DocumentId { get; set; }

    [Keyword(Name = "item")]
    public override string Item { get; set; }

    [Keyword(Ignore = true)]
    public override string Routing { get; set; }

    [Nest.Text(Name = "collectionName")]
    public override string CollectionName { get; set; }

    [Keyword(Name = "collectionId")]
    public override string CollectionId { get; set; }

    [Keyword(Name = "teamId")]
    public string TeamId { get; set; }

    [Nest.Text(Name = "teamName")]
    public string TeamName { get; set; }

    [Keyword(Name = "projectId")]
    public string ProjectId { get; set; }

    [Nest.Text(Name = "projectName")]
    public string ProjectName { get; set; }

    [Nest.Text(Name = "description")]
    public string Description { get; set; }

    [Keyword(Name = "indexedTimeStamp")]
    public override int IndexedTimeStamp { get; set; }

    [Keyword(Name = "boardType")]
    public string BoardType { get; set; }

    [Keyword(Ignore = true)]
    public override string ParentDocumentId
    {
      get => (string) null;
      set => throw new NotImplementedException();
    }

    [Keyword(Ignore = true)]
    public override long? PreviousDocumentVersion
    {
      get => new long?();
      set => throw new NotImplementedException();
    }

    [Keyword(Ignore = true)]
    public override long CurrentDocumentVersion { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.BoardContract;

    public void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      object data,
      ParsedData parsedData)
    {
      this.m_size = parsedData.Content.Length;
      WebApiTeam teamData = JsonConvert.DeserializeObject<WebApiTeam>(Encoding.UTF8.GetString(parsedData.Content));
      BoardMetadata boardMetadata = data as BoardMetadata;
      this.PopulateFileContractDetails(requestContext.GetCollectionID().ToString(), this.GetCollectionName(requestContext), teamData, boardMetadata);
    }

    private void PopulateFileContractDetails(
      string collectionId,
      string collectionName,
      WebApiTeam teamData,
      BoardMetadata boardMetadata)
    {
      this.CollectionId = collectionId;
      this.CollectionName = collectionName;
      this.Routing = this.CollectionId;
      this.TeamId = teamData.Id.ToString();
      this.TeamName = teamData.Name;
      this.ProjectId = teamData.ProjectId.ToString();
      this.ProjectName = teamData.ProjectName;
      this.Description = teamData.Description;
      this.BoardType = boardMetadata.BoardType.ToString();
    }

    private string GetCollectionName(IVssRequestContext requestContext) => requestContext.GetService<ICollectionRedirectionService>().GetCollectionName(requestContext)?.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public override string GetFieldNameForStoredField(string storedField) => storedField;

    public override string GetStoredFieldForFieldName(string field) => field;

    public override string GetStoredFieldValue(string field, string fieldValue) => fieldValue;

    public override string GetSearchFieldForType(string type) => type;

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

    public override int GetSize() => this.m_size;

    public static class PlatformFieldNames
    {
      public const string DocumentIdField = "documentId";
      public const string DescriptionField = "description";
      public const string Description = "description";
      public const string TeamId = "teamId";
      public const string TeamName = "teamName";
      public const string ProjectId = "projectId";
      public const string ProjectName = "projectName";
      public const string BoardType = "boardType";
    }

    public static class ServiceFieldNames
    {
      public const string Description = "description";
      public const string TeamId = "teamId";
      public const string TeamName = "teamName";
      public const string ProjectId = "projectId";
      public const string ProjectName = "projectName";
      public const string BoardType = "boardType";
    }
  }
}
