// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.EnumerableConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class EnumerableConverter<TIn, TOut> : 
    IConverter<IEnumerable<TIn>, IEnumerable<TOut>>,
    IHaveInputType<IEnumerable<TIn>>,
    IHaveOutputType<IEnumerable<TOut>>
  {
    private readonly IConverter<TIn, TOut> converter;

    public EnumerableConverter(IConverter<TIn, TOut> converter) => this.converter = converter;

    public IEnumerable<TOut> Convert(IEnumerable<TIn> input) => (IEnumerable<TOut>) input.Select<TIn, TOut>((Func<TIn, TOut>) (item => this.converter.Convert(item))).ToList<TOut>();
  }
}
