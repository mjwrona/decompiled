// Decompiled with JetBrains decompiler
// Type: Nest.GetLicenseDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.LicenseApi;

namespace Nest
{
  public class GetLicenseDescriptor : 
    RequestDescriptorBase<GetLicenseDescriptor, GetLicenseRequestParameters, IGetLicenseRequest>,
    IGetLicenseRequest,
    IRequest<GetLicenseRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.LicenseGet;

    public GetLicenseDescriptor AcceptEnterprise(bool? acceptenterprise = true) => this.Qs("accept_enterprise", (object) acceptenterprise);

    public GetLicenseDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);
  }
}
