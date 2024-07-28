// Decompiled with JetBrains decompiler
// Type: Nest.ForecastJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ForecastJobDescriptor : 
    RequestDescriptorBase<ForecastJobDescriptor, ForecastJobRequestParameters, IForecastJobRequest>,
    IForecastJobRequest,
    IRequest<ForecastJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningForecastJob;

    public ForecastJobDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected ForecastJobDescriptor()
    {
    }

    Id IForecastJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public ForecastJobDescriptor MaxModelMemory(string maxmodelmemory) => this.Qs("max_model_memory", (object) maxmodelmemory);

    Time IForecastJobRequest.Duration { get; set; }

    Time IForecastJobRequest.ExpiresIn { get; set; }

    public ForecastJobDescriptor Duration(Time duration) => this.Assign<Time>(duration, (Action<IForecastJobRequest, Time>) ((a, v) => a.Duration = v));

    public ForecastJobDescriptor ExpiresIn(Time expiresIn) => this.Assign<Time>(expiresIn, (Action<IForecastJobRequest, Time>) ((a, v) => a.ExpiresIn = v));
  }
}
