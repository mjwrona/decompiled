// Decompiled with JetBrains decompiler
// Type: Nest.ScopeDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class ScopeDescriptor<T> : 
    DescriptorPromiseBase<ScopeDescriptor<T>, IReadOnlyDictionary<Field, FilterRef>>
    where T : class
  {
    public ScopeDescriptor()
      : base((IReadOnlyDictionary<Field, FilterRef>) new System.Collections.Generic.Dictionary<Field, FilterRef>())
    {
    }

    private System.Collections.Generic.Dictionary<Field, FilterRef> Dictionary => (System.Collections.Generic.Dictionary<Field, FilterRef>) this.PromisedValue;

    public ScopeDescriptor<T> Scope(Field field, FilterRef filterRef)
    {
      this.Dictionary[field] = filterRef;
      return this;
    }

    public ScopeDescriptor<T> Scope(Expression<Func<T, object>> field, FilterRef filterRef)
    {
      this.Dictionary[(Field) (Expression) field] = filterRef;
      return this;
    }
  }
}
