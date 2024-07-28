// Decompiled with JetBrains decompiler
// Type: Nest.DeleteByQueryRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;

namespace Nest
{
  public class DeleteByQueryRequest<TDocument> : 
    DeleteByQueryRequest,
    IDeleteByQueryRequest<TDocument>,
    IDeleteByQueryRequest,
    IRequest<DeleteByQueryRequestParameters>,
    IRequest
    where TDocument : class
  {
    protected IDeleteByQueryRequest<TDocument> TypedSelf => (IDeleteByQueryRequest<TDocument>) this;

    public DeleteByQueryRequest(Indices index)
      : base(index)
    {
    }

    public DeleteByQueryRequest()
      : base((Indices) typeof (TDocument))
    {
    }
  }
}
