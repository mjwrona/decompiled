// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Trigger
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Documents
{
  public class Trigger : Resource
  {
    [JsonProperty(PropertyName = "body")]
    public string Body
    {
      get => this.GetValue<string>("body");
      set => this.SetValue("body", (object) value);
    }

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "triggerType")]
    public TriggerType TriggerType
    {
      get => this.GetValue<TriggerType>("triggerType", TriggerType.Pre);
      set => this.SetValue("triggerType", (object) value.ToString());
    }

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "triggerOperation")]
    public TriggerOperation TriggerOperation
    {
      get => this.GetValue<TriggerOperation>("triggerOperation", TriggerOperation.All);
      set => this.SetValue("triggerOperation", (object) value.ToString());
    }
  }
}
