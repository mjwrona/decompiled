// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AutoUpgradePolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class AutoUpgradePolicy : JsonSerializable
  {
    public AutoUpgradePolicy()
    {
    }

    public AutoUpgradePolicy(ThroughputPolicy throughputPolicy) => this.ThroughputPolicy = throughputPolicy;

    [JsonProperty(PropertyName = "throughputPolicy", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public ThroughputPolicy ThroughputPolicy
    {
      get => this.GetValue<ThroughputPolicy>("throughputPolicy");
      set => this.SetValue("throughputPolicy", (object) value);
    }

    public static bool Equals(
      AutoUpgradePolicy autoUpgradePolicy1,
      AutoUpgradePolicy autoUpgradePolicy2)
    {
      if (autoUpgradePolicy1 == null && autoUpgradePolicy2 == null)
        return true;
      return autoUpgradePolicy1 != null && autoUpgradePolicy2 != null && ThroughputPolicy.Equals(autoUpgradePolicy1.ThroughputPolicy, autoUpgradePolicy2.ThroughputPolicy);
    }
  }
}
