// Decompiled with JetBrains decompiler
// Type: Nest.ClrTypeMapping`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class ClrTypeMapping<TDocument> : 
    ClrTypeMapping,
    IClrTypeMapping<TDocument>,
    IClrTypeMapping
    where TDocument : class
  {
    public ClrTypeMapping()
      : base(typeof (TDocument))
    {
    }

    public Expression<Func<TDocument, object>> IdProperty { get; set; }

    public IList<IClrPropertyMapping<TDocument>> Properties { get; set; }

    public Expression<Func<TDocument, object>> RoutingProperty { get; set; }
  }
}
