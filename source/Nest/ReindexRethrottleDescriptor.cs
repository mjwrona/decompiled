// Decompiled with JetBrains decompiler
// Type: Nest.ReindexRethrottleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ReindexRethrottleDescriptor : 
    RequestDescriptorBase<ReindexRethrottleDescriptor, ReindexRethrottleRequestParameters, IReindexRethrottleRequest>,
    IReindexRethrottleRequest,
    IRequest<ReindexRethrottleRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceReindexRethrottle;

    public ReindexRethrottleDescriptor(TaskId taskId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("task_id", (IUrlParameter) taskId)))
    {
    }

    [SerializationConstructor]
    protected ReindexRethrottleDescriptor()
    {
    }

    TaskId IReindexRethrottleRequest.TaskId => this.Self.RouteValues.Get<TaskId>("task_id");

    public ReindexRethrottleDescriptor RequestsPerSecond(long? requestspersecond) => this.Qs("requests_per_second", (object) requestspersecond);
  }
}
