// Decompiled with JetBrains decompiler
// Type: Nest.CatMasterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatMasterDescriptor : 
    RequestDescriptorBase<CatMasterDescriptor, CatMasterRequestParameters, ICatMasterRequest>,
    ICatMasterRequest,
    IRequest<CatMasterRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatMaster;

    public CatMasterDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatMasterDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatMasterDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatMasterDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatMasterDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatMasterDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatMasterDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
