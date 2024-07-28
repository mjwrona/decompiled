// Decompiled with JetBrains decompiler
// Type: Nest.StopRollupJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class StopRollupJobRequest : 
    PlainRequestBase<StopRollupJobRequestParameters>,
    IStopRollupJobRequest,
    IRequest<StopRollupJobRequestParameters>,
    IRequest
  {
    protected IStopRollupJobRequest Self => (IStopRollupJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupStopJob;

    public StopRollupJobRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected StopRollupJobRequest()
    {
    }

    [IgnoreDataMember]
    Id IStopRollupJobRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
