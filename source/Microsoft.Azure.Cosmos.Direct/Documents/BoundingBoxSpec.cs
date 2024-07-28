// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BoundingBoxSpec
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class BoundingBoxSpec : JsonSerializable, ICloneable
  {
    [JsonProperty(PropertyName = "xmin")]
    public double Xmin
    {
      get => this.GetValue<double>("xmin");
      set => this.SetValue("xmin", (object) value);
    }

    [JsonProperty(PropertyName = "ymin")]
    public double Ymin
    {
      get => this.GetValue<double>("ymin");
      set => this.SetValue("ymin", (object) value);
    }

    [JsonProperty(PropertyName = "xmax")]
    public double Xmax
    {
      get => this.GetValue<double>("xmax");
      set => this.SetValue("xmax", (object) value);
    }

    [JsonProperty(PropertyName = "ymax")]
    public double Ymax
    {
      get => this.GetValue<double>("ymax");
      set => this.SetValue("ymax", (object) value);
    }

    public object Clone() => (object) new BoundingBoxSpec()
    {
      Xmin = this.Xmin,
      Ymin = this.Ymin,
      Xmax = this.Xmax,
      Ymax = this.Ymax
    };

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<double>("xmin");
      this.GetValue<double>("ymin");
      this.GetValue<double>("xmax");
      this.GetValue<double>("ymax");
    }
  }
}
