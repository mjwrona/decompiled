// Decompiled with JetBrains decompiler
// Type: Nest.OverallCardinalityDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class OverallCardinalityDescriptor<T> : 
    IsADictionaryDescriptorBase<OverallCardinalityDescriptor<T>, IOverallCardinality, Nest.Field, long>
    where T : class
  {
    public OverallCardinalityDescriptor()
      : base((IOverallCardinality) new OverallCardinality())
    {
    }

    public OverallCardinalityDescriptor<T> Field(Nest.Field field, long cardinality) => this.Assign(field, cardinality);

    public OverallCardinalityDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> field,
      long cardinality)
    {
      return this.Assign((Nest.Field) (Expression) field, cardinality);
    }
  }
}
