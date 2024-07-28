// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.Property
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  public sealed class Property
  {
    [JsonProperty(PropertyName = "comment", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Comment { get; set; }

    [JsonProperty(PropertyName = "path", Required = Required.Always)]
    public string Path { get; set; }

    [JsonProperty(PropertyName = "apiname", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string ApiName { get; set; }

    [JsonProperty(PropertyName = "type", Required = Required.Always)]
    public PropertyType PropertyType { get; set; }

    [JsonProperty(PropertyName = "allowEmpty", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public AllowEmptyKind AllowEmpty { get; set; }
  }
}
