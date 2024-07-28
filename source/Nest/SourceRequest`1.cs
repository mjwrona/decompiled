// Decompiled with JetBrains decompiler
// Type: Nest.SourceRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  public class SourceRequest<TDocument> : 
    SourceRequest,
    ISourceRequest<TDocument>,
    ISourceRequest,
    IRequest<SourceRequestParameters>,
    IRequest
    where TDocument : class
  {
    protected ISourceRequest<TDocument> TypedSelf => (ISourceRequest<TDocument>) this;

    public SourceRequest(IndexName index, Id id)
      : base(index, id)
    {
    }

    public SourceRequest(Id id)
      : base((IndexName) typeof (TDocument), id)
    {
    }

    public SourceRequest(TDocument documentWithId, IndexName index = null, Id id = null)
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
    protected SourceRequest()
    {
    }
  }
}
