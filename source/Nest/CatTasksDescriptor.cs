// Decompiled with JetBrains decompiler
// Type: Nest.CatTasksDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatTasksDescriptor : 
    RequestDescriptorBase<CatTasksDescriptor, CatTasksRequestParameters, ICatTasksRequest>,
    ICatTasksRequest,
    IRequest<CatTasksRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatTasks;

    public CatTasksDescriptor Actions(params string[] actions) => this.Qs(nameof (actions), (object) actions);

    public CatTasksDescriptor Detailed(bool? detailed = true) => this.Qs(nameof (detailed), (object) detailed);

    public CatTasksDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatTasksDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatTasksDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatTasksDescriptor Nodes(params string[] nodes) => this.Qs(nameof (nodes), (object) nodes);

    public CatTasksDescriptor ParentTaskId(string parenttaskid) => this.Qs("parent_task_id", (object) parenttaskid);

    public CatTasksDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatTasksDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
