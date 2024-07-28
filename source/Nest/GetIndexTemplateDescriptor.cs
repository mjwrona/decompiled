// Decompiled with JetBrains decompiler
// Type: Nest.GetIndexTemplateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class GetIndexTemplateDescriptor : 
    RequestDescriptorBase<GetIndexTemplateDescriptor, GetIndexTemplateRequestParameters, IGetIndexTemplateRequest>,
    IGetIndexTemplateRequest,
    IRequest<GetIndexTemplateRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetTemplate;

    public GetIndexTemplateDescriptor()
    {
    }

    public GetIndexTemplateDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Names IGetIndexTemplateRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public GetIndexTemplateDescriptor Name(Names name) => this.Assign<Names>(name, (Action<IGetIndexTemplateRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public GetIndexTemplateDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public GetIndexTemplateDescriptor IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    public GetIndexTemplateDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public GetIndexTemplateDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
