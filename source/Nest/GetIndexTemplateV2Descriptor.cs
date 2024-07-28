// Decompiled with JetBrains decompiler
// Type: Nest.GetIndexTemplateV2Descriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class GetIndexTemplateV2Descriptor : 
    RequestDescriptorBase<GetIndexTemplateV2Descriptor, GetIndexTemplateV2RequestParameters, IGetIndexTemplateV2Request>,
    IGetIndexTemplateV2Request,
    IRequest<GetIndexTemplateV2RequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetTemplateV2;

    public GetIndexTemplateV2Descriptor()
    {
    }

    public GetIndexTemplateV2Descriptor(Nest.Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Nest.Name IGetIndexTemplateV2Request.Name => this.Self.RouteValues.Get<Nest.Name>("name");

    public GetIndexTemplateV2Descriptor Name(Nest.Name name) => this.Assign<Nest.Name>(name, (Action<IGetIndexTemplateV2Request, Nest.Name>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public GetIndexTemplateV2Descriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public GetIndexTemplateV2Descriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public GetIndexTemplateV2Descriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
