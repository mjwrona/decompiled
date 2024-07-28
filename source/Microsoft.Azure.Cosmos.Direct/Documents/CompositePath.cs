// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CompositePath
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class CompositePath : JsonSerializable, ICloneable
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
