// Decompiled with JetBrains decompiler
// Type: Nest.SearchShardsRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;

namespace Nest
{
  public class SearchShardsRequest<TDocument> : 
    SearchShardsRequest,
    ISearchShardsRequest<TDocument>,
    ISearchShardsRequest,
    IRequest<SearchShardsRequestParameters>,
    IRequest
    where TDocument : class
  {
    protected ISearchShardsRequest<TDocument> TypedSelf => (ISearchShardsRequest<TDocument>) this;

    public SearchShardsRequest()
      : base((Indices) typeof (TDocument))
    {
    }

    public SearchShardsRequest(Indices index)
      : base(index)
    {
    }
  }
}
