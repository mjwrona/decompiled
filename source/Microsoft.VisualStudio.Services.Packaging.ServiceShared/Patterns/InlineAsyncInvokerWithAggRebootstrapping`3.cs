// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.InlineAsyncInvokerWithAggRebootstrapping`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public class InlineAsyncInvokerWithAggRebootstrapping<TRequest, TResult, TBootstrapped1> : 
    IAsyncInvokerWithAggRebootstrapping<TRequest, TResult, TBootstrapped1>
    where TRequest : class, IFeedRequest
    where TBootstrapped1 : class
  {
    private readonly IVssRequestContext requestContext;

    public InlineAsyncInvokerWithAggRebootstrapping(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public async Task<TResult> RunAsync(
      string actionName,
      IPendingAggBootstrapper<TBootstrapped1> bootstrapper,
      TRequest request,
      CancellationToken cancellationToken,
      Func<TBootstrapped1, TRequest, CancellationToken, Task<TResult>> func)
    {
      Func<TBootstrapped1, TRequest, CancellationToken, Task<TResult>> func1 = func;
      TResult result = await func1(await bootstrapper.Bootstrap(this.requestContext, (IFeedRequest) request), request, cancellationToken);
      func1 = (Func<TBootstrapped1, TRequest, CancellationToken, Task<TResult>>) null;
      return result;
    }
  }
}
