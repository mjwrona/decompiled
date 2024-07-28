// Decompiled with JetBrains decompiler
// Type: Nest.StartTrialLicenseRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.LicenseApi;

namespace Nest
{
  public class StartTrialLicenseRequest : 
    PlainRequestBase<StartTrialLicenseRequestParameters>,
    IStartTrialLicenseRequest,
    IRequest<StartTrialLicenseRequestParameters>,
    IRequest
  {
    protected IStartTrialLicenseRequest Self => (IStartTrialLicenseRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.LicenseStartTrial;

    public bool? Acknowledge
    {
      get => this.Q<bool?>("acknowledge");
      set => this.Q("acknowledge", (object) value);
    }

    public string TypeQueryString
    {
      get => this.Q<string>("type");
      set => this.Q("type", (object) value);
    }
  }
}
