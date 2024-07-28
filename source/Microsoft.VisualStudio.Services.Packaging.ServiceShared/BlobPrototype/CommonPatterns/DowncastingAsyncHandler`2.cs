// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns.DowncastingAsyncHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns
{
  public static class DowncastingAsyncHandler<TIn, TOut>
  {
    public static DowncastingAsyncHandler<TIn, TOut>.Builder CreateBuilder() => new DowncastingAsyncHandler<TIn, TOut>.Builder();

    public class Builder
    {
      private readonly List<DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder> handlers = new List<DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder>();

      public DowncastingAsyncHandler<TIn, TOut>.Builder.WhenTypeIsCarrier<TDerived> WhenTypeIs<TDerived>() where TDerived : class, TIn
      {
        Type thisDerivedType = typeof (TDerived);
        DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder handlerHolder = this.handlers.FirstOrDefault<DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder>((Func<DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder, bool>) (handler => handler.DerivedType.IsAssignableFrom(thisDerivedType)));
        if (handlerHolder != null)
          throw new HandlerForTypeWouldBeHiddenException(thisDerivedType, handlerHolder.DerivedType);
        return new DowncastingAsyncHandler<TIn, TOut>.Builder.WhenTypeIsCarrier<TDerived>(this);
      }

      public IAsyncHandler<TIn, TOut> Build() => (IAsyncHandler<TIn, TOut>) new DowncastingAsyncHandler<TIn, TOut>.Impl(this.handlers);

      public class WhenTypeIsCarrier<TDerived> where TDerived : class, TIn
      {
        private readonly DowncastingAsyncHandler<TIn, TOut>.Builder builder;

        public WhenTypeIsCarrier(DowncastingAsyncHandler<TIn, TOut>.Builder builder) => this.builder = builder;

        public DowncastingAsyncHandler<TIn, TOut>.Builder Use(IAsyncHandler<TDerived, TOut> handler)
        {
          this.builder.handlers.Add((DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder) new DowncastingAsyncHandler<TIn, TOut>.Builder.HandlerHolder<TDerived>(handler));
          return this.builder;
        }

        public DowncastingAsyncHandler<TIn, TOut>.Builder UseValue(TOut value) => this.Use((IAsyncHandler<TDerived, TOut>) ReturnSameInstanceAsyncHandler<TIn>.Create<TOut>(value));
      }

      private class HandlerHolder<TDerived> : DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder where TDerived : class, TIn
      {
        private readonly IAsyncHandler<TDerived, TOut> innerHandler;

        public HandlerHolder(IAsyncHandler<TDerived, TOut> innerHandler) => this.innerHandler = innerHandler;

        public Type DerivedType => typeof (TDerived);

        public Task<TOut> HandleAsync(TIn request, out bool didExecute)
        {
          if (!(request is TDerived request1))
          {
            didExecute = false;
            return Task.FromResult<TOut>(default (TOut));
          }
          didExecute = true;
          return this.innerHandler.Handle(request1);
        }
      }
    }

    private class Impl : IAsyncHandler<TIn, TOut>, IHaveInputType<TIn>, IHaveOutputType<TOut>
    {
      private readonly List<DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder> handlers;

      public Impl(
        List<DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder> handlers)
      {
        this.handlers = handlers;
      }

      public Task<TOut> Handle(TIn request)
      {
        foreach (DowncastingAsyncHandler<TIn, TOut>.IHandlerHolder handler in this.handlers)
        {
          bool didExecute;
          Task<TOut> task = handler.HandleAsync(request, out didExecute);
          if (didExecute)
            return task;
        }
        throw new NoMatchingHandlerForTypeException(request.GetType());
      }
    }

    private interface IHandlerHolder
    {
      Type DerivedType { get; }

      Task<TOut> HandleAsync(TIn request, out bool didExecute);
    }
  }
}
