// Decompiled with JetBrains decompiler
// Type: Nest.DeleteForecastDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteForecastDescriptor : 
    RequestDescriptorBase<DeleteForecastDescriptor, DeleteForecastRequestParameters, IDeleteForecastRequest>,
    IDeleteForecastRequest,
    IRequest<DeleteForecastRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteForecast;

    public DeleteForecastDescriptor(Id jobId, Ids forecastId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Required("forecast_id", (IUrlParameter) forecastId)))
    {
    }

    [SerializationConstructor]
    protected DeleteForecastDescriptor()
    {
    }

    Id IDeleteForecastRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    Ids IDeleteForecastRequest.ForecastId => this.Self.RouteValues.Get<Ids>("forecast_id");

    public DeleteForecastDescriptor AllowNoForecasts(bool? allownoforecasts = true) => this.Qs("allow_no_forecasts", (object) allownoforecasts);

    public DeleteForecastDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
