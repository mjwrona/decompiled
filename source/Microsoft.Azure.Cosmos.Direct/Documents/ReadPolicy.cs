// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ReadPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class ReadPolicy : JsonSerializable
  {
    private const int DefaultPrimaryReadCoefficient = 0;
    private const int DefaultSecondaryReadCoefficient = 1;

    [JsonProperty(PropertyName = "primaryReadCoefficient")]
    public int PrimaryReadCoefficient
    {
      get => this.GetValue<int>("primaryReadCoefficient", 0);
      set => this.SetValue("primaryReadCoefficient", (object) value);
    }

    [JsonProperty(PropertyName = "secondaryReadCoefficient")]
    public int SecondaryReadCoefficient
    {
      get => this.GetValue<int>("secondaryReadCoefficient", 1);
      set => this.SetValue("secondaryReadCoefficient", (object) value);
    }
  }
}
