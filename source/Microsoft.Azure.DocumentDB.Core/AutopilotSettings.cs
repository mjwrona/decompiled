// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AutopilotSettings
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class AutopilotSettings : JsonSerializable
  {
    public AutopilotSettings()
    {
      this.Tier = 0;
      this.AutoUpgrade = false;
    }

    public AutopilotSettings(int tier, bool autoUpgrade)
    {
      this.Tier = tier;
      this.AutoUpgrade = autoUpgrade;
    }

    public AutopilotSettings(AutopilotSettings settings)
    {
      this.Tier = settings.Tier;
      this.AutoUpgrade = settings.AutoUpgrade;
      this.MaxThroughput = settings.MaxThroughput;
      this.AutoUpgradePolicy = settings.AutoUpgradePolicy;
    }

    public AutopilotSettings(int autopilotMaxThroughput, AutoUpgradePolicy autoUpgradePolicy)
    {
      this.MaxThroughput = autopilotMaxThroughput;
      this.AutoUpgradePolicy = autoUpgradePolicy;
    }

    public AutopilotSettings(int autopilotMaxThroughput) => this.MaxThroughput = autopilotMaxThroughput;

    [JsonProperty(PropertyName = "tier", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int Tier
    {
      get => this.GetValue<int>("tier");
      set => this.SetValue("tier", (object) value);
    }

    [JsonProperty(PropertyName = "targetTier", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal int? TargetTier => this.GetValue<int?>("targetTier");

    [JsonProperty(PropertyName = "maximumTierThroughput", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal int MaximumTierThroughput => this.GetValue<int>("maximumTierThroughput");

    [JsonProperty(PropertyName = "autoUpgrade", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool AutoUpgrade
    {
      get => this.GetValue<bool>("autoUpgrade");
      set => this.SetValue("autoUpgrade", (object) value);
    }

    [JsonProperty(PropertyName = "maxThroughput", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int MaxThroughput
    {
      get => this.GetValue<int>("maxThroughput");
      set => this.SetValue("maxThroughput", (object) value);
    }

    [JsonProperty(PropertyName = "targetMaxThroughput", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? TargetMaxThroughput => this.GetValue<int?>("targetMaxThroughput");

    [JsonProperty(PropertyName = "autoUpgradePolicy", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public AutoUpgradePolicy AutoUpgradePolicy
    {
      get => this.GetValue<AutoUpgradePolicy>("autoUpgradePolicy");
      set => this.SetValue("autoUpgradePolicy", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<int>("tier");
      this.GetValue<bool>("autoUpgrade");
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      this.SaveTo(stringBuilder);
      return stringBuilder.ToString();
    }

    public bool Equals(AutopilotSettings settings) => AutopilotSettings.Equals(this, settings);

    public static bool Equals(AutopilotSettings s1, AutopilotSettings s2)
    {
      if (s1 == null && s2 == null)
        return true;
      return s1 != null && s2 != null && s1.Tier == s2.Tier && s1.AutoUpgrade == s2.AutoUpgrade && s1.MaxThroughput == s2.MaxThroughput && AutoUpgradePolicy.Equals(s1.AutoUpgradePolicy, s2.AutoUpgradePolicy);
    }
  }
}
