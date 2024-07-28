// Decompiled with JetBrains decompiler
// Type: Nest.ExplainRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  public class ExplainRequest<TDocument> : 
    ExplainRequest,
    IExplainRequest<TDocument>,
    IExplainRequest,
    IRequest<ExplainRequestParameters>,
    IRequest
    where TDocument : class
  {
    protected IExplainRequest<TDocument> TypedSelf => (IExplainRequest<TDocument>) this;

    public ExplainRequest(IndexName index, Id id)
      : base(index, id)
    {
    }

    public ExplainRequest(Id id)
      : base((IndexName) typeof (TDocument), id)
    {
    }

    public ExplainRequest(TDocument documentWithId, IndexName index = null, Id id = null)
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
    protected ExplainRequest()
    {
    }

    protected override HttpMethod HttpMethod
    {
      get
      {
        ExplainRequestParameters requestParameters1 = this.RequestState.RequestParameters;
        if ((requestParameters1 != null ? (__nonvirtual (requestParameters1.ContainsQueryString("source")) ? 1 : 0) : 0) == 0)
        {
          ExplainRequestParameters requestParameters2 = this.RequestState.RequestParameters;
          if ((requestParameters2 != null ? (__nonvirtual (requestParameters2.ContainsQueryString("q")) ? 1 : 0) : 0) == 0)
            return HttpMethod.POST;
        }
        return HttpMethod.GET;
      }
    }
  }
}
