// Decompiled with JetBrains decompiler
// Type: Nest.IClrTypeMapping`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public interface IClrTypeMapping<TDocument> : IClrTypeMapping where TDocument : class
  {
    Expression<Func<TDocument, object>> IdProperty { get; set; }

    IList<IClrPropertyMapping<TDocument>> Properties { get; set; }

    Expression<Func<TDocument, object>> RoutingProperty { get; set; }
  }
}
