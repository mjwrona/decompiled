// Decompiled with JetBrains decompiler
// Type: Nest.IAsyncSearchSubmitRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.AsyncSearchApi;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [ReadAs(typeof (AsyncSearchSubmitRequest<>))]
  [InterfaceDataContract]
  public interface IAsyncSearchSubmitRequest<TInferDocument> : 
    IAsyncSearchSubmitRequest,
    IRequest<AsyncSearchSubmitRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
  }
}
