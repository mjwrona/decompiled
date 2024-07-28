// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GeospatialConfig
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  public sealed class GeospatialConfig : JsonSerializable, ICloneable
  {
    public GeospatialConfig() => this.GeospatialType = GeospatialType.Geography;

    public GeospatialConfig(GeospatialType geospatialType) => this.GeospatialType = geospatialType;

    [JsonProperty(PropertyName = "type")]
    [JsonConverter(typeof (StringEnumConverter))]
    public GeospatialType GeospatialType
    {
      get
      {
        GeospatialType geospatialType = GeospatialType.Geography;
        string str = this.GetValue<string>("type");
        if (!string.IsNullOrEmpty(str))
          geospatialType = (GeospatialType) Enum.Parse(typeof (GeospatialType), str, true);
        return geospatialType;
      }
      set => this.SetValue("type", (object) value.ToString());
    }

    public object Clone() => (object) new GeospatialConfig()
    {
      GeospatialType = this.GeospatialType
    };

    internal override void Validate()
    {
      base.Validate();
      Helpers.ValidateEnumProperties<GeospatialType>(this.GeospatialType);
    }
  }
}
