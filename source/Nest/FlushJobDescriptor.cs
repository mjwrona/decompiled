// Decompiled with JetBrains decompiler
// Type: Nest.FlushJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class FlushJobDescriptor : 
    RequestDescriptorBase<FlushJobDescriptor, FlushJobRequestParameters, IFlushJobRequest>,
    IFlushJobRequest,
    IRequest<FlushJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningFlushJob;

    public FlushJobDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected FlushJobDescriptor()
    {
    }

    Id IFlushJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public FlushJobDescriptor SkipTime(string skiptime) => this.Qs("skip_time", (object) skiptime);

    DateTimeOffset? IFlushJobRequest.AdvanceTime { get; set; }

    bool? IFlushJobRequest.CalculateInterim { get; set; }

    DateTimeOffset? IFlushJobRequest.End { get; set; }

    DateTimeOffset? IFlushJobRequest.Start { get; set; }

    public FlushJobDescriptor AdvanceTime(DateTimeOffset? advanceTime) => this.Assign<DateTimeOffset?>(advanceTime, (Action<IFlushJobRequest, DateTimeOffset?>) ((a, v) => a.AdvanceTime = v));

    public FlushJobDescriptor CalculateInterim(bool? calculateInterim = true) => this.Assign<bool?>(calculateInterim, (Action<IFlushJobRequest, bool?>) ((a, v) => a.CalculateInterim = v));

    public FlushJobDescriptor End(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IFlushJobRequest, DateTimeOffset?>) ((a, v) => a.End = v));

    public FlushJobDescriptor Start(DateTimeOffset? start) => this.Assign<DateTimeOffset?>(start, (Action<IFlushJobRequest, DateTimeOffset?>) ((a, v) => a.Start = v));
  }
}
