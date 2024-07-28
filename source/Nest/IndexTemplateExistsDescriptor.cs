// Decompiled with JetBrains decompiler
// Type: Nest.IndexTemplateExistsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class IndexTemplateExistsDescriptor : 
    RequestDescriptorBase<IndexTemplateExistsDescriptor, IndexTemplateExistsRequestParameters, IIndexTemplateExistsRequest>,
    IIndexTemplateExistsRequest,
    IRequest<IndexTemplateExistsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesTemplateExists;

    public IndexTemplateExistsDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected IndexTemplateExistsDescriptor()
    {
    }

    Names IIndexTemplateExistsRequest.Name => this.Self.RouteValues.Get<Names>("name");

    public IndexTemplateExistsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public IndexTemplateExistsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public IndexTemplateExistsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
