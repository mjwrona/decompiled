// Decompiled with JetBrains decompiler
// Type: Nest.ExecuteEnrichPolicyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.EnrichApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ExecuteEnrichPolicyDescriptor : 
    RequestDescriptorBase<ExecuteEnrichPolicyDescriptor, ExecuteEnrichPolicyRequestParameters, IExecuteEnrichPolicyRequest>,
    IExecuteEnrichPolicyRequest,
    IRequest<ExecuteEnrichPolicyRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.EnrichExecutePolicy;

    public ExecuteEnrichPolicyDescriptor(Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected ExecuteEnrichPolicyDescriptor()
    {
    }

    Name IExecuteEnrichPolicyRequest.Name => this.Self.RouteValues.Get<Name>("name");

    public ExecuteEnrichPolicyDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);
  }
}
