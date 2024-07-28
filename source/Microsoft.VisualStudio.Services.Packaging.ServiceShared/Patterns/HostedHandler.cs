// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.HostedHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public static class HostedHandler
  {
    public static IAsyncHandler<TIn, TOut> Create<TIn, TOut>(
      IExecutionEnvironment executionEnvironment,
      Func<IAsyncHandler<TIn, TOut>> handlerFuncForHosted)
    {
      return (IAsyncHandler<TIn, TOut>) new HostedHandler.Impl<TIn, TOut>(executionEnvironment, handlerFuncForHosted);
    }

    public static IAsyncHandler<TIn, TOut> Create<TIn, TOut>(
      IExecutionEnvironment executionEnvironment,
      IAsyncHandler<TIn, TOut> handlerForHosted)
    {
      return (IAsyncHandler<TIn, TOut>) new HostedHandler.Impl<TIn, TOut>(executionEnvironment, handlerForHosted);
    }

    private class Impl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IExecutionEnvironment executionEnvironment;
      private readonly Func<IAsyncHandler<TIn, TOut>> handlerFuncForHosted;

      public Impl(
        IExecutionEnvironment executionEnvironment,
        Func<IAsyncHandler<TIn, TOut>> handlerFuncForHosted)
      {
        this.executionEnvironment = executionEnvironment;
        this.handlerFuncForHosted = handlerFuncForHosted;
      }

      public Impl(
        IExecutionEnvironment executionEnvironment,
        IAsyncHandler<TIn, TOut> handlerForHosted)
      {
        this.executionEnvironment = executionEnvironment;
        this.handlerFuncForHosted = (Func<IAsyncHandler<TIn, TOut>>) (() => handlerForHosted);
      }

      public Task<TOut> Handle(TIn request) => this.executionEnvironment.IsHosted() ? this.handlerFuncForHosted().Handle(request) : Task.FromResult<TOut>(default (TOut));
    }
  }
}
