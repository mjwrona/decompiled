// Decompiled with JetBrains decompiler
// Type: Nest.CancelTasksDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TasksApi;
using System;

namespace Nest
{
  public class CancelTasksDescriptor : 
    RequestDescriptorBase<CancelTasksDescriptor, CancelTasksRequestParameters, ICancelTasksRequest>,
    ICancelTasksRequest,
    IRequest<CancelTasksRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TasksCancel;

    public CancelTasksDescriptor()
    {
    }

    public CancelTasksDescriptor(Nest.TaskId taskId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("task_id", (IUrlParameter) taskId)))
    {
    }

    Nest.TaskId ICancelTasksRequest.TaskId => this.Self.RouteValues.Get<Nest.TaskId>("task_id");

    public CancelTasksDescriptor TaskId(Nest.TaskId taskId) => this.Assign<Nest.TaskId>(taskId, (Action<ICancelTasksRequest, Nest.TaskId>) ((a, v) => a.RouteValues.Optional("task_id", (IUrlParameter) v)));

    public CancelTasksDescriptor Actions(params string[] actions) => this.Qs(nameof (actions), (object) actions);

    public CancelTasksDescriptor Nodes(params string[] nodes) => this.Qs(nameof (nodes), (object) nodes);

    public CancelTasksDescriptor ParentTaskId(string parenttaskid) => this.Qs("parent_task_id", (object) parenttaskid);

    public CancelTasksDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);
  }
}
