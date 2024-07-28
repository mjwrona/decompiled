// Decompiled with JetBrains decompiler
// Type: Nest.GetScriptRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetScriptRequest : 
    PlainRequestBase<GetScriptRequestParameters>,
    IGetScriptRequest,
    IRequest<GetScriptRequestParameters>,
    IRequest
  {
    protected IGetScriptRequest Self => (IGetScriptRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceGetScript;

    public GetScriptRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected GetScriptRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetScriptRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}
