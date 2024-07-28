// Decompiled with JetBrains decompiler
// Type: Nest.GetLicenseRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.LicenseApi;

namespace Nest
{
  public class GetLicenseRequest : 
    PlainRequestBase<GetLicenseRequestParameters>,
    IGetLicenseRequest,
    IRequest<GetLicenseRequestParameters>,
    IRequest
  {
    protected IGetLicenseRequest Self => (IGetLicenseRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.LicenseGet;

    public bool? AcceptEnterprise
    {
      get => this.Q<bool?>("accept_enterprise");
      set => this.Q("accept_enterprise", (object) value);
    }

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }
  }
}
