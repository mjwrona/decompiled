// Decompiled with JetBrains decompiler
// Type: Nest.CreateRollupJobDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class CreateRollupJobDescriptor<TDocument> : 
    RequestDescriptorBase<CreateRollupJobDescriptor<TDocument>, CreateRollupJobRequestParameters, ICreateRollupJobRequest>,
    ICreateRollupJobRequest,
    IRequest<CreateRollupJobRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupCreateJob;

    public CreateRollupJobDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected CreateRollupJobDescriptor()
    {
    }

    Id ICreateRollupJobRequest.Id => this.Self.RouteValues.Get<Id>("id");

    string ICreateRollupJobRequest.Cron { get; set; }

    IRollupGroupings ICreateRollupJobRequest.Groups { get; set; }

    string ICreateRollupJobRequest.IndexPattern { get; set; }

    IEnumerable<IRollupFieldMetric> ICreateRollupJobRequest.Metrics { get; set; }

    long? ICreateRollupJobRequest.PageSize { get; set; }

    IndexName ICreateRollupJobRequest.RollupIndex { get; set; }

    public CreateRollupJobDescriptor<TDocument> IndexPattern(string indexPattern) => this.Assign<string>(indexPattern, (Action<ICreateRollupJobRequest, string>) ((a, v) => a.IndexPattern = v));

    public CreateRollupJobDescriptor<TDocument> RollupIndex(IndexName index) => this.Assign<IndexName>(index, (Action<ICreateRollupJobRequest, IndexName>) ((a, v) => a.RollupIndex = v));

    public CreateRollupJobDescriptor<TDocument> Cron(string cron) => this.Assign<string>(cron, (Action<ICreateRollupJobRequest, string>) ((a, v) => a.Cron = v));

    public CreateRollupJobDescriptor<TDocument> PageSize(long? pageSize) => this.Assign<long?>(pageSize, (Action<ICreateRollupJobRequest, long?>) ((a, v) => a.PageSize = v));

    public CreateRollupJobDescriptor<TDocument> Groups(
      Func<RollupGroupingsDescriptor<TDocument>, IRollupGroupings> selector)
    {
      return this.Assign<Func<RollupGroupingsDescriptor<TDocument>, IRollupGroupings>>(selector, (Action<ICreateRollupJobRequest, Func<RollupGroupingsDescriptor<TDocument>, IRollupGroupings>>) ((a, v) => a.Groups = v != null ? v(new RollupGroupingsDescriptor<TDocument>()) : (IRollupGroupings) null));
    }

    public CreateRollupJobDescriptor<TDocument> Metrics(
      Func<RollupFieldMetricsDescriptor<TDocument>, IPromise<IList<IRollupFieldMetric>>> selector)
    {
      return this.Assign<Func<RollupFieldMetricsDescriptor<TDocument>, IPromise<IList<IRollupFieldMetric>>>>(selector, (Action<ICreateRollupJobRequest, Func<RollupFieldMetricsDescriptor<TDocument>, IPromise<IList<IRollupFieldMetric>>>>) ((a, v) => a.Metrics = v != null ? (IEnumerable<IRollupFieldMetric>) v(new RollupFieldMetricsDescriptor<TDocument>())?.Value : (IEnumerable<IRollupFieldMetric>) null));
    }
  }
}
