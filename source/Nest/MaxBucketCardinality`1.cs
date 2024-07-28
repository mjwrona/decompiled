// Decompiled with JetBrains decompiler
// Type: Nest.MaxBucketCardinality`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class MaxBucketCardinality<T> : MaxBucketCardinality where T : class
  {
    public void Add<TValue>(Expression<Func<T, TValue>> field, long cardinality) => this.BackingDictionary.Add((Field) (Expression) field, cardinality);
  }
}
