// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.RangeIndex
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class RangeIndex : Index, ICloneable
  {
    internal RangeIndex()
      : base(IndexKind.Range)
    {
    }

    public RangeIndex(DataType dataType)
      : this()
    {
      this.DataType = dataType;
    }

    public RangeIndex(DataType dataType, short precision)
      : this(dataType)
    {
      this.Precision = new short?(precision);
    }

    [JsonProperty(PropertyName = "dataType")]
    [JsonConverter(typeof (StringEnumConverter))]
    public DataType DataType { get; set; }

    [JsonProperty(PropertyName = "precision", NullValueHandling = NullValueHandling.Ignore)]
    public short? Precision { get; set; }

    public object Clone() => (object) new RangeIndex(this.DataType)
    {
      Precision = this.Precision
    };
  }
}
