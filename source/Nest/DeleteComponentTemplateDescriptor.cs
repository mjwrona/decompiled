// Decompiled with JetBrains decompiler
// Type: Nest.DeleteComponentTemplateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteComponentTemplateDescriptor : 
    RequestDescriptorBase<DeleteComponentTemplateDescriptor, DeleteComponentTemplateRequestParameters, IDeleteComponentTemplateRequest>,
    IDeleteComponentTemplateRequest,
    IRequest<DeleteComponentTemplateRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterDeleteComponentTemplate;

    public DeleteComponentTemplateDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected DeleteComponentTemplateDescriptor()
    {
    }

    Name IDeleteComponentTemplateRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public DeleteComponentTemplateDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public DeleteComponentTemplateDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
