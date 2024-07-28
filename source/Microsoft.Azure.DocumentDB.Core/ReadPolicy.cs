// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ReadPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
