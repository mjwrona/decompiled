// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SpatialIndex
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  public sealed class SpatialIndex : Index, ICloneable
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
