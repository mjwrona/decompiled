// Decompiled with JetBrains decompiler
// Type: Nest.ExecuteEnrichPolicyRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.EnrichApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ExecuteEnrichPolicyRequest : 
    PlainRequestBase<ExecuteEnrichPolicyRequestParameters>,
    IExecuteEnrichPolicyRequest,
    IRequest<ExecuteEnrichPolicyRequestParameters>,
    IRequest
  {
    protected IExecuteEnrichPolicyRequest Self => (IExecuteEnrichPolicyRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.EnrichExecutePolicy;

    public ExecuteEnrichPolicyRequest(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected ExecuteEnrichPolicyRequest()
    {
    }

    [IgnoreDataMember]
    Name IExecuteEnrichPolicyRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
