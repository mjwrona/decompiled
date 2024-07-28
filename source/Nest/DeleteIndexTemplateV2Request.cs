// Decompiled with JetBrains decompiler
// Type: Nest.DeleteIndexTemplateV2Request
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteIndexTemplateV2Request : 
    PlainRequestBase<DeleteIndexTemplateV2RequestParameters>,
    IDeleteIndexTemplateV2Request,
    IRequest<DeleteIndexTemplateV2RequestParameters>,
    IRequest
  {
    protected IDeleteIndexTemplateV2Request Self => (IDeleteIndexTemplateV2Request) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesDeleteTemplateV2;

    public DeleteIndexTemplateV2Request(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected DeleteIndexTemplateV2Request()
    {
    }

    [IgnoreDataMember]
    Name IDeleteIndexTemplateV2Request.Name => this.Self.RouteValues.Get<Name>("name");

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
