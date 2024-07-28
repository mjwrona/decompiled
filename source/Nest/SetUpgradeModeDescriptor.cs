// Decompiled with JetBrains decompiler
// Type: Nest.SetUpgradeModeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;

namespace Nest
{
  public class SetUpgradeModeDescriptor : 
    RequestDescriptorBase<SetUpgradeModeDescriptor, SetUpgradeModeRequestParameters, ISetUpgradeModeRequest>,
    ISetUpgradeModeRequest,
    IRequest<SetUpgradeModeRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningSetUpgradeMode;

    public SetUpgradeModeDescriptor Enabled(bool? enabled = true) => this.Qs(nameof (enabled), (object) enabled);

    public SetUpgradeModeDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
