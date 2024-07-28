// Decompiled with JetBrains decompiler
// Type: Nest.PerFieldAnalyzerDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class PerFieldAnalyzerDescriptor<T> : 
    IsADictionaryDescriptorBase<PerFieldAnalyzerDescriptor<T>, IPerFieldAnalyzer, Nest.Field, string>
    where T : class
  {
    public PerFieldAnalyzerDescriptor()
      : base((IPerFieldAnalyzer) new PerFieldAnalyzer())
    {
    }

    public PerFieldAnalyzerDescriptor<T> Field(Nest.Field field, string analyzer) => this.Assign(field, analyzer);

    public PerFieldAnalyzerDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> field,
      string analyzer)
    {
      return this.Assign((Nest.Field) (Expression) field, analyzer);
    }
  }
}
