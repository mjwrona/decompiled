// Decompiled with JetBrains decompiler
// Type: Nest.ListTasksDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.TasksApi;

namespace Nest
{
  public class ListTasksDescriptor : 
    RequestDescriptorBase<ListTasksDescriptor, ListTasksRequestParameters, IListTasksRequest>,
    IListTasksRequest,
    IRequest<ListTasksRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.TasksList;

    public ListTasksDescriptor Actions(params string[] actions) => this.Qs(nameof (actions), (object) actions);

    public ListTasksDescriptor Detailed(bool? detailed = true) => this.Qs(nameof (detailed), (object) detailed);

    public ListTasksDescriptor GroupBy(Elasticsearch.Net.GroupBy? groupby) => this.Qs("group_by", (object) groupby);

    public ListTasksDescriptor Nodes(params string[] nodes) => this.Qs(nameof (nodes), (object) nodes);

    public ListTasksDescriptor ParentTaskId(string parenttaskid) => this.Qs("parent_task_id", (object) parenttaskid);

    public ListTasksDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public ListTasksDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);
  }
}
