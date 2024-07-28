// Decompiled with JetBrains decompiler
// Type: Nest.IpAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IpAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IIpProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public IpAttribute()
      : base(FieldType.Ip)
    {
    }

    [Obsolete("The server always treated this as a noop and has been removed in 7.10")]
    public double Boost
    {
      get => this.Self.Boost.GetValueOrDefault();
      set => this.Self.Boost = new double?(value);
    }

    public bool Index
    {
      get => this.Self.Index.GetValueOrDefault();
      set => this.Self.Index = new bool?(value);
    }

    public string NullValue
    {
      get => this.Self.NullValue;
      set => this.Self.NullValue = value;
    }

    public bool IgnoreMalformed
    {
      get => this.Self.IgnoreMalformed.GetValueOrDefault();
      set => this.Self.IgnoreMalformed = new bool?(value);
    }

    double? IIpProperty.Boost { get; set; }

    bool? IIpProperty.Index { get; set; }

    string IIpProperty.NullValue { get; set; }

    private IIpProperty Self => (IIpProperty) this;

    bool? IIpProperty.IgnoreMalformed { get; set; }

    IInlineScript IIpProperty.Script { get; set; }

    OnScriptError? IIpProperty.OnScriptError { get; set; }
  }
}
