// Decompiled with JetBrains decompiler
// Type: Nest.XPackInfoRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.XPackApi;

namespace Nest
{
  public class XPackInfoRequest : 
    PlainRequestBase<XPackInfoRequestParameters>,
    IXPackInfoRequest,
    IRequest<XPackInfoRequestParameters>,
    IRequest
  {
    protected IXPackInfoRequest Self => (IXPackInfoRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.XPackInfo;

    public bool? AcceptEnterprise
    {
      get => this.Q<bool?>("accept_enterprise");
      set => this.Q("accept_enterprise", (object) value);
    }

    public string[] Categories
    {
      get => this.Q<string[]>("categories");
      set => this.Q("categories", (object) value);
    }
  }
}
