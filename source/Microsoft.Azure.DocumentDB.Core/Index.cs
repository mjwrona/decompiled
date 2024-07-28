// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Index
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  [JsonConverter(typeof (IndexJsonConverter))]
  public abstract class Index : JsonSerializable
  {
    protected Index(IndexKind kind) => this.Kind = kind;

    [JsonProperty(PropertyName = "kind")]
    [JsonConverter(typeof (StringEnumConverter))]
    public IndexKind Kind
    {
      get
      {
        IndexKind kind = IndexKind.Hash;
        string str = this.GetValue<string>("kind");
        if (!string.IsNullOrEmpty(str))
          kind = (IndexKind) Enum.Parse(typeof (IndexKind), str, true);
        return kind;
      }
      private set => this.SetValue("kind", (object) value.ToString());
    }

    public static RangeIndex Range(DataType dataType) => new RangeIndex(dataType);

    public static RangeIndex Range(DataType dataType, short precision) => new RangeIndex(dataType, precision);

    public static HashIndex Hash(DataType dataType) => new HashIndex(dataType);

    public static HashIndex Hash(DataType dataType, short precision) => new HashIndex(dataType, precision);

    public static SpatialIndex Spatial(DataType dataType) => new SpatialIndex(dataType);

    internal override void Validate()
    {
      base.Validate();
      Helpers.ValidateEnumProperties<IndexKind>(this.Kind);
    }
  }
}
