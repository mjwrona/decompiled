// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ByokConfig
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class ByokConfig : JsonSerializable
  {
    public ByokConfig() => this.ByokStatus = ByokStatus.None;

    public ByokConfig(ByokStatus byokStatus) => this.ByokStatus = byokStatus;

    [JsonProperty(PropertyName = "byokStatus")]
    [JsonConverter(typeof (StringEnumConverter))]
    public ByokStatus ByokStatus
    {
      get
      {
        ByokStatus byokStatus = ByokStatus.None;
        string str = this.GetValue<string>("byokStatus");
        if (!string.IsNullOrEmpty(str))
          byokStatus = (ByokStatus) Enum.Parse(typeof (ByokStatus), str, true);
        return byokStatus;
      }
      set => this.SetValue("byokStatus", (object) value.ToString());
    }
  }
}
