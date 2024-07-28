// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.EnumerableConverterExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class EnumerableConverterExtensions
  {
    public static IEnumerable<TOut> ConvertMultiple<TIn, TOut>(
      this IConverter<TIn, TOut> converter,
      IEnumerable<TIn> input)
    {
      return new EnumerableConverter<TIn, TOut>(converter).Convert(input);
    }

    public static IConverter<IEnumerable<TIn>, IEnumerable<TOut>> OverEnumerables<TIn, TOut>(
      this IConverter<TIn, TOut> converter)
    {
      return (IConverter<IEnumerable<TIn>, IEnumerable<TOut>>) new EnumerableConverter<TIn, TOut>(converter);
    }
  }
}
