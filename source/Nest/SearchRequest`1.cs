// Decompiled with JetBrains decompiler
// Type: Nest.SearchRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SearchRequest<TInferDocument> : 
    SearchRequest,
    ISearchRequest<TInferDocument>,
    ISearchRequest,
    IRequest<SearchRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    protected ISearchRequest<TInferDocument> TypedSelf => (ISearchRequest<TInferDocument>) this;

    public SearchRequest()
      : base((Indices) typeof (TInferDocument))
    {
    }

    public SearchRequest(Indices index)
      : base(index)
    {
    }

    Type ITypedSearchRequest.ClrType => typeof (TInferDocument);
  }
}
