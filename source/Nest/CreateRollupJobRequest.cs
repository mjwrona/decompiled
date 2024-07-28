// Decompiled with JetBrains decompiler
// Type: Nest.CreateRollupJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class CreateRollupJobRequest : 
    PlainRequestBase<CreateRollupJobRequestParameters>,
    ICreateRollupJobRequest,
    IRequest<CreateRollupJobRequestParameters>,
    IRequest
  {
    protected ICreateRollupJobRequest Self => (ICreateRollupJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupCreateJob;

    public CreateRollupJobRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected CreateRollupJobRequest()
    {
    }

    [IgnoreDataMember]
    Id ICreateRollupJobRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public string Cron { get; set; }

    public IRollupGroupings Groups { get; set; }

    public string IndexPattern { get; set; }

    public IEnumerable<IRollupFieldMetric> Metrics { get; set; }

    public long? PageSize { get; set; }

    public IndexName RollupIndex { get; set; }
  }
}
