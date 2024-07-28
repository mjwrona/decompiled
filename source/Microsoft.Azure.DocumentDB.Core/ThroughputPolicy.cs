// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ThroughputPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class ThroughputPolicy : JsonSerializable
  {
    public ThroughputPolicy()
    {
    }

    public ThroughputPolicy(bool isEnabled, int incrementPercent)
    {
      this.IsEnabled = isEnabled;
      this.IncrementPercent = incrementPercent;
    }

    [JsonProperty(PropertyName = "isEnabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool IsEnabled
    {
      get => this.GetValue<bool>("isEnabled");
      set => this.SetValue("isEnabled", (object) value);
    }

    [JsonProperty(PropertyName = "incrementPercent", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int IncrementPercent
    {
      get => this.GetValue<int>("incrementPercent");
      set => this.SetValue("incrementPercent", (object) value);
    }

    public static bool Equals(
      ThroughputPolicy throughputPolicy1,
      ThroughputPolicy throughputPolicy2)
    {
      if (throughputPolicy1 == null && throughputPolicy2 == null)
        return true;
      return throughputPolicy1 != null && throughputPolicy2 != null && throughputPolicy1.IsEnabled == throughputPolicy2.IsEnabled && throughputPolicy1.IncrementPercent == throughputPolicy2.IncrementPercent;
    }
  }
}
