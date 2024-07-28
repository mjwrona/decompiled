// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.GeospatialConfig
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class GeospatialConfig : JsonSerializable, ICloneable
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
