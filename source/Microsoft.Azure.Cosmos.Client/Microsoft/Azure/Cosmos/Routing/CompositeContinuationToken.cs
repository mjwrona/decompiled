// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.CompositeContinuationToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class CompositeContinuationToken
  {
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("range")]
    [JsonConverter(typeof (RangeJsonConverter))]
    public Microsoft.Azure.Documents.Routing.Range<string> Range { get; set; }

    [JsonIgnore]
    public Microsoft.Azure.Documents.Routing.Range<string> PartitionRange => this.Range;

    public object ShallowCopy() => this.MemberwiseClone();

    private static class PropertyNames
    {
      public const string Token = "token";
      public const string Range = "range";
      public const string Min = "min";
      public const string Max = "max";
    }
  }
}
