// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ConvertThenDelegateToInputFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class ConvertThenDelegateToInputFactory
  {
    public static IFactory<TIn, TOut> ThenDelegateTo<TIn, TConverted, TOut>(
      this IConverter<TIn, TConverted> converter,
      IFactory<TConverted, TOut> factory)
      where TIn : class
      where TConverted : class
      where TOut : class
    {
      return ConvertThenDelegateToInputFactory.Create<TIn, TConverted, TOut>(converter, factory);
    }

    public static IFactory<TIn, TOut> Create<TIn, TConverted, TOut>(
      IConverter<TIn, TConverted> converter,
      IFactory<TConverted, TOut> factory)
      where TIn : class
      where TConverted : class
      where TOut : class
    {
      return (IFactory<TIn, TOut>) new ConvertThenDelegateToInputFactory.Impl<TIn, TConverted, TOut>(converter, factory);
    }

    private class Impl<TIn, TFirst, TOut> : IFactory<TIn, TOut>
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      private readonly IConverter<TIn, TFirst> first;
      private readonly IFactory<TFirst, TOut> second;

      public Impl(IConverter<TIn, TFirst> first, IFactory<TFirst, TOut> second)
      {
        this.first = first;
        this.second = second;
      }

      public TOut Get(TIn input) => this.second.Get(this.first.Convert(input));
    }
  }
}
