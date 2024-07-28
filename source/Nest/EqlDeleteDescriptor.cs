// Decompiled with JetBrains decompiler
// Type: Nest.EqlDeleteDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.EqlApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class EqlDeleteDescriptor : 
    RequestDescriptorBase<EqlDeleteDescriptor, EqlDeleteRequestParameters, IEqlDeleteRequest>,
    IEqlDeleteRequest,
    IRequest<EqlDeleteRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.EqlDelete;

    public EqlDeleteDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected EqlDeleteDescriptor()
    {
    }

    Id IEqlDeleteRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
