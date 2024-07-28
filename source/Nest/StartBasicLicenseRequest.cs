// Decompiled with JetBrains decompiler
// Type: Nest.StartBasicLicenseRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.LicenseApi;

namespace Nest
{
  public class StartBasicLicenseRequest : 
    PlainRequestBase<StartBasicLicenseRequestParameters>,
    IStartBasicLicenseRequest,
    IRequest<StartBasicLicenseRequestParameters>,
    IRequest
  {
    protected IStartBasicLicenseRequest Self => (IStartBasicLicenseRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.LicenseStartBasic;

    public bool? Acknowledge
    {
      get => this.Q<bool?>("acknowledge");
      set => this.Q("acknowledge", (object) value);
    }
  }
}
