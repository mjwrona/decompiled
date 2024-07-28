// Decompiled with JetBrains decompiler
// Type: Nest.StopRollupJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class StopRollupJobDescriptor : 
    RequestDescriptorBase<StopRollupJobDescriptor, StopRollupJobRequestParameters, IStopRollupJobRequest>,
    IStopRollupJobRequest,
    IRequest<StopRollupJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupStopJob;

    public StopRollupJobDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected StopRollupJobDescriptor()
    {
    }

    Id IStopRollupJobRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public StopRollupJobDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public StopRollupJobDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);
  }
}
