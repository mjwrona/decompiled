// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ConvertThenDelegateHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class ConvertThenDelegateHandler
  {
    public static IAsyncHandler<TIn, TOut> Create<TIn, TConverted, TOut>(
      IConverter<TIn, TConverted> converter,
      IAsyncHandler<TConverted, TOut> handler)
    {
      return (IAsyncHandler<TIn, TOut>) new ConvertThenDelegateHandler.Impl<TIn, TConverted, TOut>(converter, handler);
    }

    public static IAsyncHandler<TIn, TOut> ThenDelegateTo<TIn, TConverted, TOut>(
      this IConverter<TIn, TConverted> converter,
      IAsyncHandler<TConverted, TOut> handler)
    {
      return ConvertThenDelegateHandler.Create<TIn, TConverted, TOut>(converter, handler);
    }

    public static IAsyncHandler<TIn, TOut> ThenDelegateTo<TIn, TConverted, TOut>(
      this IConverter<TIn, TConverted> converter,
      IConverter<TConverted, TOut> converter2)
    {
      return ConvertThenDelegateHandler.Create<TIn, TConverted, TOut>(converter, (IAsyncHandler<TConverted, TOut>) new ConverterAsAsyncHandler<TConverted, TOut>(converter2));
    }

    private class Impl<TIn, TConverted, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IConverter<TIn, TConverted> converter;
      private readonly IAsyncHandler<TConverted, TOut> handler;

      public Impl(IConverter<TIn, TConverted> converter, IAsyncHandler<TConverted, TOut> handler)
      {
        this.converter = converter;
        this.handler = handler;
      }

      public Task<TOut> Handle(TIn request) => this.handler.Handle(this.converter.Convert(request));
    }
  }
}
