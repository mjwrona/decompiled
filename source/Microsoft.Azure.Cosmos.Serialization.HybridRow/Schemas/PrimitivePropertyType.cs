// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.PrimitivePropertyType
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  public sealed class PrimitivePropertyType : PropertyType
  {
    public PrimitivePropertyType()
      : base(TypeKind.Invalid)
    {
    }

    public PrimitivePropertyType(TypeKind type)
      : base(type)
    {
    }

    [JsonProperty(PropertyName = "length")]
    [JsonConverter(typeof (StrictIntegerConverter))]
    public int Length { get; set; }

    [JsonProperty(PropertyName = "storage")]
    public StorageKind Storage { get; set; }

    [JsonProperty(PropertyName = "enum", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Enum { get; set; }

    [JsonProperty(PropertyName = "rowBufferSize", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [JsonConverter(typeof (StrictBooleanConverter))]
    public bool RowBufferSize { get; set; }
  }
}
