// Decompiled with JetBrains decompiler
// Type: Nest.ObjectAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ObjectAttribute : 
    ElasticsearchCorePropertyAttributeBase,
    IObjectProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public ObjectAttribute()
      : base(FieldType.Object)
    {
    }

    protected ObjectAttribute(FieldType type)
      : base(type)
    {
    }

    public bool Enabled
    {
      get => this.Self.Enabled.GetValueOrDefault();
      set => this.Self.Enabled = new bool?(value);
    }

    Union<bool, DynamicMapping> IObjectProperty.Dynamic { get; set; }

    bool? IObjectProperty.Enabled { get; set; }

    IProperties IObjectProperty.Properties { get; set; }

    private IObjectProperty Self => (IObjectProperty) this;
  }
}
