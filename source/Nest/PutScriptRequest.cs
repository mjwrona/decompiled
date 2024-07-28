// Decompiled with JetBrains decompiler
// Type: Nest.PutScriptRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutScriptRequest : 
    PlainRequestBase<PutScriptRequestParameters>,
    IPutScriptRequest,
    IRequest<PutScriptRequestParameters>,
    IRequest
  {
    public IStoredScript Script { get; set; }

    protected IPutScriptRequest Self => (IPutScriptRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespacePutScript;

    public PutScriptRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public PutScriptRequest(Id id, Name context)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id).Optional(nameof (context), (IUrlParameter) context)))
    {
    }

    [SerializationConstructor]
    protected PutScriptRequest()
    {
    }

    [IgnoreDataMember]
    Id IPutScriptRequest.Id => this.Self.RouteValues.Get<Id>("id");

    [IgnoreDataMember]
    Name IPutScriptRequest.Context => this.Self.RouteValues.Get<Name>("context");

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
