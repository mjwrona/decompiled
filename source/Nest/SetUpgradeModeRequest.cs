// Decompiled with JetBrains decompiler
// Type: Nest.SetUpgradeModeRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;

namespace Nest
{
  public class SetUpgradeModeRequest : 
    PlainRequestBase<SetUpgradeModeRequestParameters>,
    ISetUpgradeModeRequest,
    IRequest<SetUpgradeModeRequestParameters>,
    IRequest
  {
    protected ISetUpgradeModeRequest Self => (ISetUpgradeModeRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningSetUpgradeMode;

    public bool? Enabled
    {
      get => this.Q<bool?>("enabled");
      set => this.Q("enabled", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
