// Decompiled with JetBrains decompiler
// Type: Nest.PutMappingRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndicesApi;

namespace Nest
{
  public class PutMappingRequest<TDocument> : 
    PutMappingRequest,
    IPutMappingRequest<TDocument>,
    IPutMappingRequest,
    ITypeMapping,
    IRequest<PutMappingRequestParameters>,
    IRequest
    where TDocument : class
  {
    protected IPutMappingRequest<TDocument> TypedSelf => (IPutMappingRequest<TDocument>) this;

    public PutMappingRequest(Indices index)
      : base(index)
    {
    }

    public PutMappingRequest()
      : base((Indices) typeof (TDocument))
    {
    }
  }
}
