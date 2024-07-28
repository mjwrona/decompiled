// Decompiled with JetBrains decompiler
// Type: Nest.CatPendingTasksDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatPendingTasksDescriptor : 
    RequestDescriptorBase<CatPendingTasksDescriptor, CatPendingTasksRequestParameters, ICatPendingTasksRequest>,
    ICatPendingTasksRequest,
    IRequest<CatPendingTasksRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatPendingTasks;

    public CatPendingTasksDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatPendingTasksDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatPendingTasksDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatPendingTasksDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatPendingTasksDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatPendingTasksDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatPendingTasksDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
