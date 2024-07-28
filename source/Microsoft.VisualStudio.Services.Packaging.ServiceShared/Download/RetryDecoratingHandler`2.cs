// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.RetryDecoratingHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class RetryDecoratingHandler<TIn, TOut> : 
    IAsyncHandler<TIn, TOut>,
    IHaveInputType<TIn>,
    IHaveOutputType<TOut>
  {
    private readonly IAsyncHandler<TIn, TOut> innerHandler;
    private readonly RetryHelper retryHelper;
    private readonly Func<TOut, bool> retryOnResultPredicate;

    public RetryDecoratingHandler(IAsyncHandler<TIn, TOut> innerHandler, RetryHelper retryHelper)
      : this(innerHandler, retryHelper, (Func<TOut, bool>) null)
    {
    }

    public RetryDecoratingHandler(
      IAsyncHandler<TIn, TOut> innerHandler,
      RetryHelper retryHelper,
      Func<TOut, bool> retryOnResultPredicate)
    {
      this.innerHandler = innerHandler;
      this.retryHelper = retryHelper;
      this.retryOnResultPredicate = retryOnResultPredicate;
    }

    public RetryDecoratingHandler<TIn, TOut> WithRetryOnResult(
      Func<TOut, bool> newRetryOnResultPredicate)
    {
      return new RetryDecoratingHandler<TIn, TOut>(this.innerHandler, this.retryHelper, newRetryOnResultPredicate);
    }

    public RetryDecoratingHandler<TIn, TOut> WithRetryOnResultValidatingHandler(
      IHandler<TOut, bool> newRetryOnResultHandler)
    {
      return new RetryDecoratingHandler<TIn, TOut>(this.innerHandler, this.retryHelper, (Func<TOut, bool>) (result => !newRetryOnResultHandler.Handle(result)));
    }

    public async Task<TOut> Handle(TIn request) => await this.retryHelper.Invoke<TOut>((Func<Task<TOut>>) (() => this.\u003C\u003E4__this.innerHandler.Handle(request)), this.retryOnResultPredicate);
  }
}
