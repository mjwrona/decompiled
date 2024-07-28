// Decompiled with JetBrains decompiler
// Type: Nest.CatHelpRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatHelpRequest : 
    PlainRequestBase<CatHelpRequestParameters>,
    ICatHelpRequest,
    IRequest<CatHelpRequestParameters>,
    IRequest
  {
    protected ICatHelpRequest Self => (ICatHelpRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatHelp;

    public bool? Help
    {
      get => this.Q<bool?>("help");
      set => this.Q("help", (object) value);
    }

    public string[] SortByColumns
    {
      get => this.Q<string[]>("s");
      set => this.Q("s", (object) value);
    }
  }
}
