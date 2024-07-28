// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces.IAsyncInvokerWithAggRebootstrapping`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces
{
  public interface IAsyncInvokerWithAggRebootstrapping<TRequest, TResult, TBootstrapped1>
    where TRequest : class, IFeedRequest
    where TBootstrapped1 : class
  {
    Task<TResult> RunAsync(
      string actionName,
      IPendingAggBootstrapper<TBootstrapped1> bootstrapper,
      TRequest request,
      CancellationToken cancellationToken,
      Func<TBootstrapped1, TRequest, CancellationToken, Task<TResult>> func);
  }
}
