// Decompiled with JetBrains decompiler
// Type: Nest.GetRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  public class GetRequest<TDocument> : 
    GetRequest,
    IGetRequest<TDocument>,
    IGetRequest,
    IRequest<GetRequestParameters>,
    IRequest
    where TDocument : class
  {
    protected IGetRequest<TDocument> TypedSelf => (IGetRequest<TDocument>) this;

    public GetRequest(IndexName index, Id id)
      : base(index, id)
    {
    }

    public GetRequest(Id id)
      : base((IndexName) typeof (TDocument), id)
    {
    }

    public GetRequest(TDocument documentWithId, IndexName index = null, Id id = null)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = (IndexName) typeof (TDocument);
      Id id1 = id;
      if ((object) id1 == null)
        id1 = Id.From<TDocument>(documentWithId);
      // ISSUE: explicit constructor call
      this.\u002Ector(index1, id1);
    }

    [SerializationConstructor]
    protected GetRequest()
    {
    }
  }
}
