// Decompiled with JetBrains decompiler
// Type: Nest.IgnoreClrPropertyMapping`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class IgnoreClrPropertyMapping<TDocument> : ClrPropertyMappingBase<TDocument> where TDocument : class
  {
    public IgnoreClrPropertyMapping(Expression<Func<TDocument, object>> property)
      : base(property)
    {
      this.Self.Ignore = true;
    }
  }
}
