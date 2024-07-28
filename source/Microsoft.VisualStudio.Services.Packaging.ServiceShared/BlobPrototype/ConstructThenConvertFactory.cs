// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ConstructThenConvertFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class ConstructThenConvertFactory
  {
    public static IFactory<TOut> ConvertBy<TFirst, TOut>(
      this IFactory<TFirst> factory,
      Func<TFirst, TOut> converter)
    {
      return factory.ConvertBy<TFirst, TOut>((IConverter<TFirst, TOut>) new ByFuncConverter<TFirst, TOut>(converter));
    }

    public static IFactory<TIn, TOut> ConvertBy<TIn, TFirst, TOut>(
      this IFactory<TIn, TFirst> factory,
      Func<TFirst, TOut> converter)
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      return factory.ConvertBy<TIn, TFirst, TOut>((IConverter<TFirst, TOut>) new ByFuncConverter<TFirst, TOut>(converter));
    }

    public static IFactory<TOut> Create<TFirst, TOut>(
      IFactory<TFirst> factory,
      Func<TFirst, TOut> converter)
      where TFirst : class
      where TOut : class
    {
      return ConstructThenConvertFactory.Create<TFirst, TOut>(factory, (IConverter<TFirst, TOut>) new ByFuncConverter<TFirst, TOut>(converter));
    }

    public static IFactory<TIn, TOut> Create<TIn, TFirst, TOut>(
      IFactory<TIn, TFirst> factory,
      Func<TFirst, TOut> converter)
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      return ConstructThenConvertFactory.Create<TIn, TFirst, TOut>(factory, (IConverter<TFirst, TOut>) new ByFuncConverter<TFirst, TOut>(converter));
    }

    public static IFactory<TOut> ConvertBy<TFirst, TOut>(
      this IFactory<TFirst> factory,
      IConverter<TFirst, TOut> converter)
    {
      return ConstructThenConvertFactory.Create<TFirst, TOut>(factory, converter);
    }

    public static IFactory<TIn, TOut> ConvertBy<TIn, TFirst, TOut>(
      this IFactory<TIn, TFirst> factory,
      IConverter<TFirst, TOut> converter)
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      return ConstructThenConvertFactory.Create<TIn, TFirst, TOut>(factory, converter);
    }

    public static IFactory<TOut> Create<TFirst, TOut>(
      IFactory<TFirst> factory,
      IConverter<TFirst, TOut> converter)
    {
      return (IFactory<TOut>) new ConstructThenConvertFactory.Impl<TFirst, TOut>(factory, converter);
    }

    public static IFactory<TIn, TOut> Create<TIn, TFirst, TOut>(
      IFactory<TIn, TFirst> factory,
      IConverter<TFirst, TOut> converter)
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      return (IFactory<TIn, TOut>) new ConstructThenConvertFactory.Impl<TIn, TFirst, TOut>(factory, converter);
    }

    private class Impl<TFirst, TOut> : IFactory<TOut>
    {
      private readonly IFactory<TFirst> factory;
      private readonly IConverter<TFirst, TOut> converter;

      public Impl(IFactory<TFirst> factory, IConverter<TFirst, TOut> converter)
      {
        this.factory = factory;
        this.converter = converter;
      }

      public TOut Get() => this.converter.Convert(this.factory.Get());
    }

    private class Impl<TIn, TFirst, TOut> : IFactory<TIn, TOut>
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      private readonly IFactory<TIn, TFirst> factory;
      private readonly IConverter<TFirst, TOut> converter;

      public Impl(IFactory<TIn, TFirst> factory, IConverter<TFirst, TOut> converter)
      {
        this.factory = factory;
        this.converter = converter;
      }

      public TOut Get(TIn input) => this.converter.Convert(this.factory.Get(input));
    }
  }
}
