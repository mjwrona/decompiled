// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiContext
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiContext : IWebApiContext
  {
    private HttpRequestMessageProperties innerContext;

    public WebApiContext(HttpRequestMessageProperties context) => this.innerContext = context != null ? context : throw Error.ArgumentNull(nameof (context));

    public ApplyClause ApplyClause
    {
      get => this.innerContext.ApplyClause;
      set => this.innerContext.ApplyClause = value;
    }

    public Uri NextLink
    {
      get => this.innerContext.NextLink;
      set => this.innerContext.NextLink = value;
    }

    public Uri DeltaLink
    {
      get => this.innerContext.DeltaLink;
      set => this.innerContext.DeltaLink = value;
    }

    public Microsoft.AspNet.OData.Routing.ODataPath Path => this.innerContext.Path;

    public string RouteName => this.innerContext.RouteName;

    public IDictionary<string, object> RoutingConventionsStore => this.innerContext.RoutingConventionsStore;

    public SelectExpandClause ProcessedSelectExpandClause
    {
      get => this.innerContext.SelectExpandClause;
      set => this.innerContext.SelectExpandClause = value;
    }

    public ODataQueryOptions QueryOptions
    {
      get => this.innerContext.QueryOptions;
      set => this.innerContext.QueryOptions = value;
    }

    public long? TotalCount => this.innerContext.TotalCount;

    public int PageSize
    {
      get => this.innerContext.PageSize;
      set => this.innerContext.PageSize = value;
    }

    public Func<long> TotalCountFunc
    {
      get => this.innerContext.TotalCountFunc;
      set => this.innerContext.TotalCountFunc = value;
    }
  }
}
