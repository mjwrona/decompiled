// Decompiled with JetBrains decompiler
// Type: Nest.DenseVectorPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class DenseVectorPropertyDescriptor<T> : 
    PropertyDescriptorBase<DenseVectorPropertyDescriptor<T>, IDenseVectorProperty, T>,
    IDenseVectorProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public DenseVectorPropertyDescriptor()
      : base(FieldType.DenseVector)
    {
    }

    int? IDenseVectorProperty.Dimensions { get; set; }

    public DenseVectorPropertyDescriptor<T> Dimensions(int? dimensions) => this.Assign<int?>(dimensions, (Action<IDenseVectorProperty, int?>) ((a, v) => a.Dimensions = v));
  }
}
