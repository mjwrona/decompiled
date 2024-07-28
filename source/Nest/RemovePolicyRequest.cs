// Decompiled with JetBrains decompiler
// Type: Nest.RemovePolicyRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class RemovePolicyRequest : 
    PlainRequestBase<RemovePolicyRequestParameters>,
    IRemovePolicyRequest,
    IRequest<RemovePolicyRequestParameters>,
    IRequest
  {
    protected IRemovePolicyRequest Self => (IRemovePolicyRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementRemovePolicy;

    public RemovePolicyRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected RemovePolicyRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IRemovePolicyRequest.Index => this.Self.RouteValues.Get<IndexName>("index");
  }
}
