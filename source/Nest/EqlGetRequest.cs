// Decompiled with JetBrains decompiler
// Type: Nest.EqlGetRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.EqlApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class EqlGetRequest : 
    PlainRequestBase<EqlGetRequestParameters>,
    IEqlGetRequest,
    IRequest<EqlGetRequestParameters>,
    IRequest
  {
    protected IEqlGetRequest Self => (IEqlGetRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.EqlGet;

    public EqlGetRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected EqlGetRequest()
    {
    }

    [IgnoreDataMember]
    Id IEqlGetRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public Time KeepAlive
    {
      get => this.Q<Time>("keep_alive");
      set => this.Q("keep_alive", (object) value);
    }

    public Time WaitForCompletionTimeout
    {
      get => this.Q<Time>("wait_for_completion_timeout");
      set => this.Q("wait_for_completion_timeout", (object) value);
    }
  }
}
