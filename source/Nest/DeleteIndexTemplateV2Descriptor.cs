// Decompiled with JetBrains decompiler
// Type: Nest.DeleteIndexTemplateV2Descriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteIndexTemplateV2Descriptor : 
    RequestDescriptorBase<DeleteIndexTemplateV2Descriptor, DeleteIndexTemplateV2RequestParameters, IDeleteIndexTemplateV2Request>,
    IDeleteIndexTemplateV2Request,
    IRequest<DeleteIndexTemplateV2RequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesDeleteTemplateV2;

    public DeleteIndexTemplateV2Descriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected DeleteIndexTemplateV2Descriptor()
    {
    }

    Name IDeleteIndexTemplateV2Request.Name => this.Self.RouteValues.Get<Name>("name");

    public DeleteIndexTemplateV2Descriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public DeleteIndexTemplateV2Descriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
