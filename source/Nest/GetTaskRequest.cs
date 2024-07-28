// Decompiled with JetBrains decompiler
// Type: Nest.GetTaskRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TasksApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetTaskRequest : 
    PlainRequestBase<GetTaskRequestParameters>,
    IGetTaskRequest,
    IRequest<GetTaskRequestParameters>,
    IRequest
  {
    protected IGetTaskRequest Self => (IGetTaskRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TasksGetTask;

    public GetTaskRequest(TaskId taskId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("task_id", (IUrlParameter) taskId)))
    {
    }

    [SerializationConstructor]
    protected GetTaskRequest()
    {
    }

    [IgnoreDataMember]
    TaskId IGetTaskRequest.TaskId => this.Self.RouteValues.Get<TaskId>("task_id");

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
