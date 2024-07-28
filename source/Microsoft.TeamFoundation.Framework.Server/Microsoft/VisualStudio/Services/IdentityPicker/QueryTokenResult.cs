// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.QueryTokenResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [JsonObject(MemberSerialization.OptIn)]
  public sealed class QueryTokenResult : ISecuredObject
  {
    [JsonProperty("queryToken")]
    public string QueryToken { get; set; }

    [JsonProperty("identities")]
    public IList<Identity> Identities { get; set; }

    [JsonProperty("pagingToken")]
    public string PagingToken => !this.OptionalProperties.ContainsKey(QueryTokenResultProperties.PagingToken) || string.IsNullOrEmpty((string) this.OptionalProperties[QueryTokenResultProperties.PagingToken]) ? string.Empty : (string) this.OptionalProperties[QueryTokenResultProperties.PagingToken];

    public IDictionary<string, object> OptionalProperties { get; set; }

    private QueryTokenResult()
    {
    }

    public QueryTokenResult(QueryTokenResult queryTokenResult)
    {
      this.QueryToken = queryTokenResult.QueryToken;
      this.Identities = queryTokenResult.Identities;
      this.OptionalProperties = queryTokenResult.OptionalProperties;
    }

    public QueryTokenResult(
      string query,
      IList<Identity> identities,
      IDictionary<string, object> optionalProperties = null)
    {
      this.QueryToken = query;
      this.Identities = identities ?? (IList<Identity>) new List<Identity>();
      this.OptionalProperties = optionalProperties ?? (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    Guid ISecuredObject.NamespaceId => IdentityPickerSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => IdentityPickerSecurityConstants.RootToken;
  }
}
