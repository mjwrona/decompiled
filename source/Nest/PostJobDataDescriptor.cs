// Decompiled with JetBrains decompiler
// Type: Nest.PostJobDataDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class PostJobDataDescriptor : 
    RequestDescriptorBase<PostJobDataDescriptor, PostJobDataRequestParameters, IPostJobDataRequest>,
    IPostJobDataRequest,
    IRequest<PostJobDataRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPostJobData;

    public PostJobDataDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected PostJobDataDescriptor()
    {
    }

    Id IPostJobDataRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public PostJobDataDescriptor ResetEnd(DateTimeOffset? resetend) => this.Qs("reset_end", (object) resetend);

    public PostJobDataDescriptor ResetStart(DateTimeOffset? resetstart) => this.Qs("reset_start", (object) resetstart);

    IEnumerable<object> IPostJobDataRequest.Data { get; set; }

    public PostJobDataDescriptor Data(IEnumerable<object> data) => this.Assign<IEnumerable<object>>(data, (Action<IPostJobDataRequest, IEnumerable<object>>) ((a, v) => a.Data = v));

    public PostJobDataDescriptor Data(params object[] data) => this.Assign<object[]>(data, (Action<IPostJobDataRequest, object[]>) ((a, v) =>
    {
      if (v != null && v.Length == 1 && v[0] is IEnumerable source2)
        a.Data = source2.Cast<object>();
      else
        a.Data = (IEnumerable<object>) v;
    }));
  }
}
