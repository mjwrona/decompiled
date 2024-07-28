// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.KeyWrapMetadata
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal class KeyWrapMetadata : JsonSerializable
  {
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
