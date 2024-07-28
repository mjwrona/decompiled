// Decompiled with JetBrains decompiler
// Type: Nest.DeleteTransformDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteTransformDescriptor : 
    RequestDescriptorBase<DeleteTransformDescriptor, DeleteTransformRequestParameters, IDeleteTransformRequest>,
    IDeleteTransformRequest,
    IRequest<DeleteTransformRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformDelete;

    public DeleteTransformDescriptor(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected DeleteTransformDescriptor()
    {
    }

    Id IDeleteTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public DeleteTransformDescriptor Force(bool? force = true) => this.Qs(nameof (force), (object) force);

    public DeleteTransformDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
