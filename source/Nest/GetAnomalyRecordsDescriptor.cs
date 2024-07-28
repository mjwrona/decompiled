// Decompiled with JetBrains decompiler
// Type: Nest.GetAnomalyRecordsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetAnomalyRecordsDescriptor : 
    RequestDescriptorBase<GetAnomalyRecordsDescriptor, GetAnomalyRecordsRequestParameters, IGetAnomalyRecordsRequest>,
    IGetAnomalyRecordsRequest,
    IRequest<GetAnomalyRecordsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetAnomalyRecords;

    public GetAnomalyRecordsDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetAnomalyRecordsDescriptor()
    {
    }

    Id IGetAnomalyRecordsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    bool? IGetAnomalyRecordsRequest.Descending { get; set; }

    DateTimeOffset? IGetAnomalyRecordsRequest.End { get; set; }

    bool? IGetAnomalyRecordsRequest.ExcludeInterim { get; set; }

    IPage IGetAnomalyRecordsRequest.Page { get; set; }

    double? IGetAnomalyRecordsRequest.RecordScore { get; set; }

    Field IGetAnomalyRecordsRequest.Sort { get; set; }

    DateTimeOffset? IGetAnomalyRecordsRequest.Start { get; set; }

    public GetAnomalyRecordsDescriptor Descending(bool? descending = true) => this.Assign<bool?>(descending, (Action<IGetAnomalyRecordsRequest, bool?>) ((a, v) => a.Descending = v));

    public GetAnomalyRecordsDescriptor End(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IGetAnomalyRecordsRequest, DateTimeOffset?>) ((a, v) => a.End = v));

    public GetAnomalyRecordsDescriptor Page(Func<PageDescriptor, IPage> selector) => this.Assign<Func<PageDescriptor, IPage>>(selector, (Action<IGetAnomalyRecordsRequest, Func<PageDescriptor, IPage>>) ((a, v) => a.Page = v != null ? v(new PageDescriptor()) : (IPage) null));

    public GetAnomalyRecordsDescriptor Sort(Field field) => this.Assign<Field>(field, (Action<IGetAnomalyRecordsRequest, Field>) ((a, v) => a.Sort = v));

    public GetAnomalyRecordsDescriptor Start(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IGetAnomalyRecordsRequest, DateTimeOffset?>) ((a, v) => a.Start = v));

    public GetAnomalyRecordsDescriptor ExcludeInterim(bool? excludeInterim = true) => this.Assign<bool?>(excludeInterim, (Action<IGetAnomalyRecordsRequest, bool?>) ((a, v) => a.ExcludeInterim = v));

    public GetAnomalyRecordsDescriptor RecordScore(double? recordScore) => this.Assign<double?>(recordScore, (Action<IGetAnomalyRecordsRequest, double?>) ((a, v) => a.RecordScore = v));
  }
}
