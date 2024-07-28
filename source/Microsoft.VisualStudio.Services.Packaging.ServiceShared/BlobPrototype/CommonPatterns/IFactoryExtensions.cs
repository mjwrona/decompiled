// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns.IFactoryExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns
{
  public static class IFactoryExtensions
  {
    public static 
    #nullable disable
    IAsyncHandler<TIn, TOut> ThenForwardOriginalRequestTo<TIn, TOut>(
      this IFactory<TIn, IAsyncHandler<TIn, TOut>> factory,
      IAsyncHandler<TIn> forwardedToHandler)
      where TIn : class
      where TOut : class
    {
      return (IAsyncHandler<TIn, TOut>) new IFactoryExtensions.RequestForwardingImpl<TIn, TOut>(factory, forwardedToHandler);
    }

    public static IFactory<TIn, TOut> SingleElementCache<TIn, TOut>(
      this IFactory<TIn, TOut> factory,
      IEqualityComparer<TIn> inputComparer = null)
    {
      return factory.SingleElementCache<TIn, TIn, TOut>((Func<TIn, TIn>) (x => x), inputComparer);
    }

    public static IFactory<TIn, TOut> SingleElementCache<TIn, TKey, TOut>(
      this IFactory<TIn, TOut> factory,
      Func<TIn, TKey> keySelector,
      IEqualityComparer<TKey> keyComparer = null)
    {
      return (IFactory<TIn, TOut>) new IFactoryExtensions.SingleElementCacheImpl<TIn, TKey, TOut>(factory, keySelector, keyComparer);
    }

    public static IFactory<TOut> ExecuteOnceAndKeepReturningSameResult<TOut>(
      this IFactory<TOut> factory)
    {
      return (IFactory<TOut>) new IFactoryExtensions.ExecuteOnceAndKeepReturningSameResultImpl<TOut>(factory);
    }

    private class RequestForwardingImpl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
      where TIn : class
      where TOut : class
    {
      private readonly IFactory<TIn, IAsyncHandler<TIn, TOut>> currentHandler;
      private readonly IAsyncHandler<TIn> forwardingToThisHandler;

      public RequestForwardingImpl(
        IFactory<TIn, IAsyncHandler<TIn, TOut>> currentHandler,
        IAsyncHandler<TIn> forwardingToThisHandler)
      {
        this.currentHandler = currentHandler;
        this.forwardingToThisHandler = forwardingToThisHandler;
      }

      public async Task<TOut> Handle(TIn request)
      {
        TOut result = await this.currentHandler.Get(request).Handle(request);
        NullResult nullResult = await this.forwardingToThisHandler.Handle(request);
        TOut @out = result;
        result = default (TOut);
        return @out;
      }
    }

    private class SingleElementCacheImpl<TIn, TKey, TOut> : IFactory<TIn, TOut>
    {
      private readonly IFactory<TIn, TOut> factory;
      private readonly IEqualityComparer<TKey> keyComparer;
      private readonly Func<TIn, TKey> keySelector;
      private volatile IFactoryExtensions.SingleElementCacheImpl<TIn, TKey, TOut>.State state;

      public SingleElementCacheImpl(
        IFactory<TIn, TOut> factory,
        Func<TIn, TKey> keySelector,
        IEqualityComparer<TKey> keyComparer)
      {
        this.factory = factory;
        this.keySelector = keySelector;
        this.keyComparer = keyComparer ?? (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default;
      }

      public TOut Get(TIn input)
      {
        IFactoryExtensions.SingleElementCacheImpl<TIn, TKey, TOut>.State state1 = this.state;
        TKey key = this.keySelector(input);
        if (state1 != (IFactoryExtensions.SingleElementCacheImpl<TIn, TKey, TOut>.State) null && this.keyComparer.Equals(key, state1.Key))
          return state1.Output;
        IFactoryExtensions.SingleElementCacheImpl<TIn, TKey, TOut>.State state2 = new IFactoryExtensions.SingleElementCacheImpl<TIn, TKey, TOut>.State(key, this.factory.Get(input));
        Interlocked.MemoryBarrier();
        this.state = state2;
        return state2.Output;
      }

      private record State(TKey Key, TOut Output);
    }

    private class ExecuteOnceAndKeepReturningSameResultImpl<TOut> : IFactory<TOut>
    {
      private readonly Lazy<TOut> value;

      public ExecuteOnceAndKeepReturningSameResultImpl(IFactory<TOut> factory) => this.value = new Lazy<TOut>(new Func<TOut>(factory.Get));

      public TOut Get() => this.value.Value;
    }
  }
}
