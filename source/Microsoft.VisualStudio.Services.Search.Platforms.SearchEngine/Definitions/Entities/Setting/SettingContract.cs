// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting.SettingContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting
{
  public class SettingContract : AbstractSearchDocumentContract
  {
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

    [Nest.Text(Ignore = true)]
    public override string CollectionName { get; set; }

    [Keyword(Ignore = true)]
    public override string CollectionId { get; set; }

    [Date(Name = "indexedTimeStamp")]
    public override int IndexedTimeStamp { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.SettingContract;

    [Nest.Text(Name = "title")]
    public string Title { get; set; }

    [Nest.Text(Name = "description")]
    public string Description { get; set; }

    [Keyword(Name = "routeId")]
    public string RouteId { get; set; }

    [Keyword(Name = "routeParameterMapping")]
    public string RouteParameterMapping { get; set; }

    [Keyword(Name = "icon")]
    public string Icon { get; set; }

    [Keyword(Name = "scope")]
    public string Scope { get; set; }

    [Nest.Text(Name = "tags")]
    public IEnumerable<string> Tags { get; set; }

    public override string GetFieldNameForStoredField(string storedField) => storedField;

    public override string GetStoredFieldForFieldName(string field) => field;

    public override string GetStoredFieldValue(string field, string fieldValue) => fieldValue;

    public override string GetSearchFieldForType(string type) => type;

    public override int GetSize() => 0;

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

    public void PopulateFileContractDetails(ParsedData parsedData)
    {
      Microsoft.VisualStudio.Services.Search.Common.Setting setting = JsonConvert.DeserializeObject<Microsoft.VisualStudio.Services.Search.Common.Setting>(Encoding.UTF8.GetString(parsedData.Content));
      this.Title = setting.Title;
      this.Description = setting.Description;
      this.RouteId = setting.RouteId;
      this.RouteParameterMapping = setting.RouteParameterMapping;
      this.Tags = setting.Tags;
      this.Icon = setting.Icon;
      this.Scope = setting.Scope;
      this.DocumentId = Microsoft.VisualStudio.Services.Search.Common.Setting.GetSettingId(setting.Title, setting.Scope);
    }
  }
}
