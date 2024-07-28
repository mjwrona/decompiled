// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexingPath
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class IndexingPath : JsonSerializable, ICloneable
  {
    public IndexingPath() => this.IndexType = IndexType.Hash;

    [JsonProperty(PropertyName = "path")]
    public string Path
    {
      get => this.GetValue<string>("path");
      set => this.SetValue("path", (object) value);
    }

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "IndexType")]
    public IndexType IndexType
    {
      get
      {
        IndexType result = IndexType.Hash;
        string str = this.GetValue<string>(nameof (IndexType));
        if (!string.IsNullOrEmpty(str))
          Enum.TryParse<IndexType>(str, true, out result);
        return result;
      }
      set => this.SetValue(nameof (IndexType), (object) value.ToString());
    }

    [JsonProperty(PropertyName = "NumericPrecision")]
    public int? NumericPrecision
    {
      get
      {
        int? numericPrecision = new int?();
        string str = this.GetValue<string>(nameof (NumericPrecision));
        if (!string.IsNullOrEmpty(str))
          numericPrecision = new int?(Convert.ToInt32(str, (IFormatProvider) CultureInfo.InvariantCulture));
        return numericPrecision;
      }
      set => this.SetValue(nameof (NumericPrecision), (object) value);
    }

    [JsonProperty(PropertyName = "StringPrecision")]
    public int? StringPrecision
    {
      get
      {
        int? stringPrecision = new int?();
        string str = this.GetValue<string>(nameof (StringPrecision));
        if (!string.IsNullOrEmpty(str))
          stringPrecision = new int?(Convert.ToInt32(str, (IFormatProvider) CultureInfo.InvariantCulture));
        return stringPrecision;
      }
      set => this.SetValue(nameof (StringPrecision), (object) value);
    }

    public object Clone() => (object) new IndexingPath()
    {
      IndexType = this.IndexType,
      NumericPrecision = this.NumericPrecision,
      StringPrecision = this.StringPrecision,
      Path = this.Path
    };
  }
}
