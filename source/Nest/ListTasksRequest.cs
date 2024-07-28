// Decompiled with JetBrains decompiler
// Type: Nest.ListTasksRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.TasksApi;

namespace Nest
{
  public class ListTasksRequest : 
    PlainRequestBase<ListTasksRequestParameters>,
    IListTasksRequest,
    IRequest<ListTasksRequestParameters>,
    IRequest
  {
    protected IListTasksRequest Self => (IListTasksRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TasksList;

    public string[] Actions
    {
      get => this.Q<string[]>("actions");
      set => this.Q("actions", (object) value);
    }

    public bool? Detailed
    {
      get => this.Q<bool?>("detailed");
      set => this.Q("detailed", (object) value);
    }

    public Elasticsearch.Net.GroupBy? GroupBy
    {
      get => this.Q<Elasticsearch.Net.GroupBy?>("group_by");
      set => this.Q("group_by", (object) value);
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
