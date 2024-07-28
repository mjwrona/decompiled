// Decompiled with JetBrains decompiler
// Type: Nest.UpdateByQueryRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;

namespace Nest
{
  public class UpdateByQueryRequest<TDocument> : 
    UpdateByQueryRequest,
    IUpdateByQueryRequest<TDocument>,
    IUpdateByQueryRequest,
    IRequest<UpdateByQueryRequestParameters>,
    IRequest
    where TDocument : class
  {
    protected IUpdateByQueryRequest<TDocument> TypedSelf => (IUpdateByQueryRequest<TDocument>) this;

    public UpdateByQueryRequest(Indices index)
      : base(index)
    {
    }

    public UpdateByQueryRequest()
      : base((Indices) typeof (TDocument))
    {
    }
  }
}
