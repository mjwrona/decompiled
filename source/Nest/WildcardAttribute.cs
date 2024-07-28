// Decompiled with JetBrains decompiler
// Type: Nest.WildcardAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class WildcardAttribute : 
    ElasticsearchPropertyAttributeBase,
    IWildcardProperty,
    IProperty,
    IFieldMapping
  {
    public WildcardAttribute()
      : base(FieldType.Wildcard)
    {
    }

    int? IWildcardProperty.IgnoreAbove { get; set; }

    private IWildcardProperty Self => (IWildcardProperty) this;

    public int IgnoreAbove
    {
      get => this.Self.IgnoreAbove.GetValueOrDefault(int.MaxValue);
      set => this.Self.IgnoreAbove = new int?(value);
    }

    public string NullValue { get; set; }
  }
}
