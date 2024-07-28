// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.OnPremHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public static class OnPremHandler
  {
    public static IAsyncHandler<TIn, TOut> Create<TIn, TOut>(
      IExecutionEnvironment executionEnvironment,
      Func<IAsyncHandler<TIn, TOut>> handlerFuncForOnPrem)
    {
      return (IAsyncHandler<TIn, TOut>) new OnPremHandler.Impl<TIn, TOut>(executionEnvironment, handlerFuncForOnPrem);
    }

    public static IAsyncHandler<TIn, TOut> Create<TIn, TOut>(
      IExecutionEnvironment executionEnvironment,
      IAsyncHandler<TIn, TOut> handlerForOnPrem)
    {
      return (IAsyncHandler<TIn, TOut>) new OnPremHandler.Impl<TIn, TOut>(executionEnvironment, handlerForOnPrem);
    }

    private class Impl<TIn, TOut> : 
      IAsyncHandler<TIn, TOut>,
      IHaveInputType<TIn>,
      IHaveOutputType<TOut>
    {
      private readonly IExecutionEnvironment executionEnvironment;
      private readonly Func<IAsyncHandler<TIn, TOut>> handlerFuncForOnPrem;

      public Impl(
        IExecutionEnvironment executionEnvironment,
        Func<IAsyncHandler<TIn, TOut>> handlerFuncForOnPrem)
      {
        this.executionEnvironment = executionEnvironment;
        this.handlerFuncForOnPrem = handlerFuncForOnPrem;
      }

      public Impl(
        IExecutionEnvironment executionEnvironment,
        IAsyncHandler<TIn, TOut> handlerForOnPrem)
      {
        this.executionEnvironment = executionEnvironment;
        this.handlerFuncForOnPrem = (Func<IAsyncHandler<TIn, TOut>>) (() => handlerForOnPrem);
      }

      public Task<TOut> Handle(TIn request) => !this.executionEnvironment.IsHosted() ? this.handlerFuncForOnPrem().Handle(request) : Task.FromResult<TOut>(default (TOut));
    }
  }
}
