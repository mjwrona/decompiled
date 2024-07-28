// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.HashIndex
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  public sealed class HashIndex : Index, ICloneable
  {
    internal HashIndex()
      : base(IndexKind.Hash)
    {
    }

    public HashIndex(DataType dataType)
      : this()
    {
      this.DataType = dataType;
    }

    public HashIndex(DataType dataType, short precision)
      : this(dataType)
    {
      this.Precision = new short?(precision);
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

    [JsonProperty(PropertyName = "precision", NullValueHandling = NullValueHandling.Ignore)]
    public short? Precision
    {
      get
      {
        short? precision = new short?();
        string str = this.GetValue<string>("precision");
        if (!string.IsNullOrEmpty(str))
          precision = new short?(Convert.ToInt16(str, (IFormatProvider) CultureInfo.InvariantCulture));
        return precision;
      }
      set => this.SetValue("precision", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      Helpers.ValidateEnumProperties<DataType>(this.DataType);
      short? precision = this.Precision;
    }

    public object Clone() => (object) new HashIndex(this.DataType)
    {
      Precision = this.Precision
    };
  }
}
