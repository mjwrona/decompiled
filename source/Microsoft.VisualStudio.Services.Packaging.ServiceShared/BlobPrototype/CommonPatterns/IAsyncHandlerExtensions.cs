// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns.IAsyncHandlerExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns
{
  public static class IAsyncHandlerExtensions
  {
    public static IAsyncHandler<TIn, TOut> AsIAsyncHandler<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> that)
    {
      return that;
    }

    public static IAsyncHandler<TIn, TOut> ThenDelegateTo<TIn, TIntermediate, TOut>(
      this IAsyncHandler<TIn, TIntermediate> currentHandler,
      IAsyncHandler<TIntermediate, TOut> handler)
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.IntermediateDelegationImpl<TIn, TIntermediate, TOut>(currentHandler, handler);
    }

    public static IAsyncHandler<TIn> ThenDelegateTo<TIn, TIntermediate>(
      this IAsyncHandler<TIn, TIntermediate> currentHandler,
      IAsyncHandler<TIntermediate> handler)
    {
      return (IAsyncHandler<TIn>) new IAsyncHandlerExtensions.IntermediateDelegationNullResultImpl<TIn, TIntermediate>(currentHandler, handler);
    }

    public static IAsyncHandler<TIn, TOut> ThenDelegateTo<TIn, THandled, TOut>(
      this IAsyncHandler<TIn, THandled> currentHandler,
      IConverter<THandled, TOut> handler)
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.HandleThenConvertHandlerImpl<TIn, THandled, TOut>(currentHandler, handler);
    }

    public static IAsyncHandler<TIn, TOut> ThenForwardResultTo<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> currentHandler,
      IAsyncHandler<TOut> forwardingToThisHandler)
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.ResultForwardingImpl<TIn, TOut>(currentHandler, forwardingToThisHandler);
    }

    public static IAsyncHandler<TIn, TOut> ThenForwardOriginalRequestAndResultTo<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> currentHandler,
      IAsyncHandler<(TIn, TOut)> forwardingToThisHandler)
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.RequestAndResultForwardingImpl<TIn, TOut>(currentHandler, forwardingToThisHandler);
    }

    public static IAsyncHandler<TIn, TOut> ThenForwardOriginalRequestTo<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> currentHandler,
      IAsyncHandler<TIn, NullResult> forwardingToThisHandler)
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.RequestForwardingImpl<TIn, TOut>(currentHandler, forwardingToThisHandler);
    }

    public static IAsyncHandler<TIn, TOut> ThenActuallyHandleWith<TIn, TAny, TOut>(
      this IAsyncHandler<TIn, TAny> currentHandler,
      IAsyncHandler<TIn, TOut> handler)
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.ThenActuallyHandleWithImpl<TIn, TAny, TOut>(currentHandler, handler);
    }

    public static IAsyncHandler<TIn> ThenReturnNullResult<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> handler)
    {
      return (IAsyncHandler<TIn>) new IAsyncHandlerExtensions.ResultAlwaysNullResultImpl<TIn, TOut>(handler);
    }

    public static IAsyncHandler<TIn, TOut> CatchHandlerException<TIn, TOut, TEx>(
      this IAsyncHandler<TIn, TOut> currentHandler,
      Func<TEx, bool> handleException,
      IAsyncHandler<TIn, TOut> exceptionHandler)
      where TEx : Exception
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.CatchHandlerExceptionImpl<TIn, TOut, TEx>(currentHandler, handleException, exceptionHandler);
    }

    public static IAsyncHandler<TIn, TOut> ValidateResultWith<TIn, TOut, TValidate>(
      this IAsyncHandler<TIn, TOut> handler,
      IValidator<TValidate> validator)
      where TOut : TValidate
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.ResultValidatingHandler<TIn, TOut, TValidate>(handler, validator);
    }

    public static IAsyncHandler<TIn, (TIn, TOut)> KeepInput<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> handler)
    {
      return (IAsyncHandler<TIn, (TIn, TOut)>) new ByAsyncFuncAsyncHandler<TIn, (TIn, TOut)>((Func<TIn, Task<(TIn, TOut)>>) (async request =>
      {
        TIn @in = request;
        return (@in, await handler.Handle(request));
      }));
    }

    public static IAsyncHandler<TIn, TCombined> KeepInput<TIn, TOut, TCombined>(
      this IAsyncHandler<TIn, TOut> handler,
      Func<TIn, TOut, TCombined> transform)
    {
      return (IAsyncHandler<TIn, TCombined>) new ByAsyncFuncAsyncHandler<TIn, TCombined>((Func<TIn, Task<TCombined>>) (async request =>
      {
        Func<TIn, TOut, TCombined> func = transform;
        TIn @in = request;
        return func(@in, await handler.Handle(request));
      }));
    }

    public static IAsyncHandler<TBootstrapperIn, THandlerOut> CastToMatchInputTypeOf<TBootstrapperIn, THandlerIn, THandlerOut>(
      this IAsyncHandler<THandlerIn, THandlerOut> handler,
      IBootstrapper<IHaveInputType<TBootstrapperIn>> bootstrapper)
      where TBootstrapperIn : class, THandlerIn
    {
      return (IAsyncHandler<TBootstrapperIn, THandlerOut>) handler;
    }

    public static IAsyncHandler<TOtherIn, TOut> CastToMatchInputTypeOf<TOtherIn, TIn, TOut>(
      this IAsyncHandler<TIn, TOut> handler,
      IHaveInputType<TOtherIn> other)
      where TOtherIn : class, TIn
    {
      return (IAsyncHandler<TOtherIn, TOut>) handler;
    }

    public static IAsyncHandler<TIn, TOut> SkipAndReturnNullIf<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> handler,
      Func<TIn, bool> predicate)
    {
      return (IAsyncHandler<TIn, TOut>) new IAsyncHandlerExtensions.SkipAndReturnNullIfHandler<TIn, TOut>(handler, predicate);
    }

    public static IAsyncHandler<TIn, TOut> SkipAndReturnNullIfFeatureFlagOff<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> handler,
      IFeatureFlagService ffService,
      string flag)
    {
      return (IAsyncHandler<TIn, TOut>) new SkipAndReturnNullIfFeatureFlagOffHandler<TIn, TOut>(ffService, flag, handler);
    }

    public static IFactory<TIn, Task<TOut>> AsFactory<TIn, TOut>(
      this IAsyncHandler<TIn, TOut> handler)
    {
      return (IFactory<TIn, Task<TOut>>) new IAsyncHandlerExtensions.HandlerAsFactoryFactory<TIn, TOut>(handler);
    }

    private class ResultValidatingHandler<TIn, TOut, TValidate> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
      where TOut : TValidate
    {
      private readonly IAsyncHandler<TIn, TOut> handler;
      private readonly IValidator<TValidate> validator;

      public ResultValidatingHandler(
        IAsyncHandler<TIn, TOut> handler,
        IValidator<TValidate> validator)
      {
        this.handler = handler;
        this.validator = validator;
      }

      public async Task<TOut> Handle(TIn request)
      {
        TOut valueToValidate = await this.handler.Handle(request);
        this.validator.Validate((TValidate) valueToValidate);
        return valueToValidate;
      }
    }

    private class ResultAlwaysNullResultImpl<TIn, TOut> : 
      IAsyncHandler<TIn>,
      IAsyncHandler<TIn, NullResult>,
      IHaveInputType<TIn>,
      IHaveOutputType<NullResult>
    {
      private readonly IAsyncHandler<TIn, TOut> handler;

      public ResultAlwaysNullResultImpl(IAsyncHandler<TIn, TOut> handler) => this.handler = handler;

      public async Task<NullResult> Handle(TIn request)
      {
        TOut @out = await this.handler.Handle(request);
        NullResult nullResult;
        return nullResult;
      }
    }

    private class IntermediateDelegationImpl<TIn, TIntermediate, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IAsyncHandler<TIn, TIntermediate> firstHandler;
      private readonly IAsyncHandler<TIntermediate, TOut> secondHandler;

      public IntermediateDelegationImpl(
        IAsyncHandler<TIn, TIntermediate> firstHandler,
        IAsyncHandler<TIntermediate, TOut> secondHandler)
      {
        this.firstHandler = firstHandler;
        this.secondHandler = secondHandler;
      }

      public async Task<TOut> Handle(TIn request) => await this.secondHandler.Handle(await this.firstHandler.Handle(request));
    }

    private class IntermediateDelegationNullResultImpl<TIn, TIntermediate> : 
      IAsyncHandler<TIn>,
      IAsyncHandler<TIn, NullResult>,
      IHaveInputType<TIn>,
      IHaveOutputType<NullResult>
    {
      private readonly IAsyncHandler<TIn, TIntermediate> firstHandler;
      private readonly IAsyncHandler<TIntermediate> secondHandler;

      public IntermediateDelegationNullResultImpl(
        IAsyncHandler<TIn, TIntermediate> firstHandler,
        IAsyncHandler<TIntermediate> secondHandler)
      {
        this.firstHandler = firstHandler;
        this.secondHandler = secondHandler;
      }

      public async Task<NullResult> Handle(TIn request) => await this.secondHandler.Handle(await this.firstHandler.Handle(request));
    }

    private class ResultForwardingImpl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IAsyncHandler<TIn, TOut> currentHandler;
      private readonly IAsyncHandler<TOut> forwardingToThisHandler;

      public ResultForwardingImpl(
        IAsyncHandler<TIn, TOut> currentHandler,
        IAsyncHandler<TOut> forwardingToThisHandler)
      {
        this.currentHandler = currentHandler;
        this.forwardingToThisHandler = forwardingToThisHandler;
      }

      public async Task<TOut> Handle(TIn request)
      {
        TOut result = await this.currentHandler.Handle(request);
        NullResult nullResult = await this.forwardingToThisHandler.Handle(result);
        TOut @out = result;
        result = default (TOut);
        return @out;
      }
    }

    private class RequestForwardingImpl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IAsyncHandler<TIn, TOut> currentHandler;
      private readonly IAsyncHandler<TIn, NullResult> forwardingToThisHandler;

      public RequestForwardingImpl(
        IAsyncHandler<TIn, TOut> currentHandler,
        IAsyncHandler<TIn, NullResult> forwardingToThisHandler)
      {
        this.currentHandler = currentHandler;
        this.forwardingToThisHandler = forwardingToThisHandler;
      }

      public async Task<TOut> Handle(TIn request)
      {
        TOut result = await this.currentHandler.Handle(request);
        NullResult nullResult = await this.forwardingToThisHandler.Handle(request);
        TOut @out = result;
        result = default (TOut);
        return @out;
      }
    }

    private class RequestAndResultForwardingImpl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IAsyncHandler<TIn, TOut> currentHandler;
      private readonly IAsyncHandler<(TIn, TOut)> forwardingToThisHandler;

      public RequestAndResultForwardingImpl(
        IAsyncHandler<TIn, TOut> currentHandler,
        IAsyncHandler<(TIn, TOut)> forwardingToThisHandler)
      {
        this.currentHandler = currentHandler;
        this.forwardingToThisHandler = forwardingToThisHandler;
      }

      public async Task<TOut> Handle(TIn request)
      {
        TOut result = await this.currentHandler.Handle(request);
        NullResult nullResult = await this.forwardingToThisHandler.Handle((request, result));
        TOut @out = result;
        result = default (TOut);
        return @out;
      }
    }

    private class ThenActuallyHandleWithImpl<TIn, TAny, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IAsyncHandler<TIn, TAny> firstHandler;
      private readonly IAsyncHandler<TIn, TOut> secondHandler;

      public ThenActuallyHandleWithImpl(
        IAsyncHandler<TIn, TAny> firstHandler,
        IAsyncHandler<TIn, TOut> secondHandler)
      {
        this.firstHandler = firstHandler;
        this.secondHandler = secondHandler;
      }

      public async Task<TOut> Handle(TIn request)
      {
        TAny any = await this.firstHandler.Handle(request);
        return await this.secondHandler.Handle(request);
      }
    }

    private class CatchHandlerExceptionImpl<TIn, TOut, TEx> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
      where TEx : Exception
    {
      private readonly Func<TEx, bool> handleException;
      private readonly IAsyncHandler<TIn, TOut> handler;
      private readonly IAsyncHandler<TIn, TOut> exceptionHandler;

      public CatchHandlerExceptionImpl(
        IAsyncHandler<TIn, TOut> handler,
        Func<TEx, bool> handleException,
        IAsyncHandler<TIn, TOut> exceptionHandler)
      {
        this.handler = handler;
        this.handleException = handleException;
        this.exceptionHandler = exceptionHandler;
      }

      public async Task<TOut> Handle(TIn request)
      {
        int num;
        try
        {
          return await this.handler.Handle(request);
        }
        catch (TEx ex)
        {
          num = 1;
        }
        TOut @out;
        if (num != 1)
          return @out;
        if (this.handleException(ex))
          return await this.exceptionHandler.Handle(request);
        if (!((object) ex is Exception source))
          throw (object) ex;
        ExceptionDispatchInfo.Capture(source).Throw();
        return @out;
      }
    }

    private class HandleThenConvertHandlerImpl<TIn, THandled, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private IAsyncHandler<TIn, THandled> handler;
      private IConverter<THandled, TOut> converter;

      public HandleThenConvertHandlerImpl(
        IAsyncHandler<TIn, THandled> handler,
        IConverter<THandled, TOut> converter)
      {
        this.handler = handler;
        this.converter = converter;
      }

      public async Task<TOut> Handle(TIn request)
      {
        IConverter<THandled, TOut> converter = this.converter;
        return converter.Convert(await this.handler.Handle(request));
      }
    }

    private class SkipAndReturnNullIfHandler<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IAsyncHandler<TIn, TOut> handler;
      private readonly Func<TIn, bool> predicate;

      public SkipAndReturnNullIfHandler(IAsyncHandler<TIn, TOut> handler, Func<TIn, bool> predicate)
      {
        this.handler = handler;
        this.predicate = predicate;
      }

      public async Task<TOut> Handle(TIn request) => this.predicate(request) ? default (TOut) : await this.handler.Handle(request);
    }

    private class HandlerAsFactoryFactory<TIn, TOut> : IFactory<TIn, Task<TOut>>
    {
      private readonly IAsyncHandler<TIn, TOut> handler;

      public HandlerAsFactoryFactory(IAsyncHandler<TIn, TOut> handler) => this.handler = handler;

      public async Task<TOut> Get(TIn input) => await this.handler.Handle(input);
    }
  }
}
