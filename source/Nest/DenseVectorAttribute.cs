// Decompiled with JetBrains decompiler
// Type: Nest.DenseVectorAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class DenseVectorAttribute : 
    ElasticsearchPropertyAttributeBase,
    IDenseVectorProperty,
    IProperty,
    IFieldMapping
  {
    public DenseVectorAttribute()
      : base(FieldType.DenseVector)
    {
    }

    public int Dimensions
    {
      get => this.Self.Dimensions.GetValueOrDefault();
      set => this.Self.Dimensions = new int?(value);
    }

    int? IDenseVectorProperty.Dimensions { get; set; }

    private IDenseVectorProperty Self => (IDenseVectorProperty) this;
  }
}
