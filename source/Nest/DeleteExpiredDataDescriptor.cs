// Decompiled with JetBrains decompiler
// Type: Nest.DeleteExpiredDataDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class DeleteExpiredDataDescriptor : 
    RequestDescriptorBase<DeleteExpiredDataDescriptor, DeleteExpiredDataRequestParameters, IDeleteExpiredDataRequest>,
    IDeleteExpiredDataRequest,
    IRequest<DeleteExpiredDataRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteExpiredData;

    public DeleteExpiredDataDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("job_id", (IUrlParameter) jobId)))
    {
    }

    public DeleteExpiredDataDescriptor()
    {
    }

    Id IDeleteExpiredDataRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public DeleteExpiredDataDescriptor JobId(Id jobId) => this.Assign<Id>(jobId, (Action<IDeleteExpiredDataRequest, Id>) ((a, v) => a.RouteValues.Optional("job_id", (IUrlParameter) v)));

    public DeleteExpiredDataDescriptor RequestsPerSecond(long? requestspersecond) => this.Qs("requests_per_second", (object) requestspersecond);

    public DeleteExpiredDataDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
