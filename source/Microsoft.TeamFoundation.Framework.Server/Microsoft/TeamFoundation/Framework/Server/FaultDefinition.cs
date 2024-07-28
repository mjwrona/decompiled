// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [JsonObject(MemberSerialization.OptIn)]
  internal class FaultDefinition
  {
    public FaultDefinition()
    {
    }

    public FaultDefinition(
      string faultPoint,
      string faultType,
      FaultFilter filter,
      JObject faultSettings)
    {
      this.FaultPoint = faultPoint;
      this.FaultType = faultType;
      this.Filter = filter;
      this.Settings = faultSettings;
    }

    [JsonProperty]
    public string FaultPoint { get; set; }

    [JsonProperty]
    public string FaultType { get; set; }

    [JsonProperty]
    public JObject Settings { get; set; }

    [JsonProperty]
    public FaultFilter Filter { get; set; }

    public bool ShouldExecute() => this.Filter == null || this.Filter.ShouldExecute();

    public string Serialize() => JsonConvert.SerializeObject((object) this);
  }
}
