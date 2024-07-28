// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.KeyWrapMetadata
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal class KeyWrapMetadata : JsonSerializable
  {
    [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
    internal string Name
    {
      get => this.GetValue<string>("name");
      set => this.SetValue("name", (object) value);
    }

    [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
    internal string Type
    {
      get => this.GetValue<string>("type");
      set => this.SetValue("type", (object) value);
    }

    [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
    internal string Value
    {
      get => this.GetValue<string>("value");
      set => this.SetValue(nameof (value), (object) value);
    }
  }
}
