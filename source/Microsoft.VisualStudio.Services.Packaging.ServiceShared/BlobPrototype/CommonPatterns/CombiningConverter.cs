// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns.CombiningConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns
{
  public static class CombiningConverter
  {
    public static IConverter<TIn, TOut> Create<TIn, TInner, TOut>(
      IConverter<TIn, TInner> first,
      IConverter<TInner, TOut> second)
    {
      return (IConverter<TIn, TOut>) new CombiningConverter.Impl<TIn, TInner, TOut>(first, second);
    }

    public static IConverter<TIn, TOut> ThenConvertBy<TIn, TInner, TOut>(
      this IConverter<TIn, TInner> first,
      IConverter<TInner, TOut> second)
    {
      return CombiningConverter.Create<TIn, TInner, TOut>(first, second);
    }

    private class Impl<TIn, TInner, TOut> : 
      IConverter<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IConverter<TIn, TInner> first;
      private readonly IConverter<TInner, TOut> second;

      public Impl(IConverter<TIn, TInner> first, IConverter<TInner, TOut> second)
      {
        this.first = first;
        this.second = second;
      }

      public TOut Convert(TIn input) => this.second.Convert(this.first.Convert(input));
    }
  }
}
