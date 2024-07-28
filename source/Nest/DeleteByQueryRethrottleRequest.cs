// Decompiled with JetBrains decompiler
// Type: Nest.DeleteByQueryRethrottleRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteByQueryRethrottleRequest : 
    PlainRequestBase<DeleteByQueryRethrottleRequestParameters>,
    IDeleteByQueryRethrottleRequest,
    IRequest<DeleteByQueryRethrottleRequestParameters>,
    IRequest
  {
    protected IDeleteByQueryRethrottleRequest Self => (IDeleteByQueryRethrottleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceDeleteByQueryRethrottle;

    public DeleteByQueryRethrottleRequest(TaskId taskId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("task_id", (IUrlParameter) taskId)))
    {
    }

    [SerializationConstructor]
    protected DeleteByQueryRethrottleRequest()
    {
    }

    [IgnoreDataMember]
    TaskId IDeleteByQueryRethrottleRequest.TaskId => this.Self.RouteValues.Get<TaskId>("task_id");

    public long? RequestsPerSecond
    {
      get => this.Q<long?>("requests_per_second");
      set => this.Q("requests_per_second", (object) value);
    }
  }
}
