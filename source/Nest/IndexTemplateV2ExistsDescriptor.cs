// Decompiled with JetBrains decompiler
// Type: Nest.IndexTemplateV2ExistsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class IndexTemplateV2ExistsDescriptor : 
    RequestDescriptorBase<IndexTemplateV2ExistsDescriptor, IndexTemplateV2ExistsRequestParameters, IIndexTemplateV2ExistsRequest>,
    IIndexTemplateV2ExistsRequest,
    IRequest<IndexTemplateV2ExistsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesTemplateV2Exists;

    public IndexTemplateV2ExistsDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected IndexTemplateV2ExistsDescriptor()
    {
    }

    Name IIndexTemplateV2ExistsRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public IndexTemplateV2ExistsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public IndexTemplateV2ExistsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public IndexTemplateV2ExistsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
