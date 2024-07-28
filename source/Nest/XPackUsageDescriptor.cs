// Decompiled with JetBrains decompiler
// Type: Nest.XPackUsageDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.XPackApi;

namespace Nest
{
  public class XPackUsageDescriptor : 
    RequestDescriptorBase<XPackUsageDescriptor, XPackUsageRequestParameters, IXPackUsageRequest>,
    IXPackUsageRequest,
    IRequest<XPackUsageRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.XPackUsage;

    public XPackUsageDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
