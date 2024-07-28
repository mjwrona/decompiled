// Decompiled with JetBrains decompiler
// Type: Nest.ComponentTemplateExistsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ComponentTemplateExistsDescriptor : 
    RequestDescriptorBase<ComponentTemplateExistsDescriptor, ComponentTemplateExistsRequestParameters, IComponentTemplateExistsRequest>,
    IComponentTemplateExistsRequest,
    IRequest<ComponentTemplateExistsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterComponentTemplateExists;

    public ComponentTemplateExistsDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected ComponentTemplateExistsDescriptor()
    {
    }

    Name IComponentTemplateExistsRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public ComponentTemplateExistsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public ComponentTemplateExistsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
