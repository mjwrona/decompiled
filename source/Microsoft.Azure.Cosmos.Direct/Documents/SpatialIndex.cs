// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SpatialIndex
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class SpatialIndex : Index, ICloneable
  {
    internal SpatialIndex()
      : base(IndexKind.Spatial)
    {
    }

    public SpatialIndex(DataType dataType)
      : this()
    {
      this.DataType = dataType;
    }

    [JsonProperty(PropertyName = "dataType")]
    [JsonConverter(typeof (StringEnumConverter))]
    public DataType DataType
    {
      get
      {
        DataType dataType = DataType.Number;
        string str = this.GetValue<string>("dataType");
        if (!string.IsNullOrEmpty(str))
          dataType = (DataType) Enum.Parse(typeof (DataType), str, true);
        return dataType;
      }
      set => this.SetValue("dataType", (object) value.ToString());
    }

    internal override void Validate()
    {
      base.Validate();
      Helpers.ValidateEnumProperties<DataType>(this.DataType);
    }

    public object Clone() => (object) new SpatialIndex(this.DataType);
  }
}
