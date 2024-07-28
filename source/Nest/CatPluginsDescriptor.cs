// Decompiled with JetBrains decompiler
// Type: Nest.CatPluginsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatPluginsDescriptor : 
    RequestDescriptorBase<CatPluginsDescriptor, CatPluginsRequestParameters, ICatPluginsRequest>,
    ICatPluginsRequest,
    IRequest<CatPluginsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatPlugins;

    public CatPluginsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatPluginsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatPluginsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatPluginsDescriptor IncludeBootstrap(bool? includebootstrap = true) => this.Qs("include_bootstrap", (object) includebootstrap);

    public CatPluginsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatPluginsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatPluginsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatPluginsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
