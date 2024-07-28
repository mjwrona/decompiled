// Decompiled with JetBrains decompiler
// Type: Nest.PostJobDataRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class PostJobDataRequest : 
    PlainRequestBase<PostJobDataRequestParameters>,
    IPostJobDataRequest,
    IRequest<PostJobDataRequestParameters>,
    IRequest
  {
    protected IPostJobDataRequest Self => (IPostJobDataRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPostJobData;

    public PostJobDataRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected PostJobDataRequest()
    {
    }

    [IgnoreDataMember]
    Id IPostJobDataRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public DateTimeOffset? ResetEnd
    {
      get => this.Q<DateTimeOffset?>("reset_end");
      set => this.Q("reset_end", (object) value);
    }

    public DateTimeOffset? ResetStart
    {
      get => this.Q<DateTimeOffset?>("reset_start");
      set => this.Q("reset_start", (object) value);
    }

    public IEnumerable<object> Data { get; set; }
  }
}
