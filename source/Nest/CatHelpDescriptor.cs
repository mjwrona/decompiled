// Decompiled with JetBrains decompiler
// Type: Nest.CatHelpDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatHelpDescriptor : 
    RequestDescriptorBase<CatHelpDescriptor, CatHelpRequestParameters, ICatHelpRequest>,
    ICatHelpRequest,
    IRequest<CatHelpRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatHelp;

    public CatHelpDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatHelpDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);
  }
}
