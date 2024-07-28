// Decompiled with JetBrains decompiler
// Type: Nest.CancelTasksRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TasksApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CancelTasksRequest : 
    PlainRequestBase<CancelTasksRequestParameters>,
    ICancelTasksRequest,
    IRequest<CancelTasksRequestParameters>,
    IRequest
  {
    protected ICancelTasksRequest Self => (ICancelTasksRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TasksCancel;

    public CancelTasksRequest()
    {
    }

    public CancelTasksRequest(TaskId taskId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("task_id", (IUrlParameter) taskId)))
    {
    }

    [IgnoreDataMember]
    TaskId ICancelTasksRequest.TaskId => this.Self.RouteValues.Get<TaskId>("task_id");

    public string[] Actions
    {
      get => this.Q<string[]>("actions");
      set => this.Q("actions", (object) value);
    }

    public string[] Nodes
    {
      get => this.Q<string[]>("nodes");
      set => this.Q("nodes", (object) value);
    }

    public string ParentTaskId
    {
      get => this.Q<string>("parent_task_id");
      set => this.Q("parent_task_id", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
