// Decompiled with JetBrains decompiler
// Type: Nest.CatNodeAttributesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatNodeAttributesDescriptor : 
    RequestDescriptorBase<CatNodeAttributesDescriptor, CatNodeAttributesRequestParameters, ICatNodeAttributesRequest>,
    ICatNodeAttributesRequest,
    IRequest<CatNodeAttributesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatNodeAttributes;

    public CatNodeAttributesDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatNodeAttributesDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatNodeAttributesDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatNodeAttributesDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatNodeAttributesDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatNodeAttributesDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatNodeAttributesDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
