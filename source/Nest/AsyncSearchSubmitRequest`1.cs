// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchSubmitRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.AsyncSearchApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AsyncSearchSubmitRequest<TInferDocument> : 
    AsyncSearchSubmitRequest,
    IAsyncSearchSubmitRequest<TInferDocument>,
    IAsyncSearchSubmitRequest,
    IRequest<AsyncSearchSubmitRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    protected IAsyncSearchSubmitRequest<TInferDocument> TypedSelf => (IAsyncSearchSubmitRequest<TInferDocument>) this;

    public AsyncSearchSubmitRequest()
      : base((Indices) typeof (TInferDocument))
    {
    }

    public AsyncSearchSubmitRequest(Indices index)
      : base(index)
    {
    }

    Type ITypedSearchRequest.ClrType => typeof (TInferDocument);
  }
}
