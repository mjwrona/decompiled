// Decompiled with JetBrains decompiler
// Type: Nest.DeleteComponentTemplateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteComponentTemplateRequest : 
    PlainRequestBase<DeleteComponentTemplateRequestParameters>,
    IDeleteComponentTemplateRequest,
    IRequest<DeleteComponentTemplateRequestParameters>,
    IRequest
  {
    protected IDeleteComponentTemplateRequest Self => (IDeleteComponentTemplateRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterDeleteComponentTemplate;

    public DeleteComponentTemplateRequest(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected DeleteComponentTemplateRequest()
    {
    }

    [IgnoreDataMember]
    Name IDeleteComponentTemplateRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
