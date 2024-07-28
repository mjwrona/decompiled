// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchRequestPagingToken
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  [JsonObject(MemberSerialization.OptIn)]
  public sealed class SearchRequestPagingToken : AbstractSearchRequest
  {
    private string ddsPagingToken;
    private SearchPagingTokenType pagingTokenType;

    [JsonProperty("ddsPagingToken")]
    public string DdsPagingToken
    {
      get => this.ddsPagingToken;
      private set => this.ddsPagingToken = value;
    }

    [JsonProperty("pagingTokenType")]
    public SearchPagingTokenType PagingTokenType
    {
      get => this.pagingTokenType;
      internal set => this.pagingTokenType = value;
    }

    public SearchRequestPagingToken()
    {
      this.pagingTokenType = SearchPagingTokenType.None;
      this.ddsPagingToken = string.Empty;
    }

    public SearchRequestPagingToken(
      IdentityTypeEnum identityType,
      OperationScopeEnum scope,
      Dictionary<string, object> options,
      string query,
      HashSet<string> properties,
      HashSet<string> filterByAncestorEntityIds,
      HashSet<string> filterByEntityIds,
      SearchPagingTokenType pagingTokenType,
      string ddsPagingToken = null)
      : base(identityType, scope, options, query, properties, filterByAncestorEntityIds, filterByEntityIds)
    {
      this.pagingTokenType = pagingTokenType;
      this.ddsPagingToken = string.IsNullOrEmpty(ddsPagingToken) ? string.Empty : ddsPagingToken;
    }

    internal void Validate(IVssRequestContext requestContext, SearchRequest request)
    {
      this.Validate(requestContext);
      if (this.Query != request.Query)
        throw new IdentityPickerInvalidPagingTokenException("The search request and its paging token have different Query values.");
      if (this.IdentityType != request.IdentityType)
        throw new IdentityPickerInvalidPagingTokenException("The search request and its paging token have different IdentityTypeEnum.");
      if (this.OperationScope != request.OperationScope)
        throw new IdentityPickerInvalidPagingTokenException("The search request and its paging token have different OperationScope.");
    }
  }
}
