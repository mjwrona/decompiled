// Decompiled with JetBrains decompiler
// Type: Nest.UpdateByQueryRethrottleRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class UpdateByQueryRethrottleRequest : 
    PlainRequestBase<UpdateByQueryRethrottleRequestParameters>,
    IUpdateByQueryRethrottleRequest,
    IRequest<UpdateByQueryRethrottleRequestParameters>,
    IRequest
  {
    protected IUpdateByQueryRethrottleRequest Self => (IUpdateByQueryRethrottleRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceUpdateByQueryRethrottle;

    public UpdateByQueryRethrottleRequest(TaskId taskId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("task_id", (IUrlParameter) taskId)))
    {
    }

    [SerializationConstructor]
    protected UpdateByQueryRethrottleRequest()
    {
    }

    [IgnoreDataMember]
    TaskId IUpdateByQueryRethrottleRequest.TaskId => this.Self.RouteValues.Get<TaskId>("task_id");

    public long? RequestsPerSecond
    {
      get => this.Q<long?>("requests_per_second");
      set => this.Q("requests_per_second", (object) value);
    }
  }
}
