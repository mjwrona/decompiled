// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BoundingBoxSpec
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  public sealed class BoundingBoxSpec : JsonSerializable, ICloneable
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
