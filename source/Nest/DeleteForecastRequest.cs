// Decompiled with JetBrains decompiler
// Type: Nest.DeleteForecastRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteForecastRequest : 
    PlainRequestBase<DeleteForecastRequestParameters>,
    IDeleteForecastRequest,
    IRequest<DeleteForecastRequestParameters>,
    IRequest
  {
    protected IDeleteForecastRequest Self => (IDeleteForecastRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteForecast;

    public DeleteForecastRequest(Id jobId, Ids forecastId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Required("forecast_id", (IUrlParameter) forecastId)))
    {
    }

    [SerializationConstructor]
    protected DeleteForecastRequest()
    {
    }

    [IgnoreDataMember]
    Id IDeleteForecastRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    [IgnoreDataMember]
    Ids IDeleteForecastRequest.ForecastId => this.Self.RouteValues.Get<Ids>("forecast_id");

    public bool? AllowNoForecasts
    {
      get => this.Q<bool?>("allow_no_forecasts");
      set => this.Q("allow_no_forecasts", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
