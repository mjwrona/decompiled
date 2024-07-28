// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CompositePath
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  public sealed class CompositePath : JsonSerializable, ICloneable
  {
    [JsonProperty(PropertyName = "path")]
    public string Path
    {
      get => this.GetValue<string>("path");
      set => this.SetValue("path", (object) value);
    }

    [JsonProperty(PropertyName = "order")]
    [JsonConverter(typeof (StringEnumConverter))]
    public CompositePathSortOrder Order
    {
      get
      {
        CompositePathSortOrder order = CompositePathSortOrder.Ascending;
        string str = this.GetValue<string>("order");
        if (!string.IsNullOrEmpty(str))
          order = (CompositePathSortOrder) Enum.Parse(typeof (CompositePathSortOrder), str, true);
        return order;
      }
      set => this.SetValue("order", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<string>("path");
      Helpers.ValidateEnumProperties<CompositePathSortOrder>(this.Order);
    }

    public object Clone() => (object) new CompositePath()
    {
      Path = this.Path,
      Order = this.Order
    };
  }
}
