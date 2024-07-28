// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions.IndexSubScope
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions
{
  public class IndexSubScope
  {
    public string AccountId { get; set; } = string.Empty;

    public string CollectionId { get; set; } = string.Empty;

    public IndexInfo IndexRouting { get; set; }

    internal string GetRouteInfoToUpdate(RouteLevel routingLevel) => this.IndexRouting != null && this.IndexRouting.Routing != null ? this.IndexRouting.Routing : IndexSubScope.GetRoutingValue(routingLevel, this.CollectionId);

    internal static string GetRoutingValuesToPublishIndex(
      RouteLevel routeLevel,
      IList<IndexSubScope> validSubScopes)
    {
      return RouteLevel.None == routeLevel || validSubScopes == null || validSubScopes.Count <= 0 ? string.Empty : string.Join(",", validSubScopes.Select<IndexSubScope, string>((Func<IndexSubScope, string>) (repo => repo.IndexRouting?.Routing ?? IndexSubScope.GetRoutingValueForQueryIndexInfo(routeLevel, validSubScopes[0].AccountId, validSubScopes[0].CollectionId))).Distinct<string>());
    }

    internal static string GetRoutingValueForQueryIndexInfo(
      RouteLevel routingLevel,
      string accountId,
      string collectionId,
      bool throwIfNullOrWhiteSpace = true)
    {
      if (RouteLevel.None == routingLevel)
        return string.Empty;
      if (routingLevel == RouteLevel.Account)
      {
        if (string.IsNullOrWhiteSpace(accountId) & throwIfNullOrWhiteSpace)
          throw new ArgumentException("GetRoutingValue", nameof (accountId));
        if (string.IsNullOrWhiteSpace(collectionId) & throwIfNullOrWhiteSpace)
          throw new ArgumentException("GetRoutingValue", nameof (collectionId));
        return accountId + "," + collectionId;
      }
      if (RouteLevel.Collection != routingLevel)
        return string.Empty;
      return !(string.IsNullOrWhiteSpace(collectionId) & throwIfNullOrWhiteSpace) ? collectionId : throw new ArgumentException("GetRoutingValue", nameof (collectionId));
    }

    internal static string GetRoutingValue(
      RouteLevel routingLevel,
      string collectionId,
      bool throwIfNullOrWhiteSpace = true)
    {
      if (RouteLevel.None == routingLevel || routingLevel != RouteLevel.Account)
        return string.Empty;
      return !(string.IsNullOrWhiteSpace(collectionId) & throwIfNullOrWhiteSpace) ? collectionId : throw new ArgumentException(nameof (GetRoutingValue), nameof (collectionId));
    }

    public override string ToString() => "AccountId " + this.AccountId + " CollectionId " + this.CollectionId;

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj.GetType() == this.GetType()) && !obj.GetType().IsSubclassOf(this.GetType()))
        return false;
      IndexSubScope indexSubScope = (IndexSubScope) obj;
      if ((this.AccountId != null || indexSubScope.AccountId != null) && (this.AccountId == null || !this.AccountId.Equals(indexSubScope.AccountId)))
        return false;
      if (this.CollectionId == null && indexSubScope.CollectionId == null)
        return true;
      return this.CollectionId != null && this.CollectionId.Equals(indexSubScope.CollectionId);
    }

    public override int GetHashCode() => this.ToString().GetHashCode();
  }
}
