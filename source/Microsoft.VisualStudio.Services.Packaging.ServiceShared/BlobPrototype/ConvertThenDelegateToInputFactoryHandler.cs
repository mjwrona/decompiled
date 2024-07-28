// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ConvertThenDelegateToInputFactoryHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class ConvertThenDelegateToInputFactoryHandler
  {
    public static IAsyncHandler<TIn, TOut> ThenDelegateTo<TIn, TConverted, TOut>(
      this IConverter<TIn, TConverted> converter,
      IFactory<TConverted, IAsyncHandler<TConverted, TOut>> factory)
      where TIn : class
      where TConverted : class
      where TOut : class
    {
      return ConvertThenDelegateToInputFactoryHandler.Create<TIn, TConverted, TOut>(converter, factory);
    }

    public static IAsyncHandler<TIn, TOut> Create<TIn, TConverted, TOut>(
      IConverter<TIn, TConverted> converter,
      IFactory<TConverted, IAsyncHandler<TConverted, TOut>> factory)
      where TIn : class
      where TConverted : class
      where TOut : class
    {
      return (IAsyncHandler<TIn, TOut>) new ConvertThenDelegateToInputFactoryHandler.Impl<TIn, TConverted, TOut>(converter, factory);
    }

    private class Impl<TIn, TInConverted, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
      where TIn : class
      where TInConverted : class
    {
      private readonly IConverter<TIn, TInConverted> converter;
      private readonly IFactory<TInConverted, IAsyncHandler<TInConverted, TOut>> factory;

      public Impl(
        IConverter<TIn, TInConverted> converter,
        IFactory<TInConverted, IAsyncHandler<TInConverted, TOut>> factory)
      {
        this.converter = converter;
        this.factory = factory;
      }

      public Task<TOut> Handle(TIn request)
      {
        TInConverted inConverted = this.converter.Convert(request);
        return this.factory.Get(inConverted).Handle(inConverted);
      }
    }
  }
}
