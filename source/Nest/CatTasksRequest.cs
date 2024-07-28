// Decompiled with JetBrains decompiler
// Type: Nest.CatTasksRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatTasksRequest : 
    PlainRequestBase<CatTasksRequestParameters>,
    ICatTasksRequest,
    IRequest<CatTasksRequestParameters>,
    IRequest
  {
    protected ICatTasksRequest Self => (ICatTasksRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatTasks;

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

    public string Format
    {
      get => this.Q<string>("format");
      set
      {
        this.Q("format", (object) value);
        this.SetAcceptHeader(value);
      }
    }

    public string[] Headers
    {
      get => this.Q<string[]>("h");
      set => this.Q("h", (object) value);
    }

    public bool? Help
    {
      get => this.Q<bool?>("help");
      set => this.Q("help", (object) value);
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

    public string[] SortByColumns
    {
      get => this.Q<string[]>("s");
      set => this.Q("s", (object) value);
    }

    public bool? Verbose
    {
      get => this.Q<bool?>("v");
      set => this.Q("v", (object) value);
    }
  }
}
