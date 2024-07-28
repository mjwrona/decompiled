// Decompiled with JetBrains decompiler
// Type: Nest.NestedAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class NestedAttribute : 
    ObjectAttribute,
    INestedProperty,
    IObjectProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public NestedAttribute()
      : base(FieldType.Nested)
    {
    }

    public bool IncludeInParent
    {
      get => this.Self.IncludeInParent.GetValueOrDefault();
      set => this.Self.IncludeInParent = new bool?(value);
    }

    public bool IncludeInRoot
    {
      get => this.Self.IncludeInRoot.GetValueOrDefault();
      set => this.Self.IncludeInRoot = new bool?(value);
    }

    bool? INestedProperty.IncludeInParent { get; set; }

    bool? INestedProperty.IncludeInRoot { get; set; }

    private INestedProperty Self => (INestedProperty) this;
  }
}
