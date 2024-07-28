// Decompiled with JetBrains decompiler
// Type: Nest.DeleteExpiredDataRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteExpiredDataRequest : 
    PlainRequestBase<DeleteExpiredDataRequestParameters>,
    IDeleteExpiredDataRequest,
    IRequest<DeleteExpiredDataRequestParameters>,
    IRequest
  {
    protected IDeleteExpiredDataRequest Self => (IDeleteExpiredDataRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteExpiredData;

    public DeleteExpiredDataRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("job_id", (IUrlParameter) jobId)))
    {
    }

    public DeleteExpiredDataRequest()
    {
    }

    [IgnoreDataMember]
    Id IDeleteExpiredDataRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public long? RequestsPerSecond
    {
      get => this.Q<long?>("requests_per_second");
      set => this.Q("requests_per_second", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
