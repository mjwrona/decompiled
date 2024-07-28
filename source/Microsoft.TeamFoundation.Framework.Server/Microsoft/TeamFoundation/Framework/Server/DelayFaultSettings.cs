// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DelayFaultSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [JsonObject(MemberSerialization.OptIn)]
  internal class DelayFaultSettings : ITargetedFaultSettings, IFaultSettings
  {
    public DelayFaultSettings()
    {
    }

    internal DelayFaultSettings(string target, int delayMs)
    {
      this.DelayMs = delayMs;
      this.Target = target;
    }

    [JsonProperty]
    public string Target { get; set; }

    [JsonProperty]
    public int DelayMs { get; set; }

    [JsonProperty]
    public double Probability { get; set; } = 1.0;
  }
}
