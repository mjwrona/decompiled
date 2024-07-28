// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.ConsolidatingAsyncInvokerWithAggRebootstrappingDecorator`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public class ConsolidatingAsyncInvokerWithAggRebootstrappingDecorator<TRequest, TResult, TBootstrapped1> : 
    IAsyncInvokerWithAggRebootstrapping<TRequest, TResult, TBootstrapped1>
    where TRequest : class, IFeedRequest, IEquatable<TRequest>
    where TBootstrapped1 : class
  {
    private readonly IConcurrencyConsolidator<TRequest, TResult> concurrencyConsolidator;
    private readonly IAsyncInvokerWithAggRebootstrapping<TRequest, TResult, TBootstrapped1> innerInvoker;

    public ConsolidatingAsyncInvokerWithAggRebootstrappingDecorator(
      IAsyncInvokerWithAggRebootstrapping<TRequest, TResult, TBootstrapped1> innerInvoker,
      IConcurrencyConsolidator<TRequest, TResult> concurrencyConsolidator)
    {
      this.concurrencyConsolidator = concurrencyConsolidator;
      this.innerInvoker = innerInvoker;
    }

    public async Task<TResult> RunAsync(
      string actionName,
      IPendingAggBootstrapper<TBootstrapped1> bootstrapper,
      TRequest request,
      CancellationToken cancellationToken,
      Func<TBootstrapped1, TRequest, CancellationToken, Task<TResult>> func)
    {
      // ISSUE: variable of a compiler-generated type
      ConsolidatingAsyncInvokerWithAggRebootstrappingDecorator<TRequest, TResult, TBootstrapped1>.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30;
      // ISSUE: reference to a compiler-generated field
      return await this.concurrencyConsolidator.RunOnceAsync(request, (Func<Task<TResult>>) (async () => await cDisplayClass30.\u003C\u003E4__this.innerInvoker.RunAsync(actionName, bootstrapper, request, cancellationToken, func)));
    }
  }
}
