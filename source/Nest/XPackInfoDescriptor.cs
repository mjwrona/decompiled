// Decompiled with JetBrains decompiler
// Type: Nest.XPackInfoDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.XPackApi;

namespace Nest
{
  public class XPackInfoDescriptor : 
    RequestDescriptorBase<XPackInfoDescriptor, XPackInfoRequestParameters, IXPackInfoRequest>,
    IXPackInfoRequest,
    IRequest<XPackInfoRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.XPackInfo;

    public XPackInfoDescriptor AcceptEnterprise(bool? acceptenterprise = true) => this.Qs("accept_enterprise", (object) acceptenterprise);

    public XPackInfoDescriptor Categories(params string[] categories) => this.Qs(nameof (categories), (object) categories);
  }
}
