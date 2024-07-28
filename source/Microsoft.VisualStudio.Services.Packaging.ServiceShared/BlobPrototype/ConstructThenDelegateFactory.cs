// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ConstructThenDelegateFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class ConstructThenDelegateFactory
  {
    public static IFactory<TIn, TOut> ThenDelegateTo<TIn, TFirst, TOut>(
      this IFactory<TIn, TFirst> first,
      IFactory<TFirst, TOut> second)
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      return ConstructThenDelegateFactory.Create<TIn, TFirst, TOut>(first, second);
    }

    public static IAsyncHandler<TIn, TOut> ThenDelegateTo<TIn, TFirst, TOut>(
      this IFactory<TIn, IAsyncHandler<TIn, TFirst>> first,
      IAsyncHandler<TFirst, TOut> second)
    {
      return (IAsyncHandler<TIn, TOut>) new ConstructThenDelegateFactory.DeletateToHandlerImpl<TIn, TFirst, TOut>(first, second);
    }

    public static IFactory<TIn, TOut> Create<TIn, TFirst, TOut>(
      IFactory<TIn, TFirst> first,
      IFactory<TFirst, TOut> second)
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      return (IFactory<TIn, TOut>) new ConstructThenDelegateFactory.DelegateToFactoryImpl<TIn, TFirst, TOut>(first, second);
    }

    private class DelegateToFactoryImpl<TIn, TFirst, TOut> : IFactory<TIn, TOut>
      where TIn : class
      where TFirst : class
      where TOut : class
    {
      private readonly IFactory<TIn, TFirst> first;
      private readonly IFactory<TFirst, TOut> second;

      public DelegateToFactoryImpl(IFactory<TIn, TFirst> first, IFactory<TFirst, TOut> second)
      {
        this.first = first;
        this.second = second;
      }

      public TOut Get(TIn input) => this.second.Get(this.first.Get(input));
    }

    private class DeletateToHandlerImpl<TIn, TFirst, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IFactory<TIn, IAsyncHandler<TIn, TFirst>> factory;
      private readonly IAsyncHandler<TFirst, TOut> handler;

      public DeletateToHandlerImpl(
        IFactory<TIn, IAsyncHandler<TIn, TFirst>> factory,
        IAsyncHandler<TFirst, TOut> handler)
      {
        this.factory = factory;
        this.handler = handler;
      }

      public async Task<TOut> Handle(TIn request) => await this.handler.Handle(await this.factory.Get(request).Handle(request));
    }
  }
}
