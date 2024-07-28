// Decompiled with JetBrains decompiler
// Type: Nest.QueryWatchesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.WatcherApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class QueryWatchesDescriptor : 
    RequestDescriptorBase<QueryWatchesDescriptor, QueryWatchesRequestParameters, IQueryWatchesRequest>,
    IQueryWatchesRequest,
    IRequest<QueryWatchesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherQueryWatches;

    int? IQueryWatchesRequest.From { get; set; }

    QueryContainer IQueryWatchesRequest.Query { get; set; }

    IList<object> IQueryWatchesRequest.SearchAfter { get; set; }

    int? IQueryWatchesRequest.Size { get; set; }

    IList<ISort> IQueryWatchesRequest.Sort { get; set; }

    public QueryWatchesDescriptor From(int? from) => this.Assign<int?>(from, (Action<IQueryWatchesRequest, int?>) ((a, v) => a.From = v));

    public QueryWatchesDescriptor Query(
      Func<QueryContainerDescriptor<Watch>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<Watch>, QueryContainer>>(query, (Action<IQueryWatchesRequest, Func<QueryContainerDescriptor<Watch>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<Watch>()) : (QueryContainer) null));
    }

    public QueryWatchesDescriptor SearchAfter(IEnumerable<object> searchAfter) => this.Assign<IEnumerable<object>>(searchAfter, (Action<IQueryWatchesRequest, IEnumerable<object>>) ((a, v) => a.SearchAfter = v != null ? (IList<object>) v.ToListOrNullIfEmpty<object>() : (IList<object>) null));

    public QueryWatchesDescriptor SearchAfter(IList<object> searchAfter) => this.Assign<IList<object>>(searchAfter, (Action<IQueryWatchesRequest, IList<object>>) ((a, v) => a.SearchAfter = v));

    public QueryWatchesDescriptor SearchAfter(params object[] searchAfter) => this.Assign<object[]>(searchAfter, (Action<IQueryWatchesRequest, object[]>) ((a, v) => a.SearchAfter = (IList<object>) v));

    public QueryWatchesDescriptor Size(int? size) => this.Assign<int?>(size, (Action<IQueryWatchesRequest, int?>) ((a, v) => a.Size = v));

    public QueryWatchesDescriptor Sort(
      Func<SortDescriptor<Watch>, IPromise<IList<ISort>>> selector)
    {
      return this.Assign<Func<SortDescriptor<Watch>, IPromise<IList<ISort>>>>(selector, (Action<IQueryWatchesRequest, Func<SortDescriptor<Watch>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<Watch>())?.Value : (IList<ISort>) null));
    }
  }
}
