// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.EnumSchema
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  public sealed class EnumSchema
  {
    private List<EnumValue> values;

    public EnumSchema()
    {
      this.Type = TypeKind.Int32;
      this.values = new List<EnumValue>();
    }

    [JsonProperty(PropertyName = "comment", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Comment { get; set; }

    [JsonProperty(PropertyName = "name", Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "apitype", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string ApiType { get; set; }

    [DefaultValue(TypeKind.Int32)]
    [JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public TypeKind Type { get; set; }

    [JsonProperty(PropertyName = "values")]
    public List<EnumValue> Values
    {
      get => this.values;
      set => this.values = value ?? new List<EnumValue>();
    }
  }
}
